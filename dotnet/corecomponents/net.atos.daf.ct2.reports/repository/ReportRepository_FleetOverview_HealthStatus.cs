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
        public async Task<VehicleHealthStatus> GetVehicleHealthStatus(VehicleHealthStatusRequest vehicleHealthStatusRequest)
        {
            var parameter = new DynamicParameters();
            var vehicleHealthStatus = new VehicleHealthStatus();
            vehicleHealthStatus.VehicleSummary = await GetVehicleHealthSummary(vehicleHealthStatusRequest.VIN);
            if (vehicleHealthStatusRequest.FromDate == null && vehicleHealthStatusRequest.ToDate == null)
            {
                vehicleHealthStatus.CurrentWarning = await GetCurrentWarnning(vehicleHealthStatusRequest.VIN);
                GetPreviousQuarterTime(vehicleHealthStatusRequest);
            }
            vehicleHealthStatus.VehicleSummary.FromDate = vehicleHealthStatusRequest?.FromDate;
            vehicleHealthStatus.VehicleSummary.ToDate = vehicleHealthStatusRequest?.ToDate;
            vehicleHealthStatus.VehicleSummary.WarningType = vehicleHealthStatusRequest.WarningType ?? "All";
            vehicleHealthStatus.HistoryWarning = await GetHistoryWarning(vehicleHealthStatusRequest);
            return vehicleHealthStatus;
        }





        public async Task<VehicleHealthStatus> GetHealthStatus(VehicleHealthStatusRequest vehicleHealthStatusReques)
        {

            var parameter = new DynamicParameters();
            parameter.Add("@vin", vehicleHealthStatusReques.VIN);
            parameter.Add("@tripId", vehicleHealthStatusReques.TripId);
            parameter.Add("@enddatetime", vehicleHealthStatusReques.ToDate);
            parameter.Add("@endstarttime", vehicleHealthStatusReques.FromDate);

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
                        )

                        -- select * from WarningData

                        select distinct * from WarningData where
                                ((@startdatetime <> 0 and Lcts_TripStartTime >=@startdatetime ) OR (@startdatetime =0)) 
                                and ((@enddatetime  <> 0 and Lcts_TripEndTime <=@enddatetime) OR (@enddatetime=0))";


            var healthStatus = await _dataMartdataAccess.QueryAsync<VehicleHealthResult>(query, parameter);
            return new VehicleHealthStatus();
        }
        private async Task<VehicleSummary> GetVehicleHealthSummary(string vin)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vin", vin);
            string query = @" SELECT v.vin as VIN,
                                    v.registration_no as VehicleRegNo,
                                    v.name as VehicleName,
                                    cts.vehicle_driving_status_type as VehicleDrivingStatusEnum,
                                    cts.latest_received_position_lattitude as LastLatitude,
                                    cts.latest_received_position_longitude as LastLongitude,
                                    latgeoadd.Address as Address
                                    FROM livefleet.livefleet_current_trip_statistics cts
                                    left join master.vehicle V on cts.vin = v.vin
                                    left join master.geolocationaddress latgeoadd
                                    on TRUNC(CAST(cts.latest_received_position_lattitude as numeric),4)= TRUNC(CAST(latgeoadd.latitude as numeric),4) 
                                    and TRUNC(CAST(cts.latest_received_position_longitude as numeric),4) = TRUNC(CAST(latgeoadd.longitude as numeric),4)
                                    where v.vin =@vin"
;
            var healthStatusSummary = await _dataMartdataAccess.QueryFirstOrDefaultAsync<VehicleSummary>(query, parameter);
            healthStatusSummary.Alert = 0;
            //healthStatusSummary.VehicleDrivingStatusKey = await GetVehicleRunningStatus(healthStatusSummary.VehicleDrivingStatusEnum);


            return healthStatusSummary;
        }

        public async Task<List<VehicleHealthWarning>> GetCurrentWarnning(string vin)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vin", vin);
            string currentQuery = @"SELECT lws.id, dri.first_name as DriverFirstName ,dri.last_name as DriverLastName,
                                      lcs.vin as Vin, lws.warning_time_stamp as WarningTimestamp, lws.warning_class as WarningClass, lws.warning_number as WarningNumber, lws.latitude as WarningLat,
                                      lws.longitude as WarningLng, lws.vehicle_health_status_type as VehicleHealthStatusEnum, lws.vehicle_driving_status_type as VehicleDrivingStatusEnum,
                                      lws.driver1_id as Driver1Id, lws.warning_type as WarningTypeEnum,lws.lastest_processed_message_time_stamp as LastestProcessedMessageTimestamp FROM 
                                      livefleet.livefleet_warning_statistics lws
                                      left join livefleet.livefleet_current_trip_statistics  lcs on lws.vin=lcs.vin  
                                      left join master.driver dri on lcs.driver1_id=dri.driver_id 
                                      where lcs.vin =@vin and lcs.latest_warning_timestamp = (select max(latest_warning_timestamp) from livefleet.livefleet_current_trip_statistics)";

            var data = await _dataMartdataAccess.QueryAsync<VehicleHealthWarning>(currentQuery, parameter);
            return data.ToList();
        }


        public async Task<List<VehicleHealthWarning>> GetHistoryWarning(VehicleHealthStatusRequest request)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vin", request.VIN);
            string query = @" SELECT lws.id, dri.driver_id as DriverId ,dri.first_name as DriverFirstName ,dri.last_name as DriverLastName,
                              lcs.vin as Vin, lws.warning_time_stamp as WarningTimestamp, lws.warning_class as WarningClass, lws.warning_number as WarningNumber, lws.latitude as WarningLat,
                              lws.longitude as WarningLng, lws.vehicle_health_status_type as VehicleHealthStatusEnum, lws.vehicle_driving_status_type as VehicleDrivingStatusEnum,
                              lws.driver1_id as Driver1Id, lws.warning_type as WarningTypeEnum,lws.lastest_processed_message_time_stamp as LastestProcessedMessageTimestamp FROM 
                              livefleet.livefleet_warning_statistics lws
                              left join livefleet.livefleet_current_trip_statistics  lcs 
                              on lws.vin=lcs.vin and lcs.latest_warning_timestamp between lcs.start_time_stamp  and COALESCE(lcs.end_time_stamp, extract(epoch from current_timestamp) * 1000)
                              left join master.driver dri on lcs.driver1_id=dri.driver_id                                                
                              where lcs.vin =@vin   
                              order by lcs.end_time_stamp DESC";
            var data = await _dataMartdataAccess.QueryAsync<VehicleHealthWarning>(query, parameter);
            var warningList = data.ToList();
            await GetWarningDetails(warningList, request.LngCode);
            return warningList;
        }
        public async Task<List<VehicleHealthWarning>> GetWarningDetails(List<VehicleHealthWarning> warningList, string lngCode)
        {
            try
            {
                foreach (var vehicleHealthWarning in warningList)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@warningClass", vehicleHealthWarning.WarningClass);
                    parameter.Add("@warningNumber", vehicleHealthWarning.WarningNumber);
                    parameter.Add("@code", lngCode);
                    string query = @" SELECT id, code, type, veh_type, class, number, description as Name, advice as Advice from master.dtcwarning
                                      where class=@warningClass and number =@warningNumber and((@code != '' and code = 'EN-GB') or(@code = '' and code = ''))";
                    var result = await _dataAccess.QueryFirstOrDefaultAsync<VehicleHealthWarning>(query, parameter);
                    vehicleHealthWarning.Name = result.Name;
                    vehicleHealthWarning.Advice = result.Advice;

                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return warningList;
        }



        public void GetPreviousQuarterTime(VehicleHealthStatusRequest vehicleHealthStatusHitory)
        {
            var firstDayOfLastQMonth = DateTime.Now.AddMonths(-3);
            vehicleHealthStatusHitory.FromDate = UTCHandling.GetUTCFromDateTime(firstDayOfLastQMonth);
            vehicleHealthStatusHitory.ToDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
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
