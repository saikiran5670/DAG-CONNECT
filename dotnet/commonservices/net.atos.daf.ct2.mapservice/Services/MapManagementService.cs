using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.map;
using net.atos.daf.ct2.map.entity;

namespace net.atos.daf.ct2.mapservice
{
    public class MapManagementService : MapService.MapServiceBase
    {
        private readonly ILogger<MapManagementService> _logger;
        private readonly IAuditTraillib _auditTrail;
        private readonly IMapManager  _mapManager;
        public MapManagementService(ILogger<MapManagementService> logger,IMapManager mapManager)
        {
            _logger = logger;
            _mapManager = mapManager;
        }

        public async Task<GetMapResponse> GetMapAddress(GetMapRequest request, ServerCallContext context)
        {
            try
            {
                var response = new GetMapResponse();  
                var lookupAddress = new LookupAddress();       
                lookupAddress.Latitude = request.Latitude;
                lookupAddress.Longitude = request.Longitude;

                //var mapping = _mapManager.GetLookupAddress(lookupAddress).Result;
               
                _logger.LogInformation("Get Map details.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(null, ex);
                return await Task.FromResult(new GetMapResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responsecode.Failed
                });
            }
        }
    }
}
