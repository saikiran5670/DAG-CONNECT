using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.package;
using net.atos.daf.ct2.package.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.packageservice
{
    public class PackageManagementService : PackageService.PackageServiceBase
    {
        private readonly ILogger<PackageManagementService> _logger;
        private readonly IAuditTraillib _AuditTrail;
        private readonly IPackageManager _packageManager;
        public PackageManagementService(ILogger<PackageManagementService> logger,
                                        IAuditTraillib AuditTrail,
                                        IPackageManager packageManager)
        {
            _logger = logger;
            _AuditTrail = AuditTrail;
            _packageManager = packageManager;
        }


        public  override Task<PackageResponse> Create(PackageCreateRequest request, ServerCallContext context)
        {
            try
            {
                var package = new Package();
                package.Code = request.Code;
                package.FeatureSetID = request.FeatureSetID;
                package.Status = (package.ENUM.PackageStatus)(int)request.Status;
                package.Name = request.Name;
                package.Type = (package.ENUM.PackageType)(char)request.Type;
                package.Description = request.Description;
                package = _packageManager.Create(package).Result;
                return Task.FromResult(new PackageResponse
                {
                    Message = "Package Created " + package.Id
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new PackageResponse
                {
                    Message = "Exception " + ex.Message,
                    Code=Responsecode.Failed
                });
            }
        }


        //Update
        public override Task<PackageResponse> Update(PackageCreateRequest request, ServerCallContext context)
        {
            try
            {
                var package = new Package();
                package.Id = request.Id;
                package.Code = request.Code;
                package.FeatureSetID = request.FeatureSetID;
                package.Status = (package.ENUM.PackageStatus)request.Status;
                package.Name = request.Name;
                package.Type = (package.ENUM.PackageType)request.Type;
                package.Description = request.Description;
                package = _packageManager.Update(package).Result;
                return Task.FromResult(new PackageResponse
                {
                    Message = "Package Updated " + package.Id
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new PackageResponse
                {
                    Message = "Exception " + ex.Message
                });
            }
        }

        //Delete
        public override Task<PackageResponse> Delete(PackageDeleteRequest request, ServerCallContext context)
        {
            try
            {
                var result = _packageManager.Delete(request.Id).Result;
                return Task.FromResult(new PackageResponse
                {
                    Message = "Package deleted " + request.Id,
                    Code = Responsecode.Success
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new PackageResponse
                {
                    Message = "Exception " + ex.Message
                });
            }
        }

        //Get

        public async override Task<GetPackageResponce> Get(GetPackageRequest request, ServerCallContext context)
        {
            try
            {
                var response = new GetPackageResponce();
                var packageFilter = new PackageFilter();
                packageFilter.Id = request.Id;
                packageFilter.Code = request.Code;
                packageFilter.FeatureSetId = request.FeatureSetID;
                packageFilter.Status = (package.ENUM.PackageStatus)(int)request.Status;
                packageFilter.Name = request.Name;
                packageFilter.Type = (package.ENUM.PackageType)(int)request.Type;
                var packages = _packageManager.Get(packageFilter).Result;
                response.PacakageList.AddRange(packages
                                     .Select(x => new GetPackageRequest()
                                     { Id = x.Id,
                                         Code = x.Code,
                                         Description = x.Description,
                                         Name = x.Name,
                                         FeatureSetID=x.FeatureSetID,                                          
                                         Status = (PackageStatus)x.Status,
                                         Type = (PackageType)x.Type }).ToList()); 
                _logger.LogInformation("Get package details.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in package service:get package  details with exception - " + ex.Message + ex.StackTrace);
                return await Task.FromResult(new GetPackageResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responsecode.Failed
                });
            }
        }

        //Import
        public async override Task<ImportPackageResponce> Import(ImportPackageRequest request, ServerCallContext context)
        {
            try
            {
                var response = new ImportPackageResponce();
                var packages = new List<Package>();


                packages.AddRange(request.Packages.Select(x => new Package()
                {                    
                    Code = x.Code,
                    Description = x.Description,
                    FeatureSetID=x.FeatureSetID,
                    Name = x.Name,
                    Status = (package.ENUM.PackageStatus)x.Status,
                    Type = (package.ENUM.PackageType)x.Type
                }).ToList());
                
                var packageImported = _packageManager.Import(packages).Result;
                response.PackageList.AddRange(packageImported
                                     .Select(x=>new GetPackageRequest() {
                                         Id = x.Id,
                                         Code = x.Code,
                                         Description = x.Description,
                                         Name = x.Name,
                                         Status = (PackageStatus)x.Status,
                                         Type = (PackageType)x.Type
                                     }).ToList());


                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in package service:Import package with exception - " + ex.Message + ex.StackTrace);
                return await Task.FromResult(new ImportPackageResponce
                {
                    Message = "Exception " + ex.Message, 
                    Code = Responsecode.Failed
                });
            }
        }


    }
}
