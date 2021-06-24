using System;
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
        private readonly ILog _logger;
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
                    //var vehicleByVisibilityAndFeature
                    //                            = await _visibilityManager
                    //                                .GetVehicleByVisibilityAndFeature(request.AccountId, request.OrganizationId,
                    //                                                                   request.RoleId, vehicleDetailsAccountVisibilty,
                    //                                                                   ReportSchedulerConstant.REPORT_SCHEDULER_FEATURE_NAME);

                    //res = JsonConvert.SerializeObject(vehicleByVisibilityAndFeature);
                    //response.VehicleSubscribeandFeature.AddRange(
                    //    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleSubscribeandFeatureRequest>>(res)
                    //    );
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

        #region Create Report Scheduler
        public override async Task<ReportSchedulerResponse> CreateReportScheduler(ReportSchedulerRequest request, ServerCallContext context)
        {
            try
            {

                ReportSchedulerResponse response = new ReportSchedulerResponse();
                ReportScheduler reportscheduler = await _reportSchedulerManager.CreateReportScheduler(_mapper.ToReportSchedulerEntity(request));
                if (reportscheduler.Id > 0)
                {
                    response.Message = "Report Scheduler Created";
                    response.Code = ResponseCode.Success;
                    response.ReportSchedulerId = reportscheduler.Id;
                }
                else
                {
                    response.Message = "Report Scheduler Creation is fail";
                    response.Code = ResponseCode.Failed;
                }
                _logger.Info("Create method in report scheduler called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportSchedulerResponse
                {
                    Code = ResponseCode.Failed,
                    Message = "Create method in report scheduler fail : " + ex.Message
                });
            }
        }
        #endregion

        #region Update Report scheduler
        public override async Task<ReportSchedulerResponse> UpdateReportScheduler(ReportSchedulerRequest request, ServerCallContext context)
        {
            try
            {

                ReportSchedulerResponse response = new ReportSchedulerResponse();
                ReportScheduler reportscheduler = await _reportSchedulerManager.UpdateReportScheduler(_mapper.ToReportSchedulerEntity(request));
                if (reportscheduler.Id > 0)
                {
                    response.Message = "Report Scheduler Updated";
                    response.Code = ResponseCode.Success;
                    response.ReportSchedulerId = reportscheduler.Id;
                }
                else
                {
                    response.Message = "Report Scheduler Updatation is fail";
                    response.Code = ResponseCode.Failed;
                }
                _logger.Info("Update method in report scheduler called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportSchedulerResponse
                {
                    Code = ResponseCode.Failed,
                    Message = "Update method in report scheduler fail : " + ex.Message
                });
            }
        }
        #endregion

        #region Get Report Scheduler
        public override async Task<ReportSchedulerListResponse> GetReportSchedulerList(ReportParameterRequest request, ServerCallContext context)
        {
            try
            {
                IEnumerable<ReportSchedulerMap> reportSchedulerList = await _reportSchedulerManager.GetReportSchedulerList(request.OrganizationId);
                ReportSchedulerListResponse response = new ReportSchedulerListResponse();
                if (reportSchedulerList.Any())
                {
                    foreach (var item in reportSchedulerList)
                    {
                        response.ReportSchedulerRequest.Add(_mapper.MapReportSchedulerEntity(item));
                    }
                }
                response.Message = ReportSchedulerConstant.REPORT_SCHEDULER_GET_SUCCESS_MSG;
                response.Code = ResponseCode.Success;
                _logger.Info(ReportSchedulerConstant.REPORT_SCHEDULER_GET_CALLED_MSG);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportSchedulerListResponse
                {
                    Code = ResponseCode.Failed,
                    Message = ReportSchedulerConstant.REPORT_SCHEDULER_GET_FAIL_MSG + ex.Message
                });
            }
        }
        #endregion

        #region DeleteReportSchedule
        public override async Task<ReportStatusUpdateDeleteResponse> DeleteReportSchedule(ReportStatusUpdateDeleteRequest request, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.reportscheduler.entity.ReportStatusUpdateDeleteModel objRepoModel = new net.atos.daf.ct2.reportscheduler.entity.ReportStatusUpdateDeleteModel();
                objRepoModel.ReportId = request.ReportId;
                objRepoModel.OrganizationId = request.OrganizationId;
                objRepoModel.Status = "D";
                int reportId = await _reportSchedulerManager.ManipulateReportSchedular(objRepoModel);
                ReportStatusUpdateDeleteResponse response = new ReportStatusUpdateDeleteResponse();
                if (reportId > 0)
                {
                    response.Message = $"ReportSchedule with Report Id:{reportId} Deleted Sucessfully";
                    response.Code = ResponseCode.Success;
                    response.ReportId = reportId;
                }
                else
                {
                    response.Message = "Deletion Failed.";
                    response.Code = ResponseCode.Failed;
                    response.ReportId = reportId;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportStatusUpdateDeleteResponse
                {
                    Message = $"Exception While deleting ReportSchedule with Report Id: {request.ReportId}",
                    Code = ResponseCode.InternalServerError,
                    ReportId = request.ReportId
                });
            }
        }
        #endregion

        #region EnableDisableReportSchedule
        public override async Task<ReportStatusUpdateDeleteResponse> EnableDisableReportSchedule(ReportStatusUpdateDeleteRequest request, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.reportscheduler.entity.ReportStatusUpdateDeleteModel objRepoModel = new net.atos.daf.ct2.reportscheduler.entity.ReportStatusUpdateDeleteModel();
                objRepoModel.ReportId = request.ReportId;
                objRepoModel.OrganizationId = request.OrganizationId;
                objRepoModel.Status = request.Status;
                int reportId = await _reportSchedulerManager.ManipulateReportSchedular(objRepoModel);
                ReportStatusUpdateDeleteResponse response = new ReportStatusUpdateDeleteResponse();
                if (reportId > 0)
                {
                    response.Message = $"ReportSchedule with Report Id:{reportId}, Enable/Disable is Sucessfully";
                    response.Code = ResponseCode.Success;
                    response.ReportId = reportId;
                }
                else
                {
                    response.Message = "Enable/Disable Failed";
                    response.Code = ResponseCode.Failed;
                    response.ReportId = reportId;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportStatusUpdateDeleteResponse
                {
                    Message = $"Exception While Enable/Disable ReportSchedule with Report Id: {request.ReportId}",
                    Code = ResponseCode.InternalServerError,
                    ReportId = request.ReportId
                });
            }
        }
        #endregion

        #region Get Vehicle Id and Vehicle Group Id
        public override async Task<VehicleandVehicleGroupIdResponse> GetVehicleandVehicleGroupId(ReportParameterRequest request, ServerCallContext context)
        {
            try
            {
                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);
                VehicleandVehicleGroupIdResponse response = new VehicleandVehicleGroupIdResponse();
                if (vehicleDetailsAccountVisibilty.Any())
                {
                    foreach (var item in vehicleDetailsAccountVisibilty)
                    {
                        if (item.GroupType == "S")
                        {
                            VehicleIdList objvehicleid = new VehicleIdList();
                            objvehicleid.VehicleId = item.VehicleId;
                            if (!response.VehicleIdList.Contains(objvehicleid))
                            {
                                response.VehicleIdList.Add(objvehicleid);
                            }
                        }
                        else
                        {
                            VehicleGroupIdList objvehiclegrpid = new VehicleGroupIdList();
                            objvehiclegrpid.VehicleGroupId = item.VehicleGroupId;
                            if (!response.VehicleGroupIdList.Contains(objvehiclegrpid))
                            {
                                response.VehicleGroupIdList.Add(objvehiclegrpid);
                            }
                        }
                    }
                }

                response.Message = "Vehicle and Vehicle GroupId retrieved";
                response.Code = ResponseCode.Success;
                _logger.Info("Get method in Vehicle and Vehicle GroupId called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleandVehicleGroupIdResponse
                {
                    Code = ResponseCode.Failed,
                    Message = "Get Vehicle and Vehicle GroupId list fail : " + ex.Message
                });
            }
        }
        #endregion

        #region GetPDFBinaryFormatById
        public override async Task<ReportPDFListResponse> GetPDFBinaryFormatById(ReportPDFByIdRequest request, ServerCallContext context)
        {
            try
            {
                var data = await _reportSchedulerManager.GetPDFBinaryFormatById(request.ReportId);
                ReportPDFListResponse response = new ReportPDFListResponse();
                if (data.Any())
                {
                    foreach (var item in data)
                    {
                        response.PDFList.Add(_mapper.MapPDFRepoModel(item));
                    }
                }
                response.Message = ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_SUCCESS_MSG;
                response.Code = ResponseCode.Success;
                _logger.Info(ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_CALLED_MSG);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportPDFListResponse
                {
                    Code = ResponseCode.Failed,
                    Message = string.Format("{0} {1}", ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_FAIL_MSG, ex.Message)
                });
            }
        }
        #endregion

        #region GetPDFBinaryFormatByToken
        public override async Task<ReportPDFListResponse> GetPDFBinaryFormatByToken(ReportPDFByTokenRequest request, ServerCallContext context)
        {
            try
            {
                var data = await _reportSchedulerManager.GetPDFBinaryFormatByToken(request.Token);
                ReportPDFListResponse response = new ReportPDFListResponse();
                if (data.Any())
                {
                    foreach (var item in data)
                    {
                        response.PDFList.Add(_mapper.MapPDFRepoModel(item));
                    }
                }
                response.Message = ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_SUCCESS_MSG;
                response.Code = ResponseCode.Success;
                _logger.Info(ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_CALLED_MSG);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportPDFListResponse
                {
                    Code = ResponseCode.Failed,
                    Message = string.Format("{0} {1}", ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_FAIL_MSG, ex.Message)
                });
            }
        }
        #endregion
    }
}
