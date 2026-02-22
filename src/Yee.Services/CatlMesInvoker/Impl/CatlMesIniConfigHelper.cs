using Catl.HostComputer.CommonServices;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Ctp0600P.Shared.CatlMes;
using Catl.WebServices.MIFindCustomAndSfcData;
using Catl.WebServices.AssembleAndCollectDataForSfc;
using Catl.WebServices.MachineIntegrationServices;
using System.Reflection.Emit;
using Ctp0600P.Shared;

namespace Yee.Services.CatlMesInvoker
{
    public class CatlMesIniConfigHelper
    {
        private readonly IOptionsMonitor<CatlMesOpt> _catlMesOptsMonitor;

        public CatlMesIniConfigHelper(IOptionsMonitor<CatlMesOpt> catlMesOptsMonitor)
        {
            this._catlMesOptsMonitor = catlMesOptsMonitor;
        }

        public string MIFindCustomAndSfcDataConfigName => this._catlMesOptsMonitor.CurrentValue.MIFindCustomAndSfcDataInterfaceName;
        public string DataCollectForResourceFAIName => this._catlMesOptsMonitor.CurrentValue.DataCollectForResourceFAIInterfaceName;
        public string MICheckSFCStatusExName => this._catlMesOptsMonitor.CurrentValue.MICheckSFCStatusExInterfaceName;
        public string MICheckBOMInventoryName => this._catlMesOptsMonitor.CurrentValue.MICheckBOMInventoryInterfaceName;
        public string MiAssembleAndCollectDataForSfcName => this._catlMesOptsMonitor.CurrentValue.MiAssembleAndCollectDataForSfcInterfaceName;
        public string DataCollectForMoudleTestName => this._catlMesOptsMonitor.CurrentValue.DataCollectForMoudleTestInterfaceName;
        public string DataCollectForResourceInspect => this._catlMesOptsMonitor.CurrentValue.DataCollectForResourceInspect;

        /// <summary>
        /// 根据物料查询条码
        /// </summary>
        public string FindSfcByInventoryName => this._catlMesOptsMonitor.CurrentValue.FindSfcByInventoryName;

        /// <summary>
        /// 校验模组状态
        /// </summary>
        public string MiCheckInventoryAttributesName => this._catlMesOptsMonitor.CurrentValue.MiCheckInventoryAttributesName;

        public string GetParametricValueName => this._catlMesOptsMonitor.CurrentValue.GetParametricValueName;

        public string GetInitPath()
        {
            var dir = this._catlMesOptsMonitor.CurrentValue.IniFileDir;
            if (string.IsNullOrEmpty(dir))
            {
                dir = Directory.GetDirectoryRoot(Assembly.GetExecutingAssembly().Location);
            }

            var fname = this._catlMesOptsMonitor.CurrentValue.IniFileName;
            if (string.IsNullOrEmpty(fname))
            {
                fname = "MESCFG.ini";
            }
            return System.IO.Path.Combine(dir, fname);
        }

        public IniSection GetSection(string sectionKey, string? opCode = "")
        {
            var inifile = new IniFile();
            var inipath = this.GetInitPath();
            if (!string.IsNullOrEmpty(opCode))
                inipath = inipath.Replace(".ini", $"_{opCode}.ini");

            if (File.Exists(inipath))
            {
                inifile.Load(inipath);
            }
            else
            {
                if (!Directory.Exists(this._catlMesOptsMonitor.CurrentValue.IniFileDir))
                    Directory.CreateDirectory(this._catlMesOptsMonitor.CurrentValue.IniFileDir);
                File.Create(inipath).Close();
                inifile.Load(inipath);
            }

            //如果找到对应的键值 就返回一个
            var has = inifile.TryGetSection(sectionKey, out var section);
            if (!has)
            {
                return null;
            }

            return section;
        }

        public IniSection UpdateSection(string sectionKey, Action<IniSection> update, string opCode = "")
        {
            var inipath = this.GetInitPath();

            if (!string.IsNullOrEmpty(opCode))
                inipath = inipath.Replace(".ini", $"_{opCode}.ini");

            var inifile = new IniFile();
            if (File.Exists(inipath))
            {
                inifile.Load(inipath);
            }
            var has = inifile.TryGetSection(sectionKey, out var section);
            if (!has)
            {
                section = new IniSection();
            }

            update?.Invoke(section);

            inifile.Remove(sectionKey);
            var s = inifile.Add(sectionKey, section);
            inifile.Save(inipath, FileMode.OpenOrCreate);
            return s;
        }

        public CatlMesConnectionParams GetMesConnectionParams(IniSection section)
        {
            var url = section.TryGetValue(nameof(CatlMesConnectionParams.Url), out var urlvalue) ? urlvalue.GetString() : null;
            var username = section.TryGetValue(nameof(CatlMesConnectionParams.UserName), out var usernamevalue) ? usernamevalue.GetString() : null;
            var password = section.TryGetValue(nameof(CatlMesConnectionParams.Password), out var passwdvalue) ? passwdvalue.GetString() : null;
            var timeout = section.TryGetValue(nameof(CatlMesConnectionParams.Timeout), out var timeoutvalue) ? timeoutvalue.ToInt() : 0;
            return new CatlMesConnectionParams
            {
                Url = url,
                UserName = username,
                Password = password,
                Timeout = timeout,
            };
        }


        #region 进站接口

        public void SetMIFindCustomAndSfcDataConfig(MIFindCustomAndSfcDataConfig config, string opCode = "")
        {
            var section = GetSection(this.MIFindCustomAndSfcDataConfigName, opCode);

            this.UpdateSection(this.MIFindCustomAndSfcDataConfigName, section =>
            {
                #region MyRegion
                section.Remove(nameof(MIFindCustomAndSfcDataConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.User), out var olduser);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.User), config.InterfaceParams.User);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.Operation), out var oldoperation);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.Operation), config.InterfaceParams.Operation);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.OperationRevision), out var oldoperationRevision);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.OperationRevision), config.InterfaceParams.OperationRevision);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.ActivityId), out var oldActivityId);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.ActivityId), config.InterfaceParams.ActivityId);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.Resource), out var oldResource);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.Resource), config.InterfaceParams.Resource);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.sfc), out var oldSfcQty);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.sfc), config.InterfaceParams.sfc);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.modeProcessSfc), out var oldModeProcessSfc);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.modeProcessSfc), (int)config.InterfaceParams.modeProcessSfc);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.findSfcByInventory), out var findSfcByInventory);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.findSfcByInventory), config.InterfaceParams.findSfcByInventory);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.inventory), out var inventory);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.inventory), config.InterfaceParams.inventory);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.sfcOrder), out var sfcOrder);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.sfcOrder), config.InterfaceParams.sfcOrder);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.targetOrder), out var targetOrder);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.targetOrder), config.InterfaceParams.targetOrder);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.dcGroup), out var dcGroup);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.dcGroup), config.InterfaceParams.dcGroup);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.dcGroupRevision), out var dcGroupRevision);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.dcGroupRevision), config.InterfaceParams.dcGroupRevision);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.category), out var category);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.category), (int)config.InterfaceParams.category);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.dataField), out var dataField);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.dataField), config.InterfaceParams.dataField);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.masterDataArray), out var masterDataArray);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.masterDataArray), (int)config.InterfaceParams.masterDataArray);

                section.Remove(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.usage), out var usage);
                section.Add(nameof(MIFindCustomAndSfcDataConfig.InterfaceParams.usage), config.InterfaceParams.usage);
                #endregion
            }, opCode);
        }

        public MIFindCustomAndSfcDataConfig GetMIFindCustomAndSfcDataConfig(string opCode = "")
        {
            var section = GetSection(this.MIFindCustomAndSfcDataConfigName, opCode);
            if (section == null) return null!;
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(MIFindCustomAndSfcParams.Site), out var sitevalue) ? sitevalue.GetString() : null;
            var user = section.TryGetValue(nameof(MIFindCustomAndSfcParams.User), out var uservalue) ? uservalue.GetString() : null;
            var operation = section.TryGetValue(nameof(MIFindCustomAndSfcParams.Operation), out var operationvalue) ? operationvalue.GetString() : null;
            var operationRevision = section.TryGetValue(nameof(MIFindCustomAndSfcParams.OperationRevision), out var operationRevisionValue) ? operationRevisionValue.GetString() : "#";
            var activityId = section.TryGetValue(nameof(MIFindCustomAndSfcParams.ActivityId), out var activityIdValue) ? activityIdValue.GetString() : "EAP_WS";
            var recourse = section.TryGetValue(nameof(MIFindCustomAndSfcParams.Resource), out var rescourseValue) ? rescourseValue.GetString() : null;
            var dcGroup = section.TryGetValue(nameof(MIFindCustomAndSfcParams.dcGroup), out var dcGroupValue) ? dcGroupValue.GetString() : null;
            var dcGroupRevision = section.TryGetValue(nameof(MIFindCustomAndSfcParams.dcGroupRevision), out var dcGroupRevisionValue) ? dcGroupRevisionValue.GetString() : null;
            var sfcOrder = section.TryGetValue(nameof(MIFindCustomAndSfcParams.sfcOrder), out var sfcOrderValue) ? sfcOrderValue.GetString() : null;
            var targetOrder = section.TryGetValue(nameof(MIFindCustomAndSfcParams.targetOrder), out var targetOrderValue) ? targetOrderValue.GetString() : null;
            var modeProcessSfc = section.TryGetValue(nameof(MIFindCustomAndSfcParams.modeProcessSfc), out var modeProcessSfcValue) ? (modeProcessSFC)modeProcessSfcValue.ToInt() : modeProcessSFC.MODE_NONE;
            var category = section.TryGetValue(nameof(MIFindCustomAndSfcParams.category), out var categoryValue) ? (ObjectAliasEnum)categoryValue.ToInt() : ObjectAliasEnum.ITEM;
            var dataField = section.TryGetValue(nameof(MIFindCustomAndSfcParams.dataField), out var dataFieldValue) ? dataFieldValue.GetString() : null;
            var masterDataArray = section.TryGetValue(nameof(MIFindCustomAndSfcParams.masterDataArray), out var masterDataArrayValue) ? (ObjectAliasEnum)masterDataArrayValue.ToInt() : ObjectAliasEnum.ITEM;
            var usage = section.TryGetValue(nameof(MIFindCustomAndSfcParams.usage), out var usageValue) ? usageValue.GetString() : null;
            var findSfcByInventory = section.TryGetValue(nameof(MIFindCustomAndSfcParams.findSfcByInventory), out var findSfcByInventoryValue) ? findSfcByInventoryValue.ToBool() : true;

            var inventory = section.TryGetValue(nameof(MIFindCustomAndSfcParams.inventory), out var inventoryValue) ? inventoryValue.GetString() : null;

            return new MIFindCustomAndSfcDataConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new MIFindCustomAndSfcParams
                {
                    Site = site,
                    User = user,
                    Operation = operation,
                    OperationRevision = operationRevision,
                    ActivityId = activityId,
                    Resource = recourse,
                    modeProcessSfc = (modeProcessSFC)modeProcessSfc,
                    dcGroup = dcGroup,
                    dcGroupRevision = dcGroupRevision,
                    sfcOrder = sfcOrder,
                    targetOrder = targetOrder,
                    category = category,
                    usage = usage,
                    dataField = dataField,
                    masterDataArray = masterDataArray,
                    findSfcByInventory = findSfcByInventory,
                    inventory = inventory,
                },
            };
        }

        #endregion

        #region 首件接口

        public void SetDataCollectForResourceFAIConfig(DataCollectForResourceFAIConfig config)
        {
            var section = GetSection(this.DataCollectForResourceFAIName);

            this.UpdateSection(this.DataCollectForResourceFAIName, section =>
            {
                section.Remove(nameof(DataCollectForResourceFAIConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(DataCollectForResourceFAIConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(DataCollectForResourceFAIConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(DataCollectForResourceFAIConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(DataCollectForResourceFAIConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(DataCollectForResourceFAIConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(DataCollectForResourceFAIConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(DataCollectForResourceFAIConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.User), out var olduser);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.User), config.InterfaceParams.User);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.Operation), out var oldoperation);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.Operation), config.InterfaceParams.Operation);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.OperationRevision), out var oldoperationRevision);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.OperationRevision), config.InterfaceParams.OperationRevision);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.ActivityId), out var oldActivityId);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.ActivityId), config.InterfaceParams.ActivityId);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.Resource), out var oldResource);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.Resource), config.InterfaceParams.Resource);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.sfc), out var oldSfcQty);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.sfc), config.InterfaceParams.sfc);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.dcMode), out var dcMode);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.dcMode), (int)config.InterfaceParams.dcMode);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.dcGroup), out var dcGroup);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.dcGroup), config.InterfaceParams.dcGroup);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.dcGroupRevision), out var dcGroupRevision);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.dcGroupRevision), config.InterfaceParams.dcGroupRevision);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.material), out var material);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.material), config.InterfaceParams.material);

                section.Remove(nameof(DataCollectForResourceFAIConfig.InterfaceParams.materialRevision), out var materialRevision);
                section.Add(nameof(DataCollectForResourceFAIConfig.InterfaceParams.materialRevision), config.InterfaceParams.materialRevision);
            });
        }

        public DataCollectForResourceFAIConfig GetDataCollectForResourceFAIConfig()
        {
            var section = GetSection(this.DataCollectForResourceFAIName);
            if (section == null) return null;
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(DataCollectForResourceFAIParams.Site), out var sitevalue) ? sitevalue.GetString() : null;
            var user = section.TryGetValue(nameof(DataCollectForResourceFAIParams.User), out var uservalue) ? uservalue.GetString() : null;
            var operation = section.TryGetValue(nameof(DataCollectForResourceFAIParams.Operation), out var operationvalue) ? operationvalue.GetString() : null;
            var operationRevision = section.TryGetValue(nameof(DataCollectForResourceFAIParams.OperationRevision), out var operationRevisionValue) ? operationRevisionValue.GetString() : "#";
            var activityId = section.TryGetValue(nameof(DataCollectForResourceFAIParams.ActivityId), out var activityIdValue) ? activityIdValue.GetString() : "EAP_WS";
            var recourse = section.TryGetValue(nameof(DataCollectForResourceFAIParams.Resource), out var rescourseValue) ? rescourseValue.GetString() : null;
            var dcGroup = section.TryGetValue(nameof(DataCollectForResourceFAIParams.dcGroup), out var dcGroupValue) ? dcGroupValue.GetString() : null;
            var dcGroupSequence = section.TryGetValue(nameof(DataCollectForResourceFAIParams.dcGroupSequence), out var dcGroupSequenceValue) ? dcGroupSequenceValue.GetString() : null;
            var material = section.TryGetValue(nameof(DataCollectForResourceFAIParams.material), out var materialValue) ? materialValue.GetString() : null;
            var materialRevision = section.TryGetValue(nameof(DataCollectForResourceFAIParams.materialRevision), out var materialRevisionValue) ? materialRevisionValue.GetString() : null;
            var dcGroupRevision = section.TryGetValue(nameof(DataCollectForResourceFAIParams.dcGroupRevision), out var dcGroupRevisionValue) ? dcGroupRevisionValue.GetString() : null;
            var sfc = section.TryGetValue(nameof(DataCollectForResourceFAIParams.sfc), out var sfcValue) ? sfcValue.GetString() : null;
            var dcMode = section.TryGetValue(nameof(DataCollectForResourceFAIParams.dcMode), out var dcModeValue) ? (dcModeEnum)dcModeValue.ToInt() : dcModeEnum.SFC_DCG;


            return new DataCollectForResourceFAIConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new DataCollectForResourceFAIParams
                {
                    Site = site,
                    User = user,
                    Operation = operation,
                    OperationRevision = operationRevision,
                    ActivityId = activityId,
                    Resource = recourse,
                    sfc = sfc,
                    dcGroupSequence = dcGroupSequence,
                    material = material,
                    materialRevision = materialRevision,
                    dcMode = (dcModeEnum)dcMode,
                    dcGroup = dcGroup,
                    dcGroupRevision = dcGroupRevision,
                },
            };
        }

        #endregion

        #region 检验接口

        public void SetMICheckSFCStatusExConfig(MICheckSFCStatusExConfig config, string AutoOpName)
        {
            var section = GetSection(this.MICheckSFCStatusExName, AutoOpName);

            this.UpdateSection(this.MICheckSFCStatusExName, section =>
            {
                section.Remove(nameof(MICheckSFCStatusExConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(MICheckSFCStatusExConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(MICheckSFCStatusExConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(MICheckSFCStatusExConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(MICheckSFCStatusExConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(MICheckSFCStatusExConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(MICheckSFCStatusExConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(MICheckSFCStatusExConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(MICheckSFCStatusExConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(MICheckSFCStatusExConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(MICheckSFCStatusExConfig.InterfaceParams.Operation), out var oldoperation);
                section.Add(nameof(MICheckSFCStatusExConfig.InterfaceParams.Operation), config.InterfaceParams.Operation);

                section.Remove(nameof(MICheckSFCStatusExConfig.InterfaceParams.OperationRevision), out var oldoperationRevision);
                section.Add(nameof(MICheckSFCStatusExConfig.InterfaceParams.OperationRevision), config.InterfaceParams.OperationRevision);

                section.Remove(nameof(MICheckSFCStatusExConfig.InterfaceParams.sfc), out var oldSfcQty);
                section.Add(nameof(MICheckSFCStatusExConfig.InterfaceParams.sfc), config.InterfaceParams.sfc);
            }, AutoOpName);
        }

        public MICheckSFCStatusExConfig GetMICheckSfcStatusExConfig(string opCode = "")
        {
            var section = GetSection(this.MICheckSFCStatusExName, opCode);
            if (section == null) return null;
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(MICheckSFCStatusExParams.Site), out var sitevalue) ? sitevalue.GetString() : null;
            var operation = section.TryGetValue(nameof(MICheckSFCStatusExParams.Operation), out var operationvalue) ? operationvalue.GetString() : null;
            var operationRevision = section.TryGetValue(nameof(MICheckSFCStatusExParams.OperationRevision), out var operationRevisionValue) ? operationRevisionValue.GetString() : "#";
            var sfc = section.TryGetValue(nameof(MICheckSFCStatusExParams.sfc), out var sfcValue) ? sfcValue.GetString() : null;


            return new MICheckSFCStatusExConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new MICheckSFCStatusExParams
                {

                    Site = site,
                    Operation = operation,
                    OperationRevision = operationRevision,
                    sfc = sfc,
                },
            };
        }


        #endregion

        #region 校验贴纸PN及库存

        public void SetMICheckBOMInventoryConfig(MICheckBOMInventoryConfig config, string AutoOpName = "")
        {
            var section = GetSection(this.MICheckBOMInventoryName, AutoOpName);

            this.UpdateSection(this.MICheckBOMInventoryName, section =>
            {
                section.Remove(nameof(MICheckBOMInventoryConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(MICheckBOMInventoryConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(MICheckBOMInventoryConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(MICheckBOMInventoryConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(MICheckBOMInventoryConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(MICheckBOMInventoryConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(MICheckBOMInventoryConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(MICheckBOMInventoryConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.User), out var oldUser);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.User), config.InterfaceParams.User);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.Operation), out var oldoperation);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.Operation), config.InterfaceParams.Operation);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.OperationRevision), out var oldoperationRevision);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.OperationRevision), config.InterfaceParams.OperationRevision);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.sfc), out var oldSfcQty);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.sfc), config.InterfaceParams.sfc);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.M1_PN), out var oldM1_PN);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.M1_PN), config.InterfaceParams.M1_PN);
                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.M2_PN), out var oldM2_PN);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.M2_PN), config.InterfaceParams.M2_PN);
                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.M3_PN), out var oldM3_PN);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.M3_PN), config.InterfaceParams.M3_PN);
                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.M4_PN), out var oldM4_PN);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.M4_PN), config.InterfaceParams.M4_PN);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.modeProcessSFC), out var oldmodeProcessSFC);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.modeProcessSFC), (int)config.InterfaceParams.modeProcessSFC);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.ActivityId), out var oldActivityId);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.ActivityId), config.InterfaceParams.ActivityId);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.category), out var oldcategory);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.category), (int)config.InterfaceParams.category);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.modeCheckOperation), out var oldmodeCheckOperation);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.modeCheckOperation), config.InterfaceParams.modeCheckOperation);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.dataField), out var olddataField);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.dataField), config.InterfaceParams.dataField);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.usage), out var oldusage);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.usage), config.InterfaceParams.usage);

                section.Remove(nameof(MICheckBOMInventoryConfig.InterfaceParams.resource), out var oldresource);
                section.Add(nameof(MICheckBOMInventoryConfig.InterfaceParams.resource), config.InterfaceParams.resource);
            }, AutoOpName);
        }

        public MICheckBOMInventoryConfig GetMICheckBOMInventoryConfig(string AutoOpName = "")
        {
            var section = GetSection(this.MICheckBOMInventoryName, AutoOpName);
            if (section == null) return null;
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(MICheckBOMInventoryParams.Site), out var sitevalue) ? sitevalue.GetString() : null;
            var operation = section.TryGetValue(nameof(MICheckBOMInventoryParams.Operation), out var operationvalue) ? operationvalue.GetString() : null;
            var operationRevision = section.TryGetValue(nameof(MICheckBOMInventoryParams.OperationRevision), out var operationRevisionValue) ? operationRevisionValue.GetString() : "#";
            var sfc = section.TryGetValue(nameof(MICheckBOMInventoryParams.sfc), out var sfcValue) ? sfcValue.GetString() : null;
            var ActivityId = section.TryGetValue(nameof(MICheckBOMInventoryParams.ActivityId), out var ActivityIdValue) ? ActivityIdValue.GetString() : null;
            var category = section.TryGetValue(nameof(MICheckBOMInventoryParams.category), out var categoryValue) ? (Catl.WebServices.MICheckBOMInventory.ObjectAliasEnum)categoryValue.ToInt() : Catl.WebServices.MICheckBOMInventory.ObjectAliasEnum.ACTIVITY;
            var dataField = section.TryGetValue(nameof(MICheckBOMInventoryParams.dataField), out var dataFieldValue) ? dataFieldValue.GetString() : null;
            var usage = section.TryGetValue(nameof(MICheckBOMInventoryParams.usage), out var usageValue) ? usageValue.GetString() : null;
            var modeCheckOperation = section.TryGetValue(nameof(MICheckBOMInventoryParams.modeCheckOperation), out var modeCheckOperationValue) ? modeCheckOperationValue.ToBool() : true;
            var modeProcessSFC = section.TryGetValue(nameof(MICheckBOMInventoryParams.modeProcessSFC), out var modeProcessSFCValue) ? (Catl.WebServices.MICheckBOMInventory.modeProcessSFC)modeProcessSFCValue.ToInt() : Catl.WebServices.MICheckBOMInventory.modeProcessSFC.MODE_NONE;
            var resource = section.TryGetValue(nameof(MICheckBOMInventoryParams.resource), out var resourceValue) ? resourceValue.GetString() : null;
            var user = section.TryGetValue(nameof(MICheckBOMInventoryParams.User), out var UserValue) ? UserValue.GetString() : null;
            var M1_PN = section.TryGetValue(nameof(MICheckBOMInventoryParams.M1_PN), out var M1_PNValue) ? M1_PNValue.GetString() : null;
            var M2_PN = section.TryGetValue(nameof(MICheckBOMInventoryParams.M2_PN), out var M2_PNValue) ? M2_PNValue.GetString() : null;
            var M3_PN = section.TryGetValue(nameof(MICheckBOMInventoryParams.M3_PN), out var M3_PNValue) ? M3_PNValue.GetString() : null;
            var M4_PN = section.TryGetValue(nameof(MICheckBOMInventoryParams.M4_PN), out var M4_PNValue) ? M4_PNValue.GetString() : null;


            return new MICheckBOMInventoryConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new MICheckBOMInventoryParams
                {
                    Site = site,
                    Operation = operation,
                    OperationRevision = operationRevision,
                    sfc = sfc,
                    M1_PN = M1_PN,
                    M2_PN = M2_PN,
                    M3_PN = M3_PN,
                    M4_PN = M4_PN,
                    ActivityId = ActivityId,
                    category = category,
                    dataField = dataField,
                    usage = usage,
                    modeCheckOperation = modeCheckOperation,
                    modeProcessSFC = modeProcessSFC,
                    resource = resource,
                    User = user,
                },
            };
        }

        #endregion

        #region 组装

        public void SetMiAssembleAndCollectDataForSfcConfig(MIAssembleAndCollectDataForSfcConfig config, string opCode = "")
        {
            var section = GetSection(this.MiAssembleAndCollectDataForSfcName, opCode);

            this.UpdateSection(this.MiAssembleAndCollectDataForSfcName, section =>
            {
                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.Operation), out var oldoperation);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.Operation), config.InterfaceParams.Operation);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.OperationRevision), out var oldoperationRevision);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.OperationRevision), config.InterfaceParams.OperationRevision);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.sfc), out var oldSfcQty);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.sfc), config.InterfaceParams.sfc);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.modeProcessSFC), out var oldmodeProcessSFC);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.modeProcessSFC), (int)config.InterfaceParams.modeProcessSFC);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.ActivityId), out var oldActivityId);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.ActivityId), config.InterfaceParams.ActivityId);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.partialAssembly), out var oldpartialAssembly);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.partialAssembly), (bool)config.InterfaceParams.partialAssembly);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.resource), out var oldresource);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.resource), config.InterfaceParams.resource);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.User), out var oldUser);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.User), config.InterfaceParams.User);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.dcGroup), out var dcGroup);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.dcGroup), config.InterfaceParams.dcGroup);

                section.Remove(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.dcGroupRevision), out var dcGroupRevision);
                section.Add(nameof(MIAssembleAndCollectDataForSfcConfig.InterfaceParams.dcGroupRevision), config.InterfaceParams.dcGroupRevision);
            }, opCode);
        }

        public MIAssembleAndCollectDataForSfcConfig GetMiAssembleAndCollectDataForSfcConfig(string opCode = "")
        {
            var section = GetSection(this.MiAssembleAndCollectDataForSfcName, opCode);
            if (section == null) return null;
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.Site), out var sitevalue) ? sitevalue.GetString() : null;
            var operation = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.Operation), out var operationvalue) ? operationvalue.GetString() : null;
            var operationRevision = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.OperationRevision), out var operationRevisionValue) ? operationRevisionValue.GetString() : "#";
            var sfc = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.sfc), out var sfcValue) ? sfcValue.GetString() : null;
            var partialAssembly = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.partialAssembly), out var partialAssemblyValue) ? partialAssemblyValue.ToBool() : true;
            var ActivityId = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.ActivityId), out var ActivityIdValue) ? ActivityIdValue.GetString() : null;
            var modeProcessSFC = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.modeProcessSFC), out var modeProcessSFCValue) ? (Catl.WebServices.AssembleAndCollectDataForSfc.dataCollectForSfcModeProcessSfc)modeProcessSFCValue.ToInt() : Catl.WebServices.AssembleAndCollectDataForSfc.dataCollectForSfcModeProcessSfc.MODE_NONE;
            var resource = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.resource), out var resourceValue) ? resourceValue.GetString() : null;
            var user = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.User), out var UserValue) ? UserValue.GetString() : null;
            var dcGroup = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.dcGroup), out var dcGroupValue) ? dcGroupValue.GetString() : null;
            var dcGroupRevision = section.TryGetValue(nameof(MIAssembleAndCollectDataForSfcParams.dcGroupRevision), out var dcGroupRevisionValue) ? dcGroupRevisionValue.GetString() : null;


            return new MIAssembleAndCollectDataForSfcConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new MIAssembleAndCollectDataForSfcParams
                {
                    Site = site,
                    Operation = operation,
                    OperationRevision = operationRevision,
                    sfc = sfc,
                    ActivityId = ActivityId,
                    partialAssembly = partialAssembly,
                    modeProcessSFC = modeProcessSFC,
                    User = user,
                    resource = resource,
                    dcGroup = dcGroup,
                    dcGroupRevision = dcGroupRevision,
                },
            };
        }

        #endregion

        #region 收数

        public void SetDataCollectForMoudleTestConfig(DataCollectForMoudleTestConfig config, string AutoOpName)
        {
            var section = GetSection(this.DataCollectForMoudleTestName, AutoOpName);

            this.UpdateSection(this.DataCollectForMoudleTestName, section =>
            {
                section.Remove(nameof(DataCollectForMoudleTestConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(DataCollectForMoudleTestConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(DataCollectForMoudleTestConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(DataCollectForMoudleTestConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(DataCollectForMoudleTestConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(DataCollectForMoudleTestConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(DataCollectForMoudleTestConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(DataCollectForMoudleTestConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.Operation), out var oldoperation);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.Operation), config.InterfaceParams.Operation);

                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.OperationRevision), out var oldoperationRevision);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.OperationRevision), config.InterfaceParams.OperationRevision);

                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.sfc), out var oldSfcQty);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.sfc), config.InterfaceParams.sfc);

                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.modeProcessSFC), out var oldmodeProcessSFC);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.modeProcessSFC), (int)config.InterfaceParams.modeProcessSFC);

                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.ActivityId), out var oldActivityId);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.ActivityId), config.InterfaceParams.ActivityId);

                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.dcGroup), out var olddcGroup);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.dcGroup), config.InterfaceParams.dcGroup);
                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.dcGroupRevision), out var olddcGroupRevision);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.dcGroupRevision), config.InterfaceParams.dcGroupRevision);

                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.Resource), out var oldResource);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.Resource), config.InterfaceParams.Resource);

                section.Remove(nameof(DataCollectForMoudleTestConfig.InterfaceParams.User), out var oldUser);
                section.Add(nameof(DataCollectForMoudleTestConfig.InterfaceParams.User), config.InterfaceParams.User);
            }, AutoOpName);
        }

        public DataCollectForMoudleTestConfig GetDataCollectForMoudleTestConfig(string opCode = "")
        {
            var section = GetSection(this.DataCollectForMoudleTestName, opCode);
            if (section == null) return null;
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(DataCollectForMoudleTestParams.Site), out var sitevalue) ? sitevalue.GetString() : null;
            var operation = section.TryGetValue(nameof(DataCollectForMoudleTestParams.Operation), out var operationvalue) ? operationvalue.GetString() : null;
            var operationRevision = section.TryGetValue(nameof(DataCollectForMoudleTestParams.OperationRevision), out var operationRevisionValue) ? operationRevisionValue.GetString() : "#";
            var sfc = section.TryGetValue(nameof(DataCollectForMoudleTestParams.sfc), out var sfcValue) ? sfcValue.GetString() : null;
            var Resource = section.TryGetValue(nameof(DataCollectForMoudleTestParams.Resource), out var ResourceValue) ? ResourceValue.GetString() : null;
            var ActivityId = section.TryGetValue(nameof(DataCollectForMoudleTestParams.ActivityId), out var ActivityIdValue) ? ActivityIdValue.GetString() : null;
            var modeProcessSFC = section.TryGetValue(nameof(DataCollectForMoudleTestParams.modeProcessSFC), out var modeProcessSFCValue) ? (Catl.WebServices.MachineIntegrationServices.ModeProcessSfc)modeProcessSFCValue.ToInt() : Catl.WebServices.MachineIntegrationServices.ModeProcessSfc.MODE_NONE;
            var dcGroup = section.TryGetValue(nameof(DataCollectForMoudleTestParams.dcGroup), out var dcGroupValue) ? dcGroupValue.GetString() : null;
            var dcGroupRevision = section.TryGetValue(nameof(DataCollectForMoudleTestParams.dcGroupRevision), out var dcGroupRevisionValue) ? dcGroupRevisionValue.GetString() : null;
            var user = section.TryGetValue(nameof(DataCollectForMoudleTestParams.User), out var UserValue) ? UserValue.GetString() : null;

            return new DataCollectForMoudleTestConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new DataCollectForMoudleTestParams
                {

                    Site = site,
                    Operation = operation,
                    OperationRevision = operationRevision,
                    sfc = sfc,
                    ActivityId = ActivityId,
                    Resource = Resource,
                    User = user,
                    modeProcessSFC = modeProcessSFC,
                    dcGroup = dcGroup,
                    dcGroupRevision = dcGroupRevision,
                },
            };
        }

        #endregion
        #region 根据物料查询条码
        public FindSfcByInventoryMIFindCustomAndSfcDataConfig GetFindSfcByInventoryConfig(string opCode = "")
        {

            var section = GetSection(this.FindSfcByInventoryName, opCode);
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.Site), out var sitevalue) ? sitevalue.GetString() : "";
            var user = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.User), out var uservalue) ? uservalue.GetString() : "";
            var operation = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.Operation), out var operationvalue) ? operationvalue.GetString() : "";
            var operationRevision = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.OperationRevision), out var operationRevisionValue) ? operationRevisionValue.GetString() : "#";
            var activity = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.Activity), out var activityIdValue) ? activityIdValue.GetString() : "";
            var findSfcByInventory = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.FindSfcByInventory), out var findSfcByInventoryvalue) ? findSfcByInventoryvalue.ToBool() : false;
            var isGetXY = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.IsGetXY), out var isGetXYvalue) ? isGetXYvalue.ToBool() : false;
            var isGetCSC = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.IsGetCSC), out var isGetCSCvalue) ? isGetCSCvalue.ToBool() : false;
            var mode = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.Mode), out var modevalue) ? (Catl.WebServices.MiFindCustomAndSfcDataServiceService.FmodeProcessSFC)modevalue.ToInt() : Catl.WebServices.MiFindCustomAndSfcDataServiceService.FmodeProcessSFC.MODE_NONE;
            var sfcOrder = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.SfcOrder), out var sfcOrdervalue) ? sfcOrdervalue.GetString() : "";
            var targetOrder = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.TargetOrder), out var targetOrdervalue) ? targetOrdervalue.GetString() : "";
            var checkInventoryAB = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.CheckInventoryAB), out var checkInventoryABvalue) ? checkInventoryABvalue.GetString() : "";
            var showMarking = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.ShowMarking), out var showMarkingvalue) ? showMarkingvalue.ToBool() : false;
            var showMarkingSpecified = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.ShowMarkingSpecified), out var showMarkingSpecifiedvalue) ? showMarkingSpecifiedvalue.ToBool() : false;
            var findInventoryByGbt = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.FindInventoryByGbt), out var findInventoryByGbtvalue) ? findInventoryByGbtvalue.GetString() : "";
            var masterData = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.MasterData), out var masterDatavalue) ? (Catl.WebServices.MiFindCustomAndSfcDataServiceService.ObjectAliasEnum)masterDatavalue.ToInt() : Catl.WebServices.MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ITEM;
            var categoryData = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.CategoryData), out var categoryDatavalue) ? (Catl.WebServices.MiFindCustomAndSfcDataServiceService.ObjectAliasEnum)categoryDatavalue.ToInt() : Catl.WebServices.MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ITEM;
            var dataField = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.DataField), out var dataFieldvalue) ? dataFieldvalue.GetString() : "";
            var resource = section.TryGetValue(nameof(MiFindCustomAndSfcDataParamers.Resource), out var resourcevalue) ? resourcevalue.GetString() : "";
            return new FindSfcByInventoryMIFindCustomAndSfcDataConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new MiFindCustomAndSfcDataParamers
                {
                    Site = site,
                    User = user,
                    Operation = operation,
                    OperationRevision = operationRevision,
                    Activity = activity,
                    Mode = mode,
                    MasterData = masterData,
                    CategoryData = categoryData,
                    DataField = dataField,
                    MasterDataArray = new Catl.WebServices.MiFindCustomAndSfcDataServiceService.ObjectAliasEnum[1] { masterData },
                    CustomDataArray = new Catl.WebServices.MiFindCustomAndSfcDataServiceService.customDataInParametricData[1] { new Catl.WebServices.MiFindCustomAndSfcDataServiceService.customDataInParametricData { category = categoryData, dataField = dataField } },
                    SfcOrder = sfcOrder,
                    TargetOrder = targetOrder,
                    CheckInventoryAB = checkInventoryAB,
                    ShowMarking = showMarking,
                    ShowMarkingSpecified = showMarkingSpecified,
                    FindInventoryByGbt = findInventoryByGbt,
                    FindSfcByInventory = findSfcByInventory,
                    IsGetXY = isGetXY,
                    IsGetCSC = isGetCSC,
                    Resource = resource,
                }
            };
        }

        public void SetFindSfcByInventoryConfig(FindSfcByInventoryMIFindCustomAndSfcDataConfig config, string AutoOpName)
        {
            var section = GetSection(this.FindSfcByInventoryName, AutoOpName);

            this.UpdateSection(this.FindSfcByInventoryName, section =>
            {
                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.User), out var olduser);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.User), config.InterfaceParams.User);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Operation), out var oldoperation);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Operation), config.InterfaceParams.Operation);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.OperationRevision), out var oldoperationRevision);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.OperationRevision), config.InterfaceParams.OperationRevision);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Activity), out var oldActivity);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Activity), config.InterfaceParams.Activity);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.SfcOrder), out var oldSfcOrder);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.SfcOrder), config.InterfaceParams.SfcOrder);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.TargetOrder), out var oldTargetOrder);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.TargetOrder), config.InterfaceParams.TargetOrder);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.FindSfcByInventory), out var oldFindSfcByInventory);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.FindSfcByInventory), config.InterfaceParams.FindSfcByInventory);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.CategoryData), out var oldCategoryData);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.CategoryData), (int)config.InterfaceParams.CategoryData);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.DataField), out var oldDataField);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.DataField), config.InterfaceParams.DataField);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.MasterData), out var oldMasterData);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.MasterData), (int)config.InterfaceParams.MasterData);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Mode), out var oldMode);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Mode), (int)config.InterfaceParams.Mode);

                section.Remove(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Resource), out var oldResource);
                section.Add(nameof(FindSfcByInventoryMIFindCustomAndSfcDataConfig.InterfaceParams.Resource), config.InterfaceParams.Resource);
            }, AutoOpName);
        }

        #endregion
        #region 首件
        public DataCollectForResourceInspectConfig GetDataCollectForResourceInspectConfig(string? opCode = "")
        {

            var section = GetSection(this.DataCollectForResourceInspect, opCode);
            if (section == null) return null;
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(MIFindCustomAndSfcParams.Site), out var sitevalue) ? sitevalue.GetString() : null;
            var user = section.TryGetValue(nameof(MIFindCustomAndSfcParams.User), out var uservalue) ? uservalue.GetString() : null;
            var operation = section.TryGetValue(nameof(MIFindCustomAndSfcParams.Operation), out var operationvalue) ? operationvalue.GetString() : null;
            var operationRevision = section.TryGetValue(nameof(MIFindCustomAndSfcParams.OperationRevision), out var operationRevisionValue) ? operationRevisionValue.GetString() : "#";
            var activityId = section.TryGetValue(nameof(MIFindCustomAndSfcParams.ActivityId), out var activityIdValue) ? activityIdValue.GetString() : "EAP_WS";
            var recourse = section.TryGetValue(nameof(MIFindCustomAndSfcParams.Resource), out var rescourseValue) ? rescourseValue.GetString() : null;
            var dcGroup = section.TryGetValue(nameof(DataCollectForResourceInspectParams.DcGroup), out var dcGroupValue) ? dcGroupValue.GetString() : null;
            var dcGroupRevision = section.TryGetValue(nameof(DataCollectForResourceInspectParams.DcGroupRevision), out var dcGroupRevisionValue) ? dcGroupRevisionValue.GetString() : null;
            var dcSequence = section.TryGetValue(nameof(DataCollectForResourceInspectParams.DcSequence), out var dcSequenceValue) ? dcSequenceValue.GetString() : null;
            var executeMode = section.TryGetValue(nameof(DataCollectForResourceInspectParams.ExecuteMode), out var executeModeValue) ? executeModeValue.GetString() : null;
            var dcMode = section.TryGetValue(nameof(DataCollectForResourceInspectParams.DcMode), out var dcModeValue) ? dcModeValue.GetString() : null;


            return new DataCollectForResourceInspectConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new DataCollectForResourceInspectParams
                {
                    Site = site,
                    User = user,
                    Operation = operation,
                    OperationRevision = operationRevision,
                    ActivityId = activityId,
                    Resource = recourse,

                    DcGroup = dcGroup,
                    DcGroupRevision = dcGroupRevision,
                    DcSequence = dcSequence,
                    DcMode = dcMode,
                    ExecuteMode = executeMode

                },
            };
        }

        public void SetDataCollectForResourceInspectConfig(DataCollectForResourceInspectConfig config, string AutoOpName)
        {
            var section = GetSection(this.DataCollectForResourceInspect, AutoOpName);

            this.UpdateSection(this.DataCollectForResourceInspect, section =>
            {
                section.Remove(nameof(DataCollectForResourceInspectConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(DataCollectForResourceInspectConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(DataCollectForResourceInspectConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(DataCollectForResourceInspectConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(DataCollectForResourceInspectConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(DataCollectForResourceInspectConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(DataCollectForResourceInspectConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(DataCollectForResourceInspectConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.User), out var olduser);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.User), config.InterfaceParams.User);

                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.Operation), out var oldoperation);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.Operation), config.InterfaceParams.Operation);

                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.OperationRevision), out var oldoperationRevision);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.OperationRevision), config.InterfaceParams.OperationRevision);



                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.ActivityId), out var oldActivityId);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.ActivityId), config.InterfaceParams.ActivityId);

                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.DcGroup), out var olddcGroup);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.DcGroup), config.InterfaceParams.DcGroup);

                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.DcGroupRevision), out var dcGroupRevision);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.DcGroupRevision), config.InterfaceParams.DcGroupRevision);

                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.DcSequence), out var olddcSequence);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.DcSequence), config.InterfaceParams.DcSequence);

                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.DcMode), out var olddcMode);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.DcMode), config.InterfaceParams.DcMode);

                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.ExecuteMode), out var oldexecuteMode);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.ExecuteMode), config.InterfaceParams.ExecuteMode);



                section.Remove(nameof(DataCollectForResourceInspectConfig.InterfaceParams.Resource), out var oldResource);
                section.Add(nameof(DataCollectForResourceInspectConfig.InterfaceParams.Resource), config.InterfaceParams.Resource);
            }, AutoOpName);
        }

        #endregion

        #region 校验模组库存状态

        public MiCheckInventoryAttributesConfig GetMiCheckInventoryAttributesConfig(string? opCode = "")
        {

            var section = GetSection(this.MiCheckInventoryAttributesName, opCode);
            if (section == null) return null;
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(MiCheckInventoryAttributesParams.Site), out var sitevalue) ? sitevalue.GetString() : null;
            var user = section.TryGetValue(nameof(MiCheckInventoryAttributesParams.User), out var uservalue) ? uservalue.GetString() : null;
            var operation = section.TryGetValue(nameof(MiCheckInventoryAttributesParams.Operation), out var operationvalue) ? operationvalue.GetString() : null;
            var operationRevision = section.TryGetValue(nameof(MiCheckInventoryAttributesParams.OperationRevision), out var operationRevisionValue) ? operationRevisionValue.GetString() : "#";
            var activityId = section.TryGetValue(nameof(MiCheckInventoryAttributesParams.ActivityId), out var activityIdValue) ? activityIdValue.GetString() : "EAP_WS";
            var recourse = section.TryGetValue(nameof(MiCheckInventoryAttributesParams.Resource), out var rescourseValue) ? rescourseValue.GetString() : null;
            var modeCheckInventory = section.TryGetValue(nameof(MiCheckInventoryAttributesParams.modeCheckInventory), out var modeCheckInventoryValue) ? (Catl.WebServices.MICheckInventoryAttribute.modeCheckInventory)modeCheckInventoryValue.ToInt() : Catl.WebServices.MICheckInventoryAttribute.modeCheckInventory.MODE_NONE;

            return new MiCheckInventoryAttributesConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new MiCheckInventoryAttributesParams
                {
                    Site = site,
                    User = user,
                    Operation = operation,
                    OperationRevision = operationRevision,
                    ActivityId = activityId,
                    Resource = recourse,
                    modeCheckInventory = modeCheckInventory
                },
            };
        }

        public void SetMiCheckInventoryAttributesConfig(MiCheckInventoryAttributesConfig config, string AutoOpName)
        {
            var section = GetSection(this.MiCheckInventoryAttributesName, AutoOpName);

            this.UpdateSection(this.MiCheckInventoryAttributesName, section =>
            {
                section.Remove(nameof(MiCheckInventoryAttributesConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(MiCheckInventoryAttributesConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(MiCheckInventoryAttributesConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(MiCheckInventoryAttributesConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(MiCheckInventoryAttributesConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(MiCheckInventoryAttributesConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(MiCheckInventoryAttributesConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(MiCheckInventoryAttributesConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.User), out var olduser);
                section.Add(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.User), config.InterfaceParams.User);

                section.Remove(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.Operation), out var oldoperation);
                section.Add(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.Operation), config.InterfaceParams.Operation);

                section.Remove(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.OperationRevision), out var oldoperationRevision);
                section.Add(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.OperationRevision), config.InterfaceParams.OperationRevision);


                section.Remove(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.ActivityId), out var oldActivityId);
                section.Add(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.ActivityId), config.InterfaceParams.ActivityId);


                section.Remove(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.Resource), out var oldResource);
                section.Add(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.Resource), config.InterfaceParams.Resource);

                section.Remove(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.modeCheckInventory), out var oldModelCheckInventory);
                section.Add(nameof(MiCheckInventoryAttributesConfig.InterfaceParams.modeCheckInventory), (int)config.InterfaceParams.modeCheckInventory);
            }, AutoOpName);
        }

        #endregion

        #region 获取参数值

        public GetParametricValueConfig GetGetParametricValueConfig(string? opCode = "")
        {

            var section = GetSection(this.GetParametricValueName, opCode);
            if (section == null) return null;
            var conn = this.GetMesConnectionParams(section);

            var site = section.TryGetValue(nameof(GetParametricValueParams.Site), out var sitevalue) ? sitevalue.GetString() : null;
            var user = section.TryGetValue(nameof(GetParametricValueParams.User), out var uservalue) ? uservalue.GetString() : null;
            var recourse = section.TryGetValue(nameof(MiCheckInventoryAttributesParams.Resource), out var rescourseValue) ? rescourseValue.GetString() : null;

            return new GetParametricValueConfig
            {
                ConnectionParams = conn,
                InterfaceParams = new GetParametricValueParams
                {
                    Site = site,
                    User = user,
                    Resource = recourse,
                },
            };
        }

        public void SetGetParametricValueConfig(GetParametricValueConfig config, string AutoOpName)
        {
            var section = GetSection(this.GetParametricValueName, AutoOpName);

            this.UpdateSection(this.GetParametricValueName, section =>
            {
                section.Remove(nameof(GetParametricValueConfig.ConnectionParams.Url), out var oldurl);
                section.Add(nameof(GetParametricValueConfig.ConnectionParams.Url), config.ConnectionParams.Url);

                section.Remove(nameof(GetParametricValueConfig.ConnectionParams.Timeout), out var oldtimeout);
                section.Add(nameof(GetParametricValueConfig.ConnectionParams.Timeout), config.ConnectionParams.Timeout);

                section.Remove(nameof(GetParametricValueConfig.ConnectionParams.UserName), out var oldusername);
                section.Add(nameof(GetParametricValueConfig.ConnectionParams.UserName), config.ConnectionParams.UserName);

                section.Remove(nameof(GetParametricValueConfig.ConnectionParams.Password), out var oldpasswd);
                section.Add(nameof(GetParametricValueConfig.ConnectionParams.Password), config.ConnectionParams.Password);

                section.Remove(nameof(GetParametricValueConfig.InterfaceParams.Site), out var oldsite);
                section.Add(nameof(GetParametricValueConfig.InterfaceParams.Site), config.InterfaceParams.Site);

                section.Remove(nameof(GetParametricValueConfig.InterfaceParams.User), out var olduser);
                section.Add(nameof(GetParametricValueConfig.InterfaceParams.User), config.InterfaceParams.User);

                section.Remove(nameof(GetParametricValueConfig.InterfaceParams.Resource), out var oldResource);
                section.Add(nameof(GetParametricValueConfig.InterfaceParams.Resource), config.InterfaceParams.Resource);
            }, AutoOpName);
        }

        #endregion
    }
}
