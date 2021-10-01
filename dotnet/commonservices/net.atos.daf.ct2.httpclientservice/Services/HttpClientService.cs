using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.httpclient;
using net.atos.daf.ct2.httpclientservice.Entity;

namespace net.atos.daf.ct2.httpclientservice.Services
{

    public class HttpClientManagementService : HttpClientService.HttpClientServiceBase
    {
        private readonly ILog _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOTA22HttpClientManager _oTA22HttpClientManager;
        private readonly OTA22Configurations _oTA22Configurations;

        public HttpClientManagementService(IHttpClientFactory httpClientFactory,
                                           IConfiguration configuration,
                                           IOTA22HttpClientManager oTA22HttpClientManager)
        {
            _httpClientFactory = httpClientFactory;
            _oTA22HttpClientManager = oTA22HttpClientManager;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            configuration.GetSection("OTA22Configurations").Bind(_oTA22Configurations);
        }





    }
}
