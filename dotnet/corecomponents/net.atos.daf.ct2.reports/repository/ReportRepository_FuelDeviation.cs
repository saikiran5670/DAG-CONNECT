using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        #region Fuel Deviation Report Table Details        
        public Task<IEnumerable<FuelDeviation>> GetFilteredFuelDeviation(FuelDeviationFilter fuelDeviationFilters)
        {
            try
            {
                string query = @"SELECT 
                           trpst .trip_id as TripId
                        ,  fueldev.id as FuelDeviationId
	                    , fueldev.fuel_event_type as FuelEventType
	                    , fueldev.vehicle_activity_type as VehicleActivityType
	                    , case when fueldev.fuel_event_type='I' then 'enumfueleventtype_increase' else 'enumfueleventtype_decrease' end as FuelEventTypeKey
	                    , case when fueldev.vehicle_activity_type='S' then 'enumvehicleactivitytype_stop' else 'enumvehicleactivitytype_running' end as VehicleActivityTypeKey
	                    , ROUND(fueldev.fuel_difference,2)  as FuelDiffernce
                        , fueldev.latitude  as Latitude
                        , fueldev.longitude  as Longitude
	                    , fueldev.event_time   as EventTime
	                    , fueldev.odometer_val as Odometer
	                    , trpst.start_time_stamp as StartTimeStamp
	                    , trpst.end_time_stamp as EndTimeStamp
	                    , trpst.etl_gps_distance as Distance
	                    , trpst.idle_duration as IdleDuration
	                    , ROUND(trpst.average_speed,2) as AverageSpeed
	                    , ROUND(trpst.average_weight,2) as AverageWeight
                        , startgeoaddr.id AS StartPositionId
                        , endgeoaddr.id AS EndPositionId
                        , coalesce(startgeoaddr.address,'') AS StartPosition
                        , coalesce(endgeoaddr.address,'') AS EndPosition
	                    , trpst.etl_gps_fuel_consumed as FuelConsumed
	                    , trpst.etl_gps_driving_time as DrivingTime
	                    , trpst.no_of_alerts as Alerts
	                    , trpst.vin as VIN
	                    , CASE WHEN v.registration_no IS NULL THEN '' ELSE v.registration_no END as RegistrationNo
	                    , CASE WHEN v.name IS NULL THEN '' ELSE v.name END as VehicleName
                        , geoaddr.id as GeoLocationAddressId
	                    , coalesce(geoaddr.address,'') as GeoLocationAddress
                        , trpst.start_position_lattitude AS StartPositionLattitude
	                    , trpst.start_position_longitude AS StartPositionLongitude
	                    , trpst.end_position_lattitude AS EndPositionLattitude
	                    , trpst.end_position_longitude AS EndPositionLongitude
                    from tripdetail.trip_statistics as trpst	 
	                     INNER JOIN livefleet.livefleet_trip_fuel_deviation as fueldev
	 	                    ON fueldev.trip_id = trpst.trip_id AND trpst.vin = Any(@vins)
	                                            AND (
		                                            trpst.end_time_stamp >= @StartDateTime
		                                            AND trpst.end_time_stamp <= @EndDateTime
		                                            )
	                     Left JOIN master.vehicle as v 
	 	                    ON v.vin = trpst.vin
	                     left JOIN master.geolocationaddress as geoaddr
                            on TRUNC(CAST(geoaddr.latitude as numeric),4)= TRUNC(CAST(fueldev.latitude as numeric),4) 
                               and TRUNC(CAST(geoaddr.longitude as numeric),4) = TRUNC(CAST(fueldev.longitude as numeric),4)
                         left JOIN master.geolocationaddress as startgeoaddr
                            on TRUNC(CAST(startgeoaddr.latitude as numeric),4)= TRUNC(CAST(trpst.start_position_lattitude as numeric),4) 
                               and TRUNC(CAST(startgeoaddr.longitude as numeric),4) = TRUNC(CAST(trpst.start_position_longitude as numeric),4)
                        left JOIN master.geolocationaddress as endgeoaddr
                            on TRUNC(CAST(endgeoaddr.latitude as numeric),4)= TRUNC(CAST(trpst.end_position_lattitude as numeric),4) 
                               and TRUNC(CAST(endgeoaddr.longitude as numeric),4) = TRUNC(CAST(trpst.end_position_longitude as numeric),4)";

                var parameter = new DynamicParameters();
                parameter.Add("@StartDateTime", fuelDeviationFilters.StartDateTime);
                parameter.Add("@EndDateTime", fuelDeviationFilters.EndDateTime);
                parameter.Add("@vins", fuelDeviationFilters.VINs.ToArray());
                return _dataMartdataAccess.QueryAsync<FuelDeviation>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Fuel Deviation Report Chartss  
        #endregion
    }
}
