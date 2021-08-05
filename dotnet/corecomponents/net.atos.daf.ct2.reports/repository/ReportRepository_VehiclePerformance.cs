using System;
using System.Collections.Generic;
using System.Linq;
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
                var lstengion = await _dataAccess.QueryAsync<VehicleChartData>(queryEngineLoadData, parameter);
                vehiclePerformanceChartTemplate.VehChartList = lstengion.ToList();


                return vehiclePerformanceChartTemplate;
            }
            catch (Exception Ex)
            {

                throw;
            }

        }


        public async Task CalculateKPIData(List<VehPerformanceChartData> vehicleChartDatas, string performanceType)
        {
            List<KpiDataRange> rangeData = await GetRangeData(performanceType);
            long multiTripDuration = 0;
            foreach (var trip in vehicleChartDatas)
            {

                if (trip.MatrixValue != null)
                {
                    multiTripDuration += trip.TripDuration;
                    var matrixValues = trip.MatrixValue.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                    foreach (var value in matrixValues)
                    {
                        foreach (var range in rangeData)
                        {
                            switch (range.Kpi)
                            {
                                case "O":
                                    if (value >= range.LowerVal && value <= range.UpperVal)
                                        range.Value += value;
                                    break;
                                case "A":
                                    if (value >= range.LowerVal && value <= range.UpperVal)
                                        range.Value += value;
                                    break;
                                case "P":
                                    if (value >= range.LowerVal && value <= range.UpperVal)
                                        range.Value += value;
                                    break;
                                case "E":
                                    if (value >= range.LowerVal && value <= range.UpperVal)
                                        range.Value += value;
                                    break;
                                case "N":
                                    if (value >= range.LowerVal && value <= range.UpperVal)
                                        range.Value += value;
                                    break;
                                case "I":
                                    if (value >= range.LowerVal && value <= range.UpperVal)
                                        range.Value += value;
                                    break;
                                case "D":
                                    if (value >= range.LowerVal && value <= range.UpperVal)
                                        range.Value += value;
                                    break;
                                case "U":
                                    if (value >= range.LowerVal && value <= range.UpperVal)
                                        range.Value += value;
                                    break;

                                default:
                                    break;
                            }

                        }

                    }

                    List<KPIs> lstKpis = new List<KPIs>();
                    foreach (var kpiDict in rangeData)
                    {
                        KPIs kPIs = new KPIs();
                        kPIs.Label = kpiDict.Kpi;
                        kPIs.Value = kpiDict.Value / Convert.ToInt32(multiTripDuration);
                        lstKpis.Add(kPIs);
                    }
                    trip.ListKPIs = lstKpis;
                }

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
        public async Task<List<VehPerformanceChartData>> GetVehPerformanceBubbleChartData(VehiclePerformanceRequest vehiclePerformanceRequest)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vin", vehiclePerformanceRequest.Vin);
                parameter.Add("@performancetype", vehiclePerformanceRequest.PerformanceType);
                parameter.Add("@StartDateTime", vehiclePerformanceRequest.StartTime);
                parameter.Add("@EndDateTime", vehiclePerformanceRequest.EndTime);
                //var vehPerformanceChartData = new List<VehPerformanceChartData>();
                string query = GetQueryAsPerPerformanceType(vehiclePerformanceRequest);
                //var lstengion = (List<VehPerformanceChartData>)await _dataMartdataAccess.QueryAsync<VehPerformanceChartData>(query, parameter);
                var lstengion = await _dataMartdataAccess.QueryAsync<VehPerformanceChartData>(query, parameter);
                await CalculateKPIData(lstengion.ToList(), vehiclePerformanceRequest.PerformanceType);
                return lstengion.ToList();
            }
            catch (Exception Ex)
            {

                throw;
            }
        }
        private async Task<List<KpiDataRange>> GetRangeData(string pType)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@performancetype", pType);
                var query = @"SELECT  vehicle_performance_type as PerformanceType, index as Index, kpi as Kpi, lower_val as LowerVal, 
                            upper_val as UpperVal FROM master.performancekpiranges where vehicle_performance_type=@performancetype;";
                var result = await _dataAccess.QueryAsync<KpiDataRange>(query, parameter);
                return result.ToList();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private string GetQueryAsPerPerformanceType(VehiclePerformanceRequest vehiclePerformanceRequest)
        {
            var query = string.Empty;
            switch (vehiclePerformanceRequest.PerformanceType)
            {

                case "E":
                    query = @"SELECT trip_id as TripId, vin as Vin, 
                                abs_rpm_torque as AbsRpmtTrque, ord_rpm_torque as OrdRpmTorque,
                                array_to_string(nonzero_matrix_val_rpm_torque, ',', '*') as MatrixValue,
                                array_to_string(num_val_rpm_torque, ',', '*') as CountPerIndex,
                                array_to_string(col_index_rpm_torque, ',', '*') as ColumnIndex,
                                etl_gps_driving_time as TripDuration
                                FROM tripdetail.trip_statistics 
                                where vin = @vin and
                               is_ongoing_trip = false AND end_time_stamp >= @StartDateTime  and end_time_stamp<= @EndDateTime";
                    break;
                case "S":
                    query = @"SELECT id as Id, trip_id as TripId, vin as Vin, 
                               
                                abs_speed_rpm as AbsRpmtTrque,
                                ord_speed_rpm as OrdRpmTorque,
                                array_to_string(nonzero_matrix_val_speed_rpm, ',', '*') as MatrixValue,
                                array_to_string(num_val_speed_rpm, ',', '*') as CountPerIndex, 
                                array_to_string(col_index_speed_rpm, ',', '*') as ColumnIndex,
                                etl_gps_driving_time as TripDuration
                                FROM tripdetail.trip_statistics 
                                where vin = @vin and
                               is_ongoing_trip = false AND end_time_stamp >= @StartDateTime  and end_time_stamp<= @EndDateTime";
                    break;
                case "B":
                    query = @"SELECT id as Id, trip_id as TripId, vin as Vin, 
                                    abs_acceleration_speed as AbsRpmtTrque,
                                    ord_acceleration_speed as OrdRpmTorque, 
                                    array_to_string(nonzero_matrix_val_acceleration_speed, ',', '*') as MatrixValue,
                                    array_to_string(nonzero_matrix_val_brake_pedal_acceleration_speed, ',', '*') as Breakacc, 
                                    array_to_string(num_val_acceleration_speed, ',', '*') as CountPerIndex, 
                                    array_to_string(col_index_acceleration_speed, ',', '*') as ColumnIndex,
                                    etl_gps_driving_time as TripDuration
                                    FROM tripdetail.trip_statistics 
                                    where vin = @vin and
                                    is_ongoing_trip = false AND end_time_stamp >= @StartDateTime  and end_time_stamp<= @EndDateTime";
                    break;
                default:
                    break;
            }
            return query;
        }
        public async Task<List<VehPerformanceProperty>> GetVehPerformanceType()
        {
            string query = @"SELECT key as Name,
                                            enum as Value, Type as Type
                                            FROM translation.enumtranslation
                                            Where type in ('Y','N')";

            List<VehPerformanceProperty> response = (List<VehPerformanceProperty>)await _dataAccess.QueryAsync<VehPerformanceProperty>(query);
            if (response.Count > 0)
            {
                return response;
            }
            else
            {
                return new List<VehPerformanceProperty>();
            }
        }
    }
}
