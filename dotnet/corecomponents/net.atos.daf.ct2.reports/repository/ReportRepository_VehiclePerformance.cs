using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {

        public async Task<List<EngineLoadDistributionTemplate>> GetEngineLoadDistribution(int enginetypeid)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@enginetypeid", "enginetypeid");
            string queryEngineLoadData = @"";

            List<EngineLoadDistributionTemplate> lstengion = (List<EngineLoadDistributionTemplate>)await _dataAccess.QueryAsync<EngineLoadDistributionTemplate>(queryEngineLoadData, parameter);

            return lstengion;


        }


    }
}
