using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.package;
using net.atos.daf.ct2.package.entity;
using net.atos.daf.ct2.package.ENUM;
using net.atos.daf.ct2.packageservicerest.entity;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.packageservicerest.Controllers
{

    [Route("package")]
    [ApiController]
    public class PackageController : ControllerBase
    {

        private readonly ILogger<PackageController> logger;

        private readonly IPackageManager _packageManager;
        IAuditTraillib _auditlog;

        private IHttpContextAccessor _httpContextAccessor;
        //private string FK_Constraint = "violates foreign key constraint";
        public PackageController(ILogger<PackageController> _logger,
                                 IPackageManager packageManager,
                                 IHttpContextAccessor httpContextAccessor,
                                 IAuditTraillib auditTraillib)
        {
            logger = _logger;
            _packageManager = packageManager;
            _httpContextAccessor = httpContextAccessor;
            _auditlog = auditTraillib;

        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> create(PackageRequest request)
        {
            try
            {

                logger.LogInformation("Package create function called ");           

                var ObjPackage = new Package()
                {
                    Code = request.code,
                    //Default = request.is_default,
                    FeatureSetID = request.feature_set.FeatureSetID,
                    Status =request.is_active,
                    Name = request.name,
                    Type = request.type,
                    Description = request.description,
                   // StartDate = request.start_date,
                   // EndDate = request.end_date

                };

                var pkgId = await _packageManager.Create(ObjPackage);
                _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Package Component", "Package Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create method in pacakge manager", 0, pkgId.Id, JsonConvert.SerializeObject(request));
                if (pkgId.Id < 1)
                {
                    return StatusCode(400, "This package is already exist :" + request.code);
                }
                else
                {
                    return Ok("Pacakge Created :" + pkgId.Code); //need to clarify
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Pacakge Component", "Pacakge Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create method in package manager", 0, 0, JsonConvert.SerializeObject(request));

                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

    }
}
