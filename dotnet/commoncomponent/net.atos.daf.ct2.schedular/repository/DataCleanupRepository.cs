using System;
using System.Collections.Generic;
using System.Text;
using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.schedular.repository
{
    public class DataCleanupRepository : IDataCleanupRepository
    {
        private readonly IDataAccess _dataAccess;
        public DataCleanupRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
    }
}
