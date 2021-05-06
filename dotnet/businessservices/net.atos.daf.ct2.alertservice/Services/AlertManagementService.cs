using Grpc.Core;
using log4net;
using net.atos.daf.ct2.alert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alertservice.Services
{
    public class AlertManagementService : AlertService.AlertServiceBase
    {
        private ILog _logger;
        private readonly IAlertManager _alertManager;
        public AlertManagementService(IAlertManager alertManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _alertManager = alertManager;
        }

        #region ActivateAlert,SuspendAlert and  DeleteAlert
        public override async Task<AlertResponse> ActivateAlert(IdRequest request, ServerCallContext context)
        {
            try
            {
                var id = await _alertManager.ActivateAlert(request.AlertId, 'A');
                return await Task.FromResult(new AlertResponse
                {
                    Message = id > 0 ? $"Alert is Activated successful for id:- {id}." : $"Activate Alert Failed for id:- {request.AlertId}.",
                    Code = id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw ex;
            }
        }

        public override async Task<AlertResponse> SuspendAlert(IdRequest request, ServerCallContext context)
        {
            try
            {
                var id = await _alertManager.SuspendAlert(request.AlertId, 'I');
                return await Task.FromResult(new AlertResponse
                {
                    Message = id > 0 ? $"Alert is Suspended successful for id:- {id}." : $"Suspend Alert Failed for id:- {request.AlertId}.",
                    Code = id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw ex;
            }
        }

        #endregion

        
        #region Update Alert



        #endregion

    }
}
