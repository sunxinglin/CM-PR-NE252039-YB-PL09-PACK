using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using MiGetPrintContent.MiGetPrintContent;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;

namespace MiGetPrintContent
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

        /// <returns></returns>
        public override List<object> GetResult(List<object> objs)
        {
            StringBuilder Strb_Log = new StringBuilder();
            string sfc = string.Empty;
            string templeteVersion = string.Empty;
            List<string> fields;
            List<object> ReturnResult = new List<object>();
            List<string> labels = new List<string>();
            if (objs != null && objs.Count >= 3)
            {
                sfc = (string)objs[0];
                templeteVersion = (string)objs[1];
                fields = (List<string>)objs[2];
            }
            else
            {
                txtFile.WriteFile(SectionNamePath, "Invalid input params");
                ReturnResult.Add(999);
                ReturnResult.Add("输入参数非法");
                ReturnResult.Add(sfc);
                ReturnResult.Add(labels);
                return ReturnResult;
            }

            MesDataModel model = (MesDataModel)Model;
            MiGetPrintContentServiceService webservice = new MiGetPrintContentServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;
            webservice.PreAuthenticate = true;
            getPrintContentRequest Requst = new getPrintContentRequest();
            Requst.site = model.SiteField;
            Requst.item = model.ItemField;
            Requst.template = model.TemplateField;
            if (string.IsNullOrEmpty(templeteVersion))
            {
                templeteVersion = "";
            }
            Requst.templateVersion = templeteVersion;
            Requst.sfc = sfc;
            Requst.field = fields.ToArray();


            miGetSlotData Requst1 = new miGetSlotData();
            Requst1.MiGetBarcodeRequest = Requst;


            Requst.sfc = sfc;

            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(Requst.sfc).Append("\r\n");

                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");

                miGetSlotDataResponse response = webservice.miGetSlotData(Requst1);
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
                            retCode = 0;
                            labels.Clear();
                            Strb_Log.Append("条码信息:").Append(",");
                            foreach (var item in response.@return.labelListArray)
                            {
                                labels.Add(item.value);
                                Strb_Log.Append(item.value).Append(",");
                            }
                            Strb_Log.Append("\r\n");
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
                ReturnResult.Add(labels);
                //ReturnResult.Add(datas);
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
