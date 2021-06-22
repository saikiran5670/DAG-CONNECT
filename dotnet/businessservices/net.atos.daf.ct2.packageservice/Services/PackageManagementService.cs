using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.package;
using net.atos.daf.ct2.package.entity;

namespace net.atos.daf.ct2.packageservice
{
    public class PackageManagementService : PackageService.PackageServiceBase
    {
        private readonly ILog _logger;
        private readonly IAuditTraillib _auditTraillib;
        private readonly IPackageManager _packageManager;
        public PackageManagementService(
                                        IAuditTraillib AuditTrail,
                                        IPackageManager packageManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _auditTraillib = AuditTrail;
            _packageManager = packageManager;
        }


        public override Task<PackageResponse> Create(PackageCreateRequest request, ServerCallContext context)
        {
            try
            {
                var package = new Package();
                package.Code = request.Code;
                package.FeatureSetID = request.FeatureSetID;
                //   package.Status = request.Status;
                package.Name = request.Name;
                package.Type = request.Type;
                package.Description = request.Description;
                package.State = request.State;
                package = _packageManager.Create(package).Result;
                if (package.Id == -1)
                {
                    return Task.FromResult(new PackageResponse
                    {
                        Message = "Package Code is " + package.Code + " already exists ",
                        PackageId = package.Id,
                        Code = Responsecode.Conflict
                    });
                }
                else
                {
                    return Task.FromResult(new PackageResponse
                    {
                        Message = "Package Created " + package.Id,
                        PackageId = package.Id
                    });
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(new PackageResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responsecode.Failed
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
                //  package.Status = request.Status;
                package.Name = request.Name;
                package.Type = request.Type;
                package.Description = request.Description;
                package.State = request.State;

                package = _packageManager.Update(package).Result;

                if (package.Id == -1)
                {
                    return Task.FromResult(new PackageResponse
                    {
                        Message = "Package Code is " + package.Code + " already exists ",
                        PackageId = package.Id,
                        Code = Responsecode.Conflict
                    });
                }
                else
                {
                    return Task.FromResult(new PackageResponse
                    {
                        Message = "Package Updated ",
                        PackageId = package.Id
                    });
                }
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
        public async override Task<PackageResponse> Delete(PackageDeleteRequest request, ServerCallContext context)
        {
            try
            {
                var result = _packageManager.Delete(request.Id).Result;
                var response = new PackageResponse();
                if (result)
                {
                    response.Code = Responsecode.Success;
                    response.Message = "Package Deleted.";
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = "Package Not Found.";
                }

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new PackageResponse
                {
                    Code = Responsecode.Failed,
                    Message = "Package Deletion Faile due to - " + ex.Message,

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
                packageFilter.State = request.State;
                packageFilter.Type = request.Type;
                var packages = _packageManager.Get(packageFilter).Result;
                response.PacakageList.AddRange(packages
                                     .Select(x => new GetPackageRequest()
                                     {
                                         Id = x.Id,
                                         Code = x.Code,
                                         Description = x.Description,
                                         Name = x.Name,
                                         FeatureSetID = x.FeatureSetID,
                                         //   Status = x.Status,
                                         State = x.State,
                                         CreatedAt = x.CreatedAt,
                                         Type = x.Type
                                     }).ToList());
                _logger.Info("Get package details.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
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
                    FeatureSetID = x.FeatureSetID,
                    Name = x.Name,
                    //  Status = x.Status,
                    Type = x.Type,
                    State = x.State
                }).ToList());

                var packageImported = await _packageManager.Import(packages);
                response.PackageList.AddRange(packageImported
                                     .Select(x => new GetPackageRequest()
                                     {
                                         Id = x.Id,
                                         Code = x.Code,
                                         Description = x.Description,
                                         Name = x.Name,
                                         FeatureSetID = x.FeatureSetID,
                                         // Status = x.Status,
                                         Type = x.Type,
                                         State = x.State,
                                         CreatedAt = x.CreatedAt
                                     }).ToList());

                response.Code = Responsecode.Success;
                response.Message = "Package imported successfully.";
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ImportPackageResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responsecode.Failed
                });
            }
        }



        //Update Package Status
        public override Task<UpdatePackageStateResponse> UpdatePackageState(UpdatePackageStateRequest request, ServerCallContext context)
        {
            try
            {
                var package = new Package();
                package.Id = request.PackageId;
                package.State = request.State;
                package = _packageManager.UpdatePackageState(package).Result;
                return Task.FromResult(new UpdatePackageStateResponse
                {
                    Message = "Package state Updated ",
                    PackageStateResponse = request
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new UpdatePackageStateResponse
                {
                    Message = "Exception " + ex.Message
                });
            }
        }
    }
}
