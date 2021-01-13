using System;
// using Grpc.Core;
using System.Threading.Tasks;
// using Newtonsoft.Json;
//using DAF.IdentityService.Protos;
// using DAF.IdentityLogic;
// using DAF.Entity;

namespace DAF.IdentityService
{
    public class AuthenticationService //: AuthService.AuthServiceBase
    {
        // private readonly IAutheticator _autheticator;
        // public AuthenticationService (IAutheticator autheticator)
        // {
        //     _autheticator=autheticator;
        //     // _logger = logger;
        // }   

        // public async override Task<TokenResponse> GenerateToken(UserRequest request, ServerCallContext context)
        // {
        //      var grpcresponse = new TokenResponse();

        //      var user = new User
        //     {
        //         UserName = request.Username,
        //         Password= request.Password
        //     };
        //     try 
        //     {
        //         var response = await _autheticator.AccessToken(user);                
        //         IDPToken token = JsonConvert.DeserializeObject<IDPToken>(Convert.ToString(response.Result));
        //         if(!string.IsNullOrEmpty(token.Error))
        //         {
        //             grpcresponse.Error=token.Error;
        //         }
        //         grpcresponse.Accesstoken=token.access_token;
        //         grpcresponse.Expiresin=token.expires_in;
        //         grpcresponse.Refreshexpiresin=token.refresh_expires_in;
        //         grpcresponse.Refreshtoken=token.refresh_token;
        //         grpcresponse.Tokentype=token.token_type;
        //         grpcresponse.Notbeforepolicy=token.not_before_policy;
        //         grpcresponse.Sessionstate=token.session_state;
        //         grpcresponse.Scope=token.scope;
            
        //     }
        //     catch(Exception ex)
        //     {
        //         if(!string.IsNullOrEmpty(grpcresponse.Error))
        //         {
        //             grpcresponse.Error=grpcresponse.Error +" "+ex.StackTrace;
        //         }
        //         else
        //         {
        //             grpcresponse.Error = ex.StackTrace;
        //         }
        //     }
        //     return grpcresponse;
        // }

    }
}
