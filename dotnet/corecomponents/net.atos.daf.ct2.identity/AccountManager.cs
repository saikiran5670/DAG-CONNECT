using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.identity.Common;
using net.atos.daf.ct2.identity.entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace net.atos.daf.ct2.identity
{
    public class AccountManager : IAccountManager
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
            String accessToekn = string.Empty;
            Response objResponse = new Response();
            try
            {
                var querystringSAT = new StringContent("client_id=" + _settings.UserMgmClientId + "&client_secret=" + _settings.UserMgmClientSecret + "&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var urlSAT = _settings.BaseUrl + _settings.AuthUrl.Replace("{{realm}}", _settings.Realm);
                // url=url.Replace("{{realm}}",realm);

                var httpResponseSAT = await client.PostAsync(urlSAT, querystringSAT);
                string resultSAT = httpResponseSAT.Content.ReadAsStringAsync().Result;
                if (httpResponseSAT.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    token = JsonConvert.DeserializeObject<IDPToken>(resultSAT);
                    accessToekn = token.access_token;
                    client = new HttpClient();
                    client = PrepareClientHeader(token.access_token);

                    var contentData = new StringContent(GetUserBody(user, "", "INSERT"), System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = client.PostAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm), contentData).Result;
                    if (httpResponse.IsSuccessStatusCode && httpResponse.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        objResponse.StatusCode = httpResponse.StatusCode;
                        objResponse.Result = JsonConvert.SerializeObject("User has been created.");
                    }
                    else
                    {
                        objResponse.StatusCode = httpResponse.StatusCode;
                        objResponse.Result = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
                else
                {
                    objResponse.StatusCode = httpResponseSAT.StatusCode;
                    objResponse.Result = resultSAT;
                }
                return objResponse;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public async Task<Response> UpdateUser(Identity user)
        {
            return await UpdateOrDeleteUser(user, "UPDATE");
        }
        public async Task<Response> DeleteUser(Identity user)
        {
            return await UpdateOrDeleteUser(user, "DELETE");
        }
        public async Task<Response> ChangeUserPassword(Identity user)
        {
            if (!IdentityUtilities.ValidationByRegex(new Regex(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{10,256})"), user.Password))
            {
                return new Response()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Result = JsonConvert.SerializeObject(new IdentityResponse()
                    {
                        Error_Description = "Please provide password of min 10 chars, should contain at least one uppercase, lowercase, number and special character.",
                        Error = "InValidPassword"
                    })
                };
            }
            return await UpdateOrDeleteUser(user, "CHANGEPASSWORD");
        }
        public async Task<Response> ResetUserPasswordInitiate()
        {
            return await Task.FromResult(new Response()
            {
                Result = Guid.NewGuid(),
                StatusCode = HttpStatusCode.OK
            });
        }
        public async Task<Response> LogOut(Identity user)
        {
            return await UpdateOrDeleteUser(user, "LOGOUT");
        }
        internal async Task<Response> UpdateOrDeleteUser(Identity user, string actionType)
        {
            IDPToken token = new IDPToken();
            String accessToekn = string.Empty;
            Response objResponse = new Response();
            try
            {
                var querystringSAT = new StringContent("client_id=" + _settings.UserMgmClientId + "&client_secret=" + _settings.UserMgmClientSecret + "&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var urlSAT = _settings.BaseUrl + _settings.AuthUrl.Replace("{{realm}}", _settings.Realm);

                var httpResponseSAT = await client.PostAsync(urlSAT, querystringSAT);
                string resultSAT = httpResponseSAT.Content.ReadAsStringAsync().Result;
                if (httpResponseSAT.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    token = JsonConvert.DeserializeObject<IDPToken>(resultSAT);
                    HttpResponseMessage httpResponseUser = await GetUserIdByEmail(token.access_token, user);
                    if (httpResponseUser.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        JArray userJSON = JsonConvert.DeserializeObject<JArray>(httpResponseUser.Content.ReadAsStringAsync().Result);
                        if (userJSON != null && userJSON.Count > 0)
                        {
                            string keycloakUserId = string.Empty;
                            foreach (JObject obj in userJSON)
                            {
                                keycloakUserId = obj.GetValue("id").ToString();
                                break;
                            }
                            client = new HttpClient();
                            client = PrepareClientHeader(token.access_token);
                            var contentData = new StringContent(GetUserBody(user, keycloakUserId, actionType), System.Text.Encoding.UTF8, "application/json");
                            HttpResponseMessage httpResponse = new HttpResponseMessage();
                            if (actionType == "UPDATE" || actionType == "DELETE")
                            {
                                httpResponse = client.PutAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm) + "/" + keycloakUserId, contentData).Result;
                            }
                            if (actionType == "CHANGEPASSWORD")
                            {
                                httpResponse = client.PutAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm) + "/" + keycloakUserId + "/" + "reset-password", contentData).Result;
                            }
                            if (actionType == "LOGOUT")
                            {
                                httpResponse = client.PutAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm) + "/" + keycloakUserId + "/" + "logout", contentData).Result;
                            }
                            if (httpResponse.IsSuccessStatusCode && httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                            {
                                objResponse.StatusCode = httpResponse.StatusCode;
                                if (actionType == "UPDATE")
                                {
                                    objResponse.Result = JsonConvert.SerializeObject("User has been updated.");
                                }
                                if (actionType == "DELETE")
                                {
                                    objResponse.Result = JsonConvert.SerializeObject("User has been deleted.");
                                }
                                if (actionType == "CHANGEPASSWORD")
                                {
                                    objResponse.Result = JsonConvert.SerializeObject("User password has been changed.");
                                }
                                if (actionType == "LOGOUT")
                                {
                                    objResponse.Result = JsonConvert.SerializeObject("User has logout from IDP");
                                }
                            }
                            else
                            {
                                objResponse.StatusCode = httpResponse.StatusCode;
                                objResponse.Result = httpResponse.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else
                        {
                            objResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                            objResponse.Result = "{error: User is not available}";
                        }
                    }
                    else
                    {
                        objResponse.StatusCode = httpResponseUser.StatusCode;
                        objResponse.Result = resultSAT;
                    }
                }
                else
                {
                    objResponse.StatusCode = httpResponseSAT.StatusCode;
                    objResponse.Result = resultSAT;
                }
                return objResponse;
            }
            catch (System.Exception)
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
            return client;
        }

        private async Task<HttpResponseMessage> GetUserIdByEmail(string accesstoken, Identity user)
        {
            string strUserId = string.Empty;
            client = new HttpClient();
            client = PrepareClientHeader(accesstoken);
            return await client.GetAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm) + "?username=" + user.UserName);
        }

        private string GetUserBody(Identity user, string keycloakUserid = null, string actiontype = "")
        {
            string stringData = string.Empty;
            switch (actiontype.ToUpper())
            {
                case "INSERT":
                    keycloakCreateUserModel modelCreate = new keycloakCreateUserModel();
                    modelCreate.username = user.UserName;
                    modelCreate.email = user.EmailId;
                    modelCreate.firstName = user.FirstName;
                    modelCreate.lastName = user.LastName;
                    modelCreate.enabled = true;

                    stringData = JsonConvert.SerializeObject(modelCreate, Formatting.Indented);
                    break;
                case "UPDATE":
                    keycloakUpdateUserModel modelUpdate = new keycloakUpdateUserModel();
                    modelUpdate.id = keycloakUserid;
                    // modelCreate.EmailId=user.EmailId;
                    modelUpdate.firstName = user.FirstName;
                    modelUpdate.lastName = user.LastName;
                    modelUpdate.enabled = true;
                    stringData = JsonConvert.SerializeObject(modelUpdate, Formatting.Indented);
                    break;
                case "DELETE":
                    keycloakDeleteUserModel modelDelete = new keycloakDeleteUserModel();
                    modelDelete.id = keycloakUserid;
                    modelDelete.enabled = false;
                    // modelCreate.EmailId=user.EmailId;
                    // modelUpdate.FirstName=user.FirstName;
                    // modelUpdate.LastName=user.LastName;
                    stringData = JsonConvert.SerializeObject(modelDelete, Formatting.Indented);
                    break;
                case "CHANGEPASSWORD":
                    keycloakPwdChangeModel modelUserChangePwd = new keycloakPwdChangeModel();
                    modelUserChangePwd.type = "password";
                    modelUserChangePwd.value = user.Password;
                    modelUserChangePwd.temporary = false;
                    stringData = JsonConvert.SerializeObject(modelUserChangePwd, Formatting.Indented);
                    break;
                case "LOGOUT":
                    stringData = "";
                    break;
            }
            return stringData;
        }
    }
    public class keycloakPwdChangeModel
    {
        public string type { get; set; }
        public string value { get; set; }
        public bool temporary { get; set; }
    }
    public class keycloakCreateUserModel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool enabled { get; set; }
        public List<keycloakPwdChangeModel> credentials { get; set; }
    }
    public class keycloakUpdateUserModel
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool enabled { get; set; }
    }
    public class keycloakDeleteUserModel
    {
        public string id { get; set; }
        public bool enabled { get; set; }
    }
}