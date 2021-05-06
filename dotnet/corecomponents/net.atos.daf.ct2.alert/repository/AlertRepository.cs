using Dapper;
using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.ENUM;
using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert.repository
{
    public class AlertRepository : IAlertRepository
    {
        private readonly IDataAccess dataAccess;
        public AlertRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;

        }


        #region Update Alert

        public async Task<Alert> UpdateAlert(Alert alert)
        {
            //Begin transaction scope for master.alert table
            dataAccess.connection.Open();
            var transactionScope = dataAccess.connection.BeginTransaction();
            try
            {

                var QueryStatement = @" UPDATE master.alert
                                        SET 
                                         name=@name                                        
        	                            ,validity_period_type=@validity_period_type
                                        ,validity_start_date=@validity_start_date
                                        ,validity_end_date=@validity_end_date
                                        ,vehicle_group_id=@vehicle_group_id
                                        ,modified_at=@modified_at
                                        ,modified_by=@modified_by
                                         WHERE id = @id
                                         RETURNING id;";

                var parameter = new DynamicParameters();
                parameter.Add("@id", alert.Id);
                parameter.Add("@name", alert.Name);
                parameter.Add("@validity_period_type", alert.ValidityPeriodType);
                if (alert.ValidityPeriodType.ToUpper().ToString() == ((char)ValidityPeriodType.Custom).ToString())
                {
                    parameter.Add("@validity_start_date", alert.ValidityStartDate);
                    parameter.Add("@validity_end_date", alert.ValidityEndDate);
                }
                else
                {
                    parameter.Add("@validity_start_date", null);
                    parameter.Add("@validity_end_date", null);
                }
                parameter.Add("@vehicle_group_id", alert.VehicleGroupId);
                parameter.Add("@modified_at", alert.ModifiedAt);
                parameter.Add("@modified_by", alert.ModifiedBy);
                int alertId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                alert.Id = alertId;

                bool IsRefDeleted = await RemoveAlertRef(alert.ModifiedAt, alert.Id);
            }
            catch (Exception ex)
            {
                transactionScope.Rollback();
                throw ex;
            }
            finally
            {
                dataAccess.connection.Close();
            }

            return alert;
        }

        #endregion

        #region Update Alert State
        public async Task<int> UpdateAlertState(int alertId, char state)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", alertId);
                parameter.Add("@state", state);
                parameter.Add("@checkstate", state == ((char)AlertState.Active) ? ((char)AlertState.Suspend) : ((char)AlertState.Active));
                var query = $"update master.Alert set state = @state where id=@id and state=@checkstate RETURNING id";
                return await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> AlertStateToDelete(int alertId, char state)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", alertId);
                parameter.Add("@state", state);
                var query = $"update master.Alert set state = @state where id=@id";
                return await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<bool> CheckIsNotificationExitForAlert(int alertId)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Alert Category
        public async Task<IEnumerable<EnumTranslation>> GetAlertCategory()
        {
            try
            {
                var QueryStatement = @"SELECT                                     
                                    id as Id, 
                                    type as Type, 
                                    enum as Enum, 
                                    parent_enum as ParentEnum, 
                                    key as Key
                                    FROM translation.enumtranslation;";

                IEnumerable<EnumTranslation> enumtranslationlist = await dataAccess.QueryAsync<EnumTranslation>(QueryStatement, null);

                return enumtranslationlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private method

        private async Task<bool> RemoveAlertRef(long modifiedAt, int alertId)
        {
            char deleteChar = 'D';
            await dataAccess.ExecuteAsync("UPDATE master.alertfilterref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId });
            await dataAccess.ExecuteAsync("UPDATE master.alertlandmarkref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId });
            await dataAccess.ExecuteAsync("UPDATE master.alerturgencylevelref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId });
            await dataAccess.ExecuteAsync("UPDATE master.notification SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId });
            await dataAccess.ExecuteAsync("UPDATE master.notificationavailabilityperiod SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId });
            await dataAccess.ExecuteAsync("UPDATE master.notificationlimit SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId });
            await dataAccess.ExecuteAsync("UPDATE master.notificationrecipient SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId });
            await dataAccess.ExecuteAsync("UPDATE master.notificationtemplate SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId });
            return true;
        }

        #endregion
    }
}
