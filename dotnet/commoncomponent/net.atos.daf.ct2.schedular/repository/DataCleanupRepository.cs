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
                if (dataCleanupConfiguration.DatabaseName != "dafconnectmasterdatabase")
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connectString))
                    {
                        await conn.OpenAsync();
                        var parameter = new DynamicParameters();
                        parameter.Add("@days", dataCleanupConfiguration.RetentionPeriod, System.Data.DbType.Int32);
                        var query = String.Format("select count(*) from {0}.{1} where  to_timestamp({2} /1000)::date < (now()::date -  @days)", dataCleanupConfiguration.SchemaName, dataCleanupConfiguration.TableName, dataCleanupConfiguration.ColumnName);
                        rowCount = await conn.QueryFirstOrDefaultAsync<int>(query, parameter);
                        Console.Write(rowCount);
                        Console.WriteLine(@"value of rowcount = {0}, tble name {2} , thread = {1}", rowCount, Thread.CurrentThread.ManagedThreadId, dataCleanupConfiguration.TableName);
                    }
                }
                return rowCount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public int DataPurging(DataCleanupConfiguration dataCleanupConfiguration)
        {

            try
            {
                var noOfRows = 0;
                //var purgingConfig = await GetDataPurgingConfiguration();
                //  foreach (var item in purgingConfig)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@days", dataCleanupConfiguration.RetentionPeriod, System.Data.DbType.Int32);
                    //SELECT extract(epoch from current_timestamp) *1000 + extract(milliseconds from current_timestamp);
                    //var queryStatement = @"select * from tripdetail.tripalert limit 10"; //String.Format("select count(*) from {0}.{1} where  to_timestamp({2} /1000)::date < (now()::date -  @days)", dataCleanupConfiguration.SchemaName, dataCleanupConfiguration.TableName, dataCleanupConfiguration.ColumnName);
                    //    dataCleanupConfiguration.TableName, dataCleanupConfiguration.ColumnName);
                    //queryStatement += ") ";
                    //var queryStatement = @"WITH deleted AS(" + String.Format("delete from {0}.{1} where  to_timestamp(created_at /1000)::date < (now()::date -  @days)", dataCleanupConfiguration.SchemaName,
                    //    dataCleanupConfiguration.TableName, dataCleanupConfiguration.ColumnName);
                    //queryStatement += ") IS TRUE RETURNING *) SELECT count(*) FROM deleted";

                    if (dataCleanupConfiguration.DatabaseName == "dafconnectmasterdatabase")
                    {
                        //using (var _dataAccess1 = _dataAccess)
                        //{

                        //    var queryStatement = @"select  id from master.vehicle limit 10";
                        //    var result = await _dataAccess1.Query<dynamic>(queryStatement);
                        //    noOfRows = Convert.ToInt32(result);
                        Console.Write("Table name " + dataCleanupConfiguration.TableName + noOfRows);
                        //}
                    }
                    else
                    {


                        var queryStatement = @"select id from tripdetail.tripalert limit 10";
                        var result = _dataMartDataAccess.Query<dynamic>(queryStatement);
                        //connection.QueryAsync(queryStatement, transaction);


                        //}
                        //  }


                        //using (var _dataMartDataAccess1 = _dataMartDataAccess)
                        //{



                        //    var purgeSatrtTime = UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString());
                        //    var queryStatement = @"select id from tripdetail.tripalert limit 10";
                        //    var result = await _dataMartDataAccess1.Query<dynamic>(queryStatement);
                        //    //noOfRows = Convert.ToInt32(result);
                        //    Console.Write("No of rows deleted :  " + noOfRows + " table name " + dataCleanupConfiguration.TableName);
                        //    //   var log = ToTableLog(dataCleanupConfiguration, purgeSatrtTime);
                        //    //  await CreateDataPurgingTableLog(log);
                        //}

                    }


                }



                return noOfRows;
            }
            catch (Exception ex)
            {
                throw;
            }

        }



        public async Task<DataPurgingTableLog> CreateDataPurgingTableLog(DataPurgingTableLog log)
        {
            try
            {
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

                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                log.Id = id;
            }
            catch (Exception)
            {
                throw;
            }
            return log;
        }

        public void Worker(DataCleanupConfiguration op)
        {
            Console.WriteLine("Worker {0} is processing item {1}", op.Id, op.TableName);
            try
            {

                {
                    var tries = 0;
                    var maxRetryCount = 5;
                    while (tries < maxRetryCount)
                    {
                        try
                        {
                            tries++;
                            var noOfDeletedData = DataPurging(op);

                            //Thread.Sleep(50000);
                            break;
                        }
                        catch (Exception)
                        {
                            // if (tries > 3)

                            // StopAsync(new CancellationToken()).ConfigureAwait(true); ;
                            throw;
                            //add log
                        }
                    }
                }
                //  _threadData.TryDequeue(out op);
                Console.WriteLine("Worker {0} is stopping.", op.Id);
                //Write a logic to deque
                //write a logic to add log for success & retires count 
                //write a table specific logs e

            }
            catch (Exception ex)
            {
                // _log.Info("AddExistingTripCorridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(existingTripCorridor.Id));
                // _log.Error(ex.ToString());
            }
            //finally
            //{
            //    if (_dataAccess.Connection.State == System.Data.ConnectionState.Open) _dataAccess.Connection.Close();
            //    if (_dataMartDataAccess.Connection.State == System.Data.ConnectionState.Open) _dataMartDataAccess.Connection.Close();
            //}
            //logic to insert data into logtable

        }

    }
}
