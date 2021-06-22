using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.utilities;
using System.Runtime.InteropServices;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
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
        public Task<IEnumerable<ReportDetails>> GetReportDetails()
        {
            try
            {
                var query = @"select id as Id,name as Name, key as Key from master.report";
                return _dataAccess.QueryAsync<ReportDetails>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }
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
	                                 d.key as Key,case when rp.state is null then 'I' else rp.state end as State, rp.id as ReportReferenceId, rp.chart_type as ChartType, rp.type as ReportReferenceType, rp.threshold_limit_type as ThresholdType, rp.threshold_value as ThresholdValue
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
            _dataAccess.Connection.Open();
            string queryInsert = @"INSERT INTO master.reportpreference
                                    (organization_id,account_id, report_id, type, data_attribute_id,state,chart_type,created_at,modified_at,threshold_limit_type,threshold_value)
                             VALUES (@organization_id,@account_id,@report_id,@type,@data_attribute_id,@state,@chart_type,@created_at, @modified_at,@threshold_type,@threshold_value)";

            string queryDelete = @"DELETE FROM master.reportpreference
                                  WHERE organization_id=@organization_id and account_id=@account_id AND report_id=@report_id";
            int rowsEffected = 0; var userPreference = new DynamicParameters();
            userPreference.Add("@account_id", objUserPreferenceRequest.AccountId);
            userPreference.Add("@report_id", objUserPreferenceRequest.ReportId);
            userPreference.Add("@organization_id", objUserPreferenceRequest.OrganizationId);
            userPreference.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            userPreference.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

            using (var transactionScope = _dataAccess.Connection.BeginTransaction())
            {
                try
                {
                    await _dataAccess.ExecuteAsync(queryDelete, userPreference);
                    for (int i = 0; i < objUserPreferenceRequest.AtributesShowNoShow.Count; i++)
                    {
                        userPreference.Add("@data_attribute_id", objUserPreferenceRequest.AtributesShowNoShow[i].DataAttributeId);
                        userPreference.Add("@state", objUserPreferenceRequest.AtributesShowNoShow[i].State);
                        userPreference.Add("@type", objUserPreferenceRequest.AtributesShowNoShow[i].Type);
                        userPreference.Add("@chart_type", objUserPreferenceRequest.AtributesShowNoShow[i].ChartType);
                        userPreference.Add("@threshold_type", objUserPreferenceRequest.AtributesShowNoShow[i].ThresholdType);
                        userPreference.Add("@threshold_value", objUserPreferenceRequest.AtributesShowNoShow[i].ThresholdValue);
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
                    _dataAccess.Connection.Close();
                }
            }
            return rowsEffected;
        }
        #endregion

        #region - GetReportQuery

        public async Task<object> GetReportSearchParameterByVIN(int reportID, long startDateTime, long endDateTime, List<string> vin, [Optional] string reportView)
        {
            var parameterOfReport = new DynamicParameters();
            parameterOfReport.Add("@FromDate", startDateTime);
            parameterOfReport.Add("@ToDate", endDateTime);
            parameterOfReport.Add("@Vins", vin.ToArray());
            // TODO:: Delete once sql View is in use
            _log.Info(reportView);
            string queryDriversPull = GetReportQuery(reportID, "@FromDate", "@ToDate", "@Vins");

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
        private static string GetReportQuery(int reportId, string fromDateParameter, string endDateParameter, string vinssParamter, [Optional] string optionalParameter, [Optional] string reportSQLView)
        {
            string query;
            switch (reportId)
            {
                case 1:
                    // For - Trip Report
                    query = string.Empty;
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;

                case 2:
                    // For - Trip Tracing
                    query = string.Empty;
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                case 3:
                    // For - Advanced Fleet Fuel Report
                    query = string.Empty;
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                case 4:
                    // For - Fleet Fuel Report
                    query = string.Empty;
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                case 5:
                    // For - Fleet Utilisation Report
                    query = string.Empty;
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                case 6:
                    // For - Fuel Benchmarking
                    query = string.Empty;
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                case 7:
                    // For - Fuel Deviation Report
                    query = string.Empty;
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                case 8:
                    // For - Vehicle Performance Report
                    query = string.Empty;
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                case 9:
                    // For - Drive Time Management
                    query = @"SELECT da.vin VIN, da.driver_id DriverId, d.first_name FirstName, d.last_name LastName, da.activity_date ActivityDateTime FROM livefleet.livefleet_trip_driver_activity da Left join master.driver d on d.driver_id=da.driver_id WHERE (da.activity_date >= {0} AND da.activity_date <= {1}) and vin=ANY ({2}) GROUP BY da.driver_id, da.vin,d.first_name,d.last_name,da.activity_date ORDER BY da.driver_id DESC";
                    //_query = @"SELECT da.vin VIN, da.driver_id DriverId, d.first_name FirstName, d.last_name LastName, da.activity_date ActivityDateTime FROM livefleet.livefleet_trip_driver_activity da Left join master.driver d on d.driver_id=da.driver_id WHERE (da.activity_date >= {0} AND da.activity_date <= {1}) GROUP BY da.driver_id, da.vin,d.first_name,d.last_name,da.activity_date ORDER BY da.driver_id DESC";
                    query = string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                case 10:
                    // For -  Eco Score Report
                    query = @"SELECT da.vin VIN, da.driver1_id DriverId, d.first_name FirstName, d.last_name LastName
											FROM tripdetail.ecoscoredata da
                                            Left join master.driver d on d.driver_id=da.driver1_id
											WHERE (da.start_time >= {0} AND da.end_time <= {1}) and vin=ANY ({2}) 
                                            GROUP BY da.driver1_id, da.vin,d.first_name,d.last_name
											ORDER BY da.driver1_id DESC
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                case 11:
                    // For -  Schedule Report
                    query = string.Empty;
                    string.Format(query, fromDateParameter, endDateParameter, vinssParamter, optionalParameter);
                    break;
                default:
                    // Use this logic once VIEW implementation is done
                    query = "SELECT * from {0}";
                    string.Format(query, reportSQLView);
                    break;

            }
            return query;
        }
        #endregion
    }
}
