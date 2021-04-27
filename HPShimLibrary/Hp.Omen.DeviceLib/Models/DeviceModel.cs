using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using Hp.Omen.AppShim;
using Hp.Omen.DeviceLib.Models.DeviceEnums;
using Hp.Omen.OmenCommonLib.Enums;
using Hp.Omen.OmenCommonLib.Structs;
using Hp.Omen.OmenCommonLib.Utilities;
using Hp.Omen.OmenCommonLib.WMI;
using Newtonsoft.Json;

namespace Hp.Omen.DeviceLib.Models
{
    public sealed class DeviceModel : IDeviceModel
    {
        private static DeviceModel _instance;
        private static DeviceType? _deviceType;
        private static ICollection<string> _featureByte;
        private static bool? _isHp;
        private static bool? _isOmen;
        private static bool? _isGamingProduct;
        private static Platform? _omenPlatform;
        private static List<Platform> _omenPlatformInfo;
        private static DeviceType? _thisDevice;
        private static string _thisSystemId;
        private static readonly object DeviceTypeLock = new object();
        private static readonly object FeatureByteLock = new object();
        private static readonly object OmenPlatformLock = new object();
        private static readonly object OmenPlatformInfoLock = new object();
        private static readonly object ThisDeviceLock = new object();
        private static readonly object ThisSystemIdLock = new object();
        public static DeviceModel Instance => _instance ??= new DeviceModel();

        public static bool IsOmen => _isOmen ??= ThisDevice != DeviceType.None && ThisDisplayName.Contains("OMEN");

        public static bool IsNvStudio = FeatureByte.Contains("kR");

        public static string ThisDisplayName => OmenPlatform.DisplayName ?? "unknown";

        public static DeviceType DeviceType
        {
            get
            {
                if (_deviceType == null)
                    lock (DeviceTypeLock)
                    {
                        _deviceType ??= OmenPlatform.Name;
                        OMENEventSource.Log.Info("DeviceLib: Platform = " + _deviceType);
                    }

                return _deviceType.GetValueOrDefault();
            }
        }

        public static ICollection<string> FeatureByte
        {
            get
            {
                if (_featureByte == null)
                    lock (FeatureByteLock)
                    {
                        if (_featureByte == null)
                        {
                            try
                            {
                                var input = string.Empty;
                                foreach (var info in HardwareInfo.ComputerSystem)
                                    if (info.OEMStringArray != string.Empty)
                                    {
                                        input = info.OEMStringArray;
                                        break;
                                    }

                                var matchCollection = Regex.Matches(input, "(?<=FBYTE#)(.+?)(?=;)");
                                if (matchCollection.Count > 0)
                                    _featureByte = new HashSet<string>(Regex.Split(matchCollection[0].ToString(),
                                        "(?<=\\G.{2})", RegexOptions.Singleline));
                            }
                            catch (Exception ex)
                            {
                                OMENEventSource.Log.Error("Read Feature byte data exception:" + ex.Message);
                            }

                            if (_featureByte == null) _featureByte = new HashSet<string>();
                        }
                    }

                return _featureByte;
            }
        }

        public static bool IsHp
        {
            get
            {
                if (_isHp == null)
                {
                    var value = GetManageObjValue("root\\CIMV2", "SELECT * FROM Win32_BaseBoard", "Manufacturer")
                        ?.ToString();
                    if (value == null) value = string.Empty;
                    _isHp = value.Contains("Hp") || value.Contains("Hewlett-Packard");
                }

                return _isHp ?? false;
            }
        }

        public static bool IsGamingProduct
        {
            get
            {
                if (_isGamingProduct == null)
                {
                    if (IsOmen) _isGamingProduct = true;
                    else if (FeatureByte.Contains("7K") && FeatureByte.Contains("fd") &&
                             ThisDisplayName.Contains("PAVILION"))
                        _isGamingProduct = true;
                    else _isGamingProduct = false;
                }

                return _isGamingProduct.Value;
            }
        }

        public static Platform OmenPlatform
        {
            get
            {
                if (_omenPlatform == null)
                    lock (OmenPlatformLock)
                    {
                        if (_omenPlatform == null)
                        {
                            foreach (var value in OmenPlatformInfo)
                                if (value.ProductNum.Contains(ThisSystemId))
                                {
                                    if (value.Name == DeviceType.HolmesG)
                                    {
                                        var flag = FeatureByte.Contains("7K") && FeatureByte.Contains("fd");
                                        _omenPlatform = flag ? value : _omenPlatform;
                                        OMENEventSource.Log.Info($"DeviceLib: HolmesG Platform : Gaming SKU = {flag}");
                                        break;
                                    }

                                    _omenPlatform = value;
                                    break;
                                }

                            if (_omenPlatform == null && IsHp &&
                                Misc.GetDeviceStatus(@"SWC\HpIC0003") == DeviceStatus.Ok)
                            {
                                _omenPlatform = OmenPlatformInfo.Find(x => x.Name == DeviceType.HpPC);
                            }

                            _omenPlatform ??= new Platform();
                        }
                    }

                return _omenPlatform.GetValueOrDefault();
            }
        }

        public static List<Platform> OmenPlatformInfo
        {
            get
            {
                if (_omenPlatformInfo == null)
                    lock (OmenPlatformInfoLock)
                    {
                        _omenPlatformInfo ??=
                            JsonConvert.DeserializeObject<List<Platform>>(
                                Encoding.UTF8.GetString(Resources.Hp_Omen_DeviceLib_JSON_DeviceList));
                    }

                return _omenPlatformInfo;
            }
        }

        public static DeviceType ThisDevice
        {
            get
            {
                if (_thisDevice == null)
                    lock (ThisDeviceLock)
                    {
                        _thisDevice ??= DeviceType;
                    }

                return _thisDevice.GetValueOrDefault();
            }
        }

        public static string ThisSystemId
        {
            get
            {
                if (_thisSystemId == null)
                    lock (ThisSystemIdLock)
                    {
                        if (_thisSystemId == null)
                            if (CurrentApp.Properties[PlatformDataConsts.SSID] == null)
                            {
                                var ssid = OmenSMBiosHelper.SystemID;

                                if (!string.IsNullOrEmpty(ssid)) _thisSystemId = ssid;

                                CurrentApp.Properties[PlatformDataConsts.SSID] = _thisSystemId;
                            }
                            else
                            {
                                _thisSystemId = (string) CurrentApp.Properties[PlatformDataConsts.SSID];
                            }

                        OMENEventSource.Log.Info("DeviceLib: SSID = " + _thisSystemId);
                    }

                return _thisSystemId;
            }
        }

        private static object GetManageObjValue(string queryScope, string queryStr, string propertyStr)
        {
            object obj = null;
            foreach (var baseObj in new ManagementObjectSearcher(queryScope, queryStr).Get())
                try
                {
                    obj = baseObj.GetPropertyValue(propertyStr);
                    if (obj != null)
                    {
                        if (obj is string[])
                            OMENEventSource.Log.Info(propertyStr + " \"" + ((string[]) obj)[0] + "\"");
                        else
                            OMENEventSource.Log.Info(propertyStr + " \"" + obj + "\"");
                        return obj;
                    }

                    OMENEventSource.Log.Info(propertyStr + " \"Not Found!!\"");
                }
                catch (Exception ex)
                {
                    OMENEventSource.Log.Error(propertyStr + " \"" + ex.Message + "\"");
                }

            return obj;
        }

        public DeviceType GetDevice()
        {
            return ThisDevice;
        }
    }
}