using Microsoft.Extensions.Logging;

namespace net.atos.daf.ct2.mapservice
{
    public class MapManagementService : MapService.MapServiceBase
    {
        private readonly ILogger<MapManagementService> _logger;
        public MapManagementService(ILogger<MapManagementService> logger)
        {
            _logger = logger;
        }       
    }
}
