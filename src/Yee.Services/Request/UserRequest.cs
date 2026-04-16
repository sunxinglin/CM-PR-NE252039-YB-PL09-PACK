namespace Yee.Services.Request
{
    public class UserRequest
    {
    }

    public class ValidateUserInput
    {
        public string? Account { get; set; }
        public string? Password { get; set; }
    }

    public class ModuleView
    {
        public int SortNo { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? IconName { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }

        public string? Code { get; set; }
        public string? Url { get; set; }

        public string? CascadeId { get; set; }
        public bool IsSys { get; set; }
    }

    public class LoadMenusInput
    { 
    public string? ModuleId { get; set; }
      
    }

    public class LoadForRoleInput
    { 
    public int? FirstId { get; set; }
    }
    public class LoadMenusOutput
    { 
    public int Id { get; set; }
        public string Label { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public bool? IsSys { get; set; }
        public string? Code { get; set; }
    } 
}
