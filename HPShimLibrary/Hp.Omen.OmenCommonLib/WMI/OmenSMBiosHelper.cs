using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.CompilerServices;
using Hp.Omen.OmenCommonLib.Structs;
using Hp.Omen.OmenCommonLib.Utilities;

namespace Hp.Omen.OmenCommonLib.WMI
{
    public static class OmenSMBiosHelper
    {
        private const string DEFAULT_MANAGEMENT_OBJECT_SEARCHER_SCOPE = "root\\CIMV2";
        private static readonly object BiosVersionLock = new object();
        private static readonly object MemoryInfoLock = new object();
        private static readonly object SystemIdLock = new object();
        private static readonly object SystemSkuLock = new object();
        private static string biosVersion;
        private static List<PhysicalMemoryInfo> memoryInfo;
        private static string systemId;
        private static string systemSku;

        public static string BIOSVersion
        {
            get
            {
                if (biosVersion == null)
                {
                    var biosVersionLock = BiosVersionLock;
                    lock (biosVersionLock)
                    {
                        if (biosVersion == null)
                        {
                            try
                            {
                                string[] array = null;
                                using var mngObjSearcher =
                                    CreateDefaultManagementObjectSearcher("SELECT BIOSVersion FROM Win32_BIOS");
                                using var mngObjCollection = mngObjSearcher.Get();
                                foreach (var mngBaseObject in mngObjCollection)
                                {
                                    array = (string[]) mngBaseObject["BIOSVersion"];
                                }

                                if (array != null)
                                {
                                    biosVersion = ((array.Length == 1) ? array[0] : array[1]);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogError(ex);
                            }

                            if (string.IsNullOrEmpty(biosVersion)) biosVersion = string.Empty;
                        }
                    }
                }

                return biosVersion;
            }
        }

        public static ManagementObjectSearcher CreateDefaultManagementObjectSearcher(string query)
            => new ManagementObjectSearcher(@"root\CIMV2", query, new EnumerationOptions
            {
                ReturnImmediately = true,
                Rewindable = false
            });

        public static void LogError(Exception exception, [CallerMemberName] string callerName = "")
        {
            OMENEventSource.Log.Error(
                $"OmenSMBiosHelper{(string.IsNullOrEmpty(callerName) ? string.Empty : "." + callerName)} Exception: {exception}");
        }

        public static string SystemID
        {
            get
            {
                if (systemId == null)
                {
                    lock (SystemIdLock)
                    {
                        if (systemId == null)
                        {
                            try
                            {
                                using var mngObjSearcher =
                                    CreateDefaultManagementObjectSearcher("SELECT Product FROM Win32_BaseBoard");
                                using var mngObjCollection = mngObjSearcher.Get();
                                foreach (var mngBaseObject in mngObjCollection)
                                {
                                    systemId = mngBaseObject["Product"].ToString();
                                }
                            }
                            catch (Exception ex)
                            {
                                LogError(ex);
                            }

                            systemId ??= string.Empty;
                        }
                    }
                }

                return systemId;
            }
        }
    }
}