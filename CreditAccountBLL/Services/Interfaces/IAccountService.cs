using System;
using System.Collections.Generic;
using System.Text;
using CurrencyCodesResolver;
using System.Threading.Tasks;

namespace CreditAccountBLL
{
    public interface IAccountService
    {
        Task<Result<decimal>> PutMoney(long userId, decimal amount, int currencyCode);
        Task<Result<decimal>> WithdrawMoney(long userId, decimal amount, int currencyCode);
        Task<Result<Dictionary<int, decimal>>> ChangeCurrency(long userId, decimal amount, int from, int to);
        Task<Result<Dictionary<int, decimal>>> GetBalance(long userId);
    }
}
