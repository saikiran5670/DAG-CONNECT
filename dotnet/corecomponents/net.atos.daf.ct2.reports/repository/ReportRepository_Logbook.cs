using System;
using System.Collections.Generic;
using System.Linq;
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


            // public int AlertGeolocationAddressId { get; set; }
            //  public string AlertGeolocationAddress { get; set; }

            var parameter = new DynamicParameters();
            parameter.Add("@vins", vins);
            parameter.Add("@days", 90); // return last 3 month of data
            IEnumerable<LogbookTripAlertDetails> tripAlertList;
            string query = @"select distinct id, tripalert.vin as Vin
                              ,tripalert.trip_id as TripId
                         --   ,tripalert.alert_id as AlertId
                            ,tripalert.alert_generated_time as AlertGeneratedTime
                         --   ,processed_message_time_stamp as ProcessedMessageTimestamp
                         --   ,tripalert.latitude as AlertLatitude
                          --  ,tripalert.longitude as AlertLongitude
                            ,tripalert.category_type as AlertCategoryType
                            ,tripalert.type as AlertType
                            ,tripalert.urgency_level_type as AlertLevel
                        --    ,tripalert.name as AlertName
                          --  , alertgeoadd.id as AlertGeolocationAddressId
                          --  ,coalesce(alertgeoadd.address,'') as AlertGeolocationAddress
                            from tripdetail.tripalert tripalert  
                           -- left join tripdetail.trip_statistics lcts on lcts.vin=tripalert.vin and lcts.trip_id=tripalert.trip_id
                          --  left join master.geolocationaddress alertgeoadd
                           -- on TRUNC(CAST(alertgeoadd.latitude as numeric),4)= TRUNC(CAST(tripalert.latitude as numeric),4)
                         --   and TRUNC(CAST(alertgeoadd.longitude as numeric),4) = TRUNC(CAST(tripalert.longitude as numeric),4)
                            where tripalert.vin= ANY(@vins)
                           and ((to_timestamp(tripalert.alert_generated_time/1000)::date) <= (now()::date) and (to_timestamp(tripalert.alert_generated_time/1000)::date) >= (now()::date - @days)) ";

            tripAlertList = await _dataMartdataAccess.QueryAsync<LogbookTripAlertDetails>(query, parameter);
            return tripAlertList.AsList<LogbookTripAlertDetails>();
        }

        public async Task<List<LogbookDetails>> GetLogbookDetails(LogbookDetailsFilter logbookFilter)
        {
            List<LogbookDetails> logbookDetailsFilters = new List<LogbookDetails>();

            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@start_time_stamp", logbookFilter.Start_Time / 1000, System.Data.DbType.Int32);
                parameter.Add("@end_time_stamp", logbookFilter.End_time / 1000, System.Data.DbType.Int32);
                string queryLogBookPull = @"select distinct ta.vin as VIN,
                                v.registration_no as VehicleRegNo,
                                v.name as VehicleName,
                                ta.trip_id as TripId,
                                ta.category_type as AlertCategory,
                                ta.type as AlertType,
                                ta.name as Alertname,
                                ta.alert_id as AlertId,                             
                                ta.latitude as Latitude,
                                ta.longitude as Longitude,
                                ta.urgency_level_type as AlertLevel,
                                ta.alert_generated_time as AlertGeneratedTime,
                                processed_message_time_stamp as ProcessedMessageTimestamp,
                                ts.start_time_stamp as TripStartTime,
                                ts.end_time_stamp as TripEndTime,
                                alertgeoadd.id as AlertGeolocationAddressId,
                                coalesce(alertgeoadd.address,'') as AlertGeolocationAddress
                                from tripdetail.tripalert ta inner join master.vehicle v on ta.vin = v.vin 
                                left join tripdetail.trip_statistics ts
                                on ta.vin = ts.vin  --and ta.trip_id=ts.trip_id 
                                left join master.geolocationaddress alertgeoadd
                                on TRUNC(CAST(alertgeoadd.latitude as numeric),4)= TRUNC(CAST(ta.latitude as numeric),4) 
                                and TRUNC(CAST(alertgeoadd.longitude as numeric),4) = TRUNC(CAST(ta.longitude as numeric),4)
                                where 1=1 
                                and ((to_timestamp(ta.alert_generated_time/1000)::date) >= (to_timestamp(@start_time_stamp)::date)
                                and (to_timestamp(ta.alert_generated_time/1000)::date) <= (to_timestamp(@end_time_stamp )::date))";



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



        public async Task<List<AlertThresholdDetails>> GetThresholdDetails(List<int> alertId, List<string> alertLevel)
        {
            IEnumerable<AlertThresholdDetails> thresholdList;
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alert_id", alertId);
                parameter.Add("@urgency_level_type", alertLevel);
                string query = @" SELECT id, alert_id as AlertId, urgency_level_type as AlertLevel, threshold_value as ThresholdValue, unit_type as ThresholdUnit from master.alerturgencylevelref
                                where alert_id = any(@alert_id) and urgency_level_type = any(@urgency_level_type) and state ='A' ";
                thresholdList = await _dataAccess.QueryAsync<AlertThresholdDetails>(query, parameter);
            }
            catch (Exception ex)
            {
                throw;
            }
            return thresholdList.ToList();
        }
    }
}


