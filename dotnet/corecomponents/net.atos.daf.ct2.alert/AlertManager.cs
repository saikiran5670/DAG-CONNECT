using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert
{
    public class AlertManager :IAlertManager
    {
        IAlertRepository alertRepository;
        public AlertManager(IAlertRepository _alertRepository)
        {
            alertRepository = _alertRepository;
        }

        public async Task<Alert> UpdateAlert(Alert alert)
        {
            return await alertRepository.UpdateAlert(alert);
        }
    }
}
