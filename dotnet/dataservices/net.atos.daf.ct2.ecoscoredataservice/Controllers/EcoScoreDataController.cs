using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.ecoscoredataservice.CustomAttributes;
using System;
using log4net;
using System.Net;
using System.Threading.Tasks;
using System.Reflection;
using net.atos.daf.ct2.ecoscoredataservice.Entity;
using net.atos.daf.ct2.audit.Enum;
using System.Linq;
using Newtonsoft.Json;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace net.atos.daf.ct2.ecoscoredataservice.Controllers
{
    [ApiController]
    [Route("ecoscore")]
    [Authorize(Policy = AccessPolicies.MAIN_ACCESS_POLICY)]
    public class EcoScoreDataController : ControllerBase
    {
        private readonly IAuditTraillib _auditTrail;
        private readonly ILog _logger;
        private readonly IReportManager _reportManager;
        private readonly IAccountManager _accountManager;
        private readonly IOrganizationManager _organizationManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IConfiguration _configuration;
        public EcoScoreDataController(IAuditTraillib auditTrail, IReportManager reportManager, IAccountManager accountManager, IOrganizationManager organizationManager, IVehicleManager vehicleManager, IConfiguration configuration)
        {
            _reportManager = reportManager;
            _accountManager = accountManager;
            _organizationManager = organizationManager;
            _vehicleManager = vehicleManager;
            _auditTrail = auditTrail;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _configuration = configuration;
        }

        [HttpGet]
        [Route("kpiinfo")]
        public async Task<IActionResult> GetKPIInfo([FromQuery] int? minDistance, [FromBody] EcoScoreRequest request)
        {
            try
            {
                minDistance = minDistance ?? 0;
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Eco-Score Data Service", nameof(GetKPIInfo), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get KPI info method Eco-Score data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request, minDistance);
                if (result is NoContentResult)
                {
                    var response = await _reportManager.GetKPIInfo(MapRequest(request, minDistance));

                    return Ok(response);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing KPI Info data.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Eco-Score Data Service", nameof(GetKPIInfo), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get KPI info method Eco-Score data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpGet]
        [Route("chartinfo")]
        public async Task<IActionResult> GetChartInfo([FromQuery] int? minDistance, [FromBody] EcoScoreRequest request)
        {
            try
            {
                minDistance = minDistance ?? 0;
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Eco-Score Data Service", nameof(GetChartInfo), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get Chart info method Eco-Score data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request, minDistance);
                if (result is NoContentResult)
                {
                    var response = await _reportManager.GetChartInfo(MapRequest(request, minDistance));

                    return Ok(response);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Chart Info data.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Eco-Score Data Service", nameof(GetChartInfo), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get Chart info method Eco-Score data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        private async Task<IActionResult> ValidateParameters(EcoScoreRequest request, int? minDistance)
        {
            if (minDistance < 0)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "INVALID_PARAMETER", parameter: nameof(minDistance));

            var account = await _accountManager.GetAccountByEmailId(request.Account);
            if (account == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.Account));

            if (string.IsNullOrEmpty(account.DriverId) || (!string.IsNullOrEmpty(account.DriverId) && !account.DriverId.Equals(request.DriverId)))
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INCORRECT_DRIVERID", parameter: nameof(request.DriverId));

            var org = await _organizationManager.GetOrganizationByOrgCode(request.OrganizationId);
            if (org == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ORGANIZATION_NOT_FOUND", parameter: nameof(request.OrganizationId));

            var orgs = await _accountManager.GetAccountOrg(account.Id);
            if (!orgs.Select(x => x.Id).ToArray().Contains(org.Id))
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.Account));

            var vehicle = await _vehicleManager.Get(new VehicleFilter() { VIN = request.VIN });
            if (vehicle.FirstOrDefault() == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "VIN_NOT_FOUND", parameter: nameof(request.VIN));

            var visibleVehicles = await _vehicleManager.GetVisibilityVehicles(account.Id, org.Id);
            if (!visibleVehicles.Any(x => x.VIN == request.VIN))
            {
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "VIN_NOT_FOUND", parameter: nameof(request.VIN));
            }

            return NoContent();
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string errorCode, string parameter)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = errorCode,
                Value = parameter
            });
        }

        private EcoScoreDataServiceRequest MapRequest(EcoScoreRequest request, int? minDistance)
        {
            return new EcoScoreDataServiceRequest
            {
                AccountEmail = request.Account,
                DriverId = request.DriverId,
                OrganizationId = request.OrganizationId,
                VIN = request.VIN,
                AggregationType = Enum.Parse<AggregateType>(request.AggregationType, true),
                StartTimestamp = request.StartTimestamp.Value,
                EndTimestamp = request.EndTimestamp.Value,
                MinDistance = minDistance ?? 0,
                EcoScoreRecordsLimit = Convert.ToInt32(_configuration["EcoScoreRecordsLimit"])
            };
        }
    }
}
