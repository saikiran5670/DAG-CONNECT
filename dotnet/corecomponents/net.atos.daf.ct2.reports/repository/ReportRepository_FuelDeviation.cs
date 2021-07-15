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
	                    , fueldev.fuel_difference  as FuelDiffernce
                        , fueldev.latitude  as Latitude
                        , fueldev.longitude  as Longitude
	                    , fueldev.event_time   as EventTime
	                    , fueldev.odometer_val as Odometer
	                    , trpst.start_time_stamp as StartTimeStamp
	                    , trpst.end_time_stamp as EndTimeStamp
	                    , trpst.etl_gps_distance as Distance
	                    , trpst.idle_duration as IdleDuration
	                    , trpst.average_speed as AverageSpeed
	                    , trpst.average_weight as AverageWeight
                        , CASE WHEN trpst.start_position IS NULL THEN '' ELSE trpst.start_position END AS StartPosition
                        , CASE WHEN trpst.end_position IS NULL THEN '' ELSE trpst.end_position END AS EndPosition
	                    , trpst.etl_gps_fuel_consumed as FuelConsumed
	                    , trpst.etl_gps_driving_time as DrivingTime
	                    , trpst.no_of_alerts as Alerts
	                    , trpst.vin as VIN
	                    , CASE WHEN v.registration_no IS NULL THEN '' ELSE v.registration_no END as RegistrationNo
	                    , CASE WHEN v.name IS NULL THEN '' ELSE v.name END as VehicleName
                        , fueldev.geolocation_address_id as GeoLocationAddressId
	                    , CASE WHEN geoaddr.address IS NULL THEN '' ELSE geoaddr.address END as GeoLocationAddress
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
                    and TRUNC(CAST(geoaddr.longitude as numeric),4) = TRUNC(CAST(fueldev.longitude as numeric),4) ";

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
