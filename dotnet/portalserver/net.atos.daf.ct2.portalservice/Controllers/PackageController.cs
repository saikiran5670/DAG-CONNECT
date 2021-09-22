﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.packageservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Package;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.Controllers
{

    [ApiController]
    [Route("package")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class PackageController : BaseController
    {
        private readonly AuditHelper _auditHelper;
        private readonly PackageService.PackageServiceClient _packageClient;
        private readonly FeatureService.FeatureServiceClient _featureclient;
        private readonly PackageMapper _packageMapper;
        private readonly ILog _logger;
        private readonly FeatureSetMapper _featureSetMapper;

        public PackageController(PackageService.PackageServiceClient packageClient,
            FeatureService.FeatureServiceClient featureclient
            , AuditHelper auditHelper, SessionHelper sessionHelper, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor, sessionHelper)
        {
            _packageClient = packageClient;
            _featureclient = featureclient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _featureSetMapper = new FeatureSetMapper(featureclient);
            _packageMapper = new PackageMapper(_featureclient);
            _auditHelper = auditHelper;

        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(PackagePortalRequest request)
        {
            var packageResponse = new PackageResponse();
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

                    packageResponse = await _packageClient.CreateAsync(createPackageRequest);

                    if (packageResponse.PackageId == -1 && packageResponse.Code == Responsecode.Conflict)
                    {
                        return StatusCode(409, packageResponse.Message);
                    }

                    if (packageResponse != null
                       && packageResponse.Message == PortalConstants.PackageValidation.ERROR_MESSAGE)
                    {
                        return StatusCode(500, PortalConstants.PackageValidation.ERROR_MESSAGE);
                    }
                    // The package type should be single character
                    if (request.Type.Length > 1)
                    {
                        return StatusCode(400, PortalConstants.PackageValidation.INVALID_PACKAGE_TYPE);
                    }

                    else if (packageResponse != null && packageResponse.Code == Responsecode.Success)
                    {
                        await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                          "Package service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                          "Create method in Package controller", 0, packageResponse.PackageId, JsonConvert.SerializeObject(request),
                                           _userDetails);

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

                await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                            "Package service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                            "Create method in Package controller", 0, packageResponse.PackageId, JsonConvert.SerializeObject(request),
                                             _userDetails);

                _logger.Error(null, ex);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_CONSTRAINT))
                {
                    return StatusCode(400, PortalConstants.ExceptionKeyWord.FK_CONSTRAINT);
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
                _logger.Info("Update method in package API called.");

                // Validation 
                if (request.Id <= 0 || string.IsNullOrEmpty(request.Code) || request.FeatureSetID <= 0)
                {
                    return StatusCode(400, PortalConstants.PackageValidation.CREATE_REQUIRED);
                }
                // The package type should be single character
                if (request.Type.Length > 1)
                {
                    return StatusCode(400, PortalConstants.PackageValidation.INVALID_PACKAGE_TYPE);
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
                    var packageResponse = new PackageResponse();
                    packageResponse = await _packageClient.UpdateAsync(createPackageRequest);

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
                        await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                           "Package service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "Update method in Package controller", request.Id, packageResponse.PackageId, JsonConvert.SerializeObject(request),
                                            _userDetails);

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


                await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                             "Package service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                             "Update method in Package controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                                              _userDetails);

                _logger.Error(null, ex);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_CONSTRAINT))
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
                    State = filterRequest.State ?? string.Empty,
                    Code = filterRequest.Code ?? string.Empty,
                    Type = filterRequest.Type ?? string.Empty,
                    FeatureSetID = filterRequest.FeatureSetId
                };
                var response = await _packageClient.GetAsync(request);
                int level = _userDetails.RoleLevel;

                response.PacakageList.Where(S => S.FeatureSetID > 0)
                                                .Select(S => { S.FeatureIds.AddRange(_featureSetMapper.GetFeatureIds(S.FeatureSetID).Result); return S; }).ToList();



                if (response != null && response.Code == Responsecode.Success)
                {
                    if (response.PacakageList != null && response.PacakageList.Count > 0)
                    {
                        if (level != 10)
                        {
                            //platform package should be visible to platform admin only.
                            //due to readonly proto properties clreared list and added new
                            var packagelist = response.PacakageList.Where(s => s.Type != "Platform");
                            response.PacakageList.Clear();
                            response.PacakageList.AddRange(packagelist);
                        }
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(404, "Package details are not found.");
                    }
                }
                else if (response.Code == Responsecode.Failed)
                {
                    return StatusCode(404, "Package details are not found.");
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }



        //Delete package
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(int packageId)
        {
            var packageRequest = new PackageDeleteRequest();
            try
            {
                // Validation                 
                if (packageId <= 0)
                {
                    return StatusCode(400, "Package id is required.");
                }
                packageRequest = new PackageDeleteRequest();
                packageRequest.Id = packageId;
                var response = await _packageClient.DeleteAsync(packageRequest);
                response.PackageDeleteRequest = packageRequest;
                if (response != null && response.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                           "Package service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "Delete method in Package controller", packageRequest.Id, packageRequest.Id, JsonConvert.SerializeObject(packageId),
                                            _userDetails);

                    return Ok(response);
                }

                else
                    return StatusCode(404, "Package not configured.");
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                            "Package service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                            "Delete method in Package controller", packageRequest.Id, packageRequest.Id, JsonConvert.SerializeObject(packageId),
                                             _userDetails);

                _logger.Error(null, ex);
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
                if (request.PackagesToImport.Count <= 0)
                {
                    return StatusCode(400, "Package data is required.");
                }
                bool hasFeature = request.PackagesToImport.Any(x => x.Features.Count == 0);
                if (!hasFeature)
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
                        await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                             "Package service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "Import method in Package controller", 0, 0, JsonConvert.SerializeObject(request),
                                              _userDetails);

                        return Ok(packageResponse);
                    }
                    else
                    {
                        if (packageResponse.PackageList.Count == 0)
                            return StatusCode(409, "package code already exists");
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
                await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                             "Package service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                             "Import method in Package controller", 0, 0, JsonConvert.SerializeObject(request),
                                              _userDetails);

                _logger.Error(null, ex);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_CONSTRAINT))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("updatestatus")]
        public async Task<IActionResult> UpdatePackageStatus(UpdatePackageStateRequest request)
        {
            try
            {
                _logger.Info("Update package status method in package API called.");

                // Validation 
                if (request.PackageId <= 0 || string.IsNullOrEmpty(request.State))
                {
                    return StatusCode(400, PortalConstants.PackageValidation.PACKAGE_STATUS_REQUIRED);
                }
                // The package status should be single character
                if (request.State.Length > 1)
                {
                    return StatusCode(400, PortalConstants.PackageValidation.INVALID_PACKAGE_STATUS);
                }


                var packageResponse = await _packageClient.UpdatePackageStateAsync(request);


                if (packageResponse != null && packageResponse.Code == Responsecode.Failed
                     && packageResponse.Message == "There is an error in updating package state.")
                {
                    return StatusCode(500, "There is an error  in updating package state.");
                }
                else if (packageResponse != null && packageResponse.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                             "Package service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "UpdatePackageStatus method in Package controller", 0, 0, JsonConvert.SerializeObject(request),
                                              _userDetails);

                    return Ok(packageResponse);
                }
                else
                {
                    return StatusCode(500, "packageResponse is null");
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Package Component",
                                            "Package service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                            "UpdatePackageStatus method in Package controller", 0, 0, JsonConvert.SerializeObject(request),
                                             _userDetails);

                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

    }
}
