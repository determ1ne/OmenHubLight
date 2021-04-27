using System.Collections.Generic;
using OpenHardwareMonitor.Hardware;
using OpenHardwareMonitor.Hardware.CPU;

namespace OmenHubLight.Lib
{
    public static class CpuInfo
    {
        private static CPUGroup _cpuGroup;
        private static object CPUGroupLock = new();

        public static void OpenCpuGroups()
        {
            if (_cpuGroup == null)
            {
                lock (CPUGroupLock)
                {
                    if (_cpuGroup == null)
                    {
                        var settings = new Computer.Settings();
                        _cpuGroup = new CPUGroup(settings);
                    }
                }
            }
        }

        public static IEnumerable<IHasTempSensor> GetCpuHardwares()
        {
            lock (CPUGroupLock)
            {
                if (_cpuGroup == null) yield break;

                foreach (var h in _cpuGroup.Hardware)
                {
                    if (h is IHasTempSensor i)
                    {
                        yield return i;
                    }
                }
            }
        }

        public static void CloseCpuGroups()
        {
            lock (CPUGroupLock)
            {
                if (_cpuGroup != null)
                {
                    _cpuGroup.Close();
                }

                _cpuGroup = null;
            }
        }
    }
}