using System.ComponentModel.DataAnnotations;
using CurrencyCodesResolver;

namespace CreditAccount
{
    public class CurrencyExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            ICurrencyCodesResolver currencyCodeResolver = (ICurrencyCodesResolver) validationContext.GetService(typeof(ICurrencyCodesResolver));

            if (!currencyCodeResolver.IsExists(value.ToString()))
                return new ValidationResult($"Currency code: {value} is unknown");

            return ValidationResult.Success;
        }
    }
}
