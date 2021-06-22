using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.identity.entity;

namespace net.atos.daf.ct2.identity
{
    public class AccountAuthenticator : IAccountAuthenticator
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly IdentityJsonConfiguration _settings;
        public AccountAuthenticator(IOptions<IdentityJsonConfiguration> setting)
        {
            _settings = setting.Value;
        }
        /// <summary>
        ///  This method will be used to obtain the access token for a user
        /// </summary>
        /// <param name="user"> User model that will having username and password as an input</param>
        /// <returns>Httpstatuscode along with Authentication token as a JSON or error message if any</returns>
        public async Task<Response> AccessToken(Identity user)
        {
            Response response = new Response();
            try
            {
                var requestContent = string.Format("username={0}&password={1}&grant_type={2}&client_id={3}",
                Uri.EscapeDataString(user.UserName),
                Uri.EscapeDataString(user.Password),
                Uri.EscapeDataString("password"),
                Uri.EscapeDataString(_settings.AuthClientId)
                );

                // var querystring = new StringContent("username="+user.UserName+"&password="+user.Password+"&grant_type=password&client_id="+_settings.AuthClientId, Encoding.UTF8, "application/x-www-form-urlencoded");
                var querystring = new StringContent(requestContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                var url = _settings.BaseUrl + _settings.AuthUrl;
                url = url.Replace("{{realm}}", _settings.Realm);

                var httpResponse = await _client.PostAsync(url, querystring);
                string result = httpResponse.Content.ReadAsStringAsync().Result;
                response.StatusCode = httpResponse.StatusCode;
                response.Result = result;
                return response;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  This action will for testing purpose and need to be delete later on 
        /// </summary>
        /// <param name="user"> User model that will have username and password as an input</param>
        /// <returns>check server link and password</returns>
        public string GetURL(Identity user)
        {
            try
            {
                var querystring = new StringContent("username=" + user.UserName + "&password=" + user.Password + "&grant_type=password&client_id=" + _settings.AuthClientId, Encoding.UTF8, "application/x-www-form-urlencoded");
                var url = _settings.BaseUrl + _settings.AuthUrl;
                url = url.Replace("{{realm}}", _settings.Realm);
                return url + ";    " + user.UserName + ": " + user.Password;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}