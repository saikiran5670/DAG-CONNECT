using net.atos.daf.ct2.alert.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert
{
    public interface IAlertManager
    {
        Task<bool> ActivateAlert(int alertId, char state);
        Task<bool> DeleteAlert(int alertId, char state);
        Task<bool> SuspendAlert(int alertId, char state);
        Task<Alert> UpdateAlert(Alert alert);
    }
}
