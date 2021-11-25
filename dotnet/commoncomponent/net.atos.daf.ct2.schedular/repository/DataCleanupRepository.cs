using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.schedular.entity;

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

        public async Task<int> DataPurging(DataCleanupConfiguration dataCleanupConfiguration)
        {
            try
            {
                var noOfRows = 0;
                //var purgingConfig = await GetDataPurgingConfiguration();
                //  foreach (var item in purgingConfig)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@days", dataCleanupConfiguration.RetentionPeriod, System.Data.DbType.Int32);
                    var queryStatement = String.Format("select count(*) from { 0}.{ 1}  where to_timestamp(created_at / 1000)::date < (now()::date - @days)", //convert now to double for compare witout converting created at in timestamp
                        dataCleanupConfiguration.SchemaName, dataCleanupConfiguration.TableName);
                    //    dataCleanupConfiguration.TableName, dataCleanupConfiguration.ColumnName);
                    //queryStatement += ") ";
                    //var queryStatement = @"WITH deleted AS(" + String.Format("delete from {0}.{1} where  to_timestamp(created_at /1000)::date < (now()::date -  @days)", dataCleanupConfiguration.SchemaName,
                    //    dataCleanupConfiguration.TableName, dataCleanupConfiguration.ColumnName);
                    //queryStatement += ") IS TRUE RETURNING *) SELECT count(*) FROM deleted";
                    if (dataCleanupConfiguration.DatabaseName == "dafconnectmasterdatabase")
                    {
                        var result = await _dataAccess.QueryAsync<int>(queryStatement, parameter);
                        noOfRows = Convert.ToInt32(result);
                    }
                    else
                    {
                        var result = await _dataMartDataAccess.QueryAsync<int>(queryStatement, parameter);
                        noOfRows = Convert.ToInt32(result);
                    }

                }



                return noOfRows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Worker(int workerId, ConcurrentQueue<DataCleanupConfiguration> _dataCleanupConfigurations)
        {
            Console.WriteLine("Worker {0} is starting.", workerId);

            bool doneEnqueueing = true;
            do
            {
                DataCleanupConfiguration op;
                while (_dataCleanupConfigurations.TryDequeue(out op))
                {
                    Console.WriteLine("Worker {0} is processing item {1}", workerId, op.ColumnName);
                    _dataAccess.Connection.Open();
                    var transactionScope = _dataAccess.Connection.BeginTransaction();

                    try
                    {
                        var tries = 0;
                        var maxRetryCount = 5;
                        while (tries < maxRetryCount)
                        {
                            try
                            {
                                tries++;
                                var noOfDeletedData = DataPurging(op).Result;
                                break;
                            }
                            catch (Exception)
                            {
                                throw;
                                //add log
                            }
                        }

                        transactionScope.Commit();

                    }
                    catch (Exception ex)
                    {
                        transactionScope.Rollback();
                        // _log.Info("AddExistingTripCorridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(existingTripCorridor.Id));
                        // _log.Error(ex.ToString());

                    }
                    finally
                    {
                        _dataAccess.Connection.Close();
                    }
                    // Task.Delay(TimeSpan.FromMilliseconds(1)).Wait();
                    SpinWait.SpinUntil(() => Volatile.Read(ref doneEnqueueing) || (_dataCleanupConfigurations.Count > 0));
                }
                SpinWait.SpinUntil(() => Volatile.Read(ref doneEnqueueing) || (_dataCleanupConfigurations.Count > 0));
            }
            while (!Volatile.Read(ref doneEnqueueing) || (_dataCleanupConfigurations.Count > 0));

            //logic to insert data into logtable
            Console.WriteLine("Worker {0} is stopping.", workerId);
        }




    }
}
