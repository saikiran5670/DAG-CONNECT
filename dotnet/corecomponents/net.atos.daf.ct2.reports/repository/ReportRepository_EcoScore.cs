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
                        (profile_id,ecoscore_kpi_id,limit_val,target_val, lower_val, upper_val, created_at, created_by)
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
                                 case when default_es_version_type is null then 'TRUE' ELSE 'FALSE' end as IsDeleteAllowed
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
            objProfile.IsDeleteAllowed = Convert.ToBoolean(profile.isdeleteallowed);
            objProfile.OrganizationId = profile.organizationid;
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
                    query.Append("on pro.id= prokpi.profile_id ");
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
                query.Append(" where profile_id=@Id and ecoscore_kpi_id = @KPIId RETURNING id");

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

                string query = @"WITH ecoscore AS (
                                 SELECT 
                                 dr.first_name || ' ' || dr.last_name AS driverName,
                                 eco.driver1_id as driverid,
                                 (
                                 	(
                                 	 (SUM(dpa_braking_score)/ SUM(dpa_braking_count)) +
                                 	 (SUM(dpa_anticipation_score)/ SUM(dpa_anticipation_count))
                                 	)/2
                                 )/10 as ecoscoreranking
                                 FROM tripdetail.ecoscoredata eco
                                 JOIN master.driver dr 
                                 	ON dr.driver_id = eco.driver1_id
                                 WHERE eco.start_time >= @FromDate
                                 	AND eco.end_time <= @ToDate
                                 	AND eco.vin = ANY( @Vins )
                                 GROUP BY dr.first_name, dr.last_name, eco.driver1_id
                                 ORDER BY ecoscoreranking DESC
                                 )
                                 SELECT ROW_NUMBER () OVER (ORDER BY  ecoscoreranking DESC) as Ranking,
                                 driverName, driverid, ecoscoreranking
                                 FROM ecoscore
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
        public async Task<EcoScoreKPIRanking> GetEcoScoreTargetProfileKPIValues(EcoScoreReportByAllDriversRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ReportId", request.ReportId);
                parameters.Add("@OrgId", request.OrgId);
                parameters.Add("@AccountId", request.AccountId);
                parameters.Add("@TargetProfileId", request.TargetProfileId);

                string query = @"SELECT eco.id as profileid, kpimst.name as kpiname,
                                 kpi.limit_val as minvalue, kpi.target_val as targetvalue
                                 FROM master.ecoscoreprofile eco
                                 JOIN master.reportpreference rep
                                 	ON eco.id= rep.ecoscore_profile_id
                                 JOIN master.ecoscoreprofilekpi kpi
                                 	ON eco.id=kpi.profile_id
                                 JOIN master.ecoscorekpi kpimst
                                 	ON kpimst.id = kpi.ecoscore_kpi_id
                                 WHERE rep.report_id = @ReportId
                                 AND rep.organization_id = @OrgId
                                 AND rep.account_id = @AccountId
                                 AND eco.id = @TargetProfileId
                                 AND kpimst.name = 'Eco-Score'";

                EcoScoreKPIRanking obkEcoScoreKPI = (EcoScoreKPIRanking)await _dataAccess.QueryAsync<dynamic>(query, parameters);
                return obkEcoScoreKPI;
            }
            catch (Exception)
            {
                throw;
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
                                            AND report_id = @ReportId
                                            AND state ='A'
                                            AND type= 'D'  RETURNING id";

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

    }
}
