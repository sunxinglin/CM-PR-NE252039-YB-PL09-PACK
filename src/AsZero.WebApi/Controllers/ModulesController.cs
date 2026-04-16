using System.Security.Claims;

using AsZero.Core.Entities;
using AsZero.Core.Services;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using AsZero.WebApi.Helpers;

using Microsoft.AspNetCore.Mvc;

using Yee.Services.Request;

namespace AsZero.WebApi.Controllers.BaseData
{
    /// <summary>
    /// 功能模块及菜单管理
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    //[Authorize]
    public class ModulesController : ControllerBase
    {
        private readonly FuncModuleService _moduleService;
        private readonly SysLogService sys_LogService;
        private readonly RolesService rolesService;

        public ModulesController(FuncModuleService moduleService,SysLogService sys_LogService,RolesService rolesService)
        {
            _moduleService = moduleService;
            this.sys_LogService = sys_LogService;
            this.rolesService = rolesService;
        }




        //[Authorize]
        [HttpGet]
        public async Task<Response<IEnumerable<TreeItem<FuncModule>>>> GetList()
        {
            var roles = HttpContext.User.FindAll(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();


            var result = new Response<IEnumerable<TreeItem<FuncModule>>>();
            try
            {
                IList<FuncModule> list = null;
                if (roles.Contains(Defines.Claim_Admin))
                {
                    list = await this._moduleService.GetList();
                }
                else
                {
                    list = await this._moduleService.GetListByRolesAsync(roles);
                }



                var tree = list.GenerateTree(t => t.Id, t => t.ParentId);
                result.Result = tree;

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        //[Authorize]
        [HttpGet]
        public async Task<Response<IList<FuncModule>>> GetFuncModuleList()
        {
            var roles = HttpContext.User.FindAll(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();


            var result = new Response<IList<FuncModule>>();
            try
            {
                
                 result.Result = await this._moduleService.GetFuncModuleListAdmin();
              

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }


        #region 添加编辑模块

        //添加或修改
        [HttpPost]
        public async Task<Response<FuncModule>> Add(FuncModule obj)
        {
            var result = new Response<FuncModule>();
            try
            {
                await _moduleService.Add(obj);
                result.Result = obj;
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
        public async Task<Response> Update(FuncModule obj)
        {
            var result = new Response();
            try
            {
                await _moduleService.Update(obj);

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
                await _moduleService.Delete(input.Ids);

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        #endregion 添加编辑模块

        /// <summary>
        /// 获取发起页面的菜单权限
        /// </summary>
        /// <returns>System.String.</returns>
        [HttpGet]
        public async Task<Response<IEnumerable<TreeItem<FuncModule>>>> LoadMenus([FromQuery] LoadMenusInput input)
        {
            var result = new Response<IEnumerable<TreeItem<FuncModule>>>();
            try
            {


                IList<FuncModule> list = await _moduleService.ModulesViewList(0);

                var tree = list.GenerateTree(t => t.Id, t => t.ParentId);
                result.Result = tree;

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;




        }
        /// <summary>
        /// 获取发起页面的菜单权限
        /// </summary>
        /// <returns>System.String.</returns>
        [HttpGet]
        public async Task<Response<List<FuncModule>>> LoadForRole([FromQuery] LoadForRoleInput input)
        {
            var result = new Response<List<FuncModule>>();
            try
            {

                result.Result = await _moduleService.LoadForRole(input.FirstId);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpPost]
        public async Task<Response> AssignRoleFuncModule(AssignRoleFunModulesInput input)
        {
            var result = new Response<FuncModule>();
            try
            {
                var user = Request.Cookies.Where(p=>p.Key == "SET_NAME").First().Value.ToString();
                var list = await _moduleService.LoadForRole(input.RoleId);
                var oldIds = list.Select(s => s.Id).ToList();
                var newIds = input.SelectedModuleId.ToList();
                //找差集
                //取差集时,主集合不同,取得的结果不同
                var deleteIds = oldIds.Except(newIds).ToList();
                var addIds = newIds.Except(oldIds).ToList();
                foreach (var did in deleteIds)
                { //原来有的，现在没有了，要删除
                    await _moduleService.DeleteRoleFunModule(did, input.RoleId);
                }
                //原来没有，现在有，要新增
                await _moduleService.AddRoleFunModule(addIds, input.RoleId);
                var roles =await rolesService.GetById(input.RoleId);
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.权限配置, Message = $"权限配置:{roles.ClaimValue}", Operator = user });



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