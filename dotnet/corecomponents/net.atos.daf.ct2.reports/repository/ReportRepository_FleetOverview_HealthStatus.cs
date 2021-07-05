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
                GetPreviousQuarterTime(vehicleHealthStatusRequest);
            }
            vehicleHealthStatus.VehicleSummary.FromDate = vehicleHealthStatusRequest?.FromDate;
            vehicleHealthStatus.VehicleSummary.ToDate = vehicleHealthStatusRequest?.ToDate;
            vehicleHealthStatus.VehicleSummary.WarningType = vehicleHealthStatusRequest.WarningType ?? "All";
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
            healthStatusSummary.VehicleDrivingStatusKey = await GetVehicleRunningStatus(healthStatusSummary.VehicleDrivingStatusEnum);
            return healthStatusSummary;
        }

        private async Task<List<VehicleHealthWarning>> GetCurrentWarnning(string vin)
        {
            //TODO add preference condition
            var parameter = new DynamicParameters();
            parameter.Add("@vin", vin);
            string query = @" SELECT v.vin,
                                    v.latest_warning_type,
                                    wc.activate_time,
                                    wc.deactivated_time,
                                    d.first_name,
                                    d.second_name,
                                    wd.advice
                                    FROM livefleet.livefleet_warning_statistics ws
                                    left join master.warning_details wd on ws.vin = v.vin where vin =@vin";
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
