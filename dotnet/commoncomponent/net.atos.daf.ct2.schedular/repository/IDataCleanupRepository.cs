using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.schedular.entity;

namespace net.atos.daf.ct2.schedular.repository
{
    public interface IDataCleanupRepository
    {
        int DataPurging(DataCleanupConfiguration data);
        Task<List<DataCleanupConfiguration>> GetDataPurgingConfiguration();
        void Worker(DataCleanupConfiguration op);
        Task<int> DeleteDataFromTables(string connectString, DataCleanupConfiguration dataCleanupConfiguration);
        Task<DataPurgingTableLog> CreateDataPurgingTableLog(DataPurgingTableLog log);
    }
}