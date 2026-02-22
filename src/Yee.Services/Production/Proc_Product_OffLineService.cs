using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using FutureTech.Dal.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Entitys.Response;
using Yee.Services.Request;

namespace Yee.Services.Production
{
    public class Proc_Product_OffLineService
    {
        private readonly AsZeroDbContext _dBContext;
        private readonly ProductService productService;

        public Proc_Product_OffLineService(AsZeroDbContext asZeroDbContext, ProductService productService)
        {
            this._dBContext = asZeroDbContext;
            this.productService = productService;
        }

        public async Task<MethodResult> Add(Proc_Product_Offline entity)
        {
            MethodResult result = new MethodResult();
            try
            {
                ///1.获取全部产品
                var allproductlist = await this.productService.GetAll(null);
                foreach (var item in allproductlist)
                {
                    if (IsMatch(entity.ProductCode, item.PackPNRule))
                    {
                        entity.Productid = item.Id;
                        break;
                    }
                }
                if (entity.Productid == null)
                {
                    result.Success = false;
                    result.ResultMsg = "无匹配产品,入库失败!";
                    return result;
                }
                var res = await _dBContext.AddAsync(entity);

                var taskmain = await _dBContext.Proc_StationTask_Mains.Where(o => !o.IsDeleted && o.StepId == entity.StepId && o.PackCode == entity.ProductCode).FirstOrDefaultAsync();
                if (taskmain == null)
                {
                    Proc_StationTask_Main task = new Proc_StationTask_Main()
                    {
                        //PackCode=entity.ProductCode,

                        PackCode = entity.PackCode ?? string.Empty,
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
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ResultMsg = $"{ex.Message}";
                return result;
            }
        }
        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Proc_Product_Offline>> Load(Prodct_StatisticsInput request)
        {
            //var query = this._dBContext.Proc_Product_OffLine.Where(o => !o.IsDeleted && o.State == request.states);
            var query = this._dBContext.Proc_Product_Offlines.Where(o => !o.IsDeleted && o.Productid == request.Productid && o.InstorageTime.Date >= request.BegainTime && o.InstorageTime.Date < request.EndTime);
            if (request.Key != null)
            {
                query = query.Where(o => o.ProductCode.Contains(request.Key));
            }
            if (request.BegainTime != null && request.EndTime != null)
            {
                switch (request.SelectType)
                {
                    case SelectTypeEnmu.日:
                        query = query.Where((o) => o.InstorageTime.Date >= request.BegainTime.Value.Date && o.InstorageTime.Date <= request.EndTime.Value.Date);

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
            var list = await query.ToListAsync();
            return list;
        }
        /// <summary>
        /// 验证字符串是否匹配正则表达式描述的规则
        /// </summary>
        /// <param name="inputStr">待验证的字符串</param>
        /// <param name="patternStr">正则表达式字符串</param>
        /// <returns>是否匹配</returns>
        private bool IsMatch(string inputStr, string patternStr)
        {
            try
            {
                if (string.IsNullOrEmpty(inputStr) || string.IsNullOrEmpty(patternStr)) return false;
                if (inputStr.Length != patternStr.Length) return false;
                if (!inputStr.StartsWith(patternStr.Replace("*", ""))) return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 获取一段时间内下线的产品包的饼状图数据
        /// </summary>
        /// <param name="BeginTime">开始时间</param>
        /// <param name="Endtime">结束时间</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public async Task<List<ChartModel>> GetRoundCakeData(InStatisticsDto dto)
        {
            List<ChartModel> chartModels = new List<ChartModel>();
            try
            {
                var alldata = await this._dBContext.Proc_Product_Offlines.Where(o => !o.IsDeleted && o.State == dto.states && o.InstorageTime.Date >= dto.BeginTime.Date && o.InstorageTime.Date <= dto.EndTime.Date).Include(o => o.Product).ToListAsync();
                var datas = alldata.GroupBy(o => o.Productid).ToList();
                foreach (var data in datas)
                {
                    var productname = data.First().Product.Name;
                    var chartmodel = new ChartModel() { ProductId = data.Key.Value, Percentage = data.Count() * 100 / alldata.Count, ProductName = productname };
                    chartModels.Add(chartmodel);
                }

            }
            catch (Exception)
            {

                throw;
            }
            return chartModels;
        }
        /// <summary>
        /// 获取一段时间内下线的产品包的折线图数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<ChartModel>> GetBrokenLineData(InStatisticsDto dto)
        {
            List<ChartModel> chartModels = new List<ChartModel>();



            try
            {
                var alldata = await this._dBContext.Proc_Product_Offlines.Where(o => !o.IsDeleted && o.State == dto.states && o.InstorageTime.Date >= dto.BeginTime.Date && o.InstorageTime.Date <= dto.EndTime.Date && o.Productid == dto.Productid).Include(o => o.Product).ToListAsync();
                var datas = alldata.OrderBy(o => o.InstorageTime).GroupBy(o => o.InstorageTime.Date).ToList();

                foreach (var data in datas)
                {
                    var productname = data.First().Product.Name;
                    var chartmodel = new ChartModel() { Date = data.Key.ToString("yyyy-MM-dd"), Count = data.Count(), ProductName = productname };
                    chartModels.Add(chartmodel);
                }

            }
            catch (Exception)
            {

                throw;
            }
            return chartModels;
        }
        /// <summary>
        /// 根据产品ID获取数据列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Proc_Product_Offline>> GetListByProductId(InStatisticsDto dto)
        {
            var dd = dto.EndTime.Date.AddDays(1);
            var query = this._dBContext.Proc_Product_Offlines.Where(o => !o.IsDeleted && o.State == dto.states && o.InstorageTime >= dto.BeginTime.Date && o.InstorageTime <= dto.EndTime.Date.AddDays(1) && o.Productid == dto.Productid);
            if (!string.IsNullOrEmpty(dto.Key))
            {
                query = query.Where(o => o.ProductCode.Contains(dto.Key));
            }
            query = query.Include(o => o.Product).Include(o => o.Station);
            return await query.ToListAsync();
        }


        /// <summary>
        /// 查询产量
        /// </summary>
        /// <returns></returns>
        public async Task<OutPut> GetOutPut()
        {
            var weekfirst = DateTime.Now.Date.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            var weeklast = DateTime.Now.Date.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).AddDays(6);
            var monthfirst = DateTime.Now.Date.AddDays(1 - DateTime.Now.Date.Day);
            var monthlast = DateTime.Now.Date.AddDays(1 - DateTime.Now.Date.Day).AddMonths(1).AddDays(-1);
            OutPut outPut = new OutPut();
            var TodayOrder = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= DateTime.Now.Date && o.State == Proc_ProductStates.正常下线 && o.ProductCode.Length > 20).ToListAsync();
            var TodayWeight = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= DateTime.Now.Date && o.State == Proc_ProductStates.正常下线 && o.ProductCode.Length < 20).ToListAsync();
            var WeekOrder = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= weekfirst && o.InstorageTime <= weeklast && o.State == Proc_ProductStates.正常下线 && o.ProductCode.Length > 20).ToListAsync();
            var WeekWeight = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= weekfirst && o.InstorageTime <= weeklast && o.State == Proc_ProductStates.正常下线 && o.ProductCode.Length < 20).ToListAsync();
            var MonthOrder = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= monthfirst && o.InstorageTime <= monthlast && o.State == Proc_ProductStates.正常下线 && o.ProductCode.Length > 20).ToListAsync();
            var MonthWeight = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= monthfirst && o.InstorageTime <= monthlast && o.State == Proc_ProductStates.正常下线 && o.ProductCode.Length < 20).ToListAsync();
            var TodayOrderNG = await this._dBContext.Proc_Product_Offlines.Where
    (o => !o.IsDeleted && o.InstorageTime >= DateTime.Now.Date && o.State == Proc_ProductStates.NG下线 && o.ProductCode.Length > 20).ToListAsync();
            var TodayWeightNG = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= DateTime.Now.Date && o.State == Proc_ProductStates.NG下线 && o.ProductCode.Length < 20).ToListAsync();
            var WeekOrderNG = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= weekfirst && o.InstorageTime <= weeklast && o.State == Proc_ProductStates.NG下线 && o.ProductCode.Length > 20).ToListAsync();
            var WeekWeightNG = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= weekfirst && o.InstorageTime <= weeklast && o.State == Proc_ProductStates.NG下线 && o.ProductCode.Length < 20).ToListAsync();
            var MonthOrderNG = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= monthfirst && o.InstorageTime <= monthlast && o.State == Proc_ProductStates.NG下线 && o.ProductCode.Length > 20).ToListAsync();
            var MonthWeightNG = await this._dBContext.Proc_Product_Offlines.Where
                (o => !o.IsDeleted && o.InstorageTime >= monthfirst && o.InstorageTime <= monthlast && o.State == Proc_ProductStates.NG下线 && o.ProductCode.Length < 20).ToListAsync();
            outPut.TodayOrder = TodayOrder.Count;
            outPut.TodayWeight = TodayWeight.Count;
            outPut.WeekOrder = WeekOrder.Count;
            outPut.WeekWeight = WeekWeight.Count;
            outPut.MonthOrder = MonthOrder.Count;
            outPut.MonthWeight = MonthWeight.Count;

            outPut.TodayOrderNG = TodayOrderNG.Count;
            outPut.TodayWeightNG = TodayWeightNG.Count;
            outPut.WeekOrderNG = WeekOrderNG.Count;
            outPut.WeekWeightNG = WeekWeightNG.Count;
            outPut.MonthOrderNG = MonthOrderNG.Count;
            outPut.MonthOrderNG = MonthWeightNG.Count;
            return outPut;
        }

        /// <summary>
        /// 首页统计数据
        /// </summary>
        /// <returns></returns>
        public async Task<Response<IndexStatisticsResponse>> GetIndexStatistics()
        {
            var result = new Response<IndexStatisticsResponse>();
            try
            {
                var data = new IndexStatisticsResponse();


                var op090Ids = await _dBContext.Base_Steps.Where(e => e.Code == "OP090").Select(e => e.Id).ToArrayAsync();
                var op210Ids = await _dBContext.Base_Steps.Where(e => e.Code == "OP210").Select(e => e.Id).ToArrayAsync();


                var moduleInBoxCompletePackQuery = _dBContext.Proc_StationTask_Mains.Where(e => e.IsDeleted == false && op090Ids.Contains(e.StepId ?? 0) && e.Status == Common.Library.CommonEnum.StationTaskStatusEnum.已完成);

                var completePackQuery = _dBContext.Proc_StationTask_Mains.Where(e => e.IsDeleted == false && op210Ids.Contains(e.StepId ?? 0) && e.Status == Common.Library.CommonEnum.StationTaskStatusEnum.已完成);

                var moduleInBoxTimeOutPackQuery = _dBContext.Proc_StationTask_CheckTimeouts.Where(e => e.IsDeleted == false && e.TimeName == "涂胶入箱超时" && e.Status == Common.Library.CommonEnum.StationTaskStatusEnum.已完成 && e.Pass == false);

                var screwTimeOutPackQuery = _dBContext.Proc_StationTask_CheckTimeouts.Where(e => e.IsDeleted == false && e.TimeName == "涂胶模组拧紧时长" && e.Status == Common.Library.CommonEnum.StationTaskStatusEnum.已完成 && e.Pass == false);

                #region 今日数据统计

                var todayBegin = DateTime.Now.Date;
                var todayEnd = todayBegin.AddDays(1);

                //今日入箱完成数
                data.DayModuleInBoxCompletePackCount = await moduleInBoxCompletePackQuery
                    .Where(e => e.CreateTime >= todayBegin && e.CreateTime < todayEnd)
                    .CountAsync();

                //今天Pack完成数
                data.DayCompletePackCount = await completePackQuery
                    .Where(e => e.CreateTime >= todayBegin && e.CreateTime < todayEnd)
                    .CountAsync();

                //今天入箱超时数
                data.DayModuleInBoxTimeOutPackCount = await moduleInBoxTimeOutPackQuery
                    .Where(e => e.CreateTime >= todayBegin && e.CreateTime < todayEnd)
                    .CountAsync();

                //今日拧紧超时数
                data.DayModuleInBoxTimeOutPackCount = await screwTimeOutPackQuery
                    .Where(e => e.CreateTime >= todayBegin && e.CreateTime < todayEnd)
                    .CountAsync();

                #endregion

                #region 本周统计数据

                var weekIndex = (int)DateTime.Now.DayOfWeek;
                weekIndex = weekIndex == 0 ? 7 : weekIndex;
                var weekBegin = DateTime.Now.Date.AddDays(1 - weekIndex);
                var weekEnd = weekBegin.AddDays(7);

                //本周Pack完成数
                data.WeekCompletePackCount = await completePackQuery
                    .Where(e => e.CreateTime >= weekBegin && e.CreateTime < weekEnd)
                    .CountAsync();

                //本周入箱超时数
                data.WeekModuleInBoxTimeOutPackCount = await moduleInBoxTimeOutPackQuery
                    .Where(e => e.CreateTime >= weekBegin && e.CreateTime < weekEnd)
                    .CountAsync();

                //本周拧紧超时数
                data.WeekModuleInBoxTimeOutPackCount = await screwTimeOutPackQuery
                    .Where(e => e.CreateTime >= weekBegin && e.CreateTime < weekEnd)
                    .CountAsync();

                #endregion

                #region 本月统计数据

                var monthBegin = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);
                var monthEnd = monthBegin.AddMonths(1);

                //本月Pack完成数
                data.MonthCompletePackCount = await completePackQuery
                    .Where(e => e.CreateTime >= monthBegin && e.CreateTime < monthEnd)
                    .CountAsync();
                //本月入箱超时数
                data.MonthModuleInBoxTimeOutPackCount = await moduleInBoxTimeOutPackQuery
                    .Where(e => e.CreateTime >= monthBegin && e.CreateTime < monthEnd)
                    .CountAsync();
                //本月拧紧超时数
                data.MonthModuleInBoxTimeOutPackCount = await screwTimeOutPackQuery
                    .Where(e => e.CreateTime >= monthBegin && e.CreateTime < monthEnd)
                    .CountAsync();

                #endregion

                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    }
}
