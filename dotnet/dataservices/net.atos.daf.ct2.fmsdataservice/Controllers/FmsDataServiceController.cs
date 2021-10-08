﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.fmsdataservice.entity;
using Newtonsoft.Json;
using net.atos.daf.ct2.fms;
using net.atos.daf.ct2.fms.entity;
using net.atos.daf.ct2.fmsdataservice.customattributes;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.utilities;
using System.Net;
using System.Linq;
using System.Security.Claims;
using net.atos.daf.ct2.account;
using Microsoft.Extensions.Primitives;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.fmsdataservice.Entity;
using System.Collections.Generic;

namespace net.atos.daf.ct2.fmsdataservice.controllers
{
    [Route("vehicle")]
    [ApiController]
    public class FmsDataServiceController : ControllerBase
    {
        private readonly ILogger<FmsDataServiceController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAuditTraillib _auditTrail;
        private readonly IFmsManager _fmsManager;
        private readonly IAccountManager _accountManager;
        private readonly IVehicleManager _vehicleManager;

        internal int AccountId { get; set; }
        internal int OrgId { get; set; }

        public FmsDataServiceController(IAuditTraillib auditTrail
            , ILogger<FmsDataServiceController> logger
            , IConfiguration configuration
            , IFmsManager fmsManager
            , IAccountManager accountManager
            , IVehicleManager vehicleManager)
        {
            _logger = logger;
            _auditTrail = auditTrail;
            _accountManager = accountManager;
            _configuration = configuration;
            _fmsManager = fmsManager;
            _vehicleManager = vehicleManager;
        }

        [HttpGet]
        [Route("position/current")]
        //[Authorize(Policy = AccessPolicies.FMS_VEHICLE_POSITION_ACCESS_POLICY)]
        public async Task<IActionResult> Position([FromQuery] VehiclePositionRequest vehiclePositionRequest)
        {
            try
            {
                //var selectedType = string.Empty;
                //long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                //this.Request.Headers.TryGetValue("Accept", out StringValues acceptHeader);

                //this.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues xCorrelationId);

                //if (!this.Request.Headers.ContainsKey("X-Correlation-ID") || (this.Request.Headers.ContainsKey("X-Correlation-ID") && acceptHeader.Count() == 0))
                //    return GenerateErrorResponse(HttpStatusCode.BadRequest, "X-Correlation-ID", "INVALID_PARAMETER");

                //if (!this.Request.Headers.ContainsKey("Accept") || (this.Request.Headers.ContainsKey("Accept") && acceptHeader.Count() == 0))
                //    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept", "INVALID_PARAMETER");

                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "FMS Data Service Postion", "FMS data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.PARTIAL, "FMS dataservice position received object", 0, 0, JsonConvert.SerializeObject(vehiclePositionRequest), 0, 0);
                _logger.LogInformation("Fms vehicle position function called - " + vehiclePositionRequest.VIN, vehiclePositionRequest.Since);
                //if (acceptHeader.Any(x => x.Trim().Equals(FMSResponseTypeConstants.ACCPET_TYPE_VEHICLE_JSON, StringComparison.CurrentCultureIgnoreCase)))
                //    selectedType = FMSResponseTypeConstants.ACCPET_TYPE_VEHICLE_JSON;
                //else
                //    return GenerateErrorResponse(HttpStatusCode.NotAcceptable, "Accept", "NOT_ACCEPTABLE value in accept - " + acceptHeader);
                net.atos.daf.ct2.fms.entity.VehiclePositionResponse vehiclePositionResponse = null;
                await GetUserDetails();
                var visibleVehicles = await _vehicleManager.GetVisibilityVehicles(AccountId, OrgId);
                List<string> objVisibilityVinList = new List<string>();
                if (!string.IsNullOrEmpty(vehiclePositionRequest.VIN))
                {
                    var dataVisibleVehicle = visibleVehicles.Select(a => a.Value).ToList();
                    foreach (var item in dataVisibleVehicle)
                    {
                        foreach (var i in item)
                        {
                            objVisibilityVinList.Add(i.VIN);
                        }
                    }
                    vehiclePositionResponse = await _fmsManager.GetVehiclePosition(objVisibilityVinList, vehiclePositionRequest.Since);
                    if (vehiclePositionResponse != null && vehiclePositionResponse.VehiclePosition.Count > 0)
                    {
                        return Ok(vehiclePositionResponse);
                    }
                    else
                    {
                        return StatusCode(400, string.Empty);
                    }
                }
                string since = vehiclePositionRequest.Since;
                var isValid = ValidateParameter(ref since, out bool isNumeric);

                if (isValid)
                {
                    bool isPassedVinInVisibility = false;
                    if (visibleVehicles.Count > 0)//case to chek if the user has visibility
                    {
                        var dataVisibleVehicle = visibleVehicles.Select(a => a.Value).ToList();
                        if (dataVisibleVehicle != null && dataVisibleVehicle.Count > 0)
                        {
                            foreach (var item in dataVisibleVehicle)
                            {
                                foreach (var i in item)
                                {
                                    if (i.VIN.Contains(vehiclePositionRequest.VIN))
                                    {
                                        isPassedVinInVisibility = true;
                                    }
                                }
                            }
                        }
                        if (isPassedVinInVisibility)
                        {
                            vehiclePositionResponse = await _fmsManager.GetVehiclePosition(vehiclePositionRequest.VIN, vehiclePositionRequest.Since);
                        }
                        else
                        {
                            return StatusCode(400, "Vin not in Visiblity");
                        }
                    }
                    else
                    {
                        return StatusCode(400, $"No vehicle found for Account Id {AccountId} and Organization Id {OrgId}");
                    }
                    if (vehiclePositionResponse != null && vehiclePositionResponse.VehiclePosition.Count > 0)
                    {
                        return Ok(vehiclePositionResponse);
                    }
                    else
                    {
                        return StatusCode(400, string.Empty);
                    }
                }
                else
                {
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(since));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing fms vehicle position data.");
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Fms Data Service", "Fms data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "fms data service position object", 0, 0, JsonConvert.SerializeObject(vehiclePositionRequest), 0, 0);
                return StatusCode(500, string.Empty);
            }
        }
        [HttpGet]
        [Route("status/current")]
        //[Authorize(Policy = AccessPolicies.FMS_VEHICLE_STATUS_ACCESS_POLICY)]
        public async Task<IActionResult> Status([FromQuery] VehicleStatusRequest vehicleStatusRequest)
        {
            try
            {
                //var selectedType = string.Empty;
                //long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                //this.Request.Headers.TryGetValue("Accept", out StringValues acceptHeader);

                //this.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues xCorrelationId);

                //if (!this.Request.Headers.ContainsKey("X-Correlation-ID") || (this.Request.Headers.ContainsKey("X-Correlation-ID") && acceptHeader.Count() == 0))
                //    return GenerateErrorResponse(HttpStatusCode.BadRequest, "X-Correlation-ID", "INVALID_PARAMETER");

                //if (!this.Request.Headers.ContainsKey("Accept") || (this.Request.Headers.ContainsKey("Accept") && acceptHeader.Count() == 0))
                //    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept", "INVALID_PARAMETER");

                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "FMS Data Service Status", "FMS data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.PARTIAL, "FMS dataservice status received object", 0, 0, JsonConvert.SerializeObject(vehicleStatusRequest), 0, 0);
                _logger.LogInformation("Fms vehicle status function called - " + vehicleStatusRequest.VIN, vehicleStatusRequest.Since);

                //if (acceptHeader.Any(x => x.Trim().Equals(FMSResponseTypeConstants.ACCPET_TYPE_VEHICLE_JSON, StringComparison.CurrentCultureIgnoreCase)))
                //    selectedType = FMSResponseTypeConstants.ACCPET_TYPE_VEHICLE_JSON;
                //else
                //    return GenerateErrorResponse(HttpStatusCode.NotAcceptable, "Accept", "NOT_ACCEPTABLE value in accept - " + acceptHeader);
                await GetUserDetails();
                var visibleVehicles = await _vehicleManager.GetVisibilityVehicles(AccountId, OrgId);
                net.atos.daf.ct2.fms.entity.VehicleStatusResponse vehicleStatusResponse = null;
                List<string> objVisibilityVinList = new List<string>();
                if (string.IsNullOrEmpty(vehicleStatusRequest.VIN))
                {
                    var dataVisibleVehicle = visibleVehicles.Select(a => a.Value).ToList();
                    foreach (var item in dataVisibleVehicle)
                    {
                        foreach (var i in item)
                        {
                            objVisibilityVinList.Add(i.VIN);
                        }
                    }
                    vehicleStatusResponse = await _fmsManager.GetVehicleStatus(objVisibilityVinList, vehicleStatusRequest.Since);
                    if (vehicleStatusResponse != null && vehicleStatusResponse.VehicleStatus.Count > 0)
                    {
                        return Ok(vehicleStatusResponse);
                    }
                    else
                    {
                        return StatusCode(400, string.Empty);
                    }
                }
                string since = vehicleStatusRequest.Since;
                var isValid = ValidateParameter(ref since, out bool isNumeric);
                if (isValid)
                {
                    bool isPassedVinInVisibility = false;
                    if (visibleVehicles.Count > 0)//case to chek if the user has visibility
                    {
                        var dataVisibleVehicle = visibleVehicles.Select(a => a.Value).ToList();
                        if (dataVisibleVehicle != null && dataVisibleVehicle.Count > 0)
                        {
                            foreach (var item in dataVisibleVehicle)
                            {
                                foreach (var i in item)
                                {
                                    if (i.VIN.Contains(vehicleStatusRequest.VIN))
                                    {
                                        isPassedVinInVisibility = true;
                                    }
                                }
                            }
                        }
                        if (isPassedVinInVisibility)
                        {
                            vehicleStatusResponse = await _fmsManager.GetVehicleStatus(vehicleStatusRequest.VIN, vehicleStatusRequest.Since);
                        }
                        else
                        {
                            return StatusCode(400, "Vin not in Visiblity");
                        }
                    }
                    else
                    {
                        return StatusCode(400, $"No vehicle found for Account Id {AccountId} and Organization Id {OrgId}");
                    }
                    if (vehicleStatusResponse != null)
                    {
                        return Ok(vehicleStatusResponse);
                    }
                    else
                    {
                        return StatusCode(400, "No vehicle found with the VIN given");
                    }
                }
                else
                {
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(since));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing fms vehicle status data.");
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Fms Data Service", "Fms data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "fms data service status object", 0, 0, JsonConvert.SerializeObject(vehicleStatusRequest), 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpGet]
        [Route("vehicle/current")]
        //[Authorize(Policy = AccessPolicies.FMS_VEHICLE_STATUS_ACCESS_POLICY)]
        public async Task<IActionResult> Vehicle([FromQuery] VehicleStatusRequest vehicleStatusRequest)
        {
            try
            {
                //var selectedType = string.Empty;
                //long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                //this.Request.Headers.TryGetValue("Accept", out StringValues acceptHeader);

                //this.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues xCorrelationId);

                //if (!this.Request.Headers.ContainsKey("X-Correlation-ID") || (this.Request.Headers.ContainsKey("X-Correlation-ID") && acceptHeader.Count() == 0))
                //    return GenerateErrorResponse(HttpStatusCode.BadRequest, "X-Correlation-ID", "INVALID_PARAMETER");

                //if (!this.Request.Headers.ContainsKey("Accept") || (this.Request.Headers.ContainsKey("Accept") && acceptHeader.Count() == 0))
                //    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept", "INVALID_PARAMETER");

                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "FMS Data Service Vehicle", "FMS data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.PARTIAL, "FMS dataservice vehicle received object", 0, 0, JsonConvert.SerializeObject(vehicleStatusRequest), 0, 0);
                _logger.LogInformation("Fms Vehicle function called - ");

                //if (acceptHeader.Any(x => x.Trim().Equals(FMSResponseTypeConstants.ACCPET_TYPE_VEHICLE_JSON, StringComparison.CurrentCultureIgnoreCase)))
                //    selectedType = FMSResponseTypeConstants.ACCPET_TYPE_VEHICLE_JSON;
                //else
                //    return GenerateErrorResponse(HttpStatusCode.NotAcceptable, "Accept", "NOT_ACCEPTABLE value in accept - " + acceptHeader);
                await GetUserDetails();
                var visibleVehicles = await _vehicleManager.GetVisibilityVehicles(AccountId, OrgId);
                List<string> objVisibilityVinList = new List<string>();
                var dataVisibleVehicle = visibleVehicles.Select(a => a.Value).ToList();
                foreach (var item in dataVisibleVehicle)
                {
                    foreach (var i in item)
                    {
                        objVisibilityVinList.Add(i.VIN);
                    }
                }
                return Ok(objVisibilityVinList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing fms vehicle status data.");
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Fms Data Service", "Fms data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "fms data service status object", 0, 0, JsonConvert.SerializeObject(vehicleStatusRequest), 0, 0);
                return StatusCode(500, string.Empty);
            }
        }
        private bool ValidateParameter(ref string since, out bool isNumeric)
        {
            isNumeric = long.TryParse(since, out _);
            if (isNumeric)
            {
                string sTimezone = "UTC";
                try
                {
                    string converteddatetime = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(since), sTimezone, null);
                    if (!DateTime.TryParse(converteddatetime, out DateTime dDate))
                        return false;
                    else
                        since = converteddatetime;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else if (!(string.IsNullOrEmpty(since) || since.ToLower().Equals("yesterday") || since.ToLower().Equals("today")))
            {
                return false;
            }
            return true;
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string value)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = "INVALID_PARAMETER",
                Value = value
            });
        }
        #region Error Generator
        IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string value, string message)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = message,
                Value = value
            });
        }
        #endregion
        private async Task GetUserDetails()
        {
            var accountEmailId = User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
            var account = await _accountManager.GetAccountByEmailId(accountEmailId.Value.ToLower());
            var orgs = await _accountManager.GetAccountOrg(account.Id);
            OrgId = orgs.First().Id;
            AccountId = account.Id;
        }
    }
}

