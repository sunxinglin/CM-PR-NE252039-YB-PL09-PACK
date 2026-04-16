using System.Threading.Tasks;
using RogerTech.Share.Dto;

namespace RogerTech.Share.Interfaces.Services.IApi
{
    /// <summary>
    /// 设备状态同步服务接口
    /// </summary>
    public interface IDeviceStatusSyncService
    {
        /// <summary>
        /// 同步设备状态到RMS（通用方法）
        /// </summary>
        /// <param name="deviceNo">设备编号</param>
        /// <param name="deviceStatus">设备状态字符串</param>
        /// <param name="deviceDescription">设备描述</param>
        /// <returns>同步结果</returns>
        Task<DeviceStatusSyncResult> SyncDeviceStatusAsync(string deviceNo, string deviceStatus, string deviceDescription);

        /// <summary>
        /// 切换设备到空闲状态
        /// </summary>
        Task<DeviceStatusSyncResult> SwitchToIdleAsync(string deviceNo);

        /// <summary>
        /// 切换设备到工作中状态
        /// </summary>
        Task<DeviceStatusSyncResult> SwitchToWorkingAsync(string deviceNo);

        /// <summary>
        /// 切换设备到故障状态
        /// </summary>
        Task<DeviceStatusSyncResult> SwitchToFaultAsync(string deviceNo);

        /// <summary>
        /// 切换设备到下线状态
        /// </summary>
        Task<DeviceStatusSyncResult> SwitchToOfflineAsync(string deviceNo);
    }

    /// <summary>
    /// 设备状态同步结果
    /// </summary>
    public class DeviceStatusSyncResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 结果代码（0表示成功）
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 结果消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 创建成功结果
        /// </summary>
        public static DeviceStatusSyncResult CreateSuccess(string message)
        {
            return new DeviceStatusSyncResult
            {
                IsSuccess = true,
                Code = 0,
                Message = message
            };
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        public static DeviceStatusSyncResult CreateFail(int code, string message)
        {
            return new DeviceStatusSyncResult
            {
                IsSuccess = false,
                Code = code,
                Message = message
            };
        }
    }
}
