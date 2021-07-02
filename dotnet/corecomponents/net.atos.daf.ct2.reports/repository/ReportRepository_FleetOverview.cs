using System;
using System.Collections.Generic;
using System.Text;
using net.atos.daf.ct2.reports.entity;
using System.Threading.Tasks;
using Dapper;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        #region Fleet Overview Filter
        public async Task<List<FilterProperty>> GetAlertLevelList()
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "U");
            string queryAlertLevelPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type";

            List<FilterProperty> lstAlertLevel = (List<FilterProperty>)await _dataAccess.QueryAsync<FilterProperty>(queryAlertLevelPull, parameter);
            if (lstAlertLevel.Count > 0)
            {
                return lstAlertLevel;
            }
            else
            {
                return new List<FilterProperty>();
            }
        }

        public async Task<List<AlertCategory>> GetAlertCategoryList()
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "C");
            string queryAlertCategoryPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type";

            List<AlertCategory> lstAlertCat = (List<AlertCategory>)await _dataAccess.QueryAsync<AlertCategory>(queryAlertCategoryPull, parameter);
            if (lstAlertCat.Count > 0)
            {
                return lstAlertCat;
            }
            else
            {
                return new List<AlertCategory>();
            }
        }

        public async Task<List<FilterProperty>> GetHealthStatusList()
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "H");
            string queryHealthStatusPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type";

            List<FilterProperty> lstHealthStatus = (List<FilterProperty>)await _dataAccess.QueryAsync<FilterProperty>(queryHealthStatusPull, parameter);
            if (lstHealthStatus.Count > 0)
            {
                return lstHealthStatus;
            }
            else
            {
                return new List<FilterProperty>();
            }
        }

        public async Task<List<FilterProperty>> GetOtherFilter()
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "L");
            string queryOtherFilterPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type";

            List<FilterProperty> lstOtherFilter = (List<FilterProperty>)await _dataAccess.QueryAsync<FilterProperty>(queryOtherFilterPull, parameter);
            if (lstOtherFilter.Count > 0)
            {
                return lstOtherFilter;
            }
            else
            {
                return new List<FilterProperty>();
            }
        }
        #endregion

        public async Task<List<FleetOverviewDetails>> GetFleetOverviewDetail(FleetRequestFilter request)
        {
            List<FleetOverviewDetails> fleetOverviewDetails = new List<FleetOverviewDetails>();

            return fleetOverviewDetails;

        }

    }
}
