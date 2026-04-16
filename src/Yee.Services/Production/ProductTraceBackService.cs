using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DTOS;
using Yee.Tools;

namespace Yee.Services.Production
{
    public class ProductTraceBackService
    {
        private readonly IServiceProvider service;
        private readonly AsZeroDbContext dbcontext;


        public ProductTraceBackService(IServiceProvider service, AsZeroDbContext dbcontext)
        {
            this.service = service;
            this.dbcontext = dbcontext;

        }


        /// <summary>
        /// 获取正向追溯数据
        /// </summary>
        /// <param name="code">pack条码</param>
        /// <returns></returns>
        public async Task<ForwardTrace?> GetForward(string code)
        {
            var query = this.dbcontext.Proc_StationTask_Mains.Where(o => !o.IsDeleted && o.PackCode == code);
            query = query.Include(o => o.Station).Include(o => o.CreateUser).Include(o => o.UpdateUser).Include(o => o.DeleteUser);
            var maintasks = await query.ToListAsync();
            if (maintasks != null && maintasks.Count != 0)
            {
                foreach (var maintask in maintasks)
                {
                    var taskresource = await this.dbcontext.Proc_StationTask_Records.Where(o => o.Proc_StationTask_MainId == maintask.Id && !o.IsDeleted).Include(o => o.Base_StationTask)
                   .Include(o => o.CreateUser).Include(o => o.UpdateUser).Include(o => o.DeleteUser).ToListAsync();
                    if (taskresource != null && taskresource.Count != 0)
                    {
                        foreach (var item in taskresource)
                        {
                            //扫码任务
                            var boms = await this.dbcontext.Proc_StationTask_Boms.Where(o => o.StationTask_RecordId == item.Id && !o.IsDeleted).Include(o => o.Base_ProResource).Include(o => o.CreateUser).Include(o => o.UpdateUser).Include(o => o.DeleteUser).ToListAsync();
                            //扫码详情
                            foreach (var bom in boms)
                            {
                                var bomDetails = await this.dbcontext.Proc_StationTask_BomDetails.Where(o => o.Proc_StationTask_BomId == bom.Id && !o.IsDeleted).Include(o => o.CreateUser).Include(o => o.UpdateUser).Include(o => o.DeleteUser).ToListAsync();
                                bom.Proc_StationTask_BomDetails = bomDetails;
                            }
                            item.Proc_StationTask_Boms = boms;

                            //拧紧任务
                            var blotgundetails = await this.dbcontext.Proc_StationTask_BlotGunDetails.Include(o => o.Proc_StationTask_BlotGun).Include(o => o.CreateUser).Include(o => o.UpdateUser).Include(o => o.DeleteUser).Where(o => o.Proc_StationTask_BlotGun.StationTask_RecordId == item.Id && !o.IsDeleted)
                                .Include(o => o.Base_ProResource).Include(o => o.Proc_StationTask_BlotGun.CreateUser).Include(o => o.Proc_StationTask_BlotGun.UpdateUser).Include(o => o.Proc_StationTask_BlotGun.DeleteUser).ToListAsync();
                            var gundic = blotgundetails.GroupBy(o => o.Proc_StationTask_BlotGun);
                            var blotguns = new List<Proc_StationTask_BlotGun>();
                            foreach (var blotitem in gundic)
                            {
                                Proc_StationTask_BlotGun blotGun = blotitem.Key;
                                blotGun.Proc_StationTask_BlotGunDetails = blotitem.ToList();
                                blotguns.Add(blotGun);
                            }
                            item.Proc_StationTask_BlotGuns = blotguns;

                            //称重任务
                            var anyload = await this.dbcontext.Proc_StationTask_AnyLoads.Where(o => o.StationTask_RecordId == item.Id && !o.IsDeleted).FirstOrDefaultAsync();
                            item.Proc_StationTask_AnyLoad = anyload;

                            //扫码输入
                            var scanCollect = await dbcontext.Proc_StationTask_ScanCollects.Where(o => o.StationTask_RecordId == item.Id && !o.IsDeleted).FirstOrDefaultAsync();
                            item.Proc_StationTask_ScanCollect = scanCollect;
                            //用户输入
                            var userscan = await dbcontext.Proc_StationTask_UserInputs.Where(o => !o.IsDeleted && o.StationTask_RecordId == item.Id).FirstOrDefaultAsync();
                            item.Proc_StationTask_UserInput = userscan;

                            //涂胶超时
                            var gluingTime = await dbcontext.Proc_StationTask_CheckTimeouts.Where(o => o.StationTask_RecordId == item.Id && !o.IsDeleted).FirstOrDefaultAsync();
                            item.Proc_StationTask_GluingTime = gluingTime;

                            item .Proc_StationTask_StewingTime = await dbcontext.Proc_StationTask_TimeRecords.Where(o => o.Proc_StationTask_RecordId == item.Id && !o.IsDeleted).FirstOrDefaultAsync();
                            //扫描员工卡
                            var scanAccount = await dbcontext.Proc_StationTask_ScanAccountCards.Where(o => o.StationTask_RecordId == item.Id && !o.IsDeleted).FirstOrDefaultAsync();
                            item.Proc_StationTask_ScanAccountCard = scanAccount;

                        }

                    }

                    maintask.Proc_StationTask_Records = taskresource;

                    var step = await this.dbcontext.Base_Steps.Where(o => o.Id == maintask.StepId).FirstOrDefaultAsync();

                    if (step != null && step.StepType == StepTypeEnum.自动站 && step.Name == "模组入箱")
                    {
                    }

                    if (step != null && step.StepType == StepTypeEnum.自动站 && step.Name == "上盖拧紧")
                    {
                        var autoBoltList = await this.dbcontext.Proc_AutoBoltInfo_Details.Where(o => o.Proc_StationTask_MainId == maintask.Id && !o.IsDeleted).ToListAsync();
                        maintask.AutoBoltList = autoBoltList;
                    }
                }
                return new ForwardTrace() { Code = code, StationTask_Mains_Realtime = maintasks };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 导出Pack所有的工位数据
        /// </summary>
        /// <param name="packCode"></param>
        /// <returns></returns>
        public async Task<(Boolean, string)> ExportStationTask(string packCode)
        {
            string filepath = Directory.GetCurrentDirectory() + @"\Pack数据导出.xls";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            var stream = new FileStream(filepath, FileMode.OpenOrCreate);
            try
            {
                List<PackTaskExcel> packExcellist = new();
                var mains = await dbcontext.Proc_StationTask_Mains.Where(w => !w.IsDeleted && w.PackCode == packCode).ToListAsync();

                foreach (var main in mains)
                {
                    var records = await dbcontext.Proc_StationTask_Records.Include(i => i.Base_StationTask).Where(w => !w.IsDeleted && w.Proc_StationTask_MainId == main.Id).ToListAsync();
                    foreach (var record in records)
                    {
                        switch (record.Base_StationTask!.Type)
                        {
                            case StationTaskTypeEnum.扫描员工卡:
                                packExcellist.AddRange(await GetScanAccountData(main, record));
                                break;
                            case StationTaskTypeEnum.扫码:
                                packExcellist.AddRange(await GetScanData(main, record));
                                break;
                            case StationTaskTypeEnum.人工拧螺丝:
                                packExcellist.AddRange(await GetBlotData(main, record));
                                break;
                            case StationTaskTypeEnum.扫码输入:
                                packExcellist.AddRange(await GetScanInputData(main, record));
                                break;
                            case StationTaskTypeEnum.用户输入:
                                packExcellist.AddRange(await GetUserInputData(main, record));
                                break;
                            case StationTaskTypeEnum.称重:
                                packExcellist.AddRange(await GetAnyLoadData(main, record));
                                break;
                            case StationTaskTypeEnum.时间记录:
                                packExcellist.AddRange(await GetTimeRecordData(main, record));
                                break;
                            case StationTaskTypeEnum.超时检测:
                                packExcellist.AddRange(await GetTimeCheckData(main, record));
                                break;
                            case StationTaskTypeEnum.补拧:
                                packExcellist.AddRange(await GetRepairData(main, record));
                                break;
                        }
                    }
                }


                ExcelPackage excelPackage = new ExcelPackage();
                excelPackage = ExcelToEntity.ListToExcek<PackTaskExcel>(excelPackage, "Pack数据导出", 2, packExcellist);
                excelPackage.SaveAs(stream);
                stream.Close();
                return (true, filepath);
            }
            catch (Exception ex)
            {
                stream.Close();
                return (false, $"导出出现错误{ex.Message}");
            }
         

        }

        private async Task<IList<PackTaskExcel>> GetScanAccountData(Proc_StationTask_Main main, Proc_StationTask_Record record)
        {
            var account = await dbcontext.Proc_StationTask_ScanAccountCards.Where(w => !w.IsDeleted && w.StationTask_RecordId == record.Id).ToListAsync();

            return account.Select(s => new PackTaskExcel
            {
                PackCode = main.PackCode,
                StationCode = main.StationCode,
                TaskName = record.TaskName,
                TaskType = record.Base_StationTask!.Type.ToString(),
                UpMesCode1 = s.UpMesCode,
                数据1 = s.AccountValue,
                任务时间 = s.CreateTime.ToString()
            }).ToList();
        }

        private async Task<IList<PackTaskExcel>> GetScanData(Proc_StationTask_Main main, Proc_StationTask_Record record)
        {
            var q = from boms in dbcontext.Proc_StationTask_Boms
                    from bomDetails in dbcontext.Proc_StationTask_BomDetails
                    where boms.StationTask_RecordId == record.Id && !boms.IsDeleted && !bomDetails.IsDeleted && bomDetails.Proc_StationTask_BomId == boms.Id
                    select bomDetails;
            var detail = await q.ToListAsync();
            return detail.Select(s => new PackTaskExcel
            {
                PackCode = main.PackCode,
                StationCode = main.StationCode,
                TaskName = record.TaskName,
                TaskType = record.Base_StationTask!.Type.ToString(),
                任务时间 = s.CreateTime.ToString(),
                内部码 = s.TracingType == TracingTypeEnum.扫库存 ? s.UniBarCode : s.BatchBarCode,
                外部码 = s.GoodsOuterCode
            }).ToList();
        }

        private async Task<IList<PackTaskExcel>> GetBlotData(Proc_StationTask_Main main, Proc_StationTask_Record record)
        {
            var q = from blots in dbcontext.Proc_StationTask_BlotGuns
                    from blotDetails in dbcontext.Proc_StationTask_BlotGunDetails
                    where blots.StationTask_RecordId == record.Id && !blots.IsDeleted && !blotDetails.IsDeleted && blotDetails.Proc_StationTask_BlotGunId == blots.Id
                    select blotDetails;
            var detail = await q.ToListAsync();
            return detail.Select(s => new PackTaskExcel
            {
                PackCode = main.PackCode,
                StationCode = main.StationCode,
                TaskName = record.TaskName,
                TaskType = record.Base_StationTask!.Type.ToString(),
                任务时间 = s.CreateTime.ToString(),
                拧紧结果 = s.ResultIsOK ? "OK" : "NG",
                数据1 = s.FinalTorque.ToString(),
                数据2 = s.FinalAngle.ToString(),
                OrderNo = s.OrderNo.ToString()!,
                UpMesCode1 = s.UploadCode,
                UpMesCode2 = s.UploadCode_JD,
            }).ToList();
        }

        private async Task<IList<PackTaskExcel>> GetScanInputData(Proc_StationTask_Main main, Proc_StationTask_Record record)
        {
            var account = await dbcontext.Proc_StationTask_ScanCollects.Where(w => !w.IsDeleted && w.StationTask_RecordId == record.Id).ToListAsync();

            return account.Select(s => new PackTaskExcel
            {
                PackCode = main.PackCode,
                StationCode = main.StationCode,
                TaskName = record.TaskName,
                TaskType = record.Base_StationTask!.Type.ToString(),
                UpMesCode1 = s.UpMesCode,
                数据1 = s.ScanCollectData,
                任务时间 = s.CreateTime.ToString()
            }).ToList();
        }
        private async Task<IList<PackTaskExcel>> GetUserInputData(Proc_StationTask_Main main, Proc_StationTask_Record record)
        {
            var account = await dbcontext.Proc_StationTask_UserInputs.Where(w => !w.IsDeleted && w.StationTask_RecordId == record.Id).ToListAsync();

            return account.Select(s => new PackTaskExcel
            {
                PackCode = main.PackCode,
                StationCode = main.StationCode,
                TaskName = record.TaskName,
                TaskType = record.Base_StationTask!.Type.ToString(),
                UpMesCode1 = s.UpMesCode,
                数据1 = s.UserInputData,
                任务时间 = s.CreateTime.ToString()
            }).ToList();
        }

        private async Task<IList<PackTaskExcel>> GetTimeRecordData(Proc_StationTask_Main main, Proc_StationTask_Record record)
        {
            var account = await dbcontext.Proc_StationTask_TimeRecords.Where(w => !w.IsDeleted && w.Proc_StationTask_RecordId == record.Id).ToListAsync();

            return account.Select(s => new PackTaskExcel
            {
                PackCode = main.PackCode,
                StationCode = main.StationCode,
                TaskName = record.TaskName,
                TaskType = record.Base_StationTask!.Type.ToString(),
                UpMesCode1 = s.UploadMesCode,
                数据1 = s.TimeValue,
                任务时间 = s.CreateTime.ToString()
            }).ToList();
        }

        private async Task<IList<PackTaskExcel>> GetTimeCheckData(Proc_StationTask_Main main, Proc_StationTask_Record record)
        {
            var account = await dbcontext.Proc_StationTask_CheckTimeouts.Where(w => !w.IsDeleted && w.StationTask_RecordId == record.Id).ToListAsync();

            return account.Select(s => new PackTaskExcel
            {
                PackCode = main.PackCode,
                StationCode = main.StationCode,
                TaskName = record.TaskName,
                TaskType = record.Base_StationTask!.Type.ToString(),
                UpMesCode1 = s.UpMesCode,
                数据1 = s.Time.ToString(),
                任务时间 = s.CreateTime.ToString()
            }).ToList();
        }

        private async Task<IList<PackTaskExcel>> GetAnyLoadData(Proc_StationTask_Main main, Proc_StationTask_Record record)
        {
            var account = await dbcontext.Proc_StationTask_AnyLoads.Where(w => !w.IsDeleted && w.StationTask_RecordId == record.Id).ToListAsync();

            return account.Select(s => new PackTaskExcel
            {
                PackCode = main.PackCode,
                StationCode = main.StationCode,
                TaskName = record.TaskName,
                TaskType = record.Base_StationTask!.Type.ToString(),
                UpMesCode1 = s.UpMesCode,
                数据1 = s.WeightData.ToString(),
                任务时间 = s.CreateTime.ToString()
            }).ToList();
        }

        private async Task<IList<PackTaskExcel>> GetRepairData(Proc_StationTask_Main main, Proc_StationTask_Record record)
        {
            var account = await dbcontext.Proc_StationTask_TightenReworks.Where(w => !w.IsDeleted && w.StationTaskRecordId == record.Id).ToListAsync();

            return account.Select(s => new PackTaskExcel
            {
                PackCode = main.PackCode,
                StationCode = main.StationCode,
                TaskName = record.TaskName,
                TaskType = record.Base_StationTask!.Type.ToString(),
                UpMesCode1 = s.UpMesCode,
                拧紧结果 = s.ResultOk ? "OK" : "NG",
                数据1 = s.TorqueValue.ToString(),
                数据2 = s.AngleValue.ToString(),
                OrderNo = s.OrderNo.ToString()!,
                任务时间 = s.CreateTime.ToString()
            }).ToList();
        }
    }
}
