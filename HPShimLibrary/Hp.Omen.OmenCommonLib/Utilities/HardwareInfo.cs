using System;
using System.Collections.Generic;
using System.Management;
using Hp.Omen.OmenCommonLib.Models;
using Hp.Omen.OmenCommonLib.Structs;

namespace Hp.Omen.OmenCommonLib.Utilities
{
    public static class HardwareInfo
    {
        private static List<ComputerSystemInfo> _computerSystems;
        private static readonly object ComputerSystemsLock = new object();

        public static List<ComputerSystemInfo> ComputerSystem
        {
            get
            {
                if (_computerSystems == null)
                {
                    lock (ComputerSystemsLock)
                    {
                        var list = new List<ComputerSystemInfo>();
                        if (_computerSystems == null)
                        {
                            if (AppShim.CurrentApp.Properties[PlatformDataConsts.Manufacturer] == null
                                || AppShim.CurrentApp.Properties[PlatformDataConsts.OEMStringArray] == null
                                || AppShim.CurrentApp.Properties[PlatformDataConsts.NumberOfLogicalProcessors] == null)
                            {
                                try
                                {
                                    using var searcher = new ManagementObjectSearcher(@"root\CIMV2",
                                        "SELECT Manufacturer, OEMStringArray, NumberOfLogicalProcessors FROM Win32_ComputerSystem",
                                        new EnumerationOptions
                                        {
                                            Rewindable = false
                                        });
                                    using var collection = searcher.Get();
                                    foreach (var obj in collection)
                                    {
                                        list.Add(new ComputerSystemInfo
                                        {
                                            Manufacturer = obj["Manufacturer"].ToString(),
                                            OEMStringArray = string.Concat((string[]) obj["OEMStringArray"] ??
                                                                           Array.Empty<string>()),
                                            NumberOfLogicalProcessors = obj["NumberOfLogicalProcessors"].ToString()
                                        });
                                    }

                                    if (list.Count > 0)
                                    {
                                        AppShim.CurrentApp.Properties[PlatformDataConsts.Manufacturer] =
                                            list[0].Manufacturer;
                                        AppShim.CurrentApp.Properties[PlatformDataConsts.OEMStringArray] =
                                            list[0].OEMStringArray;
                                        AppShim.CurrentApp.Properties[PlatformDataConsts.NumberOfLogicalProcessors] =
                                            list[0].NumberOfLogicalProcessors;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    OMENEventSource.Log.Error($"HardwareInfo.ComputerSystem Exception: {ex}");
                                }
                            }
                        }
                        else
                        {
                            list.Add(new ComputerSystemInfo
                            {
                                Manufacturer = (string) AppShim.CurrentApp.Properties[PlatformDataConsts.Manufacturer],
                                OEMStringArray =
                                    (string) AppShim.CurrentApp.Properties[PlatformDataConsts.OEMStringArray],
                                NumberOfLogicalProcessors =
                                    (string) AppShim.CurrentApp.Properties[PlatformDataConsts.NumberOfLogicalProcessors]
                            });
                        }

                        _computerSystems = list;
                    }
                }

                return _computerSystems;
            }
        }
    }
}