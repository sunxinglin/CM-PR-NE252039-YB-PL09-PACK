using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RogerTech.Common;
using RogerTech.Common.Models;
using RogerTech.Tool;

namespace RogerTech.BussnessCore.Bussness
{
    public class PlcProcessFindSfcByInventory : PlcInProgressBase
    {
        private readonly MesInterface _mesInterface;

        public PlcProcessFindSfcByInventory(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
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
            string ModuleCode = string.Empty;
            string ModulePN = string.Empty;
            try
            {
                #region Tag获取和数据校验
                Tag moduleCode = group.GetTag("Code");
                if (moduleCode == null) OnTagNullError("Code", group.GroupName);

                List<object> inputs = new List<object>();
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
                            inputs.Add(item.Result.Value.ToString());
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

                //空循环模式
                if (business.bMesSimulation)
                {
                    resultCode = 0;
                    return;
                }

                List<object> output = business.MesInvoke(inputs, _mesInterface);

                if (db.Queryable<UploadData>().AS("UploadData").Where(p => p.SFC.Contains(sfc)).Any())
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
                    ModuleCode = output[3].ToString();
                    ModulePN = output[4].ToString();
                    WriteResult(output[2].ToString(), moduleCode.TagName);
                    message.Append($"调用mes接口[FindSfcByInventory]获取物料数据{output[2].ToString()}成功");
                }
                else
                {
                    resultCode = (int)output[0];
                    message.Append($"调用mes接口[FindSfcByInventory]获取物料数据失败MES代码[{output[0]}] MES信息[{output[1]}]");
                }
                #endregion
            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
            }
            finally
            {
                PlcGroup.GetTag("ModuleCode").WriteValue(ModuleCode);
                PlcGroup.GetTag("ModulePN").WriteValue(ModulePN);
                WriteResult(resultCode);
                WriteFinishSignal(false);
                Task.Run(() => { DbContext.Info(sfc, message.ToString(), resultCode, PlcGroup.GroupName); });
            }
        }
    }
}
