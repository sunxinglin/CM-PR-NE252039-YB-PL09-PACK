using FutureTech.Dal.Repository;
using FutureTech.Dal.Services;
using FutureTech.OpResults;
using AsZero.DbContexts;
using AsZero.Core.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AsZero.Core.Entities;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using AsZero.Core.Services.Repos;
using Yee.Entitys.CommonEntity;
using Microsoft.Extensions.Configuration;

namespace AsZero.Core.Services.Auth
{

    public class DefaultUserManager : IUserManager
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ITimeLimitedDataProtector _protector;
        public DefaultUserManager(AsZeroDbContext aszeroDbContext, IPasswordHasher passwordHasher, IConfiguration configuration, IDataProtectionProvider provider)
        {
            this._dbContext = aszeroDbContext;
            this._passwordHasher = passwordHasher;
            this._protector = provider.CreateProtector($"ResetPasswordToken.{nameof(DefaultUserManager)}").ToTimeLimitedDataProtector();
            _configuration = configuration;
        }


        /// <summary>
        /// 校验用户密码
        /// </summary>
        /// <returns></returns>
        public Task<OpResult<User>> ValidateUserAsync(string account, string password)
        {
            var user = this._dbContext.Users.FirstOrDefault(u => u.Account == account);

            // 账号不存在
            if (user == null)
            {
                OpResult<User> res1 = new FailedOpResult<User>("账号或密码错误");      // 可能客户要求提示账号不存在
                return Task.FromResult(res1);
            }

            // 密码不对
            if (user.Password != this._passwordHasher.ComputeHash(password, user.Salt))
            {
                OpResult<User> res2 = new FailedOpResult<User>("用户名或密码错误");   // 可能客户要求提示密码不对

                return Task.FromResult(res2);
            }

            // 账号异常
            if (user.Status != 0)
            {
                OpResult<User> res3 = new FailedOpResult<User>("用户账户异常");
                return Task.FromResult(res3);
            }

            OpResult<User> res = new SucceededOpResult<User>(user);
            return Task.FromResult(res);
        }
        public Task<OpResult<User>> ValidateCardNoAsync(string cardNo)
        {
            var user = this._dbContext.Users.FirstOrDefault(u => u.Account == cardNo);

            // 账号不存在
            if (user == null)
            {
                OpResult<User> res1 = new FailedOpResult<User>("账号或密码错误");      // 可能客户要求提示账号不存在
                return Task.FromResult(res1);
            }

            

            // 账号异常
            if (user.Status != 0)
            {
                OpResult<User> res3 = new FailedOpResult<User>("用户账户异常");
                return Task.FromResult(res3);
            }

            OpResult<User> res = new SucceededOpResult<User>(user);
            return Task.FromResult(res);
        }
        public Task<User?> GetUserAsync(string account)
        {
            var d = this._dbContext.Users.FirstOrDefault(u => u.Account == account && u.Status == 0);
            return Task.FromResult(d);
        }

        public Task<User?> GetUserByWorkIdAsync(string workId)
        {
            var d = this._dbContext.Users.FirstOrDefault(u => u.WorkId == workId && u.Status == 0);
            return Task.FromResult(d);
        }

        public async Task<OpResult<User>> Check_CreateAccountCardAsync(string workId)
        {
            var result = new OpResult<User>();
            var rule = _configuration.GetSection("AppOpts").Get<AppOpts>().AccountCardRule;

            if (string.IsNullOrEmpty(rule))
            {
                result.Success = false;
                result.Message = "校验失败，请联系管理员配置卡号规则！";
                result.Data = null;
                return result;
            }
            if (workId.Length == rule.Length && workId.StartsWith(rule.TrimEnd('*')))
            {
                var user = this._dbContext.Users.FirstOrDefault(u => u.WorkId == workId);
                if (user == null)
                {
                    var userResult = await CreateUserAsync(workId, workId, workId, workId);
                    if (userResult.Success)
                    {
                        user = userResult.Data;
                        result.Success = true;
                        result.Data = user;
                    }
                    else
                    {
                        result.Success = false;
                        result.Data = user;
                        result.Message = "校验失败，尝试匹配账户信息失败！";
                    }
                }
                else if (user.Status == 0)
                {
                    result.Success = true;
                    result.Data = user;
                }
                else
                {
                    result.Success = false;
                    result.Message = "用户账户状态异常！";
                }
                return result;
            }

            result.Success = false;
            result.Message = "校验失败，卡号规则不合法！";
            result.Data = null;
            return result;
        }


        public bool UserExists(string accout)
        {
            return this._dbContext.Users.Any(u => u.Account == accout);
        }


        public async Task<ClaimsPrincipal?> LoadPrincipalAsync(string account, bool force)
        {
            var user = await this._dbContext.Users
                .AsNoTracking()
                .Where(u => u.Account == account)
                .Include(u => u.UserClaims)
                .ThenInclude(uc => uc.ClaimEntity)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            IList<Claim> claims = user.UserClaims?.Where(a=>!a.IsDeleted)
                .Select(uc => new Claim(uc.ClaimEntity.ClaimType, uc.ClaimEntity.ClaimValue))
                .ToList() ?? new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, account));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim("Id", user.Id.ToString()));

            var identity = new ClaimsIdentity(claims, "X-Authen");
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

        public async Task<OpResult<User>> CreateUserAsync(string account, string password, string name, string workId, int status = 0)
        {
            var user = this._dbContext.Users.FirstOrDefault(u => u.Account == account);
            if (user != null)
            {
                OpResult<User> res = new FailedOpResult<User>($"账号={account}已经存在！");
                return res;
            }

            var salt = Guid.NewGuid().ToString();
            var pass = this._passwordHasher.ComputeHash(password, salt);

            user = new User
            {
                Account = account,
                Password = pass,
                Name = name,
                Salt = salt,
                Status = status,
                WorkId = workId
            };
            this._dbContext.Users.Add(user);
            await this._dbContext.SaveChangesAsync();
            return new SucceededOpResult<User>(user);

        }

        public async Task<OpResult<User>> UpdateUserAsync(int Id, string account, string password, string name, string workId, int status = 0)
        {
            var user = this._dbContext.Users.FirstOrDefault(u => u.Id == Id);
            if (user != null)
            {
                user.Account = account;
                user.Name = name;
                user.Status = status;
                if (!user.Password.Equals(password))
                {
                    var salt2 = Guid.NewGuid().ToString();
                    var pass2 = this._passwordHasher.ComputeHash(password, salt2);
                    user.Password = pass2;
                    user.Salt = salt2;
                    user.WorkId = workId;
                }
                this._dbContext.Users.Update(user);
                await this._dbContext.SaveChangesAsync();
            }



            return new SucceededOpResult<User>(user);

        }


        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        public async Task<OpResult<User>> ChangePasswordAsync(string account, string newPass)
        {
            var existed = this._dbContext.Users.FirstOrDefault(u => u.Account == account);
            if (existed == null)
            {
                return new NotFoundOpResult<User>()
                {
                    Message = $"未找到account={account}的用户记录",
                };
            }
            var encryptedPwd = this._passwordHasher.ComputeHash(newPass, existed.Salt);
            existed.Password = encryptedPwd;
            await this._dbContext.SaveChangesAsync();
            return new SucceededOpResult<User>(existed);
        }


        public string GenerateResetPasswordToken(string account, int expires = 12)
        {
            var payload = GenerateResetTokenPayloadForAccount(account);
            var str = this._protector.Protect(payload, TimeSpan.FromHours(expires));
            return str;
        }

        private static string GenerateResetTokenPayloadForAccount(string account) => $"{account}:12bc1ec2-cda5-4630-a726-77a36ae65b79";

        public bool ValidateResetPasswordToken(string account, string token)
        {
            try
            {
                var str = this._protector.Unprotect(token, out var expiration);
                var payload = GenerateResetTokenPayloadForAccount(account);
                return (expiration >= DateTime.Now && str == payload);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }


        /// <summary>
        /// 加载当前登录用户可访问的一个部门及子部门全部用户
        /// 如果请求的request.OrgId为空，则可以获取到已被删除机构的用户（即：没有分配任何机构的用户）
        /// </summary>
        public async Task<TableData> Load(QueryUserListReq request)
        {
            var query = this._dbContext.Users.Where(a => !a.IsDeleted);
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(e => e.Status == Convert.ToInt32(request.Status));
            }
            else
            {
                query = query.Where(e => e.Status == 0);  
            }
            if (!string.IsNullOrEmpty(request.key))
            {
                query = query.Where(u => u.Account.Contains(request.key) || u.Name.Contains(request.key) || u.WorkId.Contains(request.key));
            }
            var result = new TableData();

            result.data = await query.AsNoTracking()
                .Skip((request.page - 1) * request.limit)
                .Take(request.limit).ToListAsync();
            result.count = await query.CountAsync();
            return result;
        }

        public async Task<List<User>> GetList(GetUserListReq request)
        {
            var query = this._dbContext.UserClaims.Where(a => !a.IsDeleted);
            if (request.roleId != null)
            {

                query = query.Where(a => a.ClaimEntityId == request.roleId);
            }
            //if (!string.IsNullOrEmpty(request.userName))
            //{
            //    query = query.Where(a=>a.User.Account.Contains(request.userName));
            //}

            return await query.Select(a => a.User).AsNoTracking().ToListAsync();
        }
        public async Task<OpResult<bool>> InportUser(List<UserParamsExcel> excelINs)
        {
            using (var trance = await _dbContext.Database.BeginTransactionAsync())
            {
                foreach (var item in excelINs)
                {
                    var result = await CreateUserAsync(item.Account, item.Account, item.UserName,item.WorkId);
                    if (!result.Success)
                    {
                        await trance.RollbackAsync();
                        return new FailedOpResult<bool>($"创建用户失败：{result.Message}");
                    }
                }
                await trance.CommitAsync();
            }
            return new SucceededOpResult<bool>(true);
        }
        public async Task<OpResult<List<UserParamsExcel>>> ExportUser()
        {
            var exdatas = new List<UserParamsExcel>();
            var dats = await _dbContext.Users.ToListAsync();
            foreach (var data in dats)
            {
                var exdata = new UserParamsExcel()
                {
                    
                    UserName = data.Name,
                    Account = data.Account,
                    WorkId = data.WorkId??""
                };
                exdatas.Add(exdata);
            }
            return new SucceededOpResult<List<UserParamsExcel>>(exdatas);
        }
    }

}
