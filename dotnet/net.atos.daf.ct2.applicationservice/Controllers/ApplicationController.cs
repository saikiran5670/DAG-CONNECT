using Microsoft.AspNetCore.Mvc;

namespace net.atos.daf.ct2.applicationservice.Controllers
{
    [ApiController]
    //[Route("application")]
    public class ApplicationController : ControllerBase
    {
        [HttpGet]
        [Route("~/")]
        public IActionResult Index()
        {
            return StatusCode(401, "");
        }
        [HttpGet]
        [Route("~/health-check")]
        public IActionResult CheckHealth()
        {
            return StatusCode(200, "");
        }
    }
}
