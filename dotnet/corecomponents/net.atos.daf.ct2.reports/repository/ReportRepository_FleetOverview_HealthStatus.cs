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
                GetNextQuarterTime(vehicleHealthStatusRequest);
            }
            vehicleHealthStatus.VehicleSummary.FromDate = vehicleHealthStatusRequest?.FromDate;
            vehicleHealthStatus.VehicleSummary.ToDate = vehicleHealthStatusRequest?.ToDate;

            return vehicleHealthStatus;
        }
  

        private async Task<VehicleSummary> GetVehicleHealthSummary(string vin)
        {
            //TODO add preference condition
            var parameter = new DynamicParameters();
            parameter.Add("@vin", vin);
            string query = @" SELECT v.vin as VIN,
                                    v.registration_no as VehicleRegNo,
                                    v.name as VehicleName,
                                    cts.vehicle_driving_status_type as VehicleDrivingStatus,
                                    cts.latest_received_position_lattitude as LastLatitude,
                                    cts.latest_received_position_longitude as LastLongitude
                                    FROM livefleet.livefleet_current_trip_statistics cts
                                    left join master.vehicle V on cts.vin = v.vin where v.vin =@vin";
            var healthStatusSummary = await _dataMartdataAccess.QueryFirstOrDefaultAsync<dynamic>(query, parameter);
         //   healthStatusSummary.Alert = 0;
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

        public void GetNextQuarterTime(VehicleHealthStatusRequest vehicleHealthStatusHitory)
        {
            var date = DateTime.Now;
            var quarterNumber = ((date.Month - 1) / 3) + 1;
            var firstDayOfQuarter = new DateTime(date.Year, ((quarterNumber - 1) * 3) + 1, 1);
            var lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
            var nextQuarter = quarterNumber + 1;
            var firstDayOfnextQuarter = new DateTime(date.Year, ((nextQuarter - 1) * 3) + 1, 1);
            var lastDayOfnextQuarter = firstDayOfnextQuarter.AddMonths(3).AddDays(-1);

            vehicleHealthStatusHitory.FromDate = UTCHandling.GetUTCFromDateTime(firstDayOfnextQuarter);
            vehicleHealthStatusHitory.ToDate = UTCHandling.GetUTCFromDateTime(lastDayOfnextQuarter);

        }

    }
}
