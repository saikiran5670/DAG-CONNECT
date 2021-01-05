using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
// using Dapper;
namespace net.atos.daf.ct2.data
{
    public interface IDataAccess : IDisposable
    {
        #region Sync Methods

        int Execute(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?));

        IEnumerable<dynamic> Query(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?));

        IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?));

        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?));

        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, IDbTransaction transaction = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
            string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null,
            IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null,
            CommandType? commandType = null);

        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
            string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null,
            IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null,
            CommandType? commandType = null);

        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null,
            IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null,
            CommandType? commandType = null);

        IEnumerable<TReturn> Query<TReturn>(
            string sql, Type[] types,
            Func<object[], TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true,
            string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        dynamic QueryFirst(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        dynamic QueryFirstOrDefault(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        dynamic QuerySingle(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        dynamic QuerySingleOrDefault(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        object QueryFirst(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        object QueryFirstOrDefault(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        object QuerySingle(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        object QuerySingleOrDefault(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        T QueryFirst<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        T QueryFirstOrDefault<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        T QuerySingle<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        T QuerySingleOrDefault<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        IEnumerable<T> Query<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?));

        // // IGridReader QueryMultiple(
        // //     string sql,
        // //     object param = null,
        // //     IDbTransaction transaction = null,
        // //     int? commandTimeout = default(int?),
        // //     CommandType? commandType = default(CommandType?));

        // int Execute(CommandDefinition command);

        object ExecuteScalar(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null);

        T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null);

        // object ExecuteScalar(CommandDefinition command);

        // T ExecuteScalar<T>(CommandDefinition command);

        IDataReader ExecuteReader(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        // IDataReader ExecuteReader(CommandDefinition command);

        // IDataReader ExecuteReader(CommandDefinition command, CommandBehavior commandBehavior);

        IEnumerable<object> Query(Type type, string sql, object param = null,
            IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null,
            CommandType? commandType = null);

        // IEnumerable<T> Query<T>(CommandDefinition command);

        // T QueryFirst<T>(CommandDefinition command);

        // T QueryFirstOrDefault<T>(CommandDefinition command);

        // T QuerySingle<T>(CommandDefinition command);

        // T QuerySingleOrDefault<T>(CommandDefinition command);

        // // IGridReader QueryMultiple(CommandDefinition command);

        #endregion Sync Methods

        // #region Additional Extensions

        // IEnumerable<T> QueryAndTrimResults<T>(
        //     string sql,
        //     object param = null,
        //     IDbTransaction transaction = null,
        //     bool buffered = true,
        //     int? commandTimeout = default(int?),
        //     CommandType? commandType = default(CommandType?));

        // IEnumerable<TReturn> QueryAndTrimResults<TFirst, TSecond, TReturn>(
        //     string sql,
        //     Func<TFirst, TSecond, TReturn> map,
        //     object param = null,
        //     IDbTransaction transaction = null,
        //     bool buffered = true,
        //     string splitOn = "Id",
        //     int? commandTimeout = default(int?),
        //     CommandType? commandType = default(CommandType?));

        // #endregion Additional Extensions

        #region Async Methods

        Task<int> ExecuteAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?));

        Task<IEnumerable<dynamic>> QueryAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?));

        Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = default(int?),
            CommandType? commandType = default(CommandType?));

        // // Task<IGridReader> QueryMultipleAsync(
        // //     string sql,
        // //     object param = null,
        // //     IDbTransaction transaction = null,
        // //     int? commandTimeout = default(int?),
        // //     CommandType? commandType = default(CommandType?));

        // Task<IEnumerable<dynamic>> QueryAsync(CommandDefinition command);

        // Task<dynamic> QueryFirstAsync(CommandDefinition command);

        // Task<dynamic> QueryFirstOrDefaultAsync(CommandDefinition command);

        // Task<dynamic> QuerySingleAsync(CommandDefinition command);

        // Task<dynamic> QuerySingleOrDefaultAsync(CommandDefinition command);

        Task<T> QueryFirstAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        Task<T> QuerySingleAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        Task<T> QuerySingleOrDefaultAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        Task<IEnumerable<object>> QueryAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        Task<object> QueryFirstAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        Task<object> QueryFirstOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        Task<object> QuerySingleAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        Task<object> QuerySingleOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        // Task<IEnumerable<object>> QueryAsync(Type type, CommandDefinition command);

        // Task<object> QueryFirstAsync(Type type, CommandDefinition command);

        // Task<object> QueryFirstOrDefaultAsync(Type type, CommandDefinition command);

        // Task<object> QuerySingleAsync(Type type, CommandDefinition command);

        // Task<object> QuerySingleOrDefaultAsync(Type type, CommandDefinition command);

        Task<dynamic> QueryFirstAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<dynamic> QuerySingleAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<dynamic> QuerySingleOrDefaultAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        // Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command);

        // Task<T> QueryFirstAsync<T>(CommandDefinition command);

        // Task<T> QueryFirstOrDefaultAsync<T>(CommandDefinition command);

        // Task<T> QuerySingleAsync<T>(CommandDefinition command);

        // Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command);

        // Task<int> ExecuteAsync(CommandDefinition command);

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        // Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id");

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        // Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id");

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        // Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn = "Id");

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        // Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn = "Id");

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        // Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn = "Id");

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        // Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn = "Id");

        Task<IEnumerable<TReturn>> QueryAsync<TReturn>(string sql, Type[] types,
            Func<object[], TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true,
            string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        // // Task<IGridReader> QueryMultipleAsync(CommandDefinition command);

        Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        // Task<IDataReader> ExecuteReaderAsync(CommandDefinition command);

        // Task<IDataReader> ExecuteReaderAsync(CommandDefinition command, CommandBehavior commandBehavior);

        Task<object> ExecuteScalarAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        // Task<object> ExecuteScalarAsync(CommandDefinition command);

        // Task<T> ExecuteScalarAsync<T>(CommandDefinition command);

        #endregion Async Methods
    }
}