using System;
using System.Collections.Generic;
using Hp.Omen.DeviceLib.Models.DeviceEnums;
using Hp.Omen.OmenCommonLib;
using Hp.Omen.OmenCommonLib.PowerControl.Enum;
using Hp.Omen.OmenCommonLib.WMI;

namespace Hp.Omen.DeviceLib.Models
{
    public static class PerformanceControlHelper
    {
        private static OmenHsaClient _omenHsaClient = new OmenHsaClient();
        private static bool? _isBiosCoolModeSupported;
        private static bool? _isBiosPerformanceControlSupport;
        private static bool? _isMaxFanSupported;
        private static bool? _isPowerAwareSupported;
        private static bool? _isPowerControlSupported;
        private static bool? _isSupported;
        private static List<PerformanceCtrlPlatform> _supportPlatforms;

        private static bool IsBiosCoolModeSupported { get; }

        public static bool IsBiosPerformanceControlSupport
        {
            get
            {
                return _isBiosPerformanceControlSupport ??=
                    _omenHsaClient.GetThermalPolicyVersion() == ThermalPolicyVersion.V1;
            }
        }

        public static bool IsMaxFanSupported { get; }
        public static bool IsPowerAwareSupported { get; }
        public static bool IsPowerControlSupported { get; }
        public static bool IsSupported { get; }


        public static List<PerformanceCtrlPlatform> SupportPlatforms { get; }


        private static void CheckForDRX()
        {
            var biosVersion = OmenSMBiosHelper.BIOSVersion;
            if (!string.IsNullOrEmpty(biosVersion))
            {
                var num = string.Compare(biosVersion, "F.07", StringComparison.OrdinalIgnoreCase);
                _isMaxFanSupported = num >= 0;
                return;
            }

            _isMaxFanSupported = false;
        }

        private static bool CheckIfPowerControlSupported()
        {
            return DeviceModel.Instance.GetDevice() != DeviceType.Gamora10;
        }
    }
}