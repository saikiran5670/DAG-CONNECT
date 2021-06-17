using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.ReportScheduler;
using net.atos.daf.ct2.reportschedulerservice;
using PortalAlertEntity = net.atos.daf.ct2.portalservice.Entity.ReportScheduler;
namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("reportscheduler")]
    public class ReportSchedulerController : BaseController
    {
        private ILog _logger;
        private readonly ReportSchedulerService.ReportSchedulerServiceClient _reportschedulerClient;
        private readonly AuditHelper _auditHelper;
        private readonly Entity.ReportScheduler.Mapper _mapper;
        public ReportSchedulerController(ReportSchedulerService.ReportSchedulerServiceClient reportschedulerClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _reportschedulerClient = reportschedulerClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _mapper = new Entity.ReportScheduler.Mapper();
        }

        #region Get Report Scheduler Paramenter
        [HttpGet]
        [Route("getreportschedulerparameter")]
        public async Task<IActionResult> GetReportScheduleraParameter(int accountId, int orgnizationid, int roleid)
        {
            try
            {
                if (orgnizationid == 0) return BadRequest(ReportSchedulerConstants.REPORTSCHEDULER_ORG_ID_NOT_NULL_MSG);
                ReportParameterResponse response = await _reportschedulerClient.GetReportParameterAsync(new ReportParameterRequest { AccountId = accountId, OrganizationId = orgnizationid, RoleId = roleid });

                if (response == null)
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE);
                if (response.Code == ResponseCode.Success)
                    return Ok(response);
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportSchedulerConstants.REPORTSCHEDULER_PARAMETER_NOT_FOUND_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                 ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "GetReportScheduleraParameter", ex.Message), 1, 2, Convert.ToString(accountId),
                  _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion


        #region Create Schedular Report
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateReportScheduler(PortalAlertEntity.ReportScheduler request)
        {
            try
            {
                ReportSchedulerRequest reportSchedulerRequest = _mapper.ToReportSchedulerEntity(request);
                ReportSchedulerResponse reportSchedulerResponse = new ReportSchedulerResponse();
                reportSchedulerResponse = _reportschedulerClient.CreateReportScheduler(reportSchedulerRequest);

                if (reportSchedulerResponse != null && reportSchedulerResponse.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_CREATE_FAILED_MSG);
                }
                else if (reportSchedulerResponse != null && reportSchedulerResponse.Code == ResponseCode.Conflict)
                {
                    return StatusCode(409, reportSchedulerResponse.Message);
                }
                else if (reportSchedulerResponse != null && reportSchedulerResponse.Code == ResponseCode.Success)
                {
                    return Ok(reportSchedulerResponse);
                }
                else
                {
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_CREATE_FAILED_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                  ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "CreateReportScheduler", ex.Message), 1, 2, "1",
                   _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion
    }
}
