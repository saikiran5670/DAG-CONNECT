using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.role.repository
{
    public class RoleRepository : IRoleRepository
    {

        private readonly IDataAccess _dataAccess;

        public RoleRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task<int> CreateRole(RoleMaster roleMaster)
        {

            var RoleQueryStatement = @" INSERT INTO master.role
                                    (organization_id,name,state,created_at,created_by,description,feature_set_id,level,code) 
	                                VALUES (@organization_id,@name,@state,@created_at,@created_by,@description,@feature_set_id,@level,@code)
	                                RETURNING id";

            var roleparameter = new DynamicParameters();
            roleparameter.Add("@organization_id", roleMaster.Organization_Id == 0 ? null : roleMaster.Organization_Id);
            roleparameter.Add("@name", roleMaster.Name);
            roleparameter.Add("@state", 'A');
            roleparameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
            roleparameter.Add("@created_by", roleMaster.Created_by);
            roleparameter.Add("@description", roleMaster.Description);
            roleparameter.Add("@feature_set_id", roleMaster.Feature_set_id > 0 ? roleMaster.Feature_set_id : null);
            roleparameter.Add("@level", roleMaster.Level);
            roleparameter.Add("@code", roleMaster.Code.ToUpper());

            int insertedRoleId = await _dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, roleparameter);

            return insertedRoleId;
        }

        public async Task<int> Updaterolefeatureset(int roleId, int featureSetId)
        {
            var RoleQueryStatement = @"UPDATE master.role
                                    SET feature_set_id = @feature_set_id
                                    ,updated_date = @modified_date
                                    ,updated_by = @modified_by
                                    WHERE id = @role_id
                                    RETURNING id;";


            var Roleparameter = new DynamicParameters();
            Roleparameter.Add("@feature_set_id", featureSetId);
            Roleparameter.Add("@role_id", roleId);


            int InsertedRoleId = await _dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, Roleparameter);
            return InsertedRoleId;
        }

        public async Task<int> DeleteRole(int roleid, int Accountid)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@roleid", roleid);
                parameter.Add("@state", 'D');
                parameter.Add("@updated_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@updated_by", Accountid);

                var RoleQueryStatement = @"UPDATE master.role
                                    SET state = @state
                                    ,modified_at = @updated_date
                                    ,modified_by = @updated_by
                                    WHERE id = @roleid
                                    RETURNING id;";

                int resultDeletedRole = await _dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, parameter);

                // var RoleFeatureQueryStatement = @"UPDATE dafconnectmaster.rolefeatureset 
                //                         SET isactive = @isactive
                //                         ,updateddate = @updateddate
                //                         ,updatedby = @updatedby
                //                         WHERE rolemasterid = @roleid
                //                         RETURNING rolefeaturesetid;";
                // int resultDeleteRoleFeature = await dataAccess.ExecuteScalarAsync<int>(RoleFeatureQueryStatement, parameter);
                // if (resultDeleteRoleFeature > 0)
                // {
                //     return resultDeleteRoleFeature;
                // }
                return resultDeletedRole;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IEnumerable<AssignedRoles>> IsRoleAssigned(int roleid)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@roleid", roleid);

                string roleQueryStatement = @"SELECT distinct ar.account_id as accountid, ar.role_id as roleid, a.salutation as salutation, a.first_name as firstname, a.last_name as lastname
                                            FROM master.accountrole ar left join master.account a
                                            on ar.account_id= a.id
                                            where role_id=@roleid";
                var accounts = await _dataAccess.QueryAsync<AssignedRoles>(roleQueryStatement, parameter);

                return accounts;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IEnumerable<RoleMaster>> GetRoles(RoleFilter roleFilter)
        {

            //    var QueryStatement = @" SELECT role.rolemasterid, 
            //                                     role.name,
            //                                     role.isactive,
            //                                     role.createddate,
            //                                     role.createdby,
            //                                     role.updateddate,
            //                                     role.updatedby,
            //                                     roleFeatureSet.featuresetid
            //                             FROM dafconnectmaster.rolemaster role
            //                             LEFT JOIN dafconnectmaster.rolefeatureset roleFeatureSet
            //                             ON role.rolemasterid=roleFeatureSet.rolemasterid
            //                             WHERE (role.rolemasterid=@roleid or @roleid=0)
            //                             and role.isactive=true;";

            var QueryStatement = @"SELECT role.id, 
                                role.organization_id, 
                                role.name, 
                                role.state, 
                                role.description,
                                role.created_at, 
                                role.created_by, 
                                role.modified_at,
                                role.modified_by,
                                role.feature_set_id,
                                role.level,
                                role.code
	                            FROM master.role role
								WHERE state != 'D'";


            var parameter = new DynamicParameters();
            if (roleFilter.RoleId > 0)
            {
                parameter.Add("@id", roleFilter.RoleId);
                QueryStatement = QueryStatement + " and id  = @id";

            }
            // organization id filter
            if (roleFilter.Organization_Id > 0)
            {
                parameter.Add("@organization_id", roleFilter.Organization_Id);
                QueryStatement = QueryStatement + " and organization_id  = @organization_id";

                if (roleFilter.IsGlobal == true)
                {
                    parameter.Add("@id", roleFilter.RoleId);
                    QueryStatement = QueryStatement + " or (organization_id  is null and state = 'A')";

                }

            }
            else if (roleFilter.Organization_Id == 0)
            {
                if (roleFilter.IsGlobal == true)
                {
                    QueryStatement = QueryStatement + " and  organization_id  is null";

                }

            }


            IEnumerable<RoleMaster> roledetails = await _dataAccess.QueryAsync<RoleMaster>(QueryStatement, parameter);
            return roledetails;

        }



        public async Task<int> UpdateRole(RoleMaster roleMaster)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@id", roleMaster.Id);
            parameter.Add("@organization_id", roleMaster.Organization_Id);
            parameter.Add("@name", roleMaster.Name);
            parameter.Add("@feature_set_id", roleMaster.Feature_set_id);
            parameter.Add("@updatedby", roleMaster.Updatedby);
            parameter.Add("@updateddate", UTCHandling.GetUTCFromDateTime(DateTime.Now));
            parameter.Add("@description", roleMaster.Description);
            parameter.Add("@level", roleMaster.Level);
            parameter.Add("@code", roleMaster.Code.ToUpper());
            var RoleQueryStatement = @" UPDATE master.role
                                            SET name=@name,
                                            description= @Description,
                                             feature_set_id = @feature_set_id,
                                            modified_by=@updatedby,
                                           modified_at=@updateddate,
                                           level=@level,
                                           code=@code
                                        WHERE id = @id
                                        RETURNING id;";
            int resultUpdatedRole = await _dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, parameter);

            // if (roleMaster.FeatureSetID > 0)
            // {
            //     var RoleFeatureQueryStatement = @" UPDATE dafconnectmaster.rolefeatureset
            //                                 SET featuresetid=@featuresetid,
            //                                 updatedby=@updatedby,
            //                                 updateddate=@updateddate  
            //                             WHERE rolemasterid = @rolemasterid
            //                             RETURNING rolemasterid;";
            //     int resultUpdatedRoleFeature = await dataAccess.ExecuteScalarAsync<int>(RoleFeatureQueryStatement, parameter);
            //     return resultUpdatedRoleFeature;
            // }
            return resultUpdatedRole;
        }

        public int CheckRoleNameExist(string roleName, int Organization_Id, int roleid)
        {
            var QueryStatement = @" SELECT CASE WHEN id IS NULL THEN 0 ELSE id END
                                    FROM master.role 
                                    WHERE state='A'
                                    AND LOWER(name) = LOWER(@roleName) and (organization_id = @organization_id or organization_id is null)";
            var parameter = new DynamicParameters();
            if (roleid > 0)
            {
                parameter.Add("@roleid", roleid);
                QueryStatement = QueryStatement + " and id != @roleid";
            }

            parameter.Add("@roleName", roleName.Trim());
            parameter.Add("@organization_id", Organization_Id);
            int resultRoleId = _dataAccess.ExecuteScalar<int>(QueryStatement, parameter);
            return resultRoleId;

        }

        public async Task<IEnumerable<string>> GetCode(RoleCodeFilter roleFilter)
        {

            var queryStatement = @"select distinct code 
                                            from master.role 
                                            where state=@state and level=@level ";

            var parameter = new DynamicParameters();
            parameter.Add("@level", roleFilter.RoleLevel);
            parameter.Add("@state", 'A');
            // organization id filter
            if (roleFilter.OrganizationId > 0)
            {
                parameter.Add("@organization_id", roleFilter.OrganizationId);
                queryStatement = queryStatement + " and organization_id  = @organization_id";
            }
            else if (roleFilter.OrganizationId == 0)
            {
                //if (roleFilter.IsGlobal == true)
                //{
                queryStatement = queryStatement + " and  organization_id  is null";

                // }
            }
            IEnumerable<string> roledetails = await _dataAccess.QueryAsync<string>(queryStatement, parameter);
            return roledetails;
        }

    }
}
