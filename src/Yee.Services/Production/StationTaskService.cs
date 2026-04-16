using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class StationTaskService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly SysLogService sys_LogService;
        private readonly StationService _stationService;
        private readonly Proc_StationTask_PeiFangService _PeiFangService;

        public StationTaskService(AsZeroDbContext dbContext, SysLogService sys_LogService, StationService stationService, Proc_StationTask_PeiFangService peiFangService)
        {
            this._dbContext = dbContext;
            this.sys_LogService = sys_LogService;
            _stationService = stationService;
            _PeiFangService = peiFangService;
        }

        public async Task<IList<Base_StationTask>> GetList()
        {
            var list = await this._dbContext.Base_StationTasks
                .Where(r => !r.IsDeleted)
                .ToListAsync();
            return list;
        }



        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<StationTaskDTO>> GetStationTaskDTOByStation(Base_Step step, string stationCode, int productId)
        {
            try
            {
                var listResult = new List<StationTaskDTO>();

                //var list = await GetStationTaskByStation(step.Id);
                var list = await GetStationTaskByStep(step.Id, productId);
                foreach (var mapping in list)
                {
                    var StationTaskScrew = await _dbContext.Base_StationTaskScrews.Include(s => s.StationTask).Where(d => d.IsDeleted == false && d.StationTaskId == mapping.Id).ToListAsync();

                    var DeviceNosList = StationTaskScrew.Select(s => s.DeviceNos).ToList();
                    var DeviceNos = String.Join(",", DeviceNosList.ToArray());
                    if (!string.IsNullOrEmpty(DeviceNos))
                    {
                        DeviceNosList = DeviceNos.Split(",").ToList();
                        mapping.ListResource = _dbContext.Base_ProResources.Where(r => r.StationCode == stationCode && r.ProResourceType == ProResourceTypeEnum.拧紧枪 && DeviceNosList.Contains(r.DeviceNo) && !r.IsDeleted).ToList();
                    }

                    var bomList = await _dbContext.Base_StationTaskBoms.Include(s => s.StationTask).Where(d => d.IsDeleted == false && d.StationTaskId == mapping.Id).ToListAsync();
                    //foreach (var bom in bomList)
                    //{
                    //    if (bom.TracingType == Common.Library.CommonEnum.TracingTypeEnum.精追)
                    //    {
                    //        bom.CodeRexText = $"批次码[{bom.GoodsPNRex}] 精追码[{bom.OuterGoodsPNRex}]";
                    //    }
                    //    if (bom.TracingType == Common.Library.CommonEnum.TracingTypeEnum.批追)
                    //    {
                    //        bom.CodeRexText = $"批次码[{bom.GoodsPNRex}]";
                    //    }
                    //    if (bom.TracingType == Common.Library.CommonEnum.TracingTypeEnum.扫库存)
                    //    {
                    //        bom.CodeRexText = $"库存码[{bom.GoodsPNRex}]";
                    //    }
                    //}

                    //var leaks = _dbContext.Base_StationTaskLeaks.Where(d => d.IsDeleted == false && d.StationTaskId == mapping.Id).ToList();
                    //var LaMaoGun = _dbContext.Base_StationTaskLaMaoGuns.FirstOrDefault(d => d.IsDeleted == false && d.StationTaskId == mapping.Id);

                    listResult.Add(new StationTaskDTO
                    {
                        Id = mapping.Id,
                        Sequence = mapping.Sequence,
                        StationTask = mapping,
                        StationTaskId = mapping.Id,
                        StepId = step.Id,
                        //StationTaskLeaks = leaks,
                        //StationTaskLaMaoGun = LaMaoGun,
                        StationTaskBom = bomList,
                        StationTaskScrew = StationTaskScrew,
                        StationTaskAnyLoad = _dbContext.Base_StationTaskAnyLoads.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskCheckTimeout = _dbContext.Base_StationTaskCheckTimeOuts.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskStewingTime = _dbContext.Base_StationTaskStewingTimes.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskUserInput = _dbContext.Base_StationTaskUserInputs.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskScanCollect = _dbContext.Base_StationTaskScanCollects.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskScanAccountCard = _dbContext.Base_StationTaskScanAccountCards.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                    });
                }
                return listResult;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<StationTaskDTO>> GetStationTaskDTOByStation(Base_Station station, int productId)
        {
            try
            {
                var step = station.Step;
                var stationCode = station.Code;
                var listResult = new List<StationTaskDTO>();

                //var list = await GetStationTaskByStation(step.Id);
                var list = await GetStationTaskByStep(step.Id, productId);
                foreach (var mapping in list)
                {
                    var StationTaskScrew = await _dbContext.Base_StationTaskScrews.Include(s => s.StationTask).Where(d => d.IsDeleted == false && d.StationTaskId == mapping.Id).ToListAsync();

                    var DeviceNosList = StationTaskScrew.Select(s => s.DeviceNos).ToList();
                    var DeviceNos = String.Join(",", DeviceNosList.ToArray());
                    if (!string.IsNullOrEmpty(DeviceNos))
                    {
                        DeviceNosList = DeviceNos.Split(",").ToList();
                        mapping.ListResource = _dbContext.Base_ProResources.Where(r => r.StationCode == stationCode && r.ProResourceType == ProResourceTypeEnum.拧紧枪 && DeviceNosList.Contains(r.DeviceNo) && !r.IsDeleted).ToList();
                    }

                    var bomList = await _dbContext.Base_StationTaskBoms.Include(s => s.StationTask).Where(d => d.IsDeleted == false && d.StationTaskId == mapping.Id).ToListAsync();
                    //foreach (var bom in bomList)
                    //{
                    //    if (bom.TracingType == Common.Library.CommonEnum.TracingTypeEnum.精追)
                    //    {
                    //        bom.CodeRexText = $"批次码[{bom.GoodsPNRex}] 精追码[{bom.OuterGoodsPNRex}]";
                    //    }
                    //    if (bom.TracingType == Common.Library.CommonEnum.TracingTypeEnum.批追)
                    //    {
                    //        bom.CodeRexText = $"批次码[{bom.GoodsPNRex}]";
                    //    }
                    //    if (bom.TracingType == Common.Library.CommonEnum.TracingTypeEnum.扫库存)
                    //    {
                    //        bom.CodeRexText = $"库存码[{bom.GoodsPNRex}]";
                    //    }
                    //}

                    //var leaks = _dbContext.Base_StationTaskLeaks.Where(d => d.IsDeleted == false && d.StationTaskId == mapping.Id).ToList();
                    //var LaMaoGun = _dbContext.Base_StationTaskLaMaoGuns.FirstOrDefault(d => d.IsDeleted == false && d.StationTaskId == mapping.Id);

                    listResult.Add(new StationTaskDTO
                    {
                        Id = mapping.Id,
                        Sequence = mapping.Sequence,
                        StationTask = mapping,
                        StationTaskId = mapping.Id,
                        StepId = step.Id,
                        StationTaskBom = bomList,
                        StationTaskScrew = StationTaskScrew,
                        StationTaskAnyLoad = _dbContext.Base_StationTaskAnyLoads.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskCheckTimeout = _dbContext.Base_StationTaskCheckTimeOuts.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskStewingTime = _dbContext.Base_StationTaskStewingTimes.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskUserInput = _dbContext.Base_StationTaskUserInputs.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskScanCollect = _dbContext.Base_StationTaskScanCollects.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                        StationTaskScanAccountCard = _dbContext.Base_StationTaskScanAccountCards.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                    });
                }
                return listResult;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// 通过工位ID查询
        /// </summary>
        public async Task<List<Base_StationTask>> GetStepTaskByStep(int stepId)
        {
            var list = await _dbContext.Base_StationTasks.Include(s => s.Step).Where(d => d.IsDeleted == false && d.StepId == stepId).OrderBy(s => s.Sequence).ToListAsync();
            return list;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_StationTask>> GetAll(string? key)
        {
            if (key == null)
            {
                var list = await _dbContext.Base_StationTasks.Where(d => d.IsDeleted == false).ToListAsync();
                return list;
            }
            else
            {
                var list = await _dbContext.Base_StationTasks.Where(d => d.IsDeleted == false && (d.Code.Contains(key) || d.Name.Contains(key))).ToListAsync();
                return list;
            }
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Base_StationTask> GetById(int id)
        {
            var entity = await _dbContext.Base_StationTasks.Include(o => o.Step).Where(o => o.Id == id).FirstOrDefaultAsync();

            return entity;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Base_StationTask> Add(Base_StationTask entity, string op)
        {
            var res = await _dbContext.AddAsync(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位任务，工位id:{entity.StepId}", Operator = op });
            await _dbContext.SaveChangesAsync();


            return res.Entity;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task Delete(Base_StationTask entity, string op)
        {
            entity.IsDeleted = true;
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
            var list = await this.GetStationTaskByStep(entity.StepId, entity.ProductId);
            var newlist = list.OrderBy(x => x.Sequence).ToList();
            for (int i = 0; i < newlist.Count; i++)
            {
                newlist[i].Sequence = i + 1;
            }
            this._dbContext.UpdateRange(newlist);
            await _dbContext.SaveChangesAsync();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工序任务，工序id:{entity.StepId}" });
        }

        public async Task DeleteByTaskID(int taskid, string op)
        {
            var entity = await GetById(taskid);

            entity.IsDeleted = true;

            _dbContext.UpdateRange(entity);
            //await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位任务，工位id:{entity.StationId}", Operator = entity.UpdateUser.Account });
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位任务，工位id:{entity.StepId}", Operator = op });
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Base_StationTask> Update(Base_StationTask entity, string op)
        {
            var resold = _dbContext.Base_StationTasks.Where(a => a.Id == entity.Id && !a.IsDeleted).Select(a => a.StepId).FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位任务，工位id:{resold}", Operator = op });


            var res = _dbContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后工位任务，工位id:{entity.StepId}", Operator = op });
            await _dbContext.SaveChangesAsync();
            return res.Entity;
        }


        /// <summary>
        /// 通过工位ID查询
        /// </summary>
        public async Task<List<Base_StationTask>> GetStationTaskByStep(int stepId, int productId)
        {
            var list = await _dbContext.Base_StationTasks.Include(s => s.Step).Where(d => d.IsDeleted == false && d.StepId == stepId && d.ProductId == productId).OrderBy(s => s.Sequence).ToListAsync();
            return list;
        }

        public async Task<bool> HasCode(string code)
        {
            var entity = await _dbContext.Base_StationTasks.Where(d => !d.IsDeleted && d.Code == code).FirstOrDefaultAsync();
            if (entity != null)
                return true;
            else
                return false;
        }

        public async Task<Base_StationTask?> GetStationTaskBySequence(int sequence, int stepId, int productid)
        {
            var entity = _dbContext.Base_StationTasks.Where(o => !o.IsDeleted && o.Sequence == sequence && o.StepId == stepId && o.ProductId == productid).FirstOrDefault();
            return entity;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<StationTaskDTO>> GetStepTaskDTOByStep(int productId, int stepId, string stationCode)
        {
            var listResult = new List<StationTaskDTO>();
            var list = await GetStationTaskByStep(productId, stepId);
            foreach (var mapping in list)
            {
                var StationTaskScrew = await _dbContext.Base_StationTaskScrews.Include(s => s.StationTask).Where(d => d.IsDeleted == false && d.StationTaskId == mapping.Id).ToListAsync();

                var DeviceNosList = StationTaskScrew.Select(s => s.DeviceNos).ToList();
                var DeviceNos = String.Join(",", DeviceNosList.ToArray());
                DeviceNosList = DeviceNos.Split(",").ToList();
                mapping.ListResource = _dbContext.Base_ProResources.Where(r => r.StationCode == stationCode
                && r.ProResourceType == ProResourceTypeEnum.拧紧枪 && DeviceNosList.Contains(r.DeviceNo) && !r.IsDeleted).ToList();

                var bomList = await _dbContext.Base_StationTaskBoms.Include(s => s.StationTask).Where(d => d.IsDeleted == false && d.StationTaskId == mapping.Id).ToListAsync();
                listResult.Add(new StationTaskDTO
                {
                    Id = mapping.Id,
                    Sequence = mapping.Sequence,
                    StationTask = mapping,
                    StationTaskId = mapping.Id,
                    StepId = mapping.StepId,
                    StationTaskBom = bomList,
                    StationTaskScrew = StationTaskScrew,
                    StationTaskAnyLoad = _dbContext.Base_StationTaskAnyLoads.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                    StationTaskCheckTimeout = _dbContext.Base_StationTaskCheckTimeOuts.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                    StationTaskStewingTime = _dbContext.Base_StationTaskStewingTimes.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                    StationTaskUserInput = _dbContext.Base_StationTaskUserInputs.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                    StationTaskScanCollect = _dbContext.Base_StationTaskScanCollects.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                    StationTaskScanAccountCard = _dbContext.Base_StationTaskScanAccountCards.FirstOrDefault(a => !a.IsDeleted && a.StationTaskId == mapping.Id),
                });
            }
            return listResult;
        }

        /// <summary>
        /// 加载未完成的工位任务数据
        /// </summary>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public async Task<Response<StaionHisDataDTO>> LoadStationTaskHistoryData(int stepId, string packCode)
        {
            var result = new Response<StaionHisDataDTO>() { Result = new StaionHisDataDTO() };
            try
            {
                //(ps.Status == StationTaskStatusEnum.未开始 || ps.Status == StationTaskStatusEnum.进行中)  去除此条件,返工逻辑
                var pStationTaskMain = _dbContext.Proc_StationTask_Mains.FirstOrDefault(ps => ps.IsDeleted == false && ps.StepId == stepId && ps.PackCode == packCode);
                if (pStationTaskMain != null)
                {
                    pStationTaskMain.Proc_StationTask_Records = _dbContext.Proc_StationTask_Records.Where(t => t.IsDeleted == false && t.Proc_StationTask_MainId == pStationTaskMain.Id).ToList();

                    if (pStationTaskMain.Proc_StationTask_Records != null && pStationTaskMain.Proc_StationTask_Records.Count > 0)
                    {
                        foreach (var record in pStationTaskMain.Proc_StationTask_Records)
                        {
                            record.Proc_StationTask_Boms = _dbContext.Proc_StationTask_Boms.Where(t => t.IsDeleted == false && t.StationTask_RecordId == record.Id).ToList();
                            if (record.Proc_StationTask_Boms != null && record.Proc_StationTask_Boms.Count > 0)
                            {
                                foreach (var goods in record.Proc_StationTask_Boms)
                                {
                                    goods.Proc_StationTask_BomDetails = _dbContext.Proc_StationTask_BomDetails.Where(t => t.IsDeleted == false && t.Proc_StationTask_BomId == goods.Id).ToList();
                                }
                            }

                            record.Proc_StationTask_BlotGuns = _dbContext.Proc_StationTask_BlotGuns.Where(t => t.IsDeleted == false && t.StationTask_RecordId == record.Id).ToList();

                            if (record.Proc_StationTask_BlotGuns != null && record.Proc_StationTask_BlotGuns.Count > 0)
                            {
                                foreach (var boltGun in record.Proc_StationTask_BlotGuns)
                                {
                                    boltGun.Proc_StationTask_BlotGunDetails = _dbContext.Proc_StationTask_BlotGunDetails.Where(t => t.IsDeleted == false && t.Proc_StationTask_BlotGunId == boltGun.Id).ToList();
                                }
                            }
                            record.Proc_StationTask_ScanAccountCard = await _dbContext.Proc_StationTask_ScanAccountCards.FirstOrDefaultAsync(t => t.IsDeleted == false && t.StationTask_RecordId == record.Id);
                            record.Proc_StationTask_AnyLoad = await _dbContext.Proc_StationTask_AnyLoads.FirstOrDefaultAsync(t => t.IsDeleted == false && t.StationTask_RecordId == record.Id);
                            record.Proc_StationTask_ScanCollect = await _dbContext.Proc_StationTask_ScanCollects.FirstOrDefaultAsync(t => t.IsDeleted == false && t.StationTask_RecordId == record.Id);
                            record.Proc_StationTask_UserInput = await _dbContext.Proc_StationTask_UserInputs.FirstOrDefaultAsync(t => t.IsDeleted == false && t.StationTask_RecordId == record.Id);
                            record.Proc_StationTask_GluingTime = await _dbContext.Proc_StationTask_CheckTimeouts.FirstOrDefaultAsync(t => t.IsDeleted == false && t.StationTask_RecordId == record.Id);

                            record.Proc_StationTask_StewingTime = await _dbContext.Proc_StationTask_TimeRecords.FirstOrDefaultAsync(t => t.IsDeleted == false && t.Proc_StationTask_RecordId == record.Id);
                        }
                    }
                    var step = await _stationService.GetPrevStepByPackCode(stepId, pStationTaskMain.PackCode);
                }
                result.Code = 200;
                result.Message = "读取成功";
                result.Result.Proc_StationTask_Main = pStationTaskMain;
                if (pStationTaskMain != null)
                {
                    result.Result.StationTaskList = await _PeiFangService.GetPeiFangDataByID((int)pStationTaskMain?.PeiFang_MD5_ID);
                }

                var pStationTaskMainLast = _dbContext.Proc_StationTask_Mains.FirstOrDefault(ps => ps.IsDeleted == false && ps.PackCode == packCode && (ps.Status == StationTaskStatusEnum.未开始 || ps.Status == StationTaskStatusEnum.进行中));
                if (pStationTaskMainLast != null)
                {
                    var stationLast = _dbContext.Base_Stations.FirstOrDefault(s => s.Id == pStationTaskMainLast.StationId && s.IsDeleted == false);
                    result.Result.LastStation = stationLast;
                }
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = $"读取失败！{ex.Message}";
            }
            return result;
        }


        /// <summary>
        /// 更新工位任务顺序
        /// </summary>
        /// <param name="taskorder">需要更新的任务和顺序</param>
        /// <returns></returns>
        public async Task<(Boolean, string)> UpTaskOrder(string[] taskorders)
        {
            try
            {
                var tasks = new List<Base_StationTask>();
                foreach (var item in taskorders)
                {
                    if (item.Length > 0)
                    {
                        var taskorder = item.Split(",");
                        if (taskorder.Length > 0)
                        {
                            var task = this._dbContext.Base_StationTasks.FirstOrDefault(o => !o.IsDeleted && o.Id == int.Parse(taskorder[0]));

                            if (task != null)
                            {
                                task.Sequence = int.Parse(taskorder[1]);
                                tasks.Add(task);
                            }
                        }

                    }

                }
                this._dbContext.Base_StationTasks.UpdateRange(tasks);
                await this._dbContext.SaveChangesAsync();
                return (true, "");
            }
            catch (Exception ex)
            {

                return (false, ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public async Task<bool> CheckStationTaskExistByStepId(int stepId)
        {
            Base_StationTask? stationTask = await _dbContext.Base_StationTasks.FirstOrDefaultAsync(task => task.StepId == stepId);
            return stationTask != null;
        }
    }
}
