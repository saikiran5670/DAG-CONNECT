using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace net.atos.daf.ct2.vehicleservicerest.Controllers
{
    public class VehicleController : ControllerBase
    {

        [HttpPost]
        [Route("Test")]
        public async Task<IActionResult> Test(string request)
        {
            try
            {   
                return Ok("You Name" + request);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}
