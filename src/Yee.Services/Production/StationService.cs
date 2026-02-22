using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using Ctp0600P.Shared;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.CATL;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.BaseData;
using Yee.Services.Request;

namespace Yee.Services.Production
{
    public class StationService
    {
        private readonly FlowService _flowService;
        private readonly SysLogService sys_LogService;
        private readonly AsZeroDbContext _dBContext;
        private readonly ProductService _productService;
        public StationService(AsZeroDbContext dBContext,
            FlowService flowService, SysLogService sys_LogService, ProductService productService)
        {
            _flowService = flowService;
            this.sys_LogService = sys_LogService;
            _dBContext = dBContext;
            _productService = productService;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_Station>> GetAll(string? key)
        {
            if (key == null)
            {
                var list = await _dBContext.Base_Stations.Where(d => d.IsDeleted == false).Include(d => d.Step).ToListAsync();
                return list;
            }
            else
            {
                var list = await _dBContext.Base_Stations.Where(d => d.IsDeleted == false && (d.Code.Contains(key) || d.Name.Contains(key))).Include(d => d.Step).ToListAsync();
                return list;
            }
        }
        /// 通过ID查询
        /// </summary>

        public async Task<Base_Station> Add(Base_Station entity, string op)
        {
            var res = await _dBContext.AddAsync(entity);

            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增工位, Message = $"新增工位，工位名称:{entity.Name}", Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }


        public async Task<Base_Station> Update(Base_Station entity, string op)
        {
            var resold = _dBContext.Base_Stations.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改工位, Message = $"修改前工位，工位名称:{entity.Name}", Operator = op });

            var res = _dBContext.Update(entity);

            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改工位, Message = $"修改前工位，工位名称:{entity.Name}", Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<Base_Station> GetStationByCode(string code)
        {
            var entity = await _dBContext.Base_Stations.FirstOrDefaultAsync(s => s.Code == code && s.IsDeleted == false);
            return entity;
        }
        /// <summary>
        /// 根据工序id获取工位（工序与工位具有唯一性）
        /// </summary>
        /// <param name="stepid"></param>
        /// <returns></returns>
        public async Task<Base_Station> GetStationByStepid(int stepid)
        {
            var entity = await _dBContext.Base_Stations.Include(o => o.Step).FirstOrDefaultAsync(s => s.StepId == stepid && s.IsDeleted == false);
            return entity;
        }

        public async Task<List<Base_Station>> GetStationsByStepId(int stepid)
        {
            var entity = _dBContext.Base_Stations.Include(o => o.Step).Where(s => s.StepId == stepid && s.IsDeleted == false).ToList();
            return entity;
        }

        public async Task<bool> HasCode(string code, int stepId)
        {
            var entity = await _dBContext.Base_Stations.Where(d => !d.IsDeleted && d.Code == code && d.StepId == stepId).FirstOrDefaultAsync();
            if (entity != null)
                return true;
            else
                return false;
        }

        public async Task<Base_Step> GetStationByPackCode(string code, string packPN)
        {
            var steps = await _flowService.GetStepsByFlowCode(packPN);
            if (steps == null)
            {
                return null;
            }
            var entity = await _dBContext.Base_Steps.FirstOrDefaultAsync(s => s.Code == code && s.IsDeleted == false && steps.Select(o => o.StepId).Contains(s.Id));
            return entity;
        }


        public async Task<(bool check, string checkMsg)> CheckPrevStationWorkStatus(Base_Step step, Base_Flow entityFlow, string packPN)
        {
            // 根据Pack条码 工位找到此工位在当前工艺流程中的上一个工位
            var prevStep = await GetPrevStationByPackCode_LoadPeiFang(step, entityFlow, packPN, false);
            if (prevStep != null)
            {
                if (prevStep.Steptype == StepTypeEnum.线内可跳过人工站)
                {
                    // 查找上一工位的历史纪录
                    var taskMainPrev = await _dBContext.Proc_StationTask_Mains.FirstOrDefaultAsync(p => p.IsDeleted == false && p.StepId == prevStep.Id && p.PackCode == packPN);
                    if (taskMainPrev != null && taskMainPrev.Status != Common.Library.CommonEnum.StationTaskStatusEnum.已完成)
                    {
                        return (false, $"Pack码{packPN}对应的工位数据逻辑顺序有误");
                    }
                    else if (taskMainPrev == null)
                    {
                        prevStep = await GetPrevStationByPackCode_LoadPeiFang(step, entityFlow, packPN);
                        if (prevStep != null)
                        {
                            // 查找上一工位的历史纪录
                            taskMainPrev = await _dBContext.Proc_StationTask_Mains.FirstOrDefaultAsync(p => p.IsDeleted == false && p.StepId == prevStep.Id && p.PackCode == packPN);
                            if (taskMainPrev == null || (taskMainPrev != null && taskMainPrev.Status != Common.Library.CommonEnum.StationTaskStatusEnum.已完成))
                            {
                                return (false, $"Pack码{packPN}对应的工位数据逻辑顺序有误");
                            }
                        }
                    }
                }
                else
                {
                    // 查找上一工位的历史纪录
                    var taskMainPrev = await _dBContext.Proc_StationTask_Mains.FirstOrDefaultAsync(p => p.IsDeleted == false && p.StepId == prevStep.Id && p.PackCode == packPN);
                    if (taskMainPrev == null || (taskMainPrev != null && taskMainPrev.Status != Common.Library.CommonEnum.StationTaskStatusEnum.已完成))
                    {
                        return (false, $"Pack码{packPN}对应的工位数据逻辑顺序有误");
                    }
                    var taskMain = await _dBContext.Proc_StationTask_Mains.FirstOrDefaultAsync(p => p.IsDeleted == false && p.StepId == step.Id && p.PackCode == packPN);
                    if (taskMain != null && taskMain.Status == Common.Library.CommonEnum.StationTaskStatusEnum.已完成)
                    {
                        return (false, $"Pack码{packPN}在当站已完成");
                    }
                }
                return (true, "");
            }
            return (true, "");
        }


        /// <summary>
        /// 加载配方专用  查找上一个工位
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="packPN"></param>
        /// <returns></returns>
        public async Task<Base_Step> GetPrevStationByPackCode_LoadPeiFang(Base_Step step, Base_Flow entityFlow, string packPN, bool allowPass = true)
        {
            var mapp = _dBContext.Base_FlowStepMappings.FirstOrDefault(f => !f.IsDeleted && f.FlowId == entityFlow.Id && f.StepId == step.Id);
            if (mapp == null) return null;

            if (allowPass)
            {
                var mappPrev = _dBContext.Base_FlowStepMappings.Include(m => m.Step).OrderByDescending(m => m.OrderNo).FirstOrDefault(f => !f.IsDeleted && f.FlowId == entityFlow.Id && f.OrderNo < mapp.OrderNo && f.Step.Steptype != Common.Library.CommonEnum.StepTypeEnum.线内可跳过人工站);
                if (mappPrev == null) return null;
                return _dBContext.Base_Steps.FirstOrDefault(s => s.IsDeleted == false && s.Id == mappPrev.StepId);
            }
            else
            {
                var mappPrev = _dBContext.Base_FlowStepMappings.Include(m => m.Step).OrderByDescending(m => m.OrderNo).FirstOrDefault(f => !f.IsDeleted && f.FlowId == entityFlow.Id && f.OrderNo < mapp.OrderNo);
                if (mappPrev == null) return null;
                return _dBContext.Base_Steps.FirstOrDefault(s => s.IsDeleted == false && s.Id == mappPrev.StepId);
            }

        }


        public async Task<Base_Step> GetPrevStepByPackCode(int stepId, string packPN)
        {
            var entityList = _dBContext.Products.Where(f => !string.IsNullOrEmpty(f.PackPNRule) && !f.IsDeleted).ToList();
            var entity = entityList.FirstOrDefault(f => f.PackPNRule.Length == packPN.Length && packPN.StartsWith(f.PackPNRule.TrimEnd('*')));

            if (entity != null)
            {
                var entityFlow = _dBContext.Base_Flows.FirstOrDefault(f => f.ProductId == entity.Id && !f.IsDeleted);
                if (entityFlow == null) return null;
                var mapp = _dBContext.Base_FlowStepMappings.FirstOrDefault(f => f.FlowId == entityFlow.Id && f.StepId == stepId && !f.IsDeleted);
                if (mapp == null) return null;

                var mappPrev = _dBContext.Base_FlowStepMappings.Include(s => s.Step).OrderByDescending(m => m.OrderNo).FirstOrDefault(f => f.FlowId == entityFlow.Id && f.OrderNo < mapp.OrderNo && !f.IsDeleted);
                if (mappPrev == null) return null;
                return mappPrev.Step;
            }

            return null;
        }

        public async Task<(Base_Product product, Base_Flow flow, Base_FlowStepMapping flowStepMapping, Base_Step step, Base_Station station)> GetStationInfoByProductCode(string packCode, string stepCode, string stationCode)
        {
            // 找到pack条码对应的Product
            var product = await _productService.GetByPackcode(packCode);
            if (product == null)
            {
                throw new Exception($"未找到Pack{packCode}对应的产品");
            }
            // 根据Product 找到对应得工艺流程
            var entityFlow = _dBContext.Base_Flows.FirstOrDefault(f => f.ProductId == product.Id && !f.IsDeleted);
            if (entityFlow == null)
            {
                throw new Exception($"未找到Pack码{packCode}对应的工艺流程");
            }
            // 根据Pack条码 工位编码 查找对应的工位
            var flowStepMappings = await _dBContext.Base_FlowStepMappings.Where(f => !f.IsDeleted && f.FlowId == entityFlow.Id).OrderByDescending(o => o.OrderNo).ToListAsync();
            if (flowStepMappings == null)
            {
                throw new Exception($"未找到Pack码{packCode}对应的工序");
            }

            var step = await _dBContext.Base_Steps.FirstOrDefaultAsync(s => s.Code == stepCode && s.IsDeleted == false && flowStepMappings.Select(o => o.StepId).Contains(s.Id));
            if (step == null)
            {
                throw new Exception($"未找到Pack码{packCode}对应的工序");
              ;
            }
            var flowStepMapping = flowStepMappings.Where(e => e.StepId == step.Id).FirstOrDefault();
            if (flowStepMapping == null)
            {
                throw new Exception($"未找到Pack码{packCode}对应的工序");
            }

            var station = await _dBContext.Base_Stations.FirstOrDefaultAsync(s => s.Code == stationCode && s.IsDeleted == false && s.StepId == step.Id);
            if (station == null)
            {
                throw new Exception($"未找到Pack码{packCode}对应的工站");
            }
            return (product, entityFlow, flowStepMapping, step, station);
        }

        public async Task<StepStationDTO> LoadStepStationData(string packCode, string productId, string stationCode)
        {

            // 找到pack条码对应的Product
            var product = await _productService.GetById(int.Parse(productId));
            if (product == null)
            {
                throw new Exception($"未找到Pack{packCode}对应的产品");
            }
            // 根据Product 找到对应得工艺流程
            var entityFlow = _dBContext.Base_Flows.FirstOrDefault(f => f.ProductId == product.Id && !f.IsDeleted);
            if (entityFlow == null)
            {

                throw new Exception($"未找到Pack码{packCode}对应的工艺流程");
            }
            // 根据Pack条码 工位编码 查找对应的工位
            var flowStepMappings = await _dBContext.Base_FlowStepMappings.Where(f => !f.IsDeleted && f.FlowId == entityFlow.Id).OrderByDescending(o => o.OrderNo).ToListAsync();
            if (flowStepMappings == null)
            {
                throw new Exception($"未找到Pack码{packCode}对应的工序");
            }
            var stationCodeList = stationCode.Split(',').ToList();
            var stations = _dBContext.Base_Stations.Where(s => stationCodeList.Contains(s.Code) && s.IsDeleted == false).ToList();
            if (stations == null || stations.Count == 0)
            {
                throw new Exception($"未找到Pack码{packCode}对应的工序");
            }
            var stepIds = stations.Select(s => s.StepId).ToList();

            var mappingOrder = flowStepMappings.Where(f => stepIds.Contains(f.StepId)).OrderBy(f => f.OrderNo).ToList();
            foreach (var mapping in mappingOrder)
            {
                var hisMain = _dBContext.Proc_StationTask_Mains.FirstOrDefault(m => !m.IsDeleted && m.StepId == mapping.StepId && m.Status == StationTaskStatusEnum.已完成);
                if (hisMain == null)
                {
                    var result = new StepStationDTO();
                    result.Step = _dBContext.Base_Steps.FirstOrDefault(s => s.Id == mapping.StepId);
                    result.Station = stations.FirstOrDefault(s => s.StepId == mapping.StepId);
                    return result;
                }
            }
            return null;
        }
    }
}
