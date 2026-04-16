using System;
using System.Collections.Generic;
using System.Configuration;
using RogerTech.BussnessCore.Bussness;
using RogerTech.Tool;

namespace RogerTech.BussnessCore
{
    public class BussnessDic
    {
        public Dictionary<string, IPlcInProcess> ProcessDics { get; private set; }

        public List<Group> PlcGroups { get; set; } = new List<Group>();
        public Server PlcServer = Server.GetInstance();
        private readonly string _stationName = ConfigurationManager.AppSettings["StationName"];
        bool AutomaticLabelingIsEnable = bool.Parse(ConfigurationManager.AppSettings["AutomaticLabelingIsEnable"] ?? "false");

        public void Initialize()
        {        
            ProcessDics = new Dictionary<string, IPlcInProcess>();

            var group = new Group("心跳.csv", PlcServer);
            PlcGroups.Add(group);

            switch (_stationName)
            {
                case "下箱体上线":
                    group = new Group("下箱体上线进站.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    //group = new Group("下箱体上线物料校验.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    //PlcGroups.Add(group);
                    //group = new Group("下箱体上线装配.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    //PlcGroups.Add(group);
                    //group = new Group("下箱体上线过程参数收集.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForSfcEx(group.GroupName, MesInterface.DataCollectForSfcEx));
                    //PlcGroups.Add(group);
                    //group = new Group("下箱体上线申请SFC.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    //PlcGroups.Add(group);
                    //group = new Group("下箱体上线首件.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    //PlcGroups.Add(group);
                    group = new Group("下箱体上线请求刷卡登录.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessGetMHRUsers(group.GroupName, MesInterface.GetMHRUser));
                    PlcGroups.Add(group);
                    break;
                case "下箱体涂胶":
                    group = new Group("下箱体涂胶进站.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    //group = new Group("下箱体涂胶物料校验.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    //PlcGroups.Add(group);
                    //group = new Group("下箱体涂胶装配.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    //PlcGroups.Add(group);
                    group = new Group("下箱体涂胶过程参数收集.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForSfcEx(group.GroupName, MesInterface.DataCollectForSfcEx));
                    PlcGroups.Add(group);
                    //group = new Group("下箱体涂胶首件.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    //PlcGroups.Add(group);
                    //group = new Group("下箱体涂胶获取参数.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessGetParametricValue(group.GroupName, MesInterface.GetParametricValue));
                    //PlcGroups.Add(group);
                    //group = new Group("下箱体涂胶申请SFC.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    //PlcGroups.Add(group);
                    group = new Group("下箱体涂胶请求刷卡登录.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessGetMHRUsers(group.GroupName, MesInterface.GetMHRUser));
                    PlcGroups.Add(group);
                    break;
                case "模组入箱":
					group = new Group("模组入箱获取模组码.csv", PlcServer);
					ProcessDics.Add(group.GroupName, new PlcProcessFindSfcByInventory(group.GroupName, MesInterface.FindSfcByInventory));
					PlcGroups.Add(group);
					group = new Group("模组入箱校验模组库存.csv", PlcServer);
					ProcessDics.Add(group.GroupName, new PlcProcessMiCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
					PlcGroups.Add(group);
					//group = new Group("校验是否可入箱.csv", PlcServer);
					//ProcessDics.Add(group.GroupName, new PlcProcessMiCheckSfcStatusEx(group.GroupName, MesInterface.CheckSfcStatus));
					//PlcGroups.Add(group);
					group = new Group("模组入箱获取PACK涂胶时间.csv", PlcServer);
					ProcessDics.Add(group.GroupName, new PlcProcessGetParametricValue(group.GroupName, MesInterface.GetParametricValue));
					PlcGroups.Add(group);
					group = new Group("模组入箱PACK进站.csv", PlcServer);
					ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
					PlcGroups.Add(group);
					group = new Group("模组入箱PACK装配.csv", PlcServer);
					ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
					PlcGroups.Add(group);
					group = new Group("模组入箱PACK收数.csv", PlcServer);
					ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForSfcEx(group.GroupName, MesInterface.DataCollectForSfcEx));
					PlcGroups.Add(group);
					//group = new Group("蓝本校验.csv", PlcServer);
					//ProcessDics.Add(group.GroupName, new PlcProcessCheckVersion(group.GroupName, MesInterface.DataCollectForSfcEx));
					//PlcGroups.Add(group);
					group = new Group("模组入箱请求刷卡登录.csv", PlcServer);
					ProcessDics.Add(group.GroupName, new PlcProcessGetMHRUsers(group.GroupName, MesInterface.GetMHRUser));
					PlcGroups.Add(group);
                    break;
				case "肩部涂胶":
                    group = new Group("肩部涂胶进站.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    //group = new Group("肩部涂胶物料校验.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    //PlcGroups.Add(group);
                    //group = new Group("肩部涂胶装配.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    //PlcGroups.Add(group);
                    group = new Group("肩部涂胶过程参数收集.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForSfcEx(group.GroupName, MesInterface.DataCollectForSfcEx));
                    PlcGroups.Add(group);
                    //group = new Group("肩部涂胶首件.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    //PlcGroups.Add(group);
                    //group = new Group("肩部涂胶获取参数.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessGetParametricValue(group.GroupName, MesInterface.GetParametricValue));
                    //PlcGroups.Add(group);
                    //group = new Group("肩部涂胶申请SFC.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    //PlcGroups.Add(group);
                    group = new Group("肩部涂胶请求刷卡登录.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessGetMHRUsers(group.GroupName, MesInterface.GetMHRUser));
                    PlcGroups.Add(group);
                    break;
                case "压条自动加压和拧紧":
                    group = new Group("压条自动加压和拧紧进站.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧物料校验.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧装配.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧过程参数收集.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessTighteningDataCollectForSfcEx(group.GroupName, MesInterface.None));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧首件.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧申请SFC.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    PlcGroups.Add(group);
                    //group = new Group("压条自动加压和拧紧校验涂胶超时.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessGetParametricValue(group.GroupName, MesInterface.GetParametricValue));
                    //PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧请求刷卡登录.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessGetMHRUsers(group.GroupName, MesInterface.GetMHRUser));
                    PlcGroups.Add(group);
                    break;
                case "上盖拧紧":
                    group = new Group("上盖拧紧进站.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    //group = new Group("上盖拧紧物料校验.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    //PlcGroups.Add(group);
                    //group = new Group("上盖拧紧装配.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    //PlcGroups.Add(group);
                    //group = new Group("上盖拧紧申请SFC.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    //PlcGroups.Add(group);
                    //group = new Group("上盖拧紧首件.csv", PlcServer);
                    //ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    //PlcGroups.Add(group);
                    group = new Group("上盖拧紧过程参数收集.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessTighteningDataCollectForSfcEx(group.GroupName, MesInterface.None));
                    PlcGroups.Add(group);
                    group = new Group("上盖拧紧请求刷卡登录.csv", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessGetMHRUsers(group.GroupName, MesInterface.GetMHRUser));
                    PlcGroups.Add(group);
                    break;
                default:
                    throw new Exception("未配置对应工站业务处理类，请检查配置文件StationName值是否正确");
            }

        }
        public BussnessDic()
        {
            Initialize();
        }
    }
}
