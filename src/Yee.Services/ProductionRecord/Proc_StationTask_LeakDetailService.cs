using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Request;
using Yee.Services.Request;
using Yee.Tools;

namespace Yee.Services.ProductionRecord
{
    public class Proc_StationTask_LeakDetailService
    {
        private readonly AsZeroDbContext _dbContext;

        public Proc_StationTask_LeakDetailService(AsZeroDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Response<List<Proc_StationTask_LeakDetail>>> GetPageList(LeakInfoDTO dto)
        {
            var result = new Response<List<Proc_StationTask_LeakDetail>>();
            try
            {
                var query = _dbContext.Proc_StationTask_LeakDetails.OrderByDescending(o => o.CreateTime).Where(e => e.IsDeleted == false);

                if (dto.PackPN != "")
                {
                    query = query.Where(e => e.PackPN == dto.PackPN);
                }

                if (dto.EndTime != null)
                {
                    query = query.Where(o => o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
                }

                if (dto.BeginTime != null)
                {
                    query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
                }
                //总数
                result.Count = await query.CountAsync();

                //列表
                var list = await query
                    .OrderByDescending(e => e.Id)
                    .Skip((dto.Page - 1) * dto.Limit)
                    .Take(dto.Limit)
                    .ToListAsync();

                result.Data = list;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 充气数据导出
        /// </summary>
        /// <param name="pack码"></param>
        /// <returns></returns>
        public async Task<(Boolean, string)> ExportGluingInfo(LeakInfoDTO dto)
        {
            //文件路径
            string filepath = Directory.GetCurrentDirectory() + @"\充气数据导出.xls";
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
                List<GulingInfoToExcel> gulingExcellist = new();
                //获取Pack加压数据
                var gluingDatas = await GetPageListExport(dto);
                if (gluingDatas == null)
                {
                    return (false, $"找不到Pack={dto.PackPN}的数据");
                }
                foreach (var gulingNeedToExport in gluingDatas.Data)
                {
                    gulingExcellist.Add(
                        new GulingInfoToExcel
                        {
                            PackPN = gulingNeedToExport.PackPN,
                            ParameterName = gulingNeedToExport.ParamName,
                            UpMesCodePN = gulingNeedToExport.UploadCode,
                            UpValue = gulingNeedToExport.ParamValue,
                            CreateTime = gulingNeedToExport.CreateTime.ToString(),
                        }
                    );
                }
                //转换list为Excel 
                ExcelPackage excelPackage = new ExcelPackage();
                excelPackage = ExcelToEntity.ListToExcek<GulingInfoToExcel>(excelPackage, "充气数据导出", 2, gulingExcellist);
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

        /// <summary>
        /// 导出专用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Response<List<Proc_StationTask_LeakDetail>>> GetPageListExport(LeakInfoDTO dto)
        {
            var result = new Response<List<Proc_StationTask_LeakDetail>>();
            try
            {
                var query = _dbContext.Proc_StationTask_LeakDetails.OrderByDescending(o => o.CreateTime).Where(e => e.IsDeleted == false);

                if (dto.PackPN != "")
                {
                    query = query.Where(e => e.PackPN == dto.PackPN);
                }

                if (dto.EndTime != null)
                {
                    query = query.Where(o => o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
                }

                if (dto.BeginTime != null)
                {
                    query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
                }
                //总数
                result.Count = await query.CountAsync();

                result.Data = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据主记录ID查询详情数据
        /// </summary>
        /// <param name="mainId">主记录ID</param>
        /// <returns></returns>
        public async Task<Response<List<Proc_StationTask_LeakDetail>>> LoadDetailByMainId(int mainId)
        {
            var result = new Response<List<Proc_StationTask_LeakDetail>>();
            try
            {
                var query = _dbContext.Proc_StationTask_LeakDetails
                    .Where(e => e.IsDeleted == false && e.Proc_StationTask_LeakId == mainId)
                    .OrderByDescending(o => o.CreateTime);

                result.Data = await query.ToListAsync();
                result.Count = result.Data.Count;
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