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

        public async Task<IEnumerable<LogbookTripAlertDetails>> GetLogbookSearchParameter(List<string> vins)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vins", vins);
            parameter.Add("@days", 90); // return last 3 month of data
            IEnumerable<LogbookTripAlertDetails> tripAlertList;
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
                           and ((to_timestamp(tripalert.alert_generated_time)::date) <= (now()::date) and (to_timestamp(tripalert.alert_generated_time)::date) >= (now()::date - @days)) ";

            tripAlertList = await _dataMartdataAccess.QueryAsync<LogbookTripAlertDetails>(query, parameter);
            return tripAlertList.AsList<LogbookTripAlertDetails>();
        }

        public async Task<List<LogbookDetails>> GetLogbookDetails(LogbookDetailsFilter logbookFilter)
        {
            List<LogbookDetails> logbookDetailsFilters = new List<LogbookDetails>();

            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@start_time_stamp", logbookFilter.Start_Time, System.Data.DbType.Int32);
                parameter.Add("@end_time_stamp", logbookFilter.End_time, System.Data.DbType.Int32);
                string queryLogBookPull = @"select ta.vin as VIN,
                                v.registration_no,
                                v.name as Vehicle_Name,
                                ta.trip_id,category_type as Alert_Category,
                                ta.type as Alert_Type,
                                ta.name as Alert_name,
                                alert_id AlertId,                             
                                latitude,
                                longitude,
                                alert_generated_time,
                                start_time_stamp as Trip_Start,
                                end_time_stamp as Trip_End
                                from tripdetail.tripalert ta left join master.vehicle v on ta.vin = v.vin inner join tripdetail.trip_statistics ts
                                on ta.vin = ts.vin where 1=1 
                                and ((to_timestamp(ta.alert_generated_time)::date) >= (to_timestamp(@start_time_stamp)::date)
                                and (to_timestamp(ta.alert_generated_time)::date) <= (to_timestamp(@end_time_stamp )::date))";



                if (logbookFilter.VIN.Count > 0)
                {
                    parameter.Add("@vin", logbookFilter.VIN);
                    queryLogBookPull += " and ta.vin = Any(@vin) ";
                }
                if (logbookFilter.AlertLevel.Count > 0)
                {
                    parameter.Add("@alert_level", logbookFilter.AlertLevel);
                    queryLogBookPull += " and ta.alert_level = Any(@alert_level) ";
                }

                if (logbookFilter.AlertType.Count > 0)
                {
                    parameter.Add("@alert_level", logbookFilter.AlertLevel);
                    queryLogBookPull += " and ta.alert_level = Any(@alert_level) ";
                }
                if (logbookFilter.AlertCategory.Count > 0)
                {
                    parameter.Add("@alert_category", logbookFilter.AlertCategory);
                    queryLogBookPull += " and ta.alert_category = Any(@alert_category) ";
                }
                var logBookDetailsResult = await _dataMartdataAccess.QueryAsync<LogbookDetails>(queryLogBookPull, parameter);
                if (logBookDetailsResult.AsList<LogbookDetails>().Count > 0)
                {
                    return logBookDetailsResult.AsList<LogbookDetails>();
                }
                else
                {
                    return new List<LogbookDetails>();
                }

            }

            catch (Exception ex)
            {
                throw;
            }
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


