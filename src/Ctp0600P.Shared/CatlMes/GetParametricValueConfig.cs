namespace Ctp0600P.Shared.CatlMes
{

    public class GetParametricValueConfig
    {
        public CatlMesConnectionParams ConnectionParams { get; set; }
        public GetParametricValueParams InterfaceParams { get; set; }
    }
    public class GetParametricValueParams : CatlMesConfigurationBase
    {
        public string Resource { get; set; }
    }
}
