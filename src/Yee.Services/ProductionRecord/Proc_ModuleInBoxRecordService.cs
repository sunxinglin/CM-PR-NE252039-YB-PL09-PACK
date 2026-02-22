using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Entitys.Request;
using Yee.Services.Request;
using Yee.Tools;

namespace Yee.Services.ProductionRecord
{
    public class Proc_ModuleInBoxRecordService
    {
        private readonly AsZeroDbContext _dbContext;

        public Proc_ModuleInBoxRecordService(AsZeroDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Response<List<Proc_ModuleInBox_ModuleRecord>>> GetPageList(ModuleInBoxInfosDto dto)
        {
            var result = new Response<List<Proc_ModuleInBox_ModuleRecord>>();
            //try
            //{
            //    var query = _dbContext.Proc_BlockInCase_BlockRecords.OrderByDescending(o => o.CreateTime).Where(e => e.IsDeleted == false);
            //    if (dto.PackPN != "")
            //    {
            //        query = query.Where(e => e.PackCode == dto.PackPN);
            //        if (query.Count() == 0)
            //        {
            //            query= _dbContext.Proc_BlockInCase_BlockRecords.OrderByDescending(o => o.CreateTime).Where(e => e.IsDeleted == false && e.BlockCode==dto.PackPN);
            //        }
            //    }

            //    if (dto.EndTime != null)
            //    {
            //        query = query.Where(o => o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
            //    }

            //    if (dto.BeginTime != null)
            //    {
            //        query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
            //    }
            //    //总数
            //    result.Count = await query.CountAsync();

            //    //列表
            //    var list = await query
            //        .OrderByDescending(e => e.Id)
            //        .Skip((dto.Page - 1) * dto.Limit)
            //        .Take(dto.Limit)
            //        .ToListAsync();

            //    result.Data = list;
            //}
            //catch (Exception ex)
            //{
            //    result.Code = 500;
            //    result.Message = ex.InnerException?.Message ?? ex.Message;
            //}
            return result;
        }


        /// <summary>
        /// Block入箱数据导出
        /// </summary>
        /// <returns></returns>
        public async Task<(Boolean, string)> ExportGluingInfo(ModuleInBoxInfosDto dto)
        {
            //文件路径
            string filepath = Directory.GetCurrentDirectory() + @"\Block入箱数据导出.xls";
            //判断文件路径是否存在 存在就删除
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            //创建文件
            var stream = new FileStream(filepath, FileMode.OpenOrCreate);
            //try
            //{
            //    //创建list
            //    List<BlockInfoRecordToExcel> gulingExcellist = new();
            //    //获取Pack加压数据
            //    var gluingDatas = await GetPageListExport(dto);
            //    if (gluingDatas == null)
            //    {
            //        return (false, $"找不到={dto.PackPN}的数据");
            //    }
            //    foreach (var gulingNeedToExport in gluingDatas.Data)
            //    {
            //        gulingExcellist.Add(
            //            new BlockInfoRecordToExcel
            //            {
            //                PackCode = gulingNeedToExport.PackCode,
            //                BlockCode = gulingNeedToExport.BlockCode,
            //                CreateTime = gulingNeedToExport.CreateTime.ToString(),
            //            }
            //        );
            //    }
            //    //转换list为Excel 
            //    ExcelPackage excelPackage = new ExcelPackage();
            //    excelPackage = ExcelToEntity.ListToExcek<BlockInfoRecordToExcel>(excelPackage, "Block入箱数据导出", 2, gulingExcellist);
            //    excelPackage.SaveAs(stream);
            //}
            //catch (Exception ex)
            //{
            //    stream.Close();
            //    return (false, $"导出出现错误{ex.Message}");
            //}
            stream.Close();
            return (true, filepath);

        }

        /// <summary>
        /// 导出专用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Response<List<Proc_ModuleInBox_ModuleRecord>>> GetPageListExport(ModuleInBoxInfosDto dto)
        {
            var result = new Response<List<Proc_ModuleInBox_ModuleRecord>>();
            //try
            //{
            //    var query = _dbContext.Proc_BlockInCase_BlockRecords.OrderByDescending(o => o.CreateTime).Where(e => e.IsDeleted == false);
            //    if (dto.PackPN != "")
            //    {
            //        query = query.Where(e => e.PackCode == dto.PackPN);
            //        if (query.Count() == 0)
            //        {
            //            query = _dbContext.Proc_BlockInCase_BlockRecords.OrderByDescending(o => o.CreateTime).Where(e => e.IsDeleted == false && e.BlockCode == dto.PackPN);
            //        }
            //    }

            //    if (dto.EndTime != null)
            //    {
            //        query = query.Where(o => o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
            //    }

            //    if (dto.BeginTime != null)
            //    {
            //        query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
            //    }
            //    //总数
            //    result.Count = await query.CountAsync();

            //    result.Data = await query.ToListAsync();
            //}
            //catch (Exception ex)
            //{
            //    result.Code = 500;
            //    result.Message = ex.InnerException?.Message ?? ex.Message;
            //}
            return result;
        }

        #region 获取模组抓件记录
        public class GetModuleGrapRecordsRequest
        {
            public int Limit { get; set; } = 20;
            public int Page { get; set; } = 1;
            public int? Id { get; set; }
            public string? CellCode { get; set; }
            public string? ModuleCode { get; set; }
            public string? PackCode { get; set; }
            public string? StationCode { get; set; }
        }

        public async Task<(List<Proc_ModuleInBox_GrapRecord>, int)> GetModuleGrapRecords(GetModuleGrapRecordsRequest request)
        {
            var query = _dbContext.Proc_ModuleInBox_GrapRecords.Where(x => !x.IsDeleted).AsQueryable();
            if (request != null)
            {
                if (request.Id != null)
                {
                    query = query.Where(x => x.Id == request.Id.Value);
                }
                if (!string.IsNullOrWhiteSpace(request.CellCode))
                {
                    query = query.Where(x => x.CellCode == request.CellCode);
                }
                if (!string.IsNullOrWhiteSpace(request.ModuleCode))
                {
                    query = query.Where(x => x.ModuleCode == request.ModuleCode);
                }
                if (!string.IsNullOrWhiteSpace(request.PackCode))
                {
                    query = query.Where(x => x.PackCode == request.PackCode);
                }
                if (!string.IsNullOrWhiteSpace(request.StationCode))
                {
                    query = query.Where(x => x.StationCode == request.StationCode);
                }
            }

            var limit = 20;
            var page = 1;
            if (request != null)
            {
                limit = request.Limit <= 0 ? 20 : request.Limit;
                page = request.Page <= 0 ? 1 : request.Page;
            }

            query = query.OrderBy(x => x.Id);
            var total = await query.CountAsync();
            var list = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

            return (list, total);
        }
        #endregion

        #region 模组装配
        /// <summary>
        /// 将某个模组装配到指定Pack包中（需要该模组已经有对应的条目，并且没有装配过），并且可以指定是否已经使用（即在单步入箱时，是否需要再次传给Catl MES）
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <param name="packCode"></param>
        /// <param name="deleted"></param>
        /// <returns></returns>
        public async Task<int> AssembleModule(string moduleCode, string packCode, bool hasUsed)
        {
            var query = _dbContext.Proc_ModuleInBox_GrapRecords.Where(x => !x.IsDeleted && x.ModuleCode == moduleCode && x.PackCode == null);

            var entity = await query.FirstOrDefaultAsync();
            if (entity ==  null)
            {
                return 0;
            }

            entity.PackCode = packCode;
            entity.HasUsed = hasUsed;
            await _dbContext.SaveChangesAsync();

            return 1;
        }

        /// <summary>
        /// 将某个模组从当前组装的Pack包中踢出，并且可以指定是否直接删除该条目
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <param name="packCode"></param>
        /// <param name="deleted"></param>
        /// <returns></returns>
        public async Task<int> KickModule(string moduleCode, string? packCode = null, bool deleted = false)
        {
            var query = _dbContext.Proc_ModuleInBox_GrapRecords.Where(x => !x.IsDeleted && x.ModuleCode == moduleCode);
            if (!string.IsNullOrWhiteSpace(packCode))
            {
                query = query.Where(x => x.PackCode == packCode);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
            {
                return 0;
            }

            entity.HasUsed = false;
            entity.IsDeleted = deleted;
            entity.PackCode = null;
            await _dbContext.SaveChangesAsync();

            return 1;
        }
        #endregion
    }
}
