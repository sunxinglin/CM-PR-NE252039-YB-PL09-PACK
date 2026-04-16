namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi;

public class AnyLoadConfigList
{
    public List<AnyLoadConfig> ConfigList { get; set; }
}

public class AnyLoadConfig
{
    public string AnyLoadIP { get; set; }
    public int AnyLoadPort { get; set; }
}