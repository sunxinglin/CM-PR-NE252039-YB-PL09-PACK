using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using CatlMesBase;
using System.Reflection;
using RogerTech.Common.Models;
using RogerTech.Common;

namespace RogerTech.BussnessCore
{
    //公共任务
    public class BussnessUtility
    {

        static string filePath = Directory.GetCurrentDirectory();
        static string fileName = "MESCFG.ini";
        static string Simulation = ConfigurationManager.AppSettings["Simulation"];
        public Dictionary<string, string> ShoporderPnDic { get; set; }
        public Dictionary<string, Type> propertyDictionary { get; set; }
        private static BussnessUtility instance;
        public bool bMesSimulation { get; set; }
        public static BussnessUtility GetInstance()
        {
            lock (Locker)
            {
                if (instance == null)
                {
                    instance = new BussnessUtility();
                }
                return instance;
            }
        }
        private BussnessUtility()
        {
            ShoporderPnDic = new Dictionary<string, string>();
            bool simulation = false;
            if (bool.TryParse(Simulation, out simulation))
            {

            }
            bMesSimulation = simulation;
            propertyDictionary = new Dictionary<string, Type>();
            try
            {
                foreach (var item in Enum.GetNames(typeof(MesInterface)))
                {
                    MesInvokeBase mes = MesInvoke((MesInterface)Enum.Parse((typeof(MesInterface)), item));
                    if (mes == null)
						continue;

					PropertyInfo[] properties = mes.Model.GetType().GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (property.PropertyType.IsEnum)
                        {
                            string propertyName = property.Name;
                            Type propertyType = property.PropertyType;
                            //  propertyDictionary.Add(propertyName.Replace("Field", ""), propertyType.Name);
                            string key = propertyName.Replace("Field", "");

                            if (!propertyDictionary.ContainsKey(key))
                            {
                                propertyDictionary.Add(key, propertyType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        //申请模组号

        //获取校验用模组码
        private static readonly object Locker = new object();
        public List<object> MesInvoke(List<object> inputs, MesInterface interfaceType)
        {
            string fullName = Path.Combine(filePath, fileName);
            MesInvokeBase mesInvoke = null;
            switch (interfaceType)
            {
                case MesInterface.ReleaseSfcWithActivity:
                    mesInvoke = new miReleaseSfcWithActivity.MesInvoke(fullName, "ReleaseSfcWithActivity", "释放模组号");
                    break;
                case MesInterface.ReleaseSfcByShopOrder:
                    mesInvoke = new miReleaseSfcByOrder.MesInvoke(fullName, "ReleaseSfcByShopOrder", "工单释放模组号");
                    break;
                case MesInterface.CheckCellAttribute:
                    mesInvoke = new miCheckInventoryAttributes.MesInvoke(fullName, "CheckCellAttribute", "电芯校验");
                    break;
                case MesInterface.AssembleAndCollectDataForSfc:
                    mesInvoke = new MiAssembleAndCollectDataForSfc.MesInvoke(fullName, "AssembleAndCollectDataForSfc", "电芯装配");
                    break;
                case MesInterface.GetModulePn:
                    mesInvoke = new miFindCustomAndSfcData.MesInvoke(fullName, "GetModulePn", "获取Pn");
                    break;
                case MesInterface.MiFindCustomData:
                    mesInvoke = new miFindCustomAndSfcData.MesInvoke(fullName, "MiFindCustomData", "模组进站");
                    break;
                case MesInterface.FindSfcByInventory:
                    mesInvoke = new miFindCustomAndSfcData.MesInvoke(fullName, "FindSfcByInventory", "获取模组码");
                    break;
                case MesInterface.DataCollectForSfcEx:
                    mesInvoke = new dataCollectForSfcEx.MesInvoke(fullName, "DataCollectForSfcEx", "模组出站");
                    break;
                case MesInterface.CheckSfcStatus:
                    mesInvoke = new miCheckSfcStatusEx.MesInvoke(fullName, "CheckSfcStatus", "检查模组状态");
                    break;
                case MesInterface.DataCollectForCleanData:
                    mesInvoke = new dataCollectForSfcEx.MesInvoke(fullName, "DataCollectForCleanData", "清洗数据上传");
                    break;
                case MesInterface.CellOcvCheck:
                    mesInvoke = new cellCustomDCCheck.MesInvoke(fullName, "CellOcvCheck", "OCV校验");
                    break;
                case MesInterface.AssembleComponentToSfc:
                    mesInvoke = new MiAssembleComponentsToSfcs.MesInvoke(fullName, "AssembleComponentsToSfcs", "物料装配");
                    break;
                case MesInterface.CheckBOMInventory:
                    mesInvoke = new MiCheckBOMInventory.MesInvoke(fullName, "CheckBOMInventory", "BOM库存校验");
                    break;
                case MesInterface.DataCollectForResourceInspectTask:
                    mesInvoke = new DataCollectForResourceInspectTask.MesInvoke(fullName, "DataCollectForResourceInspectTask", "资源检查上传");
                    break;
                case MesInterface.GetParametricValue:
                    mesInvoke = new GetParametricValue.MesInvoke(fullName, "GetParametricValue", "获取参数值");
                    break;
                case MesInterface.SFCAttriDataEntry:
                    mesInvoke = new MiSFCAttriDataEntry.MesInvoke(fullName, "SFCAttriDataEntry", "电芯装配顺序比对");
                    break;
                case MesInterface.MiCheckShoporderInfo:
                    mesInvoke = new MiCheckShoporderInfo.MesInvoke(fullName, "MiCheckShoporderInfo", "获取工单信息");
                    break;
                case MesInterface.GetMHRUser:
                    mesInvoke = new GetMHRUser.MesInvoke(fullName, "GetMHRUser", "获取用户");
                    break;
                default:
                    break;
            }
            return mesInvoke.GetResult(inputs);
        }
        public MesInvokeBase MesInvoke(MesInterface interfaceType)
        {
            string fullName = Path.Combine(filePath, fileName);
            CatlMesBase.MesInvokeBase mesInvoke = null;
            switch (interfaceType)
            {
                case MesInterface.ReleaseSfcWithActivity:
                    mesInvoke = new miReleaseSfcWithActivity.MesInvoke(fullName, "ReleaseSfcWithActivity", "释放模组号");
                    break;
                case MesInterface.ReleaseSfcByShopOrder:
                    mesInvoke = new miReleaseSfcByOrder.MesInvoke(fullName, "ReleaseSfcByShopOrder", "工单释放模组号");
                    break;
                case MesInterface.CheckCellAttribute:
                    mesInvoke = new miCheckInventoryAttributes.MesInvoke(fullName, "CheckCellAttribute", "电芯校验");
                    break;
                case MesInterface.AssembleAndCollectDataForSfc:
                    mesInvoke = new MiAssembleAndCollectDataForSfc.MesInvoke(fullName, "AssembleAndCollectDataForSfc", "电芯装配");
                    break;
                case MesInterface.GetModulePn:
                    mesInvoke = new miFindCustomAndSfcData.MesInvoke(fullName, "GetModulePn", "获取Pn");
                    break;
                case MesInterface.MiFindCustomData:
                    mesInvoke = new miFindCustomAndSfcData.MesInvoke(fullName, "MiFindCustomData", "模组进站");
                    break;
                case MesInterface.FindSfcByInventory:
                    mesInvoke = new miFindCustomAndSfcData.MesInvoke(fullName, "FindSfcByInventory", "获取模组码");
                    break;
                case MesInterface.DataCollectForSfcEx:
                    mesInvoke = new dataCollectForSfcEx.MesInvoke(fullName, "DataCollectForSfcEx", "模组出站");
                    break;
                case MesInterface.CheckSfcStatus:
                    mesInvoke = new miCheckSfcStatusEx.MesInvoke(fullName, "CheckSfcStatus", "检查模组状态");
                    break;
                case MesInterface.DataCollectForCleanData:
                    mesInvoke = new dataCollectForSfcEx.MesInvoke(fullName, "DataCollectForCleanData", "清洗数据上传");
                    break;
                case MesInterface.CellOcvCheck:
                    mesInvoke = new cellCustomDCCheck.MesInvoke(fullName, "CellOcvCheck", "OCV校验");
                    break;
                case MesInterface.AssembleComponentToSfc:
                    mesInvoke = new MiAssembleComponentsToSfcs.MesInvoke(fullName, "AssembleComponentsToSfcs", "物料装配");
                    break;
                case MesInterface.CheckBOMInventory:
                    mesInvoke = new MiCheckBOMInventory.MesInvoke(fullName, "CheckBOMInventory", "BOM库存校验");
                    break;
                case MesInterface.DataCollectForResourceInspectTask:
                    mesInvoke = new DataCollectForResourceInspectTask.MesInvoke(fullName, "DataCollectForResourceInspectTask", "资源检查上传");
                    break;
                case MesInterface.GetParametricValue:
                    mesInvoke = new GetParametricValue.MesInvoke(fullName, "GetParametricValue", "获取参数值");
                    break;
                case MesInterface.SFCAttriDataEntry:
                    mesInvoke = new MiSFCAttriDataEntry.MesInvoke(fullName, "SFCAttriDataEntry", "电芯装配顺序比对");
                    break;
                case MesInterface.MiCheckShoporderInfo:
                    mesInvoke = new MiCheckShoporderInfo.MesInvoke(fullName, "MiCheckShoporderInfo", "获取工单信息");
                    break;
                case MesInterface.GetMHRUser:
                    mesInvoke = new GetMHRUser.MesInvoke(fullName, "GetMHRUser", "获取用户");
                    break;
                case MesInterface.None:
					break;
                default:
                    break;
            }

            return mesInvoke;

        }

        public KeyValuePair<string, string> MesInvokeMesCollectData(List<UploadData> lstData)
        {
            try
            {
                if (lstData == null || lstData.Count == 0) return new KeyValuePair<string, string>("-9999", "传参数据为空");
                List<object> input = new List<object>();
                List<object> output = new List<object>();
                input.Add(lstData[0].SFC);
                //将lstData 的time修改为当前时间后插入

                List<dataCollectForSfcEx.dataCollectForSfcEx.machineIntegrationParametricData> datas = new List<dataCollectForSfcEx.dataCollectForSfcEx.machineIntegrationParametricData>();
                foreach (var item in lstData)
                {
                    datas.Add(new dataCollectForSfcEx.dataCollectForSfcEx.machineIntegrationParametricData()
                    {
                        name = item.UploadDataName,
                        dataType = (dataCollectForSfcEx.dataCollectForSfcEx.ParameterDataType)item.UploadDataType,
                        value = item.UploadDataValue
                    });
                }
                input.Add(datas);
                output = MesInvoke(input, (MesInterface)Enum.Parse((typeof(MesInterface)), lstData[0].InterfaceName));
                DbContext.GetInstance().Updateable<UploadData>()
                  .AS("UploadData")
                 .SetColumns(u => u.IsReupload == true)
                 .Where(u => u.SFC == lstData[0].SFC)
                 .ExecuteCommand();

                lstData.ForEach(item => item.Time = DateTime.Now);
                DbContext.GetInstance().Insertable(lstData).ExecuteCommand();
                return new KeyValuePair<string, string>(output[0].ToString(), output[1].ToString());
            }
            catch (Exception ex)
            {
                return new KeyValuePair<string, string>("-9999", ex.Message);
            }
        }
    }

    public enum MesInterface
    {
        ReleaseSfcWithActivity,
        ReleaseSfcByShopOrder,
        CheckCellAttribute,
        AssembleComponentToSfc,
        AssembleAndCollectDataForSfc,
        GetModulePn,
        MiFindCustomData,
        FindSfcByInventory,
        DataCollectForSfcEx,
        CheckSfcStatus,
        DataCollectForCleanData,
        CellOcvCheck,
        CheckBOMInventory,
        DataCollectForResourceInspectTask,
        GetParametricValue,
        SFCAttriDataEntry,
        MiCheckShoporderInfo,
        GetMHRUser,
        //MiGetPrintContent,
        None
    }
}
