using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CreditAccountDAL
{
    public class AccountRepository : IAccountRepository
    {
        private SqlConnection _connection;

        public AccountRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        private Result<T> CreateUserNotExistsResult<T>(long userId)
        {
            return Result<T>.CreateError($"User doesn't exist. User id: {userId}");
        }

        private SqlParameter GetAccountOperationStatusParameter()
        {
            SqlParameter resultStatus = new SqlParameter()
            {
                ParameterName = "@resultStatus",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };
            return resultStatus;
        }

        private void FillGetBalanceParameters(SqlCommand procedure, long userId)
        {
            procedure.Parameters.Add(procedure.CreateBigInt("@userId", userId));
        }

        private void FillPutWithdrawMoneyParameters(SqlCommand procedure, long userId, int currencyCode, decimal money)
        {
            procedure.Parameters.Add(procedure.CreateBigInt("@userId", userId));
            procedure.Parameters.Add(procedure.CreateInt("@currencyCode", currencyCode));
            procedure.Parameters.Add(procedure.CreateMoney("@money", money));
        }

        private void FillChangeCurrencyParameters(SqlCommand procedure, long userId, int fromCurrencyCode, decimal fromCurrencyMoney, int toCurrencyCode, decimal toCurrencyMoney)
        {
            procedure.Parameters.Add(procedure.CreateBigInt("@userId", userId));
            procedure.Parameters.Add(procedure.CreateInt("@fromCurrencyCode", fromCurrencyCode));
            procedure.Parameters.Add(procedure.CreateMoney("@fromCurrencyMoney", fromCurrencyMoney));
            procedure.Parameters.Add(procedure.CreateInt("@toCurrencyCode", toCurrencyCode));
            procedure.Parameters.Add(procedure.CreateMoney("@toCurrencyMoney", toCurrencyMoney));
        }

        private Balance ReadBalance(SqlDataReader reader)
        {
            int currencyCode = reader.GetInt32(0);
            decimal money = reader.GetDecimal(1);
            return new Balance(currencyCode, money);
        }

        private EAccountOperationStatus GetStatus(SqlParameter param)
        {
            string resultStr = param.Value.ToString();
            if (String.IsNullOrEmpty(resultStr))
                return EAccountOperationStatus.Success;

            return Enum.Parse<EAccountOperationStatus>(resultStr);
        }

        private Result ProcessStatus(EAccountOperationStatus status, long userId, decimal money, int currencyCode, decimal convertedMoney)
        {
            switch (status)
            {
                case EAccountOperationStatus.Success:
                    return Result.CreateSuccess();
                case EAccountOperationStatus.UserNotExists:
                    return AccountRepositoryErrorResultCreator.UserNotExists(userId);
                case EAccountOperationStatus.MoneyLessOrEqualZero:
                    return AccountRepositoryErrorResultCreator.MoneyLessOrEqualZero(money);
                case EAccountOperationStatus.NoCurrencyAccount:
                    return AccountRepositoryErrorResultCreator.NoCurrencyAccount(currencyCode);
                case EAccountOperationStatus.NotEnoughMoney:
                    return AccountRepositoryErrorResultCreator.NotEnoughMoney(currencyCode);
                case EAccountOperationStatus.MoneyLessOrEqualZeroToDestinationAccount:
                    return AccountRepositoryErrorResultCreator.CreateConvertationError(convertedMoney);
                case EAccountOperationStatus.CurrenciesAreSame:
                    return AccountRepositoryErrorResultCreator.CurrenciesAreSame(currencyCode);
                default:
                    throw new NotImplementedException($"Unknown status. AccountOperationStatus [{status}]");
            }
        }
        
        private async Task<Result> ExecuteGetWithdrawOperation(long userId, int currencyCode, decimal money, SqlCommand procedure)
        {
            FillPutWithdrawMoneyParameters(procedure, userId, currencyCode, money);
            SqlParameter resultStatus = GetAccountOperationStatusParameter();
            procedure.Parameters.Add(resultStatus);
            await procedure.ExecuteNonQueryAsync();
            EAccountOperationStatus status = GetStatus(resultStatus);
            return ProcessStatus(status, userId, money, currencyCode, default);
        }

        public async Task<Result<Balance[]>> GetBalanceAsync(long userId)
        {
            SqlCommand procedure =_connection.CreateProcedureCommand("GetBalance");
            FillGetBalanceParameters(procedure, userId);
            SqlParameter resultStatus = GetAccountOperationStatusParameter();
            procedure.Parameters.Add(resultStatus);
            await procedure.ExecuteNonQueryAsync();
            EAccountOperationStatus spStatus = GetStatus(resultStatus);

            if (spStatus == EAccountOperationStatus.UserNotExists)
                return CreateUserNotExistsResult<Balance[]>(userId);

            using (SqlDataReader reader = await procedure.ExecuteReaderAsync())
            {
                List<Balance> balances = new List<Balance>();
                while (await reader.ReadAsync())
                {
                    Balance balance = ReadBalance(reader);
                    balances.Add(balance);
                }
                return Result<Balance[]>.CreateSuccess(balances.ToArray());
            }
        }

        public async Task<Result> PutMoneyAsync(long userId, int currencyCode, decimal money)
        {
            SqlCommand procedure = _connection.CreateProcedureCommand("PutMoney");
            return await ExecuteGetWithdrawOperation(userId, currencyCode, money, procedure);
        }

        public async Task<Result> WithdrawMoneyAsync(long userId, int currencyCode, decimal money)
        {
            SqlCommand procedure = _connection.CreateProcedureCommand("WithdrawMoney");
            return await ExecuteGetWithdrawOperation(userId, currencyCode, money, procedure);
        }

        public async Task<Result> ChangeCurrencyAsync(long userId, int fromCurrencyCode, decimal fromCurrencyMoney, int toCurrencyCode, decimal toCurrencyMoney)
        {
            SqlCommand procedure = _connection.CreateProcedureCommand("ChangeCurrency");
            FillChangeCurrencyParameters(procedure, userId, fromCurrencyCode, fromCurrencyMoney, toCurrencyCode, toCurrencyMoney);
            SqlParameter resultStatus = GetAccountOperationStatusParameter();
            procedure.Parameters.Add(resultStatus);
            await procedure.ExecuteNonQueryAsync();
            EAccountOperationStatus status = GetStatus(resultStatus);
            return ProcessStatus(status, userId, fromCurrencyMoney, fromCurrencyCode, toCurrencyMoney);
        }
    }
}
