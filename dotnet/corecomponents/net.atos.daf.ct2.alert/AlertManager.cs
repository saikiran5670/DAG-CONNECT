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
        public async Task<int> ActivateAlert(int alertId, char state, char checkState)
        {
            return await alertRepository.UpdateAlertState(alertId,state, checkState);
        }

        public async Task<int> SuspendAlert(int alertId, char state, char checkState)
        {
            return await alertRepository.UpdateAlertState(alertId, state, checkState);
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
        public async Task<Alert> CreateAlert(Alert alert)
        {
            return await alertRepository.CreateAlert(alert);
        }
        public async Task<Alert> UpdateAlert(Alert alert)
        {
            return await alertRepository.UpdateAlert(alert);
        }
        public async Task<IEnumerable<Alert>> GetAlertList(int accountid, int organizationid)
        {
            return await alertRepository.GetAlertList(accountid, organizationid);
        }

        #region Alert Category
        public async Task<IEnumerable<EnumTranslation>> GetAlertCategory()
        {
            try
            {
                return await alertRepository.GetAlertCategory();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region Duplicate Alert Type
        public Task<DuplicateAlertType> DuplicateAlertType(int alertId)
        {
            return alertRepository.DuplicateAlertType(alertId);
        }
        #endregion

        public async Task<bool> IsLandmarkActiveInAlert(List<int> landmarkId)
        {
            return await alertRepository.IsLandmarkActiveInAlert(landmarkId);
        }
        public async Task<IEnumerable<NotificationTemplate>> GetAlertNotificationTemplate()
        {
            try
            {
                return await alertRepository.GetAlertNotificationTemplate();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<NotificationRecipient>> GetRecipientLabelList(int organizationId)
        {
            try
            {
                return await alertRepository.GetRecipientLabelList(organizationId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
