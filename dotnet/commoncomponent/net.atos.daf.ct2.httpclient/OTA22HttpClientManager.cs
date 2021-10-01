using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.httpclient.entity;
using net.atos.daf.ct2.httpclient.Entity;
using net.atos.daf.ct2.httpclient.ENUM;
using net.atos.daf.ct2.httpclient.extensions;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclient
{

    public class OTA22HttpClientManager : IOTA22HttpClientManager
    {

        private readonly ILog _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OTA22Configurations _oTA22Configurations;

        public OTA22HttpClientManager(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            configuration.GetSection("OTA22Configurations").Bind(_oTA22Configurations);
        }

        public async Task<VehiclesStatusOverviewResponse> GetVehiclesStatusOverview(VehiclesStatusOverviewRequest request)
        {
            try
            {
                var client = await GetHttpClient();
                var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_oTA22Configurations.API_BASE_URL}\vehiclesstatusoverview");
                httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var serializedRequest = JsonConvert.SerializeObject(request);
                httpRequest.Content = new StringContent(serializedRequest);
                using (var response = await client.SendAsync(httpRequest,
                    HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        // inspect the status code
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return new VehiclesStatusOverviewResponse { HttpStatusCode = response.StatusCode };
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            return new VehiclesStatusOverviewResponse { HttpStatusCode = response.StatusCode };
                        }

                    }
                    response.EnsureSuccessStatusCode();
                    var stream = await response.Content.ReadAsStreamAsync();
                    return new VehiclesStatusOverviewResponse { HttpStatusCode = response.StatusCode, VehiclesStatusOverview = stream.ReadAndDeserializeFromJson<VehiclesStatusOverview>() };
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// This method is used to create Http Client wit hautherization token
        /// </summary>
        /// <returns></returns>
        private async Task<HttpClient> GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient("OTA22Client");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var token = await GetElibilityToken(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return client;
        }

        /// <summary>
        /// This method is used to get tokens from Oauth2 provider
        /// </summary>
        /// <returns></returns>
        private async Task<OTA22Token> GetElibilityToken(HttpClient client)
        {
            var form = new Dictionary<string, string>
                {
                    {"grant_type", _oTA22Configurations.GRANT_TYPE},
                    {"scope", _oTA22Configurations.CLIENT_SCOPE},
                    {"client_id", _oTA22Configurations.CLIENT_ID},
                    {"client_secret", _oTA22Configurations.CLIENT_SECRET},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync(_oTA22Configurations.AUTH_URL, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<OTA22Token>(jsonContent);
            return token;
        }
    }
}
