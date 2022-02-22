using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VisualLayer.Resources.Classes.ValidationRules
{
    public class IsDoubleValidationRule : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is double) return ValidationResult.ValidResult;
            else if(value is string s)
            {
                if(double.TryParse(s, out _))
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "El valor no se puede convertir");
                }
            }
            return new ValidationResult(false, "El valor no se puede convertir");

        }
    }
}
