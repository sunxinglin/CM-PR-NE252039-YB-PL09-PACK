using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using MiAssembleAndCollectDataForSfc.miAssmebleAndCollectDataForSfc;
using System.Net;
using System.Configuration;

namespace MiAssembleAndCollectDataForSfc
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
            MiAssembleAndCollectDataForSfcServiceService webservice = new MiAssembleAndCollectDataForSfcServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;
            assembleAndCollectDataForSfcRequest Requst = new assembleAndCollectDataForSfcRequest();
            Requst.site = model.SiteField;
            Requst.dcGroup = model.DcGroupField;
            Requst.user = model.UserField;
            Requst.dcGroupRevision = model.DcGroupRevisionField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = "#";
            Requst.resource = model.ResourceField;
            miAssmebleAndCollectDataForSfc.miAssmebleAndCollectDataForSfc Requst1 = new miAssmebleAndCollectDataForSfc.miAssmebleAndCollectDataForSfc();
            Requst1.AssembleAndCollectDataForSfcRequest = Requst;
            Requst.activityId = model.ActivityIdField;
            Requst.modeProcessSFC = model.AssembleAndCollectDataForSfcModeProcessSfcField;
            string sfc = (string)objs[0];
            //  List<object> invs = (List<object>)objs[1];
            List<miInventoryData> invDatas = new List<miInventoryData>();
            invDatas = (List<miInventoryData>)objs[1];
            //miInventoryData[] invDatas = new miInventoryData[invs.Count];
            //for (int i = 0; i < invs.Count; i++)
            //{
            //    miInventoryData data = new miInventoryData();
            //    data.qty = "1";
            //    data.inventory = invs[i].ToString();
            //    invDatas[i] = data;

            //}
            Requst.inventoryArray = invDatas.ToArray();

            if (objs.Count >= 3)
            {
                //数据收集
              //  List<object> values = (List<object>)objs[2];
                List<machineIntegrationParametricData> datas = new List<machineIntegrationParametricData>();
                datas = (List<machineIntegrationParametricData>)objs[2];
                //for (int i = 0; i < values.Count; i++)
                //{
                //    machineIntegrationParametricData data = new machineIntegrationParametricData();
                //    data.value = (string)values[i];
                //    data.name = paramsDic[i].Key;
                //    data.dataType = (ParameterDataType)Enum.Parse((typeof(ParameterDataType)), paramsDic[i].Value);
                //    datas.Add(data);
                //}
                Requst.sfc = sfc;
                Requst.parametricDataArray = datas.ToArray();
            }
            Requst.sfc = sfc;
            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(Requst.sfc).Append("\r\n");
                //Strb_Log.Append("电芯码:").Append(",");
                //foreach (var item in Requst.inventoryArray)
                //{
                //    Strb_Log.Append($"{item.inventory}").Append(",");
                //}
                //Strb_Log.Append("\r\n");
                //Strb_Log.Append(ParameterLog(Requst)).Append("\r\n");

                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");
                miAssmebleAndCollectDataForSfcResponse response = webservice.miAssmebleAndCollectDataForSfc(Requst1);
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
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            MiAssembleAndCollectDataForSfcServiceService webservice = new MiAssembleAndCollectDataForSfcServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;
            assembleAndCollectDataForSfcRequest Requst = new assembleAndCollectDataForSfcRequest();
            Requst.site = model.SiteField;
            Requst.dcGroup = model.DcGroupField;
            Requst.user = model.UserField;
            Requst.dcGroupRevision = model.DcGroupRevisionField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = "#";
            Requst.resource = model.ResourceField;
            miAssmebleAndCollectDataForSfc.miAssmebleAndCollectDataForSfc Requst1 = new miAssmebleAndCollectDataForSfc.miAssmebleAndCollectDataForSfc();
            Requst1.AssembleAndCollectDataForSfcRequest = Requst;
            Requst.activityId = model.ActivityIdField;
            Requst.modeProcessSFC = model.AssembleAndCollectDataForSfcModeProcessSfcField;
            string sfc = (string)objs[0];
            //  List<object> invs = (List<object>)objs[1];
            List<miInventoryData> invDatas = new List<miInventoryData>();
            invDatas = (List<miInventoryData>)objs[1];
            //miInventoryData[] invDatas = new miInventoryData[invs.Count];
            //for (int i = 0; i < invs.Count; i++)
            //{
            //    miInventoryData data = new miInventoryData();
            //    data.qty = "1";
            //    data.inventory = invs[i].ToString();
            //    invDatas[i] = data;

            //}
            Requst.inventoryArray = invDatas.ToArray();

            if (objs.Count >= 3)
            {
                //数据收集
                //  List<object> values = (List<object>)objs[2];
                List<machineIntegrationParametricData> datas = new List<machineIntegrationParametricData>();
                datas = (List<machineIntegrationParametricData>)objs[2];
                //for (int i = 0; i < values.Count; i++)
                //{
                //    machineIntegrationParametricData data = new machineIntegrationParametricData();
                //    data.value = (string)values[i];
                //    data.name = paramsDic[i].Key;
                //    data.dataType = (ParameterDataType)Enum.Parse((typeof(ParameterDataType)), paramsDic[i].Value);
                //    datas.Add(data);
                //}
                Requst.sfc = sfc;
                Requst.parametricDataArray = datas.ToArray();
            }
            Requst.sfc = sfc;
            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(Requst.sfc).Append("\r\n");
                //Strb_Log.Append("电芯码:").Append(",");
                //foreach (var item in Requst.inventoryArray)
                //{
                //    Strb_Log.Append($"{item.inventory}").Append(",");
                //}
                //Strb_Log.Append("\r\n");
                //Strb_Log.Append(ParameterLog(Requst)).Append("\r\n");

                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");
                // miAssmebleAndCollectDataForSfcResponse response = webservice.miAssmebleAndCollectDataForSfc(Requst1);
                //miAssmebleAndCollectDataForSfcResponse response = new miAssmebleAndCollectDataForSfcResponse();
                
                //Strb_Log.Append("返回参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(response).Replace(",", ";")).Append("\r\n");

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
