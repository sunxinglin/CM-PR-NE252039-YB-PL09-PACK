using System.Threading.Tasks;

using AsZero.Core.Services.Messages;

using Ctp0600P.Shared.NotificationDTO;

namespace Ctp0600P.Shared
{
    public interface ICommonClient
    {

        /// <summary>
        /// UI Log
        /// </summary>
        /// <param name="alarmmsg"></param>
        /// <returns></returns>
        Task ReceiveUILogMessageNotification(LogMessage alarmmsg);
        /// <summary>
        /// UI Log
        /// </summary>
        /// <param name="alarmmsg"></param>
        /// <returns></returns>
        Task ReceiveUILogAlarmNotification(AlarmMessage alarmmsg);
    }


    public interface IAllMessageHubClient : ICommonClient
    {
        /// <summary>
        /// agv消息
        /// </summary>
        /// <param name="action">1：进站 2：离站</param>
        /// <param name="agvNo">小车号</param>
        /// <param name="stationCode">工位</param>
        /// <returns></returns>
        Task AgvActionMsg(AgvMsgContextNotification dto);
    }
}
