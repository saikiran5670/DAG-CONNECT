using Grpc.Core;
using log4net;
using net.atos.daf.ct2.reports.ENUM;
using net.atos.daf.ct2.reports;
using ReportComponent = net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reportservice.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.reportservice.Services
{
    public class ReportManagementService : ReportService.ReportServiceBase
    {
        private ILog _logger;
        private readonly IReportManager _reportManager;
        private readonly Mapper _mapper;
        public ReportManagementService(IReportManager reportManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _reportManager = reportManager;
            _mapper = new Mapper();
        }

        #region Select User Preferences
        public override async Task<UserPreferenceDataColumnResponse> GetUserPreferenceReportDataColumn(IdRequest request, ServerCallContext context)
        {
            try
            {
                var userPrefernces = await _reportManager.GetUserPreferenceReportDataColumn(request.ReportId, request.AccountId, request.OrganizationId);
                if(userPrefernces.Count() == 0)
                {
                    return await Task.FromResult(new UserPreferenceDataColumnResponse
                    {
                        Message = String.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, request.AccountId, request.ReportId, ReportConstants.USER_PREFERENCE_FAILURE_MSG2),
                        Code = Responsecode.Failed
                    });
                }
                if (!userPrefernces.Any(a => !string.IsNullOrEmpty(a.State))) 
                {
                    var roleBasedUserPrefernces = await _reportManager.GetRoleBasedDataColumn(request.ReportId, request.AccountId, request.OrganizationId);
                    var roleBadresponse = new UserPreferenceDataColumnResponse
                    {
                        Message = String.Format(ReportConstants.USER_PREFERENCE_SUCCESS_MSG, request.AccountId, request.ReportId),
                        Code = Responsecode.Success
                    };
                    roleBadresponse.UserPreferences.AddRange(_mapper.MapUserPrefences(roleBasedUserPrefernces));
                    return await Task.FromResult(roleBadresponse);
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
                    Message = ex.Message,
                    Code = Responsecode.InternalServerError
                });
            }
        }
        #endregion

        #region Get Vins from data mart trip_statistics
        //This code is not in use, may require in future use.
        public override async Task<VehicleFilterResponse> GetVinsFromTripStatistics(VehicleFilterRequest request, ServerCallContext context)
        {
            try
            {
                var vinList = await _reportManager.GetVinsFromTripStatistics(request.FromDate, request.ToDate, request.VinList);

                var response = new VehicleFilterResponse
                {
                    Message = ReportConstants.GET_VIN_SUCCESS_MSG,
                    Code = Responsecode.Success
                };
                response.VinList.AddRange(_mapper.MapVinList(vinList));
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                var errorResponse = new VehicleFilterResponse
                {
                    Message = ex.Message,
                    Code = Responsecode.InternalServerError
                };
                errorResponse.VinList.Add(new List<string>());
                return await Task.FromResult(errorResponse);
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
                if (result.Count > 0)
                {
                    var res = JsonConvert.SerializeObject(result);
                    response.TripData.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<TripDetils>>(res)
                        );

                    //foreach (ReportComponent.entity.TripFilterRequest entity in result)
                    //{
                    //    response.TripData.Add(_mapper.ToTripResponce(entity));
                    //}
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
    }
}
