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
            vehicleHealthStatus.HistoryWarning = await GetHistoryWarnning(vehicleHealthStatusRequest.VIN);
            return vehicleHealthStatus;
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


        public async Task<List<VehicleHealthWarning>> GetHistoryWarnning(string vin)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vin", vin);
            string query = @" SELECT lws.id, dri.driver_id as DriverId ,dri.first_name as DriverFirstName ,dri.last_name as DriverLastName,
                              lcs.vin as Vin, lws.warning_time_stamp as WarningTimestamp, lws.warning_class as WarningClass, lws.warning_number as WarningNumber, lws.latitude as WarningLat,
                              lws.longitude as WarningLng, lws.vehicle_health_status_type as VehicleHealthStatusEnum, lws.vehicle_driving_status_type as VehicleDrivingStatusEnum,
                              lws.driver1_id as Driver1Id, lws.warning_type as WarningTypeEnum,lws.lastest_processed_message_time_stamp as LastestProcessedMessageTimestamp FROM 
                              livefleet.livefleet_current_trip_statistics  lcs 
                              left join livefleet.livefleet_warning_statistics lws
                              on lws.vin=lcs.vin and lcs.latest_warning_timestamp between lcs.start_time_stamp  and COALESCE(lcs.end_time_stamp, extract(epoch from current_timestamp) * 1000)
                              left join master.driver dri on lcs.driver1_id=dri.driver_id                                                
                              where lcs.vin =@vin   
                              order by lcs.end_time_stamp DESC";
            var data = await _dataMartdataAccess.QueryAsync<VehicleHealthWarning>(query, parameter);
            return data.ToList();
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
