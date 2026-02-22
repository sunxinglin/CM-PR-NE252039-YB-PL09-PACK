using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ctp0600P.Client.Validations
{
    public class NotNullValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string valueStr = value as string;
            var result = ValidationHelper.NotNullValidation(valueStr);
            if (result) return new ValidationResult(true, null);
            return new ValidationResult(false, "不能为空！");
        }
    }
}
