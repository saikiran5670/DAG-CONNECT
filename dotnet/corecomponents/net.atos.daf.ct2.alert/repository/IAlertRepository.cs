using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.alert.entity;

namespace net.atos.daf.ct2.alert.repository
{
    public interface IAlertRepository
    {
        Task<Alert> CreateAlert(Alert alert);
        Task<Alert> UpdateAlert(Alert alert);
        Task<int> UpdateAlertState(int alertId, char state, char checkState);
        Task<int> AlertStateToDelete(int alertId, char state);
        Task<bool> CheckIsNotificationExitForAlert(int alertId);
        Task<IEnumerable<Alert>> GetAlertList(int accountid, int organizationid, List<int> featureIds, List<int> vehicleIds);

        #region Alert Category
        Task<IEnumerable<EnumTranslation>> GetAlertCategory();
        Task<DuplicateAlertType> DuplicateAlertType(int alertId);
        #endregion

        Task<bool> IsLandmarkActiveInAlert(List<int> landmarkId, string Landmarktype);
        //Task<bool> IsLandmarkActiveInAlert(List<int> landmarkId);
        Task<IEnumerable<NotificationTemplate>> GetAlertNotificationTemplate();
        Task<IEnumerable<NotificationRecipient>> GetRecipientLabelList(int organizationId);
        Task<int> InsertViewNotification(List<NotificationViewHistory> notificationViewHistories, int accountId);
        Task<OfflinePushNotification> GetOfflinePushNotification(OfflinePushNotificationFilter offlinePushNotificationFilter);
    }
}
