using BaseClass.Base.Interface;
using BaseClass.Database.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Database.Databases
{
    public class SQLServer : IDatabase
    {
        private readonly IBase baseClass;

        public SQLServer(IBase BaseClass)
        {
            baseClass = BaseClass;

            Username = BaseClass.DBUser;
            Password = ConvertToSecureString(BaseClass.DBPassword.ToCharArray());
            //DBName = BaseClass.DBName;
            //Server = BaseClass.DBServer;
        }

        private string? Username { get; set; } 
        private SecureString? Password { get; set; } 
        public string? DBName => baseClass.DBName;
        public string? Server => baseClass.DBServer;

        private IDbConnection CreateSqlConnection()
        {
            var credential = new SqlCredential(Username, Password);
            return new SqlConnection($"Server={Server};Database={DBName};", credential);
        }

        public bool ConnectionOpen()
        {
            return Task.Run(() => TestConnection()).Result;
        }

        private bool TestConnection()
        {
            bool res = false;

            using IDbConnection connection = CreateSqlConnection();
            try
            {
                connection.Open();
                var result = connection.QuerySingleOrDefault<DateTime>("SELECT GETDATE()");
                
                if(result != null)
                {
                    res = true;
                }
            }
            catch (DbException)
            {
                res = false;
            }

            return res;
        }

        public DateTime? GetCurrentDateTime()
        {
            using IDbConnection connection = CreateSqlConnection();
            connection.Open();
            return connection.QueryFirstOrDefault<DateTime?>("SELECT GETDATE()");
        }

        #region Synchronous Methods
        public int Execute(string sql, object? param = null, int? commandTimeout = null)
        {
            using var conn = CreateSqlConnection();
            conn.Open();
            return conn.Execute(sql, param, commandTimeout: commandTimeout);
        }
        public T? ExecuteScalar<T>(string sql, object? param = null, int? commandTimeout = null)
        {
            using var conn = CreateSqlConnection();
            conn.Open();
            return conn.ExecuteScalar<T>(sql, param, commandTimeout: commandTimeout);
        }
        public T? QueryFirstOrDefault<T>(string sql, object? param = null, int? commandTimeout = null)
        {
            using IDbConnection connection = CreateSqlConnection();
            connection.Open();
            return connection.QueryFirstOrDefault<T>(sql, param, commandTimeout: commandTimeout);
        }
        public T? QuerySingleOrDefault<T>(string sql, object? param = null, int? commandTimeout = null)
        {
            using IDbConnection connection = CreateSqlConnection();
            connection.Open();
            return connection.QuerySingleOrDefault<T>(sql, param, commandTimeout: commandTimeout);
        }
        public IEnumerable<T?> Query<T>(string sql, object? param = null, int? commandTimeout = null)
        {
            using IDbConnection connection = CreateSqlConnection();
            connection.Open();
            return connection.Query<T>(sql, param, commandTimeout: commandTimeout);
        }
        #endregion

        #region Asynchronous Methods
        public async Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null)
        {
            using IDbConnection connection = CreateSqlConnection();
            connection.Open();
            CommandDefinition command = new(sql, param, commandTimeout: commandTimeout, cancellationToken: cancellationToken);
            return await connection.ExecuteAsync(command);
        }
        public async Task<T?> ExecuteScalarAsync<T>(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null)
        {
            using IDbConnection connection = CreateSqlConnection();
            connection.Open();
            CommandDefinition command = new(sql, param, commandTimeout: commandTimeout, cancellationToken: cancellationToken);
            return (T?)await connection.ExecuteScalarAsync(command);
        }
        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null)
        {
            using IDbConnection connection = CreateSqlConnection();
            connection.Open();
            CommandDefinition command = new(sql, param, commandTimeout: commandTimeout, cancellationToken: cancellationToken);
            return await connection.QueryFirstOrDefaultAsync<T>(command);
        }
        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null)
        {
            using IDbConnection connection = CreateSqlConnection();
            connection.Open();
            CommandDefinition command = new(sql, param, commandTimeout: commandTimeout, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<T>(command);
        }
        public async Task<IEnumerable<T?>> QueryAsync<T>(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null)
        {
            using IDbConnection connection = CreateSqlConnection();
            connection.Open();
            CommandDefinition command = new(sql, param, commandTimeout: commandTimeout, cancellationToken: cancellationToken);
            return await connection.QueryAsync<T>(command);
        }
        #endregion

        #region Misc
        private SecureString ConvertToSecureString(char[] value)
        {
            if (value == null || value.Length == 0)
                throw new ArgumentNullException(nameof(value));

            SecureString secureData = new SecureString();
            foreach (char c in value)
            {
                secureData.AppendChar(c);
            }
            secureData.MakeReadOnly();
            Array.Clear(value, 0, value.Length); // Clear plaintext data
            return secureData;
        }
        #endregion
    }
}
