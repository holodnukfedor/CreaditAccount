using System;
using System.Collections.Generic;
using CurrencyCodesResolver;
using System.Threading.Tasks;
using CreditAccountDAL;
using System.Linq;

namespace CreditAccountBLL
{
    public class EFAccountService : IAccountService
    {
        private IDbManager _dbManager;
        private ICurrencyCodesResolver _currencyCodesResolver;
        private ICurrencyConverterService _currencyConverterService;

        public EFAccountService(IDbManager dbManager, ICurrencyCodesResolver currencyCodesResolver, ICurrencyConverterService currencyConverterService)
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

        private Result<decimal> CreateErrorNoCurrencyAccount(int currencyCode)
        {
            return Result<decimal>.CreateError($"User has no account with currency. Currency code: [{_currencyCodesResolver.Resolve(currencyCode)}]");
        }

        private Result<decimal> CreateErrorNotEnoughMoney(int currencyCode, Account account)
        {
            return Result<decimal>.CreateError($"You have not enough money in current currency. Balance: [{account.Money}], currency code: [{_currencyCodesResolver.Resolve(currencyCode)}]");
        }

        private void ThrowIfUserNotExist(long userId, User user)
        {
            if (user == null)
                throw new Exception($"User doesn't exists. UserId: [{userId}]");
        }

        async public Task<Result<Dictionary<int, decimal>>> ChangeCurrency(long userId, decimal amount, int from, int to)
        {
            ThrowIfNegativeOrZeroAmountOfMoney(amount);
            ThrowIfCurrencyNoExist(from);
            ThrowIfCurrencyNoExist(to);

            using (var transaction = _dbManager.AccountContext.Database.BeginTransaction())
            {
                User user = await _dbManager.Users.Get(userId).AsTask();
                ThrowIfUserNotExist(userId, user);
                List<Account> accounts = user.Accounts.Where(x => x.CurrencyCode == from || x.CurrencyCode == to).ToList();
                Account fromAccount = accounts.FirstOrDefault(x => x.CurrencyCode == from);

                if (fromAccount == null)
                    return Result<Dictionary<int, decimal>>.CreateError($"User has no account with currency. UserId: [{userId}]. From currency: [{from}]");

                if (fromAccount.Money < amount)
                    Result<Dictionary<int, decimal>>.CreateError($"You have not enough money in from currency. Balance: [{fromAccount.Money}], From currency: [{from}]");

                fromAccount.Money -= amount;
                _dbManager.Accounts.Update(fromAccount);

                Result<decimal> convertionResult = _currencyConverterService.Convert(amount, from, to);
                if (!convertionResult.IsSuccess)
                    return Result<Dictionary<int, decimal>>.CreateError(convertionResult.ErrorMessage);

                Account toAccount = accounts.FirstOrDefault(x => x.CurrencyCode == to);

                if (toAccount == null)
                {
                    toAccount = new Account { UserId = userId, CurrencyCode = to, Money = amount };
                    _dbManager.Accounts.Create(toAccount);
                }
                else
                {
                    toAccount.Money += convertionResult.Data;
                    _dbManager.Accounts.Update(toAccount);
                }
                await _dbManager.SaveChanges();
                transaction.Commit();
                Dictionary<int, decimal> result = new Dictionary<int, decimal>
                    {{ from, fromAccount.Money }, { to, toAccount.Money }};
                return Result<Dictionary<int, decimal>>.CreateSuccess(result);
            }
        }

        async public Task<Result<Dictionary<int, decimal>>> GetBalance(long userId)
        {
            using (var transaction = _dbManager.AccountContext.Database.BeginTransaction())
            {
                User user = await _dbManager.Users.Get(userId).AsTask();
                ThrowIfUserNotExist(userId, user);
                Dictionary<int, decimal> balance = user.Accounts.Where(x => x.UserId == userId).ToDictionary(x => x.CurrencyCode, y => y.Money);
                transaction.Commit();
                return Result<Dictionary<int, decimal>>.CreateSuccess(balance);
            }
        }

        async public Task<Result<decimal>> PutMoney(long userId, decimal amount, int currencyCode)
        {
            ThrowIfNegativeOrZeroAmountOfMoney(amount);
            ThrowIfCurrencyNoExist(currencyCode);

            using (var transaction = _dbManager.AccountContext.Database.BeginTransaction())
            {
                User user = await _dbManager.Users.Get(userId).AsTask();
                ThrowIfUserNotExist(userId, user);
                Account account = user.Accounts.FirstOrDefault(x => x.CurrencyCode == currencyCode);

                if (account == null)
                {
                    account = new Account { UserId = userId, CurrencyCode = currencyCode, Money = amount };
                    _dbManager.Accounts.Create(account);
                }
                else
                {
                    account.Money += amount;
                    _dbManager.Accounts.Update(account);
                }

                await _dbManager.SaveChanges();
                transaction.Commit();
                return Result<decimal>.CreateSuccess(account.Money);
            }
        }

        async public Task<Result<decimal>> WithdrawMoney(long userId, decimal amount, int currencyCode)
        {
            ThrowIfNegativeOrZeroAmountOfMoney(amount);
            ThrowIfCurrencyNoExist(currencyCode);

            using (var transaction = _dbManager.AccountContext.Database.BeginTransaction())
            {
                User user = await _dbManager.Users.Get(userId).AsTask();
                ThrowIfUserNotExist(userId, user);
                Account account = user.Accounts.FirstOrDefault(x => x.CurrencyCode == currencyCode);

                if (account == null)
                    return CreateErrorNoCurrencyAccount(currencyCode);

                if (account.Money < amount)
                    return CreateErrorNotEnoughMoney(currencyCode, account);

                account.Money -= amount;
                await _dbManager.SaveChanges();
                transaction.Commit();
                return Result<decimal>.CreateSuccess(account.Money);
            }
        }
    }
}
