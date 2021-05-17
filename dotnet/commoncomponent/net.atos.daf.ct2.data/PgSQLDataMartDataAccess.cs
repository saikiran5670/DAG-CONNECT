using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace net.atos.daf.ct2.data
{
    public class PgSQLDataMartDataAccess : IDataMartDataAccess
    {

        #region Members

        /// <summary>
        /// The SQL connection object
        /// </summary>
        public IDbConnection connection { get; set; }

        #endregion Members
        public PgSQLDataMartDataAccess()
        {

        }

        public PgSQLDataMartDataAccess(IDbConnection dbconnection)
        {
            connection = dbconnection;
        }
        public PgSQLDataMartDataAccess(string connectionString)
        {
            connection = new NpgsqlConnection(connectionString);
        }


        #region Sync Methods

        public int Execute(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return connection.Execute(
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
            return connection.Query(
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
            return connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.Query(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public dynamic QueryFirst(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QueryFirst(sql, param, transaction, commandTimeout, commandType);
        }

        public dynamic QueryFirstOrDefault(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType);
        }

        public dynamic QuerySingle(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QuerySingle(sql, param, transaction, commandTimeout, commandType);
        }

        public dynamic QuerySingleOrDefault(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.QuerySingleOrDefault(sql, param, transaction, commandTimeout, commandType);
        }

        public object QueryFirst(Type type, string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.QueryFirst(type, sql, param, transaction, commandTimeout, commandType);
        }

        public object QueryFirstOrDefault(Type type, string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.QueryFirstOrDefault(type, sql, param, transaction, commandTimeout,
                commandType);
        }

        public object QuerySingle(Type type, string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.QuerySingle(type, sql, param, transaction, commandTimeout, commandType);
        }

        public object QuerySingleOrDefault(Type type, string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.QuerySingleOrDefault(type, sql, param, transaction, commandTimeout,
                commandType);
        }

        public T QueryFirst<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T QuerySingle<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T QuerySingleOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public IEnumerable<T> Query<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return connection.Query<T>(sql,
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
            return connection.ExecuteScalar(sql, param, transaction, commandTimeout, commandType);
        }

        public T ExecuteScalar<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
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
            return connection.ExecuteReader(sql, param, transaction, commandTimeout, commandType);
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
            return connection.Query(type, sql, param, transaction, buffered, commandTimeout, commandType);
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
            return connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<IEnumerable<dynamic>> QueryAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return connection.QueryAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?))
        {
            return connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
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
            return connection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> QuerySingleAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<IEnumerable<object>> QueryAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QueryAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<object> QueryFirstAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QueryFirstAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<object> QueryFirstOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QueryFirstOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<object> QuerySingleAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QuerySingleAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<object> QuerySingleOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QuerySingleOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType);
        }

        public Task<dynamic> QuerySingleOrDefaultAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QuerySingleOrDefaultAsync(sql, param, transaction, commandTimeout, commandType);
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
            return connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
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
            return connection.QueryAsync(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        // public async Task<IGridReader> QueryMultipleAsync(CommandDefinition command)
        // {
        //     var reader = await connection.QueryMultipleAsync(command).ConfigureAwait(false);
        //     return new GridReaderAbstraction(reader);
        // }

        public Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType);
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
            return connection.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> ExecuteScalarAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
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
            return connection.QueryFirstAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<dynamic> QueryFirstOrDefaultAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QueryFirstOrDefaultAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<dynamic> QuerySingleAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return connection.QuerySingleAsync(sql, param, transaction, commandTimeout, commandType);
        }

        #endregion Async Methods

        #region Implementation of IDisposable

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            connection.Dispose();
        }

        #endregion Implementation of IDisposable

    }
}
