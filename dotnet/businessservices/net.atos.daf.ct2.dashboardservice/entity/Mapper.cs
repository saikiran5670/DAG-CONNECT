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

        internal DashboardUserPreferenceResponse MapReportUserPreferences(IEnumerable<reports.entity.ReportUserPreference> userPreferences)
        {
            var root = userPreferences.Where(up => up.Name.IndexOf('.') == -1).First();

            var preferences = FillRecursive(userPreferences, new int[] { root.DataAttributeId }).FirstOrDefault();

            return new DashboardUserPreferenceResponse
            {
                TargetProfileId = root.TargetProfileId ?? 0,
                UserPreference = preferences,
                Code = Responsecode.Success
            };
        }

        private static List<DashboardUserPreference> FillRecursive(IEnumerable<reports.entity.ReportUserPreference> flatObjects, int[] parentIds)
        {
            List<DashboardUserPreference> recursiveObjects = new List<DashboardUserPreference>();
            if (parentIds != null)
            {
                foreach (var item in flatObjects.Where(x => parentIds.Contains(x.DataAttributeId)))
                {
                    if (item.ReportAttributeType == reports.entity.ReportAttributeType.Simple ||
                        item.ReportAttributeType == reports.entity.ReportAttributeType.Complex)
                    {
                        var preference = new DashboardUserPreference
                        {
                            DataAttributeId = item.DataAttributeId,
                            Name = item.Name ?? string.Empty,
                            Key = item.Key ?? string.Empty,
                            State = item.State ?? ((char)ReportPreferenceState.InActive).ToString(),
                            ChartType = item.ChartType ?? string.Empty,
                            ThresholdType = item.ThresholdType ?? string.Empty,
                            ThresholdValue = item.ThresholdValue
                        };
                        preference.SubReportUserPreferences.AddRange(FillRecursive(flatObjects, item.SubDataAttributes));
                        recursiveObjects.Add(preference);
                    }
                }
            }
            return recursiveObjects;
        }
    }


}
