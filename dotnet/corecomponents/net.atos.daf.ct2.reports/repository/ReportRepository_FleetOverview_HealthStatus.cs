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
                        ,cts.trip_id as LctsTripId
                        ,cts.vin as LctsVin
                        ,cts.start_time_stamp as LctsTripStartTime
                        ,cts.end_time_stamp as LctsTripEndTime
                        ,cts.driver1_id as LctsDriver1Id
                        ,cts.trip_distance as LctsTripDistance
                        ,cts.driving_time as LctsDrivingTime
                        ,cts.fuel_consumption as LctsFuelConsumption
                        , cts.vehicle_driving_status_type as LctsVehicleDrivingStatustype
                        ,cts.odometer_val as LctsOdometerVal
                        ,cts.distance_until_next_service as LctsDistanceUntilNextService
                        ,cts.latest_received_position_lattitude as LctsLatestReceivedPositionLattitude
                        ,cts.latest_received_position_longitude as LctsLatestReceivedPositionLongitude
                        ,cts.latest_received_position_heading as LctsLatestReceivedPositionHeading                      
                        ,cts.start_position_lattitude as LctsStartPositionLattitude
                        ,cts.start_position_longitude as LctsStartPositionLongitude
                        ,cts.start_position_heading as LctsStartPositionHeading 
                        ,cts.latest_processed_message_time_stamp as LctsLatestProcessedMessageTimestamp
                        ,cts.vehicle_health_status_type as LctsVehicleHealthStatusType
                        ,cts.latest_warning_class as LctsLatestWarningClass
                        ,cts.latest_warning_number as LctsLatestWarningNumber
                        ,cts.latest_warning_type as LctsLatestWarningType
                        ,cts.latest_warning_timestamp as LctsLatestWarningTimestamp
                        ,cts.latest_warning_position_latitude as LctsLatestWarningPositionLatitude
                        ,cts.latest_warning_position_longitude as LctsLatestWarningPositionLongitude,                      
                        latgeoadd.id as latgeoaddLatestGeolocationAddressId,
                        coalesce(latgeoadd.address,'') as latgeoaddLatestGeolocationAddress,
                        stageoadd.id as stageoaddStartGeolocationAddressId,
                        coalesce(stageoadd.address,'') as stageoaddStartGeolocationAddress,
                        wangeoadd.id as wangeoaddLatestWarningGeolocationAddressId,
                        coalesce(wangeoadd.address,'') as wangeoaddLatestWarningGeolocationAddress

                                       FROM livefleet.livefleet_current_trip_statistics cts
                                       inner join master.vehicle V on cts.vin = v.vin
                                       left join master.geolocationaddress latgeoadd
                                       on TRUNC(CAST(cts.latest_received_position_lattitude as numeric),4)= TRUNC(CAST(latgeoadd.latitude as numeric),4) 
                                       and TRUNC(CAST(cts.latest_received_position_longitude as numeric),4) = TRUNC(CAST(latgeoadd.longitude as numeric),4) 
                                        left join master.geolocationaddress stageoadd
                                        on TRUNC(CAST(cts.start_position_lattitude as numeric),4)= TRUNC(CAST(stageoadd.latitude as numeric),4) 
                                        and TRUNC(CAST(cts.start_position_longitude as numeric),4) = TRUNC(CAST(stageoadd.longitude as numeric),4)
                                        left join master.geolocationaddress wangeoadd
                                        on TRUNC(CAST(cts.latest_warning_position_latitude as numeric),4)= TRUNC(CAST(wangeoadd.latitude as numeric),4) 
                                        and TRUNC(CAST(cts.latest_warning_position_longitude as numeric),4) = TRUNC(CAST(wangeoadd.longitude as numeric),4)

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
                           on hs.LctsVin= lws.vin
                          inner join master.driver dri on lws.driver1_id=dri.driver_id 
                          where lws.vin =@vin  and ((@tripId <> '' and lws.trip_id=@tripId) OR (@tripId=''))  
                          and (hs.LctsTripStartTime > (extract(epoch from (now()::date - @days ))*1000) or hs.LctsTripEndTime is null)";
            //   )

            //  select distinct * from WarningData


            if (!string.IsNullOrEmpty(vehicleHealthStatusRequest.WarningType))
            {
                parameter.Add("@warningtype", Convert.ToChar(vehicleHealthStatusRequest.WarningType));

                query += " and lws.warning_type = @warningtype ";
            }
            else if (string.IsNullOrEmpty(vehicleHealthStatusRequest.WarningType))
            {

                query += " and lws.warning_type in ('A','D') ";
            }
            query += ")select distinct *from WarningData";
            var healthStatusList = (List<VehicleHealthResult>)await _dataMartdataAccess.QueryAsync<VehicleHealthResult>(query, parameter);
            if (healthStatusList.Count > 0)
            {
                return healthStatusList;
            }
            else
            {
                return new List<VehicleHealthResult>();
            }


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
        public async Task<List<WarningDetails>> GetWarningDetails(List<int> warningClass, List<int> warningNumber, string lngCode)
        {
            IEnumerable<WarningDetails> warningList;
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@warningClass", warningClass);
                parameter.Add("@warningNumber", warningNumber);
                parameter.Add("@code", lngCode.ToLower());
                string query = @" SELECT id, code, type, veh_type, class as WarningClass, number as WarningNumber, description as WarningName, advice as WarningAdvice from master.dtcwarning
                                    where class= Any(@warningClass) and number = Any(@warningNumber) and((@code != '' and Lower(code) = @code) or(@code = '' and code = ''))";
                warningList = await _dataAccess.QueryAsync<WarningDetails>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
            return warningList.ToList();
        }
        public async Task<List<DriverDetails>> GetDriverDetails(List<string> driverIds, int organizationId)
        {
            IEnumerable<DriverDetails> driverList;
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@driverIds", driverIds);
                parameter.Add("@organizationId", organizationId);
                string query = @"select driver_id_ext as DriverId,case when opt_in ='I' or opt_in='H' 
	                              then first_name|| ' ' || last_name else '*' end as DriverName,opt_in as DriverStatus  from master.driver              
                                  where driver_id_ext = Any(@driverIds) and organization_id = @organizationId and state ='A' ";
                driverList = await _dataAccess.QueryAsync<DriverDetails>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
            return driverList.ToList();
        }
    }
}
