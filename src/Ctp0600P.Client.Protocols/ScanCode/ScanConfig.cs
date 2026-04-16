namespace Ctp0600P.Client.Protocols.ScanCode
{
    public class ScanConfigList
    {
        public List<ScanConfig> ConfigList { get; set; }
    }

    public class ScanConfig
    {
        public string ScanIP { get; set; }
        public int ScanPort { get; set; }
    }
}
