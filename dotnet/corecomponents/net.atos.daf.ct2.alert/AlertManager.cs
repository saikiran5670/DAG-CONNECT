using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.repository;

namespace net.atos.daf.ct2.alert
{
    public class AlertManager : IAlertManager
    {
        IAlertRepository _alertRepository;
        public AlertManager(IAlertRepository alertRepository)
        {
            _alertRepository = alertRepository;
        }

        #region ActivateAlert,SuspendAlert and  DeleteAlert
        public async Task<int> ActivateAlert(int alertId, char state, char checkState)
        {
            return await _alertRepository.UpdateAlertState(alertId, state, checkState);
        }

        public async Task<int> SuspendAlert(int alertId, char state, char checkState)
        {
            return await _alertRepository.UpdateAlertState(alertId, state, checkState);
        }

        public async Task<int> DeleteAlert(int alertId, char state)
        {
            return await _alertRepository.AlertStateToDelete(alertId, state);
        }

        public async Task<bool> CheckIsNotificationExitForAlert(int alertId)
        {
            return await _alertRepository.CheckIsNotificationExitForAlert(alertId);
        }

        #endregion
        public async Task<Alert> CreateAlert(Alert alert)
        {
            return await _alertRepository.CreateAlert(alert);
        }
        public async Task<Alert> UpdateAlert(Alert alert)
        {
            return await _alertRepository.UpdateAlert(alert);
        }
        public async Task<IEnumerable<Alert>> GetAlertList(int accountid, int organizationid)
        {
            return await _alertRepository.GetAlertList(accountid, organizationid);
        }

        #region Alert Category
        public async Task<IEnumerable<EnumTranslation>> GetAlertCategory()
        {
            try
            {
                return await _alertRepository.GetAlertCategory();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Duplicate Alert Type
        public Task<DuplicateAlertType> DuplicateAlertType(int alertId)
        {
            return _alertRepository.DuplicateAlertType(alertId);
        }
        #endregion

        public async Task<bool> IsLandmarkActiveInAlert(List<int> landmarkId)
        {
            return await _alertRepository.IsLandmarkActiveInAlert(landmarkId);
        }
        public async Task<IEnumerable<NotificationTemplate>> GetAlertNotificationTemplate()
        {
            try
            {
                return await _alertRepository.GetAlertNotificationTemplate();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<NotificationRecipient>> GetRecipientLabelList(int organizationId)
        {
            try
            {
                return await _alertRepository.GetRecipientLabelList(organizationId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
