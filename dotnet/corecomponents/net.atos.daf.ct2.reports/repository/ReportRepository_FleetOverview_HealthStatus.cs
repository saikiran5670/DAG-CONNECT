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
            var query = @"
                         SELECT 
                        lws.id as WarningId
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
                        , lws.driver1_id as WarningDrivingId               
                        , lws.warning_type  as WarningType          
                        , lws.distance_until_next_service    as WarningDistanceUntilNectService           
                        , lws.odometer_val as WarningOdometerVal
                        ,lws.lastest_processed_message_time_stamp as WarningLatestProcessedMessageTimestamp
                        ,latgeoadd.id as WarningAddressId
                        ,coalesce(latgeoadd.address,'') as WarningAddress
                        FROM  livefleet.livefleet_warning_statistics lws
                        join master.vehicle veh
                        on lws.vin = veh.vin and lws.warning_time_stamp >= veh.reference_date
                        left join master.geolocationaddress latgeoadd  on TRUNC(CAST(lws.latitude as numeric),4)= TRUNC(CAST(latgeoadd.latitude as numeric), 4)
                        and TRUNC(CAST(lws.longitude as numeric),4) = TRUNC(CAST(latgeoadd.longitude as numeric), 4) 
                        where lws.vin =@vin  
                        and ((@tripId <> '' and lws.trip_id=@tripId) OR (@tripId=''))  
                        and to_timestamp(lws.warning_time_stamp/1000)::date >= (now()::date -  @days )
                        and message_type=10 ";

            if (!string.IsNullOrEmpty(vehicleHealthStatusRequest.WarningType))
            {
                parameter.Add("@warningtype", Convert.ToChar(vehicleHealthStatusRequest.WarningType));

                query += " and lws.warning_type = @warningtype ";
            }
            else if (string.IsNullOrEmpty(vehicleHealthStatusRequest.WarningType))
            {

                query += " and lws.warning_type in ('A','D','I') ";
            }
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
                parameter.Add("@engCode", "EN-GB");
                string query = @" SELECT warning.id, warning.code as LngCode, warning.type, warning.veh_type,warning.class as WarningClass, warning.number as WarningNumber,
                                  warning.description as WarningName, warning.advice as WarningAdvice,
								  warning.icon_id as IconId,icon.icon,icon.name as IconName,icon.color_name as ColorName
								  from master.dtcwarning warning
                                  LEFT JOIN master.icon icon on  warning.icon_id=icon.id  and icon.state in ('A','I') 
                                  where warning.class= Any(@warningClass) and warning.number = Any(@warningNumber) 
                                    and((Lower(@code) != '' and Lower(code) =Lower(@code)) or(Lower(@code) = '' and Lower(code) = ''))";
                query += @"union SELECT warning.id
                , warning.code as LngCode
                ,warning.type, warning.veh_type
                ,warning.class as WarningClass
                , warning.number as WarningNumber
                ,warning.description as WarningName
                ,warning.advice as WarningAdvice
                ,warning.icon_id as IconId
                ,icon.icon,icon.name as IconName
                ,icon.color_name as ColorName
                from master.dtcwarning warning
                LEFT JOIN master.icon icon on warning.icon_id=icon.id and icon.state in ('A','I')
                where warning.class= Any(@warningClass) and warning.number = Any(@warningNumber) 
                 and((Lower(@engCode) != '' and Lower(code) =Lower(@engCode)) or(Lower(@engCode) = '' and Lower(code) = ''))";
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
