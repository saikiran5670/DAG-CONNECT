using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.repository
{
    public class AlertRepository:IAlertRepository
    {
        private readonly IDataAccess dataAccess;
        public AlertRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;

        }

    }
}
