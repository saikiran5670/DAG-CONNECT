using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.alert.entity;

namespace net.atos.daf.ct2.alert
{
    public interface IAlertManager
    {
        Task<Alert> CreateAlert(Alert alert);
        Task<int> ActivateAlert(int alertId, char state, char checkState);
        Task<int> DeleteAlert(int alertId, char state);
        Task<int> SuspendAlert(int alertId, char state, char checkState);
        Task<bool> CheckIsNotificationExitForAlert(int alertId);
        Task<Alert> UpdateAlert(Alert alert);
        Task<IEnumerable<Alert>> GetAlertList(int accountid, int organizationid);
        Task<DuplicateAlertType> DuplicateAlertType(int alertId);
        #region Alert Category
        Task<IEnumerable<EnumTranslation>> GetAlertCategory();
        #endregion
        //Task<bool> IsLandmarkActiveInAlert(List<int> landmarkId);
        Task<IEnumerable<NotificationTemplate>> GetAlertNotificationTemplate();
        Task<IEnumerable<NotificationRecipient>> GetRecipientLabelList(int organizationId);
        Task<bool> IsLandmarkActiveInAlert(List<int> landmarkId, string Landmarktype);
    }
}
