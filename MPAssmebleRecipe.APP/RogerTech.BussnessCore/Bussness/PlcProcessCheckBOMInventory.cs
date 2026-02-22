using MiAssembleComponentsToSfcs.MiAssembleComponentsToSfcs;
using MiCheckBOMInventory.MiCheckBOMInventory;
using RogerTech.Common;
using RogerTech.Common.Models;
using RogerTech.Tool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.BussnessCore.Bussness
{
    public class PlcProcessCheckBOMInventory : PlcInProgressBase
    {
        MesInterface MesInterface;
        protected string StationName = ConfigurationManager.AppSettings["StationName"];
        public PlcProcessCheckBOMInventory(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
        {
            this.MesInterface = mesInterface;
        }


        public override void Excute(Group group)
        {

            base.Excute(group);
            Task.Run(() => { DbContext.Info("", $"收到plc请求值StartSignal：[0->1]", 0, PlcGroup.GroupName); });
            WriteFinishSignal(true);
            StringBuilder message = new StringBuilder();
            string sfc = string.Empty;
            //  var dbData = DbContext.GetInstance();
            BussnessUtility bussness = BussnessUtility.GetInstanse();
            string productId = " ";
            List<string> cellsns = new List<string>();
            int resultCode = 30001;
            try
            {
                #region Tag获取和数据校验
                Tag sfcT = group.GetTag("SFC");
                if (sfcT == null) OnTagNullError("SFC", group.GroupName);
                sfc = sfcT.Result.Value.ToString();
                if (sfc == null)
                {
                    message.Append($"绑定出站失败:传输的模组码为空");
                    WriteResult(resultCode);
                    return;
                }
                List<object> inputs = new List<object>();
                inputs.Add(sfc);
                List<checkBomInventoryData> datas = new List<checkBomInventoryData>();
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
                                // 修复逻辑：正确检查超出范围
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
                            datas.Add(new checkBomInventoryData()
                            {
                                inventory = item.Result.Value.ToString(),
                                qty = "1"
                            });
                            uploadDatas.Add(new UploadData()
                            {
                                InterfaceName = MesInterface.ToString(),
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
                            localDatas.Add(new UploadData()
                            {
                                InterfaceName = MesInterface.ToString(),
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
                List<object> output = bussness.MesInvoke(inputs, MesInterface);


                if (DbContext.GetInstance().Queryable<UploadData>().AS("UploadData").Where(p => p.SFC.Contains(productId)).Count() > 0)
                {
                    DbContext.GetInstance().Updateable<UploadData>()
                                           .AS("UploadData")
                                           .SetColumns(u => u.IsReupload == true)
                                           .Where(u => u.SFC == sfc)
                                           .ExecuteCommand();
                }

                DbContext.GetInstance().Insertable(uploadDatas).AS("UploadData").ExecuteCommand();
                DbContext.GetInstance().Insertable(localDatas).AS("LocalData").ExecuteCommand();
                if ((int)(output[0]) == 0)
                {
                    resultCode = (int)output[0];
                    message.Append("调用mes接口[CheckBOMInventory]校验成功");
                }
                else
                {
                    resultCode = (int)output[0];
                    message.Append($"调用mes接口[CheckBOMInventory]装配失败MES代码[{output[0]}] MES信息[{output[1]}]");
                    return;
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
                Task.Run(() => { DbContext.Info(productId, message.ToString(), resultCode, PlcGroup.GroupName); });
            }
        }
    }
}
