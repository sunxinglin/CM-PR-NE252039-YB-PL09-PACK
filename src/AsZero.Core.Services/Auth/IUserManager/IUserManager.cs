using System.Security.Claims;

using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;

using FutureTech.OpResults;

namespace AsZero.Core.Services.Auth
{
    public interface IUserManager
    {
        /// <summary>
        /// 用户是否存在
        /// </summary>
        /// <param name="accout"></param>
        /// <returns></returns>
        bool UserExists(string accout);

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<OpResult<User>> CreateUserAsync(string account, string password, string name,  string workId, int status = 0);
        
        Task<OpResult<User>> CreateUserAsync(string account, string password, string name,  string workId, int status, string salt);

        Task<OpResult<User>> UpdateUserAsync(int Id,string account, string password, string name, string workId, int status = 0);

        /// <summary>
        /// 根据账号获取用户实体
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<User?> GetUserAsync(string account);
        Task<User?> GetUserByWorkIdAsync(string workId);
        Task<OpResult<User>> Check_CreateAccountCardAsync(string workId);
        
        /// <summary>
        /// 验证用户的账户和密码是否正确
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<OpResult<User>> ValidateUserAsync(string account, string password);


        /// <summary>
        /// 加载用户Principal
        /// </summary>
        /// <param name="account"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task<ClaimsPrincipal?> LoadPrincipalAsync(string account, bool force);

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        Task<OpResult<User>> ChangePasswordAsync(string account, string newPass);


        /// 校验重置密码的令牌
        bool ValidateResetPasswordToken(string account, string token);

        Task<TableData> Load(QueryUserListReq request);
        Task<List<User>> GetList(GetUserListReq request);

        /// <summary>
        /// 导入用户
        /// </summary>
        /// <param name="excelINs"></param>
        /// <returns></returns>
        Task<OpResult<bool>> InportUser(List<UserParamsExcel> excelINs);
        /// <summary>
        /// 导出用户
        /// </summary>
        /// <param name="excelINs"></param>
        /// <returns></returns>
        Task<OpResult<List<UserParamsExcel>>> ExportUser();
        Task<OpResult<User>> ValidateCardNoAsync(string cardNo);
        //public async Task<List<User>> GetList(SyslogDto dto)
        //{
        //    var query = this._dBContext.Sys_Logs.Where(o => !o.IsDeleted);
        //    if ((int)dto.Type != -1)
        //    {
        //        query = query.Where(o => o.LogType == dto.Type);
        //    }
        //    if (dto.EndTime != null && dto.BeginTime != null)
        //    {
        //        query = query.Where(o => o.CreateTime.Date >= dto.BeginTime.Value.Date && o.CreateTime.Date <= dto.EndTime.Value.Date);
        //    }
        //    return await query.ToListAsync();
        //}
    }
}
