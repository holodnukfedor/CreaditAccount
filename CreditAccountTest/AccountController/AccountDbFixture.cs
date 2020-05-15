using System;
using Microsoft.Extensions.Configuration;
using CreditAccountDAL;
using System.Data.SqlClient;

namespace CreditAccountTest.AccountController
{
    public class AccountDbFixture : IDisposable
    {
        private bool _initialized;
        private string _connectionString;
        private User _testUser;
        private long _notExistsUserId;
        private DatabaseDeployer _databaseDeployer;
        public long TestUserId => _testUser.Id;
        public long NotExistUserId => _notExistsUserId;

        private void DeployDatabase(IConfigurationRoot configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _databaseDeployer = new DatabaseDeployer(_connectionString, "Sql\\Schema.sql", "Sql\\Procedures.sql");
            bool isDBCreated = _databaseDeployer.ExecuteReader("IF (OBJECT_ID ('dbo.GetBalance', 'P') IS NULL) SELECT 0 ELSE SELECT 1;", IsDatabaseCreated);
            if (!isDBCreated)
                _databaseDeployer.DeployAsync().Wait();
        }

        private User CreateUser()
        {
            string name = "Fedor" + Guid.NewGuid().ToString();
            _databaseDeployer.ExecuteNonQueryAsync($"INSERT INTO Users([Name]) VALUES('{name}')").Wait();
            long id = _databaseDeployer.ExecuteReader($"SELECT TOP 1 Id FROM Users WHERE [Name]='{name}'", ReadTestUserId);
            return new User(id, name);
        }

        private bool IsDatabaseCreated(SqlDataReader reader)
        {
            try
            {
                if (!reader.Read())
                    throw new Exception("Cant read database created bool");

                int result = reader.GetInt32(0);
                return result == 1;
            }
            finally
            {
                reader.Dispose();
            }
        }

        private long ReadTestUserId(SqlDataReader reader)
        {
            try
            {
                if (!reader.Read())
                    throw new Exception("Cant read test user Id");

                return reader.GetInt64(0);
            }
            finally
            {
                reader.Dispose();
            }
        }

        private void DeleteUser(long id)
        {
            _databaseDeployer.ExecuteNonQueryAsync($"DELETE FROM Users WHERE Id = {id}").Wait();
        }
        public AccountDbFixture()
        {

        }

        public void Initialize(IConfigurationRoot configuration)
        {
            if (_initialized)
                return;

            DeployDatabase(configuration);
            _testUser = CreateUser();
            _notExistsUserId = GetNotExistedUserId();
            _initialized = true;
        }

        public long GetNotExistedUserId()
        {
            User user = CreateUser();
            DeleteUser(user.Id);
            return user.Id;
        }

        public void Dispose()
        {
            DeleteUser(TestUserId);
        }
    }
}
