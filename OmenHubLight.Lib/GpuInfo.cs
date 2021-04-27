using System.Collections.Generic;
using OpenHardwareMonitor.Hardware;
using OpenHardwareMonitor.Hardware.ATI;
using OpenHardwareMonitor.Hardware.Nvidia;

namespace OmenHubLight.Lib
{
    public static class GpuInfo
    {
        private static List<IGroup> _gpuGroups;
        private static object GPUGroupsLock = new();

        public static void OpenGpuGroups()
        {
            if (_gpuGroups == null)
            {
                lock (GPUGroupsLock)
                {
                    if (_gpuGroups == null)
                    {
                        var settings = new Computer.Settings();
                        _gpuGroups = new();

                        var atiGroup = new ATIGroup(settings);
                        if (atiGroup.Hardware.Length > 0) _gpuGroups.Add(atiGroup);
                        else atiGroup.Close();

                        var nvidiaGroup = new NvidiaGroup(settings);
                        if (nvidiaGroup.Hardware.Length > 0) _gpuGroups.Add(nvidiaGroup);
                        else nvidiaGroup.Close();
                    }
                }
            }
        }

        public static IEnumerable<IHasTempSensor> GetGpuHardwares()
        {
            lock (GPUGroupsLock)
            {
                if (_gpuGroups == null) yield break;

                foreach (var g in _gpuGroups)
                {
                    foreach (var h in g.Hardware)
                    {
                        if (h is IHasTempSensor i)
                            yield return i;
                    }
                }
            }
        }

        public static void CloseGpuGroups()
        {
            lock (GPUGroupsLock)
            {
                if (_gpuGroups != null)
                {
                    foreach (var gpuGroup in _gpuGroups)
                    {
                        gpuGroup.Close();
                    }
                }

                _gpuGroups.Clear();
                _gpuGroups = null;
            }
        }
    }
}