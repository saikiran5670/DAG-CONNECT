using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.role;
using net.atos.daf.ct2.role.repository;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;

namespace net.atos.daf.ct2.role
{
    public class RoleManagement: IRoleManagement
    {
        IRoleRepository roleRepository;
        
        IFeatureRepository featureRepository;
        IFeatureManager FeatureManager;
        
        // IAuditLog auditlog;
        public RoleManagement(IRoleRepository _roleRepository,IFeatureManager _FeatureManager,IFeatureRepository _featureRepository)
        {
            roleRepository = _roleRepository;
            featureRepository=_featureRepository;
            FeatureManager=_FeatureManager;
            
            // auditlog=_auditlog;
        }
        public async Task<int> CreateRole(RoleMaster roleMaster)
        {
            try
            {

                   roleMaster.FeatureSet.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
                   int RoleId = 0;
                   int featuresetid = await FeatureManager.AddFeatureSet(roleMaster.FeatureSet);
                  //to get minimum features level
                  int minlevel = await FeatureManager.GetMinimumLevel(roleMaster.FeatureSet.Features);
                    roleMaster.Level = minlevel;
                   if (featuresetid > 0 )
                   {
                        roleMaster.Feature_set_id = featuresetid;
                         RoleId= await roleRepository.CreateRole(roleMaster);
                   }             
                              
                return RoleId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<int> DeleteRole(int roleid, int Accountid)
        {
            try
            {
                if (await roleRepository.IsRoleAssigned(roleid))
                {
                    return -1;
                }
                int RoleId= await roleRepository.DeleteRole(roleid, Accountid);
                // auditlog.AddLogs(userId,userId,1,"Delete Role", RoleId > 0,"Role Management", "Role Deleted With Role Id " + RoleId.ToString());
                return RoleId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<RoleMaster>> GetRoles(RoleFilter rolefilter)
        {
            try
            {
                //var Roles = roleRepository.GetRoles(rolefilter);
                var role = await roleRepository.GetRoles(rolefilter);
                foreach(var item in role)
                {
                    var features = await FeatureManager.GetFeatureIdsForFeatureSet(item.Feature_set_id ?? 0, rolefilter.LangaugeCode);
                    item.FeatureSet   = new FeatureSet();
                    item.FeatureSet.Features = new List<Feature>();
                    foreach(var t in features)
                    {
                            item.FeatureSet.Features.Add(new Feature{ Id = t.Id});
                    }
                    
                }
                return role;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateRole(RoleMaster roleMaster)
        {
            try
            {
                   roleMaster.FeatureSet.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
                   int RoleId = 0;
                   int featuresetid = await FeatureManager.AddFeatureSet(roleMaster.FeatureSet);
                //to get minimum features level
                int minlevel = await FeatureManager.GetMinimumLevel(roleMaster.FeatureSet.Features);
                roleMaster.Level = minlevel;
                if (featuresetid > 0 )
                   {
                       roleMaster.Feature_set_id = featuresetid;
                        RoleId= await roleRepository.UpdateRole(roleMaster);
               // auditlog.AddLogs(roleMaster.Updatedby,roleMaster.modifiedby,1,"Update Role", RoleId > 0,"Role Management", "Role Updated With Role Id " + RoleId.ToString());
                   }
               
                return RoleId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int  CheckRoleNameExist(string roleName,int Organization_Id,int roleid)
        {
            return  roleRepository.CheckRoleNameExist( roleName, Organization_Id,roleid);
        }
    }
}
