using System;
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
    }
}
