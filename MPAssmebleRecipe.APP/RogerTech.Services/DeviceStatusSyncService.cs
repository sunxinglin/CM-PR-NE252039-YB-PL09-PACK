using System;
using System.Threading.Tasks;
using MPAssmebleRecipe.Logger;
using MPAssmebleRecipe.Logger.Interfaces;
using RogerTech.Share.Dto;
using RogerTech.Share.Interfaces.Infrastructure;
using RogerTech.Share.Interfaces.Services.IApi;
using RogerTech.Tool;

namespace RogerTech.Services
{
    /// <summary>
    /// 设备状态同步服务（RMS设备状态管理）
    /// 实现IDeviceStatusSyncService接口，支持依赖注入
    /// </summary>
    public class DeviceStatusSyncService : IDeviceStatusSyncService
    {
        private readonly ILoggerHelper _logger;
        private readonly IApiHelper _apiHelper;

        /// <summary>
        /// IoC依赖注入构造函数
        /// </summary>
        public DeviceStatusSyncService(IApiHelper apiHelper)
        {
            _logger = NLogManager.GetLogger("DeviceStatusSyncService");
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
        }

        #region 设备状态同步方法

        /// <summary>
        /// 同步设备状态到RMS（通用方法）
        /// </summary>
        /// <param name="deviceNo">设备编号</param>
        /// <param name="deviceStatus">设备状态（使用DeviceStatus枚举的ToStatusString()转换）</param>
        /// <param name="deviceDescription">设备描述</param>
        /// <returns>同步结果</returns>
        public async Task<DeviceStatusSyncResult> SyncDeviceStatusAsync(
            string deviceNo, 
            string deviceStatus, 
            string deviceDescription)
        {
            try
            {
                _logger.Info($"[DeviceStatusSyncService] 开始同步设备状态 - 设备: {deviceNo}, 状态: {deviceStatus}");

                // 构建RMS设备状态同步请求
                var requestDto = new DeviceSyncRequestDto
                {
                    DeviceNo = deviceNo,
                    DeviceStatus = deviceStatus,
                    DeviceDescription = deviceDescription,
                    IsDel = "N"
                };

                // 使用注入的IApiHelper调用RMS API同步设备状态
                var response = await _apiHelper.RMS.PostAsync<DeviceSyncRequestDto, ApiResponse<object>>(
                    "/admin-api/external/deviceSync",
                    requestDto
                );

                if (response.IsSuccess)
                {
                    _logger.Info($"[DeviceStatusSyncService] 设备状态同步成功 - 设备: {deviceNo}, 状态: {deviceStatus}");
                    return DeviceStatusSyncResult.CreateSuccess($"设备状态切换到{deviceDescription}成功，设备号：{deviceNo}");
                }
                else
                {
                    _logger.Error($"[DeviceStatusSyncService] 设备状态同步失败 - 设备: {deviceNo}, 错误码: {response.Code}, 错误信息: {response.Msg}");
                    return DeviceStatusSyncResult.CreateFail(
                        response.Code, 
                        $"设备状态切换到{deviceDescription}失败，错误码：{response.Code}，错误信息：{response.Msg}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[DeviceStatusSyncService] 设备状态同步异常 - 设备: {deviceNo}, 异常: {ex.Message}");
                return DeviceStatusSyncResult.CreateFail(30001, $"调用RMS接口异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 切换设备到空闲状态
        /// </summary>
        public async Task<DeviceStatusSyncResult> SwitchToIdleAsync(string deviceNo)
        {
            return await SyncDeviceStatusAsync(deviceNo, DeviceStatus.Idle.ToStatusString(), "设备空闲");
        }

        /// <summary>
        /// 切换设备到工作中状态
        /// </summary>
        public async Task<DeviceStatusSyncResult> SwitchToWorkingAsync(string deviceNo)
        {
            return await SyncDeviceStatusAsync(deviceNo, DeviceStatus.Working.ToStatusString(), "设备工作中");
        }

        /// <summary>
        /// 切换设备到故障状态
        /// </summary>
        public async Task<DeviceStatusSyncResult> SwitchToFaultAsync(string deviceNo)
        {
            return await SyncDeviceStatusAsync(deviceNo, DeviceStatus.Fault.ToStatusString(), "设备故障");
        }

        /// <summary>
        /// 切换设备到下线状态
        /// </summary>
        public async Task<DeviceStatusSyncResult> SwitchToOfflineAsync(string deviceNo)
        {
            return await SyncDeviceStatusAsync(deviceNo, DeviceStatus.Offline.ToStatusString(), "设备下线");
        }

        #endregion
    }
}
