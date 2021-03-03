using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.packageservice;
using net.atos.daf.ct2.portalservice.Common;

namespace net.atos.daf.ct2.portalservice.Controllers
{
   
    [ApiController]
    [Route("package")]
    public class PackageController : ControllerBase
    {
        private readonly ILogger<PackageController> _logger;
        private readonly PackageService.PackageServiceClient _packageClient;       
      
        public PackageController(PackageService.PackageServiceClient packageClient, ILogger<PackageController> logger)
        {
            _packageClient = packageClient;
            _logger = logger;
           
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(PackageCreateRequest request)
        {
            try
            {
                // Validation 
                if ((string.IsNullOrEmpty(request.Code)) || (string.IsNullOrEmpty(request.Name))
                || (request.Features.Count == 0) || !EnumValidator.ValidateAccountType((char)request.Type))
                {
                    return StatusCode(400, "The Package code,name,type and features are required.");
                }
                var packageResponse = await _packageClient.CreateAsync(request);            
                 if (packageResponse != null  
                    && packageResponse.Message == "There is an error creating package.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
               else if (packageResponse != null && packageResponse.Code == Responsecode.Success)
                {
                    return Ok(packageResponse);
                }
                else
                {
                    return StatusCode(500, "packageResponse is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Package Service:Create : " + ex.Message + " " + ex.StackTrace);             
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
