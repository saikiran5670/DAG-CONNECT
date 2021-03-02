using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.applicationservice.Controllers
{
    [ApiController]
   // [Route("application")]
    public class ApplicationController : ControllerBase
    {
        [HttpGet]
     //   [Route("get")]
        public IActionResult Get()
        {
            string result = "Get Method Response";
            return Ok(result);
        }
    }
}
