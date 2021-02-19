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
    public class DemoController: ControllerBase
    {
        private readonly Greeter.GreeterClient _greeterClient;
        public DemoController(Greeter.GreeterClient greeterClient)
        {
            _greeterClient=greeterClient;
        }
        [HttpGet]
        [Route("getgRPC")]
        public IActionResult Get()
        {
            var result= _greeterClient.SayHello(new HelloRequest(){Name="James"} );           
            return Ok(result);
        }
    }
}
