using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ctp0600P.Client.Validations
{
    public class IPAddressRule: ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string IPAddress = value as string;

            var result=ValidationHelper.IsIpAddress(IPAddress);
            if(result)
                return new ValidationResult(true, null);
            return new ValidationResult(false, "IP地址格式不正确");

        }
    }
}
