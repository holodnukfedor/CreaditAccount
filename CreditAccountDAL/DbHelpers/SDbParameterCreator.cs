using System.Data.SqlClient;

namespace CreditAccountDAL
{
    public static class SDbParameterCreator
    {
        public static SqlParameter CreateBigInt(this SqlCommand command, string name, long value)
        {
            SqlParameter parameter = command.CreateParameter();
            parameter.DbType = System.Data.DbType.Int64;
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        public static SqlParameter CreateInt(this SqlCommand command, string name, int value)
        {
            SqlParameter parameter = command.CreateParameter();
            parameter.DbType = System.Data.DbType.Int32;
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        public static SqlParameter CreateMoney(this SqlCommand command, string name, decimal value)
        {
            SqlParameter parameter = command.CreateParameter();
            parameter.DbType = System.Data.DbType.Currency;
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }
    }
}
