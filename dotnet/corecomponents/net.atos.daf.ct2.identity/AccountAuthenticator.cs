using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;

namespace net.atos.daf.ct2.identity
{
    public class AccountAuthenticator:IAccountAuthenticator
    {
       private string baseUrl,authUrl,AuthClientId,AuthClientSecret=string.Empty;
       private string realm=string.Empty;
        private HttpClient client = new HttpClient();
       public AccountAuthenticator()
       {
            var setting = ConfigHelper.GetConfig();
            baseUrl=setting["KeycloakStrings:baseUrl"];
            authUrl=setting["KeycloakStrings:authUrl"];
            AuthClientId=setting["KeycloakStrings:AuthClientId"];
            AuthClientSecret=setting["KeycloakStrings:AuthClientSecret"];
            realm=setting["KeycloakStrings:realm"];
       }  
        /// <summary>
        ///  This method will be used to obtain the access token for a user
        /// </summary>
        /// <param name="user"> User model that will having username and password as an input</param>
        /// <returns>Httpstatuscode along with Authentication token as a JSON or error message if any</returns>
        public async Task<Response> AccessToken(Identity user)
        {
            Response response= new Response();
            try
            {
                var querystring = new StringContent("username="+user.UserName+"&password="+user.Password+"&grant_type=password&client_id="+AuthClientId, Encoding.UTF8, "application/x-www-form-urlencoded");
                var url = baseUrl + authUrl;
                url=url.Replace("{{realm}}",realm);

                var httpResponse = await client.PostAsync(url, querystring);
                string result = httpResponse.Content.ReadAsStringAsync().Result;
                response.StatusCode=response.StatusCode;
                response.Result=result;
                return response;      
            }   
            catch(System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  This action will for testing purpose and need to be delete later on 
        /// </summary>
        /// <param name="user"> User model that will have username and password as an input</param>
        /// <returns>check server link and password</returns>
        public string getURL(Identity user)
        {
            try
            {
                var querystring = new StringContent("username="+user.UserName+"&password="+user.Password+"&grant_type=password&client_id="+AuthClientId, Encoding.UTF8, "application/x-www-form-urlencoded");
                var url = baseUrl + authUrl;
                url=url.Replace("{{realm}}",realm);
                return url + ";    "+ user.UserName +": "+user.Password;      
            }   
            catch(System.Exception)
            {
                throw;
            }
        }
    }
}