using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RogerTech.Common;
using RogerTech.Common.Models;
using RogerTech.Tool;

namespace RogerTech.BussnessCore.Bussness
{
    /// <summary>
    /// 自动拧紧收数
    /// </summary>
    public class PlcProcessTighteningDataCollectForSfcEx : PlcInProgressBase
    {
        private readonly string _serverAddress = ConfigurationManager.AppSettings["ServerAddress"];
        
        public PlcProcessTighteningDataCollectForSfcEx(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
        {
        }

        public override void Execute(Group group)
        {
            base.Execute(group);
            Task.Run(() => { DbContext.Info("", $"收到plc请求值StartSignal：[0->1]", 0, PlcGroup.GroupName); });
            WriteFinishSignal(true);
            StringBuilder message = new StringBuilder();
            string sfc = string.Empty;
            int resultCode = 30001;
            try
            {
                #region Tag获取和数据校验
                if (!TryGetRequiredStringTagValue(
                        group,
                        "SFC",
                        message,
                        ref resultCode,
                        30001,
                        30001,
                        "SFC变量读取异常",
                        "上传自动拧紧数据失败:传输的PACK码为空",
                        out sfc))
                {
                    WriteResult(resultCode);
                    return;
                }
                #endregion

                #region 自动拧紧站专用逻辑

                if (StationName.Contains("拧紧"))
                {
                    TighteningDataDto tighteningDataDto = BuildTighteningDataDto(group, sfc);
                
                    if (tighteningDataDto != null)
                    {
                        if (!UploadAutoTightenExternalData(tighteningDataDto, out string errorMessage))
                        {
                            resultCode = 30001;
                            message.Append(errorMessage);
                        }
                    }
                }
                
                #endregion
            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
            }
            finally
            {
                WriteResult(resultCode);
                WriteFinishSignal(false);
                Task.Run(() => { DbContext.Info(sfc, message.ToString(), resultCode, PlcGroup.GroupName); });
            }
        }

        #region 自动拧紧站专用方法

        /// <summary>
        /// 上传自动拧紧数据到服务端
        /// </summary>
        /// <param name="dto">自动拧紧数据</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns></returns>
        private bool UploadAutoTightenExternalData(TighteningDataDto dto, out string errorMessage)
        {
            errorMessage = null;
            
            if (string.IsNullOrWhiteSpace(_serverAddress))
            {
                errorMessage = "调用自动拧紧接口失败:未配置ServerAddress";
                return false;
            }

            string url = _serverAddress.TrimEnd('/') + "/api/AutoTighten/UploadExternalData";
            string json = JsonConvert.SerializeObject(new
            {
                sfc = dto.SFC,
                stationName = StationName,
                tighteningResultList = dto.TighteningResultList
            });

            /*
             * ToOptimize:HttpClient实例应在using语句外创建并复用，避免每次请求创建新实例。
             * 建议使用IHttpClientFactory进行依赖注入管理。当前实现可能导致端口耗尽和性能问题。
             */
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(5);
                using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    if ((int)response.StatusCode != 200)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        if (!string.IsNullOrWhiteSpace(responseBody) && responseBody.Length > 500)
                        {
                            responseBody = responseBody.Substring(0, 500);
                        }
                        errorMessage = $"调用自动拧紧接口失败:HTTP{(int)response.StatusCode} {responseBody}";
                        return false;
                    }
                    return true;
                }
            }
        }
        
        
        /// <summary>
        /// 从点位组中提取拧紧相关标签（XXX[索引].字段），按索引聚合为 TighteningResult 列表
        /// </summary>
        /// <param name="group"></param>
        /// <param name="sfc"></param>
        /// <returns></returns>
        private TighteningDataDto BuildTighteningDataDto(Group group, string sfc)
        {
            TighteningDataDto dto = new TighteningDataDto
            {
                SFC = sfc,
                TighteningResultList = new List<TighteningResult>()
            };

            // 用字典按索引进行聚合
            Dictionary<short, TighteningResult> resultsByIndex = new Dictionary<short, TighteningResult>();

            foreach (var tag in group.Tags)
            {
                if (tag == null)
                {
                    continue;
                }

                // 只处理符合 “XXX[索引].字段” 且字段在白名单内的标签
                if (!TryParseTighteningTag(tag.TagName, out short index, out string field))
                {
                    continue;
                }

                // 获取/创建该索引对应的聚合对象
                if (!resultsByIndex.TryGetValue(index, out TighteningResult result))
                {
                    result = new TighteningResult
                    {
                        Index = index
                    };
                    resultsByIndex[index] = result;
                }

                object valueObj = tag.Result?.Value;
                switch (field)
                {
                    case "ResultOK":
                        result.ResultOK = ParseShortOrDefault(valueObj);
                        break;
                    case "OrderNo":
                        result.OrderNo = ParseShortOrDefault(valueObj);
                        break;
                    case "ProgramNo":
                        result.ProgramNo = ParseShortOrDefault(valueObj);
                        break;
                    case "TorqueResult":
                        result.TorqueResult = new MesMeasuredValue
                        {
                            TagName = tag.TagName,
                            TagValue = valueObj?.ToString(),
                            MesName = tag.MesName,
                            MesDataType = tag.MesDataType
                        };
                        break;
                    case "AngleResult":
                        result.AngleResult = new MesMeasuredValue
                        {
                            TagName = tag.TagName,
                            TagValue = valueObj?.ToString(),
                            MesName = tag.MesName,
                            MesDataType = tag.MesDataType
                        };
                        break;
                }
            }

            // 按索引排序，过滤全空记录
            foreach (var item in resultsByIndex.OrderBy(kvp => kvp.Key))
            {
                TighteningResult result = item.Value;
                if (result.TorqueResult == null && result.AngleResult == null && result.ResultOK == 0 && result.OrderNo == 0 && result.ProgramNo == 0)
                {
                    continue;
                }
                dto.TighteningResultList.Add(result);
            }

            return dto;
        }

        /// <summary>
        /// 解析拧紧结果标签名称，提取索引和字段
        /// </summary>
        /// <param name="tagName">标签名称</param>
        /// <param name="index">拧紧序号: 例如 XXX[?].TorqueResult 中的 ?</param>
        /// <param name="field">字段名称: 例如 XXX[?].TorqueResult 中的 TorqueResult</param>
        /// <returns>是否成功</returns>
        private static bool TryParseTighteningTag(string tagName, out short index, out string field)
        {
            index = 0;
            field = null;

            if (string.IsNullOrWhiteSpace(tagName))
            {
                return false;
            }

            // 找到 '[' 和 ']'，用于提取索引：XXX[索引].字段
            int openBracket = tagName.IndexOf('[');
            int closeBracket = tagName.IndexOf(']');
            // 校验：必须存在前缀、[]中必须有内容、且 '].' 后必须还有字段名
            if (openBracket <= 0 || closeBracket <= openBracket + 1 || closeBracket + 2 >= tagName.Length)
            {
                return false;
            }

            // 校验：']' 后面必须紧跟 '.'
            if (tagName[closeBracket + 1] != '.')
            {
                return false;
            }

            // 解析索引：提取 '[' 与 ']' 之间的内容并转换为 short
            string indexText = tagName.Substring(openBracket + 1, closeBracket - openBracket - 1);
            if (!short.TryParse(indexText, out index))
            {
                return false;
            }

            // 解析字段名：提取 '].' 后面的部分
            field = tagName.Substring(closeBracket + 2);
            return field == "ResultOK" || field == "OrderNo" || field == "ProgramNo" || field == "TorqueResult" || field == "AngleResult";
        }

        private static short ParseShortOrDefault(object valueObj)
        {
            if (valueObj == null)
            {
                return 0;
            }
            return short.TryParse(valueObj.ToString(), out short value) ? value : (short)0;
        }

        #endregion
        
    }
}
