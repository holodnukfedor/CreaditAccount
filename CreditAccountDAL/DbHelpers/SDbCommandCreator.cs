using System.Data.SqlClient;

namespace CreditAccountDAL
{
    public static class SDbCommandCreator
    {
        public static SqlCommand CreateProcedureCommand(this SqlConnection connection, string procedureName)
        {
            SqlCommand procedureCommand = connection.CreateCommand();
            procedureCommand.CommandText = procedureName;
            procedureCommand.CommandType = System.Data.CommandType.StoredProcedure;
            return procedureCommand;
        }

        public static SqlCommand CreateTextCommand(this SqlConnection connection, string sqlText)
        {
            SqlCommand procedureCommand = connection.CreateCommand();
            procedureCommand.CommandText = sqlText;
            procedureCommand.CommandType = System.Data.CommandType.Text;
            return procedureCommand;
        }
    }
}
