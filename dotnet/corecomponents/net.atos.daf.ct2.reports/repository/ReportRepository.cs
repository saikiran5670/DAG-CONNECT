using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.reports.repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        private static readonly log4net.ILog _log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReportRepository(IDataAccess dataAccess
                                , IDataMartDataAccess dataMartdataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartdataAccess = dataMartdataAccess;
        }

        #region Select User Preferences
        public Task<IEnumerable<UserPrefernceReportDataColumn>> GetUserPreferenceReportDataColumn(int reportId,
                                                                                                  int accountId,
                                                                                                  int OrganizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_id", reportId);
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_id", OrganizationId);
                #region Query Select User Preferences
                var query = @"SELECT d.id as DataAtrributeId,d.name as Name,d.description as Description,d.type as Type,
	                                 d.key as Key,case when rp.state is null then 'I' else rp.state end as State, rp.id as ReportReferenceId, rp.chart_type as ChartType, rp.type as ReportReferenceType
                              FROM  master.reportattribute rd     
                                    INNER JOIN master.dataattribute d  	 ON rd.report_id = @report_id and d.id =rd.data_attribute_id 
                                    LEFT JOIN master.reportpreference rp ON rp.account_id = @account_id and rp.organization_id = @organization_id 
										                                    and rp.report_id = @report_id  and rp.report_id = rd.report_id 
	   									                                    and rp.data_attribute_id = rd.data_attribute_id 
                              WHERE rd.report_id = @report_id";
                #endregion
                return _dataAccess.QueryAsync<UserPrefernceReportDataColumn>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<UserPrefernceReportDataColumn>> GetRoleBasedDataColumn(int reportId,
                                                                                       int accountId,
                                                                                       int OrganizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_id", reportId);
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_id", OrganizationId);
                #region Query RoleBasedDataColumn
                var query = @"SELECT d.id as DataAtrributeId,d.name as Name,d.description as Description,d.type as Type,
	                                 d.key as Key,case when t.State = 'A' then 'A' else 'I' end as State, null as ReportReferenceId, null as ChartType, null as ReportReferenceType
                              FROM master.reportattribute rd     
                              INNER JOIN master.dataattribute d  ON rd.report_id = @report_id and d.id =rd.data_attribute_id 
		                      LEFT JOIN ( SELECT da.id,'A' as State
					                      FROM master.report r 
						                     INNER JOIN master.reportattribute ra ON ra.report_id = @report_id and ra.report_id = r.id
						                     INNER JOIN master.dataattribute da ON da.id = ra.data_attribute_id 
						                     INNER JOIN master.DataAttributeSetAttribute dasa ON dasa.data_attribute_id = da.id
						                     INNER JOIN master.DataAttributeSet das ON das.id = dasa.data_attribute_set_id and das.state = 'A' and das.is_exlusive = false
						                     INNER JOIN master.Feature f ON f.data_attribute_set_id = das.id AND f.state = 'A' and f.type = 'D'
						                     INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_id = f.id
						                     INNER JOIN master.FeatureSet fset ON fsf.feature_set_id = fset.id AND fset.state = 'A'
						                     INNER JOIN master.Role ro ON ro.feature_set_id = fset.id AND ro.state = 'A'
						                     INNER JOIN master.AccountRole ar ON ro.id = ar.role_id and ar.organization_id = @organization_id
						                     INNER JOIN master.account acc ON  acc.id = @account_id AND acc.id = ar.account_id AND acc.state = 'A'
	 			                          WHERE acc.id = @account_id AND ar.Organization_id = @organization_id AND r.id = @report_id
                                        ) t 
		                                ON t.id = d.id
                              WHERE  rd.report_id = @report_id";
                #endregion
                return _dataAccess.QueryAsync<UserPrefernceReportDataColumn>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Create Preference
        public async Task<int> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceRequest)
        {
            _dataAccess.connection.Open();
            string queryInsert = @"INSERT INTO master.reportpreference
                                    (organization_id,account_id, report_id, type, data_attribute_id,state,chart_type,created_at,modified_at)
                             VALUES (@organization_id,@account_id,@report_id,@type,@data_attribute_id,@state,@chart_type,@created_at, @modified_at)";

            string queryDelete = @"DELETE FROM master.reportpreference
                                  WHERE organization_id=@organization_id and account_id=@account_id AND report_id=@report_id";
            int rowsEffected = 0; var userPreference = new DynamicParameters();
            userPreference.Add("@account_id", objUserPreferenceRequest.AccountId);
            userPreference.Add("@report_id", objUserPreferenceRequest.ReportId);
            userPreference.Add("@organization_id", objUserPreferenceRequest.OrganizationId);
            userPreference.Add("@type", objUserPreferenceRequest.Type);
            userPreference.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            userPreference.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            userPreference.Add("@chart_type", objUserPreferenceRequest.ChartType);

            using (var transactionScope = _dataAccess.connection.BeginTransaction())
            {
                try
                {
                    await _dataAccess.ExecuteAsync(queryDelete, userPreference);
                    for (int i = 0; i < objUserPreferenceRequest.AtributesShowNoShow.Count; i++)
                    {
                        userPreference.Add("@data_attribute_id", objUserPreferenceRequest.AtributesShowNoShow[i].DataAttributeId);
                        userPreference.Add("@state", objUserPreferenceRequest.AtributesShowNoShow[i].State);
                        rowsEffected = await _dataAccess.ExecuteAsync(queryInsert, userPreference);
                    }
                    transactionScope.Commit();
                }
                catch (Exception ex)
                {
                    _log.Info($"CreateUserPreference method in repository failed : {Newtonsoft.Json.JsonConvert.SerializeObject(objUserPreferenceRequest)}");
                    _log.Error(ex.ToString());
                    transactionScope.Rollback();
                    rowsEffected = 0;
                }
                finally
                {
                    _dataAccess.connection.Close();
                }
            }
            return rowsEffected;
        }
        #endregion

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
        public async Task<List<TripDetails>> GetFilteredTripDetails(TripFilterRequest TripFilters)
        {
            try
            {
                List<TripDetails> lstTripEntityResponce = new List<TripDetails>();
                string query = string.Empty;
                query = @"SELECT id
	                        ,trip_id AS tripId
	                        ,vin AS VIN
	                        ,start_time_stamp AS StartDate
	                        ,end_time_stamp AS EndDate
	                        ,veh_message_distance AS Distance
	                        ,idle_duration AS IdleDuration
	                        ,average_speed AS AverageSpeed
	                        ,average_weight AS AverageWeight
	                        ,last_odometer AS Odometer
                            ,CASE WHEN start_position IS NULL THEN 'NA' ELSE start_position END AS StartPosition
                            ,CASE WHEN end_position IS NULL THEN 'NA' ELSE end_position END AS EndPosition
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
                if (data?.Count > 0)
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

                    LiveFleetPosition objLiveFleetPosition = new LiveFleetPosition();
                    objLiveFleetPosition.GpsAltitude = positionData.GpsAltitude;
                    objLiveFleetPosition.GpsHeading = positionData.GpsHeading;
                    objLiveFleetPosition.GpsLatitude = positionData.GpsLatitude;
                    objLiveFleetPosition.GpsLongitude = positionData.GpsLongitude;
                    objLiveFleetPosition.Id = positionData.Id;
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
                List<string> combineTrips = CreateChunks(TripIds);

                List<LiveFleetPosition> lstLiveFleetPosition = new List<LiveFleetPosition>();
                if (combineTrips.Count > 0)
                {
                    foreach (var item in combineTrips)
                    {
                        // Collecting all batch to add under respective trip
                        lstLiveFleetPosition.AddRange(await GetFleetOfTripWithINClause(item));
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
        private async Task<List<LiveFleetPosition>> GetFleetOfTripWithINClause(string CommaSparatedTripIDs)
        {
            var parameterPosition = new DynamicParameters();
            parameterPosition.Add("@trip_id", CommaSparatedTripIDs);
            string queryPosition = @"select id, 
                                         vin,
                                    	 trip_id as tripid,
                                         gps_altitude, 
                                         gps_heading,
                                         gps_latitude,
                                         gps_longitude
                                    from livefleet.livefleet_position_statistics
                                    where trip_id IN (@trip_id)
                                    order by id desc";
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

        #region Generic code to Prepare In query String

        /// <summary>
        ///   Create Batch of values on dynamic chunk size
        /// </summary>
        /// <param name="ArrayForChuk">Array of IDs or values for creating batch for e.g. Batch of 100. </param>
        /// <returns>List of all batchs including comma separated id in one item</returns>
        private List<string> CreateChunks(string[] ArrayForChuk)
        {
            // Creating batch of 1000 ids as IN clause support only 1000 parameters
            var TripChunks = Common.CommonExtention.Split<string>(ArrayForChuk, 1000);
            List<string> combineTrips = new List<string>();
            foreach (var chunk in TripChunks)
            {
                combineTrips.Add(string.Join(",", chunk));
            }

            return combineTrips;
        }

        #endregion


        #endregion

        #region Driver Time management Report

        public async Task<List<DriversActivities>> GetDriversActivity(DriverActivityFilter activityFilters)
        {
            var parameterOfFilters = new DynamicParameters();
            parameterOfFilters.Add("@FromDate", activityFilters.StartDateTime);
            parameterOfFilters.Add("@ToDate", activityFilters.EndDateTime);
            parameterOfFilters.Add("@Vins", string.Join(",", activityFilters.VIN));
            parameterOfFilters.Add("@DriverIDs", string.Join(",", activityFilters.DriverId));
            string queryActivities = @"WITH cte_dailyActivity AS (
                                            		SELECT
                                            			dr.first_name || ' ' || dr.last_name AS driverName
                                            		  , da.vin
                                            		  , da.driver_id
                                            		  , da.activity_date
                                            		  , da.code
                                            		  , sum(da.duration) AS Duration
                                            		  , TO_TIMESTAMP(da.duration)
                                            		FROM livefleet.livefleet_trip_driver_activity da
                                            				JOIN master.driver dr ON dr.driver_id = da.driver_id
                                            			WHERE
                                            				da.activity_date     >= @FromDate
                                            				AND da.activity_date <= @ToDate
                                            				AND da.driver_id IN ( @DriverIDs )
                                            				----AND da.vin IN ( @Vins ) --AND da.vin IS NULL
                                            			GROUP BY da.driver_id, da.activity_date, da.code, da.duration, da.vin, dr.first_name, dr.last_name
                                            			ORDER BY da.activity_date DESC
                                            	)
                                              , cte_mappedActivity AS (
                                            		SELECT
                                            			driverName
                                            		  , driver_id
                                            		  , vin
                                            		  , activity_date
                                            		  , sum(duration) AS duration
                                            		  , code
                                            		FROM cte_dailyActivity
                                            			GROUP BY driverName, activity_date, driver_id, vin, activity_date, code
                                            			ORDER BY driver_id
                                            	)
                                              , cte_pivotedtable AS (
                                            		SELECT
                                            			driverName
                                            		  , driver_id
                                            		  , vin
                                            		  , activity_date
                                            		  , code
                                            		  , max( CASE WHEN ( code = '0') THEN duration ELSE 0 END ) AS rest_time
                                            		  , max( CASE WHEN ( code = '1' ) THEN duration ELSE 0 END) AS available_time
                                            		  , max( CASE WHEN ( code = '2' ) THEN duration ELSE 0 END ) AS work_time
                                            		  , max( CASE WHEN ( code = '3' ) THEN duration ELSE 0 END ) AS drive_time
                                            		FROM
                                            			cte_mappedActivity
                                            			GROUP BY driverName, driver_id, vin, activity_date, code
                                            			ORDER BY driver_id
                                            	)
                                            SELECT
                                            	driverName
                                              , driver_id AS driverid
                                              , vin
                                              , activity_date AS activitydate
                                              , code
                                              , rest_time                                                                           AS resttime
                                              , available_time                                                                      AS availabletime
                                              , work_time                                                                           AS worktime
                                              , drive_time                                                                          AS drivetime
                                              , sum(COALESCE(available_time, 0) + COALESCE(work_time, 0) + COALESCE(drive_time, 0)) AS ServiceTime
                                            FROM
                                            	cte_pivotedtable
                                            	GROUP BY 	driverName , driver_id, vin, activity_date, code, rest_time, available_time, work_time, drive_time";

            List<DriversActivities> lstDriverActivities = (List<DriversActivities>)await _dataMartdataAccess.QueryAsync<DriversActivities>(queryActivities, parameterOfFilters);

            if (lstDriverActivities.Count() > 0)
            {
                return lstDriverActivities;
            }
            else
            {
                return new List<DriversActivities>();
            }
        }

        #endregion
    }
}
