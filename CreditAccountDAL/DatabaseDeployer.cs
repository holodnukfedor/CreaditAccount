using System;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CreditAccountDAL
{
    public class DatabaseDeployer : IDisposable
    {
        private string _pathToSchema = "..\\CreditAccountDAL\\Sql\\Procedures.sql";
        private string _pathToProcedures = "..\\CreditAccountDAL\\Sql\\Schema.sql";
        private SqlConnection _connection;

        private static async Task ExecuteNonQueryAsync(SqlConnection connection, string sql)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.Text;
            await command.ExecuteNonQueryAsync();
        }

        public static async Task ExecuteSqlFileAsync(SqlConnection connection, string path)
        {
            string text = await File.ReadAllTextAsync(path);
            string[] batches = text.Split("GO");
            foreach (var batch in batches)
            {
                await ExecuteNonQueryAsync(connection, batch);
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
                SqlCommand command = _connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;
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

        async public Task DeployAsync()
        {
            try
            {
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
