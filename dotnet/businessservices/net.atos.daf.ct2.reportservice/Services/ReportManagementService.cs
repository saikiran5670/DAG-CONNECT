using System;
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

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        private readonly ILog _logger;
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

        public override async Task<ReportDetailsResponse> GetReportDetails(TempPara Id, ServerCallContext context)
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
    }
}
