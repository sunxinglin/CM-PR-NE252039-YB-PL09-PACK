using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.AutomaticStation;

namespace Yee.WebApi.Controllers.Automatic;

[Route("api/[controller]/[action]")]
[ApiController]
public class AutoTightenController : ControllerBase
{
    private readonly AutoTightenService _service;
    private readonly ExternalAutoTightenDataService _externalService;

    public AutoTightenController(AutoTightenService service, ExternalAutoTightenDataService externalService)
    {
        _service = service;
        _externalService = externalService;
    }

    [HttpPost]
    public async Task<ServiceErrResponse> UploadData(AutoTightenDataUploadDto dto)
    {
        var result = await _service.UploadData(dto);
        return result.IsError ? result.ErrorValue : result.ResultValue;
    }

    /// <summary>
    /// 保存C标准程序框架传来的自动拧紧数据
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ServiceErrResponse> UploadExternalData(AutoTighteningDataDto dto)
    {
        var result = await _externalService.UploadExternalData(dto);
        return result.IsError ? result.ErrorValue : result.ResultValue;
    }

    [HttpGet]
    public async Task<Response<IList<Proc_AutoBoltInfo_Detail>>> LoadAutoTightenData([FromQuery] AutoTightenInfoDto dto)
    {
        return await _service.LoadAutoTightenData(dto);
    }

    [HttpGet]
    public async Task<Response<IList<AutoBlotInfo>>> LoadAutoTightenDataDetail([FromQuery] int dataId)
    {
        return await _service.LoadAutoTightenDataDetail(dataId);
    }

    [HttpGet]
    public async Task<Response<IList<ExternalAutoTightenDataListItemDto>>> LoadExternalAutoTightenData([FromQuery] AutoTightenInfoDto dto)
    {
        return await _externalService.LoadExternalAutoTightenData(dto);
    }

    [HttpGet]
    public async Task<Response<IList<TighteningResult>>> LoadExternalAutoTightenDataDetail([FromQuery] int dataId)
    {
        return await _externalService.LoadExternalAutoTightenDataDetail(dataId);
    }

    [HttpPost]
    public async Task<IActionResult> ExportExternalAutoTightenData([FromBody] AutoTightenInfoDto dto)
    {
        var result = await _externalService.ExportExternalAutoTightenData(dto);
        if (!result.Ok)
        {
            return BadRequest(new { message = result.FilePathOrError }); 
        }

        var fileStream = new FileStream(result.FilePathOrError, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);

        return File(
            fileStream, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"{DateTime.Now:yyyyMMdd}新自动拧紧数据导出.xlsx"
        );
    }

    [HttpGet]
    public async Task<IActionResult> ExportExternalAutoTightenDataDetail([FromQuery(Name = "id")] int dataId)
    {
        var result = await _externalService.ExportExternalAutoTightenDataDetail(dataId);
        if (!result.Ok)
        {
            return BadRequest(new { message = result.FilePathOrError }); 
        }

        var fileStream = new FileStream(result.FilePathOrError, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);

        return File(
            fileStream, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"{DateTime.Now:yyyyMMdd}新自动拧紧数据导出明细.xlsx"
        );
    }

}