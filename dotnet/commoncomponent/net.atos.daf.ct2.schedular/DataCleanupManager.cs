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

        public async Task<int> DataPurging(DataCleanupConfiguration data)
        {
            return await _dataCleanupRepository.DataPurging(data);
        }
        public async Task<List<DataCleanupConfiguration>> GetDataPurgingConfiguration()
        {
            return await _dataCleanupRepository.GetDataPurgingConfiguration();
        }

        public void Worker(int workerId, ConcurrentQueue<DataCleanupConfiguration> _dataCleanupConfigurations)
        {

            _dataCleanupRepository.Worker(workerId, _dataCleanupConfigurations);
        }
    }
}
