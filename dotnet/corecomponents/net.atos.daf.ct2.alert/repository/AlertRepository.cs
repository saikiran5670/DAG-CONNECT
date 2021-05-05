using Dapper;
using net.atos.daf.ct2.alert.entity;
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
            //Begin transaction scope for master.vehicle table
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

            return alert;
        }

        #endregion

        #region Update Alert State
        public Task<bool> UpdateAlertState(int alertId, char state)
        {
            throw new NotImplementedException();
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
