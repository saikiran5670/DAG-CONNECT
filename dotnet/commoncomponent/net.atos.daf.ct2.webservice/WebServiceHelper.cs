using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.webservice
{
    public class WebServiceHelper
    {
        private readonly HttpClient _client = new HttpClient();

        private HttpClient PrepareClientHeader(string url)
        {
            object Credentials = new object();
            string cred = JsonConvert.SerializeObject(Credentials);
            _client.BaseAddress = new Uri(url);
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cred);
            return _client;
        }
    }
}
