using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reports.entity;
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
        public Task<IEnumerable<UserPrefernceReportDataColumn>> GetUserPreferenceReportDataColumn(int reportId, int accountId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_id", reportId);
                parameter.Add("@account_id", accountId);
                var query = $"SELECT d.id as DataAtrributeId,d.name as Name,d.description as Description,d.type as Type,d.key as Key,rp.is_exlusive as IsExclusive FROM master.reportdef rd     INNER JOIN master.dataattribute d  ON rd.report_id = @report_id and rd.data_attribute_id = d.id LEFT JOIN master.reportpreference rp ON rp.report_id = @report_id and rp.account_id = @account_id and rp.report_id = rd.report_id and rp.data_attribute_id = rd.data_attribute_id WHERE rd.report_id = @report_id";
                return  _dataAccess.QueryAsync<UserPrefernceReportDataColumn>(query, parameter);
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
                                    (account_id, report_id, data_attribute_id, is_exlusive)
                             VALUES (@account_id,@report_id,@data_attribute_id,@is_exlusive)";

            string queryDelete = @"DELETE FROM master.reportpreference
                                  WHERE account_id=account_id AND report_id=@report_id";
            int rowsEffected = 0; var userPreference = new DynamicParameters();
            userPreference.Add("account_id", objUserPreferenceRequest.AccountId);
            userPreference.Add("report_id", objUserPreferenceRequest.ReportId);
            
            using (var transactionScope = _dataAccess.connection.BeginTransaction())
            {
                try
                {
                    await _dataAccess.ExecuteAsync(queryDelete, userPreference);
                    for (int i = 0; i < objUserPreferenceRequest.AtributesShowNoShow.Count; i++)
                    {
                        userPreference.Add("data_attribute_id", objUserPreferenceRequest.AtributesShowNoShow[i].DataAttributeId);
                        userPreference.Add("is_exlusive", objUserPreferenceRequest.AtributesShowNoShow[i].IsExclusive);
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
    }
}
