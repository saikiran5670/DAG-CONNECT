using Grpc.Core;
using log4net;
using net.atos.daf.ct2.alert.ENUM;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reportservice.entity;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
                var userPrefernces = await _reportManager.GetUserPreferenceReportDataColumn(request.ReportId, request.AccountId);
                if(userPrefernces.Count() == 0)
                {
                    return  await Task.FromResult(new UserPreferenceDataColumnResponse
                    {
                        Message = String.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, request.ReportId, request.AccountId, ReportConstants.USER_PREFERENCE_FAILURE_MSG2),
                        Code = Responsecode.Failed
                    }); 
                }
                if (!userPrefernces.Any(a => !string.IsNullOrEmpty(a.IsExclusive))) {
                    foreach (var userpreferece in userPrefernces) {
                        userpreferece.IsExclusive = ((char)IsExclusive.No).ToString();
                    }
                }

                var response = new UserPreferenceDataColumnResponse
                {                    
                    Message = String.Format(ReportConstants.USER_PREFERENCE_SUCCESS_MSG, request.ReportId, request.AccountId),
                    Code = Responsecode.Success
                };
                response.UserPreferences.AddRange(_mapper.GetUserPrefences(userPrefernces));
                return await Task.FromResult(response); ;

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
    }
}
