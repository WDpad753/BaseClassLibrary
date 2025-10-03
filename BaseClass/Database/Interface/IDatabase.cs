using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Database.Interface
{
    public interface IDatabase
    {
        #region Properties
        //string ConnectionString { get; }
        bool ConnectionOpen();
        #endregion

        #region DBMethods
        // --------- Non-Query Methods
        int Execute(string sql, object? param = null, int? commandTimeout = null);
        Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null);

        // --------- Scalar Methods
        T? ExecuteScalar<T>(string sql, object? param = null, int? commandTimeout = null);
        Task<T?> ExecuteScalarAsync<T>(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null);

        // --------- QueryFirst
        T? QueryFirstOrDefault<T>(string sql, object? param = null, int? commandTimeout = null);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null);

        // --------- QuerySingle
        T? QuerySingleOrDefault<T>(string sql, object? param = null, int? commandTimeout = null);
        Task<T?> QuerySingleOrDefaultAsync<T>(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null);

        // --------- Query Multiple Rows
        IEnumerable<T?> Query<T>(string sql, object? param = null, int? commandTimeout = null);
        Task<IEnumerable<T?>> QueryAsync<T>(string sql, CancellationToken cancellationToken = default, object? param = null, int? commandTimeout = null);
        #endregion

        #region Verification
        // --------- DB Check
        DateTime? GetCurrentDateTime();
        #endregion
    }
}
