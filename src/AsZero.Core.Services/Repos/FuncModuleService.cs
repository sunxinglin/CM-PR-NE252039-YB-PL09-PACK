using AsZero.Core.Entities;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace AsZero.Core.Services
{

    public class FuncModuleService
    {
        private readonly AsZeroDbContext _dbContext;

        public FuncModuleService(AsZeroDbContext dbContext)
        {
            this._dbContext = dbContext;
        }


        public async Task<IList<FuncModule>> GetListByRolesAsync(IList<string> roles) 
        {
            var list = await this._dbContext.FuncModuleRoleMappings
                .Include(m => m.FuncModule)
                .Where(m => roles.Contains(m.RoleName) && m.FuncModule.IsSys && !m.FuncModule.IsDeleted)
                .Select(m => m.FuncModule).Distinct()
                .ToListAsync();
            return list;
        }


        public async Task<IList<FuncModule>> GetList()
        {
            var list = await this._dbContext.FuncModules
                .Where(r => !r.IsDeleted && r.IsSys)
                .ToListAsync();
            return list;
        }
        public async Task<IList<FuncModule>> GetFuncModuleListAdmin()
        {
            var list = await this._dbContext.FuncModules
                .Where(r => !r.IsDeleted )
                .ToListAsync();
            return list;
        }
        public async Task<FuncModule> Add(FuncModule entity)
        {
            var a = await this._dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return a.Entity;
        }

        public async Task<FuncModule> Update(FuncModule entity)
        {
            this._dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(int[] ids)
        {
            var list = await _dbContext.FuncModules.Where(a => ids.Contains(a.Id)).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;

            }
            this._dbContext.UpdateRange(list);

            await _dbContext.SaveChangesAsync();

        }

        public async Task< List<FuncModule>> ModulesViewList(int RoleId)
        {


            var list = await this._dbContext.FuncModules
                 .ToListAsync();
            return list;

        }
        public async Task<List<FuncModule>> LoadForRole(int? RoleId)
        {
           var claims= await this._dbContext.Claims.Where(a => a.Id == RoleId).FirstOrDefaultAsync();

            var list = await this._dbContext.FuncModuleRoleMappings
               .Include(m => m.FuncModule)
               .Where(m => m.RoleName==claims.ClaimValue)
               .Select(m => m.FuncModule)
               .ToListAsync();
            return list; 

        }

        public async Task DeleteRoleFunModule(int FunModuleId, int RoleId)
        {
            var claims = await this._dbContext.Claims.Where(a => a.Id == RoleId).FirstOrDefaultAsync();

            var entity = await _dbContext.FuncModuleRoleMappings.Where(a => a.FuncModuleId == FunModuleId && a.RoleName==claims.ClaimValue).FirstOrDefaultAsync();
            if (entity != null)
            {
                this._dbContext.Remove(entity);

            }
            await _dbContext.SaveChangesAsync();

        }

        public async Task<bool> AddRoleFunModule(List<int> ModuleIds,int RoleId)
        {
            var claims = await this._dbContext.Claims.Where(a => a.Id == RoleId).FirstOrDefaultAsync();
            foreach (var module in ModuleIds)
            {
                FuncModuleRoleMapping a = new FuncModuleRoleMapping();
                a.RoleName = claims.ClaimValue;
                a.FuncModuleId = module;
                 await this._dbContext.AddAsync(a);
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }




    }
}

