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

        public async Task<IEnumerable<LogbookTripAlertDetails>> GetLogbookSearchParameter(List<string> vins, List<int> featureIds)
        {


            // public int AlertGeolocationAddressId { get; set; }
            //  public string AlertGeolocationAddress { get; set; }

            var parameter = new DynamicParameters();
            var parameterAlert = new DynamicParameters();
            parameterAlert.Add("@featureIds", featureIds);
            var queryStatementFeature = @"select enum from translation.enumtranslation where feature_id = ANY(@featureIds)";
            List<string> resultFeaturEnum = (List<string>)await _dataAccess.QueryAsync<string>(queryStatementFeature, parameterAlert);

            parameter.Add("@vins", vins);
            parameter.Add("@features", resultFeaturEnum);
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
                             and tripalert.type = ANY(@features)
                             and tripalert.category_type <> 'O'
                             and tripalert.type <> 'W'
                           and ((to_timestamp(tripalert.alert_generated_time/1000)::date) <= (now()::date) and (to_timestamp(tripalert.alert_generated_time/1000)::date) >= (now()::date - @days)) ";

            tripAlertList = await _dataMartdataAccess.QueryAsync<LogbookTripAlertDetails>(query, parameter);
            return tripAlertList.AsList<LogbookTripAlertDetails>();
        }

        public async Task<List<LogbookDetails>> GetLogbookDetails(LogbookDetailsFilter logbookFilter)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@start_time_stamp", logbookFilter.Start_Time);
                parameter.Add("@end_time_stamp", logbookFilter.End_time);
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
                                RANK () OVER (PARTITION BY ta.vin,ta.alert_id
								ORDER BY ta.alert_generated_time ASC) Occurrence,
                                ta.urgency_level_type as AlertLevel,
                                ta.alert_generated_time as AlertGeneratedTime,
                                processed_message_time_stamp as ProcessedMessageTimestamp,
                                case when (ts.id is not null ) then ts.start_time_stamp when (ts.id is null and tr.id is not null ) then tr.start_time_stamp end  as TripStartTime,
                                case when (ts.id is not null ) then ts.end_time_stamp when (ts.id is null and tr.id is not null ) then tr.end_time_stamp end as TripEndTime,
                                ts.vehicle_health_status_type as VehicleHealthStatusType,
                                alertgeoadd.id as AlertGeolocationAddressId,
                                coalesce(alertgeoadd.address,'') as AlertGeolocationAddress
                                from tripdetail.tripalert ta inner join master.vehicle v on ta.vin = v.vin 
                                left join livefleet.livefleet_current_trip_statistics ts
                                on ta.vin = ts.vin and ta.trip_id=ts.trip_id
                                left join tripdetail.trip_statistics tr 
                                on  ta.vin = tr.vin  and tr.trip_id=ta.trip_id
                                left join master.geolocationaddress alertgeoadd
                                on TRUNC(CAST(alertgeoadd.latitude as numeric),4)= TRUNC(CAST(ta.latitude as numeric),4) 
                                and TRUNC(CAST(alertgeoadd.longitude as numeric),4) = TRUNC(CAST(ta.longitude as numeric),4)
                                where  (ta.alert_generated_time >= @start_time_stamp and ta.alert_generated_time <= @end_time_stamp)
                                and ta.category_type <> 'O'
                				and ta.type <> 'W' ";
                parameter.Add("@featureIds", logbookFilter.FeatureIds);
                var queryStatementFeature = @"select enum from translation.enumtranslation where feature_id = ANY(@featureIds)";
                List<string> resultFeaturEnum = (List<string>)await _dataAccess.QueryAsync<string>(queryStatementFeature, parameter);
                if (resultFeaturEnum.Count > 0)
                {
                    parameter.Add("@featureEnums", resultFeaturEnum);
                    queryLogBookPull += " and ta.type = Any(@featureEnums) ";
                }
                if (logbookFilter.VIN.Count > 0)
                {
                    parameter.Add("@vin", logbookFilter.VIN);
                    queryLogBookPull += " and ta.vin = Any(@vin) ";
                }
                if (logbookFilter.AlertLevel.Count > 0)
                {
                    parameter.Add("@alert_level", logbookFilter.AlertLevel);
                    queryLogBookPull += " and ta.urgency_level_type = Any(@alert_level) ";
                }

                if (logbookFilter.AlertType.Count > 0)
                {
                    parameter.Add("@alert_type", logbookFilter.AlertType);
                    queryLogBookPull += " and ta.type = Any(@alert_type) ";
                }
                if (logbookFilter.AlertCategory.Count > 0)
                {
                    parameter.Add("@alert_category", logbookFilter.AlertCategory);
                    queryLogBookPull += " and ta.category_type = Any(@alert_category) ";
                }
                var logBookDetailsResult = await _dataMartdataAccess.QueryAsync<LogbookDetails>(queryLogBookPull, parameter);
                if (logBookDetailsResult.AsList<LogbookDetails>().Count > 0)
                {
                    parameter.Add("@AlertIds", logBookDetailsResult.Select(x => x.AlertId).ToArray());
                    string queryAlert = @"select id, Name, organization_id as org_id from master.alert where id = ANY(@AlertIds)";
                    var alertNames = await _dataAccess.QueryAsync<AlertNameList>(queryAlert, parameter);

                    var alertids = alertNames.Where(x => x.Org_Id == logbookFilter.Org_Id).Distinct();
                    var logbookDetails = from logbookresult in logBookDetailsResult
                                         join alert in alertids
                                        on logbookresult.AlertId equals alert.Id
                                         select logbookresult;
                    foreach (var item in logbookDetails)
                    {
                        item.AlertName = alertNames.Where(x => x.Id == item.AlertId).Select(x => x.Name).FirstOrDefault();
                    }
                    return logbookDetails.AsList<LogbookDetails>();
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

        public async Task<IEnumerable<EnumTranslation>> GetAlertCategory()
        {
            try
            {
                var queryStatement = @"SELECT                                     
                                    id as Id, 
                                    type as Type, 
                                    enum as Enum, 
                                    parent_enum as ParentEnum, 
                                    key as Key
                                    FROM translation.enumtranslation;";

                IEnumerable<EnumTranslation> enumtranslationlist = await _dataAccess.QueryAsync<EnumTranslation>(queryStatement, null);

                return enumtranslationlist;
            }
            catch (Exception)
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


