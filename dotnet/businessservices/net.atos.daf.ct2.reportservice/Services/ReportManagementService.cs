﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.ENUM;
using net.atos.daf.ct2.reportservice.entity;
using net.atos.daf.ct2.visibility;
using Newtonsoft.Json;
using ReportComponent = net.atos.daf.ct2.reports;

namespace net.atos.daf.ct2.reportservice.Services
{
    public class ReportManagementService : ReportService.ReportServiceBase
    {
        private ILog _logger;
        private readonly IReportManager _reportManager;
        private readonly IVisibilityManager _visibilityManager;
        private readonly Mapper _mapper;
        public ReportManagementService(IReportManager reportManager, IVisibilityManager visibilityManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _reportManager = reportManager;
            _visibilityManager = visibilityManager;
            _mapper = new Mapper();
        }

        #region Select User Preferences
        public override async Task<UserPreferenceDataColumnResponse> GetUserPreferenceReportDataColumn(IdRequest request, ServerCallContext context)
        {
            try
            {
                var userPrefernces = await _reportManager.GetUserPreferenceReportDataColumn(request.ReportId, request.AccountId, request.OrganizationId);
                if (userPrefernces.Count() == 0)
                {
                    return await Task.FromResult(new UserPreferenceDataColumnResponse
                    {
                        Message = String.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, request.AccountId, request.ReportId, ReportConstants.USER_PREFERENCE_FAILURE_MSG2),
                        Code = Responsecode.Failed
                    });
                }
                if (!userPrefernces.Any(a => a.State == ((char)ReportPreferenceState.Active).ToString()))
                {
                    var roleBasedUserPrefernces = await _reportManager.GetRoleBasedDataColumn(request.ReportId, request.AccountId, request.OrganizationId);

                    if (!roleBasedUserPrefernces.Any(a => a.State == ((char)ReportPreferenceState.Active).ToString()))
                    {
                        foreach (var item in roleBasedUserPrefernces)
                        {
                            item.State = ((char)ReportPreferenceState.Active).ToString();
                        }
                    }

                    var roleBasedresponse = new UserPreferenceDataColumnResponse
                    {
                        Message = String.Format(ReportConstants.USER_PREFERENCE_SUCCESS_MSG, request.AccountId, request.ReportId),
                        Code = Responsecode.Success
                    };

                    roleBasedresponse.UserPreferences.AddRange(_mapper.MapUserPrefences(roleBasedUserPrefernces));
                    return await Task.FromResult(roleBasedresponse);
                }

                var response = new UserPreferenceDataColumnResponse
                {
                    Message = String.Format(ReportConstants.USER_PREFERENCE_SUCCESS_MSG, request.AccountId, request.ReportId),
                    Code = Responsecode.Success
                };
                response.UserPreferences.AddRange(_mapper.MapUserPrefences(userPrefernces));
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                var errorResponse = new UserPreferenceDataColumnResponse
                {
                    Message = ex.Message,
                    Code = Responsecode.InternalServerError
                };
                errorResponse.UserPreferences.Add(new UserPreferenceDataColumn());
                return await Task.FromResult(errorResponse);
            }
        }
        #endregion

        #region Create User Preference
        public override async Task<UserPreferenceCreateResponse> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceCreateRequest, ServerCallContext context)
        {
            try
            {
                _logger.Info("CreateUserPreference method in ReportManagement service called.");

                int insertedUserPreferenceCount = await _reportManager.CreateUserPreference(_mapper.MapCreateUserPrefences(objUserPreferenceCreateRequest));
                if (insertedUserPreferenceCount == 0)
                {
                    return await Task.FromResult(new UserPreferenceCreateResponse
                    {
                        Message = String.Format(ReportConstants.USER_PREFERENCE_CREATE_FAILURE_MSG, objUserPreferenceCreateRequest.AccountId, objUserPreferenceCreateRequest.ReportId),
                        Code = Responsecode.Failed
                    });
                }

                return await Task.FromResult(new UserPreferenceCreateResponse
                {
                    Message = String.Format(ReportConstants.USER_PREFERENCE_CREATE_SUCCESS_MSG, objUserPreferenceCreateRequest.AccountId, objUserPreferenceCreateRequest.ReportId),
                    Code = Responsecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new UserPreferenceCreateResponse
                {
                    Message = String.Format(ReportConstants.USER_PREFERENCE_CREATE_FAILURE_MSG, objUserPreferenceCreateRequest.AccountId, objUserPreferenceCreateRequest.ReportId),
                    Code = Responsecode.InternalServerError
                });
            }
        }
        #endregion

        #region Get Vins from data mart trip_statistics
        public override async Task<VehicleListAndDetailsResponse> GetVinsFromTripStatisticsWithVehicleDetails(VehicleListRequest request, ServerCallContext context)
        {
            var response = new VehicleListAndDetailsResponse();
            try
            {
                var vehicleDeatilsWithAccountVisibility =
                                await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);

                if (vehicleDeatilsWithAccountVisibility.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, request.AccountId, request.OrganizationId);
                    response.Code = Responsecode.Failed;
                    return response;
                }

                var vinList = await _reportManager
                                        .GetVinsFromTripStatistics(vehicleDeatilsWithAccountVisibility
                                                                       .Select(s => s.Vin).Distinct());
                if (vinList.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_TRIP_NOTFOUND_MSG, request.AccountId, request.OrganizationId);
                    response.Code = Responsecode.Failed;
                    response.VinTripList.Add(new List<VehicleFromTripDetails>());
                    return response;
                }
                var res = JsonConvert.SerializeObject(vehicleDeatilsWithAccountVisibility);
                response.VehicleDetailsWithAccountVisibiltyList.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleDetailsWithAccountVisibilty>>(res)
                    );
                response.Message = ReportConstants.GET_VIN_SUCCESS_MSG;
                response.Code = Responsecode.Success;
                res = JsonConvert.SerializeObject(vinList);
                response.VinTripList.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleFromTripDetails>>(res)
                    );
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                response.Message = ex.Message;
                response.Code = Responsecode.InternalServerError;
                response.VehicleDetailsWithAccountVisibiltyList.Add(new List<VehicleDetailsWithAccountVisibilty>());
                response.VinTripList.Add(new List<VehicleFromTripDetails>());
                return await Task.FromResult(response);
            }
        }
        #endregion


        #region Trip Report Table Details
        public override async Task<TripResponce> GetFilteredTripDetails(TripFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetAllTripDetails.");
                ReportComponent.entity.TripFilterRequest objTripFilter = new ReportComponent.entity.TripFilterRequest();
                objTripFilter.VIN = request.VIN;
                objTripFilter.StartDateTime = request.StartDateTime;
                objTripFilter.EndDateTime = request.EndDateTime;

                var result = await _reportManager.GetFilteredTripDetails(objTripFilter);
                TripResponce response = new TripResponce();
                if (result?.Count > 0)
                {
                    var res = JsonConvert.SerializeObject(result);
                    response.TripData.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<TripDetils>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = "No Result Found";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new TripResponce
                {
                    Code = Responsecode.Failed,
                    Message = "GetFilteredTripDetails get failed due to - " + ex.Message
                });
            }
        }
        #endregion

        #region Driver Time management Report
        /// <summary>
        /// Fetch Multiple Drivers activity data
        /// </summary>
        /// <param name="request"> Filters for driver activity with VIN and Driver ID </param>
        /// <param name="context">GRPC Context</param>
        /// <returns>Driver activity by type column</returns>
        public override async Task<DriverActivityResponce> GetDriversActivity(ActivityFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetDriversActivity for multiple drivers.");
                ReportComponent.entity.DriverActivityFilter objActivityFilter = new ReportComponent.entity.DriverActivityFilter();
                objActivityFilter.VIN.Append(request.VINs);
                objActivityFilter.StartDateTime = request.StartDateTime;
                objActivityFilter.EndDateTime = request.EndDateTime;

                var result = await _reportManager.GetDriversActivity(objActivityFilter);
                DriverActivityResponce response = new DriverActivityResponce();
                if (result?.Count > 0)
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.DriverActivities.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<DriverActivity>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = "No Result Found";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new DriverActivityResponce
                {
                    Code = Responsecode.Failed,
                    Message = "GetDriversActivity get failed due to - " + ex.Message
                });
            }
        }

        /// <summary>
        /// Fetch Single driver activity data
        /// </summary>
        /// <param name="request"> Filters for driver activity with VIN and Driver ID </param>
        /// <param name="context">GRPC Context</param>
        /// <returns>Driver activity by type column</returns>
        public override async Task<DriverActivityResponce> GetDriverActivity(ActivityFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetDriverActivity for single driver.");
                ReportComponent.entity.DriverActivityFilter objActivityFilter = new ReportComponent.entity.DriverActivityFilter();
                objActivityFilter.VIN.Append(request.VINs);
                objActivityFilter.StartDateTime = request.StartDateTime;
                objActivityFilter.EndDateTime = request.EndDateTime;

                var result = await _reportManager.GetDriverActivity(objActivityFilter);
                DriverActivityResponce response = new DriverActivityResponce();
                if (result?.Count > 0)
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.DriverActivities.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<DriverActivity>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = "No Result Found";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new DriverActivityResponce
                {
                    Code = Responsecode.Failed,
                    Message = "GetDriverActivity get failed due to - " + ex.Message
                });
            }
        }

        #endregion
    }
}
