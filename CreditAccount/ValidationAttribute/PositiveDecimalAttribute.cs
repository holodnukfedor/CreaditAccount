using System.ComponentModel.DataAnnotations;

namespace CreditAccount
{
    public class PositiveDecimalAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            decimal number = (decimal)value;
            if (number <= 0)
                return new ValidationResult($"{validationContext.DisplayName} has to be greater than zero");

            return ValidationResult.Success;
        }
    }
}
