using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.packageservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.portalservice.Entity.Package;

namespace net.atos.daf.ct2.portalservice.Controllers
{

    [ApiController]
    [Route("package")]
    public class PackageController : ControllerBase
    {
        private readonly ILogger<PackageController> _logger;
        private readonly PackageService.PackageServiceClient _packageClient;
        private readonly FeatureService.FeatureServiceClient _featureclient;
        private readonly PackageMapper _packageMapper;
        private readonly FeatureSetMapper _featureSetMapper;

        public PackageController(PackageService.PackageServiceClient packageClient,
            FeatureService.FeatureServiceClient featureclient,
            ILogger<PackageController> logger)
        {
            _packageClient = packageClient;
            _featureclient = featureclient;
            _logger = logger;
            _featureSetMapper = new FeatureSetMapper(featureclient);
            _packageMapper = new PackageMapper(_featureclient);

        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(PackagePortalRequest request)
        {
            try
            {

                if (request.FeatureIds.Count >= 1)
                {
                    var featureSetId = await _featureSetMapper.RetrieveFeatureSetIdById(request.FeatureIds);
                    request.FeatureSetID = featureSetId;
                }
                else
                {
                    return StatusCode(400, "Please provide package featureIds");
                }
                if (request.FeatureSetID > 0)
                {
                    var createPackageRequest = _packageMapper.ToCreatePackage(request);

                    var packageResponse = await _packageClient.CreateAsync(createPackageRequest);

                    if (packageResponse.PackageId == -1 && packageResponse.Code == Responsecode.Conflict)
                    {
                        return StatusCode(409, packageResponse.Message);
                    }


                    if (packageResponse != null
                       && packageResponse.Message == PortalConstants.PackageValidation.ErrorMessage)
                    {
                        return StatusCode(500, PortalConstants.PackageValidation.ErrorMessage);
                    }
                    // The package type should be single character
                    if (request.Type.Length > 1)
                    {
                        return StatusCode(400, PortalConstants.PackageValidation.InvalidPackageType);
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
                else
                {

                    return StatusCode(500, "Featureset id not created");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Package Service:Create : " + ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, PortalConstants.ExceptionKeyWord.FK_Constraint);
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }



        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(PackagePortalRequest request)
        {
            try
            {
                _logger.LogInformation("Update method in package API called.");

                // Validation 
                if (request.Id <= 0 || (string.IsNullOrEmpty(request.Code)) || request.FeatureSetID <= 0)
                {
                    return StatusCode(400, PortalConstants.PackageValidation.CreateRequired);
                }
                // The package type should be single character
                if (request.Type.Length > 1)
                {
                    return StatusCode(400, PortalConstants.PackageValidation.InvalidPackageType);
                }
                if (request.FeatureSetID > 0)
                {

                    if (request.FeatureIds.Count >= 1)
                    {
                        var featureSetId = await _featureSetMapper.UpdateFeatureSetIdById(request.FeatureIds, request.FeatureSetID);
                        request.FeatureSetID = featureSetId;
                    }
                    else
                    {
                        return StatusCode(400, "Please provide package featureIds");
                    }
                    var createPackageRequest = _packageMapper.ToCreatePackage(request);

                    var packageResponse = await _packageClient.UpdateAsync(createPackageRequest);

                    if (packageResponse.PackageId == -1 && packageResponse.Code == Responsecode.Conflict)
                    {
                        return StatusCode(409, packageResponse.Message);
                    }

                    if (packageResponse != null && packageResponse.Code == Responsecode.Failed
                         && packageResponse.Message == "There is an error updating package.")
                    {
                        return StatusCode(500, "There is an error updating package.");
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
                else
                {

                    return StatusCode(500, "Featureset id not created");

                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Package Service:Update : " + ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }


        //Get/Export Packages
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] PackageFilter filterRequest)
        {
            try
            {

                // The package type should be single character
                if (!string.IsNullOrEmpty(filterRequest.Type) && filterRequest.Type.Length > 1)
                {
                    return StatusCode(400, "The pakage type is not valid. It should be of single character");
                }
                var request = new GetPackageRequest()
                {
                    Id = filterRequest.Id,
                    Status = filterRequest.Status == null ? string.Empty : filterRequest.Status,
                    Code = filterRequest.Code == null ? string.Empty : filterRequest.Code,                   
                    Type = filterRequest.Type == null ? string.Empty : filterRequest.Type,
                    FeatureSetID = filterRequest.FeatureSetId
                };
                var response = await _packageClient.GetAsync(request);
                response.PacakageList.Where(S => S.FeatureSetID > 0)
                                                .Select(S => { S.FeatureIds.AddRange(_featureSetMapper.GetFeatureIds(S.FeatureSetID).Result); return S; }).ToList();



                if (response != null && response.Code == Responsecode.Success)
                {
                    if (response.PacakageList != null && response.PacakageList.Count > 0)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(404, "Package details are not found.");
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
                response.PackageDeleteRequest = packageRequest;
                if (response != null && response.Code == Responsecode.Success)
                    return Ok(response);
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
        public async Task<IActionResult> Import(PackageImportRequest request)
        {
            try
            {
                //Validation
                if (request.packagesToImport.Count <= 0)
                {
                    return StatusCode(400, "Package data is required.");
                }
                bool hasFeature = request.packagesToImport.Any(x => x.Features.Count >= 1);
                if (hasFeature)
                {
                    var packageRequest = _packageMapper.ToImportPackage(request);
                    var packageResponse = await _packageClient.ImportAsync(packageRequest);

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
                        if (packageResponse.PackageList.Count == 0)
                            return StatusCode(500, "package code already exists");
                        else
                        {
                            return StatusCode(500, "Package response is null");
                        }
                    }
                }
                else
                {

                    return StatusCode(400, "Please provide package features");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Package Service:Import : " + ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("updatestatus")]
        public async Task<IActionResult> UpdatePackageStatus(UpdatePackageStatusRequest request)
        {
            try
            {
                _logger.LogInformation("Update package status method in package API called.");

                // Validation 
                if (request.PackageId <= 0 || (string.IsNullOrEmpty(request.Status)))
                {
                    return StatusCode(400, PortalConstants.PackageValidation.PackageStatusRequired);
                }
                // The package status should be single character
                if (request.Status.Length > 1)
                {
                    return StatusCode(400, PortalConstants.PackageValidation.InvalidPackageStatus);
                }


                var packageResponse = await _packageClient.UpdatePackageStatusAsync(request);


                if (packageResponse != null && packageResponse.Code == Responsecode.Failed
                     && packageResponse.Message == "There is an error in updating package status.")
                {
                    return StatusCode(500, "There is an error  in updating package status.");
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
                _logger.LogError("Package Service:Update : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

    }
}
