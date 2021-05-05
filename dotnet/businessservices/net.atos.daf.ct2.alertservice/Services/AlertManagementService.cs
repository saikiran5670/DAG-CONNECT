using log4net;
using net.atos.daf.ct2.alert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alertservice.Services
{
    public class AlertManagementService: AlertService.AlertServiceBase
    {
        private ILog _logger;
        private readonly IAlertManager _alertManager;
        public AlertManagementService(IAlertManager alertManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _alertManager = alertManager;
        }

        #region ActivateAlert,SuspendAlert and  DeleteAlert
        
        #endregion

        }
}
