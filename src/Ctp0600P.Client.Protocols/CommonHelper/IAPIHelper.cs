using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.Protocols;

public interface IAPIHelper
{
    public Task<List<Base_ProResource>> LoadStationProResourceConfig(ProResourceTypeEnum type);

    public Task<Response<ClaimEntity>> CheckClaimWithAPI(string account, string password);

    /// <summary>
    /// 获取当前工位停靠的AGV信息
    /// </summary>
    /// <returns></returns>
    public Task<Response<Proc_AGVStatus>> LoadStationCurrentAGV();
    public Task<PackTaskMainDataDTO> LoadStepPackTaskMainData(DateTime searchBeginTime, DateTime searchEndTime, string stationCode);
    public Task<Response<User>> LoginWithAPI(string account, string password);
}