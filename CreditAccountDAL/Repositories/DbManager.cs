using System;
using System.Data.SqlClient;

namespace CreditAccountDAL
{
    public class DbManager : IDbManager
    {
        private SqlConnection _dbConnection;
        public IAccountRepository AccountRepository { get; }

        public DbManager(string connectionString)
        {
            _dbConnection = new SqlConnection(connectionString);
            AccountRepository = new AccountRepository(_dbConnection);
        }

        public void CloseConnection()
        {
            _dbConnection.Close();
        }

        public void Dispose()
        {
            _dbConnection.Close();
        }

        public IDisposable OpenConnection()
        {
            _dbConnection.Open();
            return this;
        }
    }
}
