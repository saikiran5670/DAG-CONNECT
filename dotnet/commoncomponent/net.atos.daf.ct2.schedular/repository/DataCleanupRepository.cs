using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<DataCleanupConfiguration> GetDataPurgingConfiguration()
        {
            try
            {
                var queryStatement = @"SELECT id, database_name, schema_name, table_name, column_name, retention_period, created_at, modified_at FROM master.datapurgingtableref";
                var data = await _dataMartDataAccess.QueryAsync<DataCleanupConfiguration>(queryStatement);

                return data.GetEnumerator().Current;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DataCleanupConfiguration> DataPurging()
        {
            try
            {
                var queryStatement = @"SELECT id, database_name, schema_name, table_name, column_name, retention_period, created_at, modified_at FROM master.datapurgingtableref";
                var data = await _dataMartDataAccess.QueryAsync<DataCleanupConfiguration>(queryStatement);

                return data.GetEnumerator().Current;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
