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

        public async Task<IEnumerable<VehiclePerformanceRequest>> GetVehPerformanceSummaryDetails(string vin)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vin", vin);
            IEnumerable<VehiclePerformanceRequest> summary;
            string query = @"SELECT vin,
                                    vid, engine_type, model_type, name FROM master.vehicle";
            summary = await _dataMartdataAccess.QueryAsync<VehiclePerformanceRequest>(query, parameter);
            return summary.AsList<VehiclePerformanceRequest>();


        }
    }
}
