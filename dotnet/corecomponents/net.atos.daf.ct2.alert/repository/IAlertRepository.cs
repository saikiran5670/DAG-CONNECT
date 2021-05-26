using net.atos.daf.ct2.alert.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert.repository
{
    public interface IAlertRepository
    {
        Task<Alert> CreateAlert(Alert alert);
        Task<Alert> UpdateAlert(Alert alert);
        Task<int> UpdateAlertState(int alertId, char state, char checkState);
        Task<int> AlertStateToDelete(int alertId, char state);
        Task<bool> CheckIsNotificationExitForAlert(int alertId);
        Task<IEnumerable<Alert>> GetAlertList(int accountid, int organizationid);

        #region Alert Category
        Task<IEnumerable<EnumTranslation>> GetAlertCategory();
        Task<DuplicateAlertType> DuplicateAlertType(int alertId);
        #endregion

        Task<bool> IsLandmarkActiveInAlert(List<int> landmarkId);
    }
}
