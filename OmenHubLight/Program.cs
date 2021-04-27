using System;
using System.Windows.Forms;
using Hp.Ohl.WmiService;
using OmenHubLight.Forms;
using OmenHubLight.Lib;
using OpenHardwareMonitor.Hardware;

namespace OmenHubLight
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WmiEventWatcher.StartHpBiosEventWatcher();
            Ring0.Open();
            Opcode.Open();
            GpuInfo.OpenGpuGroups();
            CpuInfo.OpenCpuGroups();

            Application.ApplicationExit += AppExitCleanUp;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        static void AppExitCleanUp(object sender, EventArgs e)
        {
            WmiEventWatcher.StopHpBiosEventWatcher();
            GpuInfo.CloseGpuGroups();
            CpuInfo.CloseCpuGroups();
            Opcode.Close();
            Ring0.Close();
        }
    }
}