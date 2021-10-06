using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Caching.Memory;
using net.atos.daf.ct2.otasoftwareupdate;
using net.atos.daf.ct2.otasoftwareupdate.common;
using net.atos.daf.ct2.otasoftwareupdateservice.Entity;
using net.atos.daf.ct2.visibility;
using static net.atos.daf.ct2.httpclientservice.HttpClientService;

namespace net.atos.daf.ct2.otasoftwareupdateservice.Services
{
    public partial class OTASoftwareUpdateManagementService : OTASoftwareUpdateService.OTASoftwareUpdateServiceBase
    { }

}
