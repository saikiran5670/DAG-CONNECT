using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert.repository
{
    public class AlertRepository : IAlertRepository
    {
        private readonly IDataAccess dataAccess;
        public AlertRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;

        }

        #region Update Alert

        public Task<Alert> UpdateAlert(Alert alert)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Update Alert State
        public Task<bool> UpdateAlertState(int alertId, char state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
