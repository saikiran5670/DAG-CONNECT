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
        //vin,trip_id,level,alert_generated_time,alert type,alert from datamart of 90 days and in(vinid)

        public async Task<LogbookSearchFilter> GetLogbookSearchParameter(List<string> vins)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@VehicleIds", vins);
            parameter.Add("@days", 90); // return last 3 month of data

            string query = @"select tripalert.vin as Vin
                            ,tripalert.trip_id as TripId
                            ,tripalert.alert_id as AlertId
                            ,tripalert.alert_generated_time as AlertGeneratedTime
                            ,tripalert.category_type as AlertCategoryType
                            ,type as AlertType
                            ,tripalert.name as AlertName
                            from tripdetail.tripalert tripalert   
                            left join livefleet.livefleet_current_trip_statistics lcts on lcts.vin=tripalert.vin and lcts.trip_id=tripalert.trip_id
                            where tripalert.vin= ANY(@VehicleIds)
                            and (lcts.start_time_stamp > (extract(epoch from (to_timestamp(tripalert.alert_generated_time) - @days ))*1000) and < lcts.end_time_stamp)";

            IEnumerable<LogbookSearchFilter> tripAlertList = await _dataMartdataAccess.QueryAsync<LogbookSearchFilter>(query, parameter);
            return new LogbookSearchFilter();
        }
    }

}
