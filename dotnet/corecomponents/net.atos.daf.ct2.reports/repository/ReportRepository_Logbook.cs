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

        public async Task<List<LogbookDetailsFilter>> GetLogbookDetails(LogbookFilter logbookFilter)
        {
            List<LogbookDetailsFilter> logbookDetailsFilters = new List<LogbookDetailsFilter>();

            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vins", logbookFilter.VIN);
                parameter.Add("@start_time_stamp", logbookFilter.Start_Time, System.Data.DbType.Int32);
                parameter.Add("@end_time_stamp", logbookFilter.End_time, System.Data.DbType.Int32);
                string queryLogBookPull = @"select ta.vin as VIN,
                                v.registration_no,
                                v.name as Vehicle_Name,
                                ta.trip_id,category_type as Alert_Category,
                                ta.type as Alert_Type,
                                ta.name as Alert_name
                                alert_id,
                                param_filter_distance_threshold_value as Threshold_Value,
                                param_filter_distance_threshold_value_unit_type as Threshold_unit,
                                latitude,
                                longitude,
                                alert_generated_time,
                                start_time_stamp as Trip_Start,
                                end_time_stamp as Trip_End
                                from tripdetail.tripalert ta left join master.vehicle v on ta.vin = v.vin inner join tripdetail.trip_statistics ts
                                on ta.vin = ts.vin where ta.vin =@vins and ta.start_time_stamp = @start_time_stamp and ta.end_time_stamp = @end_time_stamp";


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
                List<LogbookDetailsFilter> logBookDetailsResult = (List<LogbookDetailsFilter>)await _dataMartdataAccess.QueryAsync<LogbookDetailsFilter>(queryLogBookPull, parameter);
                if (logBookDetailsResult.Count > 0)
                {
                    return logBookDetailsResult;
                }
                else
                {
                    return new List<LogbookDetailsFilter>();
                }

            }

            catch (Exception)
            {
                throw;
            }
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


