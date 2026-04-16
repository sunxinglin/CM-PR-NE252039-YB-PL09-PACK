using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Tools;

namespace Yee.Services.ProductionRecord
{
    public class StationTask_GlueDetailService
    {
        private readonly AsZeroDbContext dbContext;

        public StationTask_GlueDetailService(AsZeroDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<(List<Proc_GluingInfo>,int)> GetGlueDetail(GlueDetailDto dto)
        {
            int totalCount = 0;

            //倒叙查询涂胶数据
            var query = this.dbContext.Proc_GluingInfos.OrderByDescending(o => o.CreateTime).Where(o => !o.IsDeleted);
            //根据输入的pack码找箱体码
            //var OuterGoodsCode = this.dbContext.Proc_Join_PackAndOuterCodes.Where(x => x.PackCode == dto.PackPN).Select(x => x.OuterGoodsCode).FirstOrDefault();
            //if (OuterGoodsCode != null)//如果不为空，那么可以确认输入的是pack码
            //{//找到了
            //    query = query.Where(o => o.PackPN == OuterGoodsCode);//根据下箱体码筛选涂胶数据
            //    foreach (var item in query)
            //    {
            //        item.PackPN = dto.PackPN;//把查到的箱体码替换成pack码
            //    }
            //}
            //else
            //{//否则 确定输入的内容是箱体码或其他
            //    if (dto.PackPN != null && dto.PackPN != string.Empty)//判断输入的内容不为空
            //    {
            //        query = query.Where(o => o.PackPN == dto.PackPN);//根据箱体码筛选涂胶数据
            //        if (query != null)//如果不为空 可以确定输入的是箱体码
            //        {
            //            var packs = this.dbContext.Proc_Join_PackAndOuterCodes.Where(x => x.OuterGoodsCode == dto.PackPN).Select(x => x.PackCode).FirstOrDefault();//根据箱体码查找pack码
            //            if (packs != null)
            //            {
            //                foreach (var item in query)
            //                {
            //                    item.PackPN = packs;//把查到的箱体码替换成pack码
            //                }
            //            }
            //        }
            //    }
            //}

            if (dto.EndTime != null)
            {
                query = query.Where(o => o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
            }

            if (dto.BeginTime != null)
            {
                query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
            }

            totalCount = query.Count();
            query = query.Skip((dto.Page - 1) * dto.Limit).Take(dto.Limit);

            var result = await query.ToListAsync();
            return (result, totalCount);

        }


        public async Task<List<Proc_GluingInfo>> GetRealTimeGlueDetail(GlueDetailDto dto)
        {
            //倒叙查询涂胶数据
            var query = this.dbContext.Proc_GluingInfos.OrderByDescending(o => o.CreateTime).Where(o => !o.IsDeleted);
            //根据输入的pack码找箱体码
            //var OuterGoodsCode = this.dbContext.Proc_Join_PackAndOuterCodes.Where(x => x.PackCode == dto.PackPN).Select(x => x.OuterGoodsCode).FirstOrDefault();
            //if (OuterGoodsCode != null)//如果不为空，那么可以确认输入的是pack码
            //{//找到了
            //    query = query.Where(o => o.PackPN == OuterGoodsCode);//根据下箱体码筛选涂胶数据
            //    foreach (var item in query)
            //    {
            //        item.PackPN = dto.PackPN;//把查到的箱体码替换成pack码
            //    }
            //}
            //else
            //{//否则 确定输入的内容是箱体码或其他
            //    if (dto.PackPN != null && dto.PackPN != string.Empty)//判断输入的内容不为空
            //    {
            //        query = query.Where(o => o.PackPN == dto.PackPN);//根据箱体码筛选涂胶数据
            //        if (query != null)//如果不为空 可以确定输入的是箱体码
            //        {
            //            var packs = this.dbContext.Proc_Join_PackAndOuterCodes.Where(x => x.OuterGoodsCode == dto.PackPN).Select(x => x.PackCode).FirstOrDefault();//根据箱体码查找pack码
            //            if (packs != null)
            //            {
            //                foreach (var item in query)
            //                {
            //                    item.PackPN = packs;//把查到的箱体码替换成pack码
            //                }
            //            }
            //        }
            //    }
            //}

            if (dto.EndTime != null)
            {
                query = query.Where(o => o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
            }

            if (dto.BeginTime != null)
            {
                query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
            }

            var result = await query.ToListAsync();
            return result;
        }


        /// <summary>
        /// 涂胶导出专用查询
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<Proc_GluingInfo>> GetGlueDetailForExport(string? PackCode)
        {
            //倒叙查询涂胶数据
            var query = this.dbContext.Proc_GluingInfos.OrderByDescending(o => o.CreateTime).Where(o => !o.IsDeleted);
            if (PackCode != null && PackCode != string.Empty)
            {
                query = query.Where(o => o.PackPN == PackCode);
            }
            var result = await query.ToListAsync();
            return result;
        }
        /// <summary>
        /// 删除涂胶数据和涂胶主记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Response<int>> DeleteGluingInfo(GluingInfoDeleteDTO dto)
        {
            var result = new Response<int>();
            try
            {
                //删除涂胶数据
                if (dto.GluingInfoIds?.Length > 0)
                {
                    var list = await dbContext.Proc_GluingInfos.Where(e => dto.GluingInfoIds.Contains(e.Id)).ToListAsync();
                    dbContext.Proc_GluingInfos.RemoveRange(list);
                }
                //删除主记录
                if (dto.MainIds?.Length > 0)
                {
                    var list = await dbContext.Proc_StationTask_Mains.Where(e => dto.MainIds.Contains(e.Id)).ToListAsync();
                    dbContext.Proc_StationTask_Mains.RemoveRange(list);
                }
                //保存
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        /// <summary>
        /// 修改涂胶时间
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Response<int>> UpGluingInfo(UpGluingDto dto)
        {
            var result = new Response<int>();
            try
            {
                if (string.IsNullOrEmpty(dto.packCode)&& string.IsNullOrEmpty(dto.glueTime.ToString()) ) 
                {
                    //if (dto.packCode.Length>24) 
                    //{
                    //    var list = await dbContext.Proc_GluingInfos.Where(e => dto.packCode == e.PackPN && e.IsDeleted == false && e.ParameterName == "涂胶开始时间").FirstOrDefaultAsync();
                    //    if (list != null)
                    //    {
                    //        list.UpValue = dto.glueTime.ToString();
                    //    }
                    //    {
                    //        result.Code = 500;
                    //        result.Message = "查询不到涂胶开始时间";
                    //        return result;
                    //    }
                    //}
                    //if (dto.packCode.Length <= 24)
                    //{
                    //    var packcodelist = await dbContext.Proc_Join_PackAndOuterCodes.Where(e => dto.packCode == e.PackCode && e.IsDeleted == false ).FirstOrDefaultAsync();
                    //    if (packcodelist != null && packcodelist.OuterGoodsCode != null) 
                    //    {
                    //        dto.packCode = packcodelist.OuterGoodsCode;
                    //        var list = await dbContext.Proc_GluingInfos.Where(e => dto.packCode == e.PackPN && e.IsDeleted == false && e.ParameterName == "涂胶开始时间").FirstOrDefaultAsync();
                    //        if (list != null)
                    //        {
                    //            list.UpValue = dto.glueTime.ToString();
                    //        }
                    //        {
                    //            result.Code = 500;
                    //            result.Message = "查询不到涂胶开始时间";
                    //            return result;
                    //        }
                    //    }
                    //    {
                    //        result.Code = 500;
                    //        result.Message = "查询不到pack码对应的下箱体码";
                    //        return result;
                    //    }                    
                    //}
                }
                result.Code = 500;
                result.Message = "下箱体码或涂胶时间为空";
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        /// <summary>
        /// 涂胶数据导出
        /// </summary>
        /// <param name="pack码"></param>
        /// <returns></returns>
        public async Task<(Boolean, string)> ExportGluingInfo(GlueDetailDto dto)
        {
            //文件路径
            string filepath = Directory.GetCurrentDirectory() + @"\涂胶数据导出.xls";
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
                //获取涂胶数据
                var (gluingDatas, count) = await GetGlueDetail(dto);
                if (gluingDatas == null)
                {
                    return (false, $"找不到Pack={dto.PackPN}的数据");
                }
                //foreach (var gulingNeedToExport in gluingDatas)
                //{
                //    gulingExcellist.Add(
                //        new GulingInfoToExcel
                //        {
                //            PackPN = gulingNeedToExport.PackPN,
                //            ParameterName = gulingNeedToExport.ParameterName,
                //            UpMesCodePN = gulingNeedToExport.UpMesCode,
                //            UpValue = gulingNeedToExport.UpValue,
                //            CreateTime = gulingNeedToExport.CreateTime.ToString(),
                //        }
                //    ) ;
                //}
                //转换list为Excel 
                ExcelPackage excelPackage = new ExcelPackage();
                excelPackage = ExcelToEntity.ListToExcek<GulingInfoToExcel>(excelPackage, "涂胶数据导出", 2, gulingExcellist);
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
