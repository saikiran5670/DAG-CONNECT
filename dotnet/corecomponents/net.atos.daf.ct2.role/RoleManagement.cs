using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.role;
using net.atos.daf.ct2.role.repository;

namespace net.atos.daf.ct2.rolerepository
{
    public class RoleManagement: IRoleManagement
    {
        IRoleRepository roleRepository;
        IAuditLog auditlog;
        public RoleManagement(IRoleRepository _roleRepository,IAuditLog _auditlog)
        {
            roleRepository = _roleRepository;
            auditlog=_auditlog;
        }
        public async Task<int> CreateRole(RoleMaster roleMaster)
        {
            try
            {
                int RoleId= await roleRepository.CreateRole(roleMaster);
                auditlog.AddLogs(roleMaster.createdby,roleMaster.createdby,1,"Add Role",RoleId > 0,"Role Management", "Role Added With Role Id " + RoleId.ToString());
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

        public async Task<IEnumerable<RoleMaster>> GetRoles(int roleId)
        {
            try
            {
                return await roleRepository.GetRoles(roleId);
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
                auditlog.AddLogs(roleMaster.modifiedby,roleMaster.modifiedby,1,"Update Role", RoleId > 0,"Role Management", "Role Updated With Role Id " + RoleId.ToString());
                return RoleId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
