using AsZero.Core.Entities;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.AutomaticStation;

namespace AsZero.Core.Services.Auth
{
    public class ResourceService
    {
        private readonly AsZeroDbContext _dbContext;

        public ResourceService(AsZeroDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Claim>> LoadAllClaimsAsync()
        {
            return (await _dbContext.Claims.Where((ClaimEntity r) => !r.IsDeleted).ToListAsync()).Select((ClaimEntity c) => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }



        public async Task<IList<FuncResource>> LoadAllResoucesAsync()
        {
            var modules = await _dbContext.FuncModules.Include(o => o.Allowed).Where((FuncModule r) => !r.IsDeleted && !string.IsNullOrEmpty( r.Code)).ToListAsync();
            return modules.Select(o => new FuncResource()
            {
                UniqueName = o.Name,
                IsDeleted = o.IsDeleted,
                AllowedClaims=o.Allowed.Select( x=> GetClimins(x.RoleName)).Where(s=>s!=null).ToList()
            }).ToList();
             Claim? GetClimins(string rolename)
            {
                var dbclaim=  _dbContext.Claims.FirstOrDefault(o => o.ClaimValue == rolename);
                if (dbclaim != null)
                    return new Claim( dbclaim.ClaimType,dbclaim.ClaimValue);
                return null;
            }

         }

       
    }
}
