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
                                         		  , da.code
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
                                         			GROUP BY da.driver_id, da.activity_date, da.code, da.duration, da.vin, dr.first_name, dr.last_name, da.end_time, da.start_time
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
                                         		  , code
                                         		FROM cte_dailyActivity
                                         			GROUP BY driverName, activity_date, driver_id, vin, activity_date, code, start_time, end_time 
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
                                         		  , code
                                         		  , max( CASE WHEN ( code = '0') THEN duration ELSE 0 END ) AS rest_time
                                         		  , max( CASE WHEN ( code = '1' ) THEN duration ELSE 0 END) AS available_time
                                         		  , max( CASE WHEN ( code = '2' ) THEN duration ELSE 0 END ) AS work_time
                                         		  , max( CASE WHEN ( code = '3' ) THEN duration ELSE 0 END ) AS drive_time
                                         		FROM
                                         			cte_mappedActivity
                                         			GROUP BY driverName, driver_id, vin, activity_date, code, start_time, end_time
                                         			ORDER BY driver_id
                                         	)
                                         SELECT
                                         	driverName
                                           , driver_id AS driverid
                                           , vin
                                           , activity_date AS activitydate
                                           , code
                                           , start_time                                                                          AS starttime
                                           , end_time                                                                            AS endtime
                                           , rest_time                                                                           AS resttime
                                           , available_time                                                                      AS availabletime
                                           , work_time                                                                           AS worktime
                                           , drive_time                                                                          AS drivetime
                                           , sum(COALESCE(available_time, 0) + COALESCE(work_time, 0) + COALESCE(drive_time, 0)) AS ServiceTime
                                         FROM
                                         	cte_pivotedtable
                                         	GROUP BY 	driverName , driver_id, vin, activity_date, code, rest_time, available_time, work_time, drive_time, start_time, end_time";

                List<DriversActivities> lstDriverActivities = (List<DriversActivities>)await _dataMartdataAccess.QueryAsync<DriversActivities>(queryActivities, parameterOfFilters);

                return lstDriverActivities?.Count() > 0 ? lstDriverActivities : new List<DriversActivities>();

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<List<Driver>> GetDriversByVIN(long StartDateTime, long EndDateTime, List<string> VIN)
        {
            var parameterOfReport = new DynamicParameters();
            parameterOfReport.Add("@FromDate", StartDateTime);
            parameterOfReport.Add("@ToDate", EndDateTime);
            parameterOfReport.Add("@Vins", VIN.ToArray());
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

        public async Task<object> GetReportSearchParameterByVIN(int ReportID, long StartDateTime, long EndDateTime, List<string> VIN, [Optional] string ReportView)
        {
            var parameterOfReport = new DynamicParameters();
            parameterOfReport.Add("@FromDate", StartDateTime);
            parameterOfReport.Add("@ToDate", EndDateTime);
            parameterOfReport.Add("@Vins", VIN.ToArray());
            // TODO:: Delete once sql View is in use
            _log.Info(ReportView);
            string queryDriversPull = GetReportQuery(ReportID, "@FromDate", "@ToDate", "@Vins");

            object lstDriver = await _dataMartdataAccess.QueryAsync(queryDriversPull, parameterOfReport);
            return lstDriver;
        }
        /// <summary>
        /// TODO :: Created this temp method till the SQL view creation get approval
        /// </summary>
        /// <param name="ReportId"></param>
        /// <param name="FromDateParameter"></param>
        /// <param name="EndDateParameter"></param>
        /// <param name="VINsParamter"></param>
        /// <param name="OptionalParameter"></param>
        /// <returns>Formated string with respective report related query.</returns>
        private static string GetReportQuery(int ReportId, string FromDateParameter, string EndDateParameter, string VINsParamter, [Optional] string OptionalParameter, [Optional] string ReportSQLView)
        {
            string _query;
            switch (ReportId)
            {
                case 1:
                    // For - Trip Report
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;

                case 2:
                    // For - Trip Tracing
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                case 3:
                    // For - Advanced Fleet Fuel Report
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                case 4:
                    // For - Fleet Fuel Report
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                case 5:
                    // For - Fleet Utilisation Report
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                case 6:
                    // For - Fuel Benchmarking
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                case 7:
                    // For - Fuel Deviation Report
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                case 8:
                    // For - Vehicle Performance Report
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                case 9:
                    // For - Drive Time Management
                    _query = @"SELECT da.vin VIN, da.driver_id DriverId, d.first_name FirstName, d.last_name LastName, da.activity_date ActivityDateTime FROM livefleet.livefleet_trip_driver_activity da Left join master.driver d on d.driver_id=da.driver_id WHERE (da.activity_date >= {0} AND da.activity_date <= {1}) and vin=ANY ({2}) GROUP BY da.driver_id, da.vin,d.first_name,d.last_name,da.activity_date ORDER BY da.driver_id DESC";
                    //_query = @"SELECT da.vin VIN, da.driver_id DriverId, d.first_name FirstName, d.last_name LastName, da.activity_date ActivityDateTime FROM livefleet.livefleet_trip_driver_activity da Left join master.driver d on d.driver_id=da.driver_id WHERE (da.activity_date >= {0} AND da.activity_date <= {1}) GROUP BY da.driver_id, da.vin,d.first_name,d.last_name,da.activity_date ORDER BY da.driver_id DESC";
                    _query = string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                case 10:
                    // For - 
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                case 11:
                    // For -  Schedule Report
                    _query = string.Empty;
                    string.Format(_query, FromDateParameter, EndDateParameter, VINsParamter, OptionalParameter);
                    break;
                default:
                    // Use this logic once VIEW implementation is done
                    _query = "SELECT * from {0}";
                    string.Format(_query, ReportSQLView);
                    break;

            }
            return _query;
        }

        #endregion
    }
}
