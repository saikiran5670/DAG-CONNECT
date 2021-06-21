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
        #region Get Vins from data mart trip_statistics
        public Task<IEnumerable<VehicleFromTripDetails>> GetVinsFromTripStatistics(IEnumerable<string> vinList)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@fromdate", UTCHandling.GetUTCFromDateTime(DateTime.Now.AddDays(-90)));
                parameter.Add("@todate", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@vins", vinList.ToArray());
                var query = @"SELECT DISTINCT vin,start_time_stamp AS StartTimeStamp,
                                     end_time_stamp AS EndTimeStamp FROM tripdetail.trip_statistics 
                              WHERE end_time_stamp >= @fromdate AND end_time_stamp <= @todate AND 
                                     vin = Any(@vins)";
                return _dataMartdataAccess.QueryAsync<VehicleFromTripDetails>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Trip Report Table Details

        /// <summary>
        /// Fetch Filtered trips along with Live Fleet Position
        /// </summary>
        /// <param name="TripFilters"></param>
        /// <returns>List of Trips Data with LiveFleet attached under *LiveFleetPosition* property</returns>
        public async Task<List<TripDetails>> GetFilteredTripDetails(TripFilterRequest TripFilters,
                                                                    bool IsLiveFleetRequired = true)
        {
            try
            {
                List<TripDetails> lstTripEntityResponce = new List<TripDetails>();
                string query = string.Empty;
                query = @"SELECT id
	                        ,trip_id AS tripId
	                        ,vin AS VIN
	                        ,start_time_stamp AS StartTimeStamp
	                        ,end_time_stamp AS EndTimeStamp
	                        ,veh_message_distance AS Distance
	                        ,idle_duration AS IdleDuration
	                        ,average_speed AS AverageSpeed
	                        ,average_weight AS AverageWeight
	                        ,last_odometer AS Odometer
                            ,CASE WHEN start_position IS NULL THEN '' ELSE start_position END AS StartPosition
                            ,CASE WHEN end_position IS NULL THEN '' ELSE end_position END AS EndPosition
	                        ,start_position_lattitude AS StartPositionLattitude
	                        ,start_position_longitude AS StartPositionLongitude
	                        ,end_position_lattitude AS EndPositionLattitude
	                        ,end_position_longitude AS EndPositionLongitude
	                        ,fuel_consumption AS FuelConsumed
	                        ,veh_message_driving_time AS DrivingTime
	                        ,no_of_alerts AS Alerts
	                        ,no_of_events AS Events
	                        ,(fuel_consumption / 100) AS FuelConsumed100km
                        FROM tripdetail.trip_statistics
                        WHERE vin = @vin
	                        AND (
		                        start_time_stamp >= @StartDateTime
		                        AND end_time_stamp <= @EndDateTime
		                        )";

                var parameter = new DynamicParameters();
                parameter.Add("@StartDateTime", TripFilters.StartDateTime);
                parameter.Add("@EndDateTime", TripFilters.EndDateTime);
                parameter.Add("@vin", TripFilters.VIN);

                List<TripDetails> data = (List<TripDetails>)await _dataMartdataAccess.QueryAsync<TripDetails>(query, parameter);
                if (data?.Count > 0 && IsLiveFleetRequired)
                {

                    // new way To pull respective trip fleet position (One DB call for batch of 1000 trips)
                    string[] TripIds = data.Select(item => item.TripId).ToArray();
                    List<LiveFleetPosition> lstLiveFleetPosition = await GetLiveFleetPosition(TripIds);
                    if (lstLiveFleetPosition.Count > 0)
                        foreach (TripDetails trip in data)
                        {
                            trip.LiveFleetPosition = lstLiveFleetPosition.Where(fleet => fleet.TripId == trip.TripId).ToList();
                        }

                    /** Old way To pull respective trip fleet position
                    foreach (var item in data)
                    {
                        await GetLiveFleetPosition(item);
                    }
                    */
                    lstTripEntityResponce = data.ToList();
                }
                return lstTripEntityResponce;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //TODO :: Remove this method after implementation of new way to Live Fleet Position
        /// <summary>
        /// Pull Live Fleet positions with specific (one) trip details
        /// </summary>
        /// <param name="Trip"></param>
        /// <returns></returns>
        private async Task<List<LiveFleetPosition>> GetLiveFleetPosition(TripDetails Trip)
        {
            var parameterPosition = new DynamicParameters();
            parameterPosition.Add("@vin", Trip.VIN);
            parameterPosition.Add("@trip_id", Trip.TripId);
            string queryPosition = @"select id, 
                              vin,
                              gps_altitude, 
                              gps_heading,
                              gps_latitude,
                              gps_longitude
                              from livefleet.livefleet_position_statistics
                              where vin=@vin and trip_id = @trip_id order by id desc";
            var PositionData = await _dataMartdataAccess.QueryAsync<LiveFleetPosition>(queryPosition, parameterPosition);
            List<LiveFleetPosition> lstLiveFleetPosition = new List<LiveFleetPosition>();

            if (PositionData.Count() > 0)
            {
                foreach (var positionData in PositionData)
                {

                    LiveFleetPosition objLiveFleetPosition = new LiveFleetPosition
                    {
                        GpsAltitude = positionData.GpsAltitude,
                        GpsHeading = positionData.GpsHeading,
                        GpsLatitude = positionData.GpsLatitude,
                        GpsLongitude = positionData.GpsLongitude,
                        Id = positionData.Id
                    };
                    lstLiveFleetPosition.Add(objLiveFleetPosition);
                }
            }
            return lstLiveFleetPosition;
        }

        private async Task<List<LiveFleetPosition>> GetLiveFleetPosition(String[] TripIds)
        {
            try
            {
                //Creating chunk of 1000 trip ids because IN clause support till 1000 paramters only
                var combineTrips = CreateChunks(TripIds);

                List<LiveFleetPosition> lstLiveFleetPosition = new List<LiveFleetPosition>();
                if (combineTrips.Count > 0)
                {
                    foreach (var item in combineTrips)
                    {
                        // Collecting all batch to add under respective trip
                        lstLiveFleetPosition.AddRange(await GetFleetOfTripWithINClause(item.ToArray()));
                    }
                }
                return lstLiveFleetPosition;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Live Fleet Position as per trip given Trip id with IN clause (Optimized pull opration)
        /// </summary>
        /// <param name="CommaSparatedTripIDs"> Comma Sparated Trip IDs (max 1000 ids)</param>
        /// <returns>List of LiveFleetPosition Object</returns>
        private async Task<List<LiveFleetPosition>> GetFleetOfTripWithINClause(string[] CommaSparatedTripIDs)
        {
            try
            {
                var parameterPosition = new DynamicParameters();
                parameterPosition.Add("@trip_id", CommaSparatedTripIDs);
                string queryPosition = @"select id, 
                                         vin,
                                    	 trip_id as Tripid,
                                         gps_altitude as GpsAltitude, 
                                         gps_heading as GpsHeading,
                                         gps_latitude as GpsLatitude,
                                         gps_longitude as GpsLongitude
                                    from livefleet.livefleet_position_statistics
                                    where trip_id = ANY (@trip_id) order by id desc";
                List<LiveFleetPosition> lstLiveFleetPosition = (List<LiveFleetPosition>)await _dataMartdataAccess.QueryAsync<LiveFleetPosition>(queryPosition, parameterPosition);

                if (lstLiveFleetPosition.Count() > 0)
                {
                    return lstLiveFleetPosition;
                }
                else
                {
                    return new List<LiveFleetPosition>();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        #region Generic code to Prepare In query String

        /// <summary>
        ///   Create Batch of values on dynamic chunk size
        /// </summary>
        /// <param name="ArrayForChuk">Array of IDs or values for creating batch for e.g. Batch of 100. </param>
        /// <returns>List of all batchs including comma separated id in one item</returns>
        private List<IEnumerable<string>> CreateChunks(string[] ArrayForChuk)
        {
            // Creating batch of 1000 ids as IN clause support only 1000 parameters
            var TripChunks = Common.CommonExtention.Split<string>(ArrayForChuk, 1000).ToList();


            return TripChunks;
        }

        #endregion


        #endregion
    }
}
