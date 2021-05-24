using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.reports.repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        private static readonly log4net.ILog log =
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
	                                 d.key as Key,rp.state as State, rp.id as ReportReferenceId, rp.chart_type as ChartType, rp.type as ReportReferenceType
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
                    log.Info($"CreateUserPreference method in repository failed : {Newtonsoft.Json.JsonConvert.SerializeObject(objUserPreferenceRequest)}");
                    log.Error(ex.ToString());
                    transactionScope.Rollback();
                    rowsEffected = 0;
                    throw ex;
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
        //This code is not in use, may require in future use.
        public Task<IEnumerable<string>> GetVinsFromTripStatistics(long fromDate, long toDate, 
                                                                   IEnumerable<string> vinList)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@fromdate", fromDate);
                parameter.Add("@todate", toDate);
                parameter.Add("@vins", vinList.ToArray());
                var query = $"SELECT DISTINCT vin FROM tripdetail.trip_statistics WHERE end_time_stamp >= @fromdate AND end_time_stamp <= @todate AND vin = Any(@vins)";
                return _dataMartdataAccess.QueryAsync<string>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Trip Report Table Details
        public async Task<List<TripDetails>> GetFilteredTripDetails(TripFilterRequest tripEntityRequest)
        {
            try
            {
                List<TripDetails> lstTripEntityResponce = new List<TripDetails>();
                string query = string.Empty;
                query = @"Select
                TS.Id Id,
                TS.trip_id TripId,
                TS.VIN VIN,
                D.first_name DriverFirstName,
                D.last_name DriverLastName,
                TS.driver2_id DriverId2,
                TS.driver1_id DriverId1,
                TS.last_odometer - TS.start_odometer Distance,
                TS.start_position StartAddress,
                TS.end_position EndAddress,
                TS.start_position_lattitude StartPositionlattitude,
                TS.start_position_longitude StartPositionLongitude,
                TS.end_position_lattitude EndPositionLattitude,
                TS.end_position_longitude EndPositionLongitude,
                TS.start_time_stamp StartTimeStamp,
                TS.end_time_stamp EndTimeStamp
               
                from tripdetail.trip_statistics TS
                left join master.driver D on TS.driver1_id=D.driver_id
                left join master.vehicle V on TS.vin=V.vin
                where TS.vin=@vin and (TS.start_time_stamp>=@StartDateTime and TS.end_time_stamp<=@EndDateTime)";

                var parameter = new DynamicParameters();
                parameter.Add("@StartDateTime", tripEntityRequest.StartDateTime);
                parameter.Add("@EndDateTime", tripEntityRequest.EndDateTime);
                parameter.Add("@vin", tripEntityRequest.VIN);

                var data = await _dataMartdataAccess.QueryAsync<TripDetails>(query, parameter);
                foreach (var item in data)
                {
                    var parameterPosition = new DynamicParameters();
                    parameterPosition.Add("@vin", item.VIN);
                    parameterPosition.Add("@trip_id", item.TripId);
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
                        item.LiveFleetPosition = lstLiveFleetPosition;
                    }
                }
                lstTripEntityResponce = data.ToList();
                return lstTripEntityResponce;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
