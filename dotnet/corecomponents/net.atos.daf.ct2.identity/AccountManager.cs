using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;

namespace net.atos.daf.ct2.identity
{
    public class AccountManager: IAccountManager
    {
     private string baseUrl,authUrl,userMgmUrl,AuthClientId,
     AuthClientSecret,UserMgmClientId,UserMgmClientSecret,realm=string.Empty;  
     private HttpClient client = new HttpClient();

       public AccountManager()
       {
            var setting = ConfigHelper.GetConfig();
            baseUrl=setting["KeycloakStrings:baseUrl"];
            authUrl=setting["KeycloakStrings:authUrl"];
            userMgmUrl=setting["KeycloakStrings:userMgmUrl"];
            AuthClientId=setting["KeycloakStrings:AuthClientId"];
            AuthClientSecret=setting["KeycloakStrings:AuthClientSecret"];
            UserMgmClientId=setting["KeycloakStrings:UserMgmClientId"];
            UserMgmClientSecret=setting["KeycloakStrings:UserMgmClientSecret"];
            realm=setting["KeycloakStrings:realm"];
       }
        /// <summary>
        ///  This method will be used to insert user onto keycloak server
        /// </summary>
        /// <param name="user"> User model that will having information of user creation</param>
        /// <returns>Httpstatuscode along with success or failed message if any</returns>
        public async Task<Response> CreateUser(Account user)
        {
            Token token = new Token();
            String accessToekn= string.Empty;
            Response objResponse= new Response();
            try
            {
                var querystringSAT = new StringContent("client_id="+UserMgmClientId+"&client_secret="+UserMgmClientSecret+"&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var urlSAT = baseUrl + authUrl.Replace("{{realm}}",realm);
                // url=url.Replace("{{realm}}",realm);

                var httpResponseSAT = await client.PostAsync(urlSAT, querystringSAT);
                string resultSAT = httpResponseSAT.Content.ReadAsStringAsync().Result;
                if(httpResponseSAT.StatusCode==System.Net.HttpStatusCode.OK)
                {
                    token = JsonConvert.DeserializeObject<Token>(resultSAT);
                    accessToekn=token.access_token;
                    client = new HttpClient();
                    client = PrepareClientHeader(baseUrl,token.access_token);

                    var contentData = new StringContent(GetUserBody(user,"","INSERT"),System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = client.PostAsync(userMgmUrl.Replace("{{realm}}",realm),contentData).Result;
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
        public async Task<Response> UpdateUser(Account user)
        {
            return await UpdateOrDeleteUser(user,"UPDATE");
        }
        public async Task<Response> DeleteUser(Account user)
        {
            return await UpdateOrDeleteUser(user,"DELETE");
        }
        public async Task<Response> ChangeUserPassword(Account user)
        {
            return await UpdateOrDeleteUser(user,"CHANGEPASSWORD");
        }
        private async Task<Response> UpdateOrDeleteUser(Account user,string actionType)
        {
            Token token = new Token();
            String accessToekn= string.Empty;
            Response objResponse= new Response();
            try
            {
                var querystringSAT = new StringContent("client_id="+UserMgmClientId+"&client_secret="+UserMgmClientSecret+"&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var urlSAT = baseUrl + authUrl.Replace("{{realm}}",realm);

                var httpResponseSAT = await client.PostAsync(urlSAT, querystringSAT);
                string resultSAT = httpResponseSAT.Content.ReadAsStringAsync().Result;
                if(httpResponseSAT.StatusCode==System.Net.HttpStatusCode.OK)
                {
                    token = JsonConvert.DeserializeObject<Token>(resultSAT);
                    HttpResponseMessage httpResponseUser = await GetUserIdByEmail(baseUrl,token.access_token,user);
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
                            client = PrepareClientHeader(baseUrl,token.access_token);
                            var contentData = new StringContent(GetUserBody(user,keycloakUserId,actionType),System.Text.Encoding.UTF8, "application/json");
                            HttpResponseMessage httpResponse = client.PutAsync(userMgmUrl.Replace("{{realm}}",realm) +"/"+keycloakUserId,contentData).Result;
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
        private HttpClient PrepareClientHeader(string baseUrl,string accesstoken)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            client.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer", accesstoken);                    
            return client;
        }
        
        private async Task<HttpResponseMessage> GetUserIdByEmail(string baseUrl,string accesstoken, Account user)
        {
            string strUserId=string.Empty;
            client = new HttpClient();
            client = PrepareClientHeader(baseUrl,accesstoken);            
            return await client.GetAsync(userMgmUrl.Replace("{{realm}}",realm) + "?username=" + user.UserName);
        }
        private string GetUserBody(Account user, string keycloakUserid = null, string actiontype="")
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
                                stringData.Append("\"type\": \"password"+",");
                                stringData.Append("\"value\": \"" + user.Password+ "\",");
                                stringData.Append("\"temporary\": \"false" );
                                break;
            } 
            stringData.Append("}");
            return stringData.ToString();
        }
    }
}