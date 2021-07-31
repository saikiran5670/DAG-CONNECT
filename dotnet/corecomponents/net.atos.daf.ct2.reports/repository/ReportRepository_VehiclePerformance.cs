using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        public async Task<IEnumerable<VehiclePerformanceRequest>> GetSummaryDetails(string vin)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vin", vin);
            IEnumerable<VehiclePerformanceRequest> tripAlertList;
            string query = @"SELECT vin,
                                    vid, engine_type, model_type, name FROM master.vehicle";
            tripAlertList = await _dataMartdataAccess.QueryAsync<VehiclePerformanceRequest>(query, parameter);
            return tripAlertList.AsList<VehiclePerformanceRequest>();

        }
    }
}
