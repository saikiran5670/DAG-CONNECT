using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.schedular.entity;
using net.atos.daf.ct2.utilities;
using Npgsql;

namespace net.atos.daf.ct2.schedular.repository
{
    public class DataCleanupRepository : IDataCleanupRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartDataAccess;
        public DataCleanupRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartDataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartDataAccess = dataMartDataAccess;
        }
        public async Task<List<DataCleanupConfiguration>> GetDataPurgingConfiguration()
        {
            try
            {

                var queryStatement = @"SELECT id, database_name as DatabaseName, schema_name as SchemaName, table_name as TableName, column_name as ColumnName,
                                              retention_period as RetentionPeriod ,  created_at as CreatedAt, modified_at as ModifiedAt FROM master.datapurgingtableref";
                var data = (List<DataCleanupConfiguration>)await _dataAccess.QueryAsync<DataCleanupConfiguration>(queryStatement);

                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> DeleteDataFromTables(string connectString, DataCleanupConfiguration dataCleanupConfiguration)
        {
            var rowCount = 0;
           
            try
            {

                using (NpgsqlConnection conn = new NpgsqlConnection(connectString))
                {
                    await conn.OpenAsync();
                    var parameter = new DynamicParameters();
                    parameter.Add("@days", UTCHandling.GetUTCFromDateTime(DateTime.Now.AddDays((-1) * dataCleanupConfiguration.RetentionPeriod)), System.Data.DbType.Int64);
                    var query = String.Format("select count(*) from {0}.{1} where  {2}  < @days", dataCleanupConfiguration.SchemaName, dataCleanupConfiguration.TableName, dataCleanupConfiguration.ColumnName);
                    //var queryStatement = @"WITH deleted AS(" + String.Format("delete from {0}.{1} where  to_timestamp(created_at /1000)::date < (now()::date -  @days)", dataCleanupConfiguration.SchemaName,
                    //    dataCleanupConfiguration.TableName, dataCleanupConfiguration.ColumnName);
                    //queryStatement += ") IS TRUE RETURNING *) SELECT count(*) FROM deleted";

                    rowCount = await conn.QueryFirstOrDefaultAsync<int>(query, parameter);
                    Console.WriteLine(@"value of rowcount = {0}, tble name {2} , thread = {1}", rowCount, Thread.CurrentThread.ManagedThreadId, dataCleanupConfiguration.TableName);
                }

                return rowCount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DataPurgingTableLog> CreateDataPurgingTableLog(DataPurgingTableLog log, string connectString)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectString))
                {
                    await conn.OpenAsync();
                    string queryduplicate = string.Empty;

                    var parameter = new DynamicParameters();
                    parameter.Add("@purging_start_time", log.PurgingStartTime);
                    parameter.Add("@purging_end_time", log.PurgingEndTime);
                    parameter.Add("@no_of_deleted_records", log.NoOfDeletedRecords);
                    parameter.Add("@database_name", log.DatabaseName);
                    parameter.Add("@schema_name", log.SchemaName);
                    parameter.Add("@table_name", log.TableName);
                    parameter.Add("@column_name", log.ColumnName);
                    parameter.Add("@duration", log.Duration);
                    parameter.Add("@state", log.State);
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

                    string query = @"INSERT INTO master.datapurgingtablelog(
	                             purging_start_time, purging_end_time, no_of_deleted_records, created_at, database_name, schema_name, table_name, column_name, duration, state)
	                             VALUES (@purging_start_time, @purging_end_time, @no_of_deleted_records, @created_at, @database_name, @schema_name, @table_name, @column_name, @duration, @state) RETURNING id";

                    var id = await conn.ExecuteScalarAsync<int>(query, parameter);
                    log.Id = id;
                    Console.WriteLine(@"log :    tble name {0} , thread = {1}", Thread.CurrentThread.ManagedThreadId, log.TableName);

                }
            }
            catch (Exception)
            {
                throw;
            }
            return log;
        }

    }
}
