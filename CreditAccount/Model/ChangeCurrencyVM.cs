using System.ComponentModel.DataAnnotations;

namespace CreditAccount
{
    public class ChangeCurrencyVM
    {
        public long UserId { get; set; }
        [PositiveDecimal]
        public decimal Amount { get; set; }
        [Required]
        [CurrencyExists]
        public string FromCurrencyCode { get; set; }
        [Required]
        [CurrencyExists]
        public string ToCurrencyCode { get; set; }
    }
}
