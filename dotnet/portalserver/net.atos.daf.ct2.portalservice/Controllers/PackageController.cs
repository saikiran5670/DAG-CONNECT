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
                    return StatusCode(500, "There is an error creating package.");
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



        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(PackageUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("Update method in package API called.");

                // Validation 
                if (request.Id <= 0 || (string.IsNullOrEmpty(request.Code)))
                {
                    return StatusCode(400, "The packageId and package code are required.");
                }

                var packageResponse = await _packageClient.UpdateAsync(request);


                if (packageResponse != null && packageResponse.Code == Responsecode.Failed
                     && packageResponse.Message == "There is an error updating package.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else if (packageResponse != null && packageResponse.Code == Responsecode.Success)
                {
                    return Ok(packageResponse);
                }
                else
                {
                    return StatusCode(500, "accountResponse is null");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Package Service:Create : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }


        //Get/Export Packages
        [HttpPost]
        [Route("getpackages")]
        public async Task<IActionResult> Get(GetPackageRequest request)
        {
            try
            {

                var response = await _packageClient.GetAsync(request);
                if (response != null && response.Code == Responsecode.Success)
                {
                    if (response.PacakageList != null && response.PacakageList.Count > 0)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(404, "Accounts details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in package service:get package with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        //Delete package
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(int packageId)
        {
            try
            {
                // Validation                 
                if (packageId <= 0)
                {
                    return StatusCode(400, "Package id is required.");
                }
                var packageRequest = new PackageDeleteRequest();
                packageRequest.Id = packageId;
                var response = await _packageClient.DeleteAsync(packageRequest);
                if (response != null && response.Code == Responsecode.Success)
                    return Ok(packageRequest);
                else
                    return StatusCode(404, "Package not configured.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Package service:delete Package with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }



        //Delete package
        [HttpPost]
        [Route("Import")]
        public async Task<IActionResult> Import(ImportPackageRequest request)
        {
            try
            {
                // Validation                 
                if (request.Packages.Count <= 0)
                {
                    return StatusCode(400, "Package data is required.");
                }
                var packageRequest = new ImportPackageRequest();
                var packageResponse = await _packageClient.ImportAsync(request);

                if (packageResponse != null
                   && packageResponse.Message == "There is an error importing package.")
                {
                    return StatusCode(500, "There is an error importing package.");
                }
                else if (packageResponse != null && packageResponse.Code == Responsecode.Success && 
                         packageResponse.PackageList != null && packageResponse.PackageList.Count > 0)
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
                _logger.LogError("Package Service:Import : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }



    }
}
