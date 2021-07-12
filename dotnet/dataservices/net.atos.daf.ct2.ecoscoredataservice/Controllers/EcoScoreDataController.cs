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

namespace net.atos.daf.ct2.ecoscoredataservice.Controllers
{
    [ApiController]
    [Route("ecoscore")]
    //[Authorize(Policy = AccessPolicies.MAIN_ACCESS_POLICY)]
    public class EcoScoreDataController : ControllerBase
    {
        private readonly IAuditTraillib _auditTrail;
        private readonly ILog _logger;
        private readonly IReportManager _reportManager;
        public EcoScoreDataController(IAuditTraillib auditTrail, IReportManager reportManager)
        {
            _reportManager = reportManager;
            _auditTrail = auditTrail;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [HttpGet]
        [Route("kpiinfo")]
        public async Task<IActionResult> GetKPIInfo([FromBody] EcoScoreRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Eco-Score Data Service", nameof(GetKPIInfo), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get KPI info method Eco-Score data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var response = _reportManager.GetKPIInfo(MapRequest(request));

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing KPI Info data.", ex);
                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Eco-Score Data Service", nameof(GetKPIInfo), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get KPI info method Eco-Score data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpGet]
        [Route("chartinfo")]
        public async Task<IActionResult> GetChartInfo(EcoScoreRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Eco-Score Data Service", nameof(GetChartInfo), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get Chart info method Eco-Score data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var response = _reportManager.GetChartInfo(MapRequest(request));

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Chart Info data.", ex);
                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Eco-Score Data Service", nameof(GetChartInfo), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get Chart info method Eco-Score data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
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

        private EcoScoreDataServiceRequest MapRequest(EcoScoreRequest request)
        {
            return new EcoScoreDataServiceRequest
            {
                AccountId = request.AccountId,
                DriverId = request.DriverId,
                OrganizationId = request.OrganizationId,
                VIN = request.VIN,
                AggregationType = Enum.Parse<AggregateType>(request.AggregationType, true),
                StartTimestamp = request.StartTimestamp.Value,
                EndTimestamp = request.EndTimestamp.Value
            };
        }
    }
}
