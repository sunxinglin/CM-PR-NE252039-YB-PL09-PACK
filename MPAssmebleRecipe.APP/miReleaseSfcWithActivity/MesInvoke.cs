using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using miReleaseSfcWithActivity.miReleaseSfcWithActivity;
using System.Net;
using System.Configuration;
using System.Threading;

namespace miReleaseSfcWithActivity
{
    public class MesInvoke:MesInvokeBase
    {
        public string SectionNamePath;
        public StringBuilder Strb_Log { get; private set; }
        public MesInvoke(string configfile, string SectionName, string interfaceName, string paramsconigfile = "") : base(configfile, SectionName, interfaceName, paramsconigfile)
        {
            Model = new MesDataModel(configfile, SectionName);
            TxtFile.ExistsPath(SectionName);
            SectionNamePath = SectionName;
        }
        public override List<object> GetResult(List<object> objs)
        {
            string modCode = string.Empty;
            Strb_Log = new StringBuilder();
            int qty = 0;
            if (objs != null && objs.Count == 1)
            {
                qty = int.Parse(objs[0].ToString());
            }
            else
            {
                txtFile.WriteFile(SectionNamePath,"Invalid input params");
                return null;
            }
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            MiReleaseSfcWithActivityServiceService webservice = new MiReleaseSfcWithActivityServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;

            releaseSfcWithActivityRequest Requst = new releaseSfcWithActivityRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;
            Requst.resource = model.ResourceField;
            Requst.user = model.UserField;
            Requst.activity = model.ActivityField;
            Requst.isCarrierType = model.IsCarrierTypeField;
            Requst.modeProcessSFC = model.ReleaseSfcWithActivityModeProcessSFCField;

            Requst.sfcQty = qty;
            miReleaseSfcWithActivity.miReleaseSfcWithActivity Requst1 = new miReleaseSfcWithActivity.miReleaseSfcWithActivity();
            Requst1.ReleaseSfcWithActivityRequest = Requst;
            int retCode = 111;
            string errMsg = string.Empty;
            List<string> sfcs = new List<string>();
            for (int i = 0; i < qty; i++)
            {
                sfcs.Add(string.Empty);
            }
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("sfcQty:").Append(",").Append(Requst.sfcQty).Append("\r\n");



                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");

                miReleaseSfcWithActivity.miReleaseSfcWithActivityResponse response = webservice.miReleaseSfcWithActivity(Requst1);

                Strb_Log.Append("返回参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(response).Replace(",", ";")).Append("\r\n");

                DateTime dtEndTime = DateTime.Now;
                TimeSpan span = dtEndTime - dtStartTime;
                string WasteTime = span.TotalMilliseconds.ToString();
                Strb_Log.Append(dtEndTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("调用结束").Append("\r\n");
                Strb_Log.Append("耗时:").Append(",").Append(WasteTime).Append("ms").Append("\r\n");
                if (response != null)
                {
                    //ok
                    if (response.@return != null)
                    {
                        retCode = response.@return.code;
                        //当返回代码大于0时查询
                        if (retCode > 0)
                        {
                            if (string.IsNullOrEmpty(errMsg))
                            {
                                errMsg = response.@return.message;
                            }
                            else
                            {
                                //将中英文均显示
                                errMsg += "\r\n";
                                errMsg += response.@return.message;
                            }
                        }
                        else
                        {
                            //modCode = response.@return.sfcArray[0].sfc;
                            for (int i = 0; i < qty; i++)
                            {
                                sfcs[i] = response.@return.sfcArray[i].sfc;
                                Strb_Log.Append("模组号:").Append(sfcs[i]).Append(",");
                            }
                            Strb_Log.Append("\r\n");
                            errMsg = "释放模组号成功";
                        }

                    }//end if (response.@return)
                    else
                    {
                        retCode = 201;
                        errMsg = "接口response.@return返回值为空";
                    }
                }
                else
                {
                    retCode = 211;
                    errMsg = "接口返回值为空";
                }
            }
            catch (WebException ex)
            {
                retCode = 221;
                errMsg = ex.Message;
                
            }
            catch (Exception ex)
            {
                retCode = 231;
                errMsg = ex.Message;
            }
            finally
            {
                Strb_Log.Append("返回代码").Append(",").Append(retCode).Append(",").Append("返回信息").Append(",").Append(errMsg).Append("\r\n");
             
                ReturnResult.Add(retCode);
                ReturnResult.Add(errMsg);
                ReturnResult.Add(sfcs);
            }
            try
            {
                txtFile.WriteFile(SectionNamePath,Strb_Log.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ReturnResult;
        }

        public override List<object> GetResultSimulation(List<object> objs)
        {
            Strb_Log = new StringBuilder();
            int qty = 1;
            if (objs != null && objs.Count == 1)
            {
                qty = int.Parse(objs[0].ToString());
            }
            else
            {
                txtFile.WriteFile(SectionNamePath,"Invalid input params");
                return null;
            }
            qty = 1;
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            List<string> sfcs = new List<string>();
            for (int i = 0; i < qty; i++)
            {
                sfcs.Add(string.Empty);
            }

            releaseSfcWithActivityRequest Requst = new releaseSfcWithActivityRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;
            Requst.resource = model.ResourceField;
            Requst.user = model.UserField;
            Requst.activity = model.ActivityField;
            Requst.isCarrierType = model.IsCarrierTypeField;
            Requst.modeProcessSFC = model.ReleaseSfcWithActivityModeProcessSFCField;

            Requst.sfcQty = qty;
            miReleaseSfcWithActivity.miReleaseSfcWithActivity Requst1 = new miReleaseSfcWithActivity.miReleaseSfcWithActivity();
            Requst1.ReleaseSfcWithActivityRequest = Requst;
            int retCode = 111;
            string moddCode = string.Empty;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("sfcQty:").Append(",").Append(Requst.sfcQty).Append("\r\n");
                Strb_Log.Append(ParameterLog(Requst)).Append("\r\n");
                DateTime dtEndTime = DateTime.Now;
                TimeSpan span = dtEndTime - dtStartTime;
                string WasteTime = span.TotalMilliseconds.ToString();
                Strb_Log.Append(dtEndTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("调用结束").Append("\r\n");
                Strb_Log.Append("耗时:").Append(",").Append(WasteTime).Append("ms").Append("\r\n");
                string OKOrNGOrRandom = ConfigurationManager.AppSettings["OKOrNGOrRandom"];
                Random n1 = new Random();
                if (OKOrNGOrRandom == "0")
                {
                    retCode = 0;
                    errMsg = "模拟OK";
                }
                else if (OKOrNGOrRandom == "1")
                {
                    retCode = 1;
                    errMsg = "模拟NG";
                }
                else
                {
                    retCode = n1.Next(0, 20);
                    retCode = retCode % 2 == 0 ? 0 : 1;
                    errMsg = retCode % 2 == 0 ? "随机模拟OK" : "随机模拟NG";
                }

                for (int i = 0; i < qty; i++)
                {
                    sfcs[i] =  "SFC" + DateTime.Now.ToString("yyyyMMddhhmmss");
                    Strb_Log.Append("模组号:").Append(sfcs[i]).Append(",");
                }
                Strb_Log.Append("\r\n");
            
               
            }
            catch (WebException ex)
            {
                retCode = 221;
                errMsg = ex.Message;
            }
            catch (Exception ex)
            {
                retCode = 231;
                errMsg = ex.Message;
            }
            finally
            {
                Strb_Log.Append("返回代码").Append(",").Append(retCode).Append(",").Append("返回信息").Append(",").Append(errMsg).Append("\r\n");
                ReturnResult.Add(retCode);
                ReturnResult.Add(errMsg);
                ReturnResult.Add(sfcs);
            }
            try
            {
                txtFile.WriteFile(SectionNamePath,Strb_Log.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ReturnResult;
        }
        protected StringBuilder ParameterLog(releaseSfcWithActivityRequest request)//数据收集参数
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("调用传参:").Append(",");
            temp.Append("site=").Append(request.site.ToString()).Append("; operation=").Append(request.operation.ToString()).Append("; operationRevision=").Append(request.operationRevision).Append("; ");
            temp.Append("resource=").Append(request.resource.ToString()).Append("; user=").Append(request.user.ToString()).Append("; ");
            temp.Append("activity=").Append(request.activity.ToString()).Append("; isCarrierType=").Append(request.isCarrierType.ToString()).Append("; modeProcessSFC=").Append(request.modeProcessSFC.ToString()).Append("; ");
            return temp;
        }
    }
}
