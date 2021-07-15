using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            try
            {


                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", activityFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", activityFilters.EndDateTime);
                parameterOfFilters.Add("@Vins", activityFilters.VIN);
                parameterOfFilters.Add("@DriverIDs", activityFilters.DriverId);
                //parameterOfFilters.Add("@Vins", string.Join(",", activityFilters.VIN));
                //parameterOfFilters.Add("@DriverIDs", string.Join(",", activityFilters.DriverId));
                string queryActivities = @"WITH cte_dailyActivity AS (
                                         		SELECT
                                         			dr.first_name || ' ' || dr.last_name AS driverName
                                         		  , da.vin
                                         		  , da.driver_id
                                         		  , da.activity_date
                                         		  , da.start_time
                                         		  , da.end_time
                                         		  , da.logical_code
                                         		  , sum(da.duration) AS Duration
                                         		  , TO_TIMESTAMP(da.duration)
                                         		FROM livefleet.livefleet_trip_driver_activity da
                                         				JOIN master.driver dr ON dr.driver_id = da.driver_id
                                         			WHERE
                                         				da.activity_date     >= @FromDate
                                         				AND da.activity_date <= @ToDate
                                                        AND da.driver_id = ANY( @DriverIDs )
                                         				AND da.vin = ANY( @Vins )
                                         				----AND da.driver_id IN ( @DriverIDs )
                                         				----AND da.vin IN ( @Vins )
                                         			GROUP BY da.driver_id, da.activity_date, da.logical_code, da.duration, da.vin, dr.first_name, dr.last_name, da.end_time, da.start_time
                                         			ORDER BY da.activity_date DESC
                                         	)
                                           , cte_mappedActivity AS (
                                         		SELECT
                                         			driverName
                                         		  , driver_id
                                         		  , vin
                                         		  , activity_date
                                           	      , start_time 
                                         	      , end_time
                                         		  , sum(duration) AS duration
                                         		  , logical_code
                                         		FROM cte_dailyActivity
                                         			GROUP BY driverName, activity_date, driver_id, vin, activity_date, logical_code, start_time, end_time 
                                         			ORDER BY driver_id
                                         	)
                                           , cte_pivotedtable AS (
                                         		SELECT
                                         			driverName
                                         		  , driver_id
                                         		  , vin
                                         		  , activity_date
                                         	      , start_time 
                                         	      , end_time
                                         		  , logical_code
                                         		  , max( CASE WHEN ( logical_code = '0') THEN duration ELSE 0 END ) AS rest_time
                                         		  , max( CASE WHEN ( logical_code = '1' ) THEN duration ELSE 0 END) AS available_time
                                         		  , max( CASE WHEN ( logical_code = '2' ) THEN duration ELSE 0 END ) AS work_time
                                         		  , max( CASE WHEN ( logical_code = '3' ) THEN duration ELSE 0 END ) AS drive_time
                                         		FROM
                                         			cte_mappedActivity
                                         			GROUP BY driverName, driver_id, vin, activity_date, logical_code, start_time, end_time
                                         			ORDER BY driver_id
                                         	)
                                         SELECT
                                         	driverName
                                           , driver_id AS driverid
                                           , vin
                                           , activity_date AS activitydate
                                           , logical_code AS code
                                           , start_time                                                                          AS starttime
                                           , end_time                                                                            AS endtime
                                           , rest_time                                                                           AS resttime
                                           , available_time                                                                      AS availabletime
                                           , work_time                                                                           AS worktime
                                           , drive_time                                                                          AS drivetime
                                           , sum(COALESCE(available_time, 0) + COALESCE(work_time, 0) + COALESCE(drive_time, 0)) AS ServiceTime
                                         FROM
                                         	cte_pivotedtable
                                         	GROUP BY 	driverName , driver_id, vin, activity_date, logical_code, rest_time, available_time, work_time, drive_time, start_time, end_time";

                List<DriversActivities> lstDriverActivities = (List<DriversActivities>)await _dataMartdataAccess.QueryAsync<DriversActivities>(queryActivities, parameterOfFilters);

                return lstDriverActivities?.Count() > 0 ? lstDriverActivities : new List<DriversActivities>();

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<List<Driver>> GetDriversByVIN(long startDateTime, long endDateTime, List<string> vin)
        {
            var parameterOfReport = new DynamicParameters();
            parameterOfReport.Add("@FromDate", startDateTime);
            parameterOfReport.Add("@ToDate", endDateTime);
            parameterOfReport.Add("@Vins", vin.ToArray());
            string queryDriversPull = @"SELECT da.vin VIN,
                                               da.driver_id DriverId,
                                               d.first_name FirstName,
                                               d.last_name LastName,
                                               da.activity_date ActivityDateTime
                                            FROM livefleet.livefleet_trip_driver_activity da
                                            Left join master.driver d on d.driver_id=da.driver_id
                                            WHERE (da.activity_date >= @FromDate AND da.activity_date <= @ToDate) and vin=ANY (@Vins)
                                            GROUP BY da.driver_id, da.vin,d.first_name,d.last_name,da.activity_date
                                            ORDER BY da.driver_id DESC ";

            List<Driver> lstDriver = (List<Driver>)await _dataMartdataAccess.QueryAsync<Driver>(queryDriversPull, parameterOfReport);
            if (lstDriver?.Count() > 0)
            {
                return lstDriver;
            }
            else
            {
                return new List<Driver>();
            }
        }

        public async Task<List<DriverActivityChart>> GetDriversActivityChartDetails(DriverActivityChartFilter activityFilters)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", activityFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", activityFilters.EndDateTime);
                parameterOfFilters.Add("@DriverId", activityFilters.DriverId);
                string queryActivities = @"SELECT
                                           	activity_date as activitDate
                                             , code
                                             , SUM(duration) as Duration
                                             , start_time as starttime
                                             , end_time as endtime
                                           FROM
                                              livefleet.livefleet_trip_driver_activity
                                           WHERE 
                                                start_time >= @FromDate
                                           	    AND end_time <= @ToDate
                                           	    AND driver_id = @DriverId
                                           GROUP BY
                                               code
                                             , date_trunc('day', to_timestamp(activity_date/1000)) 
                                             , activity_date
                                             , start_time
                                             , end_time";

                List<DriverActivityChart> lstDriverActivitiesChartData = (List<DriverActivityChart>)await _dataMartdataAccess.QueryAsync<DriverActivityChart>(queryActivities, parameterOfFilters);

                return lstDriverActivitiesChartData;

            }
            catch (System.Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
