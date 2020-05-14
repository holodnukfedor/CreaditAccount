using System;
using System.Collections.Generic;
using System.Text;

namespace CreditAccountDAL
{
    public class Balance
    {
        public int CurrencyCode { get; }
        public decimal Money { get; }

        public Balance(int currencyCode, decimal money)
        {
            CurrencyCode = currencyCode;
            Money = money;
        }
    }
}
