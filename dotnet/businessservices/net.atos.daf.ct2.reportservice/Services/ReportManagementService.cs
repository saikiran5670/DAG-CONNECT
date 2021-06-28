using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.ENUM;
using net.atos.daf.ct2.reportservice.entity;
using net.atos.daf.ct2.visibility;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        private readonly ILog _logger;
        private readonly IReportManager _reportManager;
        private readonly IVisibilityManager _visibilityManager;
        private readonly Mapper _mapper;
        private readonly IConfiguration _configuration;
        public ReportManagementService(IReportManager reportManager, IVisibilityManager visibilityManager, IConfiguration configuration)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _reportManager = reportManager;
            _visibilityManager = visibilityManager;
            _mapper = new Mapper();
            _configuration = configuration;
        }

        #region Select User Preferences

        public override async Task<ReportDetailsResponse> GetReportDetails(TempPara id, ServerCallContext context)
        {
            try
            {
                var reportDetails = await _reportManager.GetReportDetails();
                var reportDetailsResponse = new ReportDetailsResponse
                {
                    Code = Responsecode.Success,
                    Message = ReportConstants.GET_REPORT_DETAILS_SUCCESS_MSG
                };
                var res = JsonConvert.SerializeObject(reportDetails);
                reportDetailsResponse.ReportDetails.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<ReportDetails>>(res)
                    );
                return await Task.FromResult(reportDetailsResponse);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ReportDetailsResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = ex.Message
                });
            }
        }
        public override async Task<UserPreferenceDataColumnResponse> GetUserPreferenceReportDataColumn(IdRequest request, ServerCallContext context)
        {
            try
            {
                IEnumerable<UserPreferenceReportDataColumn> userPreferences = null;
                var userPreferencesExists = await _reportManager.CheckIfUserPreferencesExist(request.ReportId, request.AccountId, request.OrganizationId);
                var roleBasedUserPreferences = await _reportManager.GetRoleBasedDataColumn(request.ReportId, request.AccountId, request.RoleId, request.OrganizationId, request.ContextOrgId);

                if (userPreferencesExists)
                {
                    var preferences = await _reportManager.GetReportUserPreference(request.ReportId, request.AccountId, request.OrganizationId);

                    //Filter out preferences based on Account role and org package subscription
                    userPreferences = preferences.Where(x => roleBasedUserPreferences.Any(y => y.DataAtrributeId == x.DataAtrributeId));
                }
                else
                {
                    userPreferences = roleBasedUserPreferences;
                }

                var response = new UserPreferenceDataColumnResponse();
                response.Code = Responsecode.Success;
                response.UserPreferences.AddRange(_mapper.MapUserPreferences(userPreferences));
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return new UserPreferenceDataColumnResponse
                {
                    Message = ex.Message,
                    Code = Responsecode.InternalServerError
                };
            }
        }
        #endregion

        #region Create User Preference
        public override async Task<UserPreferenceCreateResponse> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceCreateRequest, ServerCallContext context)
        {
            try
            {
                _logger.Info("CreateUserPreference method in ReportManagement service called.");

                int insertedUserPreferenceCount = await _reportManager.CreateUserPreference(_mapper.MapCreateUserPreferences(objUserPreferenceCreateRequest));
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

        #region - Common search parameter for all reports

        public override async Task<ReportSearchParameterResponse> GetReportSearchParameter(IdRequestForDriverActivity request, ServerCallContext context)
        {
            ReportSearchParameterResponse response = new ReportSearchParameterResponse();
            try
            {
                var visibleVehicle = await GetVisibleVINDetails(request.AccountId, request.OrganizationId);
                if (visibleVehicle?.Item1?.Count() > 0)
                {
                    response.VehicleDetailsWithAccountVisibiltyList.AddRange(visibleVehicle.Item1);
                    object lstDriver = await _reportManager.GetReportSearchParameterByVIN(request.ReportId, request.StartDateTime, request.EndDateTime, visibleVehicle.Item2);
                    if (lstDriver != null)
                    {
                        string resDrivers = JsonConvert.SerializeObject(lstDriver);
                        response.AdditionalSearchParameter.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<SearchParameter>>(resDrivers));
                    }
                    else
                    {
                        response.AdditionalSearchParameter.Add(new SearchParameter());
                    }
                }
                else
                {
                    response.AdditionalSearchParameter.Add(new SearchParameter());
                    response.VehicleDetailsWithAccountVisibiltyList.Add(new VehicleDetailsWithAccountVisibilty());
                }
                response.Code = Responsecode.Success;
                response.Message = Responsecode.Success.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ReportSearchParameterResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetDriverActivityParameters failed due to - " + ex.Message
                });
            }
            return await Task.FromResult(response);
        }
        #endregion
    }
}
