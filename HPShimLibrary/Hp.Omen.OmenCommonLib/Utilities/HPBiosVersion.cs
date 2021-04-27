using System;
using Hp.Omen.OmenCommonLib.Enums;
using Hp.Omen.OmenCommonLib.WMI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hp.Omen.OmenCommonLib.Utilities
{
    public class HpBiosVersion
    {
        private const string BetaHeader = "B";
        private const string FormalHeader = "F";

        public static readonly HpBiosVersion Current = new HpBiosVersion(OmenSMBiosHelper.BIOSVersion);

        public HpBiosVersion()
        {
        }

        public HpBiosVersion(string version)
        {
            if (string.IsNullOrEmpty(version)) return;
            try
            {
                var arr = version.Split('.');
                MajorVersion = arr[0] switch
                {
                    BetaHeader => HpBiosMajorVersion.Beta,
                    FormalHeader => HpBiosMajorVersion.Formal,
                    _ => HpBiosMajorVersion.Formal
                };

                int.TryParse(arr[1].Substring(0, 2), out var minorVersion);
                MinorVersion = minorVersion;
            }
            catch (Exception ex)
            {
                OMENEventSource.Log.Error("HpBiosVersion : " + ex.Message);
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty]
        public HpBiosMajorVersion MajorVersion { get; set; }

        [JsonProperty] public int MinorVersion { get; set; }
    }
}