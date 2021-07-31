using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.rfms;
using net.atos.daf.ct2.rfmsdataservice.CustomAttributes;
using net.atos.daf.ct2.rfmsdataservice.Entity;
using Microsoft.Extensions.Primitives;
using net.atos.daf.ct2.vehicle;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using net.atos.daf.ct2.rfms.entity;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.rfmsdataservice.Controllers
{
    [ApiController]
    [Route("rfms")]
    public class RFMSVehiclePositionsController : ControllerBase
    {
        private readonly ILogger<RfmsDataServiceController> _logger;
        private readonly IRfmsManager _rfmsManager;
        private readonly IAuditTraillib _auditTrail;
        private readonly IAccountManager _accountManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public RFMSVehiclePositionsController(IAccountManager accountManager,
                                         ILogger<RfmsDataServiceController> logger,
                                         IAuditTraillib auditTrail,
                                         IRfmsManager rfmsManager,
                                         IVehicleManager vehicleManager,
                                         IConfiguration configuration,
                                         IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _auditTrail = auditTrail;
            this._rfmsManager = rfmsManager;
            this._accountManager = accountManager;
            this._vehicleManager = vehicleManager;
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
        }

        

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string value, string message)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = message,
                Value = value
            });
        }
    }
}
