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
        public ReportSchedulerController(ReportSchedulerService.ReportSchedulerServiceClient reportschedulerClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _reportschedulerClient = reportschedulerClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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


        //#region Create Schedular Report
        //[HttpPost]
        //[Route("CreateReportSchedulerParameter")]
        //public async Task<IActionResult> CreateReportSchedulerParameter(ReportSchedulerRequest request)
        //{
        //    try
        //    {
        //        ReportSchedulerService.ReportSchedulerServiceClient reportResponse = await _reportschedulerClient.CreateReportScheduler();
        //        return StatusCode(500, ex.Message + " " + ex.StackTrace);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
        //          ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
        //         string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "GetReportScheduleraParameter", ex.Message), 1, 2, Convert.ToString(accountId),
        //           _userDetails);
        //        _logger.Error(null, ex);
        //        return StatusCode(500, ex.Message + " " + ex.StackTrace);
        //    }
        //}

        //#endregion
    }
}
