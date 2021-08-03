using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {

        public async Task<VehiclePerformanceChartTemplate> GetVehPerformanceChartTemplate(VehiclePerformanceRequest vehiclePerformanceRequest)
        {
            var parameter = new DynamicParameters();
            var vehiclePerformanceChartTemplate = new VehiclePerformanceChartTemplate();
            var vehSummary = await GetVehPerformanceSummaryDetails(vehiclePerformanceRequest.Vin);
            vehiclePerformanceChartTemplate.VehiclePerformanceSummary = vehSummary;
            parameter.Add("@enginetype", vehSummary.EngineType);
            string queryEngineLoadData = @"";
            List<EngineLoadType> lstengion = (List<EngineLoadType>)await _dataAccess.QueryAsync<EngineLoadType>(queryEngineLoadData, parameter);
            vehiclePerformanceChartTemplate.VehChartList = lstengion;
            return vehiclePerformanceChartTemplate;
        }

        public async Task<VehiclePerformanceSummary> GetVehPerformanceSummaryDetails(string vin)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vin", vin);
            IEnumerable<VehiclePerformanceSummary> summary;
            string query = @"SELECT vin as Vin,  engine_type as EngineType, model_type as ModelType, name as VehicleName FROM master.vehicle where vin = @vin";
            summary = await _dataMartdataAccess.QueryAsync<VehiclePerformanceSummary>(query, parameter);
            return summary as VehiclePerformanceSummary;

        }
    }
}
