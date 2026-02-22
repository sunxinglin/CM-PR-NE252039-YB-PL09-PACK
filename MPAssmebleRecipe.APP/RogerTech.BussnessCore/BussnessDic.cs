using RogerTech.BussnessCore.Bussness;
using RogerTech.Tool;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.BussnessCore
{
    public class BussnessDic
    {
        public Dictionary<string, IPlcInProcess> ProcessDics { get; private set; }

        public List<Group> PlcGroups { get; set; } = new List<Group>();
        public Server PlcServer = Server.GetInstace();
        protected string StationName = ConfigurationManager.AppSettings["StationName"];

        public void Initialize()
        {
            ProcessDics = new Dictionary<string, IPlcInProcess>();
            Group group;

            group = new Group("心跳.txt", PlcServer);
            PlcGroups.Add(group);

            switch (StationName)
            {
                case "下箱体上线":
                    group = new Group("下箱体上线进站.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    group = new Group("下箱体上线物料校验.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    PlcGroups.Add(group);
                    group = new Group("下箱体上线装配.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    PlcGroups.Add(group);
                    group = new Group("下箱体上线申请SFC.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    PlcGroups.Add(group);
                    group = new Group("下箱体上线首件.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    PlcGroups.Add(group);
                    group = new Group("下箱体上线过程参数收集.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForSfcEx(group.GroupName, MesInterface.DataCollectForSfcEx ));
                    PlcGroups.Add(group);
                    break;
                case "下箱体涂胶":
                    group = new Group("下箱体涂胶进站.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    group = new Group("下箱体涂胶物料校验.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    PlcGroups.Add(group);
                    group = new Group("下箱体涂胶装配.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    PlcGroups.Add(group);
                    group = new Group("下箱体涂胶申请SFC.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    PlcGroups.Add(group);
                    group = new Group("下箱体涂胶首件.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    PlcGroups.Add(group);
                    group = new Group("下箱体涂胶过程参数收集.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForSfcEx(group.GroupName, MesInterface.DataCollectForSfcEx));
                    PlcGroups.Add(group);
                    group = new Group("下箱体涂胶获取参数.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessGetParametricValue(group.GroupName, MesInterface.GetParametricValue));
                    PlcGroups.Add(group);
                    break;
                case "肩部涂胶":
                    group = new Group("肩部涂胶进站.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    group = new Group("肩部涂胶物料校验.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    PlcGroups.Add(group);
                    group = new Group("肩部涂胶装配.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    PlcGroups.Add(group);
                    group = new Group("肩部涂胶申请SFC.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    PlcGroups.Add(group);
                    group = new Group("肩部涂胶首件.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    PlcGroups.Add(group);
                    group = new Group("肩部涂胶过程参数收集.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForSfcEx(group.GroupName, MesInterface.DataCollectForSfcEx));
                    PlcGroups.Add(group);
                    break;
                case "压条自动加压和拧紧":
                    group = new Group("压条自动加压和拧紧进站.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧物料校验.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧装配.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧申请SFC.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧首件.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧过程参数收集.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForSfcEx(group.GroupName, MesInterface.DataCollectForSfcEx));
                    PlcGroups.Add(group);
                    group = new Group("压条自动加压和拧紧校验涂胶超时.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessFindSfcByInventory(group.GroupName, MesInterface.FindSfcByInventory));
                    PlcGroups.Add(group);
                    break;
                case "上盖拧紧":
                    group = new Group("上盖拧紧进站.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessMiFindCustomAndSfcData(group.GroupName, MesInterface.MiFindCustomData));
                    PlcGroups.Add(group);
                    group = new Group("上盖拧紧物料校验.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessCheckBOMInventory(group.GroupName, MesInterface.CheckBOMInventory));
                    PlcGroups.Add(group);
                    group = new Group("上盖拧紧装配.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessAssembleComponentToSfc(group.GroupName, MesInterface.AssembleComponentToSfc));
                    PlcGroups.Add(group);
                    group = new Group("上盖拧紧申请SFC.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessReleaseSfcByShopOrder(group.GroupName, MesInterface.ReleaseSfcByShopOrder));
                    PlcGroups.Add(group);
                    group = new Group("上盖拧紧首件.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForResourceInspectTask(group.GroupName, MesInterface.DataCollectForResourceInspectTask));
                    PlcGroups.Add(group);
                    group = new Group("上盖拧紧过程参数收集.txt", PlcServer);
                    ProcessDics.Add(group.GroupName, new PlcProcessDataCollectForSfcEx(group.GroupName, MesInterface.DataCollectForSfcEx));
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
