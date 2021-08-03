using System;
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
            try
            {
                var parameter = new DynamicParameters();
                var vehiclePerformanceChartTemplate = new VehiclePerformanceChartTemplate();
                var vehSummary = await GetVehPerformanceSummaryDetails(vehiclePerformanceRequest.Vin);
                vehiclePerformanceChartTemplate.VehiclePerformanceSummary = vehSummary;
                parameter.Add("@enginetype", vehSummary.EngineType);
                parameter.Add("@performancetype", vehiclePerformanceRequest.PerformanceType);
                string queryEngineLoadData = @"
	                Select engine_type as Enginetype,is_default as IsDefault, index,range,array_to_string(row, ',','*') as Axisvalues
	                from master.vehicleperformancetemplate pt
	                join master.performancematrix pm
	                on pt.template=pm.template
	                where vehicle_performance_type= @performancetype
	                and engine_type = @enginetype";
                List<EngineLoadType> lstengion = (List<EngineLoadType>)await _dataAccess.QueryAsync<EngineLoadType>(queryEngineLoadData, parameter);
                vehiclePerformanceChartTemplate.VehChartList = lstengion;
                return vehiclePerformanceChartTemplate;
            }
            catch (Exception Ex)
            {

                throw;
            }

        }

        public async Task<VehiclePerformanceSummary> GetVehPerformanceSummaryDetails(string vin)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vin", vin);
                string query = @"SELECT vin as Vin,  engine_type as EngineType, model_type as ModelType, name as VehicleName FROM master.vehicle where vin = @vin";
                var summary = await _dataMartdataAccess.QueryFirstOrDefaultAsync<VehiclePerformanceSummary>(query, parameter);
                return (VehiclePerformanceSummary)summary;
            }
            catch (Exception)
            {
                throw;
            }


        }
    }
}
