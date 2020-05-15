using System.Collections.Generic;
using CurrencyCodesResolver;
using System.Threading.Tasks;
using CreditAccountDAL;

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

        async public Task<Result> ChangeCurrencyAsync(long userId, decimal amount, int from, int to)
        {
           
            if (amount <= 0)
                return AccountRepositoryErrorResultCreator.MoneyLessOrEqualZero(amount);

            if (!_currencyCodesResolver.IsExists(to))
                return AccountRepositoryErrorResultCreator.CurrencyNotExists(to);

            if (!_currencyCodesResolver.IsExists(from))
                return AccountRepositoryErrorResultCreator.CurrencyNotExists(from);

            if (from == to)
                return AccountRepositoryErrorResultCreator.CurrenciesAreSame(from);

            Result<decimal> convertionResult = _currencyConverterService.Convert(amount, from, to);
            if (!convertionResult.IsSuccess)
                return Result<Dictionary<int, decimal>>.CreateError(convertionResult.ErrorMessage);

            if (convertionResult.Data <= 0)
                return AccountRepositoryErrorResultCreator.CreateConvertationError(convertionResult.Data);

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
            if (amount <= 0)
                return AccountRepositoryErrorResultCreator.MoneyLessOrEqualZero(amount);

            if (!_currencyCodesResolver.IsExists(currencyCode))
                return AccountRepositoryErrorResultCreator.CurrencyNotExists(currencyCode);

            using (_dbManager.OpenConnection())
            {
                return await _dbManager.AccountRepository.PutMoneyAsync(userId, currencyCode, amount);
            }
        }

        async public Task<Result> WithdrawMoneyAsync(long userId, decimal amount, int currencyCode)
        {
            if (amount <= 0)
                return AccountRepositoryErrorResultCreator.MoneyLessOrEqualZero(amount);

            if (!_currencyCodesResolver.IsExists(currencyCode))
                return AccountRepositoryErrorResultCreator.CurrencyNotExists(currencyCode);

            using (_dbManager.OpenConnection())
            {
                return await _dbManager.AccountRepository.WithdrawMoneyAsync(userId, currencyCode, amount);
            }
        }
    }
}
