using Catl.WebServices.DataCollectForResourceInspect;
using Catl.WebServices.MIFindCustomAndSfcData;

using Yee.Entitys;
using Yee.Entitys.CATL;

namespace Yee.Services.CatlMesInvoker
{
    /// <summary>
    /// 调用CATL WS，接收的参数和返回的类型和WSDL相关。并自动按C的要求记录Excel日志
    /// </summary>
    public interface ICatlMesInvoker
    {
        Task<CatlMESReponse> dataCollect(string sfc, IList<DcParamValue> uploadCATLData, bool isStatusChange, string stationCode = "");
        Task<CatlMESReponse> DataCollectForResourceInspectTask(List<machineIntegrationParametricData> machineIntegrationParametricDatas, string dcGroup, string StationCode = "");
        Task<CatlMESReponse> MiAssembleAndCollectDataForSfc(IList<BomData> bomDatas, string sfc, string stationCode = "");
        Task<CatlMESReponse> MICheckBOMInventory(string sfc, string MatteryPN, string Barcode, int useNum = 1, string stationCode = "");
        Task<CatlMESReponse> MiFindCustomAndSfcData(string sfc, modeProcessSFC? modeProcessSFC, string StationCode = "");
        Task<CatlMESReponse> GetModuleCodeByCellCode(string cellCode, string StationCode = "");
        Task<CatlMESReponse> CheckSfcStatu(string sfc, string StationCode = "");
        Task<CatlMESReponse> CheckInventoryAttributes(string sfc, string moduleCode, string StationCode = "");
        Task<CatlMESReponse> GetParametricValue(string sfc, string parameter, string stationCode = "");
        Task<CatlMESReponse> dataCollect(string sfc, IList<DcParamValue> uploadCATLData, string DcGroup, bool isStatusChange, string stationCode = "");
        Task<CatlMESReponse> AutoNc(string sfc, string stationCode = "", string ncCode = "");
    }
}
