using System;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.dashboard;

namespace net.atos.daf.ct2.dashboardservice
{
    public class DashBoardManagementService : DashBoardGRPCService.DashBoardGRPCServiceBase
    {
        private readonly ILog _logger;
        private readonly IDashBoardManager _dashBoardManager;

        public DashBoardManagementService(IDashBoardManager dashBoardManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _dashBoardManager = dashBoardManager;
        }
    }
}
