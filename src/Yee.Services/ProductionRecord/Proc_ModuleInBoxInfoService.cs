using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Tools;

namespace Yee.Services.ProductionRecord
{
    public class Proc_ModuleInBoxInfoService
    {
        private readonly AsZeroDbContext _dbContext;

        public Proc_ModuleInBoxInfoService(AsZeroDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Response<List<Proc_ModuleInBox_DataCollect>>> GetPageList(ModuleInBoxInfosDto dto)
        {
            var result = new Response<List<Proc_ModuleInBox_DataCollect>>();
            try
            {
                var query = _dbContext.Proc_ModuleInBox_DataCollects.OrderByDescending(o => o.CreateTime).Where(e => e.IsDeleted == false);
                if (dto.PackPN != "")
                {
                    query = query.Where(e => e.PackCode == dto.PackPN);

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
        /// Block入箱数据导出
        /// </summary>
        /// <returns></returns>
        public async Task<(Boolean, string)> ExportGluingInfo(ModuleInBoxInfosDto dto)
        {
            //文件路径
            string filepath = Directory.GetCurrentDirectory() + @"\Block入箱数据导出.xls";
            //判断文件路径是否存在 存在就删除
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
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
                    return (false, $"找不到={dto.PackPN}的数据");
                }
                foreach (var gulingNeedToExport in gluingDatas.Data)
                {
                    gulingExcellist.Add(
                        new GulingInfoToExcel
                        {
                            PackPN = gulingNeedToExport.PackPN,
                            ParameterName = gulingNeedToExport.ParameterName,
                            UpMesCodePN = gulingNeedToExport.UpMesCodePN,
                            UpValue = gulingNeedToExport.UpValue,
                            CreateTime = gulingNeedToExport.CreateTime.ToString(),
                        }
                    );
                }
                //转换list为Excel 
                ExcelPackage excelPackage = new ExcelPackage();
                excelPackage = ExcelToEntity.ListToExcek<GulingInfoToExcel>(excelPackage, "Block入箱数据导出", 2, gulingExcellist);
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
        public async Task<Response<List<Proc_ModuleInBox_DataCollect>>> GetPageListExport(ModuleInBoxInfosDto dto)
        {
            var result = new Response<List<Proc_ModuleInBox_DataCollect>>();
            try
            {
                var query = _dbContext.Proc_ModuleInBox_DataCollects.OrderByDescending(o => o.CreateTime).Where(e => e.IsDeleted == false);
                if (dto.PackPN != "")
                {
                    query = query.Where(e => e.PackCode == dto.PackPN);

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
    }
}
