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
                parameters.Add("@created_by", dto.ActionedBy);

                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameters);
                dto.Id = id;

                //Insert into EcoScoreProfileKPI table
                foreach(var profileKPI in dto.ProfileKPIs)
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
                    parameters.Add("@created_by", dto.ActionedBy);

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

        #endregion
        public async Task<string> UpdateEcoScoreProfile(EcoScoreProfileDto ecoScoreProfileDto)
        {
            _dataAccess.Connection.Open();
            IDbTransaction txnScope = _dataAccess.Connection.BeginTransaction();
            try
            {

                var Updateparameter = new DynamicParameters();

            StringBuilder queryForUpdateEcoScoreProfile = new StringBuilder();

            queryForUpdateEcoScoreProfile.Append("UPDATE master.ecoscoreprofile set modified_at=@modified_at");

            if (!string.IsNullOrEmpty(ecoScoreProfileDto.Name))
            {
                Updateparameter.Add("@name", ecoScoreProfileDto.Name);
                queryForUpdateEcoScoreProfile.Append(", name=@name");
            }
            if (!string.IsNullOrEmpty(ecoScoreProfileDto.Description))
            {
                Updateparameter.Add("@description", ecoScoreProfileDto.Description);
                queryForUpdateEcoScoreProfile.Append(", description=@description");
            }
            Updateparameter.Add("@modified_by", ecoScoreProfileDto.ActionedBy);
            queryForUpdateEcoScoreProfile.Append(", modified_by=@modified_by");
            Updateparameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            Updateparameter.Add("@id", ecoScoreProfileDto.Id);
            queryForUpdateEcoScoreProfile.Append(" where id=@id RETURNING id");

            

            var id = await _dataAccess.ExecuteScalarAsync<int>(queryForUpdateEcoScoreProfile.ToString(), Updateparameter);

                var tripDetails = await UpdateEcoscoreProfileKpi(ecoScoreProfileDto.ProfileKPIs, ecoScoreProfileDto.ActionedBy, id);

                txnScope.Commit();
                return ecoScoreProfileDto.Name;
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
        private async Task<bool> UpdateEcoscoreProfileKpi(List<EcoScoreProfileKPI> ecoScoreProfileKPI, int ActionedBy, int id)
        {
            var Updateparameter = new DynamicParameters();
            foreach (var item in ecoScoreProfileKPI)
            {

                var temp = new EcoScoreProfileKPI();
                temp.KPIId = item.KPIId;
                temp.LimitValue = item.LimitValue;
                temp.LowerValue = item.LowerValue;
                temp.TargetValue = item.TargetValue;
                temp.UpperValue = item.UpperValue;

                //Updateparameter.Add("@KPIId", temp.KPIId);
                Updateparameter.Add("@LimitValue", temp.LimitValue);
                Updateparameter.Add("@LowerValue", temp.LowerValue);
                Updateparameter.Add("@TargetValue", temp.TargetValue);
                Updateparameter.Add("@UpperValue", temp.UpperValue);
                Updateparameter.Add("@KPIId", temp.KPIId);
                Updateparameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                Updateparameter.Add("@modified_by", ActionedBy);
                Updateparameter.Add("@Id", id);

                var updateEcoscoreProfileKpiTable = @"UPDATE master.ecoscoreprofilekpi
                                          SET   limit_val=@LimitValue,
                                                target_val=@TargetValue, 
                                                lower_val=@LowerValue, 
                                                upper_val=@UpperValue, 
                                                modified_at=@modified_at, 
                                                modified_by=@modified_by
                                                where profile_id = @Id 
                                                       and ecoscore_kpi_id = @KPIId RETURNING id";

                id = await _dataAccess.ExecuteScalarAsync<int>(updateEcoscoreProfileKpiTable, Updateparameter);
            }
            return id > 0 ? true : false;
        }

        public async Task<bool> CheckEcoScoreProfileIsexist(int? OrganizationId, string Name)
        {
            var parameterduplicate = new DynamicParameters();

            StringBuilder queryForProfileIsexist = new StringBuilder();

            queryForProfileIsexist.Append("SELECT id FROM master.ecoscoreprofile where state in ('A','I')");

            if (!string.IsNullOrEmpty(Name))
            {
                parameterduplicate.Add("@name", Name);
                queryForProfileIsexist.Append("and name=@name");
            }

            if (OrganizationId != 0)
            {
                parameterduplicate.Add("@organization_id", OrganizationId);
                queryForProfileIsexist.Append(" and organization_id = @organization_id");
            }
            else
            {
                queryForProfileIsexist.Append(" and organization_id is null ");
            }
            int ReportNameExist = await _dataAccess.ExecuteScalarAsync<int>(queryForProfileIsexist.ToString(), parameterduplicate);

            return ReportNameExist == 0 ? false : true;
        }
    }
}
