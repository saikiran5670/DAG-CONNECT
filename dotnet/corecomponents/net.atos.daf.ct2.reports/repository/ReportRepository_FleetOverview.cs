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
        public async Task<List<AlertLevel>> GetAlertLevelList()
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "U");
            string queryAlertLevelPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type";

            List<AlertLevel> lstAlertLevel = (List<AlertLevel>)await _dataAccess.QueryAsync<AlertLevel>(queryAlertLevelPull, parameter);
            if (lstAlertLevel.Count > 0)
            {
                return lstAlertLevel;
            }
            else
            {
                return new List<AlertLevel>();
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

        #endregion

    }
}
