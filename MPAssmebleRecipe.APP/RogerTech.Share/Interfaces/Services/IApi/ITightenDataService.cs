using System.Collections.Generic;
using System.Threading.Tasks;
using RogerTech.Share.Dto;

namespace RogerTech.Share.Interfaces.Services.IApi
{
    /// <summary>
    /// 拧紧数据服务接口
    /// </summary>
    public interface ITightenDataService
    {
        /// <summary>
        /// 上传拧紧数据到RMS
        /// </summary>
        /// <param name="tightenDataList">拧紧数据列表</param>
        /// <returns>上传结果</returns>
        Task<TightenDataUploadResult> UploadTightenDataAsync(List<TightenDataDto> tightenDataList);

        /// <summary>
        /// 保存并发送拧紧数据（本地保存 + RMS上传）
        /// </summary>
        /// <param name="tightenDataList">拧紧数据列表</param>
        /// <returns>处理结果</returns>
        Task<TightenDataUploadResult> SaveAndSendTightenDataAsync(List<TightenDataDto> tightenDataList);
    }

    /// <summary>
    /// 拧紧数据上传结果
    /// </summary>
    public class TightenDataUploadResult
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
        public static TightenDataUploadResult CreateSuccess(string message)
        {
            return new TightenDataUploadResult
            {
                IsSuccess = true,
                Code = 0,
                Message = message
            };
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        public static TightenDataUploadResult CreateFail(int code, string message)
        {
            return new TightenDataUploadResult
            {
                IsSuccess = false,
                Code = code,
                Message = message
            };
        }
    }
}
