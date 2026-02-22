using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using miCheckInventoryAttributes.MesService;
using System.Net;
using System.Windows.Forms;
using System.Configuration;

namespace miCheckInventoryAttributes
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

            StringBuilder Strb_Log = new StringBuilder();
            string sfc = string.Empty;
            string Cell = string.Empty;
            if (objs != null && objs.Count == 2)
            {
                sfc = (string)objs[0];
                Cell = (string)objs[1];
                if (Cell == string.Empty)
                {
                    DateTime dtStartTime = DateTime.Now;
                    Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(sfc).Append(",").Append("电芯号:").Append(",").Append(Cell).Append("\r\n");
                    DateTime dtEndTime = DateTime.Now;
                    TimeSpan span = dtEndTime - dtStartTime;
                    string WasteTime = span.TotalMilliseconds.ToString();
                    Strb_Log.Append(dtEndTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("调用结束").Append("\r\n");
                    Strb_Log.Append("耗时:").Append(",").Append(WasteTime).Append("ms").Append("\r\n");
                    Strb_Log.Append("返回代码").Append(",").Append("30002").Append(",").Append("返回信息").Append(",").Append("校验电芯码为空").Append("\r\n");
                    List<object> res = new List<object>();
                    res.Add(30002);
                    res.Add("校验电芯码为空");
                    res.Add(sfc);
                    res.Add(Cell);
                    try
                    {
                        txtFile.WriteFile(SectionNamePath, Strb_Log.ToString());
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return res;
                }

            }
            else
            {
                txtFile.WriteFile(SectionNamePath, "Invalid input params");
                return null;
            }

            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            MiCheckInventoryAttributesServiceService webservice = new MiCheckInventoryAttributesServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            webservice.Url = model.Url;
            webservice.Timeout = model.TimeOut;
            webservice.Credentials = credentials;
            webservice.PreAuthenticate = true;

            ModuleCellMarkingOrTimeCheckRequest Requst = new ModuleCellMarkingOrTimeCheckRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;
            Requst.resource = model.ResourceField;
            Requst.user = model.UserField;
            Requst.sfc = sfc;
            Requst.activityId = model.ActivityIdField;
            Requst.modeCheckInventory = model.ModeCheckInventoryField;
            int retCode = 111;
            Random n1 = new Random();
            Requst.inventoryArray = new string[] { Cell };
            MesService.miCheckInventoryAttributes Requst1 = new MesService.miCheckInventoryAttributes();
            Requst1.CheckInventoryAttributesRequest = Requst;

            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(sfc).Append(",").Append("电芯号:").Append(",").Append(Cell).Append("\r\n");


                Strb_Log.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");

                miCheckInventoryAttributesResponse response = webservice.miCheckInventoryAttributes(Requst1);

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
                            errMsg = "电芯校验成功";
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
                ReturnResult.Add(Cell);
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
            string Cell = string.Empty;
            if (objs != null && objs.Count == 2)
            {
                sfc = (string)objs[0];
                Cell = (string)objs[1];
            }
            else
            {
                txtFile.WriteFile(SectionNamePath, "Invalid input params");
                return null;
            }

            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;

            ModuleCellMarkingOrTimeCheckRequest Requst = new ModuleCellMarkingOrTimeCheckRequest();
            Requst.site = model.SiteField;
            Requst.operation = model.OperationField;
            Requst.operationRevision = model.OperationRevisionField;
            Requst.resource = model.ResourceField;
            Requst.user = model.UserField;
            Requst.sfc = sfc;            
            Requst.activityId = model.ActivityIdField;
            int retCode = 111;
            // Random n1 = new Random();
            Requst.inventoryArray = new string[] { Cell };
            MesService.miCheckInventoryAttributes Requst1 = new MesService.miCheckInventoryAttributes();
            Requst1.CheckInventoryAttributesRequest = Requst;

            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                Strb_Log.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append(",").Append("模组号:").Append(",").Append(sfc).Append(",").Append("电芯号:").Append(",").Append(Cell).Append("\r\n");
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
                ReturnResult.Add(Cell);
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
        protected StringBuilder ParameterLog(ModuleCellMarkingOrTimeCheckRequest request)//数据收集参数
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("调用传参:").Append(",");
            temp.Append("inventoryArray=").Append(request.inventoryArray[0].ToString());
            temp.Append(";site=").Append(request.site.ToString()).Append("; operation=").Append(request.operation.ToString()).Append("; operationRevision=").Append(request.operationRevision).Append("; resource=").Append(request.resource.ToString()).Append("; ");
            temp.Append("user=").Append(request.user.ToString()).Append("; ");
            temp.Append("Resource=").Append(request.resource.ToString()).Append("; ");
            temp.Append("requiredQuantity=").Append(request.requiredQuantity.ToString()).Append("; ");
            temp.Append("activityId=").Append(request.activityId.ToString()).Append("; ");
            temp.Append("modeCheckInventory=").Append(request.modeCheckInventory.ToString()).Append("; ");
            return temp;
        }
    }
}
