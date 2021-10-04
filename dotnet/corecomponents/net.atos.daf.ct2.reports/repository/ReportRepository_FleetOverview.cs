﻿using System;
using System.Collections.Generic;
using System.Text;
using net.atos.daf.ct2.reports.entity;
using System.Threading.Tasks;
using Dapper;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        #region Fleet Overview Filter
        public async Task<List<FilterProperty>> GetAlertLevelList()
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "U");
            string queryAlertLevelPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type";

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

        public async Task<List<AlertCategory>> GetAlertCategoryList()
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "C");
            string queryAlertCategoryPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type";

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

        public async Task<List<FilterProperty>> GetHealthStatusList()
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "H");
            string queryHealthStatusPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type";

            List<FilterProperty> lstHealthStatus = (List<FilterProperty>)await _dataAccess.QueryAsync<FilterProperty>(queryHealthStatusPull, parameter);
            if (lstHealthStatus.Count > 0)
            {
                return lstHealthStatus;
            }
            else
            {
                return new List<FilterProperty>();
            }
        }

        public async Task<List<FilterProperty>> GetOtherFilter()
        {
            var parameter = new DynamicParameters();
            parameter.Add("@type", "D");
            string queryOtherFilterPull = @"SELECT key as Name,
                                                     enum as Value
	                                          FROM translation.enumtranslation 
                                              Where type=@type";

            List<FilterProperty> lstOtherFilter = (List<FilterProperty>)await _dataAccess.QueryAsync<FilterProperty>(queryOtherFilterPull, parameter);
            if (lstOtherFilter.Count > 0)
            {
                return lstOtherFilter;
            }
            else
            {
                return new List<FilterProperty>();
            }
        }

        public async Task<List<DriverFilter>> GetDriverList(List<string> vins, int organizationId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@VehicleIds", vins);
            parameter.Add("@organization_id", organizationId);
            string queryDriverFilterPull = @"select distinct dri.driver_id as DriverId
                                                ,dri.first_name as FirstName
                                                ,dri.last_name as LastName
                                                ,dri.organization_id as OrganizationId
                                                from livefleet.livefleet_current_trip_statistics cts
                                                inner join master.driver dri
                                                on cts.driver1_id=driver_id
                                                where cts.vin= ANY(@VehicleIds)
                                                and dri.organization_id=@organization_id";

            List<DriverFilter> lstOtherFilter = (List<DriverFilter>)await _dataMartdataAccess.QueryAsync<DriverFilter>(queryDriverFilterPull, parameter);
            if (lstOtherFilter.Count > 0)
            {
                return lstOtherFilter;
            }
            else
            {
                return new List<DriverFilter>();
            }
        }

        #endregion

        public async Task<List<FleetOverviewDetails>> GetFleetOverviewDetails(FleetOverviewFilter fleetOverviewFilter)
        {
            List<FleetOverviewDetails> fleetOverviewDetails = new List<FleetOverviewDetails>();
            MapperRepo repositoryMapper = new MapperRepo();
            try
            {
                var parameterFleetOverview = new DynamicParameters();
                parameterFleetOverview.Add("@vins", fleetOverviewFilter.VINIds);
                //filter trip data by n days
                //parameterFleetOverview.Add("@days", string.Concat("'", fleetOverviewFilter.Days.ToString(), "d", "'"));
                parameterFleetOverview.Add("@days", fleetOverviewFilter.Days, System.Data.DbType.Int32);
                parameterFleetOverview.Add("@drivinginterval", fleetOverviewFilter.UnknownDrivingStateCheckInterval, System.Data.DbType.Int32);
                string queryFleetOverview = @"With CTE_Trips_By_Vin as(
                    select 
                    lcts.id,
                    lcts.trip_id,
                    lcts.vin,
                    lcts.start_time_stamp,
                    lcts.end_time_stamp,
                    lcts.driver1_id,
                    lcts.trip_distance,
                    lcts.driving_time,
                    lcts.fuel_consumption,
                    lcts.vehicle_driving_status_type,
                    lcts.odometer_val,
                    lcts.distance_until_next_service,
                    lcts.latest_received_position_lattitude,
                    lcts.latest_received_position_longitude,
                    lcts.latest_received_position_heading,
                    lcts.start_position_lattitude,
                    lcts.start_position_longitude,
                    lcts.start_position_heading,
                    lcts.latest_processed_message_time_stamp,
                    lcts.vehicle_health_status_type,
                    lcts.latest_warning_class,
                    lcts.latest_warning_number,
                    lcts.latest_warning_type,
                    lcts.latest_warning_timestamp,
                    lcts.latest_warning_position_latitude,
                    lcts.latest_warning_position_longitude,
                    RANK() Over ( Partition By lcts.vin Order by  lcts.start_time_stamp desc ) Veh_trip_rank
                    from livefleet.livefleet_current_trip_statistics lcts
                    where lcts.vin = Any(@vins) 
                    and to_timestamp(lcts.latest_processed_message_time_stamp/1000)::date >= (now()::date -  @days )
                    )
                    ,CTE_Unique_latest_trip as (
                     select 
                     *,
                     ROW_NUMBER() OVER( PARTITION BY Vin ORDER BY Id desc) AS row_num 
                     from CTE_Trips_By_Vin 
                     where Veh_trip_rank =1 
                    )
                    select 
                    lcts.id as lcts_Id,
                    lcts.trip_id as lcts_TripId,
                    lcts.vin as lcts_Vin,
                    lcts.start_time_stamp as lcts_StartTimeStamp,
                    lcts.end_time_stamp as lcts_EndTimeStamp,
                    coalesce(lcts.driver1_id,'') as lcts_Driver1Id,
                    lcts.trip_distance as lcts_TripDistance,
                    lcts.driving_time as lcts_DrivingTime,
                    lcts.fuel_consumption as lcts_FuelConsumption,
                    case when lcts.vehicle_driving_status_type in('D','I') 
                    and EXTRACT(EPOCH FROM (now() - to_timestamp(end_time_stamp / 1000)))/60 >= @drivinginterval 
                    then 'U' else vehicle_driving_status_type end lcts_VehicleDrivingStatusType,
                    lcts.odometer_val as lcts_OdometerVal,
                    lcts.distance_until_next_service as lcts_DistanceUntilNextService,
                    lcts.latest_received_position_lattitude as lcts_LatestReceivedPositionLattitude,
                    lcts.latest_received_position_longitude as lcts_LatestReceivedPositionLongitude,
                    lcts.latest_received_position_heading as lcts_LatestReceivedPositionHeading,
                    lcts.start_position_lattitude as lcts_StartPositionLattitude,
                    lcts.start_position_longitude as lcts_StartPositionLongitude,
                    lcts.start_position_heading as lcts_StartPositionHeading,
                    lcts.latest_processed_message_time_stamp as lcts_LatestProcessedMessageTimeStamp,
                    lcts.vehicle_health_status_type as lcts_VehicleHealthStatusType,
                    lcts.latest_warning_class as lcts_LatestWarningClass,
                    lcts.latest_warning_number as lcts_LatestWarningNumber,
                    coalesce(lcts.latest_warning_type,'') as lcts_LatestWarningType,
                    lcts.latest_warning_timestamp as lcts_LatestWarningTimestamp,
                    lcts.latest_warning_position_latitude as lcts_LatestWarningPositionLatitude,
                    lcts.latest_warning_position_longitude as lcts_LatestWarningPositionLongitude,
                    coalesce(veh.vid,'') as veh_Vid,
                    coalesce(veh.registration_no,'') as veh_RegistrationNo,
                    coalesce(veh.name,'') as veh_name,
                    lps.id as lps_Id,
                    coalesce(lps.trip_id,'') as lps_TripId,
                    coalesce(lps.vin,'') as lps_Vin,
                    lps.gps_altitude as lps_GpsAltitude,
                    lps.gps_heading as lps_GpsHeading,
                    lps.gps_latitude as lps_GpsLatitude,
                    lps.gps_longitude as lps_GpsLongitude,
                    lps.co2_emission as lps_Co2Emission,
                    lps.fuel_consumption as lps_FuelConsumption,
                    lps.last_odometer_val as lps_LastOdometerVal,
                    latgeoadd.id as latgeoadd_LatestGeolocationAddressId,
                    coalesce(latgeoadd.address,'') as latgeoadd_LatestGeolocationAddress,
                    stageoadd.id as stageoadd_StartGeolocationAddressId,
                    coalesce(stageoadd.address,'') as stageoadd_StartGeolocationAddress,
                    wangeoadd.id as wangeoadd_LatestWarningGeolocationAddressId,
                    coalesce(wangeoadd.address,'') as wangeoadd_LatestWarningGeolocationAddress,
                    alertgeoadd.id as alertgeoadd_LatestAlertGeolocationAddressId,
                    coalesce(alertgeoadd.address,'') as alertgeoadd_LatestAlertGeolocationAddress,
                    tripal.id as tripal_Id,
					tripal.alert_id as tripal_AlertId,
					coalesce(tripal.vin,'') as tripal_Vin,
                    coalesce(tripal.trip_id,'') as tripal_TripId,
                    coalesce(tripal.name,'') as alertname,
                    coalesce(tripal.type,'') as alerttype,
                    tripal.alert_generated_time as AlertTime,
                    coalesce(tripal.urgency_level_type,'') as AlertLevel,
                    coalesce(tripal.category_type,'') as CategoryType,
                    tripal.latitude as AlertLatitude,
                    tripal.longitude as AlertLongitude
                    from CTE_Unique_latest_trip lcts
                    left join 
                    livefleet.livefleet_position_statistics lps
                    on lcts.trip_id = lps.trip_id and lcts.vin = lps.vin
                    left join master.vehicle veh
                    on lcts.vin=veh.vin                   
                    left join tripdetail.tripalert tripal
                    on lcts.vin=tripal.vin and  lcts.trip_id=tripal.trip_id 
                    left join master.geolocationaddress alertgeoadd
                    on TRUNC(CAST(tripal.latitude as numeric),4)= TRUNC(CAST(alertgeoadd.latitude as numeric),4) 
                    and TRUNC(CAST(tripal.longitude as numeric),4) = TRUNC(CAST(alertgeoadd.longitude as numeric),4)
                    left join master.geolocationaddress latgeoadd
                    on TRUNC(CAST(lcts.latest_received_position_lattitude as numeric),4)= TRUNC(CAST(latgeoadd.latitude as numeric),4) 
                    and TRUNC(CAST(lcts.latest_received_position_longitude as numeric),4) = TRUNC(CAST(latgeoadd.longitude as numeric),4)
                    left join master.geolocationaddress stageoadd
                    on TRUNC(CAST(lcts.start_position_lattitude as numeric),4)= TRUNC(CAST(stageoadd.latitude as numeric),4) 
                    and TRUNC(CAST(lcts.start_position_longitude as numeric),4) = TRUNC(CAST(stageoadd.longitude as numeric),4)
                    left join master.geolocationaddress wangeoadd
                    on TRUNC(CAST(lcts.latest_warning_position_latitude as numeric),4)= TRUNC(CAST(wangeoadd.latitude as numeric),4) 
                    and TRUNC(CAST(lcts.latest_warning_position_longitude as numeric),4) = TRUNC(CAST(wangeoadd.longitude as numeric),4)
                    where row_num=1 ";
                if (fleetOverviewFilter.DriverId.Count > 0)
                {
                    parameterFleetOverview.Add("@driverids", fleetOverviewFilter.DriverId);
                    queryFleetOverview += " and lcts.driver1_id = Any(@driverids) ";
                }
                if (fleetOverviewFilter.HealthStatus.Count > 0)
                {
                    parameterFleetOverview.Add("@healthstatus", fleetOverviewFilter.HealthStatus);
                    queryFleetOverview += " and lcts.vehicle_health_status_type = Any(@healthstatus) ";
                }
                if (fleetOverviewFilter.AlertCategory.Count > 0)
                {
                    //need to be implement in upcomming sprint 

                    parameterFleetOverview.Add("@alertcategory", fleetOverviewFilter.AlertCategory);
                    queryFleetOverview += " and tripal.category_type = Any(@alertcategory) ";
                }
                if (fleetOverviewFilter.AlertLevel.Count > 0)
                {
                    //need to be implement in upcomming sprint 
                    parameterFleetOverview.Add("@alertlevel", fleetOverviewFilter.AlertLevel);
                    queryFleetOverview += " and tripal.urgency_level_type = Any(@alertlevel) ";
                }
                IEnumerable<FleetOverviewResult> alertResult = await _dataMartdataAccess.QueryAsync<FleetOverviewResult>(queryFleetOverview, parameterFleetOverview);
                return repositoryMapper.GetFleetOverviewDetails(alertResult);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<FleetOverviewDetails>> GetFleetOverviewDetails_NeverMoved(FleetOverviewFilter fleetOverviewFilter)
        {
            List<FleetOverviewDetails> fleetOverviewDetails = new List<FleetOverviewDetails>();
            MapperRepo repositoryMapper = new MapperRepo();
            try
            {
                var parameterFleetOverview = new DynamicParameters();
                parameterFleetOverview.Add("@vins", fleetOverviewFilter.VINIds);
                //filter trip data by n days
                //parameterFleetOverview.Add("@days", string.Concat("'", fleetOverviewFilter.Days.ToString(), "d", "'"));
                parameterFleetOverview.Add("@days", fleetOverviewFilter.Days, System.Data.DbType.Int32);
                string queryFleetOverview = @"With CTE_Warnings_By_Vin as (SELECT 
                                lws.id,
                                lws.trip_id,
                                lws.vin,
                                lws.warning_time_stamp,
                                lws.warning_class,
                                lws.warning_number,
                                lws.latitude,
                                lws.longitude,
                                lws.heading,
                                lws.vehicle_health_status_type,
                                lws.vehicle_driving_status_type,
                                lws.driver1_id,
                                lws.warning_type,
                                lws.distance_until_next_service,
                                lws.odometer_val,
                                lws.lastest_processed_message_time_stamp,
                                lws.message_type,
                                coalesce(veh.vid,'') as veh_Vid,
                                coalesce(veh.registration_no,'') as veh_RegistrationNo,
                                coalesce(veh.name,'') as veh_name,
                                wangeoadd.id as wangeoadd_LatestWarningGeolocationAddressId,
                                coalesce(wangeoadd.address,'') as wangeoadd_LatestWarningGeolocationAddress,
                                rank() over (partition by (lws.vin,lws.warning_class,lws.warning_number)
                                order by warning_time_stamp desc) warningrank
                                FROM livefleet.livefleet_warning_statistics lws
                                left join master.vehicle veh
                                on lws.vin=veh.vin 
                                left join master.geolocationaddress wangeoadd
                                on TRUNC(CAST(lws.latitude as numeric),4)= TRUNC(CAST(wangeoadd.latitude as numeric),4) 
                                and TRUNC(CAST(lws.longitude as numeric),4) = TRUNC(CAST(wangeoadd.longitude as numeric),4)
                                where lws.vin = Any(@vins)  
                                and to_timestamp(lws.warning_time_stamp/1000)::date >= (now()::date - @days )
                                )
                                --select * from CTE_Warnings_By_Vin where warningrank =1 order by latest_warning_class 
                                ,CTE_Warnings_Vin_Rank as (
                                 select 
                                 *,
                                rank() over (partition by (vin)
                                order by id desc) row_num
                                 from CTE_Warnings_By_Vin 
                                 where warningrank =1 and warning_type='A'   
                                )
                                --select * from CTE_Warnings_Vin_Rank
                                ,CTE_Alerts_By_Vin as
                                (select 
                                tripal.id as tripal_Id,
                                tripal.alert_id as tripal_AlertId,
                                coalesce(tripal.vin,'') as tripal_Vin,
                                coalesce(tripal.trip_id,'') as tripal_TripId,
                                coalesce(tripal.name,'') as alertname,
                                coalesce(tripal.type,'') as alerttype,
                                tripal.alert_generated_time as AlertTime,
                                coalesce(tripal.urgency_level_type,'') as AlertLevel,
                                coalesce(tripal.category_type,'') as CategoryType,
                                tripal.latitude as AlertLatitude,
                                tripal.longitude as AlertLongitude,
                                alertgeoadd.id as alertgeoadd_LatestAlertGeolocationAddressId,
                                coalesce(alertgeoadd.address,'') as alertgeoadd_LatestAlertGeolocationAddress
                                from tripdetail.tripalert tripal
                                left join master.geolocationaddress alertgeoadd
                                on TRUNC(CAST(tripal.latitude as numeric),4)= TRUNC(CAST(alertgeoadd.latitude as numeric),4) 
                                and TRUNC(CAST(tripal.longitude as numeric),4) = TRUNC(CAST(alertgeoadd.longitude as numeric),4)
                                where tripal.vin = Any(@vins) 
                                and to_timestamp(tripal.alert_generated_time/1000)::date >= (now()::date -  @days)
                                 )
                                select
                                CWVR.id as lcts_id,
                                CWVR.trip_id,
                                CWVR.vin as lcts_vin,
                                'N' as lcts_VehicleDrivingStatusType,
                                case when warning_class >=4 and warning_class <= 7 then 'T'
                                when warning_class >=8 and warning_class <= 10 then 'V'
                                else 'N' end as lcts_VehicleHealthStatusType,                                
                                CWVR.warning_class as lcts_LatestWarningClass,
                                CWVR.warning_number as lcts_LatestWarningNumber,
                                coalesce(warning_type,'') as lcts_LatestWarningType,
                                CWVR.warning_time_stamp as lcts_LatestWarningTimestamp,
                                CWVR.latitude as lcts_LatestWarningPositionLatitude,
                                CWVR.longitude as lcts_LatestWarningPositionLongitude,
                                CWVR.wangeoadd_LatestWarningGeolocationAddressId,
                                CWVR.wangeoadd_LatestWarningGeolocationAddress,
                                CWVR.veh_Vid,
                                CWVR.veh_RegistrationNo,
                                CWVR.veh_name,
                                CABV.tripal_Id,
                                CABV.tripal_AlertId,
                                CABV.tripal_Vin,
                                CABV.tripal_TripId,
                                CABV.alertname,
                                CABV.alerttype,
                                CABV.AlertTime,
                                CABV.AlertLevel,
                                CABV.CategoryType,
                                CABV.AlertLatitude,
                                CABV.AlertLongitude,
                                CABV.alertgeoadd_LatestAlertGeolocationAddress,
                                CABV.alertgeoadd_LatestAlertGeolocationAddress
                                from 
                                CTE_Warnings_Vin_Rank CWVR 
                                left join 
                                CTE_Alerts_By_Vin CABV
                                ON CWVR.vin = CABV.tripal_Vin
                                AND CWVR.warningrank=1 and CWVR.warning_type='A'
                                where CWVR.row_num=1 
";
                if (fleetOverviewFilter.DriverId.Count > 0)
                {
                    parameterFleetOverview.Add("@driverids", fleetOverviewFilter.DriverId);
                    queryFleetOverview += " and driver1_id = Any(@driverids) ";
                }
                if (fleetOverviewFilter.HealthStatus.Count > 0)
                {
                    parameterFleetOverview.Add("@healthstatus", fleetOverviewFilter.HealthStatus);
                    queryFleetOverview += " and vehicle_health_status_type = Any(@healthstatus) ";
                }
                if (fleetOverviewFilter.AlertCategory.Count > 0)
                {
                    //need to be implement in upcomming sprint 

                    parameterFleetOverview.Add("@alertcategory", fleetOverviewFilter.AlertCategory);
                    queryFleetOverview += " and tripal.category_type = Any(@alertcategory) ";
                }
                if (fleetOverviewFilter.AlertLevel.Count > 0)
                {
                    //need to be implement in upcomming sprint 
                    parameterFleetOverview.Add("@alertlevel", fleetOverviewFilter.AlertLevel);
                    queryFleetOverview += " and tripal.urgency_level_type = Any(@alertlevel) ";
                }
                IEnumerable<FleetOverviewResult> alertResult = await _dataMartdataAccess.QueryAsync<FleetOverviewResult>(queryFleetOverview, parameterFleetOverview);
                return repositoryMapper.GetFleetOverviewDetails(alertResult);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<FleetOverviewDetails>> GetFleetOverviewDetails_NeverMoved_NoWarnings(FleetOverviewFilter fleetOverviewFilter)
        {
            List<FleetOverviewDetails> fleetOverviewDetails = new List<FleetOverviewDetails>();
            MapperRepo repositoryMapper = new MapperRepo();
            try
            {
                var parameterFleetOverview = new DynamicParameters();
                parameterFleetOverview.Add("@vins", fleetOverviewFilter.VINIds);
                //filter trip data by n days
                //parameterFleetOverview.Add("@days", string.Concat("'", fleetOverviewFilter.Days.ToString(), "d", "'"));
                string queryFleetOverview = @"select
                    veh.id as lcts_id,
                    '' as trip_id,
                    veh.vin as lcts_vin,
                    'N' as lcts_VehicleDrivingStatusType,
                    'N' as lcts_VehicleHealthStatusType,
                    0 as lcts_LatestWarningClass,
                    0 as lcts_LatestWarningNumber,
                    '' as lcts_LatestWarningType,
                    0 as lcts_LatestWarningTimestamp,
                    0 as lcts_LatestWarningPositionLatitude,
                    0 as lcts_LatestWarningPositionLongitude,
                    0 as wangeoadd_LatestWarningGeolocationAddressId,
                    '' as wangeoadd_LatestWarningGeolocationAddress,
                    0 as tripal_Id,
                    0 as tripal_AlertId,
                    '' as tripal_Vin,
                    '' as tripal_TripId,
                    '' as alertname,
                    '' as alerttype,
                    0 as AlertTime,
                    '' as AlertLevel,
                    '' as CategoryType,
                    0 as AlertLatitude,
                    0 as AlertLongitude,
                    0 as alertgeoadd_LatestAlertGeolocationAddress,
                    0 as alertgeoadd_LatestAlertGeolocationAddress,
                    coalesce(veh.vid,'') as veh_Vid,
                    coalesce(veh.registration_no,'') as veh_RegistrationNo,
                    coalesce(veh.name,'') as veh_name
                    FROM master.vehicle veh
                    where  veh.vin = Any(@vins)  ";
                IEnumerable<FleetOverviewResult> alertResult = await _dataMartdataAccess.QueryAsync<FleetOverviewResult>(queryFleetOverview, parameterFleetOverview);
                return repositoryMapper.GetFleetOverviewDetails(alertResult);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}