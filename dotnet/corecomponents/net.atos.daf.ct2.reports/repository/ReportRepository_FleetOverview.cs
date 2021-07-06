using System;
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

        public async Task<List<DriverFilter>> GetDriverList(List<string> vins)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@VehicleIds", vins);
            string queryDriverFilterPull = @"select dri.driver_id as DriverId
                                                ,dri.first_name as FirstName
                                                ,dri.last_name as LastName
                                                ,dri.organization_id as OrganizationId
                                                from livefleet.livefleet_current_trip_statistics cts
                                                inner join master.driver dri
                                                on cts.driver1_id=driver_id
                                                where cts.vin= ANY(@VehicleIds)";

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
                parameterFleetOverview.Add("@vin", fleetOverviewFilter.VINIds);
                string queryFleetOverview = @"select 
                    lcts.id as lcts_Id,
                    lcts.trip_id as lcts_TripId,
                    lcts.vin as lcts_Vin,
                    lcts.start_time_stamp as lcts_StartTimeStamp,
                    lcts.end_time_stamp as lcts_EndTimeStamp,
                    lcts.driver1_id as lcts_Driver1Id,
                    lcts.trip_distance as lcts_TripDistance,
                    lcts.driving_time as lcts_DrivingTime,
                    lcts.fuel_consumption as lcts_FuelConsumption,
                    lcts.vehicle_driving_status_type as lcts_VehicleDrivingStatusType,
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
                    lcts.latest_warning_type as lcts_LatestWarningType,
                    lcts.latest_warning_timestamp as lcts_LatestWarningTimestamp,
                    lcts.latest_warning_position_latitude as lcts_LatestWarningPositionLatitude,
                    lcts.latest_warning_position_longitude as lcts_LatestWarningPositionLongitude,
                    veh.created_at as veh_Vid,
                    veh.modified_at as veh_RegistrationNo,
                    dri.first_name as dri_FirstName,
                    dri.last_name as dri_LastName,
                    lps.id as lps_Id,
                    lps.trip_id as lps_TripId,
                    lps.vin as lps_Vin,
                    lps.gps_altitude as lps_GpsAltitude,
                    lps.gps_heading as lps_GpsHeading,
                    lps.gps_latitude as lps_GpsLatitude,
                    lps.gps_longitude as lps_GpsLongitude,
                    lps.co2_emission as lps_Co2Emission,
                    lps.fuel_consumption as lps_FuelConsumption,
                    lps.last_odometer_val as lps_LastOdometerVal,
                    latgeoadd.id as latgeoadd_LatestGeolocationAddressId,
                    latgeoadd.address as latgeoadd_LatestGeolocationAddress,
                    stageoadd.id as stageoadd_StartGeolocationAddressId,
                    stageoadd.address as stageoadd_StartGeolocationAddress,
                    wangeoadd.id as wangeoadd_LatestWarningGeolocationAddressId,
                    wangeoadd.address as wangeoadd_LatestWarningGeolocationAddress
                    from livefleet.livefleet_current_trip_statistics lcts
                    left join 
                    livefleet.livefleet_position_statistics lps
                    on lcts.trip_id = lps.trip_id and lcts.vin = lps.vin
                    left join master.vehicle veh
                    on lcts.vin=veh.vin
                    left join master.driver dri
                    on lcts.driver1_id=dri.driver_id
                    left join master.geolocationaddress latgeoadd
                    on TRUNC(CAST(lcts.latest_received_position_lattitude as numeric),4)= TRUNC(CAST(latgeoadd.latitude as numeric),4) 
                    and TRUNC(CAST(lcts.latest_received_position_longitude as numeric),4) = TRUNC(CAST(latgeoadd.longitude as numeric),4)
                    left join master.geolocationaddress stageoadd
                    on TRUNC(CAST(lcts.start_position_latitude as numeric),4)= TRUNC(CAST(stageoadd.latitude as numeric),4) 
                    and TRUNC(CAST(lcts.start_position_longitude as numeric),4) = TRUNC(CAST(stageoadd.longitude as numeric),4)
                    left join master.geolocationaddress wangeoadd
                    on TRUNC(CAST(lcts.latest_warning_position_latitude as numeric),4)= TRUNC(CAST(wangeoadd.latitude as numeric),4) 
                    and TRUNC(CAST(lcts.latest_warning_position_longitude as numeric),4) = TRUNC(CAST(wangeoadd.longitude as numeric),4)
                    where lcts.vin = Any(@vins) ";
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