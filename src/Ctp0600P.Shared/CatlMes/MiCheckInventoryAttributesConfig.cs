using Catl.WebServices.MICheckInventoryAttribute;

namespace Ctp0600P.Shared.CatlMes
{

    public class MiCheckInventoryAttributesConfig
    {
        public CatlMesConnectionParams ConnectionParams { get; set; }
        public MiCheckInventoryAttributesParams InterfaceParams { get; set; }
    }
    public class MiCheckInventoryAttributesParams : CatlMesConfigurationBase
    {
        public string Resource { get; set; }
        public modeCheckInventory modeCheckInventory { get; set; }
    }

}
