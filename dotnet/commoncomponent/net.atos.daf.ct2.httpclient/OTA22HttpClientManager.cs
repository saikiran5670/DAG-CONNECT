using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.httpclientfactory.entity.ota22;
using net.atos.daf.ct2.httpclientfactory.Entity.ota22;
using net.atos.daf.ct2.httpclientfactory.extensions;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclientfactory
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
            _oTA22Configurations = new OTA22Configurations();
            configuration.GetSection("OTA22Configurations").Bind(_oTA22Configurations);
        }

        public async Task<VehiclesStatusOverviewResponse> GetVehiclesStatusOverview(VehiclesStatusOverviewRequest request)
        {
            int i = 0;
            string result = null;
            try
            {
                _logger.Info("OTA22HttpClientManager:GetVehiclesStatusOverview Started.");
                var client = await GetHttpClient();
                var data = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;

                while (!(response.StatusCode == HttpStatusCode.OK) && i < _oTA22Configurations.RETRY_COUNT)
                {
                    _logger.Info("GetVehiclesStatusOverview:Calling OTA 22 rest API for sending data");
                    response = await client.PostAsync($"{_oTA22Configurations.API_BASE_URL}vehiclesstatusoverview", data);

                    _logger.Info("GetVehiclesStatusOverview: OTA 22 Api respone is " + response.StatusCode);
                    result = response.Content.ReadAsStringAsync().Result;

                    i++;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.Info(result);
                }
                else
                {
                    _logger.Error(result);
                }

                return new VehiclesStatusOverviewResponse { HttpStatusCode = 200, VehiclesStatusOverview = JsonConvert.DeserializeObject<VehiclesStatusOverview>(result) };
            }
            catch (Exception ex)
            {
                _logger.Error($"OTA22HttpClientManager:GetVehiclesStatusOverview.Error:-{ex.Message}");
                return new VehiclesStatusOverviewResponse { HttpStatusCode = 500 };
            }
        }

        public async Task<VehicleUpdateDetailsResponse> GetVehicleUpdateDetails(VehicleUpdateDetailsRequest request)
        {
            int i = 0;
            string result = null;
            try
            {
                _logger.Info("OTA22HttpClientManager:GetVehicleUpdateDetails Started.");
                var client = await GetHttpClient();
                //var data = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_oTA22Configurations.API_BASE_URL}vehicles/vin");
                httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpRequest.Content = new StringContent(JsonConvert.SerializeObject(request));
                httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;

                while (!(response.StatusCode == HttpStatusCode.OK) && i < _oTA22Configurations.RETRY_COUNT)
                {
                    _logger.Info("GetVehicleUpdateDetails:Calling OTA 22 rest API for sending data");
                    response = await client.SendAsync(httpRequest);

                    _logger.Info("GetVehicleUpdateDetails:OTA 22 respone is " + response.StatusCode);
                    result = response.Content.ReadAsStringAsync().Result;

                    i++;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.Info(result);
                }
                else
                {
                    _logger.Error(result);
                }

                return new VehicleUpdateDetailsResponse { HttpStatusCode = 200, VehicleUpdateDetails = JsonConvert.DeserializeObject<VehicleUpdateDetails>(result) };
            }
            catch (Exception ex)
            {
                _logger.Error($"OTA22HttpClientManager:GetVehicleUpdateDetails.Error:-{ex.Message}");
                return new VehicleUpdateDetailsResponse { HttpStatusCode = 500 };
            }
        }

        public async Task<CampiagnSoftwareReleaseNoteResponse> GetSoftwareReleaseNote(CampiagnSoftwareReleaseNoteRequest request)
        {
            int i = 0;
            string result = null;
            try
            {
                _logger.Info("OTA22HttpClientManager:GetSoftwareReleaseNote Started.");
                var client = await GetHttpClient();
                var data = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;

                while (!(response.StatusCode == HttpStatusCode.OK) && i < _oTA22Configurations.RETRY_COUNT)
                {
                    _logger.Info("GetSoftwareReleaseNote:Calling OTA 22 rest API for sending data");
                    response = await client.PostAsync($"{_oTA22Configurations.API_BASE_URL}softwareupdateoverview", data);

                    _logger.Info("GetSoftwareReleaseNote:OTA 22 respone is " + response.StatusCode);
                    result = response.Content.ReadAsStringAsync().Result;

                    i++;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.Info(result);
                }
                else
                {
                    _logger.Error(result);
                }

                return new CampiagnSoftwareReleaseNoteResponse { HttpStatusCode = 200, CampiagnSoftwareReleaseNote = JsonConvert.DeserializeObject<CampiagnSoftwareReleaseNote>(result) };
            }
            catch (Exception ex)
            {
                _logger.Error($"OTA22HttpClientManager:GetSoftwareReleaseNote.Error:-{ex.Message}");
                return new CampiagnSoftwareReleaseNoteResponse { HttpStatusCode = 500 };
            }
        }

        /// <summary>
        /// This method is used to create Http Client wit hautherization token
        /// </summary>
        /// <returns></returns>
        private async Task<HttpClient> GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = new TimeSpan(0, 0, 30);
            var token = await GetElibilityToken(client);
            //client.DefaultRequestHeaders.Accept.Clear();
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
