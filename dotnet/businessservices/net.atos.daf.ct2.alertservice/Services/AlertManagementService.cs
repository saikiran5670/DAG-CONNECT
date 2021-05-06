﻿using Grpc.Core;
using log4net;
using net.atos.daf.ct2.alert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.alertservice.Entity;
using net.atos.daf.ct2.alert.entity;

namespace net.atos.daf.ct2.alertservice.Services
{
    public class AlertManagementService : AlertService.AlertServiceBase
    {
        private ILog _logger;
        private readonly IAlertManager _alertManager;
        private readonly IVehicleManager _vehicelManager;
        private readonly Mapper _mapper;
        public AlertManagementService(IAlertManager alertManager, IVehicleManager vehicelManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _alertManager = alertManager;
            _vehicelManager = vehicelManager;
            _mapper = new Mapper();
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
        public override async Task<AlertCategoryResponse> GetAlertCategory(AccountIdRequest request, ServerCallContext context)
        {
            try
            {
                IEnumerable<net.atos.daf.ct2.alert.entity.EnumTranslation> enumTranslationList = await _alertManager.GetAlertCategory();

                IEnumerable<VehicleGroupList> VehicleGroupList = await _vehicelManager.GetVehicleGroupbyAccountId(request.AccountId);
                AlertCategoryResponse response = new AlertCategoryResponse();
                foreach (var item in enumTranslationList)
                {
                    response.EnumTranslation.Add(_mapper.MapEnumTranslation(item));
                }
                foreach (var item in VehicleGroupList)
                {
                    response.VehicleGroup.Add(_mapper.MapVehicleGroup(item));
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

        public override async Task<AlertResponse> UpdateAlert(AlertRequest request, ServerCallContext context)
        {
            try
            {
                Alert alert = new Alert();
                alert = _mapper.ToAlertEntity(request);
                alert = await _alertManager.UpdateAlert(alert);
                return await Task.FromResult(new AlertResponse
                {
                    Message = alert.Id > 0 ? $"Alert is updated successful for id:- {alert.Id}." : $"Activate Alert Failed for id:- {request.Id}.",
                    Code = alert.Id > 0 ? ResponseCode.Success : ResponseCode.Failed,
                });

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponseCode.Failed,
                    AlertRequest = null
                });
            }
        }

        #endregion

        #region Create Alert
        public override async Task<AlertResponse> CreateAlert(AlertRequest request, ServerCallContext context)
        {
            try
            {
                Alert alert = new Alert();
                alert = _mapper.ToAlertEntity(request);
                alert = await _alertManager.CreateAlert(alert);
                return await Task.FromResult(new AlertResponse
                {
                    Message = alert.Id > 0 ? $"Alert is created successful for id:- {alert.Id}." : $"Alert creation is failed for {alert.Name}" ,
                    Code = alert.Id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponseCode.Failed,
                    AlertRequest = null
                });
            }
        }
        #endregion
    }
}
