//using AsZero.DbContexts;
//using Microsoft.EntityFrameworkCore;
//using Yee.Common.Library.CommonEnum;
//using Yee.Entitys.CATL;
//using Yee.Entitys.DBEntity;
//using Yee.Entitys.DTOS;
//using Yee.Entitys.Production;
//using Yee.Services.Production;

//namespace Yee.Services.ProductionRecord
//{
//    public class TaskUpMESDataService
//    {
//        private readonly AsZeroDbContext dbContext;

//        public TaskUpMESDataService(IServiceProvider service)
//        {
//            var scope = service.CreateScope();
//            var servicep = scope.ServiceProvider;
//            this.dbContext = servicep.GetRequiredService<AsZeroDbContext>();
//        }
//        public async Task<UploadCATLData> GetUploadCATLData(string packCode, string stationCode, Base_Step step, int stationId = 0)
//        {
//            var result = new UploadCATLData();
//            result.PackPN = packCode;
//            result.DCParams = new List<DcParamValue>();
//            result.ScanCodeData = new List<Proc_StationTask_BomDetail>();
//            //result.UserScan = new List<Proc_StationTask_UserInput>();

//            try
//            {
//                //var main = this.dbContext.Proc_StationTask_Mains.OrderByDescending(m => m.CreateTime).FirstOrDefault(o => !o.IsDeleted && o.Status == Common.Library.CommonEnum.StationTaskStatusEnum.进行中 && o.StepId == step.Id && o.PackCode == packCode);
//                //尝试拧紧数据
//                var BlotList = this.dbContext.Proc_StationTask_BlotGunDetails.Where(s => s.StepId == step.Id && !s.IsDeleted && s.ResultIsOK && s.PackPN == packCode).ToList();
//                foreach (var item in BlotList)
//                {
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = item.UploadCode,
//                        ParamValue = item.FinalTorque.ToString(),
//                        DataType = ValueTypeEnum.NUMBER,
//                    });
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = item.UploadCode_JD,
//                        ParamValue = item.FinalAngle.ToString(),
//                        DataType = ValueTypeEnum.NUMBER,
//                    });
//                }

//                var autoRepaire = "";
//                if (step.Code == "OP130") autoRepaire = "OP120";
//                if (step.Code == "OP190") autoRepaire = "OP180";
//                if (step.Code == "OP420") autoRepaire = "OP410";

//                if (!string.IsNullOrEmpty(autoRepaire))
//                {
//                    //尝试自动站数据
//                    var mainAuto = this.dbContext.Proc_StationTask_Mains.Include(m => m.Step).FirstOrDefault(o => !o.IsDeleted && o.PackCode == packCode && o.Step.Code == autoRepaire);
//                    var queryAutoList = this.dbContext.Proc_AutoBoltInfo_Details.Where(o => !o.IsDeleted && o.PackPN == packCode && o.Proc_StationTask_MainId == mainAuto.Id && o.ResultIsOK).ToList();
//                    foreach (var item in queryAutoList)
//                    {
//                        result.DCParams.Add(new DcParamValue
//                        {
//                            UpMesCode = item.UploadCode,
//                            ParamValue = item.FinalTorque.ToString(),
//                            DataType = ValueTypeEnum.NUMBER,
//                        });
//                        result.DCParams.Add(new DcParamValue
//                        {
//                            UpMesCode = item.UploadCode_JD,
//                            ParamValue = item.FinalAngle.ToString(),
//                            DataType = ValueTypeEnum.NUMBER,
//                        });
//                    }
//                }

//                //尝试扫码外部输入数据
//                var scanCodeDetails = this.dbContext.Proc_StationTask_BomDetails.Where(r => r.StepId == step.Id && r.PackPN == packCode && !r.IsDeleted).ToList();
//                foreach (var det in scanCodeDetails)
//                {
//                    if (det.OuterParam1 != Common.Library.OuterParamTypeEnum.无输入)
//                    {
//                        result.DCParams.Add(new DcParamValue
//                        {
//                            UpMesCode = det.UpValue_1,
//                            ParamValue = det.OuterParam1Result,
//                            DataType = ValueTypeEnum.NUMBER,
//                        });
//                    }
//                    if (det.OuterParam2 != Common.Library.OuterParamTypeEnum.无输入)
//                    {
//                        result.DCParams.Add(new DcParamValue
//                        {
//                            UpMesCode = det.UpValue_2,
//                            ParamValue = det.OuterParam2Result,
//                            DataType = ValueTypeEnum.NUMBER,
//                        });
//                    }
//                    if (det.OuterParam3 != Common.Library.OuterParamTypeEnum.无输入)
//                    {
//                        result.DCParams.Add(new DcParamValue
//                        {
//                            UpMesCode = det.UpValue_3,
//                            ParamValue = det.OuterParam3Result,
//                            DataType = ValueTypeEnum.NUMBER,
//                        });
//                    }
//                }

//                //尝试扫码数据（避免重传，使用HasUpMesDone字段判断）
//                var detailsScan = new List<Proc_StationTask_BomDetail>();
//                detailsScan = this.dbContext.Proc_StationTask_BomDetails.Where(r => r.StepId == step.Id && !r.HasUpMesDone && r.PackPN == packCode && !r.IsDeleted).ToList();
//                foreach (var det in detailsScan)
//                {
//                    if (det.UseNum == null || det.UseNum == 0)
//                    {
//                        var oldVersion = this.dbContext.Proc_StationTask_BomDetails.Include(s => s.Proc_StationTask_Bom).FirstOrDefault(r => r.Id == det.Id);
//                        det.UseNum = oldVersion.Proc_StationTask_Bom.UseNum;
//                    }
//                    result.ScanCodeData.Add(det);
//                }

//                var needJoinTask = this.dbContext.Proc_Join_PackAndOuterCodes.FirstOrDefault(o => !o.IsDeleted && o.PackCode == packCode && o.StepId == step.Id && o.StationId == stationId);
//                if (needJoinTask != null && !string.IsNullOrEmpty(needJoinTask.OuterGoodsCode))
//                    GetUploadCATLData(result, needJoinTask.OuterGoodsCode);


//                //尝试称重数据
//                var anyload = this.dbContext.Proc_StationTask_AnyLoads.OrderByDescending(s => s.CreateTime).FirstOrDefault(o => !o.IsDeleted && o.PackPN == packCode && o.StepId == step.Id);
//                if (anyload != null)
//                {
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = anyload.UpMesCode,
//                        ParamValue = anyload.WeightData.ToString(),
//                        DataType = ValueTypeEnum.NUMBER
//                    });
//                }

//                //尝试用户输入数据
//                var userScans = this.dbContext.Proc_StationTask_UserInputs.Where(o => !o.IsDeleted && o.PackPN == packCode && o.StepId == step.Id).ToList();
//                foreach (var scan in userScans)
//                {
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = scan.UpMesCode,
//                        ParamValue = scan.ScanData,
//                        DataType = ValueTypeEnum.TEXT
//                    });
//                }

//                //尝试扫码输入数据
//                var ScanCollects = this.dbContext.Proc_StationTask_ScanCollects.Where(o => !o.IsDeleted && o.PackPN == packCode && o.StepId == step.Id).ToList();
//                foreach (var scanCollect in ScanCollects)
//                {
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = scanCollect.UpMesCode,
//                        ParamValue = scanCollect.ScanCollectData,
//                        DataType = ValueTypeEnum.TEXT
//                    });
//                }
//                //尝试涂胶超时数据
//                var gluingTime = this.dbContext.Proc_StationTask_GluingTimes.OrderByDescending(s => s.CreateTime).FirstOrDefault(o => !o.IsDeleted && o.PackPN == packCode && o.StepId == step.Id);
//                if (gluingTime != null)
//                {
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = gluingTime.UpMesCode,
//                        ParamValue = gluingTime.GluingTime.ToString(),
//                        DataType = ValueTypeEnum.NUMBER
//                    });

//                    //result.DCParams.Add(new Proc_StationTask_DCParamValue
//                    //{
//                    //    UpMesCode = gluingTime.UpMesCode_RXWCSJ,
//                    //    ParamValue = gluingTime.RXWCSJ.ToString(),
//                    //    DataType = ValueTypeEnum.TEXT
//                    //});
//                }

//                var scanAccountCard = this.dbContext.Proc_StationTask_ScanAccountCards.OrderByDescending(s => s.CreateTime).FirstOrDefault(o => !o.IsDeleted && o.PackPN == packCode && o.StepId == step.Id);
//                if (scanAccountCard != null)
//                {
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = scanAccountCard.UpMesCode,
//                        ParamValue = scanAccountCard.AccountValue.ToString(),
//                        DataType = ValueTypeEnum.TEXT
//                    });
//                }

//                return result;
//            }
//            catch (Exception ex) { return null; }
//        }

//        private void GetUploadCATLData(UploadCATLData result, string packCode)
//        {
//            //尝试拧紧数据
//            var BlotList = this.dbContext.Proc_StationTask_BlotGunDetails.Where(s => !s.IsDeleted && s.ResultIsOK && s.PackPN == packCode).ToList();
//            foreach (var item in BlotList)
//            {
//                result.DCParams.Add(new DcParamValue
//                {
//                    UpMesCode = item.UploadCode,
//                    ParamValue = item.FinalTorque.ToString(),
//                    DataType = ValueTypeEnum.NUMBER,
//                });
//                result.DCParams.Add(new DcParamValue
//                {
//                    UpMesCode = item.UploadCode_JD,
//                    ParamValue = item.FinalAngle.ToString(),
//                    DataType = ValueTypeEnum.NUMBER,
//                });
//            }
//            // 涂胶
//            var queryAutoList = this.dbContext.Proc_GluingInfos.Where(o => !o.IsDeleted && o.PackPN == packCode).ToList();
//            foreach (var item in queryAutoList)
//            {
//                result.DCParams.Add(new DcParamValue
//                {
//                    UpMesCode = item.UpMesCodePN,
//                    ParamValue = item.UpValue.ToString(),
//                    DataType = ValueTypeEnum.NUMBER,
//                });
//            }

//            //尝试扫码外部输入数据
//            var scanCodeDetails = this.dbContext.Proc_StationTask_BomDetails.Where(r => r.PackPN == packCode && !r.IsDeleted).ToList();
//            foreach (var det in scanCodeDetails)
//            {
//                if (det.OuterParam1 != Common.Library.OuterParamTypeEnum.无输入)
//                {
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = det.UpValue_1,
//                        ParamValue = det.OuterParam1Result,
//                        DataType = ValueTypeEnum.NUMBER,
//                    });
//                }
//                if (det.OuterParam2 != Common.Library.OuterParamTypeEnum.无输入)
//                {
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = det.UpValue_2,
//                        ParamValue = det.OuterParam2Result,
//                        DataType = ValueTypeEnum.NUMBER,
//                    });
//                }
//                if (det.OuterParam3 != Common.Library.OuterParamTypeEnum.无输入)
//                {
//                    result.DCParams.Add(new DcParamValue
//                    {
//                        UpMesCode = det.UpValue_3,
//                        ParamValue = det.OuterParam3Result,
//                        DataType = ValueTypeEnum.NUMBER,
//                    });
//                }
//            }

//            //尝试扫码数据（避免重传，使用HasUpMesDone字段判断）
//            var detailsScan = new List<Proc_StationTask_BomDetail>();
//            detailsScan = this.dbContext.Proc_StationTask_BomDetails.Where(r => !r.HasUpMesDone && r.PackPN == packCode && !r.IsDeleted).ToList();
//            foreach (var det in detailsScan)
//            {
//                if (det.UseNum == null || det.UseNum == 0)
//                {
//                    var oldVersion = this.dbContext.Proc_StationTask_BomDetails.Include(s => s.Proc_StationTask_Bom).FirstOrDefault(r => r.Id == det.Id);
//                    det.UseNum = oldVersion.Proc_StationTask_Bom.UseNum;
//                }
//                result.ScanCodeData.Add(det);
//            }

//            //尝试用户输入数据
//            var userScans = this.dbContext.Proc_StationTask_UserInputs.Where(o => !o.IsDeleted && o.PackPN == packCode).ToList();
//            foreach (var scan in userScans)
//            {
//                result.DCParams.Add(new DcParamValue
//                {
//                    UpMesCode = scan.UpMesCode,
//                    ParamValue = scan.ScanData,
//                    DataType = ValueTypeEnum.TEXT
//                });
//            }

//            //尝试扫码输入数据
//            var ScanCollects = this.dbContext.Proc_StationTask_ScanCollects.Where(o => !o.IsDeleted && o.PackPN == packCode).ToList();
//            foreach (var scanCollect in ScanCollects)
//            {
//                result.DCParams.Add(new DcParamValue
//                {
//                    UpMesCode = scanCollect.UpMesCode,
//                    ParamValue = scanCollect.ScanCollectData,
//                    DataType = ValueTypeEnum.TEXT
//                });
//            }
//            //尝试涂胶超时数据
//            var gluingTime = this.dbContext.Proc_StationTask_GluingTimes.OrderByDescending(s => s.CreateTime).FirstOrDefault(o => !o.IsDeleted && o.PackPN == packCode);
//            if (gluingTime != null)
//            {
//                result.DCParams.Add(new DcParamValue
//                {
//                    UpMesCode = gluingTime.UpMesCode,
//                    ParamValue = gluingTime.GluingTime.ToString(),
//                    DataType = ValueTypeEnum.NUMBER
//                });
//            }

//            //尝试涂胶检测数据
//            var gluingCheckData = this.dbContext.Proc_StationTask_GluingCheckDatas.OrderByDescending(s => s.CreateTime).FirstOrDefault(o => !o.IsDeleted && o.PackPN == packCode);
//            if (gluingCheckData != null)
//            {
//                result.DCParams.Add(new DcParamValue
//                {
//                    UpMesCode = gluingCheckData.UpMesCode,
//                    ParamValue = gluingCheckData.Number.ToString(),
//                    DataType = ValueTypeEnum.NUMBER
//                });
//            }
//        }

//        public void UpdateUpMesStatus(UploadCATLData? uploadCATLData)
//        {
//            var details = new List<Proc_StationTask_BomDetail>();
//            if (uploadCATLData != null && uploadCATLData.ScanCodeData != null && uploadCATLData.ScanCodeData.Count > 0)
//            {
//                foreach (var bomDetail in uploadCATLData.ScanCodeData)
//                {
//                    var detail = dbContext.Proc_StationTask_BomDetails.FirstOrDefault(s => s.Id == bomDetail.Id);
//                    if (detail != null)
//                    {
//                        detail.HasUpMesDone = true;
//                        details.Add(detail);
//                    }
//                }
//            }
//            uploadCATLData.ScanCodeData.RemoveAll(s => s.Id == 0);
//            dbContext.Proc_StationTask_BomDetails.UpdateRange(details);
//            dbContext.SaveChanges();
//        }
//    }
//}
