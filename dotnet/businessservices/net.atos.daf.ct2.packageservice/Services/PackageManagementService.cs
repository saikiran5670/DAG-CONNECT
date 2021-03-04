using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.package;
using net.atos.daf.ct2.package.entity;
using System;
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


        public override Task<PackageResponse> Create(PackageCreateRequest request, ServerCallContext context)
        {
            try
            {
                var package = new Package();
                package.Code = request.Code;
                package.Default = package.Default;
                package.FeatureSetID = package.FeatureSetID;
                // Is_Active = package.Default;,
                package.Name = package.Name;
                package.Pack_Type = package.Pack_Type;
                package.ShortDescription = package.ShortDescription;
                package.StartDate = Convert.ToDateTime(package.StartDate);
                package.EndDate = Convert.ToDateTime(package.EndDate);
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
                    Message = "Exception " + ex.Message
                });
            }
        }


    }
}
