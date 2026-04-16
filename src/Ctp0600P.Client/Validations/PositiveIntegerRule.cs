using System.Globalization;
using System.Windows.Controls;

namespace Ctp0600P.Client.Validations;

public class PositiveIntegerRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string valueStr = value as string;
        var result = ValidationHelper.PositiveInteger(valueStr);
        if (result)
        {
            return new ValidationResult(true, null);
        }
        return new ValidationResult(false, "必须为正整数！");
    }
}