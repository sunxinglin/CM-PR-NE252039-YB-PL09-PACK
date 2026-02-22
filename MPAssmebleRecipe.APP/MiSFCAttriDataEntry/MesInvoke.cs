using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using MiSFCAttriDataEntry.MiSFCAttriDataEntry;
using System.Net;
using System.Configuration;

namespace MiSFCAttriDataEntry
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
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            MiSFCAttriDataEntryServiceService webservice = new MiSFCAttriDataEntryServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;
            miSFCAttriDataEntryRequest Requst = new miSFCAttriDataEntryRequest();
            Requst.site = model.SiteField;
            Requst.userId = model.UserField;
            Requst.sfcMode = model.SfcModeField;
            string sfc = (string)objs[0];
            Requst.sfc = sfc;
            Requst.itemGroup = model.ItemGroupField;
            Requst.isCheckSequence = model.IsCheckSequence;

            List<sfcData> datas = new List<sfcData>();
            datas = (List<sfcData>)objs[1];
            Requst.sfcDatalist = datas.ToArray();
     
        
            miSfcAttriDataEntry Requst1 = new miSfcAttriDataEntry();
        
            Requst1.MiSFCAttriDataEntryRequest = Requst;

            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(Requst.sfc).Append("\r\n");
                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");
                miSfcAttriDataEntryResponse response = webservice.miSfcAttriDataEntry(Requst1);
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

    }
}
