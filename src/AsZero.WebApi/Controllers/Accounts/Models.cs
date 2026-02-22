using System.ComponentModel.DataAnnotations;

namespace AsZero.WebApi.Controllers
{
    public class LoginInput
    {
        public string? Account { get; set; }
        public string? Password { get; set; }
        public string? Equipment { get; set; }
    }
    public class LoginInputVim
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
   
    }

    public class LoginResultDto
    {
        public bool Status { get; set; }

        public string Type { get; set; } = "";


        public string CurrentAuthority { get; set; } = "";

    }
    public class LoginByCard
    {
        public string CardNo { get; set; } = "";


        public string Equipment { get; set; } = "";

    }
    public class ForgetPasswordInput : IValidatableObject
    {
        public string? Account { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Account))
            {
                yield return new ValidationResult($"{nameof(Account)}不可为空！", new[] { nameof(Account) });
            }
        }
    }
    public class ResetPasswordInput : IValidatableObject
    {
        public string? Account { get; set; }
        public string? Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Account))
            {
                yield return new ValidationResult($"{nameof(Account)}不可为空！", new[] { nameof(Account) });
            }

            if (string.IsNullOrEmpty(Password))
            {
                yield return new ValidationResult($"{nameof(Password)}不可为空！", new[] { nameof(Password) });
            }
        }
    }


    public class ChangePasswordInput
    {
        public string? Account { get; set; }
        public string? Password { get; set; }

    }
    public class GetUserInput
    {
        public string Account { get; set; }
    }


    public class CreateUserInput
    {
        public string? Account { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public List<int>? Settings { get; set; }
        public string? Email { get; set; }
        public string? GroupCode { get; set; }
        public string? WorkId { get; set; }
    }

    public class CreateAccountInput
    {
        public int? Id { get; set; }
        public string? Account { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public int Status { get; set; } = 0;
        public string? WorkId { get; set; }
    }
}