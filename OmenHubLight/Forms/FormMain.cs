using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hp.Ohl.SysInfoCommon;
using Hp.Ohl.WmiService;
using Hp.Ohl.WmiService.Models;
using Hp.Omen.OmenCommonLib;
using Hp.Omen.OmenCommonLib.PowerControl.Enum;
using OmenHubLight.Lib;
using OmenHubLight.Properties;

namespace OmenHubLight.Forms
{
    public partial class FormMain : Form
    {
        private bool AppIsExiting = false;
        private object AppExitLock = new();
        private OmenHsaClient hsaClient = new();
        private List<ToolStripMenuItem> menuFanItems;

        private delegate void ShowFormDelegate();

        private delegate void UpdateTemperatureTextDelegate(string cpuText, string gpuText);

        public FormMain()
        {
            InitializeComponent();
            WmiEventWatcher.HpBiosEventArrived += HpBiosEventHandler;
        }

        private void HpBiosEventHandler(object sender, HpBiosEventArgs e)
        {
            if (e.eventPayload is OmenKeyPressedPayload)
            {
                ShowFormDelegate d = OmenKeyPressed;
                Invoke(d);
            }
        }

        private void OmenKeyPressed()
        {
            if (Visible) Hide();
            else Show();
        }

        private void buttonShowSysInfo_Click(object sender, EventArgs e)
        {
            new FormSysInfo().ShowDialog(this);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !AppIsExiting;
            Hide();
        }

        private void menuItemQuit_Click(object sender, EventArgs e)
        {
            AppIsExiting = true;
            timerUpdateTempInfo.Stop();
            OnAppExit(sender, e);
            Application.Exit();
        }

        private void OnAppExit(object sender, EventArgs e)
        {
            lock (AppExitLock)
                notifyIcon.Visible = false;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Application.ApplicationExit += OnAppExit;

            if (Settings.Default.NotFirstLaunch == false)
            {
                // First Launch
                var closeButton = TaskDialogButton.Close;
                closeButton.Enabled = false;
                closeButton.Visible = false;
                var cancelButton = TaskDialogButton.Cancel;
                cancelButton.Enabled = false;
                cancelButton.Visible = true;
                cancelButton.AllowCloseDialog = false;

                var page = new TaskDialogPage
                {
                    Icon = TaskDialogIcon.Information,
                    AllowCancel = false,
                    DefaultButton = closeButton,
                    AllowMinimize = false,
                    Buttons =
                    {
                        closeButton,
                        cancelButton,
                    },
                    Caption = "Initializing application data",
                    ProgressBar = new TaskDialogProgressBar
                    {
                        State = TaskDialogProgressBarState.Marquee,
                    },
                    Heading = "Preparing for the first launch",
                    Text = "We are gathering essential data for App to run, it may take several seconds.",
                };

                page.Created += async (_, _) =>
                {
                    await Task.Run(() =>
                    {
                        Settings.Default.ComputerModel = SystemInfo.Model;
                        Settings.Default.NotFirstLaunch = true;
                        Settings.Default.AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
                        Settings.Default.Save();
                    });

                    closeButton.PerformClick();
                };

                TaskDialog.ShowDialog(this, page);
            }

            // Initialize Form
            menuFanModeDefault.Tag = PerformanceMode.Default;
            menuFanModePerformance.Tag = PerformanceMode.Performance;
            menuFanModeCool.Tag = PerformanceMode.Cool;
            menuFanModeDefault.Click += fanModeMenuItem_Click;
            menuFanModePerformance.Click += fanModeMenuItem_Click;
            menuFanModeCool.Click += fanModeMenuItem_Click;
            menuFanItems = new List<ToolStripMenuItem> {menuFanModeDefault, menuFanModePerformance, menuFanModeCool};
            if (Settings.Default.PerformanceMode != null)
            {
                var item = menuFanItems
                    .FirstOrDefault(x => (PerformanceMode) x.Tag == Settings.Default.PerformanceMode.Value);
                if (item != null)
                {
                    item.Checked = true;
                }
            }

            labelModel.Text = Settings.Default.ComputerModel;
            UpdateTempInfo();

            timerUpdateTempInfo.Start();
        }

        private void timerUpdateTempInfo_Tick(object sender, EventArgs e)
        {
            lock (AppExitLock)
                UpdateTempInfo();
        }

        private void UpdateTempInfo()
        {
            Task.Run(() =>
            {
                if (AppIsExiting) return;
                foreach (var h in CpuInfo.GetCpuHardwares())
                {
                    h.Update();
                }

                foreach (var h in GpuInfo.GetGpuHardwares())
                {
                    h.Update();
                }

                var cpuTemperatures = CpuInfo.GetCpuHardwares()
                    .Select(x => x.GetTempSensors().Max(x => x.Value))
                    .Max();
                var gpuTemperatures = GpuInfo.GetGpuHardwares()
                    .Select(x => x.GetTempSensors().Max(x => x.Value))
                    .Max();

                UpdateTemperatureTextDelegate d = UpdateTempInfoText;
                Invoke(d, $"CPU Temperature: {cpuTemperatures}℃", $"GPU Temperature: {gpuTemperatures}℃");
            });
        }

        private void UpdateTempInfoText(string cpuText, string gpuText)
        {
            labelCpuTemp.Text = cpuText;
            labelGpuTemp.Text = gpuText;
        }

        private void FormMain_VisibleChanged(object sender, EventArgs e)
        {
            timerUpdateTempInfo.Enabled = Visible;
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
        }

        private void fanModeMenuItem_Click(object sender, EventArgs e)
        {
            var mode = (PerformanceMode) ((ToolStripMenuItem) sender).Tag;
            ChangeFanMode(mode, (ToolStripMenuItem) sender);
        }

        private void ChangeFanMode(PerformanceMode m, ToolStripMenuItem menuItem = null)
        {
            foreach (var item in menuFanItems)
            {
                item.Checked = false;
            }

            menuItem ??= menuFanItems.First(x => (PerformanceMode) x.Tag == m);

            menuItem.Checked = true;

            if (hsaClient.GetThermalPolicyVersion() == ThermalPolicyVersion.V1)
            {
                m = m switch
                {
                    PerformanceMode.Default => PerformanceMode.L2,
                    PerformanceMode.Performance => PerformanceMode.L7,
                    PerformanceMode.Cool => PerformanceMode.L4,
                    _ => m
                };
            }

            Settings.Default.PerformanceMode = m;
            Settings.Default.Save();

            var res = hsaClient.BiosWmiCmd_Set(131080, 26, new[] {byte.MaxValue, (byte) m});
        }

        private void buttonFanMode_Click(object sender, EventArgs e)
        {
            var page = new TaskDialogPage
            {
                Caption = "Change fan mode",
                Heading = "Optimize performance by selecting the mode that suits you best.",
                AllowCancel = true,
                Buttons =
                {
                    new TaskDialogCommandLinkButton("&Default", "Suitable for all types of tasks.")
                    {
                        Tag = PerformanceMode.Default,
                    },
                    new TaskDialogCommandLinkButton("&Performance",
                        "Suitable for gaming. May increase heat and noise levels.")
                    {
                        Tag = PerformanceMode.Performance,
                    },
                    new TaskDialogCommandLinkButton("&Comfort",
                        "Suitable for casual tasks. Lowers CPU and GPU temperatures.")
                    {
                        Tag = PerformanceMode.Cool,
                    },
                },
                Icon = TaskDialogIcon.Information,
            };

            var result = TaskDialog.ShowDialog(this, page);
            if (result.Tag is PerformanceMode m)
            {
                ChangeFanMode(m);
            }
        }

        private void buttonKeyboardLight_Click(object sender, EventArgs e)
        {
            new FormSetFourZone().ShowDialog(this);
        }
    }
}