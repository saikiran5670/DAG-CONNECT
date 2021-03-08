using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime; 
using Microsoft.Extensions.Configuration;

namespace AuthServiceKeycloak
{
    public class KeycloakAuthUtil
    {
        //  private static readonly HttpClient client = new HttpClient();
         private HttpClient client = new HttpClient();
         private readonly IConfiguration  _config;
        public KeycloakAuthUtil()
        {
            _config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
        }
        public async Task<KeycloakRepository> AccessToken(KeycloakInput keycloakInput)
        {
            var data = new StringContent("username="+keycloakInput.username+"&password="+keycloakInput.password+"&grant_type=password&client_id=admin-cli", Encoding.UTF8, "application/x-www-form-urlencoded");
            // var url = "http://localhost:8080/auth/realms/master/protocol/openid-connect/token";
            // var url = "http://localhost:8080/auth/realms/DAFConnect/protocol/openid-connect/token";
            var url = _config.GetSection("KeycloakStrings:baseUrl").Value + _config.GetSection("KeycloakStrings:authUrl").Value;

            var response = await client.PostAsync(url, data);
            string result = response.Content.ReadAsStringAsync().Result;
            KeycloakRepository keycloakRepository = JsonConvert.DeserializeObject<KeycloakRepository>(result);
            return keycloakRepository;
        }
        
         public async Task<KeycloakResponse> CreateUsers(UserModel userModel)
        {
            KeycloakResponse keycloakResponse=new KeycloakResponse() ;
            try
            {
                var accessToken=string.Empty;
                // var urlSAT = "http://localhost:8080/auth/realms/demo/protocol/openid-connect/token";
                // var urlSAT = "http://localhost:8080/auth/realms/DAFConnect/protocol/openid-connect/token";
                var urlSAT = _config.GetSection("KeycloakStrings:baseUrl").Value + _config.GetSection("KeycloakStrings:authUrl").Value;
                // var dataSAT = new StringContent("client_id=demoapp&client_secret=2be57774-00b4-426d-b3d8-da0e30ee107b&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var dataSAT = new StringContent("client_id="+_config.GetSection("KeycloakStrings:clientId").Value+"&client_secret="+_config.GetSection("KeycloakStrings:clientSecret").Value+"&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var responseSAT = await client.PostAsync(urlSAT, dataSAT);
                string resultSAT = responseSAT.Content.ReadAsStringAsync().Result;
                KeycloakRepository keycloakRepositorySAT = JsonConvert.DeserializeObject<KeycloakRepository>(resultSAT);
                accessToken=keycloakRepositorySAT.access_token;
                
                string baseUrl = _config.GetSection("KeycloakStrings:baseUrl").Value;
                client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);

                client.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",
                                        keycloakRepositorySAT.access_token);

                // string stringData =  
                // "{"  
                //         + "\"username\": \"" + formInput.name + "\","  
                //         + "\"enabled\": \"" + formInput.enabled + "\""  
                // + "}" ; 

                string stringData =  
                "{"  
                        + "\"username\": \"" + userModel.username + "\","  
                        + "\"firstName\": \"" + userModel.firstname + "\","  
                        + "\"lastName\": \"" + userModel.lastname + "\","  
                        + "\"email\": \"" + userModel.email + "\","  
                        // + "\"realmRoles\": \"" + userModel.userrole + "\","  
                        + "\"enabled\": \"true\","  
                        + "\"credentials\": [" 
                        + "{"
                        + "\"type\": \"password\","  
                        + "\"value\": \"" + userModel.password + "\""  
                        + "}"
                        + "]"
                        // + "\"attributes\": [" 
                        // + "{"
                        // + "\"dafroles\": \"" + userModel.dafroles + "\""  
                        // + "}"
                        // + "]"
                + "}" ; 
                var contentData = new StringContent(stringData,System.Text.Encoding.UTF8, "application/json");

                //Demo -> Demoapp = 8855d0e5-197d-4cb8-867a-c147964efea6
                //DAFConnect-> DAF-Admin =9403585d-4d94-48ec-88b3-adcaaf597a1f
     
                HttpResponseMessage response = client.PostAsync(_config.GetSection("KeycloakStrings:userMgm").Value ,contentData).Result;
                if(response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    keycloakResponse.Message="User has created";
                    keycloakResponse.Error="";
                }
                else
                {
                    string stringJWT = response.Content.ReadAsStringAsync().Result;
                    
                    keycloakResponse.Error=response.StatusCode.ToString();
                    keycloakResponse.Message=stringJWT;
                }
            }
            catch(Exception ex)
            {
                keycloakResponse.Error=ex.Message;
                keycloakResponse.Description=ex.StackTrace;
            }
            return keycloakResponse;
        }
         public async Task<KeycloakResponse> CreateRoles(FormInput formInput)
        {
            KeycloakResponse keycloakResponse=new KeycloakResponse() ;
            try
            {
                var accessToken=string.Empty;
               // var urlSAT = "http://localhost:8080/auth/realms/demo/protocol/openid-connect/token";
                var urlSAT = "http://localhost:8080/auth/realms/DAFConnect/protocol/openid-connect/token";
                // var dataSAT = new StringContent("client_id=demoapp&client_secret=2be57774-00b4-426d-b3d8-da0e30ee107b&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var dataSAT = new StringContent("client_id=DAF-Admin&client_secret=9b057b80-4b58-4156-b10f-c124ec5cdf09&grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var responseSAT = await client.PostAsync(urlSAT, dataSAT);
                string resultSAT = responseSAT.Content.ReadAsStringAsync().Result;
                KeycloakRepository keycloakRepositorySAT = JsonConvert.DeserializeObject<KeycloakRepository>(resultSAT);
                accessToken=keycloakRepositorySAT.access_token;
                
                string baseUrl = "http://localhost:8080";
                client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                client.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",
                                        keycloakRepositorySAT.access_token);

                string stringData =  
                "{"  
                        + "\"name\": \"" + formInput.name + "\","  
                        + "\"composite\": \"" + formInput.enabled + "\""  
                + "}" ; 
                var contentData = new StringContent(stringData,System.Text.Encoding.UTF8, "application/json");
                //admin/realms/{{realm}}/clients/{{clientId}}/roles
                //Demo -> Demoapp = 8855d0e5-197d-4cb8-867a-c147964efea6
                //DAFConnect-> DAF-Admin =9403585d-4d94-48ec-88b3-adcaaf597a1f
                HttpResponseMessage response = client.PostAsync("/auth/admin/realms/DAFConnect/clients/9403585d-4d94-48ec-88b3-adcaaf597a1f/roles",contentData).Result;
                if(response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    keycloakResponse.Message="Role has created";
                    keycloakResponse.Error="";
                }
                else
                {
                    string stringJWT = response.Content.ReadAsStringAsync().Result;
                    keycloakResponse.Error=response.StatusCode.ToString();
                    keycloakResponse.Message=stringJWT;
                }
            }
            catch(Exception ex)
            {
                keycloakResponse.Error=ex.Message;
                keycloakResponse.Description=ex.StackTrace;
            }
            return keycloakResponse;
        }
        
    }

}