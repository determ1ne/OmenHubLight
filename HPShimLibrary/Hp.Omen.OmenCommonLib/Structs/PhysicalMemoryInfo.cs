namespace Hp.Omen.OmenCommonLib.Structs
{
    public class PhysicalMemoryInfo
    {
        public string Manufacturer { get; }
        public string PartNumber { get; }

        internal PhysicalMemoryInfo(string manufacturer, string partNumber)
        {
            Manufacturer = manufacturer;
            PartNumber = partNumber;
        }
    }
}