using System;
using System.Collections.Generic;
using CurrencyCodesResolver;
using System.Threading.Tasks;
using CreditAccountDAL;
using System.Linq;

namespace CreditAccountBLL
{
    public class AccountService : IAccountService
    {
        private IDbManager _dbManager;
        private ICurrencyCodesResolver _currencyCodesResolver;
        private ICurrencyConverterService _currencyConverterService;

        public AccountService(IDbManager dbManager, ICurrencyCodesResolver currencyCodesResolver, ICurrencyConverterService currencyConverterService)
        {
            _dbManager = dbManager;
            _currencyCodesResolver = currencyCodesResolver;
            _currencyConverterService = currencyConverterService;
        }

        private void ThrowIfNegativeOrZeroAmountOfMoney(decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Money amount must be greater than zero");
        }

        private void ThrowIfCurrencyNoExist(int currencyCode)
        {
            if (!_currencyCodesResolver.IsExists(currencyCode))
                throw new Exception($"Currency code doesn't exist. Currency code: [{currencyCode}]");
        }

        async public Task<Result> ChangeCurrencyAsync(long userId, decimal amount, int from, int to)
        {
            ThrowIfNegativeOrZeroAmountOfMoney(amount);
            ThrowIfCurrencyNoExist(from);
            ThrowIfCurrencyNoExist(to);

            Result<decimal> convertionResult = _currencyConverterService.Convert(amount, from, to);
            if (!convertionResult.IsSuccess)
                return Result<Dictionary<int, decimal>>.CreateError(convertionResult.ErrorMessage);

            using (_dbManager.OpenConnection())
            {
                return await _dbManager.AccountRepository.ChangeCurrencyAsync(userId, from, amount, to, convertionResult.Data);
            }
        }

        async public Task<Result<Balance[]>> GetBalanceAsync(long userId)
        {
            using (_dbManager.OpenConnection())
            {
                return await _dbManager.AccountRepository.GetBalanceAsync(userId);
            }
        }

        async public Task<Result> PutMoneyAsync(long userId, decimal amount, int currencyCode)
        {
            ThrowIfNegativeOrZeroAmountOfMoney(amount);
            ThrowIfCurrencyNoExist(currencyCode);
            using (_dbManager.OpenConnection())
            {
                return await _dbManager.AccountRepository.PutMoneyAsync(userId, currencyCode, amount);
            }
        }

        async public Task<Result> WithdrawMoneyAsync(long userId, decimal amount, int currencyCode)
        {
            ThrowIfNegativeOrZeroAmountOfMoney(amount);
            ThrowIfCurrencyNoExist(currencyCode);

            using (_dbManager.OpenConnection())
            {
                return await _dbManager.AccountRepository.WithdrawMoneyAsync(userId, currencyCode, amount);
            }
        }
    }
}
