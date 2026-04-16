using System.Security.Claims;

using AsZero.Core.Entities;
using AsZero.Core.Services.Auth;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using AsZero.Services.Auth;
using AsZero.WebApi.Shared;

using FutureTech.OpResults;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DTOS;
using Yee.Services.ProductionRecord;
using Yee.Tools;

namespace AsZero.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
//[Authorize]
public class AccountController : ControllerBase
{
    private readonly IUserManager _userMgr;
    private readonly IAuthorizationService _authZService;
    private readonly ILogger<AccountController> _logger;
    private readonly RolesService rolesService;

    private readonly ProcRecordCheckPowerLogService _CheckPowerLogService;
    private readonly SysLogService sysLogService;
    public readonly AsZeroDbContext _dBContext;
    private readonly IPasswordHasher _passwordHasher;

    public AccountController(AsZeroDbContext dBContext, IPasswordHasher passwordHasher, IUserManager userMgr, IAuthorizationService authZService, ILogger<AccountController> logger,
        RolesService rolesService, SysLogService sysLogService, ProcRecordCheckPowerLogService checkPowerLogService)
    {
        this._userMgr = userMgr;
        this._authZService = authZService;
        this._logger = logger;
        this.rolesService = rolesService;
        this._CheckPowerLogService = checkPowerLogService;
        this.sysLogService = sysLogService;
        _dBContext = dBContext;
        this._passwordHasher = passwordHasher;
    }

    //[AllowAnonymous]
    [HttpGet]
    public async Task<Response<User>> CheckAccountCard(string card)
    {
        var result = new Response<User>();
        var user = await this._userMgr.GetUserAsync(card);
        if (user == null)
        {
            result.Code = 500;
            result.Message = "不存在的员工卡";
        }
        else
        {
            result.Code = 200;
            result.Result = user;
        }
        await sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.用户登录, Message = $"用户校验权限:{card},通过", Operator = card });
        // 激活用户
        return result;
    }

    [HttpGet]
    public async Task<Response<User>> CheckAccountCardByWorkId(string card)
    {
        var result = new Response<User>();
        var user = await this._userMgr.GetUserByWorkIdAsync(card);
        if (user == null)
        {
            result.Code = 500;
            result.Message = "不存在的员工卡";
        }
        else
        {
            result.Code = 200;
            result.Result = user;
        }
        await sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.用户登录, Message = $"用户校验权限:{card},通过", Operator = card });
        // 激活用户
        return result;
    }

    [HttpGet]
    public async Task<Response<User>> Check_CreateAccountCardAsync(string card)
    {
        var result = new Response<User>();
        var userCheck = await this._userMgr.Check_CreateAccountCardAsync(card);
        if (!userCheck.Success)
        {
            result.Code = 500;
            result.Message = userCheck.Message;
        }
        else
        {
            result.Code = 200;
            result.Result = userCheck.Data;
        }
        await sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.用户登录, Message = $"用户校验权限:{card},通过", Operator = card });
        // 激活用户
        return result;
    }

    [HttpGet]
    //[Authorize()]
    public async Task<IActionResult> LogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
        var result = new OpResult<bool>();
        result.Success = true;
        result.Message = "";
        result.Data = true;
        return Ok(result);
    }

    //[AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> LoginAsync([FromBody] LoginInput input)
    {
        try
        {
            if (string.IsNullOrEmpty(input.Account))
            {
                this.ModelState.AddModelError(nameof(input.Account), "不可为空");
            }

            if (string.IsNullOrEmpty(input.Password))
            {
                this.ModelState.AddModelError(nameof(input.Password), "不可为空");
            }

            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            var result = await this._userMgr.ValidateUserAsync(input.Account!, input.Password!);

            if (result is NotFoundOpResult<User>)
            {
                result.Message = "用户不存在";
                return Ok(result);
            }

            if (!result.Success)
            {
                return Ok(result);
            }

            var principal = await this._userMgr.LoadPrincipalAsync(input.Account!, false);

            if (principal == null)
            {
                result.Success = false;
                result.Message = "无法加载用户凭证";
                return Ok(result);
            }

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddYears(99),
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            await sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.用户登录, Message = $"用户:{result.Data.Name}，登录成功", Operator = result.Data.Name });
            _dBContext.SaveChanges();
            // 激活用户
            return Ok(new SucceededOpResult<object>(new
            {
                Account = input.Account,
                UserName = principal.Identity?.Name,
            }));
        }
        catch (Exception ex)
        {
            return Ok(new OpResult<object>() { Success = false, Message = ex.Message });
        }
    }


    [AllowAnonymous]
    [HttpPost]
    public async Task<Resp<LoginResultDto>> LoginByCardAsync([FromBody] LoginByCard input)
    {
        if (string.IsNullOrEmpty(input.CardNo))
        {

            return RespExtensions.MakeFieldFail<LoginResultDto>(nameof(input.CardNo), "卡号不可为空");
        }


        var result = await this._userMgr.ValidateCardNoAsync(input.CardNo);

        if (!result.Success)
        {
            return RespExtensions.MakeFail<LoginResultDto>("400", result.Message);
        }

        var account = result.Data.Account;

        var principal = await this._userMgr.LoadPrincipalAsync(account, false);

        if (principal == null)
        {
            return RespExtensions.MakeFail<LoginResultDto>("400", "无法加载用户凭证");
        }

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddYears(99),
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

        return RespExtensions.MakeSuccess(new LoginResultDto
        {
            Status = true,
        });
    }
    [Authorize()]
    [HttpGet]
    public IEnumerable<ClaimEntity> Claims()
    {
        var claims = HttpContext.User.Claims.Select(c => new ClaimEntity { ClaimType = c.Type, ClaimValue = c.Value });
        return claims;
    }
    /// <summary>
    /// 当用户访问邮箱中的重置密码的链接后，先校验用户的Token
    ///    是否过期、
    ///    是否的确和Account关联、
    ///    是否有效、
    ///    是否伪造,
    /// 然后重置用户密码
    /// </summary>
    [HttpPost]
    //[AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordInput input)
    {
        if (string.IsNullOrEmpty(input.Account))
        {
            this.ModelState.AddModelError(nameof(input.Account), "不可为空");
        }

        if (string.IsNullOrEmpty(input.Password))
        {
            this.ModelState.AddModelError(nameof(input.Password), "不可为空");
        }

        if (!this.ModelState.IsValid)
        {
            return BadRequest(this.ModelState);
        }
        if (string.IsNullOrEmpty(token))
        {
            this.ModelState.AddModelError($"{token}", "Token不得为空！");
            this._logger.LogDebug($"当前无Token，无法重置用户密码");
            return ValidationProblem(this.ModelState);
        }
        var flag = this._userMgr.ValidateResetPasswordToken(input.Account!, token);
        if (!flag)
        {
            this.ModelState.AddModelError($"{token}", "当前Token无效！");
            this._logger.LogDebug($"当前Token无效，无法重置用户密码");
            return ValidationProblem(this.ModelState);
        }
        var res = await this._userMgr.ChangePasswordAsync(input.Account!, input.Password!);
        return new JsonResult(res);
    }

    /// <summary>
    /// 只允许Cookie认证，只允许用户自己下载
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> PullAsync()
    {
        var userId = this.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            this.ModelState.AddModelError("NameIdentifier", "NameIdentifier不得为空");
            return BadRequest(this.ModelState);
        }
        var user = await this._userMgr.GetUserAsync(userId);
        return Ok(user);
    }



    /// <summary>
    /// 允许使用 ApiKey 的方式访问 或者 Cookie 的方式访问。
    /// 使用Cookie的方式，只能修改自己的用户密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    //[Authorize(Policy = AuthDefines.Policy_CookieAuthOrApiKeyAuth)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordInput input)
    {
        if (string.IsNullOrEmpty(input.Account))
        {
            this.ModelState.AddModelError(nameof(input.Account), "不可为空");
        }

        if (string.IsNullOrEmpty(input.Password))
        {
            this.ModelState.AddModelError(nameof(input.Password), "不可为空");
        }
        if (!this.ModelState.IsValid)
        {
            return BadRequest(this.ModelState);
        }

        var r = await this._authZService.AuthorizeAsync(User, input.Account, new[] {
            new AdminApiKeyOrSameUserCookieAuthorizationRequirement()
        });
        if (r.Succeeded)
        {
            var res = await this._userMgr.ChangePasswordAsync(input.Account!, input.Password!);
            var result = new
            {
                Success = res.Success,
                Message = res.Message,
                Cipher = res.Data?.Password,
            };
            return Ok(result);
        }
        else
        {
            return Forbid();
        }
    }
    /// <summary>
    /// 允许使用 ApiKey 的方式访问 或者 Cookie 的方式访问。
    /// 使用Cookie的方式，只能修改自己的用户密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ChangeUserPassword(ChangePasswordInput input)
    {
        if (string.IsNullOrEmpty(input.Account))
        {
            this.ModelState.AddModelError(nameof(input.Account), "不可为空");
        }

        if (string.IsNullOrEmpty(input.Password))
        {
            this.ModelState.AddModelError(nameof(input.Password), "不可为空");
        }
        if (!this.ModelState.IsValid)
        {
            return BadRequest(this.ModelState);
        }




        var res = await this._userMgr.ChangePasswordAsync(input.Account!, input.Password!);
        //var result = new
        //{
        //    Success = res.Success,
        //    Message = res.Message,
        //    Cipher = res.Data?.Password,
        //};
        return Ok(res);

    }

    /// <summary>
    /// 只允许使用ApiKey的方式访问
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    [HttpGet]
    //[Authorize(Policy = AuthDefines.Policy_CookieAuthOrApiKeyAuth)]
    public async Task<IActionResult> GetUserAsync([FromQuery] GetUserInput input)
    {
        var user = await this._userMgr.GetUserAsync(input.Account);


        var result = new OpResult<User>();

        if (user == null)
        {
            return NotFound();
        }
        else
        {
            result.Success = true;
            result.Message = "";
            result.Data = user;

            return Ok(result);
        }
    }

    /// <summary>
    /// 只允许使用 ApiKey 的方式访问，且必须为 Admin类型的ApiKey
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    //[Authorize(Policy = AuthDefines.Policy_AdminApiKeyAuth)]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserInput input)
    {
        if (string.IsNullOrEmpty(input.Account))
        {
            this.ModelState.AddModelError(nameof(input.Account), "不可为空");
        }

        if (string.IsNullOrEmpty(input.Password))
        {
            this.ModelState.AddModelError(nameof(input.Password), "不可为空");
        }

        if (string.IsNullOrEmpty(input.UserName))
        {
            this.ModelState.AddModelError(nameof(input.UserName), "不可为空");
        }

        if (!this.ModelState.IsValid)
        {
            return BadRequest(this.ModelState);
        }

        var res = await this._userMgr.CreateUserAsync(input.Account!, input.Password!, input.UserName!, input.WorkId!);
        if (res.Success)
        {
            return CreatedAtAction(nameof(GetUserAsync), new { account = res.Data.Account }, res);
        }
        else
        {
            return BadRequest(res);
        }
    }

    /// <summary>
    /// 加载列表
    /// 获取当前登录用户可访问的一个部门及子部门全部用户
    /// </summary>
    [HttpGet]
    public async Task<TableData> Load([FromQuery] QueryUserListReq request)
    {
        return await this._userMgr.Load(request);
    }

    [HttpGet]
    public async Task<Response<List<User>>> GetList([FromQuery] GetUserListReq request)
    {
        var result = new Response<List<User>>();
        try
        {
            var list = await this._userMgr.GetList(request);
            result.Data = list;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }
        return result;

    }

    [HttpPost]
    //[Authorize(Policy = AuthDefines.Policy_CookieAuthOrApiKeyAuth)]
    public async Task<Response<User>> CreateAccount([FromBody] CreateAccountInput input)
    {
        Response<User> result = new Response<User>();
        try
        {
            if (string.IsNullOrEmpty(input.Account))
            {
                result.Code = 500;
                result.Message = "账号不能为空";
                return result;
            }

            if (string.IsNullOrEmpty(input.Password))
            {
                result.Code = 500;
                result.Message = "密码不可为空";
                return result;

            }

            if (string.IsNullOrEmpty(input.Name))
            {
                result.Code = 500;
                result.Message = "用户名不可为空";
                return result;

            }



            var res = await this._userMgr.CreateUserAsync(input.Account!, input.Password!, input.Name!, input.WorkId!, input.Status);
            if (res.Success)
            {
                result.Result = res.Data;
                return result;
            }
            else
            {
                result.Code = 500;
                result.Message = "添加失败";
            }



        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;



    }

    [HttpPost]
    //[Authorize(Policy = AuthDefines.Policy_CookieAuthOrApiKeyAuth)]
    public async Task<Response<User>> UpdateAccountAsync([FromBody] CreateAccountInput input)
    {
        Response<User> result = new Response<User>();

        try
        {
            var res = await this._userMgr.UpdateUserAsync((int)input.Id, input.Account!, input.Password!, input.Name!, input.WorkId, input.Status!);
            result.Result = res.Data;



        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;


    }

    /// <summary>
    /// 校验用户密码
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<Response<User>> ValidateUser(string account, string password)
    {
        var user = await this._dBContext.Users.FirstOrDefaultAsync(u => u.Account == account);

        Response<User> resUser = new Response<User>();
        // 账号不存在
        if (user == null)
        {
            resUser.Code = 500;
            resUser.Message = "账号或密码错误";
        }
        // 密码不对
        else
        {
            var enterPWD = this._passwordHasher.ComputeHash(password, user.Salt);
            if (user.Password != enterPWD)
            {
                resUser.Code = 500;
                resUser.Message = "用户名或密码错误";
            }
            // 账号异常
            else if (user.Status != 0)
            {
                resUser.Code = 500;
                resUser.Message = "用户账户异常";
            }
        }
        resUser.Result = user;
        return resUser;
    }
    /// <summary>
    /// 验证产线管理权限
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<(Boolean, string)> CheckPower([FromQuery] string account, [FromQuery] string modlename)
    {

        var result = await this.rolesService.CheckPower(account, modlename);
        await sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.用户登录, Message = $"用户权限验证:{account},权限：{modlename}通过", Operator = account });
        return result;
    }


    [HttpPost]
    public async Task Record_CheckPower_Log(RecordCheckPowerDTO dto)
    {
        await this._CheckPowerLogService.Record_CheckPower_Log(dto);
        await Task.CompletedTask;
    }

    [HttpGet]
    public async Task<Response<ClaimEntity>> CheckuserClaims(string account, string password)
    {
        Response<ClaimEntity> resUser = new Response<ClaimEntity>();

        var user = this._dBContext.Users.FirstOrDefault(u => u.Account == account && !u.IsDeleted);
        var userclaim = this._dBContext.UserClaims.Where(a => a.UserId == user.Id && !a.IsDeleted).FirstOrDefault();
        var claim = this._dBContext.Claims.FirstOrDefault(a => a.Id == userclaim.ClaimEntityId && !a.IsDeleted);
        resUser.Result = claim;
        return resUser;
    }

    [HttpPost]
    public async Task<Response<bool>> InportUser(IFormFile file)
    {
        Response<bool> resUser = new Response<bool>();

        var exceldata = ExcelToEntity.WorksheetToDataRow<UserParamsExcel>(file.OpenReadStream(), 1, 2, 0, 0);
        var res = await this._userMgr.InportUser(exceldata);
        if (!res.Success)
        {
            resUser.Code = 500;
            resUser.Message = res.Message;

        }
        else
        {

        }
        return resUser;
    }

    [HttpGet]
    public async Task<IActionResult> ExportUser()
    {
        Response<bool> resUser = new Response<bool>();

        var res = await this._userMgr.ExportUser();
        if (res.Success)
        {
            ExcelPackage excelPackage = new ExcelPackage();
            excelPackage = ExcelToEntity.ListToExcek(excelPackage, "用户导出", 1, res.Data);
            var memory = new MemoryStream();
            excelPackage.SaveAs(memory);
            memory.Position = 0;
            this.Request.ContentType = "blob";
            var filename = $"{DateTime.Now.Date.ToShortDateString()}用户数据导出.xlsx";
            return File(memory, "application/vnd.ms-excel", filename);
        }
        else
        {

            return Ok(resUser);
        }
    }

    //[HttpGet]
    //public async Task<Response<List<User>>> GetList(SyslogDto dto)
    //{
    //    var result = new Response<List<SysLog>>();
    //    try
    //    {
    //        var list =  
    //        result.Data = list.Skip((dto.Page - 1) * dto.Limit).Take(dto.Limit);
    //        result.Count = list.Count;
    //    }
    //    catch (Exception ex)
    //    {

    //        result.Code = 500;
    //        result.Message = ex.InnerException?.Message ?? ex.Message;
    //    }
    //    return result;
    //}


}