

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
namespace net.atos.daf.ct2.data
{
    /// <summary>
    /// Class to create a Sql Executor
    /// </summary>
    public class PgSQLDataAccess : IDataAccess
    {
        #region Members

        /// <summary>
        /// The SQL connection object
        /// </summary>
        public IDbConnection Connection { get; set; }

        #endregion Members

        /// <summary>
        /// Constructor for the SQL Executor
        /// </summary>
        /// <param name="connection"></param>
        public PgSQLDataAccess(IDbConnection dbconnection)
        {
            Connection = dbconnection;
        }

        public PgSQLDataAccess(string connectionString)
        {
            Connection = new NpgsqlConnection(connectionString);
        }

        #region Sync Methods

        public int Execute(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return Connection.Execute(
                sql,
                param,
                transaction,
                commandTimeout,
                commandType);
        }

        public IEnumerable<dynamic> Query(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return Connection.Query(
                sql,
                param,
                transaction,
                buffered,
                commandTimeout,
                commandType);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return Connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return Connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql,
            Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public IEnumerable<TReturn> Query<TReturn>(string sql, Type[] types,
            Func<object[], TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            // return connection.Query(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
            return Connection.Query(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public dynamic QueryFirst(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryFirst(sql, param, transaction, commandTimeout, commandType);
        }

        public dynamic QueryFirstOrDefault(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType);
        }

        public dynamic QuerySingle(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QuerySingle(sql, param, transaction, commandTimeout, commandType);
        }

        public dynamic QuerySingleOrDefault(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefault(sql, param, transaction, commandTimeout, commandType);
        }

        public object QueryFirst(Type type, string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirst(type, sql, param, transaction, commandTimeout, commandType);
        }

        public object QueryFirstOrDefault(Type type, string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirstOrDefault(type, sql, param, transaction, commandTimeout,
                commandType);
        }

        public object QuerySingle(Type type, string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingle(type, sql, param, transaction, commandTimeout, commandType);
        }

        public object QuerySingleOrDefault(Type type, string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefault(type, sql, param, transaction, commandTimeout,
                commandType);
        }

        public T QueryFirst<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T QuerySingle<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T QuerySingleOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public IEnumerable<T> Query<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return Connection.Query<T>(sql,
                param,
                transaction,
                buffered,
                commandTimeout,
                commandType);
        }

        // public IGridReader QueryMultiple(
        //     string sql,
        //     object param = null,
        //     IDbTransaction transaction = null,
        //     int? commandTimeout = default(int?),
        //     CommandType? commandType = default(CommandType?))
        // {
        //     var reader = connection.QueryMultiple(sql, param, transaction, commandTimeout, commandType);
        //     return new GridReaderAbstraction(reader);
        // }

        // public int Execute(CommandDefinition command)
        // {
        //     return connection.Execute(command);
        // }

        public object ExecuteScalar(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.ExecuteScalar(sql, param, transaction, commandTimeout, commandType);
        }

        public T ExecuteScalar<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
        }

        // public object ExecuteScalar(CommandDefinition command)
        // {
        //     return connection.ExecuteScalar(command);
        // }

        // public T ExecuteScalar<T>(CommandDefinition command)
        // {
        //     return connection.ExecuteScalar<T>(command);
        // }

        public IDataReader ExecuteReader(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.ExecuteReader(sql, param, transaction, commandTimeout, commandType);
        }

        // public IDataReader ExecuteReader(CommandDefinition command)
        // {
        //     return connection.ExecuteReader(command);
        // }

        // public IDataReader ExecuteReader(CommandDefinition command, CommandBehavior commandBehavior)
        // {
        //     return connection.ExecuteReader(command, commandBehavior);
        // }

        public IEnumerable<object> Query(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.Query(type, sql, param, transaction, buffered, commandTimeout, commandType);
        }

        // public IEnumerable<T> Query<T>(CommandDefinition command)
        // {
        //     return connection.Query<T>(command);
        // }

        // public T QueryFirst<T>(CommandDefinition command)
        // {
        //     return connection.QueryFirst<T>(command);
        // }

        // public T QueryFirstOrDefault<T>(CommandDefinition command)
        // {
        //     return connection.QueryFirstOrDefault<T>(command);
        // }

        // public T QuerySingle<T>(CommandDefinition command)
        // {
        //     return connection.QuerySingle<T>(command);
        // }

        // public T QuerySingleOrDefault<T>(CommandDefinition command)
        // {
        //     return connection.QuerySingleOrDefault<T>(command);
        // }

        // public IGridReader QueryMultiple(CommandDefinition command)
        // {
        //     var reader = connection.QueryMultiple(command);
        //     return new GridReaderAbstraction(reader);
        // }

        #endregion Sync Methods

        // #region Additional Extensions

        // /// <summary>
        // /// Perform the Query and then trim any strings that are returned
        // /// NOTE: There could be a performance hit when doing this
        // /// </summary>
        // public IEnumerable<T> QueryAndTrimResults<T>(
        //     string sql,
        //     object param = null,
        //     IDbTransaction transaction = null,
        //     bool buffered = true,
        //     int? commandTimeout = null,
        //     CommandType? commandType = null)
        // {
        //     return AdditionalDapper.Query<T>(connection, sql, param, transaction, buffered, commandTimeout, commandType);
        // }

        // /// <summary>
        // /// Perform the Query and then trim any strings that are returned
        // /// NOTE: There could be a performance hit when doing this
        // /// </summary>
        // public IEnumerable<TReturn> QueryAndTrimResults<TFirst, TSecond, TReturn>(
        //     string sql,
        //     Func<TFirst, TSecond, TReturn> map,
        //     object param = null,
        //     IDbTransaction transaction = null,
        //     bool buffered = true,
        //     string splitOn = "Id",
        //     int? commandTimeout = null,
        //     CommandType? commandType = null)
        // {
        //     return AdditionalDapper.Query(connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        // }

        // #endregion Additional Extensions

        #region Async Methods

        public Task<int> ExecuteAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return Connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<IEnumerable<dynamic>> QueryAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return Connection.QueryAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return Connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        // public async Task<IGridReader> QueryMultipleAsync(
        //     string sql,
        //     object param = null,
        //     IDbTransaction transaction = null,
        //     int? commandTimeout = default(int?),
        //     CommandType? commandType = default(CommandType?))
        // {
        //     var reader = await connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType).ConfigureAwait(false);
        //     return new GridReaderAbstraction(reader);
        // }

        // public Task<IEnumerable<dynamic>> QueryAsync(CommandDefinition command)
        // {
        //     return connection.QueryAsync(command);
        // }

        // public Task<dynamic> QueryFirstAsync(CommandDefinition command)
        // {
        //     return connection.QueryFirstAsync(command);
        // }

        // public Task<dynamic> QueryFirstOrDefaultAsync(CommandDefinition command)
        // {
        //     return connection.QueryFirstOrDefaultAsync(command);
        // }

        // public Task<dynamic> QuerySingleAsync(CommandDefinition command)
        // {
        //     return connection.QuerySingleAsync(command);
        // }

        // public Task<dynamic> QuerySingleOrDefaultAsync(CommandDefinition command)
        // {
        //     return connection.QuerySingleOrDefaultAsync(command);
        // }

        public Task<T> QueryFirstAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> QuerySingleAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<IEnumerable<object>> QueryAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<object> QueryFirstAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryFirstAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<object> QueryFirstOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryFirstOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<object> QuerySingleAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QuerySingleAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<object> QuerySingleOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<dynamic> QuerySingleOrDefaultAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefaultAsync(sql, param, transaction, commandTimeout, commandType);
        }

        // public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
        // {
        //     return connection.QueryAsync<T>(command);
        // }

        // public Task<T> QueryFirstAsync<T>(CommandDefinition command)
        // {
        //     return connection.QueryFirstAsync<T>(command);
        // }

        // public Task<T> QueryFirstOrDefaultAsync<T>(CommandDefinition command)
        // {
        //     return connection.QueryFirstOrDefaultAsync<T>(command);
        // }

        // public Task<T> QuerySingleAsync<T>(CommandDefinition command)
        // {
        //     return connection.QuerySingleAsync<T>(command);
        // }

        // public Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command)
        // {
        //     return connection.QuerySingleOrDefaultAsync<T>(command);
        // }

        // public Task<int> ExecuteAsync(CommandDefinition command)
        // {
        //     return connection.ExecuteAsync(command);
        // }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        // public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
        //     CommandDefinition command,
        //     Func<TFirst, TSecond, TReturn> map,
        //     string splitOn = "Id")
        // {
        //     return connection.QueryAsync(command, map, splitOn);
        // }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        // public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
        //     CommandDefinition command,
        //     Func<TFirst, TSecond, TThird, TReturn> map,
        //     string splitOn = "Id")
        // {
        //     return connection.QueryAsync(command, map, splitOn);
        // }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        // public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        //     CommandDefinition command,
        //     Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        //     string splitOn = "Id")
        // {
        //     return connection.QueryAsync(command, map, splitOn);
        // }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        // public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        //     CommandDefinition command,
        //     Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        //     string splitOn = "Id")
        // {
        //     return connection.QueryAsync(command, map, splitOn);
        // }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        // public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        //     CommandDefinition command,
        //     Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        //     string splitOn = "Id")
        // {
        //     return connection.QueryAsync(command, map, splitOn);
        // }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        // public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CommandDefinition command,
        //     Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        //     string splitOn = "Id")
        // {
        //     return connection.QueryAsync(command, map, splitOn);
        // }

        public Task<IEnumerable<TReturn>> QueryAsync<TReturn>(
            string sql,
            Type[] types,
            Func<object[], TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryAsync(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        // public async Task<IGridReader> QueryMultipleAsync(CommandDefinition command)
        // {
        //     var reader = await connection.QueryMultipleAsync(command).ConfigureAwait(false);
        //     return new GridReaderAbstraction(reader);
        // }

        public Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType);
        }

        // public Task<IDataReader> ExecuteReaderAsync(CommandDefinition command)
        // {
        //     return connection.ExecuteReaderAsync(command);
        // }

        // public Task<IDataReader> ExecuteReaderAsync(CommandDefinition command, CommandBehavior commandBehavior)
        // {
        //     return connection.ExecuteReaderAsync(command, commandBehavior);
        // }

        public Task<object> ExecuteScalarAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> ExecuteScalarAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        // public Task<object> ExecuteScalarAsync(CommandDefinition command)
        // {
        //     return connection.ExecuteScalarAsync(command);
        // }

        // public Task<T> ExecuteScalarAsync<T>(CommandDefinition command)
        // {
        //     return connection.ExecuteScalarAsync<T>(command);
        // }

        // public Task<IEnumerable<object>> QueryAsync(Type type, CommandDefinition command)
        // {
        //     return connection.QueryAsync(type, command);
        // }

        // public Task<object> QueryFirstAsync(Type type, CommandDefinition command)
        // {
        //     return connection.QueryFirstAsync(type, command);
        // }

        // public Task<object> QueryFirstOrDefaultAsync(Type type, CommandDefinition command)
        // {
        //     return connection.QueryFirstOrDefaultAsync(type, command);
        // }

        // public Task<object> QuerySingleAsync(Type type, CommandDefinition command)
        // {
        //     return connection.QuerySingleAsync(type, command);
        // }

        // public Task<object> QuerySingleOrDefaultAsync(Type type, CommandDefinition command)
        // {
        //     return connection.QuerySingleAsync(type, command);
        // }

        public Task<dynamic> QueryFirstAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryFirstAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<dynamic> QueryFirstOrDefaultAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QueryFirstOrDefaultAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<dynamic> QuerySingleAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.QuerySingleAsync(sql, param, transaction, commandTimeout, commandType);
        }

        #endregion Async Methods

        #region Implementation of IDisposable

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Connection.Dispose();
        }
        // START : Moq test changes
        //public Task<T> ExecuteScalarAsync<T>(string sql, object param = null)
        //{
        //    return connection.ExecuteScalarAsync<T>(sql, param);
        //}

        //public int Execute(string sql, object param = null)
        //{
        //    return connection.Execute(sql, param);
        //}

        //public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        //{
        //    return connection.QueryAsync<T>(sql, param);
        //}
        // END : Moq test changes
        #endregion Implementation of IDisposable
    }
}