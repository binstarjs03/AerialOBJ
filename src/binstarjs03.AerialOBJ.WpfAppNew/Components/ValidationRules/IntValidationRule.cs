using System.Globalization;
using System.Windows.Controls;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components.ValidationRules;

public class IntValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is not string stringValue)
            return new ValidationResult(false, "Cannot validate non-string object");
        if (int.TryParse(stringValue, out _) || string.IsNullOrEmpty(stringValue))
            return ValidationResult.ValidResult;
        return new ValidationResult(false, "Number format is invalid");
    }
}
