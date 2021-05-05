using net.atos.daf.ct2.alert.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert
{
    public interface IAlertManager
    {
        Task<int> ActivateAlert(int alertId, char state);
        Task<int> DeleteAlert(int alertId, char state);
        Task<int> SuspendAlert(int alertId, char state);
        Task<bool> CheckIsNotificationExitForAlert(int alertId);
        Task<Alert> UpdateAlert(Alert alert);
        #region Alert Category
        Task<IEnumerable<EnumTranslation>> GetAlertCategory();
        #endregion
    }
}
