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
        internal reports.entity.ReportUserPreferenceCreateRequest MapCreateReportUserPreferences(DashboardUserPreferenceCreateRequest request)
        {
            reports.entity.ReportUserPreferenceCreateRequest objRequest
                   = new reports.entity.ReportUserPreferenceCreateRequest
                   {
                       Attributes = new List<reports.entity.UserPreferenceAttribute>(),
                       OrganizationId = request.OrganizationId,
                       ReportId = request.ReportId,
                       AccountId = request.AccountId
                   };

            foreach (var attribute in request.Attributes)
            {
                objRequest.Attributes.Add(new reports.entity.UserPreferenceAttribute
                {
                    DataAttributeId = attribute.DataAttributeId,
                    ReportId = attribute.ReportId,
                    State = (reports.entity.ReportUserPreferenceState)(ReportUserPreferenceState)(char)attribute.State,
                    Type = (reports.entity.ReportPreferenceType)(ReportPreferenceType)(char)attribute.Type,
                    ChartType = (reports.entity.ReportPreferenceChartType?)(attribute.ChartType > 0 ? (ReportPreferenceChartType)(char)attribute.ChartType : new ReportPreferenceChartType?()),
                    ThresholdType = (reports.entity.ReportPreferenceThresholdType?)(attribute.ThresholdType > 0 ? (ReportPreferenceThresholdType?)(char)attribute.ThresholdType : new ReportPreferenceThresholdType?()),
                    ThresholdValue = attribute.ThresholdValue,
                });
            }
            return objRequest;
        }


        #region Get user Preference

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
                            ReportId = item.ReportId,
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
        #endregion
    }


}
