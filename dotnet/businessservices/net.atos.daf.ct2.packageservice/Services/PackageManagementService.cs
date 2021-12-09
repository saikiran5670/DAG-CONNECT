﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.kafkacdc;
using net.atos.daf.ct2.package;
using net.atos.daf.ct2.package.entity;
using net.atos.daf.ct2.packageservice.Common;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.packageservice
{
    public class PackageManagementService : PackageService.PackageServiceBase
    {
        private readonly ILog _logger;
        private readonly IAuditTraillib _auditTraillib;
        private readonly IPackageManager _packageManager;
        private readonly PackageCdcHelper _packageCdcHelper;
        private readonly IPackageAlertCdcManager _packageMgmAlertCdcManager;
        public PackageManagementService(
                                        IAuditTraillib auditTrail,
                                        IPackageManager packageManager, IPackageAlertCdcManager packageMgmAlertCdcManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _auditTraillib = auditTrail;
            _packageManager = packageManager;
            _packageMgmAlertCdcManager = packageMgmAlertCdcManager;
            _packageCdcHelper = new PackageCdcHelper(_packageMgmAlertCdcManager);
        }


        public async override Task<PackageResponse> Create(PackageCreateRequest request, ServerCallContext context)
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
                    return await Task.FromResult(new PackageResponse
                    {
                        Message = "Package Code is " + package.Code + " already exists ",
                        PackageId = package.Id,
                        Code = Responsecode.Conflict
                    });
                }
                else
                {
                    return await Task.FromResult(new PackageResponse
                    {
                        Message = "Package Created " + package.Id,
                        PackageId = package.Id
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new PackageResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responsecode.Failed
                });
            }
        }


        //Update
        public async override Task<PackageResponse> Update(PackageCreateRequest request, ServerCallContext context)
        {
            try
            {
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");
                var orgContextId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("loggedd_in_orgContxtID")).FirstOrDefault()?.Value ?? "0");

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
                    return await Task.FromResult(new PackageResponse
                    {
                        Message = "Package Code is " + package.Code + " already exists ",
                        PackageId = package.Id,
                        Code = Responsecode.Conflict
                    });
                }
                else if (package.Id > 0)
                {
                    //Triggering package cdc 
                    if (request.FeatureIds != null && request.FeatureIds.Count() > 0)
                        await _packageCdcHelper.TriggerPackageCdc(package.Id, "U", accountId, loggedInOrgId, orgContextId, request.FeatureIds.ToArray());
                    return await Task.FromResult(new PackageResponse
                    {
                        Message = "Package Updated ",
                        PackageId = package.Id
                    });
                }
                else
                {
                    return await Task.FromResult(new PackageResponse
                    {
                        Message = "Package Not Updated " + package.Id,
                        PackageId = package.Id
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new PackageResponse
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

                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");
                var orgContextId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("loggedd_in_orgContxtID")).FirstOrDefault()?.Value ?? "0");
                IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);


                var result = _packageManager.Delete(request.Id).Result;
                var response = new PackageResponse();
                if (result)
                {
                    //Triggering package cdc 
                    if (featureIds != null && featureIds.Count() > 0)
                        await _packageCdcHelper.TriggerPackageCdc(request.Id, "D", accountId, loggedInOrgId, orgContextId, featureIds.ToArray());
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
                    Message = "Package Deletion Failed due to - " + ex.Message,

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
                    Type = x.Type,
                    State = x.State
                }).ToList());
                var packageImported = await _packageManager.Import(packages);
                var packagesRes = JsonConvert.SerializeObject(packageImported.PackageList);
                var duplicatePackagesRes = JsonConvert.SerializeObject(packageImported.DuplicatePackages);

                if (packagesRes != null & packagesRes.Length > 0)
                {
                    response.PackageList.AddRange(
                           JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<GetPackageRequest>>(packagesRes,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
                if (duplicatePackagesRes != null & duplicatePackagesRes.Length > 0)
                {
                    response.DuplicatePackages.AddRange(
                      JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<GetPackageRequest>>(duplicatePackagesRes,
                      new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
                var rejectedPackagesRes = JsonConvert.SerializeObject(request.RejectedPackages);
                if (rejectedPackagesRes != null && rejectedPackagesRes.Length > 0)
                {
                    response.RejectedPackages.AddRange(request.RejectedPackages.Select(x => new GetPackageRequest()
                    {
                        Code = x.Code ?? string.Empty,
                        Description = x.Description ?? string.Empty,
                        FeatureSetID = x.FeatureSetID,
                        Name = x.Name ?? string.Empty,
                        Type = x.Type ?? string.Empty,
                        State = x.State ?? string.Empty
                    }).ToList());
                }

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
        public async override Task<UpdatePackageStateResponse> UpdatePackageState(UpdatePackageStateRequest request, ServerCallContext context)
        {
            try
            {

                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");
                var orgContextId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("loggedd_in_orgContxtID")).FirstOrDefault()?.Value ?? "0");
                IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);


                var package = new Package();
                package.Id = request.PackageId;
                package.State = request.State;
                package = _packageManager.UpdatePackageState(package).Result;
                if (package.Id > 0)
                {
                    //Triggering package cdc 
                    if (featureIds != null && featureIds.Count() > 0)
                        await _packageCdcHelper.TriggerPackageCdc(package.Id, "U", accountId, loggedInOrgId, orgContextId, featureIds.ToArray());
                    return await Task.FromResult(new UpdatePackageStateResponse
                    {
                        Message = "Package state Updated ",
                        PackageStateResponse = request
                    });
                }
                else
                {
                    return await Task.FromResult(new UpdatePackageStateResponse
                    {
                        Message = "Package state not Updated " + package.Id,
                        PackageStateResponse = request
                    });
                }

            }
            catch (Exception ex)
            {
                return await Task.FromResult(new UpdatePackageStateResponse
                {
                    Message = "Exception " + ex.Message
                });
            }
        }
    }
}
