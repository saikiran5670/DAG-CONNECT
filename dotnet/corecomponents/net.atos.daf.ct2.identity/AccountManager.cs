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
using Newtonsoft.Json.Serialization;

namespace net.atos.daf.ct2.identity
{
    public class AccountManager : IAccountManager
    {
        private HttpClient _client = new HttpClient();
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
            Response objResponse = new Response();
            try
            {
                var querystringSAT = new StringContent("client_id=" + _settings.UserMgmClientId + "&client_secret=" + _settings.UserMgmClientSecret + "&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var urlSAT = _settings.BaseUrl + _settings.AuthUrl.Replace("{{realm}}", _settings.Realm);
                // url=url.Replace("{{realm}}",realm);

                var httpResponseSAT = await _client.PostAsync(urlSAT, querystringSAT);
                string resultSAT = httpResponseSAT.Content.ReadAsStringAsync().Result;
                if (httpResponseSAT.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IDPToken token = JsonConvert.DeserializeObject<IDPToken>(resultSAT);
                    string accessToekn = token.Access_token;
                    _client = new HttpClient();
                    _client = PrepareClientHeader(token.Access_token);

                    var contentData = new StringContent(GetUserBody(user, "", "INSERT"), System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = _client.PostAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm), contentData).Result;
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
            if (!IdentityUtilities.ValidationByRegex(new Regex(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[""''!*@#$%^&+=~`^()\\/-_;:<>|{}\[\]]).{10,256})"), user.Password))
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
            Response objResponse = new Response();
            try
            {
                var querystringSAT = new StringContent("client_id=" + _settings.UserMgmClientId + "&client_secret=" + _settings.UserMgmClientSecret + "&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var urlSAT = _settings.BaseUrl + _settings.AuthUrl.Replace("{{realm}}", _settings.Realm);

                var httpResponseSAT = await _client.PostAsync(urlSAT, querystringSAT);
                string resultSAT = httpResponseSAT.Content.ReadAsStringAsync().Result;
                if (httpResponseSAT.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IDPToken token = JsonConvert.DeserializeObject<IDPToken>(resultSAT);
                    HttpResponseMessage httpResponseUser = await GetUserIdByEmail(token.Access_token, user);
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
                            _client = new HttpClient();
                            _client = PrepareClientHeader(token.Access_token);
                            var contentData = new StringContent(GetUserBody(user, keycloakUserId, actionType), System.Text.Encoding.UTF8, "application/json");
                            HttpResponseMessage httpResponse = new HttpResponseMessage();
                            if (actionType == "UPDATE" || actionType == "DELETE")
                            {
                                httpResponse = _client.PutAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm) + "/" + keycloakUserId, contentData).Result;
                            }
                            if (actionType == "CHANGEPASSWORD")
                            {
                                httpResponse = _client.PutAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm) + "/" + keycloakUserId + "/" + "reset-password", contentData).Result;
                            }
                            if (actionType == "LOGOUT")
                            {
                                httpResponse = _client.PutAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm) + "/" + keycloakUserId + "/" + "logout", contentData).Result;
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
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_settings.BaseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
            return _client;
        }

        private async Task<HttpResponseMessage> GetUserIdByEmail(string accesstoken, Identity user)
        {
            _client = new HttpClient();
            _client = PrepareClientHeader(accesstoken);
            return await _client.GetAsync(_settings.UserMgmUrl.Replace("{{realm}}", _settings.Realm) + "?username=" + user.UserName);
        }

        private string GetUserBody(Identity user, string keycloakUserid = null, string actiontype = "")
        {
            string stringData = string.Empty;
            switch (actiontype.ToUpper())
            {
                case "INSERT":
                    KeycloakCreateUserModel modelCreate = new KeycloakCreateUserModel();
                    modelCreate.Username = user.UserName;
                    modelCreate.Email = user.EmailId;
                    modelCreate.FirstName = user.FirstName;
                    modelCreate.LastName = user.LastName;
                    modelCreate.Enabled = true;

                    stringData = JsonConvert.SerializeObject(modelCreate, Formatting.Indented,
                        new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    break;
                case "UPDATE":
                    KeycloakUpdateUserModel modelUpdate = new KeycloakUpdateUserModel();
                    modelUpdate.Id = keycloakUserid;
                    // modelCreate.EmailId=user.EmailId;
                    modelUpdate.FirstName = user.FirstName;
                    modelUpdate.LastName = user.LastName;
                    modelUpdate.Enabled = true;
                    stringData = JsonConvert.SerializeObject(modelUpdate, Formatting.Indented,
                        new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    break;
                case "DELETE":
                    KeycloakDeleteUserModel modelDelete = new KeycloakDeleteUserModel();
                    modelDelete.Id = keycloakUserid;
                    modelDelete.Enabled = false;
                    // modelCreate.EmailId=user.EmailId;
                    // modelUpdate.FirstName=user.FirstName;
                    // modelUpdate.LastName=user.LastName;
                    stringData = JsonConvert.SerializeObject(modelDelete, Formatting.Indented,
                        new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    break;
                case "CHANGEPASSWORD":
                    KeycloakPwdChangeModel modelUserChangePwd = new KeycloakPwdChangeModel();
                    modelUserChangePwd.Type = "password";
                    modelUserChangePwd.Value = user.Password;
                    modelUserChangePwd.Temporary = false;
                    stringData = JsonConvert.SerializeObject(modelUserChangePwd, Formatting.Indented,
                        new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    break;
                case "LOGOUT":
                    stringData = "";
                    break;
            }
            return stringData;
        }
    }
    public class KeycloakPwdChangeModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Temporary { get; set; }
    }
    public class KeycloakCreateUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public List<KeycloakPwdChangeModel> Credentials { get; set; }
    }
    public class KeycloakUpdateUserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
    }
    public class KeycloakDeleteUserModel
    {
        public string Id { get; set; }
        public bool Enabled { get; set; }
    }
}