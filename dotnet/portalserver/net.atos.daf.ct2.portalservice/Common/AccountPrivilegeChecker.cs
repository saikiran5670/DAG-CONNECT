using System;
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
            try
            {
                LevelByRoleRequest request = new LevelByRoleRequest
                {
                    OrgId = orgId,
                    RoleId = roleId
                };
                LevelResponse response = await _organizationClient.GetLevelByRoleIdAsync(request);
                if (response != null && response.Code == Responcecode.Success)
                {
                    return response.Level;
                }
            }
            catch (Exception)
            {                
            }
            return -1;
        }
    }
}
