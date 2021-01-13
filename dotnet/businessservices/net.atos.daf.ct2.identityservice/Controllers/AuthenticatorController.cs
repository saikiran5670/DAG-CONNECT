using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
// using DAF.IdentityLogic;
// using DAF.Entity;

namespace DAF.IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticatorController : ControllerBase
    {
    //     private readonly IAutheticator _autheticator;
    //     private readonly IOptions<GlobalConfiguration> _globalConfiguration;
    //     public AuthenticatorController(IAutheticator autheticator,IOptions<GlobalConfiguration> globalConfiguration)
    //     {
    //         _autheticator=autheticator;
    //         _globalConfiguration=globalConfiguration;
    //     }

    //     /// <summary>
    //     ///  This action will used to obtian the access token for a user
    //     /// </summary>
    //     /// <param name="user"> User model that will have username and password as an input</param>
    //     /// <returns>Authentication token as a JSON, error message if any</returns>
    //     [HttpPost]        
    //     [Route("Token")]
    //     public async Task<IActionResult> GenerateToken([FromBody] User user)
    //     {
    //         string result = string.Empty;
    //         try 
    //         {
    //                 if(string.IsNullOrEmpty(user.UserName))
    //                 {
    //                     return StatusCode(401,"invalid_grant: The username is Empty.");
    //                 }
    //                 else if(string.IsNullOrEmpty(user.Password))
    //                 {
    //                     return StatusCode(401,"invalid_grant: The password is Empty.");
    //                 }
    //                 else
    //                 {
    //                     Response response = await _autheticator.AccessToken(user);
    //                     if(response!=null && response.Result==null)
    //                         return Ok(result);
    //                     else 
    //                         return StatusCode((int) response.StatusCode,response.Result);

    //                 }
    //         }
    //         catch(Exception ex)
    //         {
    //                 var p = ex.Message;
    //                 return StatusCode(500,"Internal Server Error.");
    //         }            
    //     }
        
    //     [HttpGet]
    //     public string Get()
    //     {
    //         User user =new User();
    //         user.UserName="testuser5@atos.net";
    //         user.Password="123456";
    //         return _globalConfiguration.Value.BaseUrlKeycloak;
    //         // return _autheticator.getURL(user);;
    //     }

    }
}
