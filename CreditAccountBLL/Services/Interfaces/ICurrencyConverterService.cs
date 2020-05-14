using System;
using CreditAccountDAL;

namespace CreditAccountBLL
{
    public interface ICurrencyConverterService : IDisposable
    {
        Result<decimal> Convert(decimal amount, int fromCurrency, int toCurrency);
    }
}
