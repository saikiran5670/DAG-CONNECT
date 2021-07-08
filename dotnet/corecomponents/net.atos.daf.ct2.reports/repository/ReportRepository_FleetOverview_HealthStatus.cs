using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;
using System.Linq;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        // public async Task<VehicleHealthStatus> GetVehicleHealthStatus(VehicleHealthStatusRequest vehicleHealthStatusRequest)
        // {
        //var parameter = new DynamicParameters();
        //var vehicleHealthStatus = new VehicleHealthStatus();
        //vehicleHealthStatus.VehicleSummary = await GetVehicleHealthSummary(vehicleHealthStatusRequest.VIN);
        //if (vehicleHealthStatusRequest.FromDate == null && vehicleHealthStatusRequest.ToDate == null)
        //{
        //    vehicleHealthStatus.CurrentWarning = await GetCurrentWarnning(vehicleHealthStatusRequest.VIN);
        //    GetPreviousQuarterTime(vehicleHealthStatusRequest);
        //}
        //vehicleHealthStatus.VehicleSummary.FromDate = vehicleHealthStatusRequest?.FromDate;
        //vehicleHealthStatus.VehicleSummary.ToDate = vehicleHealthStatusRequest?.ToDate;
        //vehicleHealthStatus.VehicleSummary.WarningType = vehicleHealthStatusRequest.WarningType ?? "All";
        //vehicleHealthStatus.HistoryWarning = await GetHistoryWarning(vehicleHealthStatusRequest);
        //return vehicleHealthStatus;
        //  }





        public async Task<List<VehicleHealthResult>> GetVehicleHealthStatus(VehicleHealthStatusRequest vehicleHealthStatusRequest)
        {

            var parameter = new DynamicParameters();
            parameter.Add("@vin", vehicleHealthStatusRequest.VIN);
            parameter.Add("@tripId", vehicleHealthStatusRequest.TripId);
            parameter.Add("@days", vehicleHealthStatusRequest.Days, System.Data.DbType.Int32);


            var query = @"With HealthSummary AS ( select
                         v.registration_no as VehicleRegNo
                        ,v.name as VehicleName
                        ,cts.id as Lcts_Id
                        ,cts.trip_id as Lcts_TripId
                        ,cts.vin as Lcts_Vin
                        ,cts.start_time_stamp as Lcts_TripStartTime
                        ,cts.end_time_stamp as Lcts_TripEndTime
                        ,cts.driver1_id as Lcts_Driver1Id
                        ,cts.trip_distance as Lcts_TripDistance
                        ,cts.driving_time as Lcts_DrivingTime
                        ,cts.fuel_consumption as Lcts_FuelConsumption
                        , cts.vehicle_driving_status_type as Lcts_VehicleDrivingStatus_type
                        ,cts.odometer_val as Lcts_OdometerVal
                        ,cts.distance_until_next_service as Lcts_DistanceUntilNextService
                        ,cts.latest_received_position_lattitude as Lcts_LatestReceivedPositionLattitude
                        ,cts.latest_received_position_longitude as Lcts_LatestReceivedPositionLongitude
                        ,cts.latest_received_position_heading as Lcts_LatestReceivedPositionHeading
                        ,cts.latest_geolocation_address_id as Lcts_LatestGeolocationAddressId
                        ,cts.start_position_lattitude as Lcts_StartPositionLattitude
                        ,cts.start_position_longitude as Lcts_StartPositionLongitude
                        ,cts.start_position_heading as Lcts_StartPositionHeading
                        ,cts.start_geolocation_address_id as Lcts_StartGeolocationAddressId
                        ,cts.latest_processed_message_time_stamp as Lcts_LatestProcessedMessageTimestamp
                        ,cts.vehicle_health_status_type as Lcts_VehicleHealthStatusType
                        ,cts.latest_warning_class as Lcts_LatestWarningClass
                        ,cts.latest_warning_number as Lcts_LatestWarningNumber
                        ,cts.latest_warning_type as Lcts_LatestWarningType
                        ,cts.latest_warning_timestamp as Lcts_LatestWarningTimestamp
                        ,cts.latest_warning_position_latitude as Lcts_LatestWarningPositionLatitude
                        ,cts.latest_warning_position_longitude as Lcts_LatestWarningPositionLongitude
                        ,cts.latest_warning_geolocation_address_id as Lcts_LatestWarningGeolocationAddressId
                        , latgeoadd.Address as Lcts_Address
                                       FROM livefleet.livefleet_current_trip_statistics cts
                                       inner join master.vehicle V on cts.vin = v.vin
                                       left join master.geolocationaddress latgeoadd
                                       on TRUNC(CAST(cts.latest_received_position_lattitude as numeric),4)= TRUNC(CAST(latgeoadd.latitude as numeric),4) 
                                       and TRUNC(CAST(cts.latest_received_position_longitude as numeric),4) = TRUNC(CAST(latgeoadd.longitude as numeric),4)
                                       where v.vin =@vin and ((@tripId <> '' and cts.trip_id=@tripId) OR (@tripId='')) 
                        )  ,

                     

                        WarningData as (
                         SELECT hs.*, 
						  lws.id as WarningId
                        , dri.first_name  || ' ' || dri.last_name as DriverName
                             
                        , lws.trip_id as WarningTripId        
                        , lws.vin as WarningVin          
                        , lws.warning_time_stamp as WarningTimetamp
               
                        , lws.warning_class as WarningClass
               
                        , lws.warning_number as WarningNumber
               
                        , lws.latitude as WarningLat
               
                        , lws.longitude as WarningLng
               
                        , lws.heading as WarningHeading
               
                        , lws.vehicle_health_status_type as WarningVehicleHealthStatusType
               
                        , lws.vehicle_driving_status_type as WarningVehicleDrivingStatusType
               
                        , lws.driver1_id  as WarningDrivingId
               
                        , lws.warning_type     as WarningType          
                        , lws.distance_until_next_service    as WarningDistanceUntilNectService           
                        , lws.odometer_val as WarningOdometerVal
                        ,lws.lastest_processed_message_time_stamp as WarningLatestProcessedMessageTimestamp
                           FROM  HealthSummary hs  inner join livefleet.livefleet_warning_statistics lws
                           on hs.Lcts_Vin= lws.vin
                          inner join master.driver dri on lws.driver1_id=dri.driver_id 
                          where lws.vin =@vin  and ((@tripId <> '' and lws.trip_id=@tripId) OR (@tripId=''))  
                          and (hs.Lcts_TripStartTime > (extract(epoch from (now()::date - @days ))*1000) or hs.Lcts_TripEndTime is null)";
            //   )

            //  select distinct * from WarningData


            if (!string.IsNullOrEmpty(vehicleHealthStatusRequest.WarningType))
            {
                parameter.Add("@warningtype", Convert.ToChar(vehicleHealthStatusRequest.WarningType));

                query += " and lws.warning_type = @warningtype ";
            }
            else if (string.IsNullOrEmpty(vehicleHealthStatusRequest.WarningType) || vehicleHealthStatusRequest.WarningType == "All")
            {

                query += " and lws.warning_type in ('A','D') ";
            }
            query += ")select distinct *from WarningData";
            var healthStatusList = (List<VehicleHealthResult>)await _dataMartdataAccess.QueryAsync<VehicleHealthResult>(query, parameter);
            if (healthStatusList.Count > 0)
            {
                await GetWarningDetails(healthStatusList, vehicleHealthStatusRequest.LngCode);
                return healthStatusList;
            }
            else
            {
                return new List<VehicleHealthResult>();
            }


        }




        public async Task<List<VehicleHealthResult>> GetWarningDetails(List<VehicleHealthResult> warningList, string lngCode)
        {
            try
            {
                foreach (var vehicleHealthWarning in warningList)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@warningClass", vehicleHealthWarning.WarningClass);
                    parameter.Add("@warningNumber", vehicleHealthWarning.WarningNumber);
                    parameter.Add("@code", lngCode);
                    string query = @" SELECT id, code, type, veh_type, class as WarningClass, number as WarningNumber, description as WarningName, advice as WarningAdvice from master.dtcwarning
                                      where class=@warningClass and number =@warningNumber and((@code != '' and code = 'EN-GB') or(@code = '' and code = ''))";
                    var result = await _dataAccess.QueryFirstOrDefaultAsync<WarningDetails>(query, parameter);
                    vehicleHealthWarning.WarningName = result.WarningName;
                    vehicleHealthWarning.WarningAdvice = result.WarningAdvice;

                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return warningList;
        }





        private async Task<string> GetVehicleRunningStatus(string vehicleStatus)
        {
            //TODO add preference condition
            var parameter = new DynamicParameters();
            parameter.Add("@vehicleStatus", vehicleStatus);
            string query = @"SELECT 
                         te.key as Name
                        FROM translation.enumtranslation te                      
                        Where te.type= 'D' and te.enum=@vehicleStatus";
            return await _dataAccess.QueryFirstOrDefaultAsync<string>(query, parameter);
        }


    }
}
