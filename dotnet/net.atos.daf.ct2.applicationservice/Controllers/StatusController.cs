using Microsoft.AspNetCore.Mvc;

namespace net.atos.daf.ct2.applicationservice.Controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        [Route("notfound")]
        public IActionResult Status()
        {
            NotFound notFound = new NotFound();
            notFound.Code = "404";
            notFound.Message = "Not Found";
            return NotFound(notFound);
        }
    }
    public class NotFound
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
