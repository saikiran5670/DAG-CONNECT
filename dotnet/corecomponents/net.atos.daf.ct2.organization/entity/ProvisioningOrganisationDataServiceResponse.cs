using System.Collections.Generic;

namespace net.atos.daf.ct2.organization.entity
{
    public class ProvisioningOrganisationDataServiceResponse
    {
        public List<ProvisioningOrganisation> Organisations { get; set; }
    }

    public class ProvisioningOrganisation
    {
        public string OrgId { get; set; }
        public string Name { get; set; }
    }
}
