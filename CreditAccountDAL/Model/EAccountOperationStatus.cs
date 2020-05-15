using System;
using System.Collections.Generic;
using System.Text;

namespace CreditAccountDAL
{
    public enum EAccountOperationStatus
    {
        Success = 0,
        UserNotExists = 1,
        MoneyLessOrEqualZero = 2,
        NoCurrencyAccount = 3,
        NotEnoughMoney = 4,
        MoneyLessOrEqualZeroToDestinationAccount = 5,
        CurrenciesAreSame = 6
    }
}
