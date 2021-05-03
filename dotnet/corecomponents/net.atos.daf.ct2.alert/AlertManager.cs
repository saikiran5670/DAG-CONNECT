using net.atos.daf.ct2.alert.repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert
{
    public class AlertManager :IAlertManager
    {
        IAlertRepository alertRepository;
        public AlertManager(IAlertRepository _alertRepository)
        {
            alertRepository = _alertRepository;
        }

    }
}
