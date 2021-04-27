using System;
using System.Collections.Generic;
using System.Linq;
using Hp.Ohl.WmiService;
using Hp.Omen.AppShim;
using Hp.Omen.OmenCommonLib.PowerControl.Enum;
using Hp.Omen.OmenCommonLib.Utilities;
using Hp.Omen.OmenCommonLib.WMI;

namespace Hp.Omen.OmenCommonLib
{
    public class OmenHsaClient
    {
        private static byte[] _systemDesignData;

        public byte[] SystemDesignData
        {
            get
            {
                if (_systemDesignData == null)
                {
                    _systemDesignData = (byte[]) CurrentApp.Properties["SystemDesignData"];
                    if (_systemDesignData == null)
                    {
                        var inputData = new byte[4];
                        _systemDesignData = BiosWmiCmd_Get(131080, 40, inputData, 128);
                        if (_systemDesignData != null) CurrentApp.Properties["SystemDesignData"] = _systemDesignData;
                    }
                }

                return _systemDesignData;
            }
        }

        private void OutputLogError(string message)
        {
            OMENEventSource.Log.Error($"{this} : {message}");
        }

        public byte[] BiosWmiCmd_Get(uint commandType)
        {
            return BiosWmiCmd_Get(131081, commandType, new byte[1], 128);
        }

        public byte[] BiosWmiCmd_Get(uint command, uint commandType, byte[] inputData,
            uint returnDataSize)
        {
            try
            {
                var result = HpBiosIntHelper.InvokeBiosCommand(command, commandType, returnDataSize, inputData);
                return result.Data;
            }
            catch (Exception ex)
            {
                OutputLogError(ex.Message);
            }

            return null;
        }

        public int BiosWmiCmd_Set(uint commandType, byte[] inputData)
        {
            return BiosWmiCmd_Set(131081, commandType, inputData);
        }

        public int BiosWmiCmd_Set(uint command, uint commandType, byte[] inputData)
        {
            try
            {
                var result = HpBiosIntHelper.InvokeBiosCommand(command, commandType, 4, inputData);
                return result.Data[0];
            }
            catch (Exception ex)
            {
                OutputLogError(ex.Message);
            }

            return -1;
        }

        public byte[] BiosWmiCmd_GetColor()
        {
            return BiosWmiCmd_Get(2);
        }

        public int BiosWmiCmd_SetColor(byte[] inputData)
        {
            return BiosWmiCmd_Set(3, inputData);
        }

        public byte[] BiosWmiCmd_GetBrightness()
        {
            return BiosWmiCmd_Get(4);
        }

        public int BiosWmiCmd_SetBrightness(byte[] inputData)
        {
            return BiosWmiCmd_Set(5, inputData);
        }

        public int GetKeyboardBrightness()
        {
            var array = BiosWmiCmd_GetBrightness();
            if (array != null && array.Length >= 1)
            {
                return array[0] & 128;
            }

            throw new Exception("BIOS returned malformed message");
        }

        public int SetKeyboardBrightness(byte brightness)
        {
            var d = new byte[4];
            d[0] = brightness;
            return BiosWmiCmd_SetBrightness(d);
        }

        public ThermalPolicyVersion GetThermalPolicyVersion()
        {
            var result = ThermalPolicyVersion.V0;
            var systemDesignData = SystemDesignData;
            if (systemDesignData != null && systemDesignData.Length != 0)
                result = (ThermalPolicyVersion) systemDesignData[3];
            if (new[]
            {
                "8607",
                "8746",
                "8747",
                "8749",
                "874A",
                "8748"
            }.Contains(OmenSMBiosHelper.SystemID))
                result = ThermalPolicyVersion.V0;
            OMENEventSource.Log.Info("GetThermalPolicyVersion(), version = " + result);
            return result;
        }

        public List<byte> GetFanLevel()
        {
            var data = new List<byte>();
            var result = BiosWmiCmd_Get(131080, 45, new byte[4], 128);
            if (result != null && result.Length != 0)
            {
                foreach (var b in result)
                {
                    if (b == 0) break;
                    data.Add(b);
                }
            }

            return data;
        }
    }
}