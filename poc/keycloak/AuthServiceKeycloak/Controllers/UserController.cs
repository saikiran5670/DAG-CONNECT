using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;

// using System.Web.Http.Cors;

namespace AuthServiceKeycloak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [EnableCors("AllowOrigin","*","get,post,pu,delete,options")]  
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string  Get()
        {
            KeycloakAuthUtil obj = new KeycloakAuthUtil();
            
            return "UserController";
        }    
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] UserModel userModel)  
        {
            string str="";
            try
            {
                KeycloakAuthUtil objKeycloakAuthUtil = new KeycloakAuthUtil();
                var objKeycloakResponse = await objKeycloakAuthUtil.CreateUsers(userModel);
                if(objKeycloakResponse==null)
                    return BadRequest();
                else 
                    return Ok(objKeycloakResponse);
            }
            catch(Exception ex)
            {
               str=ex.Message;
               str="{\"Error\":\"" + str + "\"}";
            }            
            return Ok(str);
        } 
    }
}
