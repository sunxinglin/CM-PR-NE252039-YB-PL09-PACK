using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MPAssmebleRecipe.Logger;
using MPAssmebleRecipe.Logger.Interfaces;
using RogerTech.Share.Dto;
using RogerTech.Share.Interfaces.Infrastructure;
using RogerTech.Share.Interfaces.Services.IApi;

namespace RogerTech.Services
{
    /// <summary>
    /// 拧紧数据服务（拧紧数据上传与管理）
    /// 实现新接口，使用依赖注入的 IApiHelper
    /// </summary>
    public class TightenDataService : ITightenDataService
    {
        private readonly ILoggerHelper _logger;
        private readonly IApiHelper _apiHelper;

        // 无参构造函数（向后兼容，用于非IoC场景）
        public TightenDataService()
        {
            _logger = NLogManager.GetLogger("TightenDataService");
            // 注意：无参构造函数无法注入 IApiHelper，这种情况下应该避免使用
            _logger.Warn("[TightenDataService] 使用无参构造函数，IApiHelper 未注入，可能导致功能异常");
        }

        // IoC依赖注入构造函数（推荐）
        public TightenDataService(IApiHelper apiHelper, ILoggerHelper logger = null)
        {
            _logger = logger ?? NLogManager.GetLogger("TightenDataService");
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
        }

        #region 拧紧数据处理方法

        /// <summary>
        /// 上传拧紧数据到RMS
        /// </summary>
        /// <param name="tightenDataList">拧紧数据列表</param>
        /// <returns>上传结果</returns>
        public async Task<TightenDataUploadResult> UploadTightenDataAsync(List<TightenDataDto> tightenDataList)
        {
            try
            {
                if (tightenDataList == null || tightenDataList.Count == 0)
                {
                    _logger.Warn($"[TightenDataService] 拧紧数据列表为空");
                    return TightenDataUploadResult.CreateFail(40001, "拧紧数据列表为空");
                }

                var sfc = tightenDataList[0].SFC;
                _logger.Info($"[TightenDataService] 开始上传拧紧数据 - SFC: {sfc}, 数量: {tightenDataList.Count}");

                // 构建请求DTO
                var requestDto = new TightenDataRequestDto
                {
                    SFC = sfc,
                    TightenDataList = tightenDataList
                };

                // 调用RMS API上传拧紧数据（使用注入的 IApiHelper）
                var response = await _apiHelper.XY.PostAsync<TightenDataRequestDto, ApiResponse<object>>(
                    "/admin-api/external/tightenData",
                    requestDto
                );

                if (response.IsSuccess)
                {
                    _logger.Info($"[TightenDataService] 拧紧数据上传成功 - SFC: {sfc}");
                    return TightenDataUploadResult.CreateSuccess($"拧紧数据上传成功，SFC：{sfc}，螺丝数量：{tightenDataList.Count}");
                }
                else
                {
                    _logger.Error($"[TightenDataService] 拧紧数据上传失败 - SFC: {sfc}, 错误码: {response.Code}, 错误信息: {response.Msg}");
                    return TightenDataUploadResult.CreateFail(
                        response.Code,
                        $"拧紧数据上传失败，错误码：{response.Code}，错误信息：{response.Msg}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[TightenDataService] 拧紧数据上传异常: {ex.Message}");
                return TightenDataUploadResult.CreateFail(30001, $"调用RMS接口异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 保存并发送拧紧数据（本地保存 + RMS上传）
        /// </summary>
        /// <param name="tightenDataList">拧紧数据列表</param>
        /// <returns>处理结果</returns>
        public async Task<TightenDataUploadResult> SaveAndSendTightenDataAsync(List<TightenDataDto> tightenDataList)
        {
            try
            {
                // TODO: 根据实际业务需求，先保存到本地数据库
                // DbContext.GetInstance().Insertable(tightenDataList).ExecuteCommand();

                // 上传到RMS
                return await UploadTightenDataAsync(tightenDataList);
            }
            catch (Exception ex)
            {
                _logger.Error($"[TightenDataService] 保存并发送拧紧数据异常: {ex.Message}");
                return TightenDataUploadResult.CreateFail(30002, $"保存并发送拧紧数据异常：{ex.Message}");
            }
        }

        #endregion
    }
}