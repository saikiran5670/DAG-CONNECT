using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.map;
using net.atos.daf.ct2.map.entity;

namespace net.atos.daf.ct2.mapservice
{
    public class MapManagementService : MapService.MapServiceBase
    {
        private readonly ILogger<MapManagementService> _logger;
        private readonly IMapManager  _mapManager;
        public MapManagementService(ILogger<MapManagementService> logger,IMapManager mapManager)
        {
            _logger = logger;
            _mapManager = mapManager;
        }



        


       

        public override async Task<GetMapResponse> GetMapAddress(GetMapRequest request, ServerCallContext context)
        {
            try
            {
               
                var lookupAddress = new LookupAddress
                {
                    Latitude = request.Latitude,
                    Longitude = request.Longitude
                };
                string appId = "LRJH9LmTMNbwteXRz03L";
                string appCode = "o9LPYEnoFvNtmkYUhCb1Tg";
                _mapManager.InitializeMapGeocoder(appId, appCode);
                var mapping = _mapManager.GetMapAddress(lookupAddress).Result;
                var response = new GetMapResponse()
                {
                    Code = MapResponsecode.Success,
                    LookupAddresses = new GetMapRequest()
                    {
                        Address = mapping.Address,
                        Id = mapping.Id,
                        Latitude = mapping.Latitude,
                        Longitude = mapping.Longitude
                    },
                    Message = "Success"
                };
                _logger.LogInformation("Get Map details.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(null, ex);
                return await Task.FromResult(new GetMapResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = MapResponsecode.Failed
                });
            }
        }
    }
}
