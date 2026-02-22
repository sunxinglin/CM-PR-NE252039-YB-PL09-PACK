using RogerTech.Common;
using RogerTech.Common.Models;
using RogerTech.Tool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.BussnessCore.Bussness
{
    public class PlcProcessReleaseSfcByShopOrder : PlcInProgressBase
    {
        MesInterface MesInterface;
        protected string StationName = ConfigurationManager.AppSettings["StationName"];
        public PlcProcessReleaseSfcByShopOrder(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
        {
            this.MesInterface= mesInterface;
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
                
                List<object> inputs = new List<object>();
       
                List<UploadData> uploadDatas = new List<UploadData>();
                List<UploadData> localDatas = new List<UploadData>();

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
                    if (output[2] as System.Collections.IEnumerable != null)
                    {
                        foreach (var item in output[2] as System.Collections.IEnumerable)
                        {
                            WriteResult(item.ToString());
                            message.Append($"调用mes接口[ReleaseSfcByShopOrder]释放条码[{item}]成功");
                        }
                    }
                }
                else
                {
                    resultCode = (int)output[0];
                    message.Append($"调用mes接口[ReleaseSfcByShopOrder]释放条码失败MES代码[{output[0]}] MES信息[{output[1]}]");
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
