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
    // [EnableCors("http:/localhost:4200","*","get,post,pu,delete,options")]  
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string  Get()
        {
            KeycloakAuthUtil obj = new KeycloakAuthUtil();
            return "LoginController";
        }    
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostLogin([FromBody] KeycloakInput keycloakInput)
        {
            string str="";
            try
            {
             KeycloakAuthUtil objKeycloakAuthUtil = new KeycloakAuthUtil();
             var objKeycloakRepository = await objKeycloakAuthUtil.AccessToken(keycloakInput);
            if(objKeycloakRepository==null)
                return BadRequest();
            else 
                return Ok(objKeycloakRepository);
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
