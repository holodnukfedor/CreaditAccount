using System.Threading.Tasks;
using CreditAccountDAL;

namespace CreditAccountBLL
{
    public interface IAccountService
    {
        Task<Result<Balance[]>> GetBalanceAsync(long userId);
        Task<Result> ChangeCurrencyAsync(long userId, decimal amount, int from, int to);
        Task<Result> PutMoneyAsync(long userId, decimal amount, int currencyCode);
        Task<Result> WithdrawMoneyAsync(long userId, decimal amount, int currencyCode);
    }
}
