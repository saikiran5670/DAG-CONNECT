using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;
// using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.role.repository;

namespace net.atos.daf.ct2.role
{
    public class RoleManagement : IRoleManagement
    {
        readonly IRoleRepository _roleRepository;
        readonly IFeatureManager _featureManager;

        public RoleManagement(IRoleRepository roleRepository, IFeatureManager featureManager)
        {
            _roleRepository = roleRepository;
            _featureManager = featureManager;
        }

        public async Task<int> CreateRole(RoleMaster roleMaster)
        {
            try
            {

                int roleId = 0;
                int featuresetid = 0;
                //to get minimum features level
                if (roleMaster.FeatureSet.Features.Count > 0)
                {
                    roleMaster.FeatureSet.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
                    featuresetid = await _featureManager.AddFeatureSet(roleMaster.FeatureSet);
                    //int minlevel = await _featureManager.GetMinimumLevel(roleMaster.FeatureSet.Features);
                    //roleMaster.Level = minlevel;
                    roleMaster.Feature_set_id = featuresetid;
                }
                roleId = await _roleRepository.CreateRole(roleMaster);


                return roleId;

            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<int> DeleteRole(int roleid, int accountid)
        {
            try
            {

                int roleId = await _roleRepository.DeleteRole(roleid, accountid);
                // auditlog.AddLogs(userId,userId,1,"Delete Role", RoleId > 0,"Role Management", "Role Deleted With Role Id " + RoleId.ToString());
                return roleId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AssignedRoles>> IsRoleAssigned(int roleid)
        {
            return await _roleRepository.IsRoleAssigned(roleid);
        }

        public async Task<IEnumerable<RoleMaster>> GetRoles(RoleFilter rolefilter)
        {
            try
            {
                //var Roles = roleRepository.GetRoles(rolefilter);
                var role = await _roleRepository.GetRoles(rolefilter);
                foreach (var item in role)
                {
                    var features = await _featureManager.GetFeatureIdsForFeatureSet(item.Feature_set_id ?? 0, rolefilter.LangaugeCode);
                    item.FeatureSet = new FeatureSet();
                    item.FeatureSet.Features = new List<Feature>();
                    foreach (var t in features)
                    {
                        item.FeatureSet.Features.Add(new Feature { Id = t.Id });
                    }

                }
                return role;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateRole(RoleMaster roleMaster)
        {
            try
            {
                int roleId = 0;
                int featuresetid = 0;
                //to get minimum features level
                if (roleMaster.FeatureSet.Features.Count > 0)
                {
                    roleMaster.FeatureSet.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
                    featuresetid = await _featureManager.AddFeatureSet(roleMaster.FeatureSet);
                    //int minlevel = await _featureManager.GetMinimumLevel(roleMaster.FeatureSet.Features);
                    //roleMaster.Level = minlevel;
                    roleMaster.Feature_set_id = featuresetid;
                }
                roleId = await _roleRepository.UpdateRole(roleMaster);
                // auditlog.AddLogs(roleMaster.Updatedby,roleMaster.modifiedby,1,"Update Role", RoleId > 0,"Role Management", "Role Updated With Role Id " + RoleId.ToString());


                return roleId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int CheckRoleNameExist(string roleName, int Organization_Id, int roleid)
        {
            return _roleRepository.CheckRoleNameExist(roleName, Organization_Id, roleid);
        }
        public async Task<IEnumerable<string>> GetCode(RoleCodeFilter roleFilter)
        {
            return await _roleRepository.GetCode(roleFilter);
        }
    }
}
