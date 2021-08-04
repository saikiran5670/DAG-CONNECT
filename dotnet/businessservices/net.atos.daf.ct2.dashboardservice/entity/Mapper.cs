using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.dashboard.entity;
using net.atos.daf.ct2.reports.ENUM;

namespace net.atos.daf.ct2.dashboardservice.entity
{
    public class Mapper
    {
        internal dashboard.entity.DashboardUserPreferenceCreateRequest MapCreateDashboardUserPreferences(DashboardUserPreferenceCreateRequest request)
        {

            dashboard.entity.DashboardUserPreferenceCreateRequest objRequest
                   = new dashboard.entity.DashboardUserPreferenceCreateRequest
                   {
                       Attributes = new List<dashboard.entity.DashboardUserPreferenceAttribute>(),
                       OrganizationId = request.OrganizationId,
                       ReportId = request.ReportId,
                       AccountId = request.AccountId
                   };

            foreach (var attribute in request.Attributes)
            {
                objRequest.Attributes.Add(new dashboard.entity.DashboardUserPreferenceAttribute
                {
                    DataAttributeId = attribute.DataAttributeId,
                    State = (dashboard.entity.ReportUserPreferenceState)(char)attribute.State,
                    Type = (dashboard.entity.ReportPreferenceType)(char)attribute.Type,
                    ChartType = attribute.ChartType > 0 ? (dashboard.entity.ReportPreferenceChartType)(char)attribute.ChartType : new dashboard.entity.ReportPreferenceChartType?(),
                    ThresholdType = attribute.ThresholdType > 0 ? (dashboard.entity.ReportPreferenceThresholdType?)(char)attribute.ThresholdType : new dashboard.entity.ReportPreferenceThresholdType?(),
                    ThresholdValue = attribute.ThresholdValue,
                });
            }
            return objRequest;

        }
    }


}
