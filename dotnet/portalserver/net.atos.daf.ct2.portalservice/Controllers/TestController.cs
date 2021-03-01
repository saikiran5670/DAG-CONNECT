using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.authenticationservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("testpoc")]
    public class TestController: ControllerBase
    {
        public TestController()
        {
        }
        [HttpGet]
        [Route("gettest")]
        public IActionResult Get()
        {
            return Ok("Working....");
        }
    }
}
