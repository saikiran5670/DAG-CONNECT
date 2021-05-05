using net.atos.daf.ct2.alert.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert
{
    public interface IAlertManager
    {
        Task<Alert> UpdateAlert(Alert alert);
    }
}
