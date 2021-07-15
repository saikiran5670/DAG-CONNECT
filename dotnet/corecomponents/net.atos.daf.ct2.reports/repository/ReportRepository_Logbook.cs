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

        public async Task<IEnumerable<LogbookSearchFilter>> GetLogbookSearchParameter(List<string> vins)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vins", vins);
            parameter.Add("@days", 90); // return last 3 month of data
            IEnumerable<LogbookSearchFilter> tripAlertList;
            string query = @"select tripalert.vin as Vin
                            ,tripalert.trip_id as TripId
                            ,tripalert.alert_id as AlertId
                            ,tripalert.alert_generated_time as AlertGeneratedTime
                            ,tripalert.category_type as AlertCategoryType
                            ,tripalert.type as AlertType
                            ,tripalert.urgency_level_type as AlertLevel
                            ,tripalert.name as AlertName
                            from tripdetail.tripalert tripalert   
                            left join tripdetail.trip_statistics lcts on lcts.vin=tripalert.vin and lcts.trip_id=tripalert.trip_id
                            where tripalert.vin= ANY(@vins)
                            and (lcts.start_time_stamp >= (extract(epoch from (to_timestamp(tripalert.alert_generated_time)::date - @days ))*1000) 
                            and (extract(epoch from (to_timestamp(tripalert.alert_generated_time)::date - @days ))*1000) <= lcts.end_time_stamp)";

            tripAlertList = await _dataMartdataAccess.QueryAsync<LogbookSearchFilter>(query, parameter);
            return tripAlertList.AsList<LogbookSearchFilter>();
        }

        public async Task<List<FilterProperty>> GetAlertLevelList(List<string> enums)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "U");
            parameter.Add("@enums", enums);
            string queryAlertLevelPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type and enum =any(@enums)";

            List<FilterProperty> lstAlertLevel = (List<FilterProperty>)await _dataAccess.QueryAsync<FilterProperty>(queryAlertLevelPull, parameter);
            if (lstAlertLevel.Count > 0)
            {
                return lstAlertLevel;
            }
            else
            {
                return new List<FilterProperty>();
            }
        }

        public async Task<List<AlertCategory>> GetAlertCategoryList(List<string> enums)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "C");
            parameter.Add("@enums", enums);
            string queryAlertCategoryPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type and enum =any(@enums)";

            List<AlertCategory> lstAlertCat = (List<AlertCategory>)await _dataAccess.QueryAsync<AlertCategory>(queryAlertCategoryPull, parameter);
            if (lstAlertCat.Count > 0)
            {
                return lstAlertCat;
            }
            else
            {
                return new List<AlertCategory>();
            }
        }
    }
}


