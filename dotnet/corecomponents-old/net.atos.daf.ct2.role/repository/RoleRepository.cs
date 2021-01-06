using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.data;
using Dapper;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.role.repository
{
    public class RoleRepository : IRoleRepository
    {

        private readonly IDataAccess dataAccess;

        public RoleRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
        public async Task<int> AddRole(RoleMaster roleMaster)
        {

            var RoleQueryStatement = @" INSERT INTO dafconnectmaster.rolemaster
                                    (name,isactive,createddate,createdby) 
	                                VALUES (@name,@isactive,@createddate,@createdby)
	                                RETURNING rolemasterid";

            var Roleparameter = new DynamicParameters();
            Roleparameter.Add("@name", roleMaster);
            Roleparameter.Add("@isactive", true);
            Roleparameter.Add("@createddate", DateTime.Now);
            Roleparameter.Add("@createdby", roleMaster.createdby);

            int InsertedRoleId = await dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, Roleparameter);
            if (roleMaster.FeatureSetID > 0)
            {
                var RoleFeatureQueryStatement = @" INSERT INTO dafconnectmaster.rolefeatureset
                                    (rolemasterid,featuresetid,isactive,createddate,createdby) 
	                                VALUES (@rolemasterid,@featuresetid,@isactive,@createddate,@createdby)
	                                RETURNING rolefeaturesetid";

                var RoleFeatureparameter = new DynamicParameters();
                RoleFeatureparameter.Add("@rolemasterid", InsertedRoleId);
                RoleFeatureparameter.Add("@featuresetid", roleMaster.FeatureSetID);
                RoleFeatureparameter.Add("@isactive", true);
                RoleFeatureparameter.Add("@createddate", DateTime.Now);
                RoleFeatureparameter.Add("@createdby", roleMaster.createdby);

                int resultAddRoleFeature = await dataAccess.ExecuteScalarAsync<int>(RoleFeatureQueryStatement, RoleFeatureparameter);
                return resultAddRoleFeature;
            }
            return InsertedRoleId;
        }

        public async Task<int> DeleteRole(int roleid, int userid)
        {

            var parameter = new DynamicParameters();
            parameter.Add("@roleid", roleid);
            parameter.Add("@isactive", false);
            parameter.Add("@updateddate", DateTime.Now);
            parameter.Add("@updatedby", userid);

            var RoleQueryStatement = @"UPDATE dafconnectmaster.rolemaster 
                                    SET isactive = @isactive
                                    ,updateddate = @updateddate
                                    ,updatedby = @updatedby
                                    WHERE rolemasterid = @roleid
                                    RETURNING rolemasterid;";

            int resultDeletedRole = await dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, parameter);

            var RoleFeatureQueryStatement = @"UPDATE dafconnectmaster.rolefeatureset 
                                    SET isactive = @isactive
                                    ,updateddate = @updateddate
                                    ,updatedby = @updatedby
                                    WHERE rolemasterid = @roleid
                                    RETURNING rolefeaturesetid;";
            int resultDeleteRoleFeature = await dataAccess.ExecuteScalarAsync<int>(RoleFeatureQueryStatement, parameter);
            if (resultDeleteRoleFeature > 0)
            {
                return resultDeleteRoleFeature;
            }
            return resultDeletedRole;
        }

        public async Task<IEnumerable<RoleMaster>> GetRoles(int RoleId)
        {

               var QueryStatement = @" SELECT role.rolemasterid, 
                                                role.name,
                                                role.isactive,
                                                role.createddate,
                                                role.createdby,
                                                role.updateddate,
                                                role.updatedby,
                                                roleFeatureSet.featuresetid
                                        FROM dafconnectmaster.rolemaster role
                                        LEFT JOIN dafconnectmaster.rolefeatureset roleFeatureSet
                                        ON role.rolemasterid=roleFeatureSet.rolemasterid
                                        WHERE (role.rolemasterid=@roleid or @roleid=0)
                                        and role.isactive=true;";

            var parameter = new DynamicParameters();
            parameter.Add("@roleid", RoleId);
            IEnumerable<RoleMaster> roledetails = await dataAccess.QueryAsync<RoleMaster>(QueryStatement, parameter);
            return roledetails;

        }

        public async Task<int> UpdateRole(RoleMaster roleMaster)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@rolemasterid", roleMaster.RoleMasterId);
            parameter.Add("@rolename", roleMaster.Name);
            parameter.Add("@featuresetid", roleMaster.FeatureSetID);
            parameter.Add("@updatedby", roleMaster.modifiedby);
            parameter.Add("@updateddate", DateTime.Now);

            var RoleQueryStatement = @" UPDATE dafconnectmaster.rolemaster
                                            SET name=@rolename,
                                            updatedby=@updatedby,
                                            updateddate=@updateddate  
                                        WHERE rolemasterid = @rolemasterid
                                        RETURNING rolemasterid;";
            int resultUpdatedRole = await dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, parameter);

            if (roleMaster.FeatureSetID > 0)
            {
                var RoleFeatureQueryStatement = @" UPDATE dafconnectmaster.rolefeatureset
                                            SET featuresetid=@featuresetid,
                                            updatedby=@updatedby,
                                            updateddate=@updateddate  
                                        WHERE rolemasterid = @rolemasterid
                                        RETURNING rolemasterid;";
                int resultUpdatedRoleFeature = await dataAccess.ExecuteScalarAsync<int>(RoleFeatureQueryStatement, parameter);
                return resultUpdatedRoleFeature;
            }
            return resultUpdatedRole;
        }

        public async Task<int> CheckRoleNameExist(string roleName)
        {
            var QueryStatement = @" SELECT CASE WHEN rolemasterid IS NULL THEN 0 ELSE rolemasterid END
                                    FROM dafconnectmaster.rolemaster 
                                    WHERE isactive=true
                                    AND LOWER(name) = LOWER(@roleName)";
            var parameter = new DynamicParameters();
            parameter.Add("@roleName", roleName);
            int resultRoleId = await dataAccess.QueryFirstAsync<int>(QueryStatement, parameter);
            return resultRoleId;

        }
    }
}
