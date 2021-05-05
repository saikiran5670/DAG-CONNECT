using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert
{
    public class AlertManager : IAlertManager
    {
        IAlertRepository alertRepository;
        public AlertManager(IAlertRepository _alertRepository)
        {
            alertRepository = _alertRepository;
        }

        #region ActivateAlert,SuspendAlert and  DeleteAlert
        public async Task<int> ActivateAlert(int alertId, char state)
        {
            return await alertRepository.UpdateAlertState(alertId,state);
        }

        public async Task<int> SuspendAlert(int alertId, char state)
        {
            return await alertRepository.UpdateAlertState(alertId, state);
        }

        public async Task<int> DeleteAlert(int alertId, char state)
        {
            return await alertRepository.AlertStateToDelete(alertId, state);
        }

        public async Task<bool> CheckIsNotificationExitForAlert(int alertId)
        {
            return await alertRepository.CheckIsNotificationExitForAlert(alertId);
        }
        
        #endregion

        public async Task<Alert> UpdateAlert(Alert alert)
        {
            return await alertRepository.UpdateAlert(alert);
        }        
    }
}
