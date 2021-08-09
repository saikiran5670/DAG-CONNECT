using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Dashboard
{
    public class DashboardMapper
    {
        /// <summary>
        /// Created User Preference Request for the Dashboard service support.
        /// </summary>
        /// <param name="objUserPreferenceCreateRequest"></param>
        /// <param name="accountId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        internal dashboardservice.DashboardUserPreferenceCreateRequest MapCreateDashboardUserPreference(DashboardUserPreferenceCreateRequest objUserPreferenceCreateRequest,
                                                                                                int accountId, int orgId)
        {
            dashboardservice.DashboardUserPreferenceCreateRequest objRequest = new dashboardservice.DashboardUserPreferenceCreateRequest();

            objRequest.ReportId = objUserPreferenceCreateRequest.ReportId;
            objRequest.AccountId = accountId;
            objRequest.OrganizationId = orgId;

            foreach (var attribute in objUserPreferenceCreateRequest.Attributes)
            {
                objRequest.Attributes.Add(new dashboardservice.DashboardUserPreferenceAttribute()
                {
                    DataAttributeId = attribute.DataAttributeId,
                    State = (int)attribute.State.ToCharArray().First(),
                    Type = (int)attribute.PreferenceType.ToCharArray().First(),
                    ChartType = !string.IsNullOrEmpty(attribute.ChartType) ? (int)attribute.ChartType.ToCharArray().First() : 0,
                    ThresholdType = !string.IsNullOrEmpty(attribute.ThresholdType) ? (int)attribute.ThresholdType.ToCharArray().First() : 0,
                    ThresholdValue = attribute.ThresholdValue
                });
            }
            return objRequest;
        }

    }
}
