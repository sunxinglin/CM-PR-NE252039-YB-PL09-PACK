using AsZero.Core.Services.Messages;

namespace Yee.Services.Hubs
{
    public interface ICommonClient
    {

        /// <summary>
        /// UI Log
        /// </summary>
        /// <param name="logmsg"></param>
        /// <returns></returns>
        Task ReceiveUILogMessageNotification(LogMessage logmsg);
        /// <summary>
        /// UI Log
        /// </summary>
        /// <param name="alarmmsg"></param>
        /// <returns></returns>
        Task ReceiveUILogAlarmNotification(AlarmMessage alarmmsg);
    }

}
