using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    }
}
