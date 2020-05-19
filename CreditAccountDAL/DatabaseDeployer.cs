using System;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text;

namespace CreditAccountDAL
{
    public class DatabaseDeployer : IDisposable
    {
        private const string _batchSeparator = "GO";
        private string _pathToSchema = "..\\CreditAccountDAL\\Sql\\Schema.sql";
        private string _pathToProcedures = "..\\CreditAccountDAL\\Sql\\Procedures.sql";
        private SqlConnection _connection;

        private static async Task ExecuteNonQueryAsync(SqlConnection connection, string sql)
        {
            SqlCommand command = connection.CreateTextCommand(sql);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task ExecuteSqlFileAsync(SqlConnection connection, string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                StringBuilder batchBuilder = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    if (String.Equals(line.Trim(), _batchSeparator, StringComparison.OrdinalIgnoreCase))
                    {
                        string batch = batchBuilder.ToString();
                        await ExecuteNonQueryAsync(connection, batch);
                        batchBuilder.Clear();
                    }
                    else
                    {
                        batchBuilder.Append(line).Append(Environment.NewLine);
                    }
                }

            }
        }

        public async Task ExecuteNonQueryAsync(string sql)
        {
            try
            {
                _connection.Open();
                await ExecuteNonQueryAsync(_connection, sql);
            }
            finally
            {
                _connection.Close();
            }
        }

        public T ExecuteReader<T>(string sql, Func<SqlDataReader, T> read)
        {
            try
            {
                _connection.Open();
                SqlCommand command = _connection.CreateTextCommand(sql);
                SqlDataReader reader = command.ExecuteReader();
                return read(reader);
            }
            finally
            {
                _connection.Close();
            }
        }

        public DatabaseDeployer(string connectionString, string pathToSchema, string pathToProcedures)
        {
            if (String.IsNullOrEmpty(pathToSchema))
                throw new Exception("Path to schema cant be empty string");

            if (String.IsNullOrEmpty(pathToProcedures))
                throw new Exception("Path to procedures cant be empty string");

            _pathToSchema = pathToSchema;
            _pathToProcedures = pathToProcedures;
            _connection = new SqlConnection(connectionString);
        }

        public DatabaseDeployer(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public async Task CreateDbIfNotExistsAsync()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(_connection.ConnectionString);
            string dbName = connectionStringBuilder.InitialCatalog;
            connectionStringBuilder.InitialCatalog = String.Empty;
            string connectionStrWithoutDb = connectionStringBuilder.ConnectionString;
            SqlConnection connWithoutDb = new SqlConnection(connectionStrWithoutDb);

            try
            {
                connWithoutDb.Open();
                SqlCommand createDbCommand = connWithoutDb.CreateTextCommand($"IF (DB_ID('{dbName}') IS NULL) CREATE DATABASE {dbName}");
                await createDbCommand.ExecuteNonQueryAsync();
            }
            finally
            {
                connWithoutDb.Close();
            }
        }

        async public Task DeployAsync()
        {
            try
            {
                await CreateDbIfNotExistsAsync();
                _connection.Open();
                await ExecuteSqlFileAsync(_connection, _pathToSchema);
                await ExecuteSqlFileAsync(_connection, _pathToProcedures);
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
