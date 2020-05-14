using System.Threading.Tasks;

namespace CreditAccountDAL
{
    public interface IAccountRepository
    {
        Task<Result<Balance[]>> GetBalanceAsync(long userId);
        Task<Result> PutMoneyAsync(long userId, int currencyCode, decimal money);
        Task<Result> WithdrawMoneyAsync(long userId, int currencyCode, decimal money);
        Task<Result> ChangeCurrencyAsync(long userId, int fromCurrencyCode, decimal fromCurrencyMoney, int toCurrencyCode, decimal toCurrencyMoney);
    }
}
