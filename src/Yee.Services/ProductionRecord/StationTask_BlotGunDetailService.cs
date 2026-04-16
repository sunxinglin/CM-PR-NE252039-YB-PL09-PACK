using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using Yee.Common.Library.Extensions;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;

namespace Yee.Services.ProductionRecord
{
    public class StationTask_BlotGunDetailService
    {
        private readonly AsZeroDbContext dbContext;

        public StationTask_BlotGunDetailService(AsZeroDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<(List<Proc_StationTask_BlotGunDetail> Items, int TotalCount)> GetBlotGunDetail(BlotGunDetailDto dto)
        {
            var query = dbContext.Proc_StationTask_BlotGunDetails.Where(o => !o.IsDeleted);

            if (!string.IsNullOrWhiteSpace(dto.PackPN))
                query = query.Where(o => o.PackPN == dto.PackPN);

            if (!string.IsNullOrWhiteSpace(dto.StationCode))
            {
                var stationId = await dbContext.Base_Stations
                    .Where(o => !o.IsDeleted && o.Code == dto.StationCode)
                    .Select(o => o.Id)
                    .FirstOrDefaultAsync();

                query = query.Where(o => o.StationId == stationId);
            }

            query = dto.ResultIsOK switch
            {
                1 => query.Where(o => !o.ResultIsOK),
                2 => query.Where(o => o.ResultIsOK),
                _ => query
            };

            if (dto.BeginTime.HasValue)
            {
                var beginDate = dto.BeginTime.Value.Date;
                query = query.Where(o => o.CreateTime >= beginDate);
            }

            if (dto.EndTime.HasValue)
            {
                var endDate = dto.EndTime.Value.Date.AddDays(1);
                query = query.Where(o => o.CreateTime < endDate);
            }

            var totalCount = await query.CountAsync();

            query = query
                .OrderByDescending(o => o.CreateTime)
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit);

            query = query
                .Include(o => o.CreateUser)
                .Include(o => o.UpdateUser)
                .Include(o => o.DeleteUser)
                .Include(o => o.Base_ProResource)
                .Include(o => o.Proc_StationTask_BlotGun.StationTask_Record.Proc_StationTask_Main.Station)
                .Include(o => o.Proc_StationTask_BlotGun.StationTask_Record.Proc_StationTask_Main.Step);

            var items = await query.ToListAsync();
            return (Items: items, TotalCount: totalCount);
        }

        /// <summary>
        /// 人工数据查询 不分页
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<Proc_StationTask_BlotGunDetail>> GetBlotGunDetails(BlotGunDetailDto dto)
        {
            var query = dbContext.Proc_StationTask_BlotGunDetails.Where(o => !o.IsDeleted);

            if (!string.IsNullOrWhiteSpace(dto.PackPN))
                query = query.Where(o => o.PackPN == dto.PackPN);

            if (!string.IsNullOrWhiteSpace(dto.StationCode))
            {
                var stationId = await dbContext.Base_Stations
                    .Where(o => !o.IsDeleted && o.Code == dto.StationCode)
                    .Select(o => o.Id)
                    .FirstOrDefaultAsync();

                query = query.Where(o => o.StationId == stationId);
            }

            query = dto.ResultIsOK switch
            {
                1 => query.Where(o => !o.ResultIsOK),
                2 => query.Where(o => o.ResultIsOK),
                _ => query
            };

            if (dto.BeginTime.HasValue)
            {
                var beginDate = dto.BeginTime.Value.Date;
                query = query.Where(o => o.CreateTime >= beginDate);
            }

            if (dto.EndTime.HasValue)
            {
                var endDate = dto.EndTime.Value.Date.AddDays(1);
                query = query.Where(o => o.CreateTime < endDate);
            }

            query = query
                .Include(o => o.CreateUser)
                .Include(o => o.UpdateUser)
                .Include(o => o.DeleteUser)
                .Include(o => o.Base_ProResource)
                .Include(o => o.Proc_StationTask_BlotGun.StationTask_Record.Proc_StationTask_Main.Station)
                .Include(o => o.Proc_StationTask_BlotGun.StationTask_Record.Proc_StationTask_Main.Step);
            var result = await query.ToListAsync();

            return result;
        }
        /// <summary>
        /// 获取自动拧紧数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<(List<Proc_AutoBoltInfo_Detail>, int)> GetAutoBlotGunDetail(BlotGunDetailDto dto)
        {
            int totalCount = 0;

            var query = this.dbContext.Proc_AutoBoltInfo_Details.Where(o => !o.IsDeleted);
            if (dto.PackPN != null && dto.PackPN != string.Empty)
            {
                query = query.Where(o => o.PackPN == dto.PackPN);
            }
            if (dto.EndTime != null)
            {
                query = query.Where(o => o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
            }

            if (dto.BeginTime != null)
            {
                query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
            }
            //if (dto.ResultIsOK==1)
            //{
            //    query = query.Where(o => o.ResultIsOK);
            //}
            //else if(dto.ResultIsOK==0)
            //{
            //    query = query.Where(o => !o.ResultIsOK);

            //}
            totalCount = query.Count();
            query = query.Skip((dto.Page - 1) * dto.Limit).Take(dto.Limit);

            query = query.Include(o => o.CreateUser).Include(o => o.UpdateUser).Include(o => o.DeleteUser).OrderByDescending(o => o.CreateTime);
            query = query.Include(o => o.Proc_StationTask_Main.Station)
                .Include(o => o.Proc_StationTask_Main.Step);
            var result = await query.ToListAsync();
            return (result, totalCount);

        }
        /// <summary>
        /// 查询自动拧紧数据 不分页
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<Proc_AutoBoltInfo_Detail>> GetAutoBlotGunDetails(BlotGunDetailDto dto)
        {

            var query = this.dbContext.Proc_AutoBoltInfo_Details.Where(o => !o.IsDeleted);
            if (dto.PackPN != null && dto.PackPN != string.Empty)
            {
                query = query.Where(o => o.PackPN == dto.PackPN);
            }
            if (dto.EndTime != null)
            {
                query = query.Where(o => o.CreateTime <= dto.EndTime);
            }

            if (dto.BeginTime != null)
            {
                query = query.Where(o => o.CreateTime >= dto.BeginTime);
            }
            //if (dto.ResultIsOK == 1)
            //{
            //    query = query.Where(o => o.ResultIsOK);
            //}
            //else if (dto.ResultIsOK == 0)
            //{
            //    query = query.Where(o => !o.ResultIsOK);

            //}


            query = query.Include(o => o.CreateUser).Include(o => o.UpdateUser).Include(o => o.DeleteUser).OrderByDescending(o => o.CreateTime);
            query = query.Include(o => o.Proc_StationTask_Main.Station)
                .Include(o => o.Proc_StationTask_Main.Step);
            var result = await query.ToListAsync();
            return result;

        }

        private IQueryable<Proc_AutoBoltInfo_Detail> BuildAutoBoltDetailsQuery(BlotGunDetailDto dto, bool includeUser = false)
        {

            var query = this.dbContext.Proc_AutoBoltInfo_Details.Where(o => !o.IsDeleted);
            if (dto.PackPN != null && dto.PackPN != string.Empty)
            {
                query = query.Where(o => o.PackPN == dto.PackPN);
            }
            if (dto.EndTime != null)
            {
                query = query.Where(o => o.CreateTime.Value.Date <= dto.EndTime.Value.Date);
            }

            if (dto.BeginTime != null)
            {
                query = query.Where(o => o.CreateTime.Value.Date >= dto.BeginTime.Value.Date);
            }

            if (includeUser)
            {
                query = query.Include(o => o.CreateUser).Include(o => o.UpdateUser).Include(o => o.DeleteUser);
            }

            query = query.OrderByDescending(o => o.CreateTime);
            query = query.Include(o => o.Proc_StationTask_Main.Station).Include(o => o.Proc_StationTask_Main.Step);
            return query;
        }

        /// <summary>
        /// 自动螺丝数据导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<(Boolean, string)> ExportAutoBlotGunInfo(BlotGunDetailDto dto)
        {
            //文件路径
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "ExportExcel", $@"自动拧紧数据导出{DateTime.Now:yyyyMMddHHmmdd.fff}.xlsx");
            //判断文件路径是否存在 存在就删除
            DeleteExcelFile("自动拧紧数据导出");
            //创建文件
            try
            {
                //获取自动拧紧数据
                var autoBlotGunDatas = BuildAutoBoltDetailsQuery(dto);
                if (autoBlotGunDatas == null || !autoBlotGunDatas.Any())
                {
                    return (false, $"找不到Pack={dto.PackPN}的数据");
                }
                // 创建 list
                var list = (await autoBlotGunDatas.Select(data => new { data.AutoBlotInfoArray, data.PackPN, data.UploadCode, data.UploadCode_JD, data.CreateTime }).ToListAsync())
                    .Where(data => data.AutoBlotInfoArray != null)
                    .Select(data => data.AutoBlotInfoArray!.Select(bolt => new AutoBlotGunInfoToExcel
                    {
                        PackPN = data.PackPN,
                        OrderNO = bolt.OrderNo.ToString(),
                        ProgramNO = bolt.ProgramNo.ToString(),
                        UploadCode = data.UploadCode,
                        UploadCode_JD = data.UploadCode_JD,
                        ResultIsOk = bolt.ResultIsOK ? "OK" : "NG",
                        FinalAngle = bolt.FinalAngle.ToString(),
                        FinalTorque = bolt.FinalTorque.ToString(),
                        CreateTime = data.CreateTime.ToString(),
                    }))
                    .Flat();

                //转换list为Excel 
                using var stream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                ExcelPackage excelPackage = new();
                //excelPackage = ExcelToEntity.ListToExcek(excelPackage, "自动拧紧数据导出", 1, list);
                var sheet = excelPackage.Workbook.Worksheets.Add("自动拧紧数据导出");
                sheet.Cells.LoadFromCollection(list, true);
                excelPackage.SaveAs(stream);
            }
            catch (Exception ex)
            {
                return (false, $"导出出现错误{ex.Message}");
            }
            return (true, Path.GetFileName(filepath));
        }

        /// <summary>
        /// 人工拧紧数据导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<(Boolean, string)> ExportBlotGunInfo(BlotGunDetailDto dto)
        {
            //文件路径
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "ExportExcel", $@"人工拧紧数据导出{DateTime.Now:yyyyMMddHHmmdd.fff}.xlsx");
            //判断文件路径是否存在 存在就删除
            DeleteExcelFile("人工拧紧数据导出");
            //创建文件
            try
            {
                //创建list
                List<BlotGunInfoToExcel> blotGunExcellist = new();
                //获取人工拧紧数据
                var blotGunDatas = await GetBlotGunDetails(dto);

                if (blotGunDatas == null)
                {
                    return (false, $"找不到Pack={dto.PackPN}的数据");
                }

                var list = blotGunDatas.Select(data =>
                {
                    return new BlotGunInfoToExcel
                    {
                        PackPN = data.PackPN,
                        StationName = data.Proc_StationTask_BlotGun?.StationTask_Record?.Proc_StationTask_Main?.Station?.Code,//this.dbContext.Base_Stations.Where(x => x.Id == data.StationId).Select(x => x.Code).FirstOrDefault(),
                        StepName = data.Proc_StationTask_BlotGun?.StationTask_Record?.Proc_StationTask_Main?.Step?.Code,//this.dbContext.Base_Steps.Where(x => x.Id == data.StepId).Select(x => x.Name).FirstOrDefault(),
                        ResultIsOk = data.ResultIsOK ? "OK" : "NG",
                        ProgramNO = data.ProgramNo.ToString(),
                        UploadCode = data.UploadCode,
                        UploadCode_JD = data.UploadCode_JD,
                        FinalAngle = data.FinalAngle.ToString(),
                        FinalTorque = data.FinalTorque.ToString(),
                        ScrewName = data.ScrewName,
                        Base_ProResource = data.DeviceNo.ToString(),
                        CreateUser = data.CreateUser?.Name,
                        CreateDate = data.CreateTime?.ToString(),
                    };
                });

                //转换list为Excel 
                ExcelPackage excelPackage = new ExcelPackage();
                //excelPackage = ExcelToEntity.ListToExcek<BlotGunInfoToExcel>(excelPackage, "人工拧紧数据导出", 1, list);
                var sheet = excelPackage.Workbook.Worksheets.Add("人工拧紧数据导出");
                sheet.Cells.LoadFromCollection(list, true);

                using var stream = new FileStream(filepath, FileMode.OpenOrCreate);
                excelPackage.SaveAs(stream);
            }
            catch (Exception ex)
            {
                return (false, $"导出出现错误{ex.Message}");
            }
            return (true, Path.GetFileName(filepath));
        }

        private void DeleteExcelFile(string prefix)
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "ExportExcel");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            foreach (var file in Directory.GetFiles(directoryPath, $"{prefix}*.xlsx"))
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    var fileTimeSpan = (DateTime.Now - fileInfo.LastWriteTime);
                    if (fileTimeSpan.TotalDays >= 1 || fileTimeSpan.TotalDays < 0)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public async Task<List<Proc_StationTask_BlotGunDetail>> LoadBoltData(int stepid, int stationtaskid, string packCode)
        {
            var main = await this.dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(o => o.StepId == stepid && o.PackCode == packCode && o.IsDeleted == false);
            if (main != null)
            {
                var record = await this.dbContext.Proc_StationTask_Records.FirstOrDefaultAsync(o => o.Proc_StationTask_MainId == main.Id && o.Base_StationTaskId == stationtaskid && o.IsDeleted == false);

                if (record != null)
                {
                    var blot = await this.dbContext.Proc_StationTask_BlotGuns.FirstOrDefaultAsync(o => o.StationTask_RecordId == record.Id && o.IsDeleted == false);

                    if (blot != null)
                    {
                        var blotdetail = await this.dbContext.Proc_StationTask_BlotGunDetails.Where(o => o.IsDeleted == false && o.PackPN == packCode && o.Proc_StationTask_BlotGunId == blot.Id && o.StepId == stepid && o.ResultIsOK).Distinct().ToListAsync();
                        return blotdetail;
                    }
                    else { return null; }
                }
                else { return null; }
            }
            else { return null; }


        }
    }
}
