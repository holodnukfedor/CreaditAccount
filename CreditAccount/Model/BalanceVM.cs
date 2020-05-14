using CurrencyCodesResolver;
using System;
using System.Collections.Generic;

namespace CreditAccount
{
    public class BalanceVM
    {
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public BalanceVM(KeyValuePair<int, decimal> keyValuePair, ICurrencyCodesResolver currencyCodesResolver)
        {
            CurrencyCode = currencyCodesResolver.Resolve(keyValuePair.Key);
            Balance = keyValuePair.Value;
        }
        public BalanceVM()
        {

        }
    }
}
