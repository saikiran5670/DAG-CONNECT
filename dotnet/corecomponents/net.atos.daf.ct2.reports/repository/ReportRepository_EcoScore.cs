using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        #region Eco Score Report - Create Profile

        public async Task<bool> CreateEcoScoreProfile(EcoScoreProfileDto dto)
        {
            var now = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            _dataAccess.Connection.Open();
            IDbTransaction txn = _dataAccess.Connection.BeginTransaction();
            try
            {
                //Insert into EcoScoreProfile table
                string query = @"INSERT INTO master.ecoscoreprofile
                                (organization_id,name,description,default_es_version_type, state, created_at, created_by) 
                                VALUES 
                                (@organization_id, @name, @description, NULL, @state, @created_at, @created_by) RETURNING id";

                var parameters = new DynamicParameters();
                parameters.Add("@organization_id", dto.OrganizationId);
                parameters.Add("@name", dto.Name);
                parameters.Add("@description", dto.Description);
                parameters.Add("@state", 'A');
                parameters.Add("@created_at", now);
                parameters.Add("@created_by", Convert.ToInt32(dto.ActionedBy));
                parameters.Add("@modified_at", now);
                parameters.Add("@modified_by", Convert.ToInt32(dto.ActionedBy));

                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameters);
                dto.Id = id;

                //Insert into EcoScoreProfileKPI table
                foreach (var profileKPI in dto.ProfileKPIs)
                {
                    query = @"INSERT INTO master.ecoscoreprofilekpi
                        (ecoscore_profile_id,ecoscore_kpi_id,limit_val,target_val, lower_val, upper_val, created_at, created_by)
                        VALUES
                        (@profile_id, @ecoscore_kpi_id, @limit_val, @target_val, @lower_val, @upper_val, @created_at, @created_by) RETURNING id";

                    parameters = new DynamicParameters();
                    parameters.Add("@profile_id", id);
                    parameters.Add("@ecoscore_kpi_id", profileKPI.KPIId);
                    parameters.Add("@limit_val", profileKPI.LimitValue);
                    parameters.Add("@target_val", profileKPI.TargetValue);
                    parameters.Add("@lower_val", profileKPI.LowerValue);
                    parameters.Add("@upper_val", profileKPI.UpperValue);
                    parameters.Add("@created_at", now);
                    parameters.Add("@created_by", Convert.ToInt32(dto.ActionedBy));
                    parameters.Add("@modified_at", now);
                    parameters.Add("@modified_by", Convert.ToInt32(dto.ActionedBy));

                    await _dataAccess.ExecuteScalarAsync<int>(query, parameters);
                }
                txn.Commit();
                return true;
            }
            catch (Exception)
            {
                txn.Rollback();
                throw;
            }
            finally
            {
                if (txn != null)
                {
                    _dataAccess.Connection.Close();
                    txn.Dispose();
                }
            }
        }

        public async Task<int> GetEcoScoreProfilesCount(int orgId)
        {
            try
            {
                string query = @"SELECT COUNT(1) 
                                FROM master.ecoscoreprofile
                                WHERE organization_id = @organization_id and state = 'A'";

                var parameters = new DynamicParameters();
                parameters.Add("@organization_id", orgId);
                return await _dataAccess.ExecuteScalarAsync<int>(query, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Eco Score Report - Get Profile and KPI Details

        /// <summary>
        /// Get Eco-Score profile list 
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<List<EcoScoreProfileDto>> GetEcoScoreProfiles(int orgId)
        {
            List<EcoScoreProfileDto> lstEcoScoreProfiles = new List<EcoScoreProfileDto>();
            _dataAccess.Connection.Open();
            try
            {
                string query = @"select id as ProfileId, organization_id as OrganizationID, name as ProfileName, description as ProfileDescription,
                                 default_es_version_type as Type
                                 from master.ecoscoreprofile
                                 where state='A' and (organization_id is null or organization_id = @organization_id)
                                 order by default_es_version_type asc, organization_id desc, name";

                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", orgId);
                dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);

                foreach (dynamic profile in result)
                    lstEcoScoreProfiles.Add(MapEcoScoreProfiles(profile));

                return lstEcoScoreProfiles;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
        }

        /// <summary>
        /// Map DB result to Eco-Score Profile object
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        private EcoScoreProfileDto MapEcoScoreProfiles(dynamic profile)
        {
            EcoScoreProfileDto objProfile = new EcoScoreProfileDto();
            objProfile.Id = profile.profileid;
            objProfile.Name = profile.profilename;
            objProfile.Description = profile.profiledescription;
            objProfile.OrganizationId = profile.organizationid;
            if (profile.type is null)
                objProfile.Type = ProfileType.None;
            else
                objProfile.Type = (ProfileType)Convert.ToChar(profile.type);

            return objProfile;
        }

        /// <summary>
        /// Get Eco-Score profile KPI Details 
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<EcoScoreProfileDto> GetEcoScoreProfileKPIDetails(int profileId)
        {
            EcoScoreProfileDto objEcoScoreProfileKPIDetails;
            _dataAccess.Connection.Open();
            try
            {
                string query1 = string.Empty;
                StringBuilder query = new StringBuilder();
                var parameter = new DynamicParameters();
                if (profileId > 0)
                {
                    query.Append("select pro.id as ProfileId, ");
                    query.Append("pro.organization_id as OrganizationID, ");
                    query.Append("pro.name as ProfileName, ");
                    query.Append("pro.description as ProfileDescription, ");
                    query.Append("acc.email as ActionedBy, ");
                    query.Append("pro.modified_at as LastUpdate,");
                    query.Append("sec.id as SectionId, ");
                    query.Append("sec.name as SectionName, ");
                    query.Append("sec.description as SectionDescription,");
                    query.Append("kpi.id as EcoScoreKPIId, ");
                    query.Append("kpi.name as KPIName, ");
                    query.Append("kpi.limit_type as LimitType, ");
                    query.Append("prokpi.limit_val as LimitValue, ");
                    query.Append("prokpi.target_val as TargetValue, ");
                    query.Append("prokpi.lower_val as LowerValue, ");
                    query.Append("prokpi.upper_val as UpperValue, ");
                    query.Append("kpi.range_value_Type as RangeValueType, ");
                    query.Append("kpi.max_upper_value as MaxUpperValue, ");
                    query.Append("kpi.seq_no as SequenceNo ");

                    query.Append("from master.ecoscoreprofile pro ");
                    query.Append("left join master.ecoscoreprofilekpi prokpi ");
                    query.Append("on pro.id= prokpi.ecoscore_profile_id ");
                    query.Append("left join master.ecoscorekpi kpi ");
                    query.Append("on prokpi.ecoscore_kpi_id = kpi.id ");
                    query.Append("left join master.ecoscoresection sec ");
                    query.Append("on sec.id= kpi.section_id ");
                    query.Append("left join master.account acc ");
                    query.Append("on pro.modified_by = acc.id ");
                    query.Append("where pro.state='A' and pro.id = @profile_id");

                    parameter.Add("@profile_id", profileId);
                }
                else
                {
                    query.Append("select sec.id as SectionId, ");
                    query.Append("sec.name as SectionName, ");
                    query.Append("sec.description as SectionDescription, ");
                    query.Append("kpi.id as EcoScoreKPIId, ");
                    query.Append("kpi.name as KPIName, ");
                    query.Append("kpi.limit_type as LimitType, ");
                    query.Append("kpi.max_upper_value as UpperValue, ");
                    query.Append("kpi.range_value_Type as RangeValueType, ");
                    query.Append("kpi.max_upper_value as MaxUpperValue, ");
                    query.Append("kpi.seq_no as SequenceNo ");

                    query.Append("from master.ecoscorekpi kpi ");
                    query.Append("join master.ecoscoresection sec ");
                    query.Append("on sec.id= kpi.section_id ");
                }

                dynamic result = await _dataAccess.QueryAsync<dynamic>(Convert.ToString(query), parameter);
                objEcoScoreProfileKPIDetails = MapEcoScoreProfileKPIDetails(result);
                return objEcoScoreProfileKPIDetails;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
        }

        /// <summary>
        /// Map DB result to Eco-Score Profile KPI 
        /// </summary>
        /// <param name="profileKPI"></param>
        /// <returns></returns>
        private EcoScoreProfileDto MapEcoScoreProfileKPIDetails(dynamic profileKPI)
        {
            EcoScoreProfileDto objProfile = new EcoScoreProfileDto();
            if (profileKPI.Count > 0)
            {
                var pro = profileKPI[0];
                if (pro.profileid > 0)
                {
                    objProfile.Id = pro.profileid;
                    objProfile.OrganizationId = pro.organizationid;
                    objProfile.Name = pro.profilename;
                    objProfile.Description = pro.profiledescription;
                    objProfile.ActionedBy = pro.actionedby;
                    objProfile.LastUpdate = pro.lastupdate != null ? Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(pro.lastupdate, "UTC", "yyyy-MM-ddTHH:mm:ss")) : default(DateTime);
                }
                List<EcoScoreProfileKPI> lstProfileKPI = new List<EcoScoreProfileKPI>();
                foreach (dynamic obj in profileKPI)
                {
                    EcoScoreProfileKPI objProfileKPI = new EcoScoreProfileKPI();
                    objProfileKPI.SectionId = obj.sectionid;
                    objProfileKPI.SectionName = obj.sectionname;
                    objProfileKPI.SectionDescription = obj.sectiondescription;
                    objProfileKPI.KPIId = obj.ecoscorekpiid;
                    objProfileKPI.KPIName = obj.kpiname;
                    objProfileKPI.LimitType = obj.limittype;
                    objProfileKPI.LimitValue = Convert.ToDouble(obj.limitvalue);
                    objProfileKPI.TargetValue = Convert.ToDouble(obj.targetvalue);
                    objProfileKPI.LowerValue = Convert.ToDouble(obj.lowervalue);
                    objProfileKPI.UpperValue = Convert.ToDouble(obj.uppervalue);
                    objProfileKPI.RangeValueType = obj.rangevaluetype;
                    objProfileKPI.MaxUpperValue = Convert.ToDouble(obj.maxuppervalue);
                    objProfileKPI.SequenceNo = Convert.ToInt32(obj.sequenceno);
                    lstProfileKPI.Add(objProfileKPI);
                }
                objProfile.ProfileKPIs = new List<EcoScoreProfileKPI>();
                objProfile.ProfileKPIs.AddRange(lstProfileKPI);
            }
            return objProfile;
        }

        #endregion

        #region Eco Score Report - Update Profile
        public async Task<int> UpdateEcoScoreProfile(EcoScoreProfileDto ecoScoreProfileDto)
        {
            _dataAccess.Connection.Open();
            IDbTransaction txnScope = _dataAccess.Connection.BeginTransaction();
            try
            {
                var updateParameter = new DynamicParameters();

                StringBuilder queryForUpdateEcoScoreProfile = new StringBuilder();

                queryForUpdateEcoScoreProfile.Append("UPDATE master.ecoscoreprofile set modified_at=@modified_at ,description=@description , modified_by= @modified_by");
                if (!string.IsNullOrEmpty(ecoScoreProfileDto.Name))
                {
                    updateParameter.Add("@Name", ecoScoreProfileDto.Name);
                    queryForUpdateEcoScoreProfile.Append(", name=@Name");
                }
                updateParameter.Add("@description", ecoScoreProfileDto.Description);

                updateParameter.Add("@modified_by", Convert.ToInt32(ecoScoreProfileDto.ActionedBy));

                updateParameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

                updateParameter.Add("@id", ecoScoreProfileDto.Id);

                queryForUpdateEcoScoreProfile.Append(" where id=@id RETURNING id");

                var id = await _dataAccess.ExecuteScalarAsync<int>(queryForUpdateEcoScoreProfile.ToString(), updateParameter);
                if (id > 0)
                {
                    var tripDetails = await UpdateEcoscoreProfileKpi(ecoScoreProfileDto.ProfileKPIs, Convert.ToInt32(ecoScoreProfileDto.ActionedBy), id);
                }
                txnScope.Commit();

                return id;
            }
            catch (Exception)
            {
                txnScope.Rollback();
                throw;
            }
            finally
            {
                if (txnScope != null)
                {
                    _dataAccess.Connection.Close();
                    txnScope.Dispose();
                }
            }


        }
        private async Task<bool> UpdateEcoscoreProfileKpi(List<EcoScoreProfileKPI> ecoScoreProfileKPI, int actionedBy, int id)
        {
            var updateParameter = new DynamicParameters();
            StringBuilder query = new StringBuilder();
            foreach (var item in ecoScoreProfileKPI)
            {

                var temp = new EcoScoreProfileKPI();
                temp.KPIId = item.KPIId;
                temp.LimitValue = item.LimitValue;
                temp.LowerValue = item.LowerValue;
                temp.TargetValue = item.TargetValue;
                temp.UpperValue = item.UpperValue;

                updateParameter.Add("@LimitValue", temp.LimitValue);
                updateParameter.Add("@LowerValue", temp.LowerValue);
                updateParameter.Add("@TargetValue", temp.TargetValue);
                updateParameter.Add("@UpperValue", temp.UpperValue);
                updateParameter.Add("@KPIId", temp.KPIId);
                updateParameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                updateParameter.Add("@modified_by", actionedBy);
                updateParameter.Add("@Id", id);

                query.Append("UPDATE master.ecoscoreprofilekpi Set modified_at =@modified_at");

                query.Append(", limit_val=@LimitValue");
                query.Append(", target_val=@TargetValue");
                query.Append(", lower_val=@LowerValue");
                query.Append(", upper_val=@UpperValue");
                query.Append(", modified_by=@modified_by");
                query.Append(" where ecoscore_profile_id=@Id and ecoscore_kpi_id = @KPIId RETURNING id");

                id = await _dataAccess.ExecuteScalarAsync<int>(query.ToString(), updateParameter);
            }
            return id > 0;
        }

        public async Task<bool> CheckEcoScoreProfileIsExist(int? organizationId, string name)
        {
            var parameterDuplicate = new DynamicParameters();

            var query = "SELECT id FROM master.ecoscoreprofile where state ='A' and name=@name and (organization_id = @organization_id or organization_id is null)";

            parameterDuplicate.Add("@name", name);
            parameterDuplicate.Add("@organization_id", organizationId);
            int reportNameExist = await _dataAccess.ExecuteScalarAsync<int>(query, parameterDuplicate);

            return reportNameExist != 0;
        }
        #endregion

        #region Eco Score Report - Delete Profile
        public async Task<int> DeleteEcoScoreProfile(int profileId)
        {
            _log.Info("Delete Eco Score Profile method called in repository");
            try
            {
                var parameter = new DynamicParameters();

                var deleteProfile = @"UPDATE master.ecoscoreprofile
                                            SET  state=@State 
                                   WHERE id = @ProfileId  RETURNING id";

                parameter.Add("@ProfileId", profileId);
                parameter.Add("@State", "D");

                var id = await _dataAccess.ExecuteScalarAsync<int>(deleteProfile, parameter);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetProfileName(int profileId)
        {
            var parameter = new DynamicParameters();

            StringBuilder query = new StringBuilder();

            query.Append("select name from master.ecoscoreprofile where id= @ProfileId ");
            parameter.Add("@ProfileId", profileId);

            string profileName = await _dataAccess.ExecuteScalarAsync<string>(query.ToString(), parameter);

            return profileName;
        }
        public async Task<string> IsEcoScoreProfileBasicOrAdvance(int profileId)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = "SELECT  default_es_version_type FROM master.ecoscoreprofile where id =@ProfileId and state = 'A' ";
                parameter.Add("@ProfileId", profileId);
                var versionType = await _dataAccess.ExecuteScalarAsync<string>(query, parameter);
                return versionType;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> GetGlobalProfile(int profileId)
        {
            var parameter = new DynamicParameters();
            var query = "select name from master.ecoscoreprofile where id= @ProfileId and organization_id is null and default_es_version_type is null and state = 'A'";
            parameter.Add("@ProfileId", profileId);

            string profileName = await _dataAccess.ExecuteScalarAsync<string>(query, parameter);

            return string.IsNullOrEmpty(profileName);
        }

        #endregion

        #region Eco Score Report By All Drivers
        /// <summary>
        /// Get Eco Score Report By All Drivers
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<List<EcoScoreReportByAllDrivers>> GetEcoScoreReportByAllDrivers(EcoScoreReportByAllDriversRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@MinTripDistance", request.MinTripDistance > 0 ? request.MinTripDistance : (double?)null);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance > 0 ? request.MinDriverTotalDistance : (double?)null);

                string query = @"WITH ecoscore AS (
                                 SELECT dr.first_name, dr.last_name, eco.driver1_id, eco.trip_distance,
                                 eco.dpa_braking_score, eco.dpa_braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count
                                 FROM tripdetail.ecoscoredata eco
                                 JOIN master.driver dr 
                                 	ON dr.driver_id = eco.driver1_id
                                 WHERE eco.start_time >= @FromDate
                                 	AND eco.end_time <= @ToDate
                                 	AND eco.vin = ANY( @Vins )
                                 	AND (eco.trip_distance < @MinTripDistance OR @MinTripDistance IS NULL)
                                 ),
                                 
                                 ecoscorealldriver as 
                                 (
                                 SELECT first_name || ' ' || last_name AS driverName,driver1_id as driverid, SUM(trip_distance)AS totaldriverdistance,
                                 CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN 
    							 (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
      							 (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
    							 else null END  as ecoscoreranking
                                 FROM ecoscore eco
                                 GROUP BY first_name, last_name, driver1_id
                                 ORDER BY ecoscoreranking DESC
                                 )
                                 
                                 SELECT ROW_NUMBER () OVER (ORDER BY  ecoscoreranking DESC) as Ranking,
                                 driverName, driverid, ecoscoreranking
                                 FROM ecoscorealldriver
                                 where 1=1 AND (totaldriverdistance < @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)
                                 ORDER BY ecoscoreranking DESC, driverName";

                List<EcoScoreReportByAllDrivers> lstByAllDrivers = (List<EcoScoreReportByAllDrivers>)await _dataMartdataAccess.QueryAsync<EcoScoreReportByAllDrivers>(query, parameters);
                return lstByAllDrivers?.Count > 0 ? lstByAllDrivers : new List<EcoScoreReportByAllDrivers>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Target Profile Eco-Score KPI Value
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<EcoScoreKPIRanking> GetEcoScoreTargetProfileKPIValues(int targetProfileId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TargetProfileId", targetProfileId);

                string query = @"SELECT eco.id as profileid, kpimst.name as kpiname,
                                 kpi.limit_val as minvalue, kpi.target_val as targetvalue
                                 FROM master.ecoscoreprofile eco
                                 JOIN master.ecoscoreprofilekpi kpi
                                 	ON eco.id=kpi.ecoscore_profile_id
                                 JOIN master.ecoscorekpi kpimst
                                 	ON kpimst.id = kpi.ecoscore_kpi_id
                                 WHERE eco.id = @TargetProfileId
                                 AND kpimst.name = 'Eco-Score'";

                return await _dataAccess.QueryFirstOrDefaultAsync<EcoScoreKPIRanking>(query, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update Eco-Score Target Profile to Report Preferences
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UpdateEcoScoreTargetProfile(EcoScoreReportByAllDriversRequest request)
        {
            _dataAccess.Connection.Open();
            IDbTransaction txn = _dataAccess.Connection.BeginTransaction();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ReportId", request.ReportId);
                parameters.Add("@OrgId", request.OrgId);
                parameters.Add("@AccountId", request.AccountId);
                parameters.Add("@TargetProfileId", request.TargetProfileId);

                var updateTargetProfile = @"UPDATE master.reportpreference
                                            SET ecoscore_profile_id = @TargetProfileId
                                            WHERE organization_id = @OrgId
                                            AND account_id = @AccountId
                                            AND report_id = @ReportId RETURNING id";

                int id = await _dataAccess.ExecuteScalarAsync<int>(updateTargetProfile, parameters);
                txn.Commit();
                return id > 0;
            }
            catch (Exception)
            {
                txn.Rollback();
                throw;
            }
            finally
            {
                if (txn != null)
                {
                    _dataAccess.Connection.Close();
                    txn.Dispose();
                }
            }
        }
        #endregion

        #region Eco Score Report - User Preferences

        public async Task<bool> CreateReportUserPreference(ReportUserPreferenceCreateRequest request)
        {
            string queryInsert = @"INSERT INTO master.reportpreference
                                   (organization_id,account_id, report_id, type, data_attribute_id,state,chart_type,created_at,modified_at,threshold_limit_type,threshold_value,reportattribute_id)
                                   VALUES (@organization_id,@account_id,@report_id,@type,@data_attribute_id,@state,@chart_type,@created_at,@modified_at,@threshold_type,@threshold_value,
                                   (SELECT id from master.reportattribute WHERE report_id=@report_id AND data_attribute_id=@data_attribute_id))";

            string queryDelete = @"DELETE FROM master.reportpreference
                                  WHERE organization_id=@organization_id and account_id=@account_id AND report_id=@report_id";

            var userPreference = new DynamicParameters();
            userPreference.Add("@account_id", request.AccountId);
            userPreference.Add("@report_id", request.ReportId);
            userPreference.Add("@organization_id", request.OrganizationId);
            userPreference.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            userPreference.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

            _dataAccess.Connection.Open();
            using (var transactionScope = _dataAccess.Connection.BeginTransaction())
            {
                try
                {
                    await _dataAccess.ExecuteAsync(queryDelete, userPreference);
                    foreach (var attribute in request.Attributes)
                    {
                        userPreference.Add("@data_attribute_id", attribute.DataAttributeId);
                        userPreference.Add("@state", (char)attribute.State);
                        userPreference.Add("@type", (char)attribute.Type);
                        userPreference.Add("@chart_type", attribute.ChartType.HasValue ? (char)attribute.ChartType : new char?());
                        userPreference.Add("@threshold_type", attribute.ThresholdType.HasValue ? (char)attribute.ThresholdType : new char?());
                        userPreference.Add("@threshold_value", attribute.ThresholdValue);
                        await _dataAccess.ExecuteAsync(queryInsert, userPreference);
                    }
                    transactionScope.Commit();
                }
                catch (Exception)
                {
                    transactionScope.Rollback();
                    throw;
                }
                finally
                {
                    if (transactionScope != null)
                        transactionScope.Dispose();
                    _dataAccess.Connection.Close();
                }
                return true;
            }
        }

        public async Task<bool> CheckIfReportUserPreferencesExist(int reportId, int accountId, int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_id", reportId);
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_id", organizationId);

                #region Query Select User Preferences
                var query = @"SELECT EXISTS 
                            (
                                SELECT 1 FROM master.reportpreference 
                                WHERE account_id = @account_id and organization_id = @organization_id and report_id = @report_id
                            )";
                #endregion

                return await _dataAccess.ExecuteScalarAsync<bool>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ReportUserPreference>> GetReportUserPreferences(int reportId, int accountId,
                                                                                             int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_id", reportId);
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_id", organizationId);

                #region Query Select User Preferences
                var query = @"SELECT d.id as DataAttributeId, d.name as Name, ra.key as Key, ra.sub_attribute_ids as SubDataAttributes, ra.type as AttributeType,
					                 CASE WHEN rp.state IS NULL THEN 'A' ELSE rp.state END as State, rp.chart_type as ChartType, rp.type as ReportPreferenceType, 
					                 rp.threshold_limit_type as ThresholdType, rp.threshold_value as ThresholdValue, rp.ecoscore_profile_id as TargetProfileId
                            FROM master.reportattribute ra
                            INNER JOIN master.dataattribute d ON ra.data_attribute_id = d.id
                            LEFT JOIN master.reportpreference rp ON rp.reportattribute_id = ra.id and 
										                            rp.account_id = @account_id and rp.organization_id = @organization_id and 
										                            rp.report_id = ra.report_id
                            WHERE ra.report_id = @report_id";
                #endregion

                return await _dataAccess.QueryAsync<ReportUserPreference>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ReportUserPreference>> GetPrivilegeBasedReportUserPreferences(int reportId, int accountId, int roleId,
                                                                                              int organizationId, int contextOrgId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_id", reportId);
                parameter.Add("@account_id", accountId);
                parameter.Add("@role_id", roleId);
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@context_org_id", contextOrgId);

                #region Query RoleBasedDataColumn
                var query = @"SELECT DISTINCT d.id as DataAttributeId,d.name as Name, ra.key as Key, 'A' as state,
                                              ra.sub_attribute_ids as SubDataAttributes, ra.type as AttributeType
                              FROM master.reportattribute ra
                              INNER JOIN master.dataattribute d ON ra.report_id = @report_id and d.id = ra.data_attribute_id 
                              INNER JOIN master.DataAttributeSetAttribute dasa ON dasa.data_attribute_id = d.id
                              INNER JOIN master.DataAttributeSet das ON das.id = dasa.data_attribute_set_id and das.state = 'A' 
                              INNER JOIN
                              (
                                  --Account Route
                                  SELECT f.id, f.data_attribute_set_id
                                  FROM master.Account acc
                                  INNER JOIN master.AccountRole ar ON acc.id = ar.account_id AND acc.id = @account_id AND ar.organization_id = @organization_id AND ar.role_id = @role_id AND acc.state = 'A'
                                  INNER JOIN master.Role r ON ar.role_id = r.id AND r.state = 'A'
                                  INNER JOIN master.FeatureSet fset ON r.feature_set_id = fset.id AND fset.state = 'A'
                                  INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
                                  INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type = 'D'
                                  INTERSECT
                                  --Subscription Route
                                  SELECT f.id, f.data_attribute_set_id
                                  FROM master.Subscription s
                                  INNER JOIN master.Package pkg ON s.package_id = pkg.id AND s.organization_id = @context_org_id AND s.state = 'A' AND pkg.state = 'A'
                                  INNER JOIN master.FeatureSet fset ON pkg.feature_set_id = fset.id AND fset.state = 'A'
                                  INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
                                  INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type = 'D'
                              ) fsets ON fsets.data_attribute_set_id = das.id";
                #endregion

                return await _dataAccess.QueryAsync<ReportUserPreference>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Eco Score Report Compare Drivers
        /// <summary>
        /// Get Eco Score Report Compare Drivers
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<List<EcoScoreReportCompareDrivers>> GetEcoScoreReportCompareDrivers(EcoScoreReportCompareDriversRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverIds", request.DriverIds.ToArray());
                // parameters.Add("@MinTripDistance", request.MinTripDistance > 0 ? request.MinTripDistance : (double?)null);
                // parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance > 0 ? request.MinDriverTotalDistance : (double?)null);

                string query = @"WITH 
                                ecoscorequery as (
                                    SELECT dr.first_name, dr.last_name, eco.driver1_id, eco.trip_distance,eco.trip_id,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, eco.vin,eco.used_fuel,eco.pto_duration,eco.end_time,eco.start_time,eco.tacho_gross_weight_combination,
                                    eco.heavy_throttle_pedal_duration,eco.idle_duration,eco.harsh_brake_duration,eco.brake_duration
                                    FROM tripdetail.ecoscoredata eco
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                    WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) -- AND eco.vin = ANY('{XLR0998HGFFT76657,XLR0998HGFFT74600}')
                                    AND eco.driver1_id = ANY(@DriverIds)
                                    --  AND (eco.trip_distance < @MinTripDistance OR eco.trip_distance IS NULL)
                                ),
                                
                                generalblk as 
                                (
                                    select eco.driver1_id, eco.first_name || ' ' || eco.last_name AS driverName, count(eco.driver1_id)  as drivercnt
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id,eco.first_name,eco.last_name
                                ),
                                AverageGrossweight as 
                                (
                                    select eco.driver1_id, (CAST(SUM (eco.tacho_gross_weight_combination) as DOUBLE PRECISION))/ nullif((CAST(SUM (eco.trip_distance) as DOUBLE PRECISION)),0) as AverageGrossweight
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                Distance as 
                                (
                                    select eco.driver1_id, (CAST(SUM (eco.trip_distance)as DOUBLE PRECISION)) as Distance
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                NumberOfTrips as 
                                (
                                    select eco.driver1_id, COUNT (eco.trip_id) as NumberOfTrips
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                NumberOfVehicles as 
                                (
                                    select eco.driver1_id, COUNT (eco.vin) as NumberOfVehicles
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                AverageDistancePerDay as 
                                (
                                    select eco.driver1_id, 0 as AverageDistancePerDay
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                EcoScore as
                                (
                                    SELECT eco.driver1_id ,
                                    CASE WHEN (SUM(dpa_Braking_count))<> 0 THEN 
                                    (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
                                      (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
                                    else null END as ecoscore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                FuelConsumption as 
                                (
                                    SELECT eco.driver1_id, (CAST(SUM (eco.used_fuel)AS DOUBLE PRECISION )) as FuelConsumption
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage as 
                                (
                                    SELECT eco.driver1_id, 0  as CruiseControlUsage
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage30 as 
                                (
                                    SELECT eco.driver1_id, 0  as CruiseControlUsage30
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage50 as 
                                (
                                    SELECT eco.driver1_id, 0  as CruiseControlUsage50
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage75 as 
                                (
                                   SELECT eco.driver1_id, 0  as CruiseControlUsage75
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                PTOUsage as 
                                (
                                   SELECT eco.driver1_id,
                                   CASE WHEN ( SUM (eco.end_time)- SUM (eco.start_time) ) <> 0 THEN
                                   SUM(eco.pto_duration) / (( SUM (eco.end_time)- SUM (eco.start_time) )/1000) 
                                   ELSE null END as PTOUsage
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id			 
                                ),
                                PTODuration as 
                                (
                                   SELECT eco.driver1_id,  SUM(eco.pto_duration) as PTODuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id	
                                ),
                                AverageDrivingSpeed as
                                (  
                                   SELECT eco.driver1_id,  
                                   CASE WHEN (((   (CAST(SUM (eco.end_time)AS DOUBLE PRECISION) )    -   (CAST(SUM (eco.start_time)AS DOUBLE PRECISION))   )/1000)-   (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))    )  <> 0 THEN
                                     (CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION) )  /(((   (CAST(SUM (eco.end_time)AS DOUBLE PRECISION) )    -   (CAST(SUM (eco.start_time)AS DOUBLE PRECISION))   )/1000)-   (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))    )  
                                   ELSE null END as AverageDrivingSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                AverageSpeed as
                                (
                                   SELECT eco.driver1_id, 
                                   CASE WHEN ((CAST(SUM (eco.end_time)AS DOUBLE PRECISION))- (CAST(SUM (eco.start_time)AS DOUBLE PRECISION))) <>0 then
                                   SUM(eco.trip_distance)/(((CAST(SUM (eco.end_time)AS DOUBLE PRECISION))- (CAST(SUM (eco.start_time)AS DOUBLE PRECISION)))/1000)  
                                   ELSE null END as AverageSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                HeavyThrottling as
                                (
                                   SELECT eco.driver1_id,
                                   CASE WHEN ((CAST(SUM (eco.end_time)AS DOUBLE PRECISION))- (CAST(SUM (eco.start_time)AS DOUBLE PRECISION))) <> 0 THEN
                                   SUM(eco.heavy_throttle_pedal_duration)/(((CAST(SUM (eco.end_time)AS DOUBLE PRECISION))- (CAST(SUM (eco.start_time)AS DOUBLE PRECISION)))/1000)  
                                   ELSE null END as HeavyThrottling
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                HeavyThrottleDuration  as
                                (
                                   SELECT eco.driver1_id, (CAST(SUM(eco.heavy_throttle_pedal_duration ) AS DOUBLE PRECISION)) as HeavyThrottleDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                Idling  as
                                (
                                
                                   SELECT eco.driver1_id,
                                   CASE WHEN ( CAST(SUM (eco.end_time)AS DOUBLE PRECISION)- CAST(SUM (eco.start_time)AS DOUBLE PRECISION))<> 0 THEN 
                                   ( CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION)/ (( CAST(SUM (eco.end_time) As DOUBLE PRECISION)- CAST(SUM (eco.start_time)AS DOUBLE PRECISION))/1000))* 100
                                   ELSE null end
                                   as Idling
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                IdleDuration  as
                                (
                                   SELECT eco.driver1_id,  CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)   as IdleDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                BrakingScore  as
                                (
                                   SELECT eco.driver1_id,( CAST(SUM(eco.dpa_Braking_score) AS DOUBLE PRECISION)/ NULLIF ( (CAST(SUM (eco.dpa_Braking_count)AS DOUBLE PRECISION)),0))/10   as BrakingScore
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                HarshBraking  as
                                (
                                   SELECT eco.driver1_id, CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION)/ NULLIF( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)),0) as HarshBraking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                HarshBrakeDuration  as
                                (
                                   SELECT eco.driver1_id, 0 as HarshBrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                BrakeDuration as
                                (
                                   SELECT eco.driver1_id, CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)/ 86400 as BrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                Braking as
                                (
                                   SELECT eco.driver1_id, ( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION))/ ((  (CAST(SUM (eco.end_time)AS DOUBLE PRECISION))  -  ( CAST(SUM (eco.start_time)AS DOUBLE PRECISION) )  )/1000))*100 as Braking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                AnticipationScore as
                                (
                                   SELECT eco.driver1_id, (   (CAST(SUM(eco.dpa_anticipation_score)AS DOUBLE PRECISION ) ) / NULLIF(  (CAST (SUM(eco.dpa_anticipation_count) AS DOUBLE PRECISION) )  ,0) )/10 as AnticipationScore
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                )
                                select eco.driver1_id as DriverId
                                ,eco.DriverName
                                
                                ,CAST(avrg.averagegrossweight AS DOUBLE PRECISION) as AverageGrossweight
                                ,CAST(dis.distance AS DOUBLE PRECISION) as Distance
                                ,CAST(notrp.numberoftrips AS DOUBLE PRECISION) as NumberOfTrips
                                ,CAST(noveh.numberofvehicles AS DOUBLE PRECISION) as NumberOfVehicles
                                ,CAST(avgdperday.averagedistanceperday AS DOUBLE PRECISION) as AverageDistancePerDay
                                
                                ,CAST(ecos.ecoscore AS DOUBLE PRECISION) as EcoScore
                                ,CAST(f.fuelconsumption AS DOUBLE PRECISION) as FuelConsumption
                                ,CAST(crus.cruisecontrolusage AS DOUBLE PRECISION) as CruiseControlUsage
                                ,CAST(crusa.cruisecontrolusage30 AS DOUBLE PRECISION) as CruiseControlUsage30
                                ,CAST(crucon.cruisecontrolusage50 AS DOUBLE PRECISION) as CruiseControlUsage50
                                ,CAST(crucont.cruisecontrolusage75 AS DOUBLE PRECISION) as CruiseControlUsage75
                                ,CAST(p.ptousage AS DOUBLE PRECISION) as PTOUsage
                                ,CAST(pto.ptoduration AS DOUBLE PRECISION) as PTODuration
                                ,CAST(ads.averagedrivingspeed AS DOUBLE PRECISION) as AverageDrivingSpeed
                                ,CAST(aspeed.averagespeed AS DOUBLE PRECISION) as AverageSpeed
                                ,CAST(h.heavythrottling AS DOUBLE PRECISION) as HeavyThrottling
                                ,CAST(he.heavythrottleduration AS DOUBLE PRECISION) as HeavyThrottleDuration
                                ,CAST(i.idling AS DOUBLE PRECISION) as Idling
                                ,CAST(ide.idleduration AS DOUBLE PRECISION) as IdleDuration
                                ,CAST(br.brakingscore AS DOUBLE PRECISION) as BrakingScore
                                ,CAST(hr.harshbraking AS DOUBLE PRECISION) as HarshBraking
                                ,CAST(hrdur.HarshBrakeDuration AS DOUBLE PRECISION) as HarshBrakeDuration
                                ,CAST(brdur.brakeduration AS DOUBLE PRECISION) as BrakeDuration
                                ,CAST(brk.braking AS DOUBLE PRECISION) as Braking
                                ,CAST(anc.anticipationscore  AS DOUBLE PRECISION) as AnticipationScore
                                
                                from generalblk eco
                                Left join AverageGrossweight avrg on avrg.driver1_id = eco.driver1_id
                                Left join Distance dis on dis.driver1_id = avrg.driver1_id
                                Left join NumberOfTrips notrp on notrp.driver1_id = dis.driver1_id
                                Left join numberofvehicles noveh on noveh.driver1_id = notrp.driver1_id
                                Left join AverageDistancePerDay avgdperday on avgdperday.driver1_id = noveh.driver1_id
                                Left join EcoScore ecos on ecos.driver1_id = avgdperday.driver1_id
                                Left join FuelConsumption f on f.driver1_id = ecos.driver1_id
                                Left join CruiseControlUsage crus  on crus.driver1_id = f.driver1_id   
                                Left join CruiseControlUsage30 crusa  on crusa.driver1_id = crus.driver1_id
                                Left join CruiseControlUsage50 crucon on crucon.driver1_id = crusa.driver1_id
                                Left join CruiseControlUsage75 crucont on crucont.driver1_id = crucon.driver1_id
                                Left join PTOUsage p on p.driver1_id = crucont.driver1_id
                                Left join PTODuration pto on pto.driver1_id = p.driver1_id
                                Left join AverageDrivingSpeed ads on ads.driver1_id = pto.driver1_id
                                
                                Left join AverageSpeed aspeed on aspeed.driver1_id = ads.driver1_id
                                Left join HeavyThrottling h on h.driver1_id = aspeed.driver1_id
                                Left join HeavyThrottleDuration he on he.driver1_id = h.driver1_id
                                Left join Idling i on i.driver1_id = he.driver1_id
                                Left join IdleDuration ide on ide.driver1_id = i.driver1_id
                                Left join BrakingScore br on br.driver1_id = ide.driver1_id
                                Left join HarshBraking hr on hr.driver1_id = br.driver1_id
                                Left join HarshBrakeDuration hrdur on hrdur.driver1_id = hr.driver1_id
                                Left join AnticipationScore anc on anc.driver1_id = hrdur.driver1_id
                                Left join BrakeDuration brdur on brdur.driver1_id = anc.driver1_id
                                Left join Braking brk on brk.driver1_id = brdur.driver1_id";

                List<EcoScoreReportCompareDrivers> lstCompareDrivers = (List<EcoScoreReportCompareDrivers>)await _dataMartdataAccess.QueryAsync<EcoScoreReportCompareDrivers>(query, parameters);
                return lstCompareDrivers?.Count > 0 ? lstCompareDrivers : new List<EcoScoreReportCompareDrivers>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Eco Score Compare Report Attributes with KPI Details
        /// </summary>
        /// <param name="reportId">ReportID</param>
        /// <param name="targetProfileId">ProfileID</param>
        /// <returns></returns>
        public async Task<List<EcoScoreCompareReportAtttributes>> GetEcoScoreCompareReportAttributes(int reportId, int targetProfileId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ReportId", reportId);
                parameters.Add("@ProfileId", targetProfileId);

                string query = @"SELECT da.id as DataAttributeId,da.name as Name, ra.key as Key, ra.sub_attribute_ids as SubDataAttributes, ra.name as dbcolumnname,
                                 eco.limit_type as LimitType, prokpi.limit_val as LimitValue, prokpi.target_val as TargetValue, eco.range_value_Type as RangeValueType
                                 FROM master.reportattribute ra 
                                 INNER JOIN master.dataattribute da ON ra.data_attribute_id = da.id and da.name not like '%Graph%'
                                 LEFT JOIN master.ecoscorekpi eco ON eco.data_attribute_id = da.id
                                 LEFT JOIN master.ecoscoreprofilekpi prokpi ON prokpi.ecoscore_kpi_id = eco.id AND prokpi.ecoscore_profile_id = @ProfileId --2
                                 WHERE ra.report_id = @ReportId --10
                                 ORDER BY ra.data_attribute_id";

                List<EcoScoreCompareReportAtttributes> lastAttributes = (List<EcoScoreCompareReportAtttributes>)await _dataAccess.QueryAsync<EcoScoreCompareReportAtttributes>(query, parameters);
                return lastAttributes?.Count > 0 ? lastAttributes : new List<EcoScoreCompareReportAtttributes>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Eco-Score Data service

        public Task<bool> GetKPIInfo(EcoScoreDataServiceRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<bool> GetChartInfo(EcoScoreDataServiceRequest request)
        {
            return Task.FromResult(true);
        }

        #endregion
    }
}
