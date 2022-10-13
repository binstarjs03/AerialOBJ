using System.Globalization;
using System.Windows.Controls;

namespace binstarjs03.AerialOBJ.WpfApp.ValidationRules;

public class DoubleValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string stringValue = (string)value;
        stringValue = stringValue.Replace('.', ',');
        if (double.TryParse(stringValue, out _) || string.IsNullOrEmpty(stringValue))
            return ValidationResult.ValidResult;
        return new ValidationResult(false, "Number format is invalid");
    }
}
