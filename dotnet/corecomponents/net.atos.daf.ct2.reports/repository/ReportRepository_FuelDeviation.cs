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
                        , fueldev.latitude  as EventLatitude
                        , fueldev.longitude  as EventLongitude
                        , fueldev.heading  as EventHeading
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
	                    , (select count(1) from tripdetail.tripalert where trip_id = trpst .trip_id and type in ('P','L','T') ) as Alerts
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
        public Task<IEnumerable<FuelDeviationCharts>> GetFuelDeviationCharts(FuelDeviationFilter fuelDeviationFilters)
        {
            try
            {
                string query = @"SELECT count(distinct ld.id) as EventCount,
                                count(distinct ld.trip_id) as TripCount,
                                count(distinct ld.vin) as VehicleCount,
                                --fuel_event_type,
                                sum(case when fuel_event_type='I' then 1 else 0 END) as IncreaseEvent,
                                sum(case when fuel_event_type='D' then 1 else 0 END) as DecreaseEvent,
                                extract(epoch from (date_trunc('day', to_timestamp(event_time/1000)))) * 1000 as date 
                                FROM livefleet.livefleet_trip_fuel_deviation ld
                                join tripdetail.trip_statistics as trpst
                                on trpst.trip_id=ld.trip_id
                                --Join Master.vehicle v
                                --on ld.vin=v.vin
                                where (start_time_stamp >= @StartDateTime 
	                                 and end_time_stamp<= @EndDateTime) and 
	                                 trpst.vin = Any(@vins)
                                Group by date_trunc('day', to_timestamp(event_time/1000))
                                --,fuel_event_type";

                var parameter = new DynamicParameters();
                parameter.Add("@StartDateTime", fuelDeviationFilters.StartDateTime);
                parameter.Add("@EndDateTime", fuelDeviationFilters.EndDateTime);
                parameter.Add("@vins", fuelDeviationFilters.VINs.ToArray());
                return _dataMartdataAccess.QueryAsync<FuelDeviationCharts>(query, parameter);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
