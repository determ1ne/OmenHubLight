using System;
using System.Diagnostics;
using System.Windows.Forms;
using Hp.Ohl.SysInfoCommon;

namespace OmenHubLight.Forms
{
    public partial class FormSysInfo : Form
    {
        public FormSysInfo()
        {
            InitializeComponent();
        }

        private void SetGridRows(object[][] values)
        {
            infoGrid.Rows.Clear();
            foreach (var pair in values)
            {
                infoGrid.Rows.Add(pair);
            }
        }

        private void SystemInformationForm_Load(object sender, EventArgs e)
        {
            infoGrid.Rows.Add("Loading...", string.Empty);
            infoLoadWorker.RunWorkerAsync();
        }

        private void infoLoadWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            object[][] value =
            {
                new[] {"Product name (Model)", SystemInfo.Model},
                new[] {"Product number (SKU)", SystemInfo.SystemSkuNumber},
                new[] {"Product Codename", SystemInfo.GetDeviceInternalCode},
                new[] {"Serial number", SystemInfo.BiosSerialNumber},
                new[] {"Software build ID", SystemInfo.BuildId},
                new[] {"Motherboard ID", SystemInfo.BaseBoardProduct},
                new[] {"BIOS Version", SystemInfo.BiosVersion},
                new[] {"BIOS Internal Version", SystemInfo.BiosVersionInternal},
                new[] {"BIOS Build Date", SystemInfo.BiosReleaseDate?.ToShortDateString()},
                new[] {"BIOS Manufacturer", SystemInfo.BiosManufacturer},
                new[] {"Keyboard (Baseboard) version", SystemInfo.BaseBoardVersion},
                new[] {"Keyboard (Baseboard) SN", SystemInfo.BaseBoardSerialNumber},
                new[] {"Total memory", SystemInfo.TotalPhysicalMemory},
                new[] {"Processor name", SystemInfo.ProcessorName},
                new[] {"System Family", SystemInfo.SystemFamily},
                new[] {"Feature Byte", SystemInfo.FeatureByte},
                new[] {"OMEN Feature", SystemInfo.OmenFeature},
                new[] {"OMEN Background Feature", SystemInfo.OmenBackgroundFeature},
                new[] {"OMEN Product Number", SystemInfo.OmenProductNum},
                new[] {"Is HP Product", SystemInfo.IsHpDevice.ToString()},
                new[] {"Is OMEN Product", SystemInfo.IsOmen.ToString()},
                new[] {"Is Gaming Product", SystemInfo.IsGamingProduct.ToString()},
                new[] {"Is NVIDIA Studio Product ", SystemInfo.IsNvStudio.ToString()},
            };
            e.Result = value;
        }

        private void infoLoadWorker_RunWorkerCompleted(object sender,
            System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            SetGridRows((object[][]) e.Result);
        }

        private void buttonAdvanced_Click(object sender, EventArgs e)
        {
            Process.Start("msinfo32");
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            new FormAbout().ShowDialog(this);
        }
    }
}