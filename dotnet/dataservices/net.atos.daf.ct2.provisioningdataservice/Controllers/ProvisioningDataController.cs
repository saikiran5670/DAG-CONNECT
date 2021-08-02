using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.audit;
using System;
using log4net;
using System.Net;
using System.Threading.Tasks;
using System.Reflection;
using net.atos.daf.ct2.provisioningdataservice.Entity;
using net.atos.daf.ct2.audit.Enum;
using System.Linq;
using Newtonsoft.Json;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.provisioningdataservice.CustomAttributes;
using net.atos.daf.ct2.driver.entity;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.driver;

namespace net.atos.daf.ct2.provisioningdataservice.Controllers
{
    [ApiController]
    [Route("provisioning")]
    //[Authorize(Policy = AccessPolicies.MAIN_ACCESS_POLICY)]
    public class ProvisioningDataController : ControllerBase
    {
        private readonly IAuditTraillib _auditTrail;
        private readonly ILog _logger;
        private readonly IAccountManager _accountManager;
        private readonly IOrganizationManager _organizationManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IDriverManager _driverManager;
        private readonly IConfiguration _configuration;
        public ProvisioningDataController(IAuditTraillib auditTrail, IDriverManager driverManager, IAccountManager accountManager, IOrganizationManager organizationManager, IVehicleManager vehicleManager, IConfiguration configuration)
        {
            _accountManager = accountManager;
            _organizationManager = organizationManager;
            _vehicleManager = vehicleManager;
            _driverManager = driverManager;
            _auditTrail = auditTrail;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _configuration = configuration;
        }

        #region Vehicles

        [HttpGet]
        [Route("vehicle/current")]
        public async Task<IActionResult> GetCurrentVehicle([FromQuery] VehicleCurrentRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetCurrentVehicle), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get current vehicle in Provisioning data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is OkObjectResult)
                {
                    var response = await _vehicleManager.GetCurrentVehicle(MapRequest(request, (int)(result as ObjectResult).Value));

                    return Ok(response);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Provisioning - Current Vehicle request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetCurrentVehicle), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get current vehicle in Provisioning data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpGet]
        [Route("vehicle/list")]
        public async Task<IActionResult> GetVehicleList([FromQuery] VehicleListRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetVehicleList), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get vehicle list in Provisioning data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is OkObjectResult)
                {
                    var response = await _vehicleManager.GetVehicleList(MapRequest(request, (int)(result as ObjectResult).Value));

                    return Ok(response);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Provisioning - Vehicle List request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetVehicleList), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get vehicle list in Provisioning data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Drivers

        [HttpGet]
        [Route("driver/current")]
        public async Task<IActionResult> GetCurrentDriver([FromQuery] DriverCurrentRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetCurrentDriver), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get current driver in Provisioning data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is NoContentResult)
                {
                    var response = await _driverManager.GetCurrentDriver(MapRequest(request));

                    return Ok(response);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Provisioning - Current Driver request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetCurrentDriver), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get current driver in Provisioning data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpGet]
        [Route("driver/list")]
        public async Task<IActionResult> GetDriverList([FromQuery] DriverListRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetDriverList), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get driver list in Provisioning data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is OkObjectResult)
                {
                    var visibleVehicles = await _vehicleManager.GetVisibilityVehiclesByOrganization((int)(result as ObjectResult).Value);
                    var response = await _driverManager.GetDriverList(MapRequest(request, visibleVehicles.Select(x => x.VIN).ToArray()));

                    return Ok(response);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Provisioning - Driver List request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetDriverList), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get driver list in Provisioning data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Organisations

        [HttpGet]
        [Route("organisation/list")]
        public async Task<IActionResult> GetOrganisationList([FromQuery] OrganisationRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetOrganisationList), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get organisation list in Provisioning data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is NoContentResult)
                {
                    var response = await _organizationManager.GetOrganisationList(MapRequest(request));

                    return Ok(response);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Provisioning - Organisation List request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Provisioning Data Service", nameof(GetOrganisationList), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get organisation list in Provisioning data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Model Validations

        private async Task<IActionResult> ValidateParameters(VehicleCurrentRequest request)
        {
            var account = await _accountManager.GetAccountByEmailId(request.Account);
            if (account == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.Account));

            if (string.IsNullOrEmpty(account.DriverId) || (!string.IsNullOrEmpty(account.DriverId) && !account.DriverId.Equals(request.DriverId)))
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.DriverId));

            var org = await _organizationManager.GetOrganizationByOrgCode(request.OrgId);
            if (org == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ORGANIZATION_NOT_FOUND", parameter: nameof(request.OrgId));

            var orgs = await _accountManager.GetAccountOrg(account.Id);
            if (orgs.Select(x => x.Id).ToArray().Contains(org.Id))
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.Account));

            return new OkObjectResult(org.Id);
        }

        private async Task<IActionResult> ValidateParameters(VehicleListRequest request)
        {
            var org = await _organizationManager.GetOrganizationByOrgCode(request.OrgId);
            if (org == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ORGANIZATION_NOT_FOUND", parameter: nameof(request.OrgId));

            if (!string.IsNullOrEmpty(request.Account))
            {
                var account = await _accountManager.GetAccountByEmailId(request.Account);
                if (account == null)
                    return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.Account));

                if (string.IsNullOrEmpty(account.DriverId) || (!string.IsNullOrEmpty(account.DriverId) && !account.DriverId.Equals(request.DriverId)))
                    return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.DriverId));

                var orgs = await _accountManager.GetAccountOrg(account.Id);
                if (orgs.Select(x => x.Id).ToArray().Contains(org.Id))
                    return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.Account));
            }

            return new OkObjectResult(org.Id);
        }

        private async Task<IActionResult> ValidateParameters(DriverCurrentRequest request)
        {
            var org = await _organizationManager.GetOrganizationByOrgCode(request.OrgId);
            if (org == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ORGANIZATION_NOT_FOUND", parameter: nameof(request.OrgId));

            var vehicleList = await _vehicleManager.Get(new VehicleFilter() { VIN = request.VIN });
            var vehicle = vehicleList.FirstOrDefault();
            if (vehicle == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "VIN_NOT_FOUND", parameter: nameof(request.VIN));

            var visibleVehicles = await _vehicleManager.GetVisibilityVehiclesByOrganization(org.Id);
            if (!visibleVehicles.Any(x => x.VIN.Contains(vehicle.VIN)))
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "VIN_NOT_FOUND", parameter: nameof(request.VIN));

            return NoContent();
        }

        private async Task<IActionResult> ValidateParameters(DriverListRequest request)
        {
            var org = await _organizationManager.GetOrganizationByOrgCode(request.OrgId);
            if (org == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ORGANIZATION_NOT_FOUND", parameter: nameof(request.OrgId));

            return new OkObjectResult(org.Id);
        }

        private async Task<IActionResult> ValidateParameters(OrganisationRequest request)
        {
            if (((!string.IsNullOrEmpty(request.Account) && string.IsNullOrEmpty(request.DriverId)) ||
                (string.IsNullOrEmpty(request.Account) && !string.IsNullOrEmpty(request.DriverId))) && string.IsNullOrEmpty(request.VIN))
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "MISSING_PARAMETER", parameter: $"{ nameof(request.Account) } - { nameof(request.DriverId) } combination");
            }

            if (!string.IsNullOrEmpty(request.Account) && !string.IsNullOrEmpty(request.DriverId) && !string.IsNullOrEmpty(request.VIN))
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_PARAMETER", parameter: $"{ nameof(request.Account) } - { nameof(request.DriverId) } and { nameof(request.VIN) } combination");
            }

            if (string.IsNullOrEmpty(request.Account) && string.IsNullOrEmpty(request.DriverId) && string.IsNullOrEmpty(request.VIN))
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "MISSING_PARAMETER", parameter: $"{ nameof(request.Account) } - { nameof(request.DriverId) } or { nameof(request.VIN) }");
            }

            if (string.IsNullOrEmpty(request.Account) && string.IsNullOrEmpty(request.DriverId) && string.IsNullOrEmpty(request.VIN))
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "MISSING_PARAMETER", parameter: $"{ nameof(request.Account) } - { nameof(request.DriverId) } or { nameof(request.VIN) }");
            }

            if (!string.IsNullOrEmpty(request.Account) && !string.IsNullOrEmpty(request.DriverId))
            {
                var account = await _accountManager.GetAccountByEmailId(request.Account);
                if (account == null)
                    return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.Account));

                if (string.IsNullOrEmpty(account.DriverId) || (!string.IsNullOrEmpty(account.DriverId) && !account.DriverId.Equals(request.DriverId)))
                    return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.DriverId));
            }

            if (!string.IsNullOrEmpty(request.VIN))
            {
                var vehicle = await _vehicleManager.Get(new VehicleFilter() { VIN = request.VIN });
                if (vehicle.FirstOrDefault() == null)
                    return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "VIN_NOT_FOUND", parameter: nameof(request.VIN));
            }

            return NoContent();
        }

        #endregion

        #region Map Requests

        private ProvisioningVehicleDataServiceRequest MapRequest(VehicleCurrentRequest request, int orgId)
        {
            return new ProvisioningVehicleDataServiceRequest
            {
                Account = request.Account,
                DriverId = request.DriverId,
                OrgId = orgId,
                StartTimestamp = request.StartTimestamp,
                EndTimestamp = request.EndTimestamp
            };
        }

        private ProvisioningVehicleDataServiceRequest MapRequest(VehicleListRequest request, int orgId)
        {
            return new ProvisioningVehicleDataServiceRequest
            {
                Account = request.Account,
                DriverId = request.DriverId,
                OrgId = orgId,
                StartTimestamp = request.StartTimestamp,
                EndTimestamp = request.EndTimestamp
            };
        }

        private ProvisioningDriverDataServiceRequest MapRequest(DriverCurrentRequest request)
        {
            return new ProvisioningDriverDataServiceRequest
            {
                VIN = request.VIN,
                OrgId = request.OrgId,
                StartTimestamp = request.StartTimestamp,
                EndTimestamp = request.EndTimestamp
            };
        }

        private ProvisioningDriverDataServiceRequest MapRequest(DriverListRequest request, string[] vins)
        {
            return new ProvisioningDriverDataServiceRequest
            {
                VINs = vins,
                OrgId = request.OrgId,
                StartTimestamp = request.StartTimestamp,
                EndTimestamp = request.EndTimestamp
            };
        }

        private ProvisioningOrganisationDataServiceRequest MapRequest(OrganisationRequest request)
        {
            return new ProvisioningOrganisationDataServiceRequest
            {
                Account = request.Account,
                DriverId = request.DriverId,
                VIN = request.VIN,
                StartTimestamp = request.StartTimestamp,
                EndTimestamp = request.EndTimestamp
            };
        }

        #endregion

        #region Generate Error Response

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string errorCode, string parameter)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = errorCode,
                Value = parameter
            });
        }

        #endregion
    }
}
