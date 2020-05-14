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
    }
}
