using System.Security.Claims;

using AsZero.Core.Entities;
using AsZero.Core.Services.Auth;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Yee.Services.Request;

namespace AsZero.WebApi.Controllers
{


    /// <summary>
    /// 角色
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly RolesService _rolesService;
        private readonly IUserManager _userMgr;
        private readonly SysLogService sys_LogService;

        public RolesController(RolesService rolesService, IUserManager userMgr,SysLogService Sys_LogService)
        {
            _rolesService = rolesService;
            _userMgr = userMgr;
            this.sys_LogService = Sys_LogService;
        }

        [Authorize]
        [HttpGet]
        public async Task<Response<IList<ClaimEntity>>> GetList([FromQuery] GetByKeyInput input)
        {
            var result = new Response<IList<ClaimEntity>>();
            try
            {
                var list = await _rolesService.GetList(input.Key);
                result.Result = list;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;

        }


        //添加或修改
        [HttpPost]
        public async Task<Response<ClaimEntity>> Add(ClaimEntity obj)
        {
            var result = new Response<ClaimEntity>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                obj.ClaimType = ClaimTypes.Role;
                await _rolesService.Add(obj);
                result.Result = obj;
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增角色, Message = $"新增角色:{obj.ClaimValue}", Operator = user });
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        //添加或修改
        [HttpPost]
        public async Task<Response> Update(ClaimEntity obj)
        {
            var result = new Response();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                obj.ClaimType = ClaimTypes.Role;
                await _rolesService.Update(obj);
     
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改角色, Message = $"修改角色:{obj.ClaimValue}",Operator = user });
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }


        [HttpPost]
        public async Task<Response> Delete(DeleteByIdsInput input)
        {
            var result = new Response();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                await _rolesService.Delete(input.Ids);
                foreach (var item in input.Ids)
                {
                    var roles = await this._rolesService.GetById(item);
                    await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除角色, Message = $"删除角色:{roles.ClaimValue}",Operator = user });
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
        public async Task<Response> AssignRoleUsers(AssignRoleUsersInput input)
        {
            var result = new Response<UserClaim>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                GetUserListReq request = new GetUserListReq();
                request.roleId = input.RoleId;
                var list = await this._userMgr.GetList(request);
                var oldIds = list.Select(s => s.Id).ToList();
                var newIds = input.UserIds.ToList();
                //找差集
                //取差集时,主集合不同,取得的结果不同
                var deleteIds = oldIds.Except(newIds).ToList();
                var addIds = newIds.Except(oldIds).ToList();
                foreach (var did in deleteIds)
                { //原来有的，现在没有了，要删除
                    await _rolesService.DeleteUserClaim(did, input.RoleId);
                }
                foreach (var aid in addIds)
                {//原来没有，现在有，要新增
                    UserClaim obj = new UserClaim()
                    {
                        UserId = aid,
                        ClaimEntityId = input.RoleId
                    };

                    await _rolesService.AddUserClaim(obj);
                }
                var roles =await this._rolesService.GetById(input.RoleId);
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.角色配置, Message = $"角色配置:{roles.ClaimValue}",Operator =user });
                


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