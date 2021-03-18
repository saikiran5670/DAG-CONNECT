using System.Threading.Tasks;
using net.atos.daf.ct2.organization.entity;
using System.Collections.Generic;

namespace net.atos.daf.ct2.organization
{
    public interface IOrganizationManager
    {
        Task<Organization> Create(Organization organization);
        Task<Organization> Update(Organization group);
        Task<bool> Delete(int organizationId);
        Task<OrganizationResponse> Get(int organizationId);
        Task<PreferenceResponse> GetPreference(int organizationId);
        Task<CustomerRequest> UpdateCustomer(CustomerRequest customer);
        Task<HandOver> KeyHandOverEvent(HandOver keyHandOver);
       // Task<int> CreateVehicleParty(List<Customer> customers);   
        Task<int> CreateOwnerRelationship(RelationshipMapping relationshipMapping);  
    }
}
