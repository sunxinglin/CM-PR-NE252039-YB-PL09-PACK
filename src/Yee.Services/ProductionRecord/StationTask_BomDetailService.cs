using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Tools;

namespace Yee.Services.ProductionRecord
{
    public class StationTask_BomDetailService
    {
        private readonly AsZeroDbContext _dbContext;

        public StationTask_BomDetailService(AsZeroDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Response<List<Proc_StationTask_BomDetail>>> GetPageList(StationTaskBomDetailGetListDTO dto)
        {
            var result = new Response<List<Proc_StationTask_BomDetail>>();
            try
            {
                var query = _dbContext.Proc_StationTask_BomDetails.Include(e => e.CreateUser).OrderByDescending(o => o.CreateTime).Where(e => e.IsDeleted == false);

                //filter
                if (!string.IsNullOrWhiteSpace(dto.PackPN))
                {
                    query = query.Where(e => e.PackPN == dto.PackPN);
                }
                if (!string.IsNullOrWhiteSpace(dto.OuterCode))
                {
                    query = query.Where(e => e.GoodsOuterCode == dto.OuterCode);
                }

                if (dto.EndTime != null)
                {
                    query = query.Where(o => o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
                }

                if (dto.BeginTime != null)
                {
                    query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
                }

                query = query.Include(x => x.Proc_StationTask_Bom.StationTask_Record.Proc_StationTask_Main.Station)
                .Include(x => x.Proc_StationTask_Bom.StationTask_Record.Proc_StationTask_Main.Step);

                result.Data = await query
                    .Skip((dto.Page - 1) * dto.Limit)
                    .Take(dto.Limit)
                    .ToListAsync();

                result.Count = await query.CountAsync();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 组装数据查询 包含时间等参数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<Proc_StationTask_BomDetail>> GetBomDetail(StationTaskBomDetailGetListDTO dto)
        {
            //倒叙查询组装数据
            var query = this._dbContext.Proc_StationTask_BomDetails.Where(o => !o.IsDeleted);

            if (!string.IsNullOrWhiteSpace(dto.PackPN))
            {
                query = query.Where(e => e.PackPN == dto.PackPN);
            }

            if (!string.IsNullOrWhiteSpace(dto.OuterCode))
            {
                query = query.Where(e => e.GoodsOuterCode == dto.OuterCode);
            }

            if (dto.EndTime != null)
            {
                query = query.Where(o =>o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
            }

            if (dto.BeginTime != null)
            {
                query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
            }
            query = query.Include(o => o.CreateUser).Include(o => o.UpdateUser).Include(o => o.DeleteUser)
                .OrderByDescending(o => o.CreateTime);

            var result = await query.ToListAsync();
            return result;
        }

        /// <summary>
        /// 组装数据导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<(Boolean, string)> ExportBomInfo(StationTaskBomDetailGetListDTO dto)
        {
            //文件路径
            string filepath = Directory.GetCurrentDirectory() + @"\组装数据导出.xls";
            //判断文件路径是否存在 存在就删除
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            //创建文件
            var stream = new FileStream(filepath, FileMode.OpenOrCreate);
            try
            {
                //创建list
                List<BomInfoToExcel> BomExcellist = new();
                //获取组装数据
                var BomDatas = await GetBomDetail(dto);
                if (BomDatas == null)
                {
                    return (false, $"找不到Pack={dto.PackPN}的数据");
                }
                foreach (var BomNeedToExport in BomDatas)
                {
                    BomInfoToExcel BomExcel = new BomInfoToExcel()
                    {
                        PackPN = BomNeedToExport.PackPN,
                        GoodsOuterCode = BomNeedToExport.GoodsOuterCode,
                        GoodsName = BomNeedToExport.GoodsName,
                        GoodsPN = BomNeedToExport.GoodsPN,
                        StationId = this._dbContext.Base_Stations.Where(x => x.Id == BomNeedToExport.StationId).Select(x => x.Code).FirstOrDefault(),
                        StepId = this._dbContext.Base_Steps.Where(x => x.Id == BomNeedToExport.StepId).Select(x => x.Name).FirstOrDefault(),
                        BatchBarCode =BomNeedToExport.BatchBarCode,
                        UniBarCode= BomNeedToExport.UniBarCode,
                        UseNum = BomNeedToExport.UseNum.ToString(),
                       
                        CreaterUser = this._dbContext.Users.Where(x => x.Id == BomNeedToExport.CreateUserID).Select(x => x.Name).FirstOrDefault(),
                        CreateTime= BomNeedToExport.CreateTime.ToString(),
                    };
                    BomExcellist.Add(BomExcel);
                }
                //转换list为Excel 
                ExcelPackage excelPackage = new ExcelPackage();
                excelPackage = ExcelToEntity.ListToExcek<BomInfoToExcel>(excelPackage, "组装数据导出", 2, BomExcellist);
                excelPackage.SaveAs(stream);
            }
            catch (Exception ex)
            {
                stream.Close();
                return (false, $"导出出现错误{ex.Message}");
            }
            stream.Close();
            return (true, filepath);

        }
    }
}
