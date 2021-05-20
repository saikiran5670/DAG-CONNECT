using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reports.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.reports.repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDataAccess _dataAccess;
        private static readonly log4net.ILog log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReportRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
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

            string queryDelete = @"DELETE FROM master.reportpreference WHERE account_id=account_id AND report_id=@report_id";
            int rowsEffected = 0;
            using (var transactionScope = _dataAccess.connection.BeginTransaction())
            {
                try
                {
                    
                    for (int i = 0; i < objUserPreferenceRequest.AtributesShowNoShow.Count; i++)
                    {
                        var userPreference = new DynamicParameters();
                        userPreference.Add("account_id", objUserPreferenceRequest.AtributesShowNoShow[i].AccountId);
                        userPreference.Add("report_id", objUserPreferenceRequest.AtributesShowNoShow[i].ReportId);
                        userPreference.Add("data_attribute_id", objUserPreferenceRequest.AtributesShowNoShow[i].DataAttributeId);
                        userPreference.Add("is_exlusive", objUserPreferenceRequest.AtributesShowNoShow[i].IsExclusive);
                      
                        if (rowsEffected == 0)
                        {
                            await _dataAccess.ExecuteAsync(queryDelete, userPreference);
                        }
                        
                        rowsEffected += await _dataAccess.ExecuteAsync(queryInsert, userPreference);
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
    }
}
