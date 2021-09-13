﻿using System;
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

        public async Task<IEnumerable<Ranking>> GetFuelBenchmarkRanking(FuelBenchmarkFilter fuelBenchmarkFilter)
        {
            try
            {
                var param = new DynamicParameters();
                string query = @"Select
                                 trips.vin,
                                 veh.name as VehicleName,
                                 Round((SUM(trips.etl_gps_fuel_consumed)/SUM(trips.etl_gps_distance)),4) as FuelConsumption
                                From
                                tripdetail.trip_statistics as trips
                                Left JOIN master.vehicle as veh
                                ON trips.vin = veh.vin
                                WHERE (end_time_stamp >= @fromDate AND end_time_stamp<= @endDate) 
                                AND trips.VIN=ANY(@vin)
                                GROUP BY
                                trips.vin,veh.name";
                param.Add("@vin", fuelBenchmarkFilter.VINs);
                param.Add("@fromDate", fuelBenchmarkFilter.StartDateTime);
                param.Add("@endDate", fuelBenchmarkFilter.EndDateTime);
                IEnumerable<Ranking> rankingList = await _dataMartdataAccess.QueryAsync<Ranking>(query, param);
                return rankingList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FuelBenchmarkConsumption> GetFuelBenchmarkDetail(FuelBenchmarkFilter fuelBenchmarkFilter)
        {
            try
            {
                var param = new DynamicParameters();
                int totalActiveVehicle = 0;
                totalActiveVehicle = fuelBenchmarkFilter.VINs.Count();

                string query = @"Select
                                @totalActiveVehicle as numbersofactivevehicle                                                  		 
                                , SUM(etl_gps_distance) as totalmileage
                                , Round(SUM(etl_gps_fuel_consumed),2) as totalfuelconsumed
                                , Round((SUM(etl_gps_fuel_consumed)/SUM(etl_gps_distance))/ @totalActiveVehicle,4) as averagefuelconsumption
                                From
                                tripdetail.trip_statistics 
                                WHERE (end_time_stamp >= @fromDate AND end_time_stamp<= @endDate) 
                                AND VIN=ANY(@vin)";
                param.Add("@vin", fuelBenchmarkFilter.VINs);
                param.Add("@fromDate", fuelBenchmarkFilter.StartDateTime);
                param.Add("@endDate", fuelBenchmarkFilter.EndDateTime);
                param.Add("@totalActiveVehicle", totalActiveVehicle);

                FuelBenchmarkConsumption fuelBenchmarkConsumption = new FuelBenchmarkConsumption();
                fuelBenchmarkConsumption = await _dataMartdataAccess.QueryFirstOrDefaultAsync<FuelBenchmarkConsumption>(query, param);
                return fuelBenchmarkConsumption;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
