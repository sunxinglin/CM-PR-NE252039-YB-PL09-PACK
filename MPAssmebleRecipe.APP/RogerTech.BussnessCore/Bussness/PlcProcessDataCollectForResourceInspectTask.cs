using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataCollectForResourceInspectTask.DataCollectForResourceInspectTask;
using RogerTech.Common;
using RogerTech.Common.Models;
using RogerTech.Tool;
using ParameterDataType = DataCollectForResourceInspectTask.DataCollectForResourceInspectTask.ParameterDataType;

namespace RogerTech.BussnessCore.Bussness
{
    /// <summary>
    /// 首件
    /// </summary>
    public class PlcProcessDataCollectForResourceInspectTask : PlcInProgressBase
    {
        private readonly MesInterface _mesInterface;
        
        public PlcProcessDataCollectForResourceInspectTask(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
        {
            _mesInterface = mesInterface;
        }

        public override void Execute(Group group)
        {
            base.Execute(group);
            Task.Run(() => { DbContext.Info("", $"收到plc请求值StartSignal：[0->1]", 0, PlcGroup.GroupName); });
            WriteFinishSignal(true);
            StringBuilder message = new StringBuilder();
            string sfc = string.Empty;
            var db = DbContext.GetInstance();
            BussnessUtility business = BussnessUtility.GetInstance();

            int resultCode = 30001;
            try
            {
                #region Tag获取和数据校验                
                List<object> inputs = new List<object>();

                List<machineIntegrationParametricData> datas = new List<machineIntegrationParametricData>();
                List<UploadData> uploadDatas = new List<UploadData>();
                List<UploadData> localDatas = new List<UploadData>();
                foreach (var item in group.Tags)
                {
                    if (item.Result.Value != null)
                    {
                        // 安全上下限检查
                        if (item.IsChecked)
                        {
                            if (float.TryParse(item.Result.Value.ToString(), out float value))
                            {
                                if (value < item.LowerLimit || value > item.UpperLimit)
                                {
                                    message.Append($"上传失败:[{item.TagName}]超出上下限限制[{item.LowerLimit}-{item.UpperLimit}]");
                                    WriteResult(30001);
                                    return;
                                }
                            }
                            else
                            {
                                message.Append($"上传失败:[{item.TagName}]值格式错误");
                                WriteResult(30001);
                                return;
                            }
                        }
                        // 根据 是否上传至工厂MES 本地数据存储
                        if (item.IsUpload)
                        {
                            // 打包工厂MES上传数据
                            datas.Add(new machineIntegrationParametricData
                            {
                                name = item.MesName,
                                value = item.Result.Value.ToString(),
                                dataType = (ParameterDataType)item.MesDataType
                            });
                            // 存储数据至uploadDatas表
                            uploadDatas.Add(new UploadData
                            {
                                InterfaceName = _mesInterface.ToString(),
                                StationName = StationName,
                                UploadDataName = item.MesName,
                                UploadDataType = item.MesDataType,
                                UploadDataValue = item.Result.Value.ToString(),
                                Time = DateTime.Now,
                                IsReupload = false,
                                SFC = sfc
                            });
                        }
                        else
                        {
                            // 存储数据至localDatas表
                            localDatas.Add(new UploadData
                            {
                                InterfaceName = _mesInterface.ToString(),
                                StationName = StationName,
                                UploadDataName = item.MesName,
                                UploadDataType = item.MesDataType,
                                UploadDataValue = item.Result.Value.ToString(),
                                Time = DateTime.Now,
                                IsReupload = false,
                                SFC = sfc
                            });
                        }
                    }
                    else
                    {
                        message.Append($"上传失败:[{item.TagName}]变量读取异常");
                        WriteResult(30001);
                        return;
                    }
                }
                inputs.Add(datas);

                //空循环模式
                if (business.bMesSimulation)
                {
                    resultCode = 0;
                    return;
                }

                List<object> output = business.MesInvoke(inputs, _mesInterface);

                // 若数据已存在，则标记为重新上传
                if (db.Queryable<UploadData>().AS("UploadData").Where(p => p.SFC == sfc).Any())
                {
                    db.Updateable<UploadData>()
                        .AS("UploadData")
                        .SetColumns(u => u.IsReupload == true)
                        .Where(u => u.SFC == sfc)
                        .ExecuteCommand();
                }

                db.Insertable(uploadDatas).AS("UploadData").ExecuteCommand();
                db.Insertable(localDatas).AS("LocalData").ExecuteCommand();
                if ((int)output[0] == 0)
                {
                    resultCode = (int)output[0];
                    message.Append("调用mes接口[DataCollectForResourceInspectTask]上传首件数据成功");
                }
                else
                {
                    resultCode = (int)output[0];
                    message.Append($"调用mes接口[DataCollectForResourceInspectTask]上传首件数据失败MES代码[{output[0]}] MES信息[{output[1]}]");
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
                Task.Run(() => { DbContext.Info("", message.ToString(), resultCode, PlcGroup.GroupName); });
            }
        }
    }
}