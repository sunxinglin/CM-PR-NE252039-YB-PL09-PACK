using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using miCheckSfcStatusEx.miCheckSFCStatusEx;
using System.Net;

namespace miCheckSfcStatusEx
{
    public class MesInvoke : MesInvokeBase
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
            Strb_Log = new StringBuilder();
            string sfc = string.Empty;
            if (objs != null && objs.Count == 1)
            {
                sfc = (string)objs[0];
            }
            else
            {
                txtFile.WriteFile(SectionNamePath, "Invalid input params");
                return null;
            }
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            MiCheckSFCStatusExServiceService webservice = new MiCheckSFCStatusExServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;

            changeSFCStatusExRequest Requst = new changeSFCStatusExRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;

            Requst.sfc = sfc;
            miCheckSFCStatusEx.miCheckSFCStatusEx Requst1 = new miCheckSFCStatusEx.miCheckSFCStatusEx();
            Requst1.ChangeSFCStatusExRequest = Requst;
            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(Requst.sfc).Append("\r\n");

                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");
                miCheckSFCStatusExResponse response = webservice.miCheckSFCStatusEx(Requst1);
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
                            errMsg = "校验成功";
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
            }
            try
            {
                txtFile.WriteFile(SectionNamePath, Strb_Log.ToString());
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
            string sfc = string.Empty;
            if (objs != null && objs.Count == 1)
            {
                sfc = (string)objs[0];
            }
            else
            {
                txtFile.WriteFile(SectionNamePath, "Invalid input params");
                return null;
            }
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            MiCheckSFCStatusExServiceService webservice = new MiCheckSFCStatusExServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;

            changeSFCStatusExRequest Requst = new changeSFCStatusExRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;

            Requst.sfc = sfc;
            miCheckSFCStatusEx.miCheckSFCStatusEx Requst1 = new miCheckSFCStatusEx.miCheckSFCStatusEx();
            Requst1.ChangeSFCStatusExRequest = Requst;
            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(Requst.sfc).Append("\r\n");
                Strb_Log.Append(ParameterLog(Requst)).Append("\r\n");
                //miCheckSFCStatusExResponse response = webservice.miCheckSFCStatusEx(Requst1);
                DateTime dtEndTime = DateTime.Now;
                TimeSpan span = dtEndTime - dtStartTime;
                string WasteTime = span.TotalMilliseconds.ToString();
                Strb_Log.Append(dtEndTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("调用结束").Append("\r\n");
                Strb_Log.Append("耗时:").Append(",").Append(WasteTime).Append("ms").Append("\r\n");

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
            }
            try
            {
                txtFile.WriteFile(SectionNamePath, Strb_Log.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ReturnResult;
        }
        protected StringBuilder ParameterLog(changeSFCStatusExRequest request)//数据收集参数
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("调用传参:").Append(",");
            temp.Append("site=").Append(request.site.ToString()).Append("; operation=").Append(request.operation.ToString()).Append("; operationRevision=").Append(request.operationRevision).Append("; ");
            return temp;
        }
    }
}
