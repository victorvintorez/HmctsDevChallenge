using System.ComponentModel.DataAnnotations;

namespace HmctsDevChallenge.Backend.Helpers.Validation;

public class FutureDateTimeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return value switch
        {
            DateTime dateTime => dateTime.ToUniversalTime().CompareTo(DateTime.UtcNow) > 0
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage),
            _ => new ValidationResult(ErrorMessage)
        };
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a DateTime in the future.";
    }
}