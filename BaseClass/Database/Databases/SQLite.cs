using BaseClass.Base.Interface;
using BaseClass.Database.Interface;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace BaseClass.Database.Databases
{
    public class SQLite : IDatabase
    {
        private readonly IBaseSettings baseClass;
        
        public SQLite(IBaseSettings BaseClass) 
        {
            baseClass = BaseClass;
        }

        public string? ConnectionString => baseClass.DBPath;
        public string? Password => baseClass.DBPassword;

        private IDbConnection CreateSqlConnection()
        {
            string? connectionString = null;
            Batteries.Init();

            if(Password != null)
            {
                connectionString = new SqliteConnectionStringBuilder(ConnectionString)
                {
                    Mode = SqliteOpenMode.ReadWriteCreate,
                    Password = Password
                }.ToString();
            }
            else
            {
                connectionString = ConnectionString;
            }

            if (connectionString == null)
                throw new ArgumentNullException($"Connection string can not be null.{nameof(connectionString)}");

            return new SqliteConnection(connectionString);
        }

        public bool ConnectionOpen()
        {
            return TestConnection();
        }

        private bool TestConnection()
        {
            bool res = false;

            if (ConnectionString == null)
                throw new ArgumentNullException("Database Path value does not exist.");

            if(!File.Exists(ConnectionString))
                throw new FileNotFoundException("Database file does not exist.", ConnectionString);

            using IDbConnection connection = CreateSqlConnection();
            try
            {
                connection.Open();
                var result = connection.QueryFirstOrDefault<DateTime?>("SELECT datetime('now');");

                if (result != null)
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
            return connection.QueryFirstOrDefault<DateTime?>("SELECT datetime('now');");
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
