using CurrencyCodesResolver;
using CreditAccountDAL;

namespace CreditAccount
{
    public class BalanceVM
    {
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public BalanceVM(Balance balance, ICurrencyCodesResolver currencyCodesResolver)
        {
            CurrencyCode = currencyCodesResolver.Resolve(balance.CurrencyCode);
            Balance = balance.Money;
        }
        public BalanceVM()
        {

        }
    }
}
