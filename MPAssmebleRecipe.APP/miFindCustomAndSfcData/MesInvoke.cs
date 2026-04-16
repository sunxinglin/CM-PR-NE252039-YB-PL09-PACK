using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using miFindCustomAndSfcData.miFindCustomAndSfcData;
using System.Net;
using System.Configuration;

namespace miFindCustomAndSfcData
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
            MiFindCustomAndSfcDataServiceService webservice = new MiFindCustomAndSfcDataServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;
            webservice.PreAuthenticate = true;
            findCustomAndSfcDataRequest Requst = new findCustomAndSfcDataRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;
            Requst.resource = model.ResourceField;
            Requst.user = model.UserField;
            Requst.modeProcessSFC = model.FindCustomAndSfcDataModeProcessSFCField;
            Requst.findSfcByInventory = model.FindSfcByInventoryField;
            List<customDataInParametricData> LIST = new List<customDataInParametricData>();
            customDataInParametricData data1 = new customDataInParametricData();
            data1.category = model.FindCustomAndSfcDataCategory;
            data1.dataField = model.DataField;
            LIST.Add(data1);
            Requst.customDataArray = LIST.ToArray();
            Requst.inventory = model.InventoryField;
            Requst.isGetXY = model.IsGetXYField;
            Requst.sfcOrder = model.SfcOrderField;
            Requst.targetOrder = model.TargetOrderField;         
            ObjectAliasEnum[] masterDataArr = new ObjectAliasEnum[] { ObjectAliasEnum.ITEM };            
            Requst.masterDataArray = masterDataArr;
            string _StrName = string.Empty;
            if (Requst.findSfcByInventory)
            {
                _StrName = "inventory";
                Requst.inventory = sfc;
            }
            else
            {
                _StrName = "sfc";
                Requst.sfc = sfc;
            }

            miFindCustomAndSfcData.miFindCustomAndSfcData Requst1 = new miFindCustomAndSfcData.miFindCustomAndSfcData();
            Requst1.FindCustomAndSfcDataRequest = Requst;
            int retCode = 111;
            string errMsg = string.Empty;
            string pn = string.Empty;
            string productType = string.Empty;
            string reSFC = string.Empty;
            try
            {

                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("" + _StrName + ":").Append(",").Append((string)objs[0]).Append("\r\n");
                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");
                miFindCustomAndSfcDataResponse response = webservice.miFindCustomAndSfcData(Requst1);
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
                            pn = response.@return.masterDataArray[0].value;
                            reSFC = response.@return.sfc;
                            productType = response.@return.customDataArray[0].dataAttribute;
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
                Strb_Log.Append("返回代码").Append(",").Append(retCode).Append(",").Append("返回信息").Append(",").Append(errMsg).Append(",").Append("\r\n");
                ReturnResult.Add(retCode);
                ReturnResult.Add(errMsg);
                ReturnResult.Add(pn);
                ReturnResult.Add(reSFC);
                ReturnResult.Add(productType);
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

            findCustomAndSfcDataRequest Requst = new findCustomAndSfcDataRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;
            Requst.resource = model.ResourceField;
            Requst.user = model.UserField;
            Requst.modeProcessSFC = model.FindCustomAndSfcDataModeProcessSFCField;
            Requst.findSfcByInventory = model.FindSfcByInventoryField;
            List<customDataInParametricData> LIST = new List<customDataInParametricData>();
            customDataInParametricData data1 = new customDataInParametricData();
            data1.category = model.FindCustomAndSfcDataCategory;
            data1.dataField = model.DataField;
            LIST.Add(data1);
            Requst.customDataArray = LIST.ToArray();
            Requst.inventory = model.InventoryField;
            Requst.isGetXY = model.IsGetXYField;
            Requst.sfcOrder = model.SfcOrderField;
            Requst.targetOrder = model.TargetOrderField;
            ObjectAliasEnum[] masterDataArr = new ObjectAliasEnum[] { ObjectAliasEnum.ITEM };
            Requst.masterDataArray = masterDataArr;
            string _StrName = string.Empty;
            if (Requst.findSfcByInventory)
            {
                _StrName = "inventory";
                Requst.inventory = sfc;
            }
            else
            {
                _StrName = "sfc";
                Requst.sfc = sfc;
            }

            miFindCustomAndSfcData.miFindCustomAndSfcData Requst1 = new miFindCustomAndSfcData.miFindCustomAndSfcData();
            Requst1.FindCustomAndSfcDataRequest = Requst;
            int retCode = 111;
            string errMsg = string.Empty;
            string pn = string.Empty;
            string productType = string.Empty;
            try
            {

                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("" + _StrName + ":").Append(",").Append((string)objs[0]).Append("\r\n");
                Strb_Log.Append(ParameterLog(Requst)).Append("\r\n");

                DateTime dtEndTime = DateTime.Now;
                TimeSpan span = dtEndTime - dtStartTime;
                string WasteTime = span.TotalMilliseconds.ToString();
                Strb_Log.Append(dtEndTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("调用结束").Append("\r\n");
                Strb_Log.Append("耗时:").Append(",").Append(WasteTime).Append("ms").Append("\r\n");
                pn = "testpn";
                productType = "XPXS";
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
                Strb_Log.Append("返回代码").Append(",").Append(retCode).Append(",").Append("返回信息").Append(",").Append(errMsg).Append(",").Append("PN:").Append(pn).Append(",").Append("productType:").Append(productType).Append("\r\n");
                ReturnResult.Add(retCode);
                ReturnResult.Add(errMsg);
                ReturnResult.Add(pn);
                ReturnResult.Add(productType);
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
        protected StringBuilder ParameterLog(findCustomAndSfcDataRequest request)//数据收集参数
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("调用传参:").Append(",");
            temp.Append("site=").Append(request.site.ToString()).Append("; operation=").Append(request.operation.ToString()).Append("; operationRevision=").Append(request.operationRevision).Append("; ");
            temp.Append("resource=").Append(request.resource.ToString()).Append("; user=").Append(request.user.ToString()).Append("; modeProcessSFC=").Append(request.modeProcessSFC).Append("findSfcByInventory=").Append(request.findSfcByInventory.ToString()).Append("; ");
            //  temp.Append("activity=").Append(request.activity.ToString()).Append(",");         
            return temp;
        }
    }
}
