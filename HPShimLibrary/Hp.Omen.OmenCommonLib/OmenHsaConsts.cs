namespace Hp.Omen.OmenCommonLib
{
    public static class OmenHsaConsts
    {
        public enum SystemType
        {
            GPU,
            CPU,
            Memory
        }

        public enum WMILedAnimation
        {
            Zone,
            ColorMode,
            Time,
            Brightness,
            ColorCount
        }

        public const int WMI_CMD_LED_LIGHTING_CONTROL = 131081;

        public const int WMI_CMD_TYPE_GET_MODE_COLOR = 2;

        public const int WMI_CMD_TYPE_SET_MODE_COLOR = 3;

        public const int WMI_CMD_TYPE_GET_BRIGHTNESS = 4;

        public const int WMI_CMD_TYPE_SET_BRIGHTNESS = 5;

        public const int WMI_CMD_TYPE_GET_LED_ANIMATION = 6;

        public const int WMI_CMD_TYPE_SET_LED_ANIMATION = 7;

        public const byte WMI_CMD_TYPE_MODE = 3;

        public const int WMI_DATA_SIZE_MODE_COLOR = 128;

        public const int WMI_DATA_SIZE_MODE_BRIGHTNESS = 4;

        public const int RETURN_SUCCESS = 0;

        public const int RETURN_FAILED = -1;

        public const int COLOR_OFFSET = 1;

        public const int COLOR_NUM = 3;

        public const int COLOR_R = 0;

        public const int COLOR_G = 1;

        public const int COLOR_B = 2;

        public const string JSON_RETURN_CODE = "returnCode";

        public const string JSON_RETURN_DATA = "returnData";

        public const byte WMI_BrightnessDimValue = 50;

        public const byte WMI_BrightnessBrightValue = 100;

        public const int WMI_DurationShortValue = 1;

        public const int WMI_DurationMediumValue = 3;

        public const int WMI_DurationLongValue = 6;

        public static readonly int CAP_MODULE_CANT_LOAD_ERROR = 1610612742;

        public static readonly string CAP_MODULE_CANT_LOAD = "CAP_MODULE_CANT_LOAD";
    }
}