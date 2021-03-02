using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace net.atos.daf.ct2.packageservice
{
    public class PackageManagementService: PackageService.PackageServiceBase
    {
        private readonly ILogger<PackageManagementService> _logger;
        public PackageManagementService(ILogger<PackageManagementService> logger)
        {
            _logger = logger;
        }
    }
}
