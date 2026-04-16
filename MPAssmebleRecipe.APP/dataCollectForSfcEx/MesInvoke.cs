using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using dataCollectForSfcEx.dataCollectForSfcEx;
using System.Net;
using System.Configuration;

namespace dataCollectForSfcEx
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
            List<machineIntegrationParametricData> datas = new List<machineIntegrationParametricData>();
            if (objs != null && objs.Count >= 2)
            {
                sfc = (string)objs[0];
                datas = (List<machineIntegrationParametricData>)objs[1];
                //第一个是sfc
                //for (int i = 1; i < objs.Count; i++)
                //{
                //    values.Add(objs[i].ToString());
                //}
                //for (int i = 0; i < values.Count; i++)
                //{
                //    machineIntegrationParametricData data = new machineIntegrationParametricData();
                //    ParameterDataType type = (ParameterDataType)Enum.Parse(typeof(ParameterDataType), paramsDic[i].Value);
                //    data.dataType = type;
                //    data.name = paramsDic[i].Key;
                //    data.value = values[i].ToString();
                //    datas.Add(data);
                //}

            }
            else
            {
                txtFile.WriteFile(SectionNamePath, "Invalid input params");
                return null;
            }
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            MachineIntegrationServiceService webservice = new MachineIntegrationServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;
            webservice.PreAuthenticate = true;
            sfcDcExRequest Requst = new sfcDcExRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;
            Requst.resource = model.ResourceField;
            Requst.user = model.UserField;
            Requst.modeProcessSfc = model.DataCollectForSfcExModeProcessSfc;
            Requst.dcGroup = model.DcGroupField;
            Requst.activityId = model.ActivityIdField;

            dataCollectForSfcEx.dataCollectForSfcEx Requst1 = new dataCollectForSfcEx.dataCollectForSfcEx();
            Requst1.SfcDcExRequest = Requst;


            Requst.sfc = sfc;
            Requst.parametricDataArray = datas.ToArray();

            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(Requst.sfc).Append("\r\n");
                // Strb_Log.Append(ParameterLog(Requst)).Append("\r\n");
                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");
                dataCollectForSfcExResponse response = webservice.dataCollectForSfcEx(Requst1);
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
                ReturnResult.Add(sfc);
                ReturnResult.Add(datas);
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
            StringBuilder Strb_Log = new StringBuilder();
            string sfc = string.Empty;
            List<machineIntegrationParametricData> datas = new List<machineIntegrationParametricData>();
            if (objs != null && objs.Count >= 2)
            {
                sfc = (string)objs[0];
                List<string> values = new List<string>();
                //第一个是sfc
                for (int i = 1; i < objs.Count; i++)
                {
                    values.Add(objs[i].ToString());
                }
                for (int i = 0; i < values.Count; i++)
                {
                    machineIntegrationParametricData data = new machineIntegrationParametricData();
                    ParameterDataType type = (ParameterDataType)Enum.Parse(typeof(ParameterDataType), paramsDic[i].Value);
                    data.dataType = type;
                    data.name = paramsDic[i].Key;
                    data.value = values[i].ToString();
                    datas.Add(data);
                }
            }
            else
            {
                txtFile.WriteFile(SectionNamePath, "Invalid input params");
                return null;
            }
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;


            sfcDcExRequest Requst = new sfcDcExRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;
            Requst.resource = model.ResourceField;
            Requst.user = model.UserField;
            Requst.modeProcessSfc = model.DataCollectForSfcExModeProcessSfc;
            Requst.dcGroup = model.DcGroupField;
            Requst.activityId = model.ActivityIdField;

            dataCollectForSfcEx.dataCollectForSfcEx Requst1 = new dataCollectForSfcEx.dataCollectForSfcEx();
            Requst1.SfcDcExRequest = Requst;


            Requst.sfc = sfc;
            Requst.parametricDataArray = datas.ToArray();

            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(Requst.sfc).Append("\r\n");
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
                ReturnResult.Add(datas);
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
        protected StringBuilder ParameterLog(sfcDcExRequest request)//数据收集参数
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("调用传参:").Append(",");
            temp.Append("site=").Append(request.site.ToString()).Append("; operation=").Append(request.operation.ToString()).Append("; operationRevision=").Append(request.operationRevision).Append("; resource=").Append(request.resource.ToString()).Append("; ");
            temp.Append("user=").Append(request.user.ToString()).Append("; ");
            temp.Append("Resource=").Append(request.resource.ToString()).Append("; ");
            temp.Append("ActivityId=").Append(request.activityId.ToString()).Append("; ");
            temp.Append("DcGroup=").Append(request.dcGroup.ToString()).Append("; ");
            temp.Append("Mode=").Append(request.modeProcessSfc.ToString()).Append("; ");
            //temp.Append("DcGroupRevision=").Append(request.dcGroupRevision.ToString()).Append("; ");
            if (request.parametricDataArray != null && request.parametricDataArray.Length > 0)
            {
                for (int i = 0; i < request.parametricDataArray.Length; i++)
                {
                    temp.Append("parametricDataArray[" + i + "].name=").Append(request.parametricDataArray[i].name).Append(";");
                    temp.Append("parametricDataArray[" + i + "].value=").Append(request.parametricDataArray[i].value).Append(";");
                    temp.Append("parametricDataArray[" + i + "].dataType=").Append(request.parametricDataArray[i].dataType).Append(";");
                }
            }
            return temp;
        }
    }
}
