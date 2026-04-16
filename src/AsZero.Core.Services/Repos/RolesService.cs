using System.Security.Claims;

using AsZero.Core.Entities;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace AsZero.Core.Services.Repos
{


    public class RolesService
    {
        private readonly AsZeroDbContext _dbContext;

        public RolesService(AsZeroDbContext dbContext)
        {
            this._dbContext = dbContext;
        }


        public async Task<IList<ClaimEntity>> GetList(string key)
        {
            var query = this._dbContext.Claims
                 .Where(m => m.ClaimType == ClaimTypes.Role && !m.IsDeleted);
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(m => m.ClaimValue.Contains(key));
            }
            return await query.ToListAsync();

        }

        public async Task<ClaimEntity> Add(ClaimEntity entity)
        {
            var a = await this._dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return a.Entity;
        }
        public async Task<ClaimEntity?> GetById(int id)
        {
            var entity = await _dbContext.Claims.FindAsync(id);
            return entity;
        }
        public async Task<ClaimEntity> Update(ClaimEntity entity)
        {
            this._dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(int[] ids)
        {
            var list = await _dbContext.Claims.Where(a => ids.Contains(a.Id)).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;

            }
            this._dbContext.UpdateRange(list);

            await _dbContext.SaveChangesAsync();

        }

        public async Task<UserClaim> AddUserClaim(UserClaim entity)
        {
            var a = await this._dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return a.Entity;
        }

        public async Task DeleteUserClaim(int UserId, int RoleId)
        {
            var entity = await _dbContext.UserClaims.Where(a => a.UserId == UserId && a.ClaimEntityId == RoleId && !a.IsDeleted).FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.IsDeleted = true;
                this._dbContext.Update(entity);

            }


            await _dbContext.SaveChangesAsync();

        }
        public async Task<(Boolean, string)> CheckPower(string account, string modlename)
        {

            ///id =4 表示为产线管理字段,系统不可删除
            ///
            var user = await this._dbContext.Users.Where(o => !o.IsDeleted && o.Account == account).FirstOrDefaultAsync();
            if (user == null)
            {
                return (false, "用户不存在");
            }

            if ((user.Status != 0))
            {
                return (false, "用户账户异常");
            }

            var entityuserClaims = await this._dbContext.UserClaims.Include(o => o.ClaimEntity).Where(o => !o.IsDeleted && o.UserId == user.Id).ToListAsync();
            if (entityuserClaims == null)
            {
                return (false, "该用户未分配角色");
            }
            //单个用户可拥有多个角色
            foreach (var userClaim in entityuserClaims)
            {

                var entitfuncmodulerolemappings = await this._dbContext.FuncModuleRoleMappings.Include(o => o.FuncModule).Where(o => o.RoleName == userClaim.ClaimEntity.ClaimValue).ToListAsync();
                if (entitfuncmodulerolemappings == null || entitfuncmodulerolemappings.Count == 0)
                {
                    //当前角色未分配模块，直接跳转至下一个角色
                    continue;
                }
                var wpffuncmodules = entitfuncmodulerolemappings.Where(o => !o.FuncModule.IsSys && o.FuncModule.Name == modlename && !o.FuncModule.IsDeleted).FirstOrDefault();
                if (wpffuncmodules != null)
                {
                    return (true, "");
                }
            }
            return (false, $"用户[{account}]没有[{modlename}]权限！");
        }


    }
}

