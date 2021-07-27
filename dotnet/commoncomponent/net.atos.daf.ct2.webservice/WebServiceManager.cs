using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.webservice.entity;

namespace net.atos.daf.ct2.webservice
{
    public class WebServiceManager
    {
        private HttpClient PrepareClientHeader(HeaderDetails header)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(header.BaseUrl);
                var contentType = new MediaTypeWithQualityHeaderValue(header.ContentType);
                client.DefaultRequestHeaders.Accept.Add(contentType);
                if (header.AuthType == "A")
                {
                    var byteArray = Encoding.ASCII.GetBytes(header.UserName + ":" + header.Password);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }

                return client;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HttpResponseMessage> HttpClientCall(HeaderDetails header)
        {
            try
            {
                //HttpClientHandler clientHandler = new HttpClientHandler();
                //clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                // Pass the handler to httpclient(from you are calling api)
                HttpClient client = new HttpClient();
                client = PrepareClientHeader(header);
                var contentData = new StringContent(header.Body, System.Text.Encoding.UTF8, header.ContentType);
                HttpResponseMessage httpResponse = await client.PostAsync(header.BaseUrl, contentData);
                return httpResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
