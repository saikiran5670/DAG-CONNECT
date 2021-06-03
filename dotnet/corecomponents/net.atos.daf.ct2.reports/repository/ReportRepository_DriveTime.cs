using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
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
                                            				AND da.vin IN ( @Vins ) --AND da.vin IS NULL
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

            if (lstDriverActivities?.Count() > 0)
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
