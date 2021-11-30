﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.reportscheduler;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.ENUM;
using net.atos.daf.ct2.reportschedulerservice.Entity;
using net.atos.daf.ct2.utilities;
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
                var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);

                ReportParameter reportparameter = await _reportSchedulerManager.GetReportParameter(request.AccountId, request.OrganizationId, request.ContextOrgId, request.RoleId);
                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId, request.ContextOrgId, featureId);
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
                var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);

                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId, request.ContextOrgId, featureId);

                IEnumerable<int> vehicleIds = vehicleDetailsAccountVisibilty.Select(x => x.VehicleId).ToList().Distinct();

                var vehgroupIds = vehicleDetailsAccountVisibilty.Select(x => x.VehicleGroupId).ToList().Distinct();

                IEnumerable<ReportSchedulerMap> reportSchedulerList = await _reportSchedulerManager.GetReportSchedulerList(request.OrganizationId, vehicleIds.ToList(), vehgroupIds.ToList());
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
                char reportStatus = request.Status == ((char)ReportSchedulerState.Active).ToString() ? (char)ReportSchedulerState.Suspend : (char)ReportSchedulerState.Active;
                objRepoModel.Status = reportStatus.ToString();
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
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);

                var featureId = await _visibilityManager.GetReportFeatureId(request.ReportId);

                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibility(request.AccountId, loggedInOrgId, request.OrganizationId, featureId);
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
        public override async Task<ReportPDFResponse> GetPDFBinaryFormatById(ReportPDFByIdRequest request, ServerCallContext context)
        {
            try
            {
                ReportPDFByidModel objReportPDFByidModel = new ReportPDFByidModel();
                objReportPDFByidModel.Id = request.ReportId;
                var data = await _reportSchedulerManager.GetPDFBinaryFormatById(objReportPDFByidModel);
                _logger.Info(ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_CALLED_MSG);
                if (data != null)
                {
                    ReportPDFResponse response = new ReportPDFResponse()
                    {
                        FileName = data.FileName ?? null,
                        Id = data.Id,
                        Report = ByteString.CopyFrom(data.Report) ?? null,
                        ScheduleReportId = data.ScheduleReportId,
                        Message = ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_SUCCESS_MSG,
                        Code = ResponseCode.Success
                    };
                    return await Task.FromResult(response);
                }
                else
                {
                    ReportPDFResponse response = new ReportPDFResponse()
                    {
                        Message = ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_FAIL_MSG,
                        Code = ResponseCode.NotFound
                    };
                    return await Task.FromResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportPDFResponse
                {
                    Code = ResponseCode.Failed,
                    Message = string.Format("{0} {1}", ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_FAIL_MSG, ex.Message)
                });
            }
        }
        #endregion

        #region GetPDFBinaryFormatByToken
        public override async Task<ReportPDFResponse> GetPDFBinaryFormatByToken(ReportPDFByTokenRequest request, ServerCallContext context)
        {
            try
            {
                ReportPDFBytokenModel objReportPDFBytokenModel = new ReportPDFBytokenModel();
                objReportPDFBytokenModel.Token = request.Token;
                PDFReportScreenModel data = await _reportSchedulerManager.GetPDFBinaryFormatByToken(objReportPDFBytokenModel);
                _logger.Info(ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_CALLED_MSG);
                long currentdate = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                if (data != null)
                {
                    if (data.ValidTill > currentdate)
                    {
                        string strUpdatedToken = await _reportSchedulerManager.UpdatePDFBinaryRecordByToken(data.Token.ToString());
                        ReportPDFResponse response = new ReportPDFResponse()
                        {
                            FileName = data.FileName ?? null,
                            Id = data.Id,
                            Report = ByteString.CopyFrom(data.Report) ?? null,
                            ScheduleReportId = data.ScheduleReportId,
                            Message = ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_SUCCESS_MSG,
                            Code = ResponseCode.Success
                        };
                        return await Task.FromResult(response);
                    }
                    else
                    {
                        ReportPDFResponse response = new ReportPDFResponse()
                        {
                            Message = ReportSchedulerConstant.REPORT_SCHEDULER_VALID_EMAIL_LINK,
                            Code = ResponseCode.Failed
                        };
                        return await Task.FromResult(response);
                    }

                }
                else
                {
                    ReportPDFResponse response = new ReportPDFResponse()
                    {
                        Message = ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_FAIL_MSG,
                        Code = ResponseCode.NotFound
                    };
                    return await Task.FromResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportPDFResponse
                {
                    Code = ResponseCode.Failed,
                    Message = string.Format("{0} {1}", ReportSchedulerConstant.REPORT_SCHEDULER_GETFORPDF_FAIL_MSG, ex.Message)
                });
            }
        }
        #endregion

        #region UnSubscribeById
        public override async Task<UnSubscribeResponse> UnSubscribeById(UnSubscribeRequest request, ServerCallContext context)
        {
            try
            {
                var isUpdated = await _reportSchedulerManager.UnSubscribeById(request.RecipentId, request.EmailId);
                _logger.Info(ReportSchedulerConstant.UN_SUBCRIBE_CALLED_MSG);

                if (isUpdated)
                {
                    var response = new UnSubscribeResponse()
                    {
                        Message = ReportSchedulerConstant.UN_SUBCRIBE_SUCCESS_MSG,
                        Code = ResponseCode.Success
                    };
                    return await Task.FromResult(response);
                }
                else
                {
                    var response = new UnSubscribeResponse()
                    {
                        Message = ReportSchedulerConstant.UN_SUBCRIBE_FAIL_MSG,
                        Code = ResponseCode.NotFound
                    };
                    return await Task.FromResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new UnSubscribeResponse
                {
                    Code = ResponseCode.Failed,
                    Message = string.Format("{0} {1}", ReportSchedulerConstant.UN_SUBCRIBE_FAIL_MSG2, ex.Message)
                });
            }
        }
        #endregion

        #region UnSubscribeAllByEmailId
        public override async Task<UnSubscribeAllResponse> UnSubscribeAllByEmailId(UnSubscribeAllRequest request, ServerCallContext context)
        {
            try
            {
                var isUpdated = await _reportSchedulerManager.UnSubscribeAllByEmailId(request.EmailId);
                _logger.Info(ReportSchedulerConstant.UN_SUBCRIBE_ALL_CALLED_MSG);

                if (isUpdated)
                {
                    var response = new UnSubscribeAllResponse()
                    {
                        Message = ReportSchedulerConstant.UN_SUBCRIBE_ALL_SUCCESS_MSG,
                        Code = ResponseCode.Success
                    };
                    return await Task.FromResult(response);
                }
                else
                {
                    var response = new UnSubscribeAllResponse()
                    {
                        Message = ReportSchedulerConstant.UN_SUBCRIBE_ALL_FAIL_MSG,
                        Code = ResponseCode.NotFound
                    };
                    return await Task.FromResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new UnSubscribeAllResponse
                {
                    Code = ResponseCode.Failed,
                    Message = string.Format("{0} {1}", ReportSchedulerConstant.UN_SUBCRIBE_ALL_FAIL_MSG2, ex.Message)
                });
            }
        }
        #endregion
    }
}
