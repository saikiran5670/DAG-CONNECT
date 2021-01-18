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
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;

        public RoleController(ILogger<RoleController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string  Get()
        {
            KeycloakAuthUtil obj = new KeycloakAuthUtil();
            return "RoleController";
        }    
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatRole([FromBody] FormInput formInput)
        {
            string str="";
            try
            {
                KeycloakAuthUtil objKeycloakAuthUtil = new KeycloakAuthUtil();
                var objKeycloakResponse = await objKeycloakAuthUtil.CreateRoles(formInput);
                if(objKeycloakResponse==null)
                    return BadRequest();
                else 
                    return Ok(objKeycloakResponse);
            }
            catch(Exception ex)
            {
            //    str=ex.Message;
               str="{\"Error\":\"" + ex.Message + "\"}";
               str="{\"Description\":\"" + ex.StackTrace + "\"}";
            }            
            return Ok(str);
        } 
    }
}
