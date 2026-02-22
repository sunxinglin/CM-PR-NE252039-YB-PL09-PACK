using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using cellCustomDCCheck.cellCustomDCCheck;
using System.Net;
using System.Configuration;
using System.Web.Services;

namespace cellCustomDCCheck
{
    public class MesInvoke : MesInvokeBase
    {
    
        public string SectionNamePath;
        public StringBuilder strB_MiCustomDCForCell { get; private set; }
        public MesInvoke(string configfile, string SectionName, string interfaceName, string paramsconigfile = "") : base(configfile, SectionName, interfaceName, paramsconigfile)
        {
            Model = new MesDataModel(configfile, SectionName);
            TxtFile.ExistsPath(SectionName);
            SectionNamePath = SectionName;          
        }
        /// <summary>
        /// MiCustomDCForCellServiceService
        /// </summary>
        /// <param name="objs">0 name,1 value ,2 inventoryId ,3 marking</param>
        /// <returns>0 retCode,1 errMsg</returns>
        public override List<object> GetResult(List<object> objs)
        {

            strB_MiCustomDCForCell = new StringBuilder();
            //解析参数
            string inventroyId = string.Empty;
            string marking = string.Empty;
            List<miCustomDCForCellInventoryData> datas = new List<miCustomDCForCellInventoryData>();
            if (objs != null && objs.Count == 2)
            {
                inventroyId = (string)objs[0];
                datas = (List<miCustomDCForCellInventoryData>)objs[1];
            }
            else
            {
                txtFile.WriteFile(SectionNamePath, "Invalid input params");
                return null;
            }

            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;
            MiCustomDCForCellServiceService MiCustomDCForCell = new MiCustomDCForCellServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            MiCustomDCForCell.Url = model.Url;
            MiCustomDCForCell.Timeout = model.TimeOut;
            MiCustomDCForCell.Credentials = credentials;
            miCustomDCForCellInventory[] inventorys = new miCustomDCForCellInventory[1];
            miCustomDCForCellInventory inventory = new miCustomDCForCellInventory();
            inventory.inventoryId = inventroyId;
            inventory.marking = marking;
            inventory.inventoryDatalist = datas.ToArray();
            inventorys[0] = inventory;

            miCustomDCForCellRequest Requst = new miCustomDCForCellRequest();
            Requst.site = model.SiteField;
            Requst.dcSequence = model.DcSequenceField;
            Requst.user = model.UserField;
            Requst.Multispec = model.MultispecField;
            Requst.operation = model.OperationField;
            Requst.resource = model.ResourceField;
            Requst.inventoryList = inventorys;
            cellCustomDCCheck.cellCustomDCCheck Requst1 = new cellCustomDCCheck.cellCustomDCCheck();
            Requst1.Request = Requst;
            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                strB_MiCustomDCForCell.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append("\r\n");
                //  strB_MiCustomDCForCell.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1).Replace(",", ";")).Append("\r\n");

                strB_MiCustomDCForCell.Append("上传参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(Requst1, new Newtonsoft.Json.JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } }).Replace(",", ";")).Append("\r\n");
                cellCustomDCCheckResponse response = MiCustomDCForCell.cellCustomDCCheck(Requst1);
                strB_MiCustomDCForCell.Append("返回参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(response).Replace(",", ";")).Append("\r\n");

            //    strB_MiCustomDCForCell.Append("返回参数:").Append(",").Append(Newtonsoft.Json.JsonConvert.SerializeObject(response).Replace(",", ";")).Append("\r\n");

                DateTime dtEndTime = DateTime.Now;
                TimeSpan span = dtEndTime - dtStartTime;
                string WasteTime = span.TotalMilliseconds.ToString();
                strB_MiCustomDCForCell.Append(dtEndTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("调用结束").Append("\r\n");
                strB_MiCustomDCForCell.Append("耗时:").Append(",").Append(WasteTime.ToString()).Append("ms").Append("\r\n");
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
                strB_MiCustomDCForCell.Append("返回代码").Append(",").Append(retCode).Append(",").Append("返回信息").Append(",").Append(errMsg).Append("\r\n");
                ReturnResult.Add(retCode);
                ReturnResult.Add(errMsg);
            }
            try
            {
                txtFile.WriteFile(SectionNamePath, strB_MiCustomDCForCell.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ReturnResult;
        }

        public override List<object> GetResultSimulation(List<object> objs)
        {
            strB_MiCustomDCForCell = new StringBuilder();
            //解析参数
            string inventroyId = string.Empty;
            string marking = string.Empty;
            List<miCustomDCForCellInventoryData> datas = new List<miCustomDCForCellInventoryData>();
            if (objs != null && objs.Count == 3)
            {
                inventroyId = (string)objs[0];
                marking = (string)objs[1];
                for (int j = 2; j < objs.Count; j++)
                {
                    miCustomDCForCellInventoryData data = new miCustomDCForCellInventoryData();
                    //   data.name = paramsDic[j - 2]; 
                    data.name = "wad";
                    data.value = (string)objs[j];
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
            MiCustomDCForCellServiceService MiCustomDCForCell = new MiCustomDCForCellServiceService();
            ICredentials credentials = new NetworkCredential(model.UserName, model.PassWord);
            MiCustomDCForCell.Url = model.Url;
            MiCustomDCForCell.Timeout = model.TimeOut;
            MiCustomDCForCell.Credentials = credentials;
            miCustomDCForCellInventory[] inventorys = new miCustomDCForCellInventory[1];
            miCustomDCForCellInventory inventory = new miCustomDCForCellInventory();
            inventory.inventoryId = inventroyId;
            inventory.marking = marking;
            inventory.inventoryDatalist = datas.ToArray();
            inventorys[0] = inventory;

            miCustomDCForCellRequest Requst = new miCustomDCForCellRequest();
            Requst.site = model.SiteField;
            Requst.dcSequence = model.DcSequenceField;
            Requst.user = model.UserField;
            Requst.Multispec = model.MultispecField;
            Requst.operation = model.OperationField;
            Requst.resource = model.ResourceField;
            Requst.inventoryList = inventorys;
            cellCustomDCCheck.cellCustomDCCheck Requst1 = new cellCustomDCCheck.cellCustomDCCheck();
            Requst1.Request = Requst;
            int retCode = 111;
            string errMsg = string.Empty;
            try
            {
                DateTime dtStartTime = DateTime.Now;
                strB_MiCustomDCForCell.Append(dtStartTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("开始调用").Append("\r\n");
                strB_MiCustomDCForCell.Append(ParameterLog(Requst)).Append("\r\n");
                //cellCustomDCCheckResponse response = MiCustomDCForCell.cellCustomDCCheck(Requst1);
                DateTime dtEndTime = DateTime.Now;
                TimeSpan span = dtEndTime - dtStartTime;
                string WasteTime = span.TotalMilliseconds.ToString();
                strB_MiCustomDCForCell.Append(dtEndTime.ToString("yyyy/MM/dd HH:mm:ss:fff")).Append(",").Append("调用结束").Append("\r\n");
                strB_MiCustomDCForCell.Append("耗时:").Append(",").Append(WasteTime.ToString()).Append("ms").Append("\r\n");
                //Random n1 = new Random();
                //retCode= n1.Next(0,20);
                //retCode = retCode % 2 == 0 ? 0: 1;
                //errMsg = retCode % 2 == 0 ? "模拟OK" : "模拟NG";
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
                strB_MiCustomDCForCell.Append("返回代码").Append(",").Append(retCode).Append(",").Append("返回信息").Append(",").Append(errMsg).Append("\r\n");
                ReturnResult.Add(retCode);
                ReturnResult.Add(errMsg);
            }
            try
            {
                txtFile.WriteFile(SectionNamePath, strB_MiCustomDCForCell.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ReturnResult;
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="request"></param>
        /// <returns>StringBuilder</returns>
        protected StringBuilder ParameterLog(miCustomDCForCellRequest request)//数据收集参数
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("调用传参:").Append(",");
            temp.Append("site=").Append(request.site.ToString()).Append("; dcSequence=").Append(request.dcSequence.ToString()).Append("; user=").Append(request.user).Append("; Multispec=").Append(request.Multispec.ToString()).Append("; ");
            temp.Append("operation=").Append(request.operation.ToString()).Append("; resource=").Append(request.resource.ToString()).Append("; ");
            if (request.inventoryList != null && request.inventoryList.Length > 0)
            {
                for (int i = 0; i < request.inventoryList.Length; i++)
                {
                    temp.Append("inventoryId=").Append(request.inventoryList[i].inventoryId).Append(";").Append("marking=").Append(request.inventoryList[i].marking).Append(";");
                    temp.Append("inventoryDatalist.name=").Append(request.inventoryList[i].inventoryDatalist[0].name).Append(";");
                    temp.Append("inventoryDatalist.value=").Append(request.inventoryList[i].inventoryDatalist[0].value).Append(";");
                }
            }
            return temp;
        }
    }
}
