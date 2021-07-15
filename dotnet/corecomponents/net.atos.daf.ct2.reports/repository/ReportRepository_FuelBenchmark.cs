using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;


namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        public Task<IEnumerable<FuelBenchmark>> GetFuelBenchmarks(FuelBenchmark fuelBenchmarkFilter)
        {
            try
            {
                var query = @"select id as Id,name as Name, key as Key from master.report";
                return _dataAccess.QueryAsync<FuelBenchmark>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Ranking>> GetFuelBenchmarkRanking(FuelBenchmarkConsumptionParameter fuelBenchmarkConsumptionParameter)
        {
            try
            {
                var param = new DynamicParameters();
                string query = @"Select
                                trips.vin,
                                veh.name,
                                Round(SUM(trips.etl_gps_fuel_consumed),2) as totalfuelconsumed
                                From
                                tripdetail.trip_statistics as trips
                                Left JOIN master.vehicle as veh
                                ON trips.vin = veh.vin
                                WHERE (start_time_stamp >= @fromDate AND end_time_stamp<= @endDate) 
                                AND VIN=ANY(@vin)
                                GROUP BY
                                trips.vin,veh.name";
                param.Add("@vin", fuelBenchmarkConsumptionParameter.VINs);
                param.Add("@fromDate", fuelBenchmarkConsumptionParameter.StartDateTime);
                param.Add("@endDate", fuelBenchmarkConsumptionParameter.EndDateTime);
                IEnumerable<Ranking> rankingList = await _dataAccess.QueryAsync<Ranking>(query, param);
                return rankingList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FuelBenchmarkConsumption> GetFuelBenchmarkDetail(FuelBenchmarkConsumptionParameter fuelBenchmarkConsumptionParameter)
        {
            try
            {
                var param = new DynamicParameters();
                string query = @"Select
                                count(VIN) as numbersofactivevehicle                                                  		 
                                , SUM(etl_gps_distance) as totalmileage
                                , Round(SUM(etl_gps_fuel_consumed),2) as totalfuelconsumed
                                , Round(SUM(etl_gps_fuel_consumed)/ count(VIN),2) as averagefuelconsumption
                                From
                                tripdetail.trip_statistics 
                                WHERE (start_time_stamp >= @fromDate AND end_time_stamp<= @endDate) 
                                AND VIN=ANY(@vin)
                                GROUP BY
                                trips.vin,veh.name";
                param.Add("@vin", fuelBenchmarkConsumptionParameter.VINs);
                param.Add("@fromDate", fuelBenchmarkConsumptionParameter.StartDateTime);
                param.Add("@endDate", fuelBenchmarkConsumptionParameter.EndDateTime);
                var fuelConsumptionList = await _dataAccess.QueryAsync<FuelBenchmarkConsumption>(query, param);
                return fuelConsumptionList as FuelBenchmarkConsumption;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
