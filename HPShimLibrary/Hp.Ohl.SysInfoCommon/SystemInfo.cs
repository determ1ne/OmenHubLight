using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Hp.Omen.DeviceLib.Models;
using Hp.Omen.OmenCommonLib.Utilities;

namespace Hp.Ohl.SysInfoCommon
{
    /// <summary>
    ///     Substitute for "HpseuCommon.HpSystemInfoWin32" in Hp System Event Utilities assemblies.
    ///     Requires WMI to work.
    /// </summary>
    public static class SystemInfo
    {
        private const string BaseBoardPrefix = "BB_";
        private const string BIOSPrefix = "BIOS_";
        private const string ComputerSystemPrefix = "CS_";
        private const string MiscPrefix = "M_";
        private static readonly Hashtable Props = new();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
        static SystemInfo()
        {
            var baseboardFilter = new List<string> {"Product", "Version", "SerialNumber"};
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            var collection = searcher.Get();
            foreach (var baseObj in collection)
            foreach (var prop in baseObj.Properties)
                if (baseboardFilter.Contains(prop.Name))
                    Props[BaseBoardPrefix + prop.Name] = prop.Value;

            var biosFilter = new List<string>
                {"SMBIOSBIOSVersion", "ReleaseDate", "Version", "SerialNumber", "Manufacturer"};
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            collection = searcher.Get();
            foreach (var baseObj in collection)
            foreach (var prop in baseObj.Properties)
                if (biosFilter.Contains(prop.Name))
                    Props[BIOSPrefix + prop.Name] = prop.Value;

            var biosReleaseDate = (string) Props[BIOSPrefix + "ReleaseDate"];
            if (!string.IsNullOrEmpty(biosReleaseDate))
                try
                {
                    Props[MiscPrefix + "BIOSReleaseDate"] = new DateTime(int.Parse(biosReleaseDate[..4]),
                        int.Parse(biosReleaseDate[4..6]), int.Parse(biosReleaseDate[6..8]));
                }
                catch (Exception)
                {
                    OMENEventSource.Log.Warn("Cannot get Win32_BIOS.BIOSReleaseDate");
                }

            var computerSystemFilter = new List<string>
                {"OEMStringArray", "Model", "SystemFamily", "SystemSKUNumber", "TotalPhysicalMemory"};
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            collection = searcher.Get();
            foreach (var baseObj in collection)
            foreach (var prop in baseObj.Properties)
                if (computerSystemFilter.Contains(prop.Name))
                    Props[ComputerSystemPrefix + prop.Name] = prop.Value;

            if (Props[ComputerSystemPrefix + "OEMStringArray"] != null)
                try
                {
                    Props[ComputerSystemPrefix + "OEMStringArray"] =
                        ((string[]) Props[ComputerSystemPrefix + "OEMStringArray"])!.Where(x =>
                            !string.IsNullOrWhiteSpace(x)).ToArray();
                    Props[MiscPrefix + "OEMString"] =
                        string.Join("", ((string[]) Props[ComputerSystemPrefix + "OEMStringArray"])!);
                }
                catch (Exception ex)
                {
                    OMENEventSource.Log.Warn("OEMStringArray is not string[]" + ex.Message);
                    Props[ComputerSystemPrefix + "OEMStringArray"] = null;
                }

            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            collection = searcher.Get();
            foreach (var baseObj in collection)
            {
                var processorName = (string) baseObj.Properties["Name"].Value;
                if (!string.IsNullOrEmpty(processorName))
                {
                    Props[MiscPrefix + "ProcessorName"] = processorName;
                    break;
                }
            }
        }

        public static string BaseBoardProduct => (string) Props[BaseBoardPrefix + "Product"];
        public static string BaseBoardVersion => (string) Props[BaseBoardPrefix + "Version"];
        public static string BaseBoardSerialNumber => (string) Props[BaseBoardPrefix + "SerialNumber"];
        public static string BiosVersion => (string) Props[BIOSPrefix + "SMBIOSBIOSVersion"];
        public static string BiosVersionInternal => (string) Props[BIOSPrefix + "Version"];
        public static DateTime? BiosReleaseDate => (DateTime?) Props[MiscPrefix + "BIOSReleaseDate"];
        public static string BiosSerialNumber => (string) Props[BIOSPrefix + "SerialNumber"];
        public static string BiosManufacturer => (string) Props[BIOSPrefix + "Manufacturer"];
        public static string Model => Props[ComputerSystemPrefix + "Model"] as string;
        public static string SystemFamily => (string) Props[ComputerSystemPrefix + "SystemFamily"];
        public static string SystemSkuNumber => (string) Props[ComputerSystemPrefix + "SystemSKUNumber"];
        public static string ProcessorName => (string) Props[MiscPrefix + "ProcessorName"];
        public static string GetDeviceInternalCode => DeviceModel.ThisDevice.ToString();
        public static bool IsHpDevice => DeviceModel.IsHp;
        public static bool IsOmen => DeviceModel.IsOmen;
        public static bool IsGamingProduct => DeviceModel.IsGamingProduct;
        public static string OmenFeature => string.Join(", ", DeviceModel.OmenPlatform.Feature ?? new List<string>());

        public static string OmenBackgroundFeature =>
            string.Join(", ", DeviceModel.OmenPlatform.BackgroundFeature ?? new List<string>());

        public static string OmenProductNum =>
            string.Join(", ", DeviceModel.OmenPlatform.ProductNum ?? new List<string>());

        public static bool IsNvStudio => DeviceModel.IsNvStudio;

        public static string BuildId
        {
            get
            {
                var oemString = (string) Props[MiscPrefix + "OEMString"];
                if (string.IsNullOrEmpty(oemString)) return null;
                var indexL = oemString.IndexOf("BUILDID#", StringComparison.InvariantCulture);
                if (indexL < 0) return null;
                var indexR = oemString!.IndexOf(";", indexL, StringComparison.InvariantCulture);
                if (indexR >= 0 && indexL + 8 < indexR)
                    return oemString[(indexL + 8)..indexR];
                return null;
            }
        }

        public static string FeatureByte
        {
            get
            {
                var oemString = (string) Props[MiscPrefix + "OEMString"];
                if (string.IsNullOrEmpty(oemString)) return null;
                var indexL = oemString.IndexOf("FBYTE#", StringComparison.InvariantCulture);
                if (indexL < 0) return null;
                var indexR = oemString!.IndexOf(";", indexL, StringComparison.InvariantCulture);
                if (indexR >= 0 && indexL + 5 < indexR)
                    return oemString[(indexL + 5)..indexR];
                return null;
            }
        }

        public static string TotalPhysicalMemory
        {
            get
            {
                var memory = (ulong?) Props[ComputerSystemPrefix + "TotalPhysicalMemory"];
                return memory == null ? null : $"{memory / 1024 / 1024} M";
            }
        }
    }
}