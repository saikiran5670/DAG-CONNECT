using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.data;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.role.repository
{
    public class RoleRepository : IRoleRepository
    {

        private readonly IDataAccess dataAccess;

        public RoleRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
        public async Task<int> CreateRole(RoleMaster roleMaster)
        {

            var RoleQueryStatement = @" INSERT INTO master.role
                                    (organization_id,name,is_active,created_date,created_by) 
	                                VALUES (@organization_id,@name,@is_active,@created_date,@created_by)
	                                RETURNING id";

            var Roleparameter = new DynamicParameters();
            Roleparameter.Add("@organization_id", roleMaster.Organization_Id);
            Roleparameter.Add("@name", roleMaster.Name);
            Roleparameter.Add("@is_active", true);
            Roleparameter.Add("@created_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
            Roleparameter.Add("@created_by", roleMaster.createdby);

            int InsertedRoleId = await dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, Roleparameter);
            // if (roleMaster.FeatureSetID > 0)
            // {
            //     var RoleFeatureQueryStatement = @" INSERT INTO dafconnectmaster.rolefeatureset
            //                         (rolemasterid,featuresetid,isactive,createddate,createdby) 
	        //                         VALUES (@rolemasterid,@featuresetid,@isactive,@createddate,@createdby)
	        //                         RETURNING rolefeaturesetid";

            //     var RoleFeatureparameter = new DynamicParameters();
            //     RoleFeatureparameter.Add("@rolemasterid", InsertedRoleId);
            //     RoleFeatureparameter.Add("@featuresetid", roleMaster.FeatureSetID);
            //     RoleFeatureparameter.Add("@isactive", true);
            //     RoleFeatureparameter.Add("@createddate", DateTime.Now);
            //     RoleFeatureparameter.Add("@createdby", roleMaster.createdby);

            //     int resultAddRoleFeature = await dataAccess.ExecuteScalarAsync<int>(RoleFeatureQueryStatement, RoleFeatureparameter);
            //     return resultAddRoleFeature;
            // }
            return InsertedRoleId;
        }

        public async Task<int> DeleteRole(int roleid, int Accountid)
        {

            var parameter = new DynamicParameters();
            parameter.Add("@roleid", roleid);
            parameter.Add("@is_active", false);
            parameter.Add("@updated_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
            parameter.Add("@updated_by", Accountid);

            var RoleQueryStatement = @"UPDATE master.role
                                    SET is_active = @is_active
                                    ,updated_date = @updated_date
                                    ,updated_by = @updated_by
                                    WHERE id = @roleid
                                    RETURNING id;";

            int resultDeletedRole = await dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, parameter);

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

            var QueryStatement= @"SELECT id, 
                                organization_id, 
                                name, 
                                is_active, 
                                created_date, 
                                created_by, 
                                updated_date,
                                updated_by
	                            FROM master.role
                                WHERE where 1=1";


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

            }

            // VIN Id Filter
            if (roleFilter.Is_Active)
            {
                parameter.Add("@vin", roleFilter.Is_Active);
                QueryStatement = QueryStatement + " and vin LIKE @vin";

            }                    
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
