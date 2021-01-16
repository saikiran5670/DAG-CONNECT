using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
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
                int RoleId= await roleRepository.CreateRole(roleMaster);
               // auditlog.AddLogs(roleMaster.Createdby,roleMaster.Createdby,1,"Add Role",RoleId > 0,"Role Management", "Role Added With Role Id " + RoleId.ToString());
               if(RoleId > 0)
               {
                   roleMaster.FeatureSet.Name = "FeatureSet_" + RoleId;
                   int featuresetid = await FeatureManager.AddFeatureSet(roleMaster.FeatureSet);
                //    int featuresetid = 4;
                    if (featuresetid > 0)
                    {
                        await roleRepository.Addrolefeatureset(RoleId,featuresetid);
                    }
               }

               
                return RoleId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CheckRoleNameExist(string roleName)
        {
            try
            {
                return await roleRepository.CheckRoleNameExist(roleName);
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
                return await roleRepository.GetRoles(rolefilter);
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
                int RoleId= await roleRepository.UpdateRole(roleMaster);
               // auditlog.AddLogs(roleMaster.Updatedby,roleMaster.modifiedby,1,"Update Role", RoleId > 0,"Role Management", "Role Updated With Role Id " + RoleId.ToString());
                return RoleId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
