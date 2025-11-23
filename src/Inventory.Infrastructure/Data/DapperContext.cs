using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Inventory.Infrastructure.Data;

/// <summary>
/// Dapper context for executing raw SQL queries and stored procedures
/// Used for performance-critical operations
/// </summary>
public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    /// <summary>
    /// Execute a stored procedure and return results
    /// </summary>
    public async Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(
        string storedProcedure, 
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<T>(
            storedProcedure, 
            parameters, 
            commandType: System.Data.CommandType.StoredProcedure);
    }

    /// <summary>
    /// Execute a stored procedure without returning results
    /// </summary>
    public async Task<int> ExecuteStoredProcedureAsync(
        string storedProcedure, 
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync(
            storedProcedure, 
            parameters, 
            commandType: System.Data.CommandType.StoredProcedure);
    }

    /// <summary>
    /// Execute a raw SQL query and return results
    /// </summary>
    public async Task<IEnumerable<T>> QueryAsync<T>(
        string sql, 
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    /// <summary>
    /// Execute a raw SQL command
    /// </summary>
    public async Task<int> ExecuteAsync(
        string sql, 
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync(sql, parameters);
    }

    /// <summary>
    /// Execute a stored procedure and return a single result
    /// </summary>
    public async Task<T?> QuerySingleOrDefaultStoredProcedureAsync<T>(
        string storedProcedure, 
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<T>(
            storedProcedure, 
            parameters, 
            commandType: System.Data.CommandType.StoredProcedure);
    }
}
