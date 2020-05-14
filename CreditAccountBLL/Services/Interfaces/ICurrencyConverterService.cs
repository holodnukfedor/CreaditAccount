using System;
using System.Collections.Generic;
using System.Text;

namespace CreditAccountBLL
{
    public interface ICurrencyConverterService : IDisposable
    {
        Result<decimal> Convert(decimal amount, int fromCurrency, int toCurrency);
    }
}
