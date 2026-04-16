namespace AsZero.Core.Services.Repos
{
    public class UserRequest
    {
    }
    public class QueryUserListReq : PageReq
    {
        public string? Status { get; set; }
    }
     public class GetUserListReq 
    {
        //public string userName { get; set; } = "";
        public int? roleId { get; set; }
    
    }
    public class RoleView
    { 
    public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AssignRoleUsersInput
    { 
    public int RoleId { get; set; } 
        public int[] UserIds { get; set; }

    }


    public class AssignRoleFunModulesInput
    {
        public int RoleId { get; set; }
        public int[] SelectedModuleId { get; set; }

    }

}
