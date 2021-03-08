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
                                    (organization_id,name,is_active,created_at,created_by,description,feature_set_id,level) 
	                                VALUES (@organization_id,@name,@is_active,@created_at,@created_by,@description,@feature_set_id,@level)
	                                RETURNING id";

            var Roleparameter = new DynamicParameters();
            Roleparameter.Add("@organization_id", roleMaster.Organization_Id == 0 ?null : roleMaster.Organization_Id);
            Roleparameter.Add("@name", roleMaster.Name);
            Roleparameter.Add("@is_active", true);
            Roleparameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
            Roleparameter.Add("@created_by", roleMaster.Createdby);
            Roleparameter.Add("@description", roleMaster.Description);
            Roleparameter.Add("@feature_set_id", roleMaster.Feature_set_id);
            Roleparameter.Add("@level", roleMaster.Level);

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

        public async Task<int>  Updaterolefeatureset(int RoleId,int FeatureSetId)
        {
             var RoleQueryStatement = @"UPDATE master.role
                                    SET feature_set_id = @feature_set_id
                                    ,updated_date = @modified_date
                                    ,updated_by = @modified_by
                                    WHERE id = @role_id
                                    RETURNING id;";


            var Roleparameter = new DynamicParameters();
            Roleparameter.Add("@feature_set_id", FeatureSetId);
            Roleparameter.Add("@role_id", RoleId);
           

            int InsertedRoleId = await dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, Roleparameter);
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
                                    ,modified_at = @modified_date
                                    ,modified_by = @modified_by
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

            var QueryStatement= @"SELECT role.id, 
                                role.organization_id, 
                                role.name, 
                                role.is_active, 
                                role.description,
                                role.created_date, 
                                role.created_by, 
                                role.updated_date,
                                role.updated_by,
                                role.feature_set_id
	                            FROM master.role role
								WHERE  is_active = true";


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

                if(roleFilter.IsGlobal == true)
                {
                    parameter.Add("@id", roleFilter.RoleId);
                    QueryStatement = QueryStatement + " or (organization_id  is null and is_active =true)";

                }

            }
            else if(roleFilter.Organization_Id == 0)
            {
                 if(roleFilter.IsGlobal == true)
                {
                    QueryStatement = QueryStatement + " and  organization_id  is null";

                }

            }

                   
            IEnumerable<RoleMaster> roledetails = await dataAccess.QueryAsync<RoleMaster>(QueryStatement, parameter);
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

            var RoleQueryStatement = @" UPDATE master.role
                                            SET name=@name,
                                            description= @Description,
                                             feature_set_id = @feature_set_id,
                                            modified_by=@updatedby,
                                           modified_at=@updateddate  
                                        WHERE id = @id
                                        RETURNING id;";
            int resultUpdatedRole = await dataAccess.ExecuteScalarAsync<int>(RoleQueryStatement, parameter);

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

        public int CheckRoleNameExist(string roleName,int Organization_Id,int roleid)
        {
            var QueryStatement = @" SELECT CASE WHEN id IS NULL THEN 0 ELSE id END
                                    FROM master.role 
                                    WHERE is_active=true
                                    AND LOWER(name) = LOWER(@roleName) and (organization_id = @organization_id or organization_id is null)";
           var parameter = new DynamicParameters();
            if(roleid>0)
            {
                parameter.Add("@roleid", roleid);
                QueryStatement = QueryStatement + " and id != @roleid";
            }
            
            parameter.Add("@roleName", roleName.Trim());
            parameter.Add("@organization_id", Organization_Id);
            int resultRoleId =  dataAccess.ExecuteScalar<int>(QueryStatement, parameter);
            return resultRoleId;

        }
    }
}
