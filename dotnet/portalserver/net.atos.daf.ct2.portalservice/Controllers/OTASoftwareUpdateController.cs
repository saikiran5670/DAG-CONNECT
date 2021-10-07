﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.otasoftwareupdateservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.OTASoftwareUpdate;
using static net.atos.daf.ct2.otasoftwareupdateservice.OTASoftwareUpdateService;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("otasoftwareupdate")]
    public class OTASoftwareUpdateController : BaseController
    {
        private readonly ILog _logger;
        private readonly OTASoftwareUpdateServiceClient _otaSoftwareUpdateServiceClient;
        private readonly AuditHelper _auditHelper;


        public OTASoftwareUpdateController(OTASoftwareUpdateServiceClient otaSoftwareUpdateServiceClient,
                               AuditHelper auditHelper,
                               IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _otaSoftwareUpdateServiceClient = otaSoftwareUpdateServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        #region GetVehicleSoftwareStatus
        [HttpGet]
        [Route("getvehiclesoftwarestatus")]
        public async Task<IActionResult> GetVehicleSoftwareStatus()
        {
            try
            {
                var response = await _otaSoftwareUpdateServiceClient.GetVehicleSoftwareStatusAsync(new NoRequest { Id = 0 });
                if (response == null)
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 1));
                if (response.Code == ResponseCode.Success)
                    return Ok(new { VehicleSoftwareStatus = response.VehicleSoftwareStatusList, Message = response.Message });
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(OTASoftwareUpdateConstants.VEHICLE_SOFTWARE_STATUS_FAILURE_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, OTASoftwareUpdateConstants.OTA_CONTROLLER_NAME,
                 OTASoftwareUpdateConstants.OTA_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(OTASoftwareUpdateConstants.OTA_EXCEPTION_LOG_MSG, "GetVehicleSoftwareStatus", ex.Message), 1, 2, string.Empty,
                  _userDetails);
                // check for fk violation
                if (ex.Message.Contains(OTASoftwareUpdateConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 2));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region GetVehicleStatusList
        [HttpGet]
        [Route("getvehicletatuslist")]
        public async Task<IActionResult> GetVehicleStatusList([FromQuery] string language, [FromQuery] string retention)
        {
            try
            {
                if (language == null || (language != null && language.Length < 2)) return StatusCode(400, OTASoftwareUpdateConstants.LANGUAGE_REQUIRED_MSG);
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());
                var response = await _otaSoftwareUpdateServiceClient
                    .GetVehicleStatusListAsync(new VehicleStatusRequest
                    {
                        AccountId = _userDetails.AccountId,
                        OrgId = GetUserSelectedOrgId(),
                        ContextOrgId = GetContextOrgId(),
                        FeatureId = featureId,
                        Language = language?.Substring(0, 2),
                        Retention = retention ?? "Active"
                    });
                if (response == null)
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 1));
                if (response.Code == ResponseCode.Success)
                    return Ok(new { VehicleStatusList = response.VehicleStatusList, Message = response.Message });
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(OTASoftwareUpdateConstants.VEHICLE_SOFTWARE_STATUS_FAILURE_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, OTASoftwareUpdateConstants.OTA_CONTROLLER_NAME,
                 OTASoftwareUpdateConstants.OTA_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(OTASoftwareUpdateConstants.OTA_EXCEPTION_LOG_MSG, "GetVehicleSoftwareStatus", ex.Message), 1, 2, string.Empty,
                  _userDetails);
                // check for fk violation
                if (ex.Message.Contains(OTASoftwareUpdateConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 2));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region GetVehicleUpdateDetails
        [HttpGet]
        [Route("getvehicleupdatedetails")]
        public async Task<IActionResult> GetVehicleUpdateDetails([FromQuery] string vin, [FromQuery] string retention)
        {
            try
            {
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());
                var response = await _otaSoftwareUpdateServiceClient
                    .GetVehicleUpdateDetailsAsync(new VehicleUpdateDetailRequest
                    {
                        Vin = vin,
                        Retention = retention
                    });
                if (response == null)
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 1));
                if (response.HttpStatusCode == ResponseCode.Success)
                    return Ok(new { VehicleUpdateDetails = response.VehicleUpdateDetail, Message = response.Message });
                if (response.HttpStatusCode == ResponseCode.InternalServerError)
                    return StatusCode((int)response.HttpStatusCode, String.Format(OTASoftwareUpdateConstants.VEHICLE_SOFTWARE_STATUS_FAILURE_MSG, response.Message));
                return StatusCode((int)response.HttpStatusCode, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, OTASoftwareUpdateConstants.OTA_CONTROLLER_NAME,
                 OTASoftwareUpdateConstants.OTA_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(OTASoftwareUpdateConstants.OTA_EXCEPTION_LOG_MSG, "GetVehicleSoftwareStatus", ex.Message), 1, 2, string.Empty,
                  _userDetails);
                // check for fk violation
                if (ex.Message.Contains(OTASoftwareUpdateConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 2));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region GetSoftwareReleaseNote
        [HttpGet]
        [Route("getsoftwarereleasenote")]
        public async Task<IActionResult> GetSoftwareReleaseNote([FromQuery] string campaignId, [FromQuery] string language, [FromQuery] string vin, [FromQuery] string retention)
        {
            try
            {
                var request = new CampiagnSoftwareReleaseNoteRequest
                {
                    Retention = retention,
                    CampaignId = campaignId,
                    Language = language
                };
                request.Vins.Add(vin);
                var response = await _otaSoftwareUpdateServiceClient
                                            .GetSoftwareReleaseNoteAsync(request);
                if (response == null)
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 1));
                if (response.HttpStatusCode == ResponseCode.Success)
                    return Ok(new { ReleaseNotes = response.ReleaseNotes, Message = response.Message });
                if (response.HttpStatusCode == ResponseCode.InternalServerError)
                    return StatusCode((int)response.HttpStatusCode, String.Format(OTASoftwareUpdateConstants.VEHICLE_SOFTWARE_STATUS_FAILURE_MSG, response.Message));
                return StatusCode((int)response.HttpStatusCode, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, OTASoftwareUpdateConstants.OTA_CONTROLLER_NAME,
                 OTASoftwareUpdateConstants.OTA_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(OTASoftwareUpdateConstants.OTA_EXCEPTION_LOG_MSG, "GetVehicleSoftwareStatus", ex.Message), 1, 2, string.Empty,
                  _userDetails);
                // check for fk violation
                if (ex.Message.Contains(OTASoftwareUpdateConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 2));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region schedulesoftwareupdate
        [HttpPost]
        [Route("getschedulesoftwareupdate")]
        public async Task<IActionResult> GetScheduleSoftwareUpdate([FromQuery] string campaignId, [FromQuery] string baselineId, [FromQuery] long scheduledDatetime, [FromQuery] string vin)
        {
            try
            {
                var request = new ScheduleSoftwareUpdateRequest
                {
                    CampaignId = campaignId,
                    BaseLineId = baselineId,
                    ScheduleDateTime = scheduledDatetime

                };
                request.Vins.Add(vin);
                var response = await _otaSoftwareUpdateServiceClient
                                            .GetScheduleSoftwareUpdateAsync(request);
                if (response == null)
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 1));
                if (response.HttpStatusCode == ResponseCode.Success)
                    return Ok(new { Message = response.Message });
                if (response.HttpStatusCode == ResponseCode.InternalServerError)
                    return StatusCode((int)response.HttpStatusCode, String.Format(OTASoftwareUpdateConstants.VEHICLE_SOFTWARE_STATUS_FAILURE_MSG, response.Message));
                return StatusCode((int)response.HttpStatusCode, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, OTASoftwareUpdateConstants.OTA_CONTROLLER_NAME,
                 OTASoftwareUpdateConstants.OTA_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(OTASoftwareUpdateConstants.OTA_EXCEPTION_LOG_MSG, "getschedulesoftwareupdate", ex.Message), 1, 2, string.Empty,
                  _userDetails);
                // check for fk violation
                if (ex.Message.Contains(OTASoftwareUpdateConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, String.Format(OTASoftwareUpdateConstants.INTERNAL_SERVER_ERROR_MSG, 2));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion
    }
}
