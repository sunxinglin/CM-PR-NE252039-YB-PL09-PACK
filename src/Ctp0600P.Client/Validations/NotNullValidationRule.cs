using System.Globalization;
using System.Windows.Controls;

namespace Ctp0600P.Client.Validations;

public class NotNullValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string valueStr = value as string;
        var result = ValidationHelper.NotNullValidation(valueStr);
        if (result)
        {
            return new ValidationResult(true, null);
        }
        return new ValidationResult(false, "不能为空！");
    }
}