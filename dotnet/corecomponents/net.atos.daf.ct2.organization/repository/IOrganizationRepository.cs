using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.organization.entity;

namespace net.atos.daf.ct2.organization.repository
{
    public interface IOrganizationRepository
    {
        Task<Organization> Create(Organization organization);
        Task<Organization> Update(Organization organization);
        Task<bool> Delete(int organizationId);
        Task<OrganizationResponse> Get(int organizationId);
        Task<OrganizationDetailsResponse> GetOrganizationDetails(int organizationId);

        Task<PreferenceResponse> GetPreference(int organizationId);

        //Task<Organization> UpdateCustomer(Organization organization);
        Task<CustomerRequest> UpdateCustomer(CustomerRequest customer);
        Task<HandOver> KeyHandOverEvent(HandOver keyHandOver);
        //   Task<int> CreateVehicleParty(List<Customer> customers);

        Task<int> CreateOwnerRelationship(RelationshipMapping relationshipMapping);
        Task<List<OrganizationResponse>> GetAll(int organizationId);
        Task<List<OrganizationNameandID>> Get(OrganizationByID objOrganizationByID);

        Task<int> IsOwnerRelationshipExist(int VehicleID);

        Task<IEnumerable<Organization>> GetAllOrganizations(int OrganizationID);
        Task<IEnumerable<OrganizationContextListResponse>> GetAllOrganizationsForContext();
        Task<int> GetLevelByRoleId(int orgId, int roleId);
    }
}
