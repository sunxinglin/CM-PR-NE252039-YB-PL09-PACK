using Yee.Tools;

namespace AsZero.Core.Services.Auth
{
    public enum CreateUserRoles
    {
        管理员 = 1,
        工程师 = 2,
        操作员 = 3,
        新用户 = 4,
    }
    public class UserParamsExcel
    {
        [ExcelToEntity(1)]
        [EntityToexcel("用户名", 1)]
        public string UserName { get; set; } = "";
        
        [EntityToexcel("账号", 2)]
        [ExcelToEntity(2)]

        public string Account { get; set; } = "";
        [EntityToexcel("工号", 3)]
        [ExcelToEntity(3)]

        public string WorkId { get; set; } = "";

        public CreateUserRoles Role { get; set; } = CreateUserRoles.新用户;
    }
}
