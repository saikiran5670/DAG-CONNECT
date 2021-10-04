using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.httpclientfactory;
using net.atos.daf.ct2.httpclientfactory.Entity.ota22;
using net.atos.daf.ct2.httpclientservice.Entity.ota22;

namespace net.atos.daf.ct2.httpclientservice.Services
{

    public class HttpClientManagementService : HttpClientService.HttpClientServiceBase
    {
        private readonly ILog _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOTA22HttpClientManager _oTA22HttpClientManager;
        private readonly OTA22Configurations _oTA22Configurations;
        private readonly Mapper _mapper;
        public HttpClientManagementService(IHttpClientFactory httpClientFactory,
                                           IConfiguration configuration,
                                           IOTA22HttpClientManager oTA22HttpClientManager)
        {
            _httpClientFactory = httpClientFactory;
            _oTA22HttpClientManager = oTA22HttpClientManager;
            _mapper = new Mapper();
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _oTA22Configurations = new OTA22Configurations();
            configuration.GetSection("OTA22Configurations").Bind(_oTA22Configurations);
        }

        public override async Task<VehiclesStatusOverviewResponse> GetVehiclesStatusOverview(VehiclesStatusOverviewRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("HttpClientManagementService:GetVehiclesStatusOverview Started.");

                httpclientfactory.entity.ota22.VehiclesStatusOverviewResponse apiResponse
                    = await _oTA22HttpClientManager.GetVehiclesStatusOverview(_mapper.MapVehiclesStatusOverviewRequest(request));
                return await Task.FromResult(_mapper.MapVehiclesStatusOverview(apiResponse));

            }
            catch (Exception ex)
            {
                _logger.Error($"HttpClientManagementService:GetVehiclesStatusOverview.Error:-{ex.Message}");
                return await Task.FromResult(new VehiclesStatusOverviewResponse
                {
                    HttpStatusCode = 500,
                    Message = $"HttpClientManagementService:GetVehiclesStatusOverview- Error:-{ex.Message}"
                });
            }
        }

        public override async Task<VehicleUpdateDetailsResponse> GetVehicleUpdateDetails(VehicleUpdateDetailsRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("HttpClientManagementService:GetVehiclesStatusOverview Started.");

                httpclientfactory.entity.ota22.VehicleUpdateDetailsResponse apiResponse
                    = await _oTA22HttpClientManager.GetVehicleUpdateDetails(_mapper.MapGetVehicleUpdateDetailsRequest(request));
                return await Task.FromResult(_mapper.MapGetVehicleUpdateDetails(apiResponse));

            }
            catch (Exception ex)
            {
                _logger.Error($"HttpClientManagementService:GetVehiclesStatusOverview.Error:-{ex.Message}");
                return await Task.FromResult(new VehicleUpdateDetailsResponse
                {
                    HttpStatusCode = 500,
                    Message = $"HttpClientManagementService:GetVehiclesStatusOverview- Error:-{ex.Message}"
                });
            }
        }

    }
}
