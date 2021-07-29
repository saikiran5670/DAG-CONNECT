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

        public async Task<int> CreateEcoScoreProfile(EcoScoreProfileDto dto)
        {
            var now = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            _dataAccess.Connection.Open();
            IDbTransaction txn = _dataAccess.Connection.BeginTransaction();
            try
            {
                //Insert into EcoScoreProfile table
                string query = @"INSERT INTO master.ecoscoreprofile
                                (organization_id,name,description,default_es_version_type, state, created_at, created_by, modified_at, modified_by) 
                                VALUES 
                                (@organization_id, @name, @description, NULL, @state, @created_at, @created_by, @modified_at, @modified_by) RETURNING id";

                var parameters = new DynamicParameters();
                parameters.Add("@organization_id", dto.OrganizationId > 0 ? dto.OrganizationId : new int?());
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
                        (ecoscore_profile_id,ecoscore_kpi_id,limit_val,target_val, lower_val, upper_val, created_at, created_by, limit_type, modified_at, modified_by)
                        VALUES
                        (@profile_id, @ecoscore_kpi_id, @limit_val, @target_val, @lower_val, @upper_val, @created_at, @created_by, @limit_type, @modified_at, @modified_by) RETURNING id";

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
                    parameters.Add("@limit_type", profileKPI.LimitType);

                    await _dataAccess.ExecuteScalarAsync<int>(query, parameters);
                }
                txn.Commit();
                return id;
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
            objProfile.OrganizationId = profile.organizationid ?? new int();
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
                    objProfile.OrganizationId = pro.organizationid ?? new int();
                    objProfile.Name = pro.profilename;
                    objProfile.Description = pro.profiledescription;
                    objProfile.ActionedBy = pro.actionedby;
                    objProfile.LastUpdate = pro.lastupdate ?? UTCHandling.GetUTCFromDateTime(default(DateTime));
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
        public async Task<int> UpdateEcoScoreProfile(EcoScoreProfileDto ecoScoreProfileDto, bool isAdminRights)
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
                if (isAdminRights)
                {
                    if (ecoScoreProfileDto.IsDAFStandard)
                    {
                        updateParameter.Add("@organizationId", null);
                    }
                    else
                    {
                        updateParameter.Add("@organizationId", ecoScoreProfileDto.OrganizationId);
                    }
                    queryForUpdateEcoScoreProfile.Append(", organization_id=@organizationId");
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
        private async Task<bool> UpdateEcoscoreProfileKpi(List<EcoScoreProfileKPI> ecoScoreProfileKPI, int actionedBy, int profileId)
        {
            int id = 0;
            foreach (var item in ecoScoreProfileKPI)
            {
                var updateParameter = new DynamicParameters();
                StringBuilder query = new StringBuilder();

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
                updateParameter.Add("@Id", profileId);

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

        public async Task<bool> CheckEcoScoreProfileIsExist(int organizationId, string name, int profileId)
        {
            var parameterDuplicate = new DynamicParameters();
            string query;

            if (organizationId > 0)
            {
                query = "SELECT id FROM master.ecoscoreprofile where state ='A' and name=@name and id <> @profileId  and organization_id = @organization_id ";
            }
            else
            {
                query = "SELECT id FROM master.ecoscoreprofile where state ='A' and name=@name and id <> @profileId  and organization_id is null";
            }

            parameterDuplicate.Add("@name", name);
            parameterDuplicate.Add("@organization_id", organizationId);
            parameterDuplicate.Add("@profileId", profileId);
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
            var query = "select exists(select 1 from master.ecoscoreprofile where id= @ProfileId and organization_id is null and state = 'A')";
            parameter.Add("@ProfileId", profileId);

            return await _dataAccess.ExecuteScalarAsync<bool>(query, parameter);
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
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH ecoscore AS (
                                 SELECT dr.first_name, dr.last_name, eco.driver1_id, eco.trip_distance,
                                 eco.dpa_braking_score, eco.dpa_braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count
                                 FROM tripdetail.ecoscoredata eco
                                 JOIN master.driver dr 
                                 	ON dr.driver_id = eco.driver1_id
                                 WHERE eco.start_time >= @FromDate
                                 	AND eco.end_time <= @ToDate
                                 	AND eco.vin = ANY( @Vins )
                                 	AND (eco.trip_distance >= @MinTripDistance OR @MinTripDistance IS NULL)
                                    AND dr.organization_id=@OrgId
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
                                 driverName, driverid, cast(ecoscoreranking as decimal(18,1)) as ecoscoreranking
                                 FROM ecoscorealldriver
                                 where 1=1 AND (totaldriverdistance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)
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
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH 
                                ecoscorequery as (
                                    SELECT dr.first_name, dr.last_name, eco.driver1_id, eco.trip_distance,eco.trip_id,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
                                    eco.vin,eco.used_fuel,eco.pto_duration,eco.end_time,eco.start_time,eco.gross_weight_combination_total,
                                    eco.heavy_throttle_pedal_duration,eco.idle_duration,eco.harsh_brake_duration,eco.brake_duration,
                                    eco.cruise_control_usage , eco.cruise_control_usage_30_50,eco.cruise_control_usage_50_75,eco.cruise_control_usage_75
                                    FROM tripdetail.ecoscoredata eco
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                    WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) -- AND eco.vin = ANY('{XLR0998HGFFT76657,XLR0998HGFFT74600}')
                                    AND eco.driver1_id = ANY(@DriverIds)
                                    AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                    AND dr.organization_id=@OrgId
                                ),
                                
                                generalblk as 
                                (
                                    select eco.driver1_id, eco.first_name || ' ' || eco.last_name AS driverName, count(eco.driver1_id)  as drivercnt
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id,eco.first_name,eco.last_name
                                ),
                                AverageGrossweight as 
                                (
                                    select eco.driver1_id, (CAST(SUM (eco.gross_weight_combination_total)as DOUBLE PRECISION))  as AverageGrossweight
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
                                    select eco.driver1_id, (SUM(eco.trip_distance) / CEIL(CAST(MAX(eco.end_time) - MIN(eco.start_time) AS DOUBLE PRECISION)/(1000 * 60 * 60 * 24))) as AverageDistancePerDay
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                EcoScore as
                                (
                                    SELECT eco.driver1_id ,
                                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN  
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
                                    SELECT eco.driver1_id, CASE WHEN SUM(trip_distance) <> 0 THEN 
									((CAST(SUM (eco.cruise_control_usage) AS DOUBLE PRECISION ))/ SUM(trip_distance))*100  
									ELSE null END as CruiseControlUsage
									FROM ecoscorequery eco
									GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage30 as 
                                (
                                    SELECT eco.driver1_id, CASE WHEN SUM(trip_distance) <> 0 THEN 
									((CAST(SUM (eco.cruise_control_usage_30_50) AS DOUBLE PRECISION ))/SUM(trip_distance))*100  
									ELSE null END as CruiseControlUsage30
									FROM ecoscorequery eco
									GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage50 as 
                                (
                                    SELECT eco.driver1_id, CASE WHEN SUM(trip_distance) <> 0 THEN 
									((CAST(SUM (eco.cruise_control_usage_50_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100  
									ELSE null END as CruiseControlUsage50
									FROM ecoscorequery eco
									GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage75 as 
                                (
                                   SELECT eco.driver1_id, CASE WHEN SUM(trip_distance) <> 0 THEN 
								   ((CAST(SUM (eco.cruise_control_usage_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100   
								   ELSE null END as CruiseControlUsage75
								   FROM ecoscorequery eco
								   GROUP BY eco.driver1_id
                                ),
                                PTOUsage as 
                                (
                                    SELECT eco.driver1_id,
                                    CASE WHEN ( SUM (eco.end_time)- SUM (eco.start_time) ) <> 0 and (( SUM (eco.end_time)- SUM (eco.start_time) )/1000) <>0 THEN
                                    (SUM(eco.pto_duration) / (( SUM (eco.end_time)- SUM (eco.start_time) )/1000))*100 
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
                                   CASE WHEN ((( (SUM (eco.end_time)) - (SUM (eco.start_time)) )/1000)- (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))  ) <> 0  THEN
                                     (CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION) )  /((( (SUM (eco.end_time)) - (SUM (eco.start_time))  )/1000)-   (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))    )  
                                   ELSE null END as AverageDrivingSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                AverageSpeed as
                                (
                                   SELECT eco.driver1_id, 
                                   CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <>0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0 then
                                   SUM(eco.trip_distance)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)  
                                   ELSE null END as AverageSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                HeavyThrottling as
                                (
                                    SELECT eco.driver1_id,
                                    CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)<>0 THEN
                                    (SUM(eco.heavy_throttle_pedal_duration)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) )*100 
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
                                    CASE WHEN ( (SUM (eco.end_time))- (SUM (eco.start_time)))<> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0  THEN 
                                    ( CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION)/ (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))* 100
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
                                   SELECT eco.driver1_id,( CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION)/ NULLIF( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)),0))*100 as HarshBraking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                HarshBrakeDuration  as
                                (
                                   SELECT eco.driver1_id, CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION) as HarshBrakeDuration
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
                                   SELECT eco.driver1_id,
	                               case when ((SUM (eco.end_time))-(SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) <>0 THEN 
                                   ( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION))/ (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) - CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION)) * 100 
	                               ELSE null END as Braking
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
                                
                                ,CAST(avrg.averagegrossweight/1000 AS DOUBLE PRECISION) as AverageGrossweight -- convert kg weight to tonnes by /1000
                                ,CAST(dis.distance/1000 AS DOUBLE PRECISION) as Distance  -- convert meter to km by /1000
                                ,CAST(notrp.numberoftrips AS DOUBLE PRECISION) as NumberOfTrips
                                ,CAST(noveh.numberofvehicles AS DOUBLE PRECISION) as NumberOfVehicles
                                ,CAST(avgdperday.averagedistanceperday/1000 AS DOUBLE PRECISION) as AverageDistancePerDay -- convert meter to km by /1000
                                
                                ,CAST(ecos.ecoscore AS DOUBLE PRECISION) as EcoScore
                                ,CAST(f.fuelconsumption/1000 AS DOUBLE PRECISION) as FuelConsumption
                                ,CAST(crus.cruisecontrolusage AS DOUBLE PRECISION) as CruiseControlUsage
                                ,CAST(crusa.cruisecontrolusage30 AS DOUBLE PRECISION) as CruiseControlUsage30
                                ,CAST(crucon.cruisecontrolusage50 AS DOUBLE PRECISION) as CruiseControlUsage50
                                ,CAST(crucont.cruisecontrolusage75 AS DOUBLE PRECISION) as CruiseControlUsage75
                                ,CAST(p.ptousage*100 AS DOUBLE PRECISION) as PTOUsage --convert Count to % by *100
                                ,CAST(pto.ptoduration AS DOUBLE PRECISION) as PTODuration
                                ,CAST(ads.averagedrivingspeed * 3.6 AS DOUBLE PRECISION) as AverageDrivingSpeed  --convert meter/second  to kmph  by *3.6
                                ,CAST(aspeed.averagespeed * 3.6 AS DOUBLE PRECISION) as AverageSpeed --convert meter/second  to kmph  by *3.6
                                ,CAST(h.heavythrottling *100 AS DOUBLE PRECISION) as HeavyThrottling  -- Conver count to % by *100
                                ,CAST(he.heavythrottleduration AS DOUBLE PRECISION) as HeavyThrottleDuration
                                ,CAST(i.idling AS DOUBLE PRECISION) as Idling  -- Conver count to % by *100
                                ,CAST(ide.idleduration AS DOUBLE PRECISION) as IdleDuration
                                ,CAST(br.brakingscore AS DOUBLE PRECISION) as BrakingScore
                                ,CAST(hr.harshbraking * 100 AS DOUBLE PRECISION) as HarshBraking -- Conver count to % by *100
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
                                Left join Braking brk on brk.driver1_id = brdur.driver1_id

                                where 1 = 1 AND(dis.distance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)";
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

        #region Eco Score Report Single Driver
        /// <summary>
        /// Get Eco Score Report Single Driver - Overall Driver 
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<IEnumerable<EcoScoreReportSingleDriver>> GetEcoScoreReportOverallDriver(EcoScoreReportSingleDriverRequest request)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH 
                                ecoscorequery as (
                                    SELECT dr.first_name, dr.last_name, eco.driver1_id, eco.trip_distance,eco.trip_id,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
                                    eco.vin,eco.used_fuel,eco.pto_duration,eco.end_time,eco.start_time,eco.gross_weight_combination_total,
                                    eco.heavy_throttle_pedal_duration,eco.idle_duration,eco.harsh_brake_duration,eco.brake_duration,
                                    eco.cruise_control_usage , eco.cruise_control_usage_30_50,eco.cruise_control_usage_50_75,eco.cruise_control_usage_75
                                    FROM tripdetail.ecoscoredata eco
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                    WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) --ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                    AND eco.driver1_id = @DriverId --ANY('{NL B000384974000000}')
                                    AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                    AND dr.organization_id=@OrgId
                                ),
                                
                                generalblk as 
                                (
                                    select eco.driver1_id, eco.first_name || ' ' || eco.last_name AS driverName, count(eco.driver1_id)  as drivercnt
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id ,eco.first_name,eco.last_name
                                ),
                                AverageGrossweight as 
                                (
                                    select eco.driver1_id, (CAST(SUM (eco.gross_weight_combination_total)as DOUBLE PRECISION))  as AverageGrossweight
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
                                    select eco.driver1_id, (SUM(eco.trip_distance) / CEIL(CAST(MAX(eco.end_time) - MIN(eco.start_time) AS DOUBLE PRECISION)/(1000 * 60 * 60 * 24))) as AverageDistancePerDay
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id
                                ),
                                EcoScore as
                                (
                                    SELECT eco.driver1_id ,
                                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN  
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
                                    SELECT eco.driver1_id,  CASE WHEN SUM(trip_distance) <>0 THEN
									((CAST(SUM (eco.cruise_control_usage) AS DOUBLE PRECISION ))/ SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage
									FROM ecoscorequery eco
									GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage30 as 
                                (
                                    SELECT eco.driver1_id,  CASE WHEN SUM(trip_distance) <>0 THEN
									((CAST(SUM (eco.cruise_control_usage_30_50) AS DOUBLE PRECISION ))/ SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage30
									FROM ecoscorequery eco
									GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage50 as 
                                (
                                    SELECT eco.driver1_id, CASE WHEN SUM(trip_distance) <>0 THEN
									((CAST(SUM (eco.cruise_control_usage_50_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage50
									FROM ecoscorequery eco
									GROUP BY eco.driver1_id
                                ),
                                CruiseControlUsage75 as 
                                (
                                   SELECT eco.driver1_id, CASE WHEN SUM(trip_distance) <>0 THEN
								   ((CAST(SUM (eco.cruise_control_usage_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100  
								   ELSE null END as  CruiseControlUsage75
								   FROM ecoscorequery eco
								   GROUP BY eco.driver1_id
                                ),
                                PTOUsage as 
                                (
                                    SELECT eco.driver1_id,
                                    CASE WHEN ( SUM (eco.end_time)- SUM (eco.start_time) ) <> 0 and (( SUM (eco.end_time)- SUM (eco.start_time) )/1000) <>0 THEN
                                    (SUM(eco.pto_duration) / (( SUM (eco.end_time)- SUM (eco.start_time) )/1000))*100 
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
                                   CASE WHEN ((( (SUM (eco.end_time)) - (SUM (eco.start_time)) )/1000)- (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))  ) <> 0  THEN
                                     (CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION) )  /((( (SUM (eco.end_time)) - (SUM (eco.start_time))  )/1000)-   (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))    )  
                                   ELSE null END as AverageDrivingSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                AverageSpeed as
                                (
                                   SELECT eco.driver1_id, 
                                   CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <>0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0 then
                                   SUM(eco.trip_distance)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)  
                                   ELSE null END as AverageSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                HeavyThrottling as
                                (
                                    SELECT eco.driver1_id,
                                    CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)<>0 THEN
                                    (SUM(eco.heavy_throttle_pedal_duration)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))*100  
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
                                    CASE WHEN ( (SUM (eco.end_time))- (SUM (eco.start_time)))<> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0  THEN 
                                    ( CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION)/ (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))* 100
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
                                   SELECT eco.driver1_id, (CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION)/ NULLIF( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)),0))*100 as HarshBraking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id
                                ),
                                HarshBrakeDuration  as
                                (
                                   SELECT eco.driver1_id, CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION) as HarshBrakeDuration
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
                                   SELECT eco.driver1_id,
	                               case when ((SUM (eco.end_time))-(SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) <>0 THEN 
                                   ( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION))/ (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) - CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))*100 
	                               ELSE null END as Braking
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
                                ,eco.DriverName ,  'Overall_Driver' as HeaderType
                                
                                ,CAST(avrg.averagegrossweight/1000 AS DOUBLE PRECISION) as AverageGrossweight -- convert kg weight to tonnes by /1000
                                ,CAST(dis.distance/1000 AS DOUBLE PRECISION) as Distance  -- convert meter to km by /1000
                                ,CAST(notrp.numberoftrips AS DOUBLE PRECISION) as NumberOfTrips
                                ,CAST(noveh.numberofvehicles AS DOUBLE PRECISION) as NumberOfVehicles
                                ,CAST(avgdperday.averagedistanceperday/1000 AS DOUBLE PRECISION) as AverageDistancePerDay -- convert meter to km by /1000
                                
                                ,CAST(ecos.ecoscore AS DOUBLE PRECISION) as EcoScore
                                ,CAST(f.fuelconsumption/1000 AS DOUBLE PRECISION) as FuelConsumption
                                ,CAST(crus.cruisecontrolusage AS DOUBLE PRECISION) as CruiseControlUsage
                                ,CAST(crusa.cruisecontrolusage30 AS DOUBLE PRECISION) as CruiseControlUsage30
                                ,CAST(crucon.cruisecontrolusage50 AS DOUBLE PRECISION) as CruiseControlUsage50
                                ,CAST(crucont.cruisecontrolusage75 AS DOUBLE PRECISION) as CruiseControlUsage75
                                ,CAST(p.ptousage*100 AS DOUBLE PRECISION) as PTOUsage --convert Count to % by *100
                                ,CAST(pto.ptoduration AS DOUBLE PRECISION) as PTODuration
                                ,CAST(ads.averagedrivingspeed * 3.6 AS DOUBLE PRECISION) as AverageDrivingSpeed  --convert meter/second  to kmph  by *3.6
                                ,CAST(aspeed.averagespeed * 3.6 AS DOUBLE PRECISION) as AverageSpeed --convert meter/second  to kmph  by *3.6
                                ,CAST(h.heavythrottling *100 AS DOUBLE PRECISION) as HeavyThrottling  -- Conver count to % by *100
                                ,CAST(he.heavythrottleduration AS DOUBLE PRECISION) as HeavyThrottleDuration
                                ,CAST(i.idling AS DOUBLE PRECISION) as Idling  -- Conver count to % by *100
                                ,CAST(ide.idleduration AS DOUBLE PRECISION) as IdleDuration
                                ,CAST(br.brakingscore AS DOUBLE PRECISION) as BrakingScore
                                ,CAST(hr.harshbraking * 100 AS DOUBLE PRECISION) as HarshBraking -- Conver count to % by *100
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
                                Left join Braking brk on brk.driver1_id = brdur.driver1_id

                                where 1 = 1 AND(dis.distance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)";

                return await _dataMartdataAccess.QueryAsync<EcoScoreReportSingleDriver>(query, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Eco Score Report Single Driver - Overall Company 
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<IEnumerable<EcoScoreReportSingleDriver>> GetEcoScoreReportOverallCompany(EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH 
                                ecoscorequery as (
                                    SELECT dr.organization_id, dr.first_name, dr.last_name, eco.driver1_id, eco.trip_distance,eco.trip_id,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
                                    eco.vin,eco.used_fuel,eco.pto_duration,eco.end_time,eco.start_time,eco.gross_weight_combination_total,
                                    eco.heavy_throttle_pedal_duration,eco.idle_duration,eco.harsh_brake_duration,eco.brake_duration,
									eco.cruise_control_usage , eco.cruise_control_usage_30_50,eco.cruise_control_usage_50_75,eco.cruise_control_usage_75
                                    FROM tripdetail.ecoscoredata eco
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                    WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) --ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                    --AND eco.driver1_id = @DriverId --ANY('{NL B000384974000000}')
                                    AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                    AND dr.organization_id=@OrgId
                                ),
                                
                                generalblk as 
                                (
                                    select eco.organization_id, count(eco.driver1_id)  as drivercnt
                                    FROM ecoscorequery eco
                                    Where eco.driver1_id =@DriverId
                                    GROUP BY eco.organization_id
                                ) ,
								 AverageGrossweight as 
                                (
                                    select eco.organization_id, (CAST(SUM (eco.gross_weight_combination_total)as DOUBLE PRECISION))  as AverageGrossweight
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
								Distance as 
                                (
                                    select eco.organization_id, (CAST(SUM (eco.trip_distance)as DOUBLE PRECISION)) as Distance
                                    FROM ecoscorequery eco
                                     GROUP BY eco.organization_id 
                                ),
                                NumberOfTrips as 
                                (
                                    select eco.organization_id,   COUNT (eco.trip_id) as NumberOfTrips
                                    FROM ecoscorequery eco
                                     GROUP BY eco.organization_id 
                                ),
                                NumberOfVehicles as 
                                (
                                    select eco.organization_id ,  COUNT (eco.vin) as NumberOfVehicles
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
                                AverageDistancePerDay as 
                                (
                                    select eco.organization_id , (SUM(eco.trip_distance) / CEIL(CAST(MAX(eco.end_time) - MIN(eco.start_time) AS DOUBLE PRECISION)/(1000 * 60 * 60 * 24))) as AverageDistancePerDay
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
								EcoScore as
                                (
                                    SELECT eco.organization_id ,
                                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN  
                                    (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
                                      (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
                                    else null END as ecoscore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
                                FuelConsumption as 
                                (
                                    SELECT eco.organization_id , (CAST(SUM (eco.used_fuel)AS DOUBLE PRECISION )) as FuelConsumption
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ) ,
                                CruiseControlUsage as 
                                (
                                    SELECT eco.organization_id , CASE WHEN SUM(trip_distance) <>0 THEN
									((CAST(SUM (eco.cruise_control_usage) AS DOUBLE PRECISION ))/ SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
                                CruiseControlUsage30 as 
                                (
                                    SELECT eco.organization_id , CASE WHEN SUM(trip_distance) <>0 THEN 
									((CAST(SUM (eco.cruise_control_usage_30_50) AS DOUBLE PRECISION ))/SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage30
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
                                CruiseControlUsage50 as 
                                (
                                    SELECT eco.organization_id , CASE WHEN SUM(trip_distance) <>0 THEN 
									((CAST(SUM (eco.cruise_control_usage_50_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage50
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
                                CruiseControlUsage75 as 
                                (
                                   SELECT eco.organization_id , CASE WHEN SUM(trip_distance) <>0 THEN 
								   ((CAST(SUM (eco.cruise_control_usage_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100  
								   ELSE null END as CruiseControlUsage75
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                PTOUsage as 
                                (
                                    SELECT eco.organization_id ,
                                    CASE WHEN ( SUM (eco.end_time)- SUM (eco.start_time) ) <> 0 and (( SUM (eco.end_time)- SUM (eco.start_time) )/1000) <>0 THEN
                                    (SUM(eco.pto_duration) / (( SUM (eco.end_time)- SUM (eco.start_time) )/1000))*100 
                                    ELSE null END as PTOUsage
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 			 
                                ),
                                PTODuration as 
                                (
                                   SELECT eco.organization_id , SUM(eco.pto_duration) as PTODuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                AverageDrivingSpeed as
                                (  
                                   SELECT eco.organization_id ,
                                   CASE WHEN ((( (SUM (eco.end_time)) - (SUM (eco.start_time)) )/1000)- (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))  ) <> 0  THEN
                                     (CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION) )  /((( (SUM (eco.end_time)) - (SUM (eco.start_time))  )/1000)-   (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))    )  
                                   ELSE null END as AverageDrivingSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                AverageSpeed as
                                (
                                   SELECT eco.organization_id , 
                                   CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <>0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0 then
                                   SUM(eco.trip_distance)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)  
                                   ELSE null END as AverageSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                HeavyThrottling as
                                (
                                    SELECT eco.organization_id , 
                                    CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)<>0 THEN
                                    (SUM(eco.heavy_throttle_pedal_duration)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))*100  
                                    ELSE null END as HeavyThrottling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
                                HeavyThrottleDuration  as
                                (
                                    SELECT eco.organization_id ,  (CAST(SUM(eco.heavy_throttle_pedal_duration ) AS DOUBLE PRECISION)) as HeavyThrottleDuration
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
                                Idling  as
                                (
                                    SELECT eco.organization_id ,
                                    CASE WHEN ( (SUM (eco.end_time))- (SUM (eco.start_time)))<> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0  THEN 
                                    ( CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION)/ (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))* 100
                                    ELSE null end
                                    as Idling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                ),
                                IdleDuration  as
                                (
                                   SELECT eco.organization_id ,  CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)   as IdleDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                BrakingScore  as
                                (
                                   SELECT eco.organization_id , ( CAST(SUM(eco.dpa_Braking_score) AS DOUBLE PRECISION)/ NULLIF ( (CAST(SUM (eco.dpa_Braking_count)AS DOUBLE PRECISION)),0))/10   as BrakingScore
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                HarshBraking  as
                                (
                                   SELECT eco.organization_id , (CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION)/ NULLIF( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)),0))*100 as HarshBraking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                HarshBrakeDuration  as
                                (
                                   SELECT eco.organization_id , CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION) as HarshBrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                BrakeDuration as
                                (
                                   SELECT eco.organization_id , CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)/ 86400 as BrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                Braking as
                                (
                                   SELECT eco.organization_id ,
	                               case when ((SUM (eco.end_time))-(SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) <>0 THEN 
                                   ( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION))/ (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) - CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))*100
	                               ELSE null END as Braking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id 
                                ),
                                AnticipationScore as
                                (
                                    SELECT eco.organization_id ,  (   (CAST(SUM(eco.dpa_anticipation_score)AS DOUBLE PRECISION ) ) / NULLIF(  (CAST (SUM(eco.dpa_anticipation_count) AS DOUBLE PRECISION) )  ,0) )/10 as AnticipationScore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id 
                                )
								select eco.organization_id as OrgId,
                                 'Overall_Company' as HeaderType
                                
                                 ,CAST(avrg.averagegrossweight/1000 AS DOUBLE PRECISION) as AverageGrossweight -- convert kg weight to tonnes by /1000
                                ,CAST(dis.distance/1000 AS DOUBLE PRECISION) as Distance  -- convert meter to km by /1000
                                ,CAST(notrp.numberoftrips AS DOUBLE PRECISION) as NumberOfTrips
                                ,CAST(noveh.numberofvehicles AS DOUBLE PRECISION) as NumberOfVehicles
                                ,CAST(avgdperday.averagedistanceperday/1000 AS DOUBLE PRECISION) as AverageDistancePerDay -- convert meter to km by /1000
                                
                                ,CAST(ecos.ecoscore AS DOUBLE PRECISION) as EcoScore
                                ,CAST(f.fuelconsumption/1000 AS DOUBLE PRECISION) as FuelConsumption
                                ,CAST(crus.cruisecontrolusage AS DOUBLE PRECISION) as CruiseControlUsage
                                ,CAST(crusa.cruisecontrolusage30 AS DOUBLE PRECISION) as CruiseControlUsage30
                                ,CAST(crucon.cruisecontrolusage50 AS DOUBLE PRECISION) as CruiseControlUsage50
                                ,CAST(crucont.cruisecontrolusage75 AS DOUBLE PRECISION) as CruiseControlUsage75
                                ,CAST(p.ptousage*100 AS DOUBLE PRECISION) as PTOUsage --convert Count to % by *100
                                ,CAST(pto.ptoduration AS DOUBLE PRECISION) as PTODuration
                                ,CAST(ads.averagedrivingspeed * 3.6 AS DOUBLE PRECISION) as AverageDrivingSpeed  --convert meter/second  to kmph  by *3.6
                                ,CAST(aspeed.averagespeed * 3.6 AS DOUBLE PRECISION) as AverageSpeed --convert meter/second  to kmph  by *3.6
                                ,CAST(h.heavythrottling *100 AS DOUBLE PRECISION) as HeavyThrottling  -- Conver count to % by *100
                                ,CAST(he.heavythrottleduration AS DOUBLE PRECISION) as HeavyThrottleDuration
                                ,CAST(i.idling AS DOUBLE PRECISION) as Idling  -- Conver count to % by *100
                                ,CAST(ide.idleduration AS DOUBLE PRECISION) as IdleDuration
                                ,CAST(br.brakingscore AS DOUBLE PRECISION) as BrakingScore
                                ,CAST(hr.harshbraking * 100 AS DOUBLE PRECISION) as HarshBraking -- Conver count to % by *100
                                ,CAST(hrdur.HarshBrakeDuration AS DOUBLE PRECISION) as HarshBrakeDuration
                                ,CAST(brdur.brakeduration AS DOUBLE PRECISION) as BrakeDuration
                                ,CAST(brk.braking AS DOUBLE PRECISION) as Braking
                                ,CAST(anc.anticipationscore  AS DOUBLE PRECISION) as AnticipationScore
                                
                                from generalblk eco
                                Left join AverageGrossweight avrg on avrg.organization_id = eco.organization_id
                                Left join Distance dis on dis.organization_id = avrg.organization_id
                                Left join NumberOfTrips notrp on notrp.organization_id = dis.organization_id
                                Left join numberofvehicles noveh on noveh.organization_id = notrp.organization_id
                                Left join AverageDistancePerDay avgdperday on avgdperday.organization_id = noveh.organization_id
                                Left join EcoScore ecos on ecos.organization_id = avgdperday.organization_id
                                Left join FuelConsumption f on f.organization_id = ecos.organization_id
                                Left join CruiseControlUsage crus  on crus.organization_id = f.organization_id   
                                Left join CruiseControlUsage30 crusa  on crusa.organization_id = crus.organization_id
                                Left join CruiseControlUsage50 crucon on crucon.organization_id = crusa.organization_id
                                Left join CruiseControlUsage75 crucont on crucont.organization_id = crucon.organization_id
                                Left join PTOUsage p on p.organization_id = crucont.organization_id
                                Left join PTODuration pto on pto.organization_id = p.organization_id
                                Left join AverageDrivingSpeed ads on ads.organization_id = pto.organization_id
                                
                                Left join AverageSpeed aspeed on aspeed.organization_id = ads.organization_id
                                Left join HeavyThrottling h on h.organization_id = aspeed.organization_id
                                Left join HeavyThrottleDuration he on he.organization_id = h.organization_id
                                Left join Idling i on i.organization_id = he.organization_id
                                Left join IdleDuration ide on ide.organization_id = i.organization_id
                                Left join BrakingScore br on br.organization_id = ide.organization_id
                                Left join HarshBraking hr on hr.organization_id = br.organization_id
                                Left join HarshBrakeDuration hrdur on hrdur.organization_id = hr.organization_id
                                Left join AnticipationScore anc on anc.organization_id = hrdur.organization_id
                                Left join BrakeDuration brdur on brdur.organization_id = anc.organization_id
                                Left join Braking brk on brk.organization_id = brdur.organization_id

                                where 1 = 1 AND(dis.distance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)";

                return await _dataMartdataAccess.QueryAsync<EcoScoreReportSingleDriver>(query, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Eco Score Report Single Driver - VIN Driver 
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<List<EcoScoreReportSingleDriver>> GetEcoScoreReportVINDriver(EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH 
                                ecoscorequery as (
                                    SELECT dr.first_name, dr.last_name, eco.driver1_id, eco.trip_distance,eco.trip_id,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
                                    eco.vin,eco.used_fuel,eco.pto_duration,eco.end_time,eco.start_time,eco.gross_weight_combination_total,
                                    eco.heavy_throttle_pedal_duration,eco.idle_duration,eco.harsh_brake_duration,eco.brake_duration,
									eco.cruise_control_usage , eco.cruise_control_usage_30_50,eco.cruise_control_usage_50_75,eco.cruise_control_usage_75
									,ve.registration_no,ve.name
                                    FROM tripdetail.ecoscoredata eco
									JOIN master.vehicle ve 
									ON eco.vin = ve.vin
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                    WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) --ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                    AND eco.driver1_id = @DriverId --ANY('{NL B000384974000000}')
                                    AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                    AND dr.organization_id=@OrgId
                                ),
                                
                                generalblk as 
                                (
                                    select eco.vin, eco.driver1_id, eco.registration_no,eco.name, eco.first_name || ' ' || eco.last_name AS driverName, count(eco.driver1_id)  as drivercnt
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin, eco.driver1_id, eco.registration_no,eco.name,eco.first_name,eco.last_name
                                ), 
								 AverageGrossweight as 
                                (
                                    select eco.vin, CAST(SUM (eco.gross_weight_combination_total)as DOUBLE PRECISION)  as AverageGrossweight
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin
                                ),
								Distance as 
                                ( 
                                    select eco.vin, CAST(SUM (eco.trip_distance)as DOUBLE PRECISION) as Distance
                                    FROM ecoscorequery eco
                                     GROUP BY eco.vin
                                ),
                                NumberOfTrips as 
                                (
                                    select eco.vin,  COUNT (eco.trip_id) as NumberOfTrips
                                    FROM ecoscorequery eco
                                     GROUP BY eco.vin
                                ),
                                NumberOfVehicles as 
                                (
                                    select eco.vin,  COUNT (eco.vin) as NumberOfVehicles
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin
                                ),
                                AverageDistancePerDay as 
                                ( 
                                    select eco.vin,  (SUM(eco.trip_distance) / CEIL(CAST(MAX(eco.end_time) - MIN(eco.start_time) AS DOUBLE PRECISION)/(1000 * 60 * 60 * 24))) as AverageDistancePerDay
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin
                                ),
                                EcoScore as
                                (
                                    SELECT eco.vin, 
                                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN  
                                    (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
                                      (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
                                    else null END as ecoscore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin
                                ),
                                FuelConsumption as 
                                (
                                    SELECT eco.vin,  (CAST(SUM (eco.used_fuel)AS DOUBLE PRECISION )) as FuelConsumption
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin
                                ),
                                CruiseControlUsage as 
                                (
                                    SELECT eco.vin, CASE WHEN SUM(trip_distance)<>0 THEN
									((CAST(SUM (eco.cruise_control_usage) AS DOUBLE PRECISION ))/ SUM(trip_distance))*100  
									ELSE null END as CruiseControlUsage
									FROM ecoscorequery eco
									GROUP BY eco.vin
                                ),
                                CruiseControlUsage30 as 
                                (
                                    SELECT eco.vin, CASE WHEN SUM(trip_distance)<>0 THEN
									((CAST(SUM (eco.cruise_control_usage_30_50) AS DOUBLE PRECISION ))/SUM(trip_distance))*100   
									ELSE null END as CruiseControlUsage30
									FROM ecoscorequery eco
									GROUP BY eco.vin
                                ),
                                CruiseControlUsage50 as 
                                (
                                    SELECT eco.vin, CASE WHEN SUM(trip_distance)<>0 THEN
									((CAST(SUM (eco.cruise_control_usage_50_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100  
									ELSE null END as CruiseControlUsage50
									FROM ecoscorequery eco
									GROUP BY eco.vin
                                ),
                                CruiseControlUsage75 as 
                                (
                                   SELECT eco.vin, CASE WHEN SUM(trip_distance)<>0 THEN
								   ((CAST(SUM (eco.cruise_control_usage_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100   
								   ELSE null END as CruiseControlUsage75
								   FROM ecoscorequery eco
								   GROUP BY eco.vin
                                ),
                                PTOUsage as 
                                (
                                    SELECT eco.vin, 
                                    CASE WHEN ( SUM (eco.end_time)- SUM (eco.start_time) ) <> 0 and (( SUM (eco.end_time)- SUM (eco.start_time) )/1000) <>0 THEN
                                    (SUM(eco.pto_duration) / (( SUM (eco.end_time)- SUM (eco.start_time) )/1000))*100 
                                    ELSE null END as PTOUsage
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin				 
                                ),
                                PTODuration as 
                                (
                                   SELECT eco.vin,   SUM(eco.pto_duration)  as PTODuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin	
                                ),
                                AverageDrivingSpeed as
                                (  
								   SELECT eco.vin, 
                                   CASE WHEN ((( (SUM (eco.end_time)) - (SUM (eco.start_time)) )/1000)- (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))  ) <> 0  THEN
                                    (CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION) )  /((( (SUM (eco.end_time)) - (SUM (eco.start_time))  )/1000)-(CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)))
                                   ELSE null END as AverageDrivingSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin
                                ),
                                AverageSpeed as
                                (
                                   SELECT eco.vin, 
                                   CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <>0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0 then
                                   SUM(eco.trip_distance)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)
                                   ELSE null END as AverageSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin
                                ),
                                HeavyThrottling as
                                (
                                    SELECT eco.vin, 
                                    CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)<>0 THEN
                                    (SUM(eco.heavy_throttle_pedal_duration)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))*100 
                                    ELSE null END as HeavyThrottling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin
                                ),
                                HeavyThrottleDuration  as
                                (
                                    SELECT eco.vin,  (CAST(SUM(eco.heavy_throttle_pedal_duration ) AS DOUBLE PRECISION)) as HeavyThrottleDuration
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin
                                ),
                                Idling  as
                                (
									
                                    SELECT eco.vin, 
                                    CASE WHEN ( (SUM (eco.end_time))- (SUM (eco.start_time)))<> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0  THEN 
                                    ( CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION)/ (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))* 100
                                    ELSE null end
                                    as Idling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin
                                ),
                                IdleDuration  as
                                (
                                   SELECT eco.vin,   CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)   as IdleDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin
                                ),
                                BrakingScore  as
                                (
                                   SELECT eco.vin, ( CAST(SUM(eco.dpa_Braking_score) AS DOUBLE PRECISION)/ NULLIF ( (CAST(SUM (eco.dpa_Braking_count)AS DOUBLE PRECISION)),0))/10   as BrakingScore
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin
                                ),
                                HarshBraking  as
                                (
                                   SELECT eco.vin,  (CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION)/ NULLIF( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)),0))*100 as HarshBraking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin
                                ),
                                HarshBrakeDuration  as
                                (
                                   SELECT eco.vin,  CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION) as HarshBrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin
                                ),
                                BrakeDuration as
                                (
                                   SELECT eco.vin,  CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)/ 86400 as BrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin
                                ),
                                Braking as
                                (
                                   SELECT eco.vin, 
	                               case when ((SUM (eco.end_time))-(SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) <>0 THEN 
                                   ( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION))/ (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) - CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))*100
	                               ELSE null END as Braking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin
                                ),
                                AnticipationScore as
                                (
                                    SELECT eco.vin,  (   (CAST(SUM(eco.dpa_anticipation_score)AS DOUBLE PRECISION ) ) / NULLIF(  (CAST (SUM(eco.dpa_anticipation_count) AS DOUBLE PRECISION) )  ,0) )/10 as AnticipationScore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin
                                )
                                select eco.driver1_id as DriverId,eco.vin as VIN
                                ,eco.DriverName, eco.registration_no as RegistrationNo,eco.name as VehicleName, 'VIN_Driver' as HeaderType
                                
                                ,CAST(avrg.averagegrossweight/1000 AS DOUBLE PRECISION) as AverageGrossweight -- convert kg weight to tonnes by /1000
                                ,CAST(dis.distance/1000 AS DOUBLE PRECISION) as Distance  -- convert meter to km by /1000
                                ,CAST(notrp.numberoftrips AS DOUBLE PRECISION) as NumberOfTrips
                                ,CAST(noveh.numberofvehicles AS DOUBLE PRECISION) as NumberOfVehicles
                                ,CAST(avgdperday.averagedistanceperday/1000 AS DOUBLE PRECISION) as AverageDistancePerDay -- convert meter to km by /1000
                                
                                ,CAST(ecos.ecoscore AS DOUBLE PRECISION) as EcoScore
                                ,CAST(f.fuelconsumption/1000 AS DOUBLE PRECISION) as FuelConsumption
                                ,CAST(crus.cruisecontrolusage AS DOUBLE PRECISION) as CruiseControlUsage
                                ,CAST(crusa.cruisecontrolusage30 AS DOUBLE PRECISION) as CruiseControlUsage30
                                ,CAST(crucon.cruisecontrolusage50 AS DOUBLE PRECISION) as CruiseControlUsage50
                                ,CAST(crucont.cruisecontrolusage75 AS DOUBLE PRECISION) as CruiseControlUsage75
                                ,CAST(p.ptousage*100 AS DOUBLE PRECISION) as PTOUsage --convert Count to % by *100
                                ,CAST(pto.ptoduration AS DOUBLE PRECISION) as PTODuration
                                ,CAST(ads.averagedrivingspeed * 3.6 AS DOUBLE PRECISION) as AverageDrivingSpeed  --convert meter/second  to kmph  by *3.6
                                ,CAST(aspeed.averagespeed * 3.6 AS DOUBLE PRECISION) as AverageSpeed --convert meter/second  to kmph  by *3.6
                                ,CAST(h.heavythrottling *100 AS DOUBLE PRECISION) as HeavyThrottling  -- Conver count to % by *100
                                ,CAST(he.heavythrottleduration AS DOUBLE PRECISION) as HeavyThrottleDuration
                                ,CAST(i.idling AS DOUBLE PRECISION) as Idling  -- Conver count to % by *100
                                ,CAST(ide.idleduration AS DOUBLE PRECISION) as IdleDuration
                                ,CAST(br.brakingscore AS DOUBLE PRECISION) as BrakingScore
                                ,CAST(hr.harshbraking * 100 AS DOUBLE PRECISION) as HarshBraking -- Conver count to % by *100
                                ,CAST(hrdur.HarshBrakeDuration AS DOUBLE PRECISION) as HarshBrakeDuration
                                ,CAST(brdur.brakeduration AS DOUBLE PRECISION) as BrakeDuration
                                ,CAST(brk.braking AS DOUBLE PRECISION) as Braking
                                ,CAST(anc.anticipationscore  AS DOUBLE PRECISION) as AnticipationScore
                                
                                from generalblk eco
                                Left join AverageGrossweight avrg on avrg.vin = eco.vin
                                Left join Distance dis on dis.vin = avrg.vin
                                Left join NumberOfTrips notrp on notrp.vin = dis.vin
                                Left join numberofvehicles noveh on noveh.vin = notrp.vin
                                Left join AverageDistancePerDay avgdperday on avgdperday.vin = noveh.vin
                                Left join EcoScore ecos on ecos.vin = avgdperday.vin
                                Left join FuelConsumption f on f.vin = ecos.vin
                                Left join CruiseControlUsage crus  on crus.vin = f.vin   
                                Left join CruiseControlUsage30 crusa  on crusa.vin = crus.vin
                                Left join CruiseControlUsage50 crucon on crucon.vin = crusa.vin
                                Left join CruiseControlUsage75 crucont on crucont.vin = crucon.vin
                                Left join PTOUsage p on p.vin = crucont.vin
                                Left join PTODuration pto on pto.vin = p.vin
                                Left join AverageDrivingSpeed ads on ads.vin = pto.vin
                                
                                Left join AverageSpeed aspeed on aspeed.vin = ads.vin
                                Left join HeavyThrottling h on h.vin = aspeed.vin
                                Left join HeavyThrottleDuration he on he.vin = h.vin
                                Left join Idling i on i.vin = he.vin
                                Left join IdleDuration ide on ide.vin = i.vin
                                Left join BrakingScore br on br.vin = ide.vin
                                Left join HarshBraking hr on hr.vin = br.vin
                                Left join HarshBrakeDuration hrdur on hrdur.vin = hr.vin
                                Left join AnticipationScore anc on anc.vin = hrdur.vin
                                Left join BrakeDuration brdur on brdur.vin = anc.vin
                                Left join Braking brk on brk.vin = brdur.vin

                                where 1 = 1 AND(dis.distance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)";

                List<EcoScoreReportSingleDriver> lstSingleDriver = (List<EcoScoreReportSingleDriver>)await _dataMartdataAccess.QueryAsync<EcoScoreReportSingleDriver>(query, parameters);
                return lstSingleDriver?.Count > 0 ? lstSingleDriver : new List<EcoScoreReportSingleDriver>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Eco Score Report Single Driver - VIN Company 
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<List<EcoScoreReportSingleDriver>> GetEcoScoreReportVINCompany(EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH 
                                ecoscorequery as (
                                    SELECT dr.organization_id, dr.first_name, dr.last_name, eco.driver1_id, eco.trip_distance,eco.trip_id,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
                                    eco.vin,eco.used_fuel,eco.pto_duration,eco.end_time,eco.start_time,eco.gross_weight_combination_total,
                                    eco.heavy_throttle_pedal_duration,eco.idle_duration,eco.harsh_brake_duration,eco.brake_duration,
									eco.cruise_control_usage , eco.cruise_control_usage_30_50,eco.cruise_control_usage_50_75,eco.cruise_control_usage_75
									,ve.registration_no,ve.name
                                    FROM tripdetail.ecoscoredata eco
									JOIN master.vehicle ve 
									ON eco.vin = ve.vin
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                   WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) --ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                    --AND eco.driver1_id = @DriverId --ANY('{NL B000384974000000}')
                                    AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                    AND dr.organization_id=@OrgId
                                ),
                                
                                generalblk as 
                                (
                                    select eco.organization_id, eco.vin,  eco.registration_no,eco.name,  count(eco.driver1_id)  as drivercnt
                                    FROM ecoscorequery eco
                                    Where eco.driver1_id =@DriverId
                                    GROUP BY eco.organization_id,eco.vin, eco.registration_no,eco.name
                                ) ,
								 AverageGrossweight as 
                                (
                                    select eco.organization_id , eco.vin, (CAST(SUM (eco.gross_weight_combination_total)as DOUBLE PRECISION))  as AverageGrossweight
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
								Distance as 
                                (
                                    select eco.organization_id, eco.vin,  (CAST(SUM (eco.trip_distance)as DOUBLE PRECISION)) as Distance
                                    FROM ecoscorequery eco
                                     GROUP BY eco.organization_id ,eco.vin
                                ),
                                NumberOfTrips as 
                                (
                                    select eco.organization_id, eco.vin,   COUNT (eco.trip_id) as NumberOfTrips
                                    FROM ecoscorequery eco
                                     GROUP BY eco.organization_id ,eco.vin
                                ),
                                NumberOfVehicles as 
                                (
                                    select eco.organization_id ,eco.vin,   COUNT (eco.vin) as NumberOfVehicles
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
                                AverageDistancePerDay as 
                                (
                                    select eco.organization_id ,eco.vin,  (SUM(eco.trip_distance) / CEIL(CAST(MAX(eco.end_time) - MIN(eco.start_time) AS DOUBLE PRECISION)/(1000 * 60 * 60 * 24))) as AverageDistancePerDay
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
								EcoScore as
                                (
                                    SELECT eco.organization_id ,eco.vin, 
                                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN  
                                    (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
                                      (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
                                    else null END as ecoscore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
                                FuelConsumption as 
                                (
                                    SELECT eco.organization_id , eco.vin,  (CAST(SUM (eco.used_fuel)AS DOUBLE PRECISION )) as FuelConsumption
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
                                CruiseControlUsage as 
                                (
                                    SELECT eco.organization_id , eco.vin, CASE WHEN SUM(trip_distance) <>0 THEN
									((CAST(SUM (eco.cruise_control_usage) AS DOUBLE PRECISION ))/ SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
                                CruiseControlUsage30 as 
                                (
                                    SELECT eco.organization_id , eco.vin, CASE WHEN SUM(trip_distance) <>0 THEN
                                    ((CAST(SUM (eco.cruise_control_usage_30_50) AS DOUBLE PRECISION ))/SUM(trip_distance))*100
                                    ELSE null END    as CruiseControlUsage30
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
                                CruiseControlUsage50 as 
                                (
                                    SELECT eco.organization_id , eco.vin, CASE WHEN SUM(trip_distance) <>0 THEN  
                                    ((CAST(SUM (eco.cruise_control_usage_50_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100 
                                    ELSE null END    as CruiseControlUsage50
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
                                CruiseControlUsage75 as 
                                (
                                   SELECT eco.organization_id , eco.vin,  CASE WHEN SUM(trip_distance) <>0 THEN
                                   ((CAST(SUM (eco.cruise_control_usage_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100  
                                   ELSE null END  as CruiseControlUsage75
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin
                                ),
                                PTOUsage as 
                                (
                                    SELECT eco.organization_id , eco.vin, 
                                    CASE WHEN ( SUM (eco.end_time)- SUM (eco.start_time) ) <> 0 and (( SUM (eco.end_time)- SUM (eco.start_time) )/1000) <>0 THEN
                                    (SUM(eco.pto_duration) / (( SUM (eco.end_time)- SUM (eco.start_time) )/1000))*100 
                                    ELSE null END as PTOUsage
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin				 
                                ),
                                PTODuration as 
                                (
                                   SELECT eco.organization_id , eco.vin,   SUM(eco.pto_duration) as PTODuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin	
                                ),
                                AverageDrivingSpeed as
                                (  
                                   SELECT eco.organization_id , eco.vin,   
                                   CASE WHEN ((( (SUM (eco.end_time)) - (SUM (eco.start_time)) )/1000)- (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))  ) <> 0  THEN
                                     (CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION) )  /((( (SUM (eco.end_time)) - (SUM (eco.start_time))  )/1000)-   (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))    )  
                                   ELSE null END as AverageDrivingSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin
                                ),
                                AverageSpeed as
                                (
                                   SELECT eco.organization_id , eco.vin, 
                                   CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <>0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0 then
                                   SUM(eco.trip_distance)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)  
                                   ELSE null END as AverageSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin
                                ),
                                HeavyThrottling as
                                (
                                    SELECT eco.organization_id , eco.vin, 
                                    CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)<>0 THEN
                                    (SUM(eco.heavy_throttle_pedal_duration)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))*100  
                                    ELSE null END as HeavyThrottling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
                                HeavyThrottleDuration  as
                                (
                                    SELECT eco.organization_id , eco.vin, (CAST(SUM(eco.heavy_throttle_pedal_duration ) AS DOUBLE PRECISION)) as HeavyThrottleDuration
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
                                Idling  as
                                (
                                    SELECT eco.organization_id , eco.vin,
                                    CASE WHEN ( (SUM (eco.end_time))- (SUM (eco.start_time)))<> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0  THEN 
                                    ( CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION)/ (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))* 100
                                    ELSE null end
                                    as Idling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                ),
                                IdleDuration  as
                                (
                                   SELECT eco.organization_id , eco.vin , CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)   as IdleDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin
                                ),
                                BrakingScore  as
                                (
                                   SELECT eco.organization_id , eco.vin,( CAST(SUM(eco.dpa_Braking_score) AS DOUBLE PRECISION)/ NULLIF ( (CAST(SUM (eco.dpa_Braking_count)AS DOUBLE PRECISION)),0))/10   as BrakingScore
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin
                                ),
                                HarshBraking  as
                                (
                                   SELECT eco.organization_id , eco.vin, (CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION)/ NULLIF( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)),0))*100 as HarshBraking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin
                                ),
                                HarshBrakeDuration  as
                                (
                                   SELECT eco.organization_id , eco.vin, CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION) as HarshBrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin
                                ),
                                BrakeDuration as
                                (
                                   SELECT eco.organization_id , eco.vin, CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)/ 86400 as BrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin
                                ),
                                Braking as
                                (
                                   SELECT eco.organization_id ,eco.vin,
	                               case when ((SUM (eco.end_time))-(SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) <>0 THEN 
                                  ( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION))/ (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) - CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))*100
	                               ELSE null END as Braking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.organization_id ,eco.vin
                                ),
                                AnticipationScore as
                                (
                                    SELECT eco.organization_id , eco.vin,  (   (CAST(SUM(eco.dpa_anticipation_score)AS DOUBLE PRECISION ) ) / NULLIF(  (CAST (SUM(eco.dpa_anticipation_count) AS DOUBLE PRECISION) )  ,0) )/10 as AnticipationScore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.vin
                                )
								select  eco.vin as VIN,eco.organization_id
                                ,  eco.registration_no as RegistrationNo ,eco.name as VehicleName, 'VIN_Company' as HeaderType
                                
                                 ,CAST(avrg.averagegrossweight/1000 AS DOUBLE PRECISION) as AverageGrossweight -- convert kg weight to tonnes by /1000
                                ,CAST(dis.distance/1000 AS DOUBLE PRECISION) as Distance  -- convert meter to km by /1000
                                ,CAST(notrp.numberoftrips AS DOUBLE PRECISION) as NumberOfTrips
                                ,CAST(noveh.numberofvehicles AS DOUBLE PRECISION) as NumberOfVehicles
                                ,CAST(avgdperday.averagedistanceperday/1000 AS DOUBLE PRECISION) as AverageDistancePerDay -- convert meter to km by /1000
                                
                                ,CAST(ecos.ecoscore AS DOUBLE PRECISION) as EcoScore
                                ,CAST(f.fuelconsumption/1000 AS DOUBLE PRECISION) as FuelConsumption
                                ,CAST(crus.cruisecontrolusage AS DOUBLE PRECISION) as CruiseControlUsage
                                ,CAST(crusa.cruisecontrolusage30 AS DOUBLE PRECISION) as CruiseControlUsage30
                                ,CAST(crucon.cruisecontrolusage50 AS DOUBLE PRECISION) as CruiseControlUsage50
                                ,CAST(crucont.cruisecontrolusage75 AS DOUBLE PRECISION) as CruiseControlUsage75
                                ,CAST(p.ptousage*100 AS DOUBLE PRECISION) as PTOUsage --convert Count to % by *100
                                ,CAST(pto.ptoduration AS DOUBLE PRECISION) as PTODuration
                                ,CAST(ads.averagedrivingspeed * 3.6 AS DOUBLE PRECISION) as AverageDrivingSpeed  --convert meter/second  to kmph  by *3.6
                                ,CAST(aspeed.averagespeed * 3.6 AS DOUBLE PRECISION) as AverageSpeed --convert meter/second  to kmph  by *3.6
                                ,CAST(h.heavythrottling *100 AS DOUBLE PRECISION) as HeavyThrottling  -- Conver count to % by *100
                                ,CAST(he.heavythrottleduration AS DOUBLE PRECISION) as HeavyThrottleDuration
                                ,CAST(i.idling AS DOUBLE PRECISION) as Idling  -- Conver count to % by *100
                                ,CAST(ide.idleduration AS DOUBLE PRECISION) as IdleDuration
                                ,CAST(br.brakingscore AS DOUBLE PRECISION) as BrakingScore
                                ,CAST(hr.harshbraking * 100 AS DOUBLE PRECISION) as HarshBraking -- Conver count to % by *100
                                ,CAST(hrdur.HarshBrakeDuration AS DOUBLE PRECISION) as HarshBrakeDuration
                                ,CAST(brdur.brakeduration AS DOUBLE PRECISION) as BrakeDuration
                                ,CAST(brk.braking AS DOUBLE PRECISION) as Braking
                                ,CAST(anc.anticipationscore  AS DOUBLE PRECISION) as AnticipationScore
                                
                                 from generalblk eco
                                Left join AverageGrossweight avrg on avrg.vin = eco.vin
                                Left join Distance dis on dis.vin = avrg.vin
                                Left join NumberOfTrips notrp on notrp.vin = dis.vin
                                Left join numberofvehicles noveh on noveh.vin = notrp.vin
                                Left join AverageDistancePerDay avgdperday on avgdperday.vin = noveh.vin
                                Left join EcoScore ecos on ecos.vin = avgdperday.vin
                                Left join FuelConsumption f on f.vin = ecos.vin
                                Left join CruiseControlUsage crus  on crus.vin = f.vin   
                                Left join CruiseControlUsage30 crusa  on crusa.vin = crus.vin
                                Left join CruiseControlUsage50 crucon on crucon.vin = crusa.vin
                                Left join CruiseControlUsage75 crucont on crucont.vin = crucon.vin
                                Left join PTOUsage p on p.vin = crucont.vin
                                Left join PTODuration pto on pto.vin = p.vin
                                Left join AverageDrivingSpeed ads on ads.vin = pto.vin
                                
                                Left join AverageSpeed aspeed on aspeed.vin = ads.vin
                                Left join HeavyThrottling h on h.vin = aspeed.vin
                                Left join HeavyThrottleDuration he on he.vin = h.vin
                                Left join Idling i on i.vin = he.vin
                                Left join IdleDuration ide on ide.vin = i.vin
                                Left join BrakingScore br on br.vin = ide.vin
                                Left join HarshBraking hr on hr.vin = br.vin
                                Left join HarshBrakeDuration hrdur on hrdur.vin = hr.vin
                                Left join AnticipationScore anc on anc.vin = hrdur.vin
                                Left join BrakeDuration brdur on brdur.vin = anc.vin
                                Left join Braking brk on brk.vin = brdur.vin

                                where 1 = 1 AND(dis.distance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)";

                List<EcoScoreReportSingleDriver> lstSingleDriver = (List<EcoScoreReportSingleDriver>)await _dataMartdataAccess.QueryAsync<EcoScoreReportSingleDriver>(query, parameters);
                return lstSingleDriver?.Count > 0 ? lstSingleDriver : new List<EcoScoreReportSingleDriver>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Eco Score Report Single Driver - Average Gross Weight Chart (Bar/Pie) 
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<List<EcoScoreSingleDriverBarPieChart>> GetEcoScoreAverageGrossWeightChartData(EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"with grossweight as (
                                	select eco.driver1_id, eco.vin, eco.trip_id, cast(eco.gross_weight_combination_total/1000 as decimal(18,4)) as AGW, 
                                	eco.trip_distance as Distance
                                	from tripdetail.ecoscoredata eco
                                	JOIN master.driver dr 
                                	ON dr.driver_id = eco.driver1_id
                                	WHERE eco.start_time >= @FromDate --1204336888377
                                	AND eco.end_time <= @ToDate --1820818919744
                                	AND eco.vin = ANY(@Vins)--ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                	AND eco.driver1_id = @DriverId --'NL B000384974000000'
                                	AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                	AND dr.organization_id = @OrgId	
                                ),
                                agw_overall as (
                                	select driver1_id, CASE WHEN agw >=0 AND agw<= 10  then '0-10 t'  
                                	WHEN agw >10 AND agw<= 20  then '10-20 t' 
                                	WHEN agw >20 AND agw<= 30  then '20-30 t' 
                                	WHEN agw >30 AND agw<= 40  then '30-40 t' 
                                	WHEN agw >40 AND agw<= 50  then '40-50 t'
                                	WHEN agw >50  then '>50 t' END
                                	as x_axis, agw, distance
                                	from grossweight
                                ),
                                total_overall as (
                                	select x_axis, sum(distance) as dist 
                                	from agw_overall
                                	group by x_axis
                                ),
                                overall as (
                                	select 'Overall Driver' as vin, x_axis, dist as distance, (select sum(dist) from total_overall) as total, 
                                	cast((dist/(select sum(dist) from total_overall))*100 as decimal(18,2)) as y_axis
                                	from total_overall
                                ),
                                agw_vin as (
                                	select driver1_id, VIN, CASE WHEN agw >=0 AND agw<= 10  then '0-10 t'  
                                	WHEN agw >10 AND agw<= 20  then '10-20 t' 
                                	WHEN agw >20 AND agw<= 30  then '20-30 t' 
                                	WHEN agw >30 AND agw<= 40  then '30-40 t' 
                                	WHEN agw >40 AND agw<= 50  then '40-50 t'
                                	WHEN agw >50  then '>50 t' END
                                	as x_axis, agw, distance
                                	from grossweight
                                ),
                                agw_dist as (
                                	select vin, x_axis, sum(distance) as dist 
                                	from agw_vin
                                	group by x_axis, vin
                                ),
                                total_vin as(
                                	select vin, sum(dist) as total
                                	from agw_dist group by vin
                                ),
                                overallvin as (
                                	select a.vin, a.x_axis, a.dist as distance, b.total, cast((a.dist/b.total)*100 as decimal(18,2)) as y_axis
                                	from agw_dist a
                                	join total_vin b
                                	on a.vin=b.vin
                                	UNION
                                	select vin, x_axis, distance, total, y_axis 
                                	from overall
                                )
                                
                                select ov.vin, CASE WHEN ov.vin='Overall Driver' then 'Overall Driver' else v.name END as vehiclename, ov.x_axis, ov.distance, ov.y_axis  
                                from overallvin ov
                                left join master.vehicle v
                                on ov.vin=v.vin
                                where 1 = 1 AND (total >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)
                                order by vin, x_axis";

                List<EcoScoreSingleDriverBarPieChart> lstAGWChart = (List<EcoScoreSingleDriverBarPieChart>)await _dataMartdataAccess.QueryAsync<EcoScoreSingleDriverBarPieChart>(query, parameters);
                return lstAGWChart?.Count > 0 ? lstAGWChart : new List<EcoScoreSingleDriverBarPieChart>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Eco Score Report Single Driver - Average Driving Speed Chart (Bar/Pie) 
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<List<EcoScoreSingleDriverBarPieChart>> GetEcoScoreAverageDrivingSpeedChartData(EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);
                parameters.Add("@Unit", request.UoM.ToLower());

                string query = @"with drivingspeed as (
                                  select eco.driver1_id, eco.vin, eco.trip_id,  CASE WHEN ((( (SUM (eco.end_time)) - (SUM (eco.start_time)))/1000)- (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))) <> 0 
								   THEN
								   ((CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION) )/((( (SUM (eco.end_time)) - (SUM (eco.start_time)))/1000)-(CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)))
								   ) ELSE null END as averagedrivingspeed,
                                  eco.trip_distance as Distance,
	                              CASE WHEN 'metric' = @Unit THEN 3.6 ELSE 2.237 END as unit --Metric/Imperial conversion
                                  from tripdetail.ecoscoredata eco
                                  JOIN master.driver dr 
                                  ON dr.driver_id = eco.driver1_id
                                  WHERE eco.start_time >= @FromDate --1204336888377
                                  AND eco.end_time <= @ToDate --1820818919744
                                  AND eco.vin = ANY (@Vins)  --ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                  AND eco.driver1_id = @DriverId
                                  AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                  AND dr.organization_id = @OrgId
	                              group by eco.trip_id,eco.driver1_id,eco.trip_distance ,eco.vin
                                ),
								unittypeconversion as 
								(
								  select driver1_id,vin, trip_id,Distance 
									,CAST(averagedrivingspeed * unit As DOUBLE PRECISION) as averagedrivingspeed
									from drivingspeed
									
								), 
                                avgdrivingspeed_overall as (
                                select driver1_id, CASE WHEN 'metric' = @Unit THEN 
								 CASE WHEN averagedrivingspeed >=0 AND averagedrivingspeed<= 30  then '0-30 kmph'  
								 WHEN averagedrivingspeed >30 AND averagedrivingspeed<= 50  then '30-50 kmph' 
								 WHEN averagedrivingspeed >50 AND averagedrivingspeed<= 75  then '50-75 kmph' 
								 WHEN averagedrivingspeed >75 AND averagedrivingspeed<= 85  then '75-85 kmph' 
								 WHEN averagedrivingspeed >85   then '>85kmph'END
							ELSE 
								 CASE WHEN averagedrivingspeed >=0 AND averagedrivingspeed<= 15  then '0-15 mph'  
								 WHEN averagedrivingspeed >15 AND averagedrivingspeed<= 30  then '15-30 mph' 
								 WHEN averagedrivingspeed >30 AND averagedrivingspeed<= 45  then '30-45 mph' 
								 WHEN averagedrivingspeed >45 AND averagedrivingspeed<= 50  then '45-50 mph' 
								 WHEN averagedrivingspeed >50   then '>50 mph'END
							END
                                  as x_axis, averagedrivingspeed , distance
                                  from unittypeconversion 
                                ) ,
                                total_overall as (
                                  select x_axis, sum(distance) as dist 
                                  from avgdrivingspeed_overall
                                  group by x_axis
                                ),
                                overall as (
                                  select 'Overall Driver' as vin, x_axis, dist as distance, (select sum(dist) from total_overall) as total, 
                                  cast((dist/(select sum(dist) from total_overall))*100 as decimal(18,2)) as y_axis
                                  from total_overall
                                ) ,
                                ads_vin as (
                                 select driver1_id,vin, CASE WHEN 'metric' = @Unit THEN 
								 CASE WHEN averagedrivingspeed >=0 AND averagedrivingspeed<= 30  then '0-30 kmph'  
								 WHEN averagedrivingspeed >30 AND averagedrivingspeed<= 50  then '30-50 kmph' 
								 WHEN averagedrivingspeed >50 AND averagedrivingspeed<= 75  then '50-75 kmph' 
								 WHEN averagedrivingspeed >75 AND averagedrivingspeed<= 85  then '75-85 kmph' 
								 WHEN averagedrivingspeed >85   then '>85kmph'END
							ELSE 
								 CASE WHEN averagedrivingspeed >=0 AND averagedrivingspeed<= 15  then '0-15 mph'  
								 WHEN averagedrivingspeed >15 AND averagedrivingspeed<= 30  then '15-30 mph' 
								 WHEN averagedrivingspeed >30 AND averagedrivingspeed<= 45  then '30-45 mph' 
								 WHEN averagedrivingspeed >45 AND averagedrivingspeed<= 50  then '45-50 mph' 
								 WHEN averagedrivingspeed >50   then '>50 mph'END
							END
						         as x_axis, averagedrivingspeed, distance
                                  from unittypeconversion 
                                ),
                                ads_dist as (
                                  select vin, x_axis, sum(distance) as dist 
                                  from ads_vin
                                  group by x_axis, vin
                                ),
                                total_vin as(
                                  select vin, sum(dist) as total
                                  from ads_dist group by vin
                                ),
                                overallvin as (
                                  select a.vin, a.x_axis, a.dist as distance, b.total, cast((a.dist/b.total)*100 as decimal(18,2)) as y_axis
                                  from ads_dist a
                                  join total_vin b
                                  on a.vin=b.vin
                                  UNION
                                  select vin, x_axis, distance, total, y_axis 
                                  from overall
                                )
                                
                                select ov.vin, CASE WHEN ov.vin='Overall Driver' then 'Overall Driver' else v.name END as vehiclename, ov.x_axis, ov.distance, ov.y_axis  
                                from overallvin ov
                                left join master.vehicle v
                                on ov.vin=v.vin
                                where 1 = 1 AND (total >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)
                                order by vin, x_axis
";

                List<EcoScoreSingleDriverBarPieChart> lstADSChart = (List<EcoScoreSingleDriverBarPieChart>)await _dataMartdataAccess.QueryAsync<EcoScoreSingleDriverBarPieChart>(query, parameters);
                return lstADSChart?.Count > 0 ? lstADSChart : new List<EcoScoreSingleDriverBarPieChart>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region EcoScore Trendlines
        /// <summary>
        /// Get Eco Score Report Single Driver - Overall Driver Trendlines
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<IEnumerable<EcoScoreReportSingleDriver>> GetEcoScoreReportOverallDriverForTrendline(EcoScoreReportSingleDriverRequest request)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH 
                                ecoscorequery as (
                                    SELECT  eco.driver1_id, eco.trip_distance,eco.trip_id,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
                                    eco.vin,eco.used_fuel,eco.pto_duration,eco.end_time,eco.start_time,eco.gross_weight_combination_total,
                                    eco.heavy_throttle_pedal_duration,eco.idle_duration,eco.harsh_brake_duration,eco.brake_duration,
                                    eco.cruise_control_usage , eco.cruise_control_usage_30_50,eco.cruise_control_usage_50_75,eco.cruise_control_usage_75
                                   ,date_trunc('day', to_timestamp(eco.end_time/1000)) as Day
                                    FROM tripdetail.ecoscoredata eco
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                    WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) --ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                    AND eco.driver1_id = @DriverId --ANY('{NL B000384974000000}')
                                    AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                    AND dr.organization_id=@OrgId
                                ),
                                
                                generalblk as 
                                (
                                    select eco.driver1_id,eco.Day
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id ,eco.Day
                                ),
                                Distance as 
                                (
                                    select eco.driver1_id,eco.Day,  (CAST(SUM (eco.trip_distance)as DOUBLE PRECISION)) as Distance
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id ,eco.Day
                                ),
                                EcoScore as
                                (
                                    SELECT eco.driver1_id ,eco.Day,
                                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN  
                                    (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
                                      (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
                                    else null END as ecoscore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id,eco.Day
                                ),
                                FuelConsumption as 
                                (
                                    SELECT eco.driver1_id,eco.Day, (CAST(SUM (eco.used_fuel)AS DOUBLE PRECISION )) as FuelConsumption
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id,eco.Day
                                ),
                               CruiseControlUsage as 
                                (
                                    SELECT eco.driver1_id, eco.Day, CASE WHEN SUM(trip_distance) <>0 THEN
									((CAST(SUM (eco.cruise_control_usage) AS DOUBLE PRECISION ))/ SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage
									FROM ecoscorequery eco
									GROUP BY eco.driver1_id,eco.Day
                                ),
                                CruiseControlUsage30 as 
                                (
                                    SELECT eco.driver1_id,eco.Day,  CASE WHEN SUM(trip_distance) <>0 THEN
									((CAST(SUM (eco.cruise_control_usage_30_50) AS DOUBLE PRECISION ))/ SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage30
									FROM ecoscorequery eco
									GROUP BY eco.driver1_id,eco.Day
                                ),
                                CruiseControlUsage50 as 
                                (
                                    SELECT eco.driver1_id,eco.Day, CASE WHEN SUM(trip_distance) <>0 THEN
									((CAST(SUM (eco.cruise_control_usage_50_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100 
									ELSE null END as CruiseControlUsage50
									FROM ecoscorequery eco
									GROUP BY eco.driver1_id,eco.Day
                                ),
                                CruiseControlUsage75 as 
                                (
                                   SELECT eco.driver1_id,eco.Day, CASE WHEN SUM(trip_distance) <>0 THEN
								   ((CAST(SUM (eco.cruise_control_usage_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100  
								   ELSE null END as  CruiseControlUsage75
								   FROM ecoscorequery eco
								   GROUP BY eco.driver1_id,eco.Day
                                ),
                                PTOUsage as 
                                (
                                    SELECT eco.driver1_id,eco.Day,
                                    CASE WHEN ( SUM (eco.end_time)- SUM (eco.start_time) ) <> 0 and (( SUM (eco.end_time)- SUM (eco.start_time) )/1000) <>0 THEN
                                    (SUM(eco.pto_duration) / (( SUM (eco.end_time)- SUM (eco.start_time) )/1000))*100 
                                    ELSE null END as PTOUsage
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id	,eco.Day			 
                                ),
                                PTODuration as 
                                (
                                   SELECT eco.driver1_id,eco.Day,  SUM(eco.pto_duration) as PTODuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id	,eco.Day
                                ),
                                AverageDrivingSpeed as
                                (  
                                   SELECT eco.driver1_id, eco.Day, 
                                   CASE WHEN ((( (SUM (eco.end_time)) - (SUM (eco.start_time)) )/1000)- (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))  ) <> 0  THEN
                                     (CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION) )  /((( (SUM (eco.end_time)) - (SUM (eco.start_time))  )/1000)-   (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))    )  
                                   ELSE null END as AverageDrivingSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id,eco.Day
                                ),
                                AverageSpeed as
                                (
                                   SELECT eco.driver1_id, eco.Day,
                                   CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <>0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0 then
                                   SUM(eco.trip_distance)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)  
                                   ELSE null END as AverageSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id,eco.Day
                                ),
                                HeavyThrottling as
                                (
                                    SELECT eco.driver1_id,eco.Day,
                                    CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)<>0 THEN
                                    (SUM(eco.heavy_throttle_pedal_duration)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))*100  
                                    ELSE null END as HeavyThrottling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id,eco.Day
                                ),
                                HeavyThrottleDuration  as
                                (
                                    SELECT eco.driver1_id,eco.Day, (CAST(SUM(eco.heavy_throttle_pedal_duration ) AS DOUBLE PRECISION)) as HeavyThrottleDuration
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id,eco.Day
                                ),
                                Idling  as
                                (
                                    SELECT eco.driver1_id,eco.Day,
                                    CASE WHEN ( (SUM (eco.end_time))- (SUM (eco.start_time)))<> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0  THEN 
                                    ( CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION)/ (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))* 100
                                    ELSE null end
                                    as Idling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id,eco.Day
                                ),
                                IdleDuration  as
                                (
                                   SELECT eco.driver1_id,eco.Day,  CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)   as IdleDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id,eco.Day
                                ),
                                BrakingScore  as
                                (
                                   SELECT eco.driver1_id,eco.Day,( CAST(SUM(eco.dpa_Braking_score) AS DOUBLE PRECISION)/ NULLIF ( (CAST(SUM (eco.dpa_Braking_count)AS DOUBLE PRECISION)),0))/10   as BrakingScore
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id,eco.Day
                                ),
                                HarshBraking  as
                                (
                                   SELECT eco.driver1_id,eco.Day,( CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION)/ NULLIF( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)),0))*100 as HarshBraking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id,eco.Day
                                ),
                                HarshBrakeDuration  as
                                (
                                   SELECT eco.driver1_id,eco.Day, CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION) as HarshBrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id,eco.Day
                                ),
                                BrakeDuration as
                                (
                                   SELECT eco.driver1_id,eco.Day, CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)/ 86400 as BrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id,eco.Day
                                ),
                                Braking as
                                (
                                   SELECT eco.driver1_id,eco.Day,
	                               case when ((SUM (eco.end_time))-(SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) <>0 THEN 
                                   ( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION))/ (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000))*100 
	                               ELSE null END as Braking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.driver1_id,eco.Day
                                ),
                                AnticipationScore as
                                (
                                    SELECT eco.driver1_id,eco.Day, (   (CAST(SUM(eco.dpa_anticipation_score)AS DOUBLE PRECISION ) ) / NULLIF(  (CAST (SUM(eco.dpa_anticipation_count) AS DOUBLE PRECISION) )  ,0) )/10 as AnticipationScore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.driver1_id,eco.Day
                                ) 
                                select 
								eco.Day,'Overall_Driver' as HeaderType, 'Overall' as VIN,'Overall' as VehicleName,
								eco.driver1_id as DriverId
                                ,CAST(ecos.ecoscore AS DOUBLE PRECISION) as EcoScore
                                ,CAST(f.fuelconsumption/1000 AS DOUBLE PRECISION) as FuelConsumption
                                ,CAST(crus.cruisecontrolusage AS DOUBLE PRECISION) as CruiseControlUsage
                                ,CAST(crusa.cruisecontrolusage30 AS DOUBLE PRECISION) as CruiseControlUsage30
                                ,CAST(crucon.cruisecontrolusage50 AS DOUBLE PRECISION) as CruiseControlUsage50
                                ,CAST(crucont.cruisecontrolusage75 AS DOUBLE PRECISION) as CruiseControlUsage75
                                ,CAST(p.ptousage*100 AS DOUBLE PRECISION) as PTOUsage --convert Count to % by *100
                                ,CAST(pto.ptoduration AS DOUBLE PRECISION) as PTODuration
                                ,CAST(ads.averagedrivingspeed * 3.6 AS DOUBLE PRECISION) as AverageDrivingSpeed  --convert meter/second  to kmph  by *3.6
                                ,CAST(aspeed.averagespeed * 3.6 AS DOUBLE PRECISION) as AverageSpeed --convert meter/second  to kmph  by *3.6
                                ,CAST(h.heavythrottling *100 AS DOUBLE PRECISION) as HeavyThrottling  -- Conver count to % by *100
                                ,CAST(he.heavythrottleduration AS DOUBLE PRECISION) as HeavyThrottleDuration
                                ,CAST(i.idling AS DOUBLE PRECISION) as Idling  -- Conver count to % by *100
                                ,CAST(ide.idleduration AS DOUBLE PRECISION) as IdleDuration
                                ,CAST(br.brakingscore AS DOUBLE PRECISION) as BrakingScore
                                ,CAST(hr.harshbraking * 100 AS DOUBLE PRECISION) as HarshBraking -- Conver count to % by *100
                                ,CAST(hrdur.HarshBrakeDuration AS DOUBLE PRECISION) as HarshBrakeDuration
                                ,CAST(brdur.brakeduration AS DOUBLE PRECISION) as BrakeDuration
                                ,CAST(brk.braking AS DOUBLE PRECISION) as Braking
                                ,CAST(anc.anticipationscore  AS DOUBLE PRECISION) as AnticipationScore
                                
                                from generalblk eco
                                Left join Distance dis on dis.driver1_id = eco.driver1_id and dis.Day=eco.Day
                                Left join EcoScore ecos on ecos.driver1_id = dis.driver1_id and dis.Day= ecos.Day
                                Left join FuelConsumption f on f.driver1_id = ecos.driver1_id  and f.Day= ecos.Day
                                Left join CruiseControlUsage crus  on crus.driver1_id = f.driver1_id  and f.Day= crus.Day 
                                Left join CruiseControlUsage30 crusa  on crusa.driver1_id = crus.driver1_id and crusa.Day= crus.Day 
                                Left join CruiseControlUsage50 crucon on crucon.driver1_id = crusa.driver1_id and crusa.Day= crucon.Day 
                                Left join CruiseControlUsage75 crucont on crucont.driver1_id = crucon.driver1_id and crucont.Day= crucon.Day 
                                Left join PTOUsage p on p.driver1_id = crucont.driver1_id and crucont.Day= p.Day 
                                Left join PTODuration pto on pto.driver1_id = p.driver1_id and pto.Day= p.Day 
                                Left join AverageDrivingSpeed ads on ads.driver1_id = pto.driver1_id and ads.Day= pto.Day 
                                
                                Left join AverageSpeed aspeed on aspeed.driver1_id = ads.driver1_id and aspeed.Day= ads.Day 
                                Left join HeavyThrottling h on h.driver1_id = aspeed.driver1_id and h.Day= aspeed.Day
                                Left join HeavyThrottleDuration he on he.driver1_id = h.driver1_id and he.Day= h.Day
                                Left join Idling i on i.driver1_id = he.driver1_id and i.Day= he.Day
                                Left join IdleDuration ide on ide.driver1_id = i.driver1_id and ide.Day= i.Day
                                Left join BrakingScore br on br.driver1_id = ide.driver1_id and br.Day= ide.Day
                                Left join HarshBraking hr on hr.driver1_id = br.driver1_id and hr.Day= br.Day
                                Left join HarshBrakeDuration hrdur on hrdur.driver1_id = hr.driver1_id and hrdur.Day= hr.Day
                                Left join AnticipationScore anc on anc.driver1_id = hrdur.driver1_id and anc.Day= hrdur.Day
                                Left join BrakeDuration brdur on brdur.driver1_id = anc.driver1_id and brdur.Day= anc.Day
                                Left join Braking brk on brk.driver1_id = brdur.driver1_id and brk.Day= brdur.Day

                                where 1 = 1 AND(dis.distance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)";

                return await _dataMartdataAccess.QueryAsync<EcoScoreReportSingleDriver>(query, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Eco Score Report Single Driver - Overall Company Trendlines
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<IEnumerable<EcoScoreReportSingleDriver>> GetEcoScoreReportOverallCompanyForTrendline(EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH 
                                ecoscorequery as (
                                    SELECT dr.organization_id, eco.driver1_id, eco.trip_distance,eco.trip_id,eco.vin,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count
                                    ,date_trunc('day', to_timestamp(eco.end_time/1000)) as Day
                                    FROM tripdetail.ecoscoredata eco
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                    WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) --ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                    AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                    AND dr.organization_id=@OrgId

                                ),
                                 generalblk as 
                                (
                                    select eco.organization_id,eco.Day
                                    FROM ecoscorequery eco
								    Where eco.driver1_id =@DriverId
                                    GROUP BY eco.organization_id ,eco.Day
                                 )  ,
                                 Distance as 
                                (
                                    select eco.organization_id,eco.Day,  (CAST(SUM (eco.trip_distance)as DOUBLE PRECISION)) as Distance
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.Day
                                ),
								EcoScore as
                                (
                                    SELECT   eco.organization_id ,eco.Day,
                                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN  
                                    (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
                                      (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
                                    else null END as ecoscore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id,eco.Day 
                                )
                                select  
								 eco.Day,'Overall_Company' as HeaderType, 'Overall' as VIN,'Overall' as VehicleName
                                , CAST(eco.ecoscore AS DOUBLE PRECISION) AS  EcoScore 
								From generalblk ger
                                Left join EcoScore eco on eco.organization_id = ger.organization_id and ger.Day=eco.Day
                                Left join Distance dis on dis.organization_id = eco.organization_id and dis.Day=eco.Day
                                where 1 = 1 AND(dis.distance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)";

                var temp = await _dataMartdataAccess.QueryAsync<EcoScoreReportSingleDriver>(query, parameters);
                return temp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Eco Score Report Single Driver - VIN Company Trendlines
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<IEnumerable<EcoScoreReportSingleDriver>> GetEcoScoreReportVinCompanyForTrendline(EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH 
                                ecoscorequery as (
                                     SELECT dr.organization_id, dr.first_name, dr.last_name, eco.driver1_id, eco.trip_distance,eco.trip_id,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
                                    eco.vin ,ve.registration_no,ve.name as VehicleName,date_trunc('day', to_timestamp(eco.end_time/1000)) as Day
                                    FROM tripdetail.ecoscoredata eco
									JOIN master.vehicle ve 
									ON eco.vin = ve.vin
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                    WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) --ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                    AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                    AND dr.organization_id=@OrgId
                                ),
                                 generalblk as 
                                (
                                    select eco.organization_id,eco.Day, eco.vin,eco.VehicleName
                                    FROM ecoscorequery eco
								    Where eco.driver1_id = @DriverId
                                    GROUP BY eco.organization_id ,eco.Day, eco.vin,eco.VehicleName
                                 )   ,
                                Distance as 
                                (
                                    select eco.organization_id,eco.Day, eco.vin,  (CAST(SUM (eco.trip_distance)as DOUBLE PRECISION)) as Distance
                                    FROM ecoscorequery eco
                                     GROUP BY eco.organization_id ,eco.Day,eco.vin
                                ),
								EcoScore as
                                (
                                    SELECT eco.organization_id ,eco.Day,eco.vin, 
                                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN  
                                    (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
                                      (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
                                    else null END as ecoscore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.organization_id ,eco.Day,eco.vin
                                )
                                select 
								eco.Day,'VIN_Company' as HeaderType, gen.vin as VIN,gen.VehicleName as VehicleName,
                                CAST(eco.ecoscore AS DOUBLE PRECISION) as EcoScore
								From generalblk gen
                                Left join EcoScore eco on eco.vin = gen.vin and eco.Day=gen.Day
                                Left join Distance dis on dis.vin = eco.vin and dis.Day=eco.Day
                                where 1 = 1 AND(dis.distance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)";

                var temp = await _dataMartdataAccess.QueryAsync<EcoScoreReportSingleDriver>(query, parameters);
                return temp;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Eco Score Report Single Driver - VIN Driver Trendlines
        /// </summary>
        /// <param name="request">Search Parameters</param>
        /// <returns></returns>
        public async Task<List<EcoScoreReportSingleDriver>> GetEcoScoreReportVINDriverForTrendline(EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", request.StartDateTime);
                parameters.Add("@ToDate", request.EndDateTime);
                parameters.Add("@Vins", request.VINs.ToArray());
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinTripDistance);
                parameters.Add("@MinDriverTotalDistance", request.MinDriverTotalDistance);
                parameters.Add("@OrgId", request.OrgId);

                string query = @"WITH 
                                ecoscorequery as (
                                    SELECT  eco.driver1_id, eco.trip_distance,eco.trip_id,
                                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
                                    eco.vin,eco.used_fuel,eco.pto_duration,eco.end_time,eco.start_time,eco.gross_weight_combination_total,
                                    eco.heavy_throttle_pedal_duration,eco.idle_duration,eco.harsh_brake_duration,eco.brake_duration,
									eco.cruise_control_usage , eco.cruise_control_usage_30_50,eco.cruise_control_usage_50_75,eco.cruise_control_usage_75
									,ve.registration_no,ve.name,date_trunc('day', to_timestamp(eco.end_time/1000)) as Day
                                    FROM tripdetail.ecoscoredata eco
									JOIN master.vehicle ve 
									ON eco.vin = ve.vin
                                    JOIN master.driver dr 
                                    ON dr.driver_id = eco.driver1_id
                                    WHERE eco.start_time >= @FromDate --1204336888377
                                    AND eco.end_time <= @ToDate --1820818919744
                                    AND eco.vin = ANY(@Vins) --ANY('{XLR0998HGFFT76666,5A37265,XLR0998HGFFT76657,XLRASH4300G1472w0,XLR0998HGFFT74600}')
                                    AND eco.driver1_id = @DriverId --ANY('{NL B000384974000000}')
                                    AND (eco.trip_distance >= @MinTripDistance OR eco.trip_distance IS NULL)
                                    AND dr.organization_id=@OrgId
                                ),
                                
                               generalblk as 
                                (
                                    select eco.vin, eco.driver1_id, eco.Day,eco.name
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin, eco.driver1_id,eco.Day,eco.name
                                ),  
                                Distance as 
                                (
                                    select eco.vin,eco.Day,  (CAST(SUM (eco.trip_distance)as DOUBLE PRECISION)) as Distance
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin,eco.Day
                                ),
                                EcoScore as
                                (
                                    SELECT eco.vin, eco.Day,
                                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  THEN  
                                    (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
                                      (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
                                    else null END as ecoscore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin,eco.Day
                                ),
                                FuelConsumption as 
                                (
                                    SELECT eco.vin, eco.Day,  (CAST(SUM (eco.used_fuel)AS DOUBLE PRECISION )) as FuelConsumption
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin,eco.Day
                                ),
                                CruiseControlUsage as 
                                (
                                    SELECT eco.vin, eco.Day, CASE WHEN SUM(trip_distance)<>0 THEN
									((CAST(SUM (eco.cruise_control_usage) AS DOUBLE PRECISION ))/ SUM(trip_distance))*100  
									ELSE null END as CruiseControlUsage
									FROM ecoscorequery eco
									GROUP BY eco.vin,eco.Day
                                ),
                                CruiseControlUsage30 as 
                                (
                                    SELECT eco.vin,eco.Day, CASE WHEN SUM(trip_distance)<>0 THEN
									((CAST(SUM (eco.cruise_control_usage_30_50) AS DOUBLE PRECISION ))/SUM(trip_distance))*100   
									ELSE null END as CruiseControlUsage30
									FROM ecoscorequery eco
									GROUP BY eco.vin,eco.Day
                                ),
                                CruiseControlUsage50 as 
                                (
                                    SELECT eco.vin,eco.Day, CASE WHEN SUM(trip_distance)<>0 THEN
									((CAST(SUM (eco.cruise_control_usage_50_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100  
									ELSE null END as CruiseControlUsage50
									FROM ecoscorequery eco
									GROUP BY eco.vin,eco.Day
                                ),
                                CruiseControlUsage75 as 
                                (
                                   SELECT eco.vin,eco.Day, CASE WHEN SUM(trip_distance)<>0 THEN
								   ((CAST(SUM (eco.cruise_control_usage_75) AS DOUBLE PRECISION ))/SUM(trip_distance))*100   
								   ELSE null END as CruiseControlUsage75
								   FROM ecoscorequery eco
								   GROUP BY eco.vin,eco.Day
                                ),
                                PTOUsage as 
                                (
                                    SELECT eco.vin, eco.Day,
                                    CASE WHEN ( SUM (eco.end_time)- SUM (eco.start_time) ) <> 0 and (( SUM (eco.end_time)- SUM (eco.start_time) )/1000) <>0 THEN
                                    (SUM(eco.pto_duration) / (( SUM (eco.end_time)- SUM (eco.start_time) )/1000))*100 
                                    ELSE null END as PTOUsage
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin,eco.Day				 
                                ),
                                PTODuration as 
                                (
                                   SELECT eco.vin,eco.Day,   SUM(eco.pto_duration)  as PTODuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin,eco.Day
                                ),
                                AverageDrivingSpeed as
                                (  
								   SELECT eco.vin,eco.Day, 
                                   CASE WHEN ((( (SUM (eco.end_time)) - (SUM (eco.start_time)) )/1000)- (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))  ) <> 0  THEN
                                    (CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION) )  /((( (SUM (eco.end_time)) - (SUM (eco.start_time))  )/1000)-(CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)))
                                   ELSE null END as AverageDrivingSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin,eco.Day
                                ),
                                AverageSpeed as
                                (
                                   SELECT eco.vin,eco.Day, 
                                   CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <>0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0 then
                                   SUM(eco.trip_distance)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)
                                   ELSE null END as AverageSpeed
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin,eco.Day
                                ),
                                HeavyThrottling as
                                (
                                    SELECT eco.vin,eco.Day, 
                                    CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)<>0 THEN
                                    (SUM(eco.heavy_throttle_pedal_duration)/(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))*100 
                                    ELSE null END as HeavyThrottling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin,eco.Day
                                ),
                                HeavyThrottleDuration  as
                                (
                                    SELECT eco.vin,eco.Day,  (CAST(SUM(eco.heavy_throttle_pedal_duration ) AS DOUBLE PRECISION)) as HeavyThrottleDuration
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin,eco.Day
                                ),
                                Idling  as
                                (
									
                                    SELECT eco.vin,eco.Day, 
                                    CASE WHEN ( (SUM (eco.end_time))- (SUM (eco.start_time)))<> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0  THEN 
                                    ( CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION)/ (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000))* 100
                                    ELSE null end
                                    as Idling
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin,eco.Day
                                ),
                                IdleDuration  as
                                (
                                   SELECT eco.vin,eco.Day,   CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)   as IdleDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin,eco.Day
                                ),
                                BrakingScore  as
                                (
                                   SELECT eco.vin,eco.Day, ( CAST(SUM(eco.dpa_Braking_score) AS DOUBLE PRECISION)/ NULLIF ( (CAST(SUM (eco.dpa_Braking_count)AS DOUBLE PRECISION)),0))/10   as BrakingScore
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin,eco.Day
                                ),
                                HarshBraking  as
                                (
                                   SELECT eco.vin,eco.Day, ( CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION)/ NULLIF( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)),0))*100 as HarshBraking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin,eco.Day
                                ),
                                HarshBrakeDuration  as
                                (
                                   SELECT eco.vin,eco.Day,  CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION) as HarshBrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin,eco.Day
                                ),
                                BrakeDuration as
                                (
                                   SELECT eco.vin,eco.Day,  CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)/ 86400 as BrakeDuration
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin,eco.Day
                                ),
                                Braking as
                                (
                                   SELECT eco.vin,eco.Day,
	                               case when ((SUM (eco.end_time))-(SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) <>0 THEN 
                                   ( (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION))/ (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000))*100 
	                               ELSE null END as Braking
                                   FROM ecoscorequery eco
                                   GROUP BY eco.vin,eco.Day
                                ),
                                AnticipationScore as
                                (
                                    SELECT eco.vin,eco.Day,  (   (CAST(SUM(eco.dpa_anticipation_score)AS DOUBLE PRECISION ) ) / NULLIF(  (CAST (SUM(eco.dpa_anticipation_count) AS DOUBLE PRECISION) )  ,0) )/10 as AnticipationScore
                                    FROM ecoscorequery eco
                                    GROUP BY eco.vin,eco.Day
                                )
                                select
								eco.Day,'VIN_Driver' as HeaderType, eco.vin as VIN,eco.name as VehicleName,
								eco.driver1_id as DriverId
                                ,CAST(ecos.ecoscore AS DOUBLE PRECISION) as EcoScore
                                ,CAST(f.fuelconsumption/1000 AS DOUBLE PRECISION) as FuelConsumption
                                ,CAST(crus.cruisecontrolusage AS DOUBLE PRECISION) as CruiseControlUsage
                                ,CAST(crusa.cruisecontrolusage30 AS DOUBLE PRECISION) as CruiseControlUsage30
                                ,CAST(crucon.cruisecontrolusage50 AS DOUBLE PRECISION) as CruiseControlUsage50
                                ,CAST(crucont.cruisecontrolusage75 AS DOUBLE PRECISION) as CruiseControlUsage75
                                ,CAST(p.ptousage*100 AS DOUBLE PRECISION) as PTOUsage --convert Count to % by *100
                                ,CAST(pto.ptoduration AS DOUBLE PRECISION) as PTODuration
                                ,CAST(ads.averagedrivingspeed * 3.6 AS DOUBLE PRECISION) as AverageDrivingSpeed  --convert meter/second  to kmph  by *3.6
                                ,CAST(aspeed.averagespeed * 3.6 AS DOUBLE PRECISION) as AverageSpeed --convert meter/second  to kmph  by *3.6
                                ,CAST(h.heavythrottling *100 AS DOUBLE PRECISION) as HeavyThrottling  -- Conver count to % by *100
                                ,CAST(he.heavythrottleduration AS DOUBLE PRECISION) as HeavyThrottleDuration
                                ,CAST(i.idling AS DOUBLE PRECISION) as Idling  -- Conver count to % by *100
                                ,CAST(ide.idleduration AS DOUBLE PRECISION) as IdleDuration
                                ,CAST(br.brakingscore AS DOUBLE PRECISION) as BrakingScore
                                ,CAST(hr.harshbraking * 100 AS DOUBLE PRECISION) as HarshBraking -- Conver count to % by *100
                                ,CAST(hrdur.HarshBrakeDuration AS DOUBLE PRECISION) as HarshBrakeDuration
                                ,CAST(brdur.brakeduration AS DOUBLE PRECISION) as BrakeDuration
                                ,CAST(brk.braking AS DOUBLE PRECISION) as Braking
                                ,CAST(anc.anticipationscore  AS DOUBLE PRECISION) as AnticipationScore
                                
                                from generalblk eco  
                                Left join Distance dis on dis.vin = eco.vin   and dis.Day=eco.Day
                                Left join EcoScore ecos on ecos.vin = dis.vin   and ecos.Day=dis.Day
                                Left join FuelConsumption f on f.vin = ecos.vin    and f.Day=ecos.Day
                                Left join CruiseControlUsage crus  on crus.vin = f.vin     and crus.Day=f.Day
                                Left join CruiseControlUsage30 crusa  on crusa.vin = crus.vin and crusa.Day=crus.Day
                                Left join CruiseControlUsage50 crucon on crucon.vin = crusa.vin  and crucon.Day=crusa.Day
                                Left join CruiseControlUsage75 crucont on crucont.vin = crucon.vin  and crucont.Day=crucon.Day
                                Left join PTOUsage p on p.vin = crucont.vin and p.Day=crucont.Day
                                Left join PTODuration pto on pto.vin = p.vin   and pto.Day=p.Day
                                Left join AverageDrivingSpeed ads on ads.vin = pto.vin   and ads.Day=pto.Day
                                
                                Left join AverageSpeed aspeed on aspeed.vin = ads.vin  and aspeed.Day=ads.Day
                                Left join HeavyThrottling h on h.vin = aspeed.vin   and h.Day=aspeed.Day
                                Left join HeavyThrottleDuration he on he.vin = h.vin   and he.Day=h.Day
                                Left join Idling i on i.vin = he.vin and i.Day=he.Day
                                Left join IdleDuration ide on ide.vin = i.vin  and ide.Day=i.Day
                                Left join BrakingScore br on br.vin = ide.vin and br.Day=ide.Day
                                Left join HarshBraking hr on hr.vin = br.vin and hr.Day=br.Day
                                Left join HarshBrakeDuration hrdur on hrdur.vin = hr.vin  and hrdur.Day=hr.Day
                                Left join AnticipationScore anc on anc.vin = hrdur.vin   and anc.Day=hrdur.Day
                                Left join BrakeDuration brdur on brdur.vin = anc.vin   and brdur.Day=anc.Day
                                Left join Braking brk on brk.vin = brdur.vin   and brk.Day=brdur.Day

                                where 1 = 1 AND(dis.distance >= @MinDriverTotalDistance OR @MinDriverTotalDistance IS NULL)";

                List<EcoScoreReportSingleDriver> lstSingleDriver = (List<EcoScoreReportSingleDriver>)await _dataMartdataAccess.QueryAsync<EcoScoreReportSingleDriver>(query, parameters);
                return lstSingleDriver?.Count > 0 ? lstSingleDriver : new List<EcoScoreReportSingleDriver>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #endregion

        #region Eco-Score Data service

        public async Task<dynamic> GetKPIInfo(EcoScoreDataServiceRequest request)
        {
            DynamicParameters parameters;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@StartTimestamp", request.StartTimestamp);
                parameters.Add("@EndTimestamp", request.EndTimestamp);
                parameters.Add("@VIN", request.VIN);
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinDistance);
                parameters.Add("@AggregationType", Enum.GetName(typeof(AggregateType), request.AggregationType));
                parameters.Add("@Limit", request.EcoScoreRecordsLimit);
                parameters.Add("@UserPrefTimeZone", GetUserTimeZonePreference(request.AccountEmail, request.OrganizationId));

                string query =
                    @"WITH 
                    ecoscorequery as (
	                    SELECT eco.driver1_id, eco.trip_distance,eco.trip_id,
				                    eco.dpa_Braking_score, eco.dpa_Braking_count, eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
				                    eco.vin,eco.used_fuel,eco.pto_duration,eco.end_time,eco.start_time,eco.gross_weight_combination_count,
				                    eco.heavy_throttle_pedal_duration,eco.idle_duration,eco.harsh_brake_duration,eco.brake_duration,
				                    eco.cruise_control_usage , eco.cruise_control_usage_30_50,eco.cruise_control_usage_50_75,eco.cruise_control_usage_75,
				                    eco.tacho_gross_weight_combination,
                                    CASE WHEN @AggregationType = 'TRIP' THEN CAST(eco.trip_id AS TEXT)
					                     ELSE CAST(date_trunc(@AggregationType, to_timestamp(eco.end_time/1000) AT TIME ZONE @UserPrefTimeZone) AS TEXT) 
                                    END as aggregation_type
	                    FROM tripdetail.ecoscoredata eco
	                    INNER JOIN master.driver dr ON dr.driver_id = eco.driver1_id
	                    WHERE eco.start_time >= @StartTimestamp     --1623325980000
	                    AND eco.end_time <= @EndTimestamp           --1627051411000
	                    AND eco.vin = @VIN                          --'XLR0998HGFFT76657'
	                    AND eco.driver1_id = @DriverId              --'NL B000384974000000'
	                    AND eco.trip_distance >= @MinTripDistance
                    ),
                    GeneralQuery as 
                    (
	                    select eco.driver1_id, eco.aggregation_type, MIN(start_time) AS StartTimestamp, MAX(end_time) AS EndTimestamp
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    AverageGrossweight as 
                    (
	                    select eco.driver1_id, eco.aggregation_type, CAST(SUM(eco.tacho_gross_weight_combination) as DOUBLE PRECISION) as AverageGrossweight_Total, CAST(SUM (eco.gross_weight_combination_count) as DOUBLE PRECISION) as AverageGrossweight_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    Distance as 
                    (
	                    select eco.driver1_id, eco.aggregation_type, (CAST(SUM (eco.trip_distance)as DOUBLE PRECISION)) as Distance_Total, COUNT(1) as Distance_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    NumberOfTrips as 
                    (
	                    select eco.driver1_id, eco.aggregation_type, CAST(COUNT(eco.trip_id) AS INTEGER) as NumberOfTrips
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    NumberOfVehicles as 
                    (
	                    select eco.driver1_id, eco.aggregation_type, CAST(1 AS INTEGER) as NumberOfVehicles
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    AverageDistancePerDay as 
                    (
	                    select eco.driver1_id, eco.aggregation_type, (SUM(eco.trip_distance) / CEIL(CAST(MAX(end_time) - MIN(start_time) AS DOUBLE PRECISION)/(1000 * 60 * 60 * 24))) as AverageDistancePerDay_Total, CEIL(CAST(MAX(end_time) - MIN(start_time) AS DOUBLE PRECISION)/(1000 * 60 * 60 * 24)) as AverageDistancePerDay_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    EcoScore as
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type,
	                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  
		                    THEN (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
			                    (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
		                    ELSE 0 END as EcoScore_Total, SUM(eco.dpa_Braking_count) as EcoScore_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    FuelConsumption as 
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type, (CAST(SUM (eco.used_fuel)AS DOUBLE PRECISION )) as FuelConsumption_Total, 1 as FuelConsumption_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    CruiseControlUsage as 
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type, (CAST(SUM (eco.cruise_control_usage) AS DOUBLE PRECISION )) as CruiseControlUsage_Total, SUM(trip_distance) as CruiseControlUsage_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    CruiseControlUsage30 as 
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type, (CAST(SUM (eco.cruise_control_usage_30_50) AS DOUBLE PRECISION )) as CruiseControlUsage30_Total, SUM(trip_distance) as CruiseControlUsage30_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    CruiseControlUsage50 as 
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type, (CAST(SUM (eco.cruise_control_usage_50_75) AS DOUBLE PRECISION )) as CruiseControlUsage50_Total, SUM(trip_distance) as CruiseControlUsage50_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    CruiseControlUsage75 as 
                    (
                        SELECT eco.driver1_id, eco.aggregation_type, (CAST(SUM (eco.cruise_control_usage_75) AS DOUBLE PRECISION )) as CruiseControlUsage75_Total, SUM(trip_distance) as CruiseControlUsage75_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    PTOUsage as 
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type,
	                    CASE WHEN ( SUM (eco.end_time)- SUM (eco.start_time) ) <> 0 and (( SUM (eco.end_time)- SUM (eco.start_time) )/1000) <>0 
		                    THEN CAST(SUM(eco.pto_duration) AS DOUBLE PRECISION)
		                    ELSE 0 END as PTOUsage_Total, CAST((SUM (eco.end_time)- SUM (eco.start_time) )/1000 as DOUBLE PRECISION) as PTOUsage_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type		 
                    ),
                    PTODuration as 
                    (
                        SELECT eco.driver1_id, eco.aggregation_type, SUM(eco.pto_duration) as PTODuration_Total, 1 as PTODuration_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type	
                    ),
                    AverageDrivingSpeed as
                    (  
                        SELECT eco.driver1_id, eco.aggregation_type,  
                        CASE WHEN ((((SUM (eco.end_time)) - (SUM (eco.start_time)) )/1000)- (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))) <> 0 OR (( (SUM (eco.end_time)) - (SUM (eco.start_time))  ) <> 0 and (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)) <>0 ) 
		                    THEN (CAST(SUM(eco.trip_distance)AS DOUBLE PRECISION))  
		                    ELSE 0 END as AverageDrivingSpeed_Total, ((((SUM (eco.end_time)) - (SUM (eco.start_time)))/1000) - (CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION))) as AverageDrivingSpeed_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    AverageSpeed as
                    (
                        SELECT eco.driver1_id, eco.aggregation_type, 
                        CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <>0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <>0
		                    THEN CAST(SUM(eco.trip_distance) AS DOUBLE PRECISION)
		                    ELSE 0 END as AverageSpeed_Total, CAST(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000 AS DOUBLE PRECISION) as AverageSpeed_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    HeavyThrottling as
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type,
	                    CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000)<>0 
		                    THEN CAST(SUM(eco.heavy_throttle_pedal_duration) AS DOUBLE PRECISION)
		                    ELSE 0 END as HeavyThrottling_Total, CAST(((SUM (eco.end_time))- (SUM (eco.start_time)))/1000 AS DOUBLE PRECISION) as HeavyThrottling_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    HeavyThrottleDuration  as
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type, (CAST(SUM(eco.heavy_throttle_pedal_duration ) AS DOUBLE PRECISION)) as HeavyThrottleDuration_Total, 1 as HeavyThrottleDuration_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    Idling  as
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type,
	                    CASE WHEN ((SUM (eco.end_time))- (SUM (eco.start_time)))<> 0 and (((SUM (eco.end_time))- (SUM (eco.start_time)))/1000) <> 0  
		                    THEN (CAST(SUM(eco.idle_duration) AS DOUBLE PRECISION))
		                    ELSE 0 END as Idling_Total, CAST(((SUM (eco.end_time))- SUM (eco.start_time))/1000 AS DOUBLE PRECISION) as Idling_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    IdleDuration  as
                    (
                        SELECT eco.driver1_id, eco.aggregation_type, CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION) as IdleDuration_Total, 1 as IdleDuration_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    BrakingScore  as
                    (
                        SELECT eco.driver1_id, eco.aggregation_type, (CAST(SUM(eco.dpa_Braking_score) AS DOUBLE PRECISION)/10)  as BrakingScore_Total, NULLIF((CAST(SUM (eco.dpa_Braking_count)AS DOUBLE PRECISION)),0) as BrakingScore_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    HarshBraking  as
                    (
                        SELECT eco.driver1_id, eco.aggregation_type, CAST(SUM(eco.harsh_brake_duration) AS DOUBLE PRECISION) as HarshBraking_Total, NULLIF((CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION)),0) as HarshBraking_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    HarshBrakeDuration  as
                    (
                        SELECT eco.driver1_id, eco.aggregation_type, CAST(SUM(eco.harsh_brake_duration)AS DOUBLE PRECISION) as HarshBrakeDuration_Total, 1 as HarshBrakeDuration_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    BrakeDuration as
                    (
                        SELECT eco.driver1_id, eco.aggregation_type, CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION) as BrakeDuration_Total, 1 as BrakeDuration_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    Braking as
                    (
                        SELECT eco.driver1_id, eco.aggregation_type,
                        CASE WHEN ((SUM (eco.end_time))-(SUM (eco.start_time))) <> 0 and (((SUM (eco.end_time))-(SUM (eco.start_time)))/1000) <> 0 
	                    THEN (CAST(SUM(eco.brake_duration)AS DOUBLE PRECISION))
	                    ELSE 0 END as Braking_Total, (CAST(((SUM(eco.end_time))-(SUM (eco.start_time)))/1000 AS DOUBLE PRECISION) - CAST(SUM(eco.idle_duration)AS DOUBLE PRECISION)) as Braking_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    AnticipationScore as
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type, ((CAST(SUM(eco.dpa_anticipation_score)AS DOUBLE PRECISION))/10) as AnticipationScore_Total, NULLIF((CAST(SUM(eco.dpa_anticipation_count) AS DOUBLE PRECISION)) ,0) as AnticipationScore_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    )
                    SELECT
                    StartTimestamp,
                    EndTimestamp,
                    -- No. of Trips
                    NumberOfTrips,
                    -- No. of Vehicles
                    NumberOfVehicles,
                    -- Average Gross Weight
                    AverageGrossweight_Total, AverageGrossweight_Count,
                    -- Distance
                    Distance_Total, Distance_Count,
                    -- Average Distance per day
                    AverageDistancePerDay_Total, AverageDistancePerDay_Count,
                    -- Eco Score
                    EcoScore_Total, EcoScore_Count,
                    -- Fuel Consumption
                    FuelConsumption_Total, FuelConsumption_Count,
                    -- Cruise Control Usage
                    CruiseControlUsage_Total * 100 as CruiseControlUsage_Total, CruiseControlUsage_Count,
                    -- Cruise Control Usage30-50
                    CruiseControlUsage30_Total * 100 as CruiseControlUsage30_Total, CruiseControlUsage30_Count,
                    -- Cruise Control Usage50-75
                    CruiseControlUsage50_Total * 100 as CruiseControlUsage50_Total, CruiseControlUsage50_Count,
                    -- Cruise Control Usage75+
                    CruiseControlUsage75_Total * 100 as CruiseControlUsage75_Total, CruiseControlUsage75_Count,
                    -- PTO Usage
                    PTOUsage_Total * 100 as PTOUsage_Total, PTOUsage_Count,
                    -- PTO Duration
                    PTODuration_Total * 1000 as PTODuration_Total, PTODuration_Count,
                    -- Average Driving Speed
                    AverageDrivingSpeed_Total, AverageDrivingSpeed_Count,
                    -- Average Speed
                    AverageSpeed_Total, AverageSpeed_Count,
                    -- Heavy Throttling
                    HeavyThrottling_Total * 100 as HeavyThrottling_Total, HeavyThrottling_Count,
                    -- Heavy Throttle Duration
                    HeavyThrottleDuration_Total * 1000 as HeavyThrottleDuration_Total, HeavyThrottleDuration_Count,
                    -- Idling
                    Idling_Total * 100 as Idling_Total, Idling_Count,
                    -- Idle Duration 
                    IdleDuration_Total * 1000 as IdleDuration_Total, IdleDuration_Count,
                    -- Braking Score
                    BrakingScore_Total, BrakingScore_Count,
                    -- Harsh Braking
                    HarshBraking_Total * 100 as HarshBraking_Total, HarshBraking_Count,
                    -- Harsh Braking Duration
                    HarshBrakeDuration_Total * 1000 as HarshBrakeDuration_Total, HarshBrakeDuration_Count,
                    -- Brake Duration
                    BrakeDuration_Total * 1000 as BrakeDuration_Total, BrakeDuration_Count,
                    -- Braking
                    Braking_Total * 100 as Braking_Total, Braking_Count,
                    -- Anticipation Score
                    AnticipationScore_Total, AnticipationScore_Count
                    FROM GeneralQuery gq
                    Left join AverageGrossweight avrg on gq.driver1_id = avrg.driver1_id and gq.aggregation_type = avrg.aggregation_type
                    Left join Distance dis on dis.driver1_id = avrg.driver1_id and dis.aggregation_type = avrg.aggregation_type
                    Left join NumberOfTrips notrp on notrp.driver1_id = dis.driver1_id and notrp.aggregation_type = dis.aggregation_type
                    Left join numberofvehicles noveh on noveh.driver1_id = notrp.driver1_id and noveh.aggregation_type = notrp.aggregation_type
                    Left join AverageDistancePerDay avgdperday on avgdperday.driver1_id = noveh.driver1_id and avgdperday.aggregation_type = noveh.aggregation_type
                    Left join EcoScore ecos on ecos.driver1_id = avgdperday.driver1_id and ecos.aggregation_type = avgdperday.aggregation_type
                    Left join FuelConsumption f on f.driver1_id = ecos.driver1_id and f.aggregation_type = ecos.aggregation_type
                    Left join CruiseControlUsage crus  on crus.driver1_id = f.driver1_id and crus.aggregation_type = f.aggregation_type
                    Left join CruiseControlUsage30 crusa  on crusa.driver1_id = crus.driver1_id  and crusa.aggregation_type = crus.aggregation_type
                    Left join CruiseControlUsage50 crucon on crucon.driver1_id = crusa.driver1_id and crucon.aggregation_type = crusa.aggregation_type
                    Left join CruiseControlUsage75 crucont on crucont.driver1_id = crucon.driver1_id and crucont.aggregation_type = crucon.aggregation_type
                    Left join PTOUsage p on p.driver1_id = crucont.driver1_id and p.aggregation_type = crucont.aggregation_type
                    Left join PTODuration pto on pto.driver1_id = p.driver1_id and pto.aggregation_type = p.aggregation_type
                    Left join AverageDrivingSpeed ads on ads.driver1_id = pto.driver1_id and ads.aggregation_type = pto.aggregation_type
                    Left join AverageSpeed aspeed on aspeed.driver1_id = ads.driver1_id and aspeed.aggregation_type = ads.aggregation_type
                    Left join HeavyThrottling h on h.driver1_id = aspeed.driver1_id and h.aggregation_type = aspeed.aggregation_type
                    Left join HeavyThrottleDuration he on he.driver1_id = h.driver1_id and he.aggregation_type = h.aggregation_type
                    Left join Idling i on i.driver1_id = he.driver1_id and i.aggregation_type = he.aggregation_type
                    Left join IdleDuration ide on ide.driver1_id = i.driver1_id and ide.aggregation_type = i.aggregation_type
                    Left join BrakingScore br on br.driver1_id = ide.driver1_id and br.aggregation_type = ide.aggregation_type
                    Left join HarshBraking hr on hr.driver1_id = br.driver1_id and hr.aggregation_type = br.aggregation_type
                    Left join HarshBrakeDuration hrdur on hrdur.driver1_id = hr.driver1_id and hrdur.aggregation_type = hr.aggregation_type
                    Left join AnticipationScore anc on anc.driver1_id = hrdur.driver1_id and anc.aggregation_type = hrdur.aggregation_type
                    Left join BrakeDuration brdur on brdur.driver1_id = anc.driver1_id and brdur.aggregation_type = anc.aggregation_type
                    Left join Braking brk on brk.driver1_id = brdur.driver1_id and brk.aggregation_type = brdur.aggregation_type
                    order by StartTimestamp LIMIT @Limit";

                return await _dataMartdataAccess.QueryAsync<dynamic>(query, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> GetChartInfo(EcoScoreDataServiceRequest request)
        {
            DynamicParameters parameters;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@StartTimestamp", request.StartTimestamp);
                parameters.Add("@EndTimestamp", request.EndTimestamp);

                parameters.Add("@VIN", request.VIN);
                parameters.Add("@DriverId", request.DriverId);
                parameters.Add("@MinTripDistance", request.MinDistance);
                parameters.Add("@AggregationType", Enum.GetName(typeof(AggregateType), request.AggregationType));
                parameters.Add("@Limit", request.EcoScoreRecordsLimit);
                parameters.Add("@UserPrefTimeZone", GetUserTimeZonePreference(request.AccountEmail, request.OrganizationId));

                string query =
                    @"WITH 
                    ecoscorequery as (
	                    SELECT eco.driver1_id,eco.dpa_Braking_score, eco.dpa_Braking_count, 
								eco.dpa_anticipation_score, eco.dpa_anticipation_count, 
								eco.used_fuel,eco.end_time,eco.start_time,
								CASE WHEN @AggregationType = 'TRIP' THEN CAST(eco.trip_id AS TEXT)
									 ELSE CAST(date_trunc(@AggregationType, to_timestamp(eco.end_time/1000) AT TIME ZONE @UserPrefTimeZone) AS TEXT) 
								END as aggregation_type
	                    FROM tripdetail.ecoscoredata eco
	                    INNER JOIN master.driver dr ON dr.driver_id = eco.driver1_id
	                    WHERE eco.start_time >= @StartTimestamp         --1623325980000
	                    AND eco.end_time <= @EndTimestamp               --1627051411000
	                    AND eco.vin = @VIN                              --'XLR0998HGFFT76657'
	                    AND eco.driver1_id = @DriverId                  --'NL B000384974000000'
	                    AND eco.trip_distance >= @MinTripDistance
                    ),
                    GeneralQuery as 
                    (
	                    select eco.driver1_id, eco.aggregation_type, MIN(start_time) AS StartTimestamp, MAX(end_time) AS EndTimestamp
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    EcoScore as
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type,
	                    CASE WHEN CAST(SUM(dpa_Braking_count) AS DOUBLE PRECISION)<> 0 and CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION) <> 0  
		                    THEN (((CAST(SUM(dpa_Braking_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_Braking_count)AS DOUBLE PRECISION)) +
			                    (CAST(SUM(dpa_anticipation_score)AS DOUBLE PRECISION) / CAST(SUM(dpa_anticipation_count)AS DOUBLE PRECISION)))/2)/10 
		                    ELSE 0 END as EcoScore_Total, SUM(eco.dpa_Braking_count) as EcoScore_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    FuelConsumption as 
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type, (CAST(SUM (eco.used_fuel)AS DOUBLE PRECISION )) as FuelConsumption_Total, 1 as FuelConsumption_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    BrakingScore  as
                    (
                        SELECT eco.driver1_id, eco.aggregation_type, (CAST(SUM(eco.dpa_Braking_score) AS DOUBLE PRECISION)/10)  as BrakingScore_Total, NULLIF((CAST(SUM (eco.dpa_Braking_count)AS DOUBLE PRECISION)),0) as BrakingScore_Count
                        FROM ecoscorequery eco
                        GROUP BY eco.driver1_id, eco.aggregation_type
                    ),
                    AnticipationScore as
                    (
	                    SELECT eco.driver1_id, eco.aggregation_type, ((CAST(SUM(eco.dpa_anticipation_score)AS DOUBLE PRECISION))/10) as AnticipationScore_Total, NULLIF((CAST(SUM(eco.dpa_anticipation_count) AS DOUBLE PRECISION)) ,0) as AnticipationScore_Count
	                    FROM ecoscorequery eco
	                    GROUP BY eco.driver1_id, eco.aggregation_type
                    )
                    SELECT
                    StartTimestamp,
                    EndTimestamp,                   
                    -- Eco Score
                    EcoScore_Total, EcoScore_Count,
                    -- Fuel Consumption
                    FuelConsumption_Total, FuelConsumption_Count,                    
                    -- Braking Score
                    BrakingScore_Total, BrakingScore_Count,                    
                    -- Anticipation Score
                    AnticipationScore_Total, AnticipationScore_Count
                    FROM GeneralQuery gq
                    Left join EcoScore ecos on ecos.driver1_id = gq.driver1_id and ecos.aggregation_type = gq.aggregation_type
                    Left join FuelConsumption f on f.driver1_id = ecos.driver1_id and f.aggregation_type = ecos.aggregation_type
                    Left join BrakingScore br on br.driver1_id = f.driver1_id and br.aggregation_type = f.aggregation_type
                    Left join AnticipationScore anc on anc.driver1_id = br.driver1_id and anc.aggregation_type = br.aggregation_type                    
                    order by StartTimestamp LIMIT @Limit";

                return await _dataMartdataAccess.QueryAsync<dynamic>(query, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> GetUserTimeZonePreference(string emailId, string orgCode)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@emailId", emailId.ToLower());

                string accountQuery =
                    @"SELECT preference_id from master.account where lower(email) = @emailId";

                var accountPreferenceId = await _dataAccess.QueryFirstAsync<int?>(accountQuery, parameter);

                if (!accountPreferenceId.HasValue)
                {
                    string orgQuery = string.Empty;
                    int? orgPreferenceId = null;
                    if (!string.IsNullOrEmpty(orgCode))
                    {
                        var orgParameter = new DynamicParameters();
                        orgParameter.Add("@orgCode", orgCode);

                        orgQuery = @"SELECT preference_id from master.organization WHERE org_id=@orgCode";

                        orgPreferenceId = await _dataAccess.QueryFirstAsync<int?>(orgQuery, orgParameter);
                    }
                    else
                    {
                        orgQuery =
                            @"SELECT o.preference_id from master.account acc
                            INNER JOIN master.accountOrg ao ON acc.id=ao.account_id
                            INNER JOIN master.organization o ON ao.organization_id=o.id
                            where lower(acc.email) = @emailId";

                        orgPreferenceId = await _dataAccess.QueryFirstAsync<int?>(orgQuery, parameter);
                    }

                    if (!orgPreferenceId.HasValue)
                        return "Europe/Amsterdam";
                    else
                        return await GetTimeZoneByPreferenceId(orgPreferenceId.Value);
                }
                return await GetTimeZoneByPreferenceId(accountPreferenceId.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> GetTimeZoneByPreferenceId(int preferenceId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@preferenceId", preferenceId);

                string query =
                    @"SELECT tz.name from master.accountpreference ap
                    INNER JOIN master.timezone tz ON ap.id = @preferenceId AND ap.timezone_id=tz.id";

                var timeZone = await _dataAccess.QueryFirstAsync<string>(query, parameter);

                return timeZone;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
