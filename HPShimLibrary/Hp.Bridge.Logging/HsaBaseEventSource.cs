using System;
using Hp.Omen.OmenCommonLib.Utilities;

namespace Hp.Bridge.Logging
{
    public class HsaBaseEventSource : OMENEventSource
    {
        private static readonly Lazy<HsaBaseEventSource> log =
            new Lazy<HsaBaseEventSource>(() => new HsaBaseEventSource());

        public new static HsaBaseEventSource Log => log.Value;
        public string ClientId => "debug";
    }
}