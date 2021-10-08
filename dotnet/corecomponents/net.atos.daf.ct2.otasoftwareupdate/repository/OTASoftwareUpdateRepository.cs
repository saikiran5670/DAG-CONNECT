using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.otasoftwareupdate.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.otasoftwareupdate.repository
{
    public class OTASoftwareUpdateRepository : IOTASoftwareUpdateRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        public OTASoftwareUpdateRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartdataAccess)
        {
            this._dataAccess = dataAccess;
            this._dataMartdataAccess = dataMartdataAccess;

        }

        #region Get GetVehicleSoftwareStatus List
        public async Task<IEnumerable<VehicleSoftwareStatus>> GetVehicleSoftwareStatus()
        {
            try
            {
                var queryAlert = @"SELECT id as Id, type as Type, enum as Enum, parent_enum as ParentEnum, key as Key, feature_id as FeatureId
                                    FROM translation.enumtranslation
                                    where type ='S'";
                return await _dataAccess.QueryAsync<VehicleSoftwareStatus>(queryAlert);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Get GetSchduleCampaignByVin List
        public async Task<IEnumerable<VehicleScheduleDetails>> GetSchduleCampaignByVin(string vin)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vin", vin);
                var queryAlert = @" SELECT id, campaign_id as CampaignID, scheduled_datetime as ScheduleDateTime, baseline::text as BaselineAssignment
                                    FROM master.otascheduledcompaign
                                    where vin=@vin and status='S'";
                return await _dataAccess.QueryAsync<VehicleScheduleDetails>(queryAlert, parameter);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Get Get Release Notes from DB
        public async Task<string> GetReleaseNotes(string campaignID, string code)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@campaign_id", campaignID);
                parameter.Add("@code", code);
                var queryAlert = @" SELECT release_notes as ReleaseNotes
                                    FROM master.otacampaigncatching
                                    WHERE campaign_id=@campaign_id and code=@code";
                return await _dataAccess.QueryFirstOrDefaultAsync<string>(queryAlert, parameter);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> InsertReleaseNotes(string campaignID, string code, string releaseNotes)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@campaign_id", campaignID);
                parameter.Add("@code", code);
                parameter.Add("@release_notes", releaseNotes);
                parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                var queryAlert = @" INSERT INTO master.otacampaigncatching(
	                                 campaign_id, release_notes, code, created_at, modified_at)
	                                VALUES ( @campaign_id, @release_notes, @code, @created_at, 0) RETURNING id";
                return await _dataAccess.ExecuteAsync(queryAlert, parameter);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Get OTA Vin from DataMart
        public async Task<IEnumerable<string>> GetVinsFromOTAAlerts(IEnumerable<string> vins)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vins", vins.ToArray());
                var queryAlert = @"SELECT vin
                                    FROM tripdetail.tripalertotaconfigparam
                                    where vin = ANY(@vins)";
                return await _dataMartdataAccess.QueryAsync<string>(queryAlert, parameter);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public async Task<OtaScheduleCompaign> InsertOtaScheduleCompaign(OtaScheduleCompaign otaScheduleCompaign)
        {
            string queryStatement = @"INSERT INTO master.otascheduledcompaign(	                                                    
	                                                     compaign_id
	                                                    , vin
	                                                    , scheduledatetime	                                                   
                                                        , created_at
                                                        , created_by
                                                        , timestamp_boash_api
                                                        , status
                                                        , baseline_id
	                                                    )
	                                                    VALUES ( @compaign_id
			                                                    , @vin
			                                                    , @scheduledatetime
			                                                    , @created_at
			                                                    , @created_by
			                                                    , @timestamp_boash_api
			                                                    , @status
			                                                    , @baseline_id ;";
            var parameter = new DynamicParameters();
            parameter.Add("@compaign_id", otaScheduleCompaign.CompaignId);
            parameter.Add("@vin", otaScheduleCompaign.Vin);
            parameter.Add("@scheduledatetime", otaScheduleCompaign.ScheduleDateTime);
            parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
            parameter.Add("@created_by", UTCHandling.GetUTCFromDateTime(DateTime.Now));
            parameter.Add("@timestamp_boash_api", otaScheduleCompaign.TimeStampBoasch);
            parameter.Add("@status", 'S');
            parameter.Add("@baseline_id", otaScheduleCompaign.BaselineId);
            int tripAlertSentId = await _dataMartdataAccess.ExecuteScalarAsync<int>(queryStatement, parameter);
            otaScheduleCompaign.Id = tripAlertSentId;
            return otaScheduleCompaign;
        }

    }
}
