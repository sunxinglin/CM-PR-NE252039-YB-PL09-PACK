using System.ServiceModel;

using Catl.HostComputer.CommonServices.Mes;
using Catl.WebServices.AssembleAndCollectDataForSfc;
using Catl.WebServices.DataCollectForResourceInspect;
using Catl.WebServices.GetParametricValueServiceService;
using Catl.WebServices.MachineIntegrationServices;
using Catl.WebServices.MICheckBOMInventory;
using Catl.WebServices.MICheckInventoryAttribute;
using Catl.WebServices.MICheckSFCStatusEx;
using Catl.WebServices.MIFindCustomAndSfcData;

using MediatR;

using Newtonsoft.Json;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.CATL;

using machineIntegrationParametricData = Catl.WebServices.DataCollectForResourceInspect.machineIntegrationParametricData;
using modeProcessSFC = Catl.WebServices.MIFindCustomAndSfcData.modeProcessSFC;
using ObjectAliasEnum = Catl.WebServices.MIFindCustomAndSfcData.ObjectAliasEnum;
using ParameterDataType = Catl.WebServices.MachineIntegrationServices.ParameterDataType;

namespace Yee.Services.CatlMesInvoker
{
    public class CatlMesInvoker : ICatlMesInvoker
    {

        private readonly CatlMesIniConfigHelper _iniHelper;
        private readonly IMediator _mediator;
        private readonly ISfcInvocationLogger _sfclogger;
        private readonly ILogger<CatlMesInvoker> _logger;
        private readonly ICatlResourceProvider _resourceProvider;

        public CatlMesInvoker(CatlMesIniConfigHelper iniHelper,
            IMediator mediator,
            ISfcInvocationLogger sfclogger,
            ILogger<CatlMesInvoker> logger,
            ICatlResourceProvider resourceProvider)
        {
            this._iniHelper = iniHelper;
            this._mediator = mediator;
            this._sfclogger = sfclogger;
            this._logger = logger;
            //this._taskUpMESDataService = taskUpMESDataService;
            this._resourceProvider = resourceProvider;
        }
        private async Task WriteExcelAsync(SfcInvocationLogging logging, string svcname, string svcdesc, string equipId, string sfcCode = "")
        {
            try
            {
                var pathD = $"D:/MesLog/{svcname}_{svcdesc}";
                if (!Directory.Exists(pathD))
                    Directory.CreateDirectory(pathD);

                pathD = pathD.Replace($"{AppDomain.CurrentDomain.FriendlyName}/", "");
                string text3 = $"{logging.SentAt:yyyyMMdd}_{sfcCode}.xlsx";
                text3 = (string.IsNullOrEmpty(equipId) ? text3 : (equipId + "_" + text3));
                pathD = Path.Combine(pathD, text3);
                await this._sfclogger.WriteLogAsync(logging, pathD);
            }
            catch (Exception ex)
            {
                this._logger.LogError($"写MES调用日志（Excel）出错：{ex.Message}\r\n{ex.StackTrace}\r\n消息内容={JsonConvert.SerializeObject(logging)}");
            }
        }

        /// <summary>
        /// 进站
        /// </summary>
        /// <param name="sfc">pack条码</param>
        /// <param name="isStatusChange">是否需要需要按照配置文件执行true：根据配置文件，false: 永远为MODE_NONE</param>
        /// <param name="StationCode">工位编码</param>
        /// <returns></returns>
        public async Task<CatlMESReponse> MiFindCustomAndSfcData(string sfc, modeProcessSFC? modeProcessSFC, string StationCode = "")
        {
            var response = new CatlMESReponse();
            int resultCode = 0;
            try
            {
                var config = _iniHelper.GetMIFindCustomAndSfcDataConfig(StationCode);
                if (config == null)
                    return new CatlMESReponse { code = 500, message = "未配置接口文件，不调用Catl MES接口" };
                if (string.IsNullOrEmpty(config.ConnectionParams.Url))
                {
                    throw new Exception($"调用CATL WebService时URL不可为空，请检查MesCfg.ini配置文件！");
                }
                var assemblereq = new findCustomAndSfcDataRequest
                {
                    operation = config.InterfaceParams.Operation,
                    operationRevision = config.InterfaceParams.OperationRevision,
                    site = config.InterfaceParams.Site,
                    activity = config.InterfaceParams.ActivityId,
                    resource = config.InterfaceParams.Resource,
                    user = config.InterfaceParams.User,
                    findSfcByInventory = config.InterfaceParams.findSfcByInventory,
                    sfcOrder = config.InterfaceParams.sfcOrder,
                    targetOrder = config.InterfaceParams.targetOrder,
                    //modeProcessSFC = isStatusChange ? Catl.WebServices.MIFindCustomAndSfcData.modeProcessSFC.MODE_START_SFC : Catl.WebServices.MIFindCustomAndSfcData.modeProcessSFC.MODE_NONE,
                    modeProcessSFC = modeProcessSFC ?? config.InterfaceParams.modeProcessSfc,
                    sfc = sfc,
                    customDataArray = new customDataInParametricData[1] { new customDataInParametricData { category = config.InterfaceParams.category, dataField = config.InterfaceParams.dataField } },
                    masterDataArray = new ObjectAliasEnum[1] { config.InterfaceParams.masterDataArray }
                };

                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.Security.Mode = config.ConnectionParams.Url.StartsWith("https") ? BasicHttpSecurityMode.TransportWithMessageCredential : BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.SendTimeout = TimeSpan.FromMilliseconds(config.ConnectionParams.Timeout);

                var address = new EndpointAddress(config.ConnectionParams.Url);
                using var scf = new ChannelFactory<MiFindCustomAndSfcDataService>(binding, address);
                scf.Credentials.UserName.UserName = config.ConnectionParams.UserName;
                scf.Credentials.UserName.Password = config.ConnectionParams.Password;
                scf.Endpoint.EndpointBehaviors.Add(new UsePreAuthenticateHttpClientEndpointBehavior(scf.Credentials.UserName.UserName, scf.Credentials.UserName.Password));
                var channel = scf.CreateChannel();

                var logging = new SfcInvocationLogging
                {
                    SentAt = DateTime.Now,
                    SfcNumber = sfc,
                };
                try
                {
                    var req = new miFindCustomAndSfcDataRequest(new miFindCustomAndSfcData
                    {
                        FindCustomAndSfcDataRequest = assemblereq
                    });
                    var resp = await channel.miFindCustomAndSfcDataAsync(req);
                    var result = resp.miFindCustomAndSfcDataResponse.@return;
                    resultCode = result.code;
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespCode = result.code.ToString();
                    logging.RespInfo = JsonConvert.SerializeObject(result);
                    string goodsPN = string.Empty;
                    if (result.code == 0 && result.masterDataArray != null && result.masterDataArray.Length > 0)
                    {
                        goodsPN = result.masterDataArray[0].value;
                    }
                    return new CatlMESReponse { code = resultCode, message = result.message, BarCode_GoodsPN = goodsPN };
                }
                catch (Exception ex)
                {
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespInfo = JsonConvert.SerializeObject(ex.Message);
                    return new CatlMESReponse { code = resultCode, message = ex.Message };
                }
                finally
                {
                    var svcname = _iniHelper.MIFindCustomAndSfcDataConfigName;
                    var svcdesc = "";
                    WriteExcelAsync(logging, svcname, svcdesc, config.InterfaceParams.Resource);
                }
            }
            catch (Exception ex)
            {
                return new CatlMESReponse { code = resultCode, message = ex.Message };
            }
        }

        /// <summary>
        /// 校验贴纸
        /// </summary>
        /// <param name="sfc">pack条码</param>
        /// <param name="MatteryPN">物料PN</param>
        /// <param name="Barcode">批次/库存条码</param>
        /// <param name="useNum">用量</param>
        /// <param name="stationCode">工位</param>
        /// <returns></returns>
        public async Task<CatlMESReponse> MICheckBOMInventory(string sfc, string MatteryPN, string Barcode, int useNum = 1, string stationCode = "")
        {
            try
            {
                var config = this._iniHelper.GetMICheckBOMInventoryConfig(stationCode);
                if (config == null)
                    return new CatlMESReponse { code = 0, message = "未配置接口文件，不调用Catl MES接口" };
                var assemblereq = new checkBOMInventoryRequest
                {
                    activity = config.InterfaceParams.ActivityId,
                    modeCheckOperation = config.InterfaceParams.modeCheckOperation,
                    modeProcessSFC = config.InterfaceParams.modeProcessSFC,
                    resource = config.InterfaceParams.resource,
                    operation = config.InterfaceParams.Operation,
                    operationRevision = config.InterfaceParams.OperationRevision,
                    site = config.InterfaceParams.Site,
                    sfc = sfc,
                    user = config.InterfaceParams.User,
                    customDataArray = new customData[1] { new customData { category = config.InterfaceParams.category, dataField = config.InterfaceParams.dataField, usage = config.InterfaceParams.usage } },
                    inventoryDataArray = new checkBomInventoryData[1] { new checkBomInventoryData { component = MatteryPN, inventory = Barcode, qty = useNum.ToString() } },
                };

                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.Security.Mode = config.ConnectionParams.Url.StartsWith("https") ? BasicHttpSecurityMode.TransportWithMessageCredential : BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.SendTimeout = TimeSpan.FromMilliseconds(config.ConnectionParams.Timeout);

                var address = new EndpointAddress(config.ConnectionParams.Url);
                using var scf = new ChannelFactory<MiCheckBOMInventoryService>(binding, address);
                scf.Credentials.UserName.UserName = config.ConnectionParams.UserName;
                scf.Credentials.UserName.Password = config.ConnectionParams.Password;
                scf.Endpoint.EndpointBehaviors.Add(new UsePreAuthenticateHttpClientEndpointBehavior(scf.Credentials.UserName.UserName, scf.Credentials.UserName.Password));
                var channel = scf.CreateChannel();
                var logging = new SfcInvocationLogging
                {
                    SentAt = DateTime.Now,
                    SfcNumber = sfc,
                };
                try
                {
                    var req = new miCheckBOMInventoryRequest(new miCheckBOMInventory
                    {
                        CheckBOMInventoryRequest = assemblereq
                    });
                    var resp = await channel.miCheckBOMInventoryAsync(req);
                    var result = resp.miCheckBOMInventoryResponse.@return;
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespCode = result.code.ToString();
                    logging.RespInfo = JsonConvert.SerializeObject(result);
                    return new CatlMESReponse { code = result.code, message = result.message };
                }
                catch (Exception ex)
                {
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespInfo = JsonConvert.SerializeObject(ex.Message);
                    return new CatlMESReponse { code = 1, message = ex.Message };
                }
                finally
                {

                    var svcname = this._iniHelper.MICheckBOMInventoryName;
                    var svcdesc = "";
                    this.WriteExcelAsync(logging, svcname, svcdesc, config.InterfaceParams.resource);
                }
            }

            catch (Exception ex)
            {
                return new CatlMESReponse { code = 1, message = ex.Message };
            }
        }

        /// <summary>
        /// 组装物料
        /// </summary>
        /// <param name="bomDatas"></param>
        /// <param name="sfc"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        public async Task<CatlMESReponse> MiAssembleAndCollectDataForSfc(IList<BomData> bomDatas, string sfc, string stationCode = "")
        {
            try
            {
                var config = this._iniHelper.GetMiAssembleAndCollectDataForSfcConfig(stationCode);
                if (config == null)
                    return new CatlMESReponse { code = 0, message = "未配置接口文件，不调用Catl MES接口" };
                if (bomDatas == null || bomDatas.Count == 0)
                {
                    return new CatlMESReponse { code = 0, message = "没有组装数据，不调用Catl MES接口" };
                }
                var assemblereq = new assembleAndCollectDataForSfcRequest
                {
                    modeProcessSFC = config.InterfaceParams.modeProcessSFC,
                    resource = config.InterfaceParams.resource,
                    operation = config.InterfaceParams.Operation,
                    operationRevision = config.InterfaceParams.OperationRevision,
                    site = config.InterfaceParams.Site,
                    sfc = sfc,
                    user = config.InterfaceParams.User,
                    activityId = config.InterfaceParams.ActivityId,
                    partialAssembly = config.InterfaceParams.partialAssembly,
                    dcGroup = config.InterfaceParams.dcGroup,
                    dcGroupRevision = config.InterfaceParams.dcGroupRevision,
                };

                List<miInventoryData> miInventoryDatas = new List<miInventoryData>();

                foreach (var Scan in bomDatas)
                {
                    string outerParamName = string.Empty;
                    if (Scan.TracingType == TracingTypeEnum.扫库存)
                    {
                        miInventoryDatas.Add(new miInventoryData
                        {
                            inventory = Scan.InternalCode,
                            qty = "1"
                        });
                        outerParamName = Scan.InternalCode;
                    }
                    if (Scan.TracingType == TracingTypeEnum.批追)
                    {
                        miInventoryDatas.Add(new miInventoryData
                        {
                            inventory = Scan.InternalCode,
                            qty = Scan.UseNum.ToString(),
                            assemblyDataFields = new AssemblyDataField[] {
                                new() { attribute = "ATLID", value = Scan.InternalCode },
                            }
                        });
                        outerParamName = Scan.InternalCode;
                    }
                    if (Scan.TracingType == TracingTypeEnum.精追)
                    {
                        // 精追，批次码一样 但是 外部码不一样，同样需要新增 上传数据 miInventoryDatas
                        miInventoryDatas.Add(new miInventoryData
                        {
                            inventory = Scan.InternalCode,
                            qty = Scan.UseNum.ToString(),
                            assemblyDataFields = new AssemblyDataField[] {
                                new() { attribute = "ATLID", value = Scan.InternalCode },
                                new() { attribute = "EXTERNAL_SERIAL", value = Scan.ExternalCode }
                            }
                        });

                        outerParamName = Scan.InternalCode + "_" + Scan.ExternalCode;
                    }
                }
                if (miInventoryDatas.Count != 0)
                    assemblereq.inventoryArray = miInventoryDatas.ToArray();
                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.Security.Mode = config.ConnectionParams.Url.StartsWith("https") ? BasicHttpSecurityMode.TransportWithMessageCredential : BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.SendTimeout = TimeSpan.FromMilliseconds(config.ConnectionParams.Timeout);

                var address = new EndpointAddress(config.ConnectionParams.Url);
                using var scf = new ChannelFactory<MiAssembleAndCollectDataForSfcService>(binding, address);
                scf.Credentials.UserName.UserName = config.ConnectionParams.UserName;
                scf.Credentials.UserName.Password = config.ConnectionParams.Password;
                scf.Endpoint.EndpointBehaviors.Add(new UsePreAuthenticateHttpClientEndpointBehavior(scf.Credentials.UserName.UserName, scf.Credentials.UserName.Password));
                var channel = scf.CreateChannel();
                var logging = new SfcInvocationLogging
                {
                    SentAt = DateTime.Now,
                    SfcNumber = sfc,
                };
                try
                {
                    var req = new miAssmebleAndCollectDataForSfcRequest()
                    {
                        miAssmebleAndCollectDataForSfc = new miAssmebleAndCollectDataForSfc
                        {
                            AssembleAndCollectDataForSfcRequest = assemblereq,
                        }
                    };
                    var resp = await channel.miAssmebleAndCollectDataForSfcAsync(req);
                    var result = resp.miAssmebleAndCollectDataForSfcResponse.@return;
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespCode = result.code.ToString();
                    logging.RespInfo = JsonConvert.SerializeObject(result);
                    return new CatlMESReponse { code = result.code, message = result.message };
                }
                catch (Exception ex)
                {
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespInfo = JsonConvert.SerializeObject(ex.Message);
                    return new CatlMESReponse { code = 1, message = ex.Message };
                }
                finally
                {
                    var svcname = this._iniHelper.MiAssembleAndCollectDataForSfcName;
                    var svcdesc = "";
                    this.WriteExcelAsync(logging, svcname, svcdesc, config.InterfaceParams.resource);
                }

            }

            catch (Exception ex)
            {
                return new CatlMESReponse { code = 1, message = ex.Message };
            }
        }

        /// <summary>
        /// 数据收集
        /// </summary>
        /// <param name="sfc">条码</param>
        /// <param name="uploadCATLData">数据</param>
        /// <param name="stationCode">工位</param>
        /// <returns></returns>
        public async Task<CatlMESReponse> dataCollect(string sfc, IList<DcParamValue> uploadCATLData, bool isStatusChange, string stationCode = "")
        {
            int resultCode = 0;
            try
            {
                var config = _iniHelper.GetDataCollectForMoudleTestConfig(stationCode);
                if (config == null)
                    return new CatlMESReponse { code = 500, message = "未配置接口文件，不调用Catl MES接口" };
                if (uploadCATLData == null || uploadCATLData.Count == 0)
                {
                    return new CatlMESReponse { code = 500, message = "未提供待收数数据,请查询后重试" };
                }
                var assemblereq = new dataCollectForSfcEx
                {
                    SfcDcExRequest = new sfcDcExRequest
                    {
                        modeProcessSfc = isStatusChange ? config.InterfaceParams.modeProcessSFC : ModeProcessSfc.MODE_NONE,
                        resource = config.InterfaceParams.Resource,
                        operation = config.InterfaceParams.Operation,
                        operationRevision = config.InterfaceParams.OperationRevision,
                        site = config.InterfaceParams.Site,
                        sfc = sfc,
                        user = config.InterfaceParams.User,
                        activityId = config.InterfaceParams.ActivityId,
                        dcGroup = config.InterfaceParams.dcGroup,
                        dcGroupRevision = config.InterfaceParams.dcGroupRevision,
                    }
                };
                List<Catl.WebServices.MachineIntegrationServices.machineIntegrationParametricData> parametricDataArray = new List<Catl.WebServices.MachineIntegrationServices.machineIntegrationParametricData>();
                foreach (var item in uploadCATLData)
                {
                    var upData = new Catl.WebServices.MachineIntegrationServices.machineIntegrationParametricData
                    {
                        name = item.UpMesCode,
                        value = item.ParamValue,
                        dataType = (ParameterDataType)item.DataType,
                    };
                    parametricDataArray.Add(upData);
                }

                if (parametricDataArray.Count > 0)
                {
                    assemblereq.SfcDcExRequest.parametricDataArray = parametricDataArray.ToArray();
                }

                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.Security.Mode = config.ConnectionParams.Url.StartsWith("https") ? BasicHttpSecurityMode.TransportWithMessageCredential : BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.SendTimeout = TimeSpan.FromMilliseconds(config.ConnectionParams.Timeout);

                var address = new EndpointAddress(config.ConnectionParams.Url);
                using var scf = new ChannelFactory<MachineIntegrationService>(binding, address);
                scf.Credentials.UserName.UserName = config.ConnectionParams.UserName;
                scf.Credentials.UserName.Password = config.ConnectionParams.Password;
                scf.Endpoint.EndpointBehaviors.Add(new UsePreAuthenticateHttpClientEndpointBehavior(scf.Credentials.UserName.UserName, scf.Credentials.UserName.Password));
                var channel = scf.CreateChannel();

                var req = new dataCollectForSfcExRequest
                {
                    dataCollectForSfcEx = assemblereq
                };

                var logging = new SfcInvocationLogging
                {
                    SentAt = DateTime.Now,
                    SfcNumber = sfc,
                };
                try
                {
                    var resp = await channel.dataCollectForSfcExAsync(req);
                    var result = resp.dataCollectForSfcExResponse.@return;
                    resultCode = result.code;
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespCode = result.code.ToString();
                    logging.RespInfo = JsonConvert.SerializeObject(result);
                    return new CatlMESReponse { code = result.code, message = result.message };
                }
                catch (Exception ex)
                {
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespInfo = JsonConvert.SerializeObject(ex.Message);
                    return new CatlMESReponse { code = resultCode, message = ex.Message };
                }
                finally
                {
                    var svcname = _iniHelper.DataCollectForMoudleTestName;
                    var svcdesc = "";
                    WriteExcelAsync(logging, svcname, svcdesc, config.InterfaceParams.Resource);
                }
            }
            catch (Exception ex)
            {
                return new CatlMESReponse { code = resultCode, message = ex.Message };
            }

        }

        /// <summary>
        /// 首件
        /// </summary>
        /// <param name="machineIntegrationParametricDatas"></param>
        /// <param name="dcGroup"></param>
        /// <param name="StationCode"></param>
        /// <returns></returns>
        public async Task<CatlMESReponse> DataCollectForResourceInspectTask(List<machineIntegrationParametricData> machineIntegrationParametricDatas, string dcGroup, string StationCode = "")
        {
            if (machineIntegrationParametricDatas.Count == 0)
                return new CatlMESReponse { code = 0, message = "没有提供首件数据，不调用Catl MES接口" };

            var config = this._iniHelper.GetDataCollectForResourceInspectConfig(StationCode);
            if (config == null)
                return new CatlMESReponse { code = 0, message = "未配置接口文件，不调用Catl MES接口" };

            var dataCollectForResourceInspectTaskRequest = new dataCollectForResourceInspectTaskRequest
            {
                site = config.InterfaceParams.Site,
                user = config.InterfaceParams.User,
                dcMode = config.InterfaceParams.DcMode,
                executeMode = config.InterfaceParams.ExecuteMode,
                resource = config.InterfaceParams.Resource,
                operation = config.InterfaceParams.Operation,
                operationRevision = config.InterfaceParams.OperationRevision,
                dcGroup = config.InterfaceParams.DcGroup,
                dcGroupRevision = config.InterfaceParams.DcGroupRevision,
                dcSequence = dcGroup,
                parametricDataArray = machineIntegrationParametricDatas.ToArray()
            };

            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.Security.Mode = config.ConnectionParams.Url.StartsWith("https") ? BasicHttpSecurityMode.TransportWithMessageCredential : BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.SendTimeout = TimeSpan.FromMilliseconds(config.ConnectionParams.Timeout);

            var address = new EndpointAddress(config.ConnectionParams.Url);
            using var scf = new ChannelFactory<DataCollectForResourceInspectTaskService>(binding, address);
            scf.Credentials.UserName.UserName = config.ConnectionParams.UserName;
            scf.Credentials.UserName.Password = config.ConnectionParams.Password;
            scf.Endpoint.EndpointBehaviors.Add(new UsePreAuthenticateHttpClientEndpointBehavior(scf.Credentials.UserName.UserName, scf.Credentials.UserName.Password));
            var channel = scf.CreateChannel();

            var req = new dataCollectForResourceInspectTaskRequest1(new dataCollectForResourceInspectTask
            {
                DataCollectForResourceInspectTaskRequest = dataCollectForResourceInspectTaskRequest
            });

            var logging = new SfcInvocationLogging
            {
                SentAt = DateTime.Now,
                SfcNumber = "",
            };
            try
            {
                var resp = await channel.dataCollectForResourceInspectTaskAsync(req);
                var result = resp.dataCollectForResourceInspectTaskResponse.@return;
                logging.ReceivedAt = DateTime.Now;
                logging.Payload = JsonConvert.SerializeObject(dataCollectForResourceInspectTaskRequest).Replace("[38]", "ITEM");
                logging.RespCode = result.code.ToString();
                logging.RespInfo = JsonConvert.SerializeObject(result);
                return new CatlMESReponse { code = result.code, message = result.message };
            }
            catch (Exception ex)
            {
                logging.ReceivedAt = DateTime.Now;
                logging.Payload = JsonConvert.SerializeObject(dataCollectForResourceInspectTaskRequest).Replace("[38]", "ITEM");
                logging.RespInfo = JsonConvert.SerializeObject(ex.Message);
                return new CatlMESReponse { code = 1, message = ex.Message };
            }
            finally
            {
                var svcname = this._iniHelper.DataCollectForResourceInspect;
                var svcdesc = "";
                this.WriteExcelAsync(logging, svcname, svcdesc, config.InterfaceParams.Resource);
            }
        }

        public async Task<CatlMESReponse> GetModuleCodeByCellCode(string cellCode, string StationCode = "")
        {
            var response = new CatlMESReponse();
            try
            {
                var config = this._iniHelper.GetMIFindCustomAndSfcDataConfig(StationCode);
                if (config == null)
                    return new CatlMESReponse { code = 0, message = "未配置接口文件，不调用Catl MES接口" };
                if (string.IsNullOrEmpty(config.ConnectionParams.Url))
                {
                    throw new Exception($"调用CATL WebService时URL不可为空，请检查MesCfg.ini配置文件！");
                }
                var assemblereq = new findCustomAndSfcDataRequest
                {
                    operation = config.InterfaceParams.Operation,
                    operationRevision = config.InterfaceParams.OperationRevision,
                    site = config.InterfaceParams.Site,
                    activity = config.InterfaceParams.ActivityId,
                    resource = config.InterfaceParams.Resource,
                    user = config.InterfaceParams.User,
                    sfcOrder = config.InterfaceParams.sfcOrder,
                    targetOrder = config.InterfaceParams.targetOrder,
                    modeProcessSFC = modeProcessSFC.MODE_NONE,
                    customDataArray = new customDataInParametricData[1] { new customDataInParametricData { category = config.InterfaceParams.category, dataField = config.InterfaceParams.dataField } },
                    masterDataArray = new ObjectAliasEnum[1] { config.InterfaceParams.masterDataArray },
                    inventory = cellCode,
                    findSfcByInventory = true,
                };

                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.Security.Mode = config.ConnectionParams.Url.StartsWith("https") ? BasicHttpSecurityMode.TransportWithMessageCredential : BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.SendTimeout = TimeSpan.FromMilliseconds(config.ConnectionParams.Timeout);

                var address = new EndpointAddress(config.ConnectionParams.Url);
                using var scf = new ChannelFactory<MiFindCustomAndSfcDataService>(binding, address);
                scf.Credentials.UserName.UserName = config.ConnectionParams.UserName;
                scf.Credentials.UserName.Password = config.ConnectionParams.Password;
                scf.Endpoint.EndpointBehaviors.Add(new UsePreAuthenticateHttpClientEndpointBehavior(scf.Credentials.UserName.UserName, scf.Credentials.UserName.Password));
                var channel = scf.CreateChannel();
                var logging = new SfcInvocationLogging
                {
                    SentAt = DateTime.Now,
                    SfcNumber = cellCode,
                };
                try
                {
                    var req = new miFindCustomAndSfcDataRequest(new miFindCustomAndSfcData
                    {
                        FindCustomAndSfcDataRequest = assemblereq
                    });
                    var resp = await channel.miFindCustomAndSfcDataAsync(req);
                    var result = resp.miFindCustomAndSfcDataResponse.@return;
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespCode = result.code.ToString();
                    logging.RespInfo = JsonConvert.SerializeObject(result);
                    string goodsPN = string.Empty;
                    if (result.code == 0 && result.masterDataArray != null && result.masterDataArray.Length > 0)
                    {
                        goodsPN = result.masterDataArray[0].value;
                    }
                    return new CatlMESReponse { code = result.code, message = result.message, BarCode_GoodsPN = goodsPN, BarCode = result.sfc };
                }
                catch (Exception ex)
                {
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespInfo = JsonConvert.SerializeObject(ex.Message);
                    return new CatlMESReponse { code = 1, message = ex.Message };
                }
                finally
                {
                    var svcname = this._iniHelper.MIFindCustomAndSfcDataConfigName;
                    var svcdesc = "";
                    this.WriteExcelAsync(logging, svcname, svcdesc, config.InterfaceParams.Resource);
                }
            }
            catch (Exception ex)
            {
                return new CatlMESReponse { code = 1, message = ex.Message };
            }
        }

        public async Task<CatlMESReponse> CheckSfcStatu(string sfc, string StationCode = "")
        {
            try
            {
                var config = this._iniHelper.GetMICheckSfcStatusExConfig(StationCode);
                if (config == null)
                    return new CatlMESReponse { code = 0, message = "未配置接口文件，不调用Catl MES接口" };
                if (string.IsNullOrEmpty(config.ConnectionParams.Url))
                {
                    throw new Exception($"调用CATL WebService时URL不可为空，请检查MesCfg.ini配置文件！");
                }
                var changeSfcReq = new changeSFCStatusExRequest
                {
                    site = config.InterfaceParams.Site,
                    sfc = sfc,
                    operationRevision = config.InterfaceParams.OperationRevision,
                    operation = config.InterfaceParams.Operation,
                    isGetSFCFromCustomerBarcode = config.InterfaceParams.IsGetSFCFromCustomerBarcode
                };


                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.Security.Mode = config.ConnectionParams.Url.StartsWith("https") ? BasicHttpSecurityMode.TransportWithMessageCredential : BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.SendTimeout = TimeSpan.FromMilliseconds(config.ConnectionParams.Timeout);

                var address = new EndpointAddress(config.ConnectionParams.Url);
                using var scf = new ChannelFactory<MiCheckSFCStatusExService>(binding, address);
                scf.Credentials.UserName.UserName = config.ConnectionParams.UserName;
                scf.Credentials.UserName.Password = config.ConnectionParams.Password;
                scf.Endpoint.EndpointBehaviors.Add(new UsePreAuthenticateHttpClientEndpointBehavior(scf.Credentials.UserName.UserName, scf.Credentials.UserName.Password));
                var channel = scf.CreateChannel();
                var logging = new SfcInvocationLogging
                {
                    SentAt = DateTime.Now,
                    SfcNumber = sfc,
                };
                try
                {
                    var req = new miCheckSFCStatusExRequest(new miCheckSFCStatusEx
                    {
                        ChangeSFCStatusExRequest = changeSfcReq
                    });
                    var resp = await channel.miCheckSFCStatusExAsync(req);
                    var result = resp.miCheckSFCStatusExResponse.@return;
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(changeSfcReq).Replace("[38]", "ITEM");
                    logging.RespCode = result.code.ToString();
                    logging.RespInfo = JsonConvert.SerializeObject(result);

                    return new CatlMESReponse { code = result.code, message = result.message };
                }
                catch (Exception ex)
                {
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(changeSfcReq).Replace("[38]", "ITEM");
                    logging.RespInfo = JsonConvert.SerializeObject(ex.Message);
                    return new CatlMESReponse { code = 1, message = ex.Message };
                }
                finally
                {
                    var svcname = this._iniHelper.MICheckSFCStatusExName;
                    var svcdesc = "";
                    this.WriteExcelAsync(logging, svcname, svcdesc, config.InterfaceParams.Operation);
                }
            }


            catch (Exception ex)
            {
                return new CatlMESReponse
                {
                    code = 1,
                    message = ex.Message
                };
            }

        }

        public async Task<CatlMESReponse> CheckInventoryAttributes(string sfc, string moduleCode, string stationCode = "")
        {
            try
            {
                var config = this._iniHelper.GetMiCheckInventoryAttributesConfig(stationCode);
                if (config == null)
                    return new CatlMESReponse { code = 0, message = "未配置接口文件，不调用Catl MES接口" };
                if (string.IsNullOrEmpty(config.ConnectionParams.Url))
                {
                    throw new Exception($"调用CATL WebService时URL不可为空，请检查MesCfg.ini配置文件！");
                }
                var assemblereq = new ModuleCellMarkingOrTimeCheckRequest
                {
                    site = config.InterfaceParams.Site,
                    sfc = sfc,
                    operation = config.InterfaceParams.Operation,
                    operationRevision = config.InterfaceParams.OperationRevision,
                    resource = config.InterfaceParams.Resource,
                    user = config.InterfaceParams.User,
                    activityId = config.InterfaceParams.ActivityId,
                    modeCheckInventory = config.InterfaceParams.modeCheckInventory,
                    requiredQuantity = 1,
                    inventoryArray = new string[] { moduleCode },
                };

                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.Security.Mode = config.ConnectionParams.Url.StartsWith("https") ? BasicHttpSecurityMode.TransportWithMessageCredential : BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.SendTimeout = TimeSpan.FromMilliseconds(config.ConnectionParams.Timeout);

                var address = new EndpointAddress(config.ConnectionParams.Url);
                using var scf = new ChannelFactory<MiCheckInventoryAttributesService>(binding, address);
                scf.Credentials.UserName.UserName = config.ConnectionParams.UserName;
                scf.Credentials.UserName.Password = config.ConnectionParams.Password;
                scf.Endpoint.EndpointBehaviors.Add(new UsePreAuthenticateHttpClientEndpointBehavior(scf.Credentials.UserName.UserName, scf.Credentials.UserName.Password));
                var channel = scf.CreateChannel();
                var logging = new SfcInvocationLogging
                {
                    SentAt = DateTime.Now,
                    SfcNumber = sfc,
                };
                try
                {
                    var req = new miCheckInventoryAttributesRequest(new miCheckInventoryAttributes
                    {
                        CheckInventoryAttributesRequest = assemblereq
                    });
                    var resp = await channel.miCheckInventoryAttributesAsync(req);
                    var result = resp.miCheckInventoryAttributesResponse.@return;
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespCode = result.code.ToString();
                    logging.RespInfo = JsonConvert.SerializeObject(result);

                    return new CatlMESReponse { code = result.code, message = result.message };
                }
                catch (Exception ex)
                {
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(assemblereq).Replace("[38]", "ITEM");
                    logging.RespInfo = JsonConvert.SerializeObject(ex.Message);
                    return new CatlMESReponse { code = 1, message = ex.Message };
                }
                finally
                {
                    var svcname = this._iniHelper.MiCheckInventoryAttributesName;
                    var svcdesc = "";
                    this.WriteExcelAsync(logging, svcname, svcdesc, config.InterfaceParams.Resource);
                }
            }
            catch (Exception ex)
            {
                return new CatlMESReponse { code = 1, message = ex.Message };
            }
        }

        public async Task<CatlMESReponse> GetParametricValue(string sfc, string parameter, string stationCode = "")
        {
            try
            {
                var config = this._iniHelper.GetGetParametricValueConfig(stationCode);
                if (config == null)
                    return new CatlMESReponse { code = 0, message = "未配置接口文件，不调用Catl MES接口" };
                if (string.IsNullOrEmpty(config.ConnectionParams.Url))
                {
                    throw new Exception($"调用CATL WebService时URL不可为空，请检查MesCfg.ini配置文件！");
                }
                var getReq = new GetParametricValueRequest
                {
                    site = config.InterfaceParams.Site,
                    sfc = sfc,
                    user = config.InterfaceParams.User,
                    parametricDataArray = new GetParametricValueRequestData[] { new() { parameter = parameter, parameterDec = string.Empty } },
                };

                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.Security.Mode = config.ConnectionParams.Url.StartsWith("https") ? BasicHttpSecurityMode.TransportWithMessageCredential : BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.SendTimeout = TimeSpan.FromMilliseconds(config.ConnectionParams.Timeout);

                var address = new EndpointAddress(config.ConnectionParams.Url);
                using var scf = new ChannelFactory<GetParametricValueService>(binding, address);
                scf.Credentials.UserName.UserName = config.ConnectionParams.UserName;
                scf.Credentials.UserName.Password = config.ConnectionParams.Password;
                scf.Endpoint.EndpointBehaviors.Add(new UsePreAuthenticateHttpClientEndpointBehavior(scf.Credentials.UserName.UserName, scf.Credentials.UserName.Password));
                var channel = scf.CreateChannel();
                var logging = new SfcInvocationLogging
                {
                    SentAt = DateTime.Now,
                    SfcNumber = sfc,
                };
                try
                {
                    var req = new getParametricValueRequest1(new getParametricValue
                    {
                        GetParametricValueRequest = getReq
                    });
                    var resp = await channel.getParametricValueAsync(req);
                    var result = resp.getParametricValueResponse;
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(getReq).Replace("[38]", "ITEM");
                    //logging.RespCode = result.code.ToString();
                    logging.RespInfo = JsonConvert.SerializeObject(result);

                    return new CatlMESReponse { code = 304, message = "测试中" };
                    //return new CatlMESReponse { code = result.code, message = result.message };
                }
                catch (Exception ex)
                {
                    logging.ReceivedAt = DateTime.Now;
                    logging.Payload = JsonConvert.SerializeObject(getReq).Replace("[38]", "ITEM");
                    logging.RespInfo = JsonConvert.SerializeObject(ex.Message);
                    return new CatlMESReponse { code = 1, message = ex.Message };
                }
                finally
                {
                    var svcname = this._iniHelper.GetParametricValueName;
                    var svcdesc = "";
                    this.WriteExcelAsync(logging, svcname, svcdesc, config.InterfaceParams.Resource);
                }
            }
            catch (Exception ex)
            {
                return new CatlMESReponse { code = 1, message = ex.Message };
            }
        }
        
        public Task<CatlMESReponse> dataCollect(string sfc, IList<DcParamValue> uploadCATLData, string DcGroup, bool isStatusChange, string stationCode = "")
        {
            throw new NotImplementedException();
        }

        public Task<CatlMESReponse> AutoNc(string sfc, string stationCode = "", string ncCode = "")
        {
            throw new NotImplementedException();
        }
    }
}

