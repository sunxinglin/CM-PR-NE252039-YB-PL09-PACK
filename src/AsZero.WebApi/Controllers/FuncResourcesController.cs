using System.Security.Claims;

using AsZero.Core.Entities;
using AsZero.Core.Services.Auth;

using Microsoft.AspNetCore.Mvc;

namespace AsZero.WebApi.Controllers
{
    /// <summary>
    /// 功能资源
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FuncResourcesController : ControllerBase
    {
        private readonly ResourceService _resourceSvc;

        public FuncResourcesController(ResourceService resourceSvc)
        {
            this._resourceSvc = resourceSvc;
        }

        [HttpGet]
        public async Task<IList<FuncResource>> All()
        {
            var list = await this._resourceSvc.LoadAllResoucesAsync();
            return list;
        }
        [HttpGet]
        public async Task<IList<Claim>> Claims()
        {
            var claims = await this._resourceSvc.LoadAllClaimsAsync();

            return claims;
        }
    }
}
