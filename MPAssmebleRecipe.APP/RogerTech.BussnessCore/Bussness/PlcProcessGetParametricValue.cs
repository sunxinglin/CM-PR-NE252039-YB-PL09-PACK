using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using GetParametricValue.GetParametricValue;
using RogerTech.Common;
using RogerTech.Common.Models;
using RogerTech.Tool;

namespace RogerTech.BussnessCore.Bussness
{
    /// <summary>
    /// 获取参数
    /// </summary>
    public class PlcProcessGetParametricValue : PlcInProgressBase
    {
        private readonly MesInterface _mesInterface;
        private readonly string GetParametricValueUpMesCode = ConfigurationManager.AppSettings["GetParametricValueUpMesCode"];
        public PlcProcessGetParametricValue(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
        {
            _mesInterface= mesInterface;
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
                        "绑定出站失败:传输的模组码为空",
                        out sfc))
                {
                    WriteResult(resultCode);
                    return;
                }
                List<object> inputs = new List<object> { sfc };
                var parameter = new List<GetParametricValueRequestData>();
                switch (StationName)
                {
                    case "下箱体涂胶":
                        for (int i = 1; i < 109; i++)
                        {
                            parameter.Add(new GetParametricValueRequestData { parameter = GetParametricValueUpMesCode + i, parameterDec = string.Empty });
                        }
                        break;
                    case "模组入箱":
                        parameter.Add(new GetParametricValueRequestData { parameter = GetParametricValueUpMesCode, parameterDec = string.Empty });
                        break;
                    case "压条自动加压和拧紧":
                        parameter.Add(new GetParametricValueRequestData { parameter = GetParametricValueUpMesCode, parameterDec = string.Empty });
                        break;
                    default:
                        break;
                }
                inputs.Add(parameter);

                //空循环模式
                if (business.bMesSimulation)
                {
                    resultCode = 0;
                    return;
                }

                List<UploadData> uploadDatas = new List<UploadData>();
                List<UploadData> localDatas = new List<UploadData>();

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
                if ((int)output[0] == 0)
                {
                    var dt = DateTime.Now;


                    resultCode = (int)output[0];
                    PlcGroup.GetTag("Year").WriteValue(dt.Year);
                    PlcGroup.GetTag("Month").WriteValue(dt.Month);
                    PlcGroup.GetTag("Day").WriteValue(dt.Day);
                    PlcGroup.GetTag("Hour").WriteValue(dt.Hour);
                    PlcGroup.GetTag("Minute").WriteValue(dt.Minute);
                    PlcGroup.GetTag("Seconds").WriteValue(dt.Second);
                    WriteResult((object[])output[3], GetParametricValueUpMesCode);
                    message.Append("调用mes接口[GetParametricValue]成功");
                }
                else
                {
                    resultCode = (int)output[0];
                    message.Append($"调用mes接口[GetParametricValue]失败MES代码[{output[0]}] MES信息[{output[1]}]");
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
