using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VisualLayer.Resources.Classes.ValidationRules
{

    class PasswordValidationRule : ValidationRule
    {
        public static string GetErrorMessage(string fieldName, object fieldValue, object passwordBox = null, object nullValue = null)
        {
            PasswordBox psBox = passwordBox as PasswordBox;
            string errorMessage = string.Empty;
            if (nullValue != null && nullValue.Equals(fieldValue))
                errorMessage = string.Format("No puedes dejar el campo {0} vacio.", fieldName);
            if (fieldValue == null || string.IsNullOrEmpty(fieldValue.ToString()))
                errorMessage = string.Format("No puedes dejar el campo {0} vacio.", fieldName);
            if (psBox != null && !fieldValue.Equals(psBox.Password))
            {
                errorMessage = string.Format("Las contraseñas no coinciden.");
            }
            return errorMessage;
        }
        public string FieldName { get; set; }
        public string PasswordBox { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = GetErrorMessage(FieldName, value, PasswordBox);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);
            return ValidationResult.ValidResult;
        }
    }
}
