using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly SessionHelper _sessionHelper;
        protected HeaderObj _userDetails;

        public BaseController(IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
            _userDetails = _sessionHelper.GetSessionInfo(httpContextAccessor.HttpContext.Session);
        }

        protected int GetContextOrgId()
        {
            return _userDetails.ContextOrgId;
        }

        protected int GetUserSelectedOrgId()
        {
            return _userDetails.OrgId;
        }

        protected bool HasAdminPrivilege()
        {
            return _userDetails.RoleLevel == 10 || _userDetails.RoleLevel == 20;
        }

        protected SessionFeature[] GetUserSubscribeFeatures()
        {
            return _userDetails.UserFeatures;
        }

        protected int GetMappedFeatureId(string path)
        {
            Dictionary<string, string> featureMapping = new Dictionary<string, string>()
            {
                { "/report/trip/getparameters", "Report.TripReport" },
                { "/report/fleetfuel/getparameters", "Report.FleetFuelReport" },
                { "/report/fleetutilization/getparameters", "Report.FleetUtilisation" },
                { "/report/fuelbenchmarking/getparameters", "Report.FuelBenchmarking" },
                { "/report/fueldeviation/getparameters", "Report.FuelDeviationReport" },
                { "/report/vehicleperformance/getparameters", "Report.VehiclePerformanceReport" },
                { "/report/drivetime/getparameters", "Report.DriveTimeManagement" },
                { "/report/ecoscore/getparameters", "Report.ECOScoreReport" },
                { "/report/fleetoverview/getlogbookfilters", "FleetOverview.LogBook" },
                { "/report/fleetoverview/getfilterdetails", "FleetOverview" },
                { "/report/fleetoverview/getfleetoverviewdetails", "FleetOverview" },
                { "/dashboard/vins", "Dashboard" },
                { "/report/fuelbenchmark/timeperiod", "Report.FuelBenchmarking" },
                { "/reportscheduler/getreportschedulerparameter", "Configuration.ReportScheduler" },
                { "/report/fleetoverview/getlogbookdetails", "FleetOverview.LogBook" },
                { "/otasoftwareupdate/getvehiclestatuslist", "VehicleUpdates" }
            };
            var featureName = featureMapping.ContainsKey(path) ? featureMapping[path] : string.Empty;

            return GetUserSubscribeFeatures()?.Where(x => x.Name.ToLower().Equals(featureName.ToLower()))
                                            ?.Select(x => x.FeatureId)?.FirstOrDefault() ?? default;
        }
        protected IEnumerable<int> GetMappedFeatureIdByStartWithName(string featureStartWith)
        {
            return GetUserSubscribeFeatures()?.Where(x => x.Name.ToLower().StartsWith(featureStartWith.ToLower()))
                                            ?.Select(x => x.FeatureId)?.ToList();
        }
    }
}
