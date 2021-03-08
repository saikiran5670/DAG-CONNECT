using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.packageservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.featureservice;
using Google.Protobuf.Collections;
using net.atos.daf.ct2.portalservice.Entity.Package;
using net.atos.daf.ct2.portalservice.Entity.Feature;

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
        private string FK_Constraint = "violates foreign key constraint";
        public PackageController(PackageService.PackageServiceClient packageClient,
            FeatureService.FeatureServiceClient featureclient,
            ILogger<PackageController> logger)
        {
            _packageClient = packageClient;
            _featureclient = featureclient;
            _logger = logger;
            _packageMapper = new PackageMapper();

        }



        private async Task<int> RetrieveFeatureSetId(List<string> features)
        {
            var featureSetId = new List<int>();
            var featureSetRequest = new FetureSetRequest();
            var featureFilterRequest = new FeaturesFilterRequest();
            var featureList = await _featureclient.GetFeaturesAsync(featureFilterRequest);
            foreach (var item in features)
            {
                bool hasFeature = featureList.Features.Any(feature => feature.Name == item);
                if (hasFeature)
                {
                    foreach (var feature in featureList.Features)
                    {
                        if (feature.Name == item)
                        {
                            featureSetId.Add(feature.Id);
                        }
                    }
                }
                else
                {
                    return 0;

                }
            }

            featureSetRequest.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            featureSetId = featureSetId.Select(x => x).Distinct().ToList();
            featureSetRequest.Features.AddRange(featureSetId);

            var ObjResponse = await _featureclient.CreateFeatureSetAsync(featureSetRequest);
            return Convert.ToInt32(ObjResponse.Message);

        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(PackagePortalRequest request)
        {
            try
            {
                var featureSetId = await RetrieveFeatureSetId(request.Features);
                if (featureSetId > 0)
                {
                    var createPackageRequest = _packageMapper.ToCreatePackage(request);
                    createPackageRequest.FeatureSetID = featureSetId;
                    var packageResponse = await _packageClient.CreateAsync(createPackageRequest);
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
                else
                {

                    return StatusCode(500, "Please provide valid feature"); //need to confirm
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Package Service:Create : " + ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
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
                if (request.Id <= 0 || (string.IsNullOrEmpty(request.Code)))
                {
                    return StatusCode(400, "The packageId and package code are required.");
                }

                var featureSetId = await RetrieveFeatureSetId(request.Features);
                if (featureSetId > 0)
                {
                    var createPackageRequest = _packageMapper.ToCreatePackage(request);
                    createPackageRequest.FeatureSetID = featureSetId;
                    var packageResponse = await _packageClient.UpdateAsync(createPackageRequest);


                    if (packageResponse != null && packageResponse.Code == Responsecode.Failed
                         && packageResponse.Message == "There is an error updating package.")
                    {
                        return StatusCode(500, "There is an error updating account.");
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
                else
                {

                    return StatusCode(500, "Please provide valid feature"); //need to confirm

                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Package Service:Update : " + ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }


        //Get/Export Packages
        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Get(GetPackageRequest request)
        {
            try
            {

                var response = await _packageClient.GetAsync(request);
                response.PacakageList.Where(S => S.FeatureSetID > 0)
                                                .Select(S => { S.Features.AddRange(GetFeatures(S.FeatureSetID).Result); return S; }).ToList();
                if (response != null && response.Code == Responsecode.Success)
                {
                    if (response.PacakageList != null && response.PacakageList.Count > 0)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(404, "Package details are found.");
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

        private async Task<IEnumerable<string>> GetFeatures(int featureSSetId)
        {
            var features = new List<string>();
            var featureFilterRequest = new FeaturesFilterRequest();
            featureFilterRequest.FeatureSetID = featureSSetId;
            var featureList = await _featureclient.GetFeaturesAsync(featureFilterRequest);
            features.AddRange(featureList.Features.Select(x => x.Name).ToList());
            return features;
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
        public async Task<IActionResult> Import(PackageImportRequest request)
        {
            try
            {
                // Validation                 
                //if (request.Packages.Count <= 0)
                //{
                //    return StatusCode(400, "Package data is required.");
                //}
                var packageRequest = new ImportPackageRequest();


                packageRequest.Packages.AddRange(request.packages
                                    .Select(x => new PackageCreateRequest()
                                    {
                                        Code = x.Code,
                                        Description = x.Description,
                                        Name = x.Name,
                                        FeatureSetID = x.FeatureSetID,
                                        Status = x.Status == "A" ? packageservice.PackageStatus.Active : packageservice.PackageStatus.Inactive,
                                        //Type = (PackageType)x.Type
                                    }).ToList());



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
                    return StatusCode(500, "packageResponse is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Package Service:Import : " + ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }



    }
}
