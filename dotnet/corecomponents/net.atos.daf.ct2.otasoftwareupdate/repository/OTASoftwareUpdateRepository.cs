using System;
using System.Collections.Generic;
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
                var queryAlert = @"SELECT id, campaign_id as CampaignID, scheduled_datetime as ScheduleDateTime, baseline as BaselineAssignment
                                    FROM master.otascheduledcompaign
                                    where vin=@vin";
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
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> InsertReleaseNotes(string campaignID, string code,string releaseNotes)
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
    }
}
