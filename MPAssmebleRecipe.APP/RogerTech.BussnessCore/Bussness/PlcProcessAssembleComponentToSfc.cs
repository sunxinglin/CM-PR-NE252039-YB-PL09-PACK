using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MiAssembleComponentsToSfcs.MiAssembleComponentsToSfcs;
using RogerTech.Common;
using RogerTech.Common.Models;
using RogerTech.Tool;

namespace RogerTech.BussnessCore.Bussness
{
    /// <summary>
    /// 组装物料
    /// </summary>
    internal class PlcProcessAssembleComponentToSfc : PlcInProgressBase
    {
        private readonly MesInterface _mesInterface;
        public PlcProcessAssembleComponentToSfc(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
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
                if (!TryGetRequiredStringTagValue(
                        group,
                        "SFC",
                        message,
                        ref resultCode,
                        30001,
                        30001,
                        "SFC变量读取异常",
                       "组装物料失败:传输的SFC为空",
                        out sfc))
                {
                    WriteResult(resultCode);
                    return;
                }
                List<object> inputs = new List<object> { sfc };
                List<inventoryData> datas = new List<inventoryData>();
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
                        if (item.IsUpload)
                        {
                            datas.Add(new inventoryData
                            {
                                inventory = item.Result.Value.ToString(),
                                qty = "1"
                            });
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
                if ((int)(output[0]) == 0)
                {
                    resultCode = (int)output[0];
                    message.Append("调用mes接口[AssembleComponentsToSfc]装配成功");
                }
                else
                {
                    resultCode = (int)output[0];
                    message.Append($"调用mes接口[AssembleComponentsToSfc]装配失败MES代码[{output[0]}] MES信息[{output[1]}]");
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
    }
}
