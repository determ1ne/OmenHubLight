using System.Collections.Generic;
using Hp.Omen.DeviceLib.Models.DeviceEnums;
using Hp.Omen.OmenCommonLib.Utilities;

namespace Hp.Omen.DeviceLib.Models
{
    public struct PerformanceCtrlPlatform
    {
        public string Sku { get; set; }
        public string SSID { get; set; }
        public bool AlwaysSupport { get; set; }
        public List<HpBiosVersion> CoolModeSupportBios { get; set; }
        public bool IsPowerAwareSupported { get; set; }
        public bool IsMaxFanSupported { get; set; }
    }

    public struct Platform
    {
        public DeviceType Name { get; set; }
        public string DisplayName { get; set; }
        public List<string> ProductNum { get; set; }
        public List<string> Feature { get; set; }
        public List<string> BackgroundFeature { get; set; }
    }
}