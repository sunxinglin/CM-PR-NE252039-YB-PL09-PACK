using System.Globalization;

using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Itminus.FSharpExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;

using Newtonsoft.Json;

using OfficeOpenXml;

using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;

namespace Yee.Services.AutomaticStation;

public class ExternalAutoTightenDataService
{
    private readonly AsZeroDbContext _dbContext;

    public ExternalAutoTightenDataService(AsZeroDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> UploadExternalData(AutoTighteningDataDto dto)
    {
        try
        {
            # region 数据校验与解析
            if (string.IsNullOrWhiteSpace(dto.SFC))
            {
                return new ServiceErrResponse()
                    .ToError(ResponseErrorType.数据异常, 400, "SFC为空")
                    .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }

            if (string.IsNullOrWhiteSpace(dto.StationName))
            {
                return new ServiceErrResponse()
                    .ToError(ResponseErrorType.数据异常, 400, "StationName为空")
                    .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }

            if (dto.TighteningResultList == null || dto.TighteningResultList.Count == 0)
            {
                return new ServiceErrResponse()
                    .ToError(ResponseErrorType.数据异常, 400, "TighteningResultList为空")
                    .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }

            var tightenType = ResolveTightenType(dto.StationName);
            if (tightenType == null)
            {
                return new ServiceErrResponse()
                    .ToError(ResponseErrorType.数据异常, 400, $"无法识别StationName={dto.StationName}")
                    .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
            
            var torquePrefix = dto.TighteningResultList[0].TorqueResult?.MesName?.Trim() ?? string.Empty;
            var anglePrefix = dto.TighteningResultList[0].AngleResult?.MesName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(torquePrefix) || string.IsNullOrWhiteSpace(anglePrefix))
            {
                return new ServiceErrResponse()
                    .ToError(ResponseErrorType.数据异常, 400, "MesName为空")
                    .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
            
            var orderNoSet = new HashSet<short>();
            foreach (var item in dto.TighteningResultList)
            {
                if (item.OrderNo <= 0)
                {
                    return new ServiceErrResponse()
                        .ToError(ResponseErrorType.数据异常, 400, $"OrderNo无效={item.OrderNo}")
                        .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                if (!orderNoSet.Add(item.OrderNo))
                {
                    return new ServiceErrResponse()
                        .ToError(ResponseErrorType.数据异常, 400, $"OrderNo重复={item.OrderNo}")
                        .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                if (!TryParseDecimal(item.TorqueResult?.TagValue, out _))
                {
                    return new ServiceErrResponse()
                        .ToError(ResponseErrorType.数据异常, 400, $"Torque TagValue无法解析, OrderNo={item.OrderNo}")
                        .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                if (!TryParseDecimal(item.AngleResult?.TagValue, out _))
                {
                    return new ServiceErrResponse()
                        .ToError(ResponseErrorType.数据异常, 400, $"Angle TagValue无法解析, OrderNo={item.OrderNo}")
                        .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
            }

            var sfc = dto.SFC.Trim();
            var stationName = dto.StationName.Trim();
            
            #endregion

            #region 存储数据到本地
            
            var existing = await _dbContext.Proc_ExternalAutoTightenDatas.FirstOrDefaultAsync(e =>
                !e.IsDeleted && e.Sfc == sfc && e.StationName == stationName);

            if (existing == null)
            {
                existing = new Proc_ExternalAutoTightenData
                {
                    Sfc = sfc,
                    StationName = stationName,
                    TightenType = tightenType.Value,
                    TighteningResultJson = JsonConvert.SerializeObject(dto.TighteningResultList),
                };
                await _dbContext.AddAsync(existing);
            }
            else
            {
                existing.TightenType = tightenType.Value;
                existing.TighteningResultJson = JsonConvert.SerializeObject(dto.TighteningResultList);
                existing.UpdateTime = DateTime.Now;
                _dbContext.Update(existing);
            }

            await _dbContext.SaveChangesAsync();
            
            #endregion
            
            return new ServiceErrResponse().ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
        }
        catch (Exception ex)
        {
            return new ServiceErrResponse()
                .ToError(ResponseErrorType.上位机错误, 500, ex.Message)
                .ToErrResult<ServiceErrResponse, ServiceErrResponse>();
        }
    }

    public async Task<Response<IList<ExternalAutoTightenDataListItemDto>>> LoadExternalAutoTightenData(AutoTightenInfoDto dto)
    {
        var resp = new Response<IList<ExternalAutoTightenDataListItemDto>>();
        try
        {
            var query = _dbContext.Proc_ExternalAutoTightenDatas.Where(w => !w.IsDeleted);

            if (!string.IsNullOrWhiteSpace(dto.PackCode))
            {
                var pack = dto.PackCode.Trim();
                query = query.Where(w => w.Sfc.Contains(pack));
            }

            var tightenType = MapTightenType(dto.TightenType);
            if (tightenType != null)
            {
                query = query.Where(w => w.TightenType == tightenType.Value);
            }

            if (dto.BeginTime != null)
            {
                var begin = dto.BeginTime.Value.Date;
                query = query.Where(o => o.CreateTime!.Value.Date >= begin);
            }

            if (dto.EndTime != null)
            {
                var end = dto.EndTime.Value.Date;
                query = query.Where(o => o.CreateTime!.Value.Date <= end);
            }

            resp.Count = await query.CountAsync();

            var datas = await query
                .OrderByDescending(o => o.UpdateTime ?? o.CreateTime)
                .Skip(dto.Limit * (dto.Page - 1))
                .Take(dto.Limit)
                .ToListAsync();

            resp.Result = datas.Select(ToListItem).ToList();
        }
        catch (Exception ex)
        {
            resp.Code = 500;
            resp.Message = ex.Message;
        }

        return resp;
    }

    public async Task<Response<IList<TighteningResult>>> LoadExternalAutoTightenDataDetail(int dataId)
    {
        var resp = new Response<IList<TighteningResult>>();
        try
        {
            var data = await _dbContext.Proc_ExternalAutoTightenDatas.FirstOrDefaultAsync(w => !w.IsDeleted && w.Id == dataId);
            if (data == null)
            {
                resp.Code = 500;
                resp.Message = "记录未找到";
                return resp;
            }

            var list = JsonConvert.DeserializeObject<List<TighteningResult>>(data.TighteningResultJson) ?? new List<TighteningResult>();
            resp.Result = list
                .OrderBy(o => o.OrderNo)
                .ThenBy(o => o.Index)
                .ToList();
        }
        catch (Exception ex)
        {
            resp.Code = 500;
            resp.Message = ex.Message;
        }
        return resp;
    }

    public async Task<(bool Ok, string FilePathOrError)> ExportExternalAutoTightenData(AutoTightenInfoDto dto)
    {
        try
        {
            var query = _dbContext.Proc_ExternalAutoTightenDatas.Where(w => !w.IsDeleted);

            if (!string.IsNullOrWhiteSpace(dto.PackCode))
            {
                var pack = dto.PackCode.Trim();
                query = query.Where(w => w.Sfc.Contains(pack));
            }

            var tightenType = MapTightenType(dto.TightenType);
            if (tightenType != null)
            {
                query = query.Where(w => w.TightenType == tightenType.Value);
            }

            if (dto.BeginTime != null)
            {
                var begin = dto.BeginTime.Value.Date;
                query = query.Where(o => o.CreateTime!.Value.Date >= begin);
            }

            if (dto.EndTime != null)
            {
                var end = dto.EndTime.Value.Date;
                query = query.Where(o => o.CreateTime!.Value.Date <= end);
            }

            var datas = await query
                .OrderByDescending(o => o.UpdateTime ?? o.CreateTime)
                .ToListAsync();

            if (datas.Count == 0)
            {
                return (false, "没有找到可导出的数据");
            }

            var exportDir = Path.Combine(Directory.GetCurrentDirectory(), "ExportExcel");
            Directory.CreateDirectory(exportDir);

            var filePath = Path.Combine(exportDir, $"新自动拧紧数据导出{DateTime.Now:yyyyMMddHHmmss.fff}.xlsx");

            var rows = new List<ExternalAutoTightenExportRow>();
            foreach (var data in datas)
            {
                var list = JsonConvert.DeserializeObject<List<TighteningResult>>(data.TighteningResultJson) ?? new List<TighteningResult>();
                foreach (var item in list.OrderBy(o => o.OrderNo).ThenBy(o => o.Index))
                {
                    rows.Add(new ExternalAutoTightenExportRow
                    {
                        DataId = data.Id,
                        Sfc = data.Sfc,
                        StationName = data.StationName,
                        TightenType = data.TightenType.ToString(),
                        CreateTime = data.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateTime = data.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                        Index = item.Index,
                        OrderNo = item.OrderNo,
                        ProgramNo = item.ProgramNo,
                        ResultOK = item.ResultOK,
                        TorqueTagName = item.TorqueResult?.TagName,
                        TorqueTagValue = item.TorqueResult?.TagValue,
                        TorqueMesName = item.TorqueResult?.MesName,
                        AngleTagName = item.AngleResult?.TagName,
                        AngleTagValue = item.AngleResult?.TagValue,
                        AngleMesName = item.AngleResult?.MesName,
                    });
                }
            }

            await using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using var excelPackage = new ExcelPackage();
                var sheet = excelPackage.Workbook.Worksheets.Add("新自动拧紧数据");
                sheet.Cells.LoadFromCollection(rows, true);
                excelPackage.SaveAs(stream);
            }
            return (true, filePath);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Ok, string FilePathOrError)> ExportExternalAutoTightenDataDetail(int dataId)
    {
        try
        {
            var data = await _dbContext.Proc_ExternalAutoTightenDatas.FirstOrDefaultAsync(w => !w.IsDeleted && w.Id == dataId);
            if (data == null)
            {
                return (false, "记录未找到");
            }

            var exportDir = Path.Combine(Directory.GetCurrentDirectory(), "ExportExcel");
            Directory.CreateDirectory(exportDir);
            var filePath = Path.Combine(exportDir, $"新自动拧紧数据明细导出{DateTime.Now:yyyyMMddHHmmss.fff}.xlsx");

            var rows = new List<ExternalAutoTightenExportRow>();
            var list = JsonConvert.DeserializeObject<List<TighteningResult>>(data.TighteningResultJson) ?? new List<TighteningResult>();
            foreach (var item in list.OrderBy(o => o.OrderNo).ThenBy(o => o.Index))
            {
                rows.Add(new ExternalAutoTightenExportRow
                {
                    DataId = data.Id,
                    Sfc = data.Sfc,
                    StationName = data.StationName,
                    TightenType = data.TightenType.ToString(),
                    CreateTime = data.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdateTime = data.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Index = item.Index,
                    OrderNo = item.OrderNo,
                    ProgramNo = item.ProgramNo,
                    ResultOK = item.ResultOK,
                    TorqueTagName = item.TorqueResult?.TagName,
                    TorqueTagValue = item.TorqueResult?.TagValue,
                    TorqueMesName = item.TorqueResult?.MesName,
                    AngleTagName = item.AngleResult?.TagName,
                    AngleTagValue = item.AngleResult?.TagValue,
                    AngleMesName = item.AngleResult?.MesName,
                });
            }
            
            await using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using var excelPackage = new ExcelPackage();
                var sheet = excelPackage.Workbook.Worksheets.Add("新自动拧紧明细");
                sheet.Cells.LoadFromCollection(rows, true);
                excelPackage.SaveAs(stream);
                return (true, filePath);    
            }
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private ExternalAutoTightenDataListItemDto ToListItem(Proc_ExternalAutoTightenData data)
    {
        var dto = new ExternalAutoTightenDataListItemDto
        {
            Id = data.Id,
            Sfc = data.Sfc,
            StationName = data.StationName,
            TightenType = (int)data.TightenType,
            CreateTime = data.CreateTime,
            UpdateTime = data.UpdateTime,
        };

        var list = JsonConvert.DeserializeObject<List<TighteningResult>>(data.TighteningResultJson) ?? new List<TighteningResult>();
        if (list.Count == 0)
        {
            return dto;
        }

        var latestByOrderNo = list
            .GroupBy(x => x.OrderNo)
            .Select(g => g.OrderByDescending(x => x.Index).First())
            .ToList();

        dto.TotalCount = latestByOrderNo.Count;
        dto.OkCount = latestByOrderNo.Count(x => x.ResultOK == 1);
        dto.NgCount = latestByOrderNo.Count(x => x.ResultOK != 1);
        return dto;
    }

    private static TightenReworkType? MapTightenType(int? tightenType)
    {
        if (tightenType == null)
        {
            return null;
        }

        return tightenType switch
        {
            0 => TightenReworkType.Module,
            1 => TightenReworkType.Lid,
            2 => TightenReworkType.Budding,
            _ => null
        };
    }

    private sealed class ExternalAutoTightenExportRow
    {
        public int DataId { get; set; }
        public string? Sfc { get; set; }
        public string? StationName { get; set; }
        public string? TightenType { get; set; }
        public string? CreateTime { get; set; }
        public string? UpdateTime { get; set; }

        public short Index { get; set; }
        public short ResultOK { get; set; }
        public short OrderNo { get; set; }
        public short ProgramNo { get; set; }

        public string? TorqueTagName { get; set; }
        public string? TorqueTagValue { get; set; }
        public string? TorqueMesName { get; set; }

        public string? AngleTagName { get; set; }
        public string? AngleTagValue { get; set; }
        public string? AngleMesName { get; set; }
    }

    /// <summary>
    /// 解析自动拧紧的类型
    /// </summary>
    /// <param name="stationName"></param>
    /// <returns></returns>
    private static TightenReworkType? ResolveTightenType(string stationName)
    {
        if (stationName.Contains("压条", StringComparison.OrdinalIgnoreCase))
        {
            return TightenReworkType.Budding;
        }

        if (stationName.Contains("上盖", StringComparison.OrdinalIgnoreCase))
        {
            return TightenReworkType.Lid;
        }

        if (stationName.Contains("模组", StringComparison.OrdinalIgnoreCase))
        {
            return TightenReworkType.Module;
        }

        return null;
    }

    /// <summary>
    /// 尝试将字符串解析为数字
    /// </summary>
    /// <param name="raw">要解析的原始字符串</param>
    /// <param name="value">解析后的数字</param>
    /// <returns>如果解析成功，则为 true；否则为 false</returns>
    private static bool TryParseDecimal(string? raw, out decimal value)
    {
        value = 0m;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var trimmed = raw.Trim();
        if (decimal.TryParse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }

        return decimal.TryParse(trimmed, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
    }
}

