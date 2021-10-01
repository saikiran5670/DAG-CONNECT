using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.httpclientservice.Entity;

namespace net.atos.daf.ct2.httpclientservice.Services
{

    public class HttpClientManagementService : HttpClientService.HttpClientServiceBase
    {
        private readonly ILog _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OTA22Configurations _oTA22Configurations;

        public HttpClientManagementService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            configuration.GetSection("OTA22Configurations").Bind(_oTA22Configurations);
        }



        /// <summary>
        /// This mthod is used to get tokens from Oauth2 provider
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetElibilityToken()
        {
            var client = _httpClientFactory.CreateClient("OTA22AuthClient");
            var form = new Dictionary<string, string>
                {
                    {"grant_type", _oTA22Configurations.GRANT_TYPE},
                    {"scope", _oTA22Configurations.CLIENT_SCOPE},
                    {"client_id", _oTA22Configurations.CLIENT_ID},
                    {"client_secret", _oTA22Configurations.CLIENT_SECRET},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync("",new FormUrlEncodedContent(form));
            return await tokenResponse.Content.ReadAsStringAsync();
        }

    }
}
