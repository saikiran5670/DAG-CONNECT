using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.notificationdataservice.Controllers
{
    [ApiController]
    [Route("notification")]
    //[Authorize(Policy = AccessPolicies.MAIN_NAMELIST_ACCESS_POLICY)]
    public class NotificationController : ControllerBase
    {

    }
}
