using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.identity.entity;

namespace net.atos.daf.ct2.identity
{
    public class AccountManager: IAccountManager
    {
        private HttpClient client = new HttpClient();
        private readonly IdentityJsonConfiguration _settings;
        public AccountManager(IOptions<IdentityJsonConfiguration> setting)
        {
            _settings = setting.Value;
        }
        /// <summary>
        ///  This method will be used to insert user onto keycloak server
        /// </summary>
        /// <param name="user"> User model that will having information of user creation</param>
        /// <returns>Httpstatuscode along with success or failed message if any</returns>
        public async Task<Response> CreateUser(Identity user)
        {
            IDPToken token = new IDPToken();
            String accessToekn= string.Empty;
            Response objResponse= new Response();
            try
            {
                var querystringSAT = new StringContent("client_id="+_settings.UserMgmClientId+"&client_secret="+_settings.UserMgmClientSecret+"&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var urlSAT = _settings.BaseUrl + _settings.AuthUrl.Replace("{{realm}}",_settings.Realm);
                // url=url.Replace("{{realm}}",realm);

                var httpResponseSAT = await client.PostAsync(urlSAT, querystringSAT);
                string resultSAT = httpResponseSAT.Content.ReadAsStringAsync().Result;
                if(httpResponseSAT.StatusCode==System.Net.HttpStatusCode.OK)
                {
                    token = JsonConvert.DeserializeObject<IDPToken>(resultSAT);
                    accessToekn=token.access_token;
                    client = new HttpClient();
                    client = PrepareClientHeader(token.access_token);

                    var contentData = new StringContent(GetUserBody(user,"","INSERT"),System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = client.PostAsync(_settings.UserMgmUrl.Replace("{{realm}}",_settings.Realm),contentData).Result;
                    if(httpResponse.IsSuccessStatusCode && httpResponse.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        objResponse.StatusCode=httpResponse .StatusCode;
                        objResponse.Result=JsonConvert.SerializeObject("User has been created.");
                    }
                    else
                    {
                        objResponse.StatusCode=httpResponse .StatusCode;
                        objResponse.Result=httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
                else 
                {
                    objResponse.StatusCode=httpResponseSAT.StatusCode;
                    objResponse.Result=resultSAT;
                }
                return objResponse;      
            }   
            catch(System.Exception)
            {
                throw;
            }
        }
        public async Task<Response> UpdateUser(Identity user)
        {
            return await UpdateOrDeleteUser(user,"UPDATE");
        }
        public async Task<Response> DeleteUser(Identity user)
        {
            return await UpdateOrDeleteUser(user,"DELETE");
        }
        public async Task<Response> ChangeUserPassword(Identity user)
        {
            return await UpdateOrDeleteUser(user,"CHANGEPASSWORD");
        }
        private async Task<Response> UpdateOrDeleteUser(Identity user,string actionType)
        {
            IDPToken token = new IDPToken();
            String accessToekn= string.Empty;
            Response objResponse= new Response();
            try
            {
                var querystringSAT = new StringContent("client_id="+_settings.UserMgmClientId+"&client_secret="+_settings.UserMgmClientSecret+"&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var urlSAT = _settings.BaseUrl + _settings.AuthUrl.Replace("{{realm}}",_settings.Realm);

                var httpResponseSAT = await client.PostAsync(urlSAT, querystringSAT);
                string resultSAT = httpResponseSAT.Content.ReadAsStringAsync().Result;
                if(httpResponseSAT.StatusCode==System.Net.HttpStatusCode.OK)
                {
                    token = JsonConvert.DeserializeObject<IDPToken>(resultSAT);
                    HttpResponseMessage httpResponseUser = await GetUserIdByEmail(token.access_token,user);
                    if(httpResponseUser.StatusCode==System.Net.HttpStatusCode.OK)
                    {
                        JArray userJSON = JsonConvert.DeserializeObject<JArray>(httpResponseUser.Content.ReadAsStringAsync().Result);
                        if(userJSON !=null && userJSON.Count>0 )
                        {
                            string keycloakUserId=string.Empty;
                            foreach(JObject obj in userJSON)
                            {
                                keycloakUserId=obj.GetValue("id").ToString();
                                break;
                            }
                            client = new HttpClient();
                            client = PrepareClientHeader(token.access_token);
                            var contentData = new StringContent(GetUserBody(user,keycloakUserId,actionType),System.Text.Encoding.UTF8, "application/json");
                            HttpResponseMessage httpResponse = client.PutAsync(_settings.UserMgmUrl.Replace("{{realm}}",_settings.Realm) +"/"+keycloakUserId +"/"+"reset-password",contentData).Result;
                            if(httpResponse.IsSuccessStatusCode && httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                            {
                                objResponse.StatusCode=httpResponse .StatusCode;
                                if(actionType=="UPDATE")
                                {
                                 objResponse.Result=JsonConvert.SerializeObject("User has been updated.");
                                }
                                if(actionType=="DELETE")
                                {
                                 objResponse.Result=JsonConvert.SerializeObject("User has been deleted.");
                                }
                                if(actionType=="CHANGEPASSWORD")
                                {
                                 objResponse.Result=JsonConvert.SerializeObject("User password has been changed.");
                                }
                            }
                            else
                            {
                                objResponse.StatusCode=httpResponse .StatusCode;
                                objResponse.Result=httpResponse.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else
                        {
                            objResponse.StatusCode=System.Net.HttpStatusCode.NotFound;
                            objResponse.Result="{error: User is not available}";
                        }
                    }
                    else 
                    {
                        objResponse.StatusCode=httpResponseSAT.StatusCode;
                        objResponse.Result=resultSAT;
                    }
                }
                else 
                {
                    objResponse.StatusCode=httpResponseSAT.StatusCode;
                    objResponse.Result=resultSAT;
                }
                return objResponse;      
            }   
            catch(System.Exception)
            {
                throw;
            }
        }
        private HttpClient PrepareClientHeader(string accesstoken)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(_settings.BaseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            client.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer", accesstoken);                    
            return client;
        }
        
        private async Task<HttpResponseMessage> GetUserIdByEmail(string accesstoken, Identity user)
        {
            string strUserId=string.Empty;
            client = new HttpClient();
            client = PrepareClientHeader(accesstoken);            
            return await client.GetAsync(_settings.UserMgmUrl.Replace("{{realm}}",_settings.Realm) + "?username=" + user.UserName);
        }
        private string GetUserBody(Identity user, string keycloakUserid = null, string actiontype="")
        {
            StringBuilder stringData = new StringBuilder();
            stringData.Append("{");
            switch(actiontype.ToUpper())
            {
                case "INSERT":  
                                stringData.Append("\"username\": \"" + user.UserName + "\",");
                                stringData.Append("\"email\": \"" + user.EmailId + "\",");
                                stringData.Append("\"firstName\": \"" + user.FirstName+ "\",");
                                stringData.Append("\"lastName\": \"" + user.LastName+ "\"," );
                                stringData.Append("\"enabled\": \"true\"");       
                                if(!string.IsNullOrEmpty(user.Password))
                                {         
                                    stringData.Append(",");
                                    stringData.Append("\"credentials\": [");
                                    stringData.Append("{");
                                    stringData.Append("\"type\": \"password\",");
                                    stringData.Append("\"value\": \"" + user.Password + "\"");
                                    stringData.Append("}");
                                    stringData.Append("]");
                                }
                                // stringData.Append(",");
                                // stringData.Append("\"attributes\": [");
                                // stringData.Append("{");
                                // stringData.Append("\"dafroles\": \"" + userModel.dafroles + "\""  );
                                // stringData.Append("}");
                                // stringData.Append("]");
                                break;
                case "UPDATE":  
                                stringData.Append("\"id\": \"" + keycloakUserid + "\",");
                                stringData.Append("\"firstName\": \"" + user.FirstName+ "\",");
                                stringData.Append("\"lastName\": \"" + user.LastName+ "\"" );
                                // stringData.Append(",");
                                // stringData.Append("\"attributes\": [");
                                // stringData.Append("{");
                                // stringData.Append("\"dafroles\": \"" + userModel.dafroles + "\""  );
                                // stringData.Append("}");
                                // stringData.Append("]");
                                break;
                case "DELETE":  
                                stringData.Append("\"id\": \"" + keycloakUserid + "\",");
                                stringData.Append("\"enabled\": \"false\"");                
                                break;
                case "CHANGEPASSWORD":  
                                stringData.Append("\"type\":\"password\""+",");
                                stringData.Append("\"value\":\"" + user.Password + "\"" + ",");
                                stringData.Append("\"temporary\": false");
                                break;
            } 
            stringData.Append("}");
            return stringData.ToString();
        }
    }
}