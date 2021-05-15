using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.organizationservice;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class AccountPrivilegeChecker
    {
        private readonly OrganizationService.OrganizationServiceClient _organizationClient;

        public AccountPrivilegeChecker(OrganizationService.OrganizationServiceClient organizationClient)
        {
            _organizationClient = organizationClient;
        }
        public async Task<int> GetLevelByRoleId(int orgId, int roleId)
        {
            int level = -1;
            try
            {
                LevelByRoleRequest request = new LevelByRoleRequest();
                request.OrgId = orgId;
                request.RoleId = roleId;
                LevelResponse response = await _organizationClient.GetLevelByRoleIdAsync(request);
                if (response != null && response.Code == Responcecode.Success)
                {
                    level = response.Level;
                }
                else
                {
                    level = -1;
                }
            }
            catch (Exception)
            {
                    level = -1;
            }
            return level;
        }
    }
}
