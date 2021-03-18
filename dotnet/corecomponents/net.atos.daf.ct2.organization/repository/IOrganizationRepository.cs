using System.Threading.Tasks;
using net.atos.daf.ct2.organization.entity;
using System.Collections.Generic;

namespace net.atos.daf.ct2.organization.repository
{
    public interface IOrganizationRepository
    {
       Task<Organization> Create(Organization organization);
        Task<Organization> Update(Organization organization);
        Task<bool> Delete(int organizationId);        
        Task<OrganizationResponse> Get(int organizationId);
        Task<PreferenceResponse> GetPreference(int organizationId);

        //Task<Organization> UpdateCustomer(Organization organization);
        Task<Customer> UpdateCustomer(Customer customer);
        Task<HandOver> KeyHandOverEvent(HandOver keyHandOver);
        Task<int> CreateVehicleParty(List<Customer> customers);
         
        Task<int> CreateOwnerRelationship(RelationshipMapping relationshipMapping);
       
    }
}
