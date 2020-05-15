
namespace CreditAccountDAL
{
    public class AccountRepositoryErrorResultCreator
    {
        public static Result UserNotExists(long userId)
        {
            return Result.CreateError($"User doesn't exist. User id: {userId}");
        }

        public static Result MoneyLessOrEqualZero(decimal money)
        {
            return Result.CreateError($"Amount of money less or equal zero. Money: {money}");
        }

        public static Result NoCurrencyAccount(int currencyCode)
        {
            return Result.CreateError($"There is no currency account. CurrencyCode: {currencyCode}");
        }

        public static Result NotEnoughMoney(int currencyCode)
        {
            return Result.CreateError($"There is not enough money on currency account. CurrencyCode: {currencyCode}");
        }

        public static Result CurrenciesAreSame(int currencyCode)
        {
            return Result.CreateError($"Currencies are the same. CurrencyCode: {currencyCode}");
        }

        public static Result CreateConvertationError(decimal money)
        {
            return Result.CreateError($"Amount of money got by convertation less or equal zero. Money: {money}");
        }

        public static Result CurrencyNotExists(int currencyCode)
        {
            return Result.CreateError($"Currency code doesn't exist. Currency code: [{currencyCode}]");
        }
    }
}
