using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.map;
using net.atos.daf.ct2.map.entity;

namespace net.atos.daf.ct2.mapservice
{
    public class MapManagementService : MapService.MapServiceBase
    {
        private readonly ILogger<MapManagementService> _logger;
        private readonly IMapManager _mapManager;
        private readonly IConfiguration _configuration;
        private readonly HereMapConfiguration _apiConfiguration;
        public MapManagementService(ILogger<MapManagementService> logger, IMapManager mapManager, IConfiguration config)
        {
            _logger = logger;
            _mapManager = mapManager;
            _configuration = config;
            _apiConfiguration = new HereMapConfiguration();
            _configuration.GetSection("HereMapCofiguration").Bind(_apiConfiguration);
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
                var response = new GetMapResponse();
                _mapManager.InitializeMapGeocoder(_apiConfiguration);
                var mapping = _mapManager.GetMapAddress(lookupAddress).Result;
                if (string.IsNullOrEmpty(mapping.Address))
                {
                    response.Code = MapResponsecode.Conflict;
                    response.Message = "Invalid format of latitue and longitude value";
                }

                else
                {
                    response.Code = MapResponsecode.Success;
                    response.Message = "Success";
                }
                response.LookupAddresses = new GetMapRequest()
                {
                    Address = mapping.Address ?? string.Empty,
                    Id = mapping.Id,
                    Latitude = mapping.Latitude,
                    Longitude = mapping.Longitude
                };
                _logger.LogInformation("Get Map details.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(null, ex);
                return await Task.FromResult(new GetMapResponse
                {
                    LookupAddresses = new GetMapRequest(),
                    Message = "Exception " + ex.Message,
                    Code = MapResponsecode.Failed
                });
            }
        }
    }
}
