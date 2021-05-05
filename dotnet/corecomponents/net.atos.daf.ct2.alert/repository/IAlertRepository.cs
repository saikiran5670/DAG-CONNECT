using net.atos.daf.ct2.alert.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert.repository
{
    public interface IAlertRepository
    {
        Task<Alert> UpdateAlert(Alert alert);
        Task<int> UpdateAlertState(int alertId, char state);
        Task<int> AlertStateToDelete(int alertId, char state);
        Task<bool> CheckIsNotificationExitForAlert(int alertId);
    }
}
