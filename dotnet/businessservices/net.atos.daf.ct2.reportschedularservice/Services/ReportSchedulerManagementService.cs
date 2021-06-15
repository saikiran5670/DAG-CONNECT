﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.reportscheduler;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportschedulerservice.Entity;
using net.atos.daf.ct2.visibility;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.reportschedulerservice.Services
{
    public class ReportSchedulerManagementService : ReportSchedulerService.ReportSchedulerServiceBase
    {
        private ILog _logger;
        private readonly IReportSchedulerManager _reportSchedulerManager;
        private readonly Mapper _mapper;
        private readonly IVisibilityManager _visibilityManager;
        public ReportSchedulerManagementService(IReportSchedulerManager reportSchedulerManager, IVisibilityManager visibilityManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _reportSchedulerManager = reportSchedulerManager;
            _mapper = new Mapper();
            _visibilityManager = visibilityManager;
        }
        #region Get Report Scheduler Parameter       
        public override async Task<ReportParameterResponse> GetReportParameter(ReportParameterRequest request, ServerCallContext context)
        {
            try
            {
                ReportParameter reportparameter = await _reportSchedulerManager.GetReportParameter(request.AccountId, request.OrganizationId);
                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);
                ReportParameterResponse response = new ReportParameterResponse();
                //Get Report Type 
                if (reportparameter.ReportType.Any())
                {
                    foreach (var item in reportparameter.ReportType)
                    {
                        response.ReportType.Add(_mapper.MapReportType(item));
                    }
                }
                //Get Driver Info
                if (reportparameter.DriverDetail.Any())
                {
                    foreach (var item in reportparameter.DriverDetail)
                    {
                        response.DriverDetail.Add(_mapper.MapDriverDetail(item));
                    }
                }

                //Get Receipt Emails Info
                if (reportparameter.ReceiptEmails.Any())
                {
                    foreach (var item in reportparameter.ReceiptEmails)
                    {
                        response.ReceiptEmails.Add(_mapper.MapReceiptEmail(item));
                    }
                }

                if (vehicleDetailsAccountVisibilty.Any())
                {

                    var res = JsonConvert.SerializeObject(vehicleDetailsAccountVisibilty);
                    response.AssociatedVehicle.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AssociatedVehicleRequest>>(res)
                        );
                }

                response.Message = "Report Parameter retrieved";
                response.Code = ResponseCode.Success;
                _logger.Info("Get method in report parameter called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportParameterResponse
                {
                    Code = ResponseCode.Failed,
                    Message = "Get report parameter list fail : " + ex.Message
                });
            }
        }
        #endregion
    }
}
