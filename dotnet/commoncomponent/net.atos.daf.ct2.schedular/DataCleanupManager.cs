using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.schedular.entity;
using net.atos.daf.ct2.schedular.repository;

namespace net.atos.daf.ct2.schedular
{
    public class DataCleanupManager : IDataCleanupManager
    {
        private readonly IDataCleanupRepository _dataCleanupRepository;
        public DataCleanupManager(IDataCleanupRepository dataCleanupRepository)
        {
            _dataCleanupRepository = dataCleanupRepository;
        }

        public async Task<DataPurgingTableLog> CreateDataPurgingTableLog(DataPurgingTableLog log)
        {
            return await _dataCleanupRepository.CreateDataPurgingTableLog(log);
        }

        public int DataPurging(DataCleanupConfiguration data)
        {
            return _dataCleanupRepository.DataPurging(data);
        }

        public async Task<int> DeleteDataFromTables(string connectString, DataCleanupConfiguration dataCleanupConfiguration)
        {
            return await _dataCleanupRepository.DeleteDataFromTables(connectString, dataCleanupConfiguration);
        }

        public async Task<List<DataCleanupConfiguration>> GetDataPurgingConfiguration()
        {
            return await _dataCleanupRepository.GetDataPurgingConfiguration();
        }

        public void Worker(DataCleanupConfiguration op)
        {

            _dataCleanupRepository.Worker(op);
        }
    }
}
