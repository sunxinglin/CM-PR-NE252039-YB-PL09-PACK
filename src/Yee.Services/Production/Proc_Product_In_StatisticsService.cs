using AsZero.DbContexts;
using FutureTech.Dal.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Services.Request;

namespace Yee.Services.Production
{
    public class Proc_Product_NG_StatisticsService
    {
        private readonly AsZeroDbContext _dBContext;

        public Proc_Product_NG_StatisticsService(AsZeroDbContext asZeroDbContext)
        {
            this._dBContext = asZeroDbContext;
        }

        public async Task<Boolean> Add(Proc_Product_Offline entity)
        {
            var res = await _dBContext.AddAsync(entity);
            try
            {
                var taskmain = await _dBContext.Proc_StationTask_Mains.Where(o => !o.IsDeleted && o.StepId == entity.StepId && o.PackCode == entity.ProductCode).FirstOrDefaultAsync();
                if (taskmain == null)
                {
                    Proc_StationTask_Main task = new Proc_StationTask_Main()
                    {
                        PackCode = entity.ProductCode,
                        StationId = entity.StationId,
                        StepId = entity.StepId,
                        Status = Common.Library.CommonEnum.StationTaskStatusEnum.已完成,
                        UseAGVCode = entity.AGVCode

                    };
                    await _dBContext.AddAsync(task);
                }
                else
                {
                    taskmain.Status = Common.Library.CommonEnum.StationTaskStatusEnum.已完成;
                    _dBContext.Update(taskmain);
                }
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                //Console.WriteLine(ex);
                return false;
            }
            
        }
        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Proc_Product_Offline>> Load(Prodct_StatisticsInput request)
        {
           var query = this._dBContext.Proc_Product_Offlines.Where(o=>!o.IsDeleted && o.State== Proc_ProductStates.正常下线);


            if (request.Key!=null)
            {
              
                query = query.Where(o => o.ProductCode.Contains(request.Key));
               
            }
            if (request.BegainTime!=null && request.EndTime!=null)
            {
                switch (request.SelectType)
                {
                    case SelectTypeEnmu.日:
                        query = query.Where( (o) => o.InstorageTime >= request.BegainTime.Value.Date && o.InstorageTime <= request.EndTime.Value.Date);

                        //var dd = Expression.Add(query.Reduce(), queryrightnewday.Reduce());
                        //query = (Expression<Func<Proc_Product_In_Statistics, bool>>)Expression.Add(query.Reduce(), queryrightnewday.Reduce()).Reduce();
                        break;
                    case SelectTypeEnmu.月:
                        query = query.Where((o) => o.InstorageTime.Month >= request.BegainTime.Value.Month && o.InstorageTime.Month <= request.EndTime.Value.Month);

                        break;
                    case SelectTypeEnmu.年:
                        query = query.Where((o) => o.InstorageTime.Year >= request.BegainTime.Value.Year && o.InstorageTime.Year <= request.EndTime.Value.Year);
                        break;
                    default:
                        break;
                }
            }
          
            
            
            var list =await  query.ToListAsync();
            return list;
           
        }
        
        
    }
}
