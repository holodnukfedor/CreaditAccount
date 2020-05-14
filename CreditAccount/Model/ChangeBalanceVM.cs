using System.ComponentModel.DataAnnotations;

namespace CreditAccount
{
    public class ChangeBalanceVM
    {
        public long UserId { get; set; }
        [PositiveDecimal]
        public decimal Amount { get; set; }
        [Required]
        [CurrencyExists]
        public string CurrencyCode { get; set; }
    }
}
