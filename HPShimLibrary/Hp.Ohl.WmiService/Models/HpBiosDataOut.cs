namespace Hp.Ohl.WmiService.Models
{
    public record HpBiosDataOut(string OriginalDataType, bool? Active, byte[] Data, string InstanceName,
        uint RwReturnCode, byte[] Sign);
}