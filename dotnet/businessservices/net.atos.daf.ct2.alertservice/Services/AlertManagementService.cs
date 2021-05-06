using Grpc.Core;
using log4net;
using net.atos.daf.ct2.alert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle;


namespace net.atos.daf.ct2.alertservice.Services
{
    public class AlertManagementService : AlertService.AlertServiceBase
    {
        private ILog _logger;
        private readonly IAlertManager _alertManager;
        private readonly IVehicleManager _vehicelManager;
        public AlertManagementService(IAlertManager alertManager, IVehicleManager vehicelManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _alertManager = alertManager;
            _vehicelManager = vehicelManager;
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

        #region Alert Category
        public override async Task<AlertCategoryResponse> GetAlertCategory(AccountIdRequest request , ServerCallContext context)
        {
            try
            {
                IEnumerable<net.atos.daf.ct2.alert.entity.EnumTranslation> enumTranslationList = await _alertManager.GetAlertCategory();

                IEnumerable<VehicleGroupList> VehicleGroupList = await _vehicelManager.GetVehicleGroupbyAccountId(request.AccountId);
                AlertCategoryResponse response = new AlertCategoryResponse();
                foreach (var item in enumTranslationList)
                {
                    EnumTranslation enumtrans = new EnumTranslation();
                    enumtrans.Id = item.Id;
                    enumtrans.Type = item.Type;
                    enumtrans.Enum = item.Enum;
                    enumtrans.ParentEnum = item.ParentEnum;
                    enumtrans.Key = item.Key;
                    response.EnumTranslation.Add(enumtrans);
                }
                foreach (var item in VehicleGroupList)
                {
                    VehicleGroup vehiclegroup = new VehicleGroup();
                    vehiclegroup.VehicleGroupId = item.VehicleGroupId;
                    vehiclegroup.Vin = item.Vin;
                    vehiclegroup.VehicleId = item.VehicleId;
                    vehiclegroup.VehicleName = item.VehicleName;   
                    response.VehicleGroup.Add(vehiclegroup);
                }
                response.Message = "Alert Category data retrieved";
                response.Code = ResponseCode.Success;
                _logger.Info("Get method in alert service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertCategoryResponse
                {
                    Code = ResponseCode.Failed,
                    Message = "Get alert category fail : " + ex.Message
                });
            }
        }
        #endregion

        
        #region Update Alert



        #endregion

    }
}
