using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using GetParametricValue.GetParametricValue;
using System.Net;
using System.Configuration;
using System.Data;

namespace GetParametricValue
{
    public class MesInvoke : MesInvokeBase
    {
        public string SectionNamePath;
        public MesInvoke(string configfile, string SectionName, string interfaceName, string paramsconigfile = "") : base(configfile, SectionName, interfaceName, paramsconigfile)
        {
            Model = new MesDataModel(configfile, SectionName);
            TxtFile.ExistsPath(SectionName);
            SectionNamePath = SectionName;
        }
        public override List<object> GetResult(List<object> objs)
        {
            StringBuilder Strb_Log = new StringBuilder();
            string sfc = string.Empty;
            List<GetParametricValueRequestData> datas = new List<GetParametricValueRequestData>();
            if (objs != null && objs.Count >= 2)
            {
                sfc = (string)objs[0];
                datas = (List<GetParametricValueRequestData>)objs[1];
            }
            else
            {
                txtFile.WriteFile(SectionNamePath, "Invalid input params");
                return null;
            }
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            GetParametricValueServiceService webservice = new GetParametricValueServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;
            webservice.PreAuthenticate = true;
            GetParametricValueRequest Requst = new GetParametricValueRequest();
            Requst.site = model.SiteField;
            Requst.sfc = sfc;   
            Requst.user = model.UserField;
            getParametricValue Requst1 = new getParametricValue();
            Requst1.GetParametricValueRequest = Requst;
            Requst.parametricDataArray = datas.ToArray();
            List<GetParametricValueData> returnData = new List<GetParametricValueData>();
            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(Requst.sfc).Append("\r\n");
                // Strb_Log.Append(ParameterLog(Requst)).Append("\r\n");
                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");
                getParametricValueResponse response = webservice.getParametricValue(Requst1);
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
                            returnData = response.@return.parametricDataArray.ToList();
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
                ReturnResult.Add(sfc);
                ReturnResult.Add(returnData);
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

       
    }
}
