using AsZero.Core.Services.Repos;

using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Yee.Services.AGV
{
    public interface IAGVResolver
    {
        /// <summary>
        /// 校验AGV
        /// </summary>
        /// <param name="object">涂胶对象</param>
        /// <returns></returns>
        Task<bool> CheckAgvBinding(int agvNo, string packPN);

        Task<Response<StationPack_AGV_Task_Record_DTO>> CheckPack_AGV_Task_Record_Auto(string stepCode, string agvNo, string packPN);
        Task<Response<StationPack_AGV_Task_Record_DTO>> CheckPack_AGV_Task_Record_Auto(string stepCode, string stationCode, string agvNo, string packPN, bool askIn = false);
        Task<Response<Base_AutoStationTaskTighten>> SelectBoltUseNum(string stationCode, string packPN);
    }
    
}
