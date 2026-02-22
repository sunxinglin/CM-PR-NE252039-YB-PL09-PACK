using AsZero.Core.Shared;
using Automatic.Entity;
using Automatic.Entity.DataDtos;
using Refit;

namespace Automatic.Protocols.Services
{
    public interface IAutomaticTraceApi
    {
        #region 通用接口
        [Get("/api/AutomaticCommon/CheckVectorBindAndFlowOrder")]
        public Task<ServiceErrResponse> CheckVectorBindAndFlowOrder(string pin, int vectorCode, string stationCode, string vectorStation, string productPn);

        [Get("/api/AutomaticCommon/GetProductPnFromCatlMes")]
        public Task<Resp<CatlMesResponse>> GetProductPnFromCatlMes(string packCode, string stationCode);

        [Get("/api/AutomaticCommon/MakePackStart")]
        public Task<Resp<CatlMesResponse>> MakePackStart(string packCode, string stationCode);

        [Get("/api/AutomaticCommon/GetCurrentStation")]
        public Task<Resp<(string, int)>> GetCurrentStation(string stationFlag, string PackCode);

        [Post("/api/AutomaticCommon/CreateMessionRecord")]
        public Task<ServiceErrResponse> CreateMessionRecord(CreateMessionRecordDto dto);

        [Get("/api/AutomaticCommon/CheckLastBoxLevel")]
        public Task<ServiceErrResponse> CheckLastBoxLevel(string StationFlag, string PackCode, int CurrentLevel);

        [Post("/api/AutomaticCommon/ChangeLastBoxLevel")]
        public Task<ServiceErrResponse> ChangeLastBoxLevel(string StationFlag, string PackCode, int CurrentLevel);

        [Post("/api/AutomaticCommon/CheckBomInventory")]
        public Task<ServiceErrResponse> CheckBomInventory(string packCode, string materialPN, string materialCode, int useNum, string stationCode);

        [Get("/api/AutomaticCommon/GetPackMAT")]
        public Task<int> GetPackMAT(string productPN);
        #endregion

        #region 模组入箱
        [Get("/api/ModuleInBox/GetModuleCodeFromCATL")]
        public Task<CatlMesResponse> GetModuleCodeFromCATL(string cellCode, string stationCode);

        [Get("/api/ModuleInBox/CheckModuleCodeFromCATL")]
        public Task<CatlMesResponse> CheckModuleCodeFromCATL(string moduleCode, string stationCode);

        [Get("/api/ModuleInBox/CheckModuleInfo")]
        public Task<ServiceErrResponse> CheckModuleInfo(string moduleCode, string modulePN, int moduleLocation, string stationCode);

        [Get("/api/ModuleInBox/CheckGlueTimeOut")]
        public Task<ServiceErrResponse> CheckGlueTimeOut(string productPn, string StationCode, string PackCode);

        [Get("/api/ModuleInBox/GetGlueDuration")]
        public Task<uint> GetGlueDuration(string productPn, string StationCode, string PackCode);

        [Post("/api/ModuleInBox/RecordModuleInfo")]
        public Task<ServiceErrResponse> RecordModuleInfo(string modulePN, string blockCode, string cellCode, string stationCode);

        [Post("/api/ModuleInBox/SaveAndUploadData")]
        public Task<ServiceErrResponse> SaveAndUploadData(ModuleInBoxDataUploadDto dto);

        [Post("/api/ModuleInBox/SaveAndUploadSingleModule")]
        public Task<ServiceErrResponse> SaveAndUploadSingleModule(ModuleInBoxSingleModuleUploadDto dto);

        #endregion

        #region 自动涂胶

        [Post("/api/AutoGlue/SaveGlueDataAndUploadCATL")]
        public Task<ServiceErrResponse> SaveGlueDataAndUploadCATL(GlueDataDto dto);

        [Get("/api/AutoGlue/GetGlueRemainDuration")]
        public Task<uint> GetGlueDuration(string PackCode);

        #endregion

        #region 自动拧紧
        [Post("/api/AutoTighten/UploadData")]
        public Task<ServiceErrResponse> SaveAndUploadTightenData(AutoTightenDataUploadDto dto);
        #endregion

        #region 自动加压

        [Post("/api/AutoPressure/SaveAndUploadPressureData")]
        public Task<ServiceErrResponse> SaveAndUploadPressureData(PressureDataUploadDto dto);
        #endregion

        #region 下箱体涂胶

        [Post("/api/LowerBoxGlue/SaveDataAndUploadCATL")]
        public Task<ServiceErrResponse> SaveLowerBoxGlueDataAndUploadCATL(LowerBoxGlueDataDto dto);

        [Post("/api/LowerBoxGlue/UpdateDataAndUploadCATL")]
        public Task<ServiceErrResponse> UpdateDataAndUploadCATL(LowerBoxReGlueDataDto dto);

        [Get("/api/LowerBoxGlue/GetGlueTime")]
        public Task<DateTime> GetLowerBoxGlueTime(string packCode, string timeFlag);


        #endregion
    }
}
