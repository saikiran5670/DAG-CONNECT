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
using net.atos.daf.ct2.httpclientfactory.entity.ota14;
using net.atos.daf.ct2.httpclientfactory.extensions;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;
namespace net.atos.daf.ct2.httpclientfactory
{
    public class OTA14HttpClientManager : IOTA14HttpClientManager
    {
        private readonly ILog _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OTA14Configurations _oTA14Configurations;
        private OTA14Token _token;

        public OTA14HttpClientManager(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _oTA14Configurations = new OTA14Configurations();
            configuration.GetSection("OTA14Configurations").Bind(_oTA14Configurations);
        }

        public async Task<ScheduleSoftwareUpdateResponse> PostManagerApproval(ScheduleSoftwareUpdateRequest request)
        {
            int i = 0;
            long boashtimestamp;
            try
            {
                _logger.Info("OTA14HttpClientManager:GetSoftwareScheduleUpdate Started.");
                var client = await GetHttpClient();
                request.ApprovalMessage = $"{_oTA14Configurations.Message_Approval} {request?.AccountEmailId}";
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;
                string etag = string.Empty;
                client.DefaultRequestHeaders.Accept.Clear();

                while (!(response.StatusCode == HttpStatusCode.OK) && i < _oTA14Configurations.RETRY_COUNT)
                {
                    response = await client.GetAsync($"{_oTA14Configurations.API_BASE_URL}{request.BaseLineId}");
                    etag = response.Headers.ETag?.ToString();
                    i++;
                }
                _logger.Info($"GetSoftwareScheduleUpdate:Calling OTA 14 Get API for sending data ETag: {etag}");
                client = await GetHttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                if (etag != null)
                    client.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", etag.Replace("\"", ""));
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                var reqObj = new ScheduleSoftwareUpdateReq { ApprovalMessage = request.ApprovalMessage, SchedulingTime = request.SchedulingTime };
                var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_oTA14Configurations.API_BASE_URL}{request.BaseLineId}/managerApprove");

                httpRequest.Content = new StringContent(JsonConvert.SerializeObject(reqObj), Encoding.UTF8, "application/json");

                boashtimestamp = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                _logger.Info("GetSoftwareScheduleUpdate:Calling OTA 14 Post API for sending data");
                response = await client.SendAsync(httpRequest);

                _logger.Info("GetSoftwareScheduleUpdate:OTA 14 Post respone is " + response.StatusCode);
                string result = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.Info($"GetSoftwareScheduleUpdate:ETag: {etag}, request: {JsonConvert.SerializeObject(request)} , result : {result}");
                }
                else
                {
                    _logger.Info($"GetSoftwareScheduleUpdate:ETag: {etag}, request: {JsonConvert.SerializeObject(request)} , result : {result}");
                    return new ScheduleSoftwareUpdateResponse { HttpStatusCode = (int)response.StatusCode, BoashTimesStamp = boashtimestamp };
                }
                return new ScheduleSoftwareUpdateResponse { HttpStatusCode = 200, BoashTimesStamp = boashtimestamp };
            }
            catch (Exception ex)
            {
                _logger.Error($"OTA14HttpClientManager:GetSoftwareScheduleUpdate.Error:-{ex.Message}");
                boashtimestamp = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                return new ScheduleSoftwareUpdateResponse { HttpStatusCode = 500, BoashTimesStamp = boashtimestamp };
            }
        }

        private async Task<HttpClient> GetEtag(HttpClient client)
        {
            await client.GetAsync($"{_oTA14Configurations.API_BASE_URL}");
            return client;
        }

        /// <summary>
        /// This method is used to create Http Client wit hautherization token
        /// </summary>
        /// <returns></returns>
        private async Task<HttpClient> GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.Timeout = new TimeSpan(0, 0, 30);
            if (_token == null)
            {
                _token = await GetElibilityToken(client);
                _logger.Info($"GetSoftwareScheduleUpdate:Calling OTA 14 Token API for sending data.");
            }
            //client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);
            return client;
        }

        /// <summary>
        /// This method is used to get tokens from Oauth2 provider
        /// </summary>
        /// <returns></returns>
        private async Task<OTA14Token> GetElibilityToken(HttpClient client)
        {
            var form = new Dictionary<string, string>
                {
                    {"grant_type", _oTA14Configurations.GRANT_TYPE},
                    {"client_id", _oTA14Configurations.CLIENT_ID},
                    {"client_secret", _oTA14Configurations.CLIENT_SECRET},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync(_oTA14Configurations.AUTH_URL, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<OTA14Token>(jsonContent);
            return token;
        }
    }
}
