using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;
using System.Linq;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        public async Task<VehicleHealthStatus> GetVehicleHealthStatus(VehicleHealthStatusHistoryRequest vehicleHealthStatusHistoryRequest)
        {
            var parameter = new DynamicParameters();
            var vehicleHealthStatus = new VehicleHealthStatus();
            vehicleHealthStatus.VehicleSummary = await GetVehicleSummary(vehicleHealthStatusHistoryRequest.VIN);
            //get quarter fisrt date and last date on load
            vehicleHealthStatus.VehicleSummary.FromDate = vehicleHealthStatusHistoryRequest.FromDate;
            vehicleHealthStatus.VehicleSummary.ToDate = vehicleHealthStatusHistoryRequest.ToDate;
            return vehicleHealthStatus;
        }

        private async Task<VehicleSummary> GetVehicleSummary(string vin)
        {
            //TODO add preference condition
            var parameter = new DynamicParameters();
            parameter.Add("@vin", vin);
            string query = @" SELECT v.vin,
                                    v.registration_no,
                                    v.name,
                                    cts.vehicle_driving_status_type,
                                    cts.latest_received_position_lattitude,
                                    cts.latest_received_position_longitude
                                    FROM livefleet.livefleet_current_trip_statistics cts
                                    left join master.vehicle V on cts.vin = v.vin where vin =@vin";
            var currentHealthStatusSummary = await _dataAccess.QueryFirstOrDefaultAsync<VehicleSummary>(query, parameter);
            currentHealthStatusSummary.Alert = 0;
            return currentHealthStatusSummary;
        }

        private async Task<List<VehicleHealthWarning>> GetWarnningSummary(string vin)
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
            var data = await _dataAccess.QueryAsync<VehicleHealthWarning>(query, parameter);
            return data.ToList();
        }
    }
}
