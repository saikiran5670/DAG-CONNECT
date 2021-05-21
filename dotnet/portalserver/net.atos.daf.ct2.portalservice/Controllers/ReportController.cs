using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.reportservice;
using net.atos.daf.ct2.portalservice.Common;
using System.Reflection;
using System.Threading.Tasks;
using static net.atos.daf.ct2.reportservice.ReportService;
using Report = net.atos.daf.ct2.portalservice.Entity.Report;
using net.atos.daf.ct2.portalservice.Entity.Report;
using System;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("report")]
    public class ReportController : ControllerBase
    {
        private ILog _logger;
        private readonly ReportServiceClient _reportServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly HeaderObj _userDetails;
        private readonly Report.Mapper _mapper;

        public ReportController(ReportServiceClient reportServiceClient,
                               AuditHelper auditHelper,
                               Common.AccountPrivilegeChecker privilegeChecker,
                               IHttpContextAccessor httpContextAccessor)
        {
            _reportServiceClient = reportServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _privilegeChecker = privilegeChecker;
            _userDetails = _auditHelper.GetHeaderData(httpContextAccessor.HttpContext.Request);
            _mapper = new Report.Mapper();
        }

        #region Select User Preferences
        [HttpGet]
        [Route("getuserpreferencereportdatacolumn")]
        public async Task<IActionResult> GetUserPreferenceReportDataColumn(int reportId, int accountId, int organizationId)
        {
            try
            {
                if (!(reportId > 0)) return BadRequest("Report id cannot be zero.");
                if (!(accountId > 0)) return BadRequest("Account id cannot be zero.");
                var response = await _reportServiceClient.GetUserPreferenceReportDataColumnAsync(new IdRequest { ReportId = reportId, AccountId = accountId, OrganizationId = organizationId });
                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response);
                if (response.Code == Responsecode.Failed)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, accountId, reportId, ReportConstants.USER_PREFERENCE_FAILURE_MSG2));
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, accountId, reportId, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Report Controller",
                 "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"GetUserPreferenceReportDataColumn method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(accountId),
                  Request);
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        [HttpPost]
        [Route("createuserpreference")]
        public async Task<IActionResult> CreateUserPreference(net.atos.daf.ct2.portalservice.Entity.Report.UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            try
            {
                var request = _mapper.MapCreateUserPrefences(objUserPreferenceCreateRequest);
                var response = await _reportServiceClient.CreateUserPreferenceAsync(request);
                if (response == null)
                    return StatusCode(500, "Internal Server Error.");

                switch (response.Code)
                {
                    case Responsecode.Success:
                        return Ok(response);
                    case Responsecode.Failed:
                        return StatusCode((int)response.Code, response.Message);
                    case Responsecode.InternalServerError:
                        return StatusCode((int)response.Code, response.Message);
                    default:
                        return StatusCode((int)response.Code, response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Report Controller",
                                 "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                 $"createuserpreference method Failed. Error:{ex.Message}", 0, 0, JsonConvert.SerializeObject(objUserPreferenceCreateRequest),
                                  Request);
                _logger.Error(null, ex);
                return StatusCode(500, $"{ex.Message} {ex.StackTrace}");
            }
        }
    }
}
