using System;
using System.Collections.Generic;
using System.Management;
using Hp.Omen.OmenCommonLib.Enums;

namespace Hp.Omen.OmenCommonLib.Utilities
{
    public class Misc
    {
        private static List<PnpDeviceInfo> pnpDeviceInfos = null;

        private static readonly string[] suffixes =
        {
            "Bytes",
            "KB",
            "MB",
            "GB",
            "TB",
            "PB"
        };

        public static DeviceStatus GetDeviceStatus(string hwid)
        {
            var result = DeviceStatus.NotExisting;
            if (pnpDeviceInfos == null)
            {
                pnpDeviceInfos = new List<PnpDeviceInfo>();
                try
                {
                    using ManagementObjectSearcher managementObjectSearcher =
                        new ManagementObjectSearcher("Select Name, Status, HardwareID from Win32_PnPEntity");
                    foreach (var baseObj in managementObjectSearcher.Get())
                    {
                        if (baseObj.GetPropertyValue("HardwareID") != null)
                        {
                            var list = pnpDeviceInfos;
                            var pnpDeviceInfo = new PnpDeviceInfo
                            {
                                HardwareIDs = (string[]) baseObj.GetPropertyValue("HardwareID"),
                                Name = baseObj.GetPropertyValue("Name")?.ToString(),
                                Status = baseObj.GetPropertyValue("Status")?.ToString(),
                            };
                            list.Add(pnpDeviceInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OMENEventSource.Log.Error("GetDeviceStatus(), error: " + ex.Message);
                }
            }

            foreach (var info in pnpDeviceInfos)
            {
                foreach (var text in info.HardwareIDs)
                {
                    if (text == hwid)
                    {
                        OMENEventSource.Log.Info(
                            $"GetDeviceStatus(), hardwareID: {text}, name: {info.Name}, status: {info.Status}");
                        result = info.Status.ToLower() == "ok" ? DeviceStatus.Ok : DeviceStatus.Disabled;
                        return result;
                    }
                }
            }

            return result;
        }

        internal class PnpDeviceInfo
        {
            public string[] HardwareIDs { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
        }
    }
}