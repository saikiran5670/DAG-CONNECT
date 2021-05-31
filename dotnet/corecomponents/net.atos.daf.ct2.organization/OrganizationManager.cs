using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization.repository;


namespace net.atos.daf.ct2.organization
{
    public class OrganizationManager : IOrganizationManager
    {
        IOrganizationRepository organizationRepository;
        IAuditTraillib auditlog;

        public OrganizationManager(IOrganizationRepository _organizationRepository, IAuditTraillib _auditlog)
        {
            organizationRepository = _organizationRepository;
            auditlog = _auditlog;
        }

        public async Task<Organization> Create(Organization organization)
        {
            return await organizationRepository.Create(organization);
        }
        public async Task<Organization> Update(Organization organization)
        {
            return await organizationRepository.Update(organization);
        }
        public async Task<bool> Delete(int organizationId)
        {
            return await organizationRepository.Delete(organizationId);
        }
        public async Task<OrganizationResponse> Get(int organizationId)
        {
            return await organizationRepository.Get(organizationId);
        }
        public async Task<OrganizationDetailsResponse> GetOrganizationDetails(int organizationId)
        {
            return await organizationRepository.GetOrganizationDetails(organizationId);
        }
        public async Task<PreferenceResponse> GetPreference(int organizationId)
        {
            return await organizationRepository.GetPreference(organizationId);
        }

        public async Task<CustomerRequest> UpdateCustomer(CustomerRequest customer)
        {
            return await organizationRepository.UpdateCustomer(customer);
        }
        public async Task<HandOver> KeyHandOverEvent(HandOver keyHandOver)
        {
            return await organizationRepository.KeyHandOverEvent(keyHandOver);
        }

        // public async Task<int> CreateVehicleParty(List<Customer> customers)
        // {
        //     return await organizationRepository.CreateVehicleParty(customers);
        // }
        public async Task<int> CreateOwnerRelationship(RelationshipMapping relationshipMapping)
        {
            return await organizationRepository.CreateOwnerRelationship(relationshipMapping);
        }

        public async Task<List<OrganizationResponse>> GetAll(int organizationId)
        {
            return await organizationRepository.GetAll(organizationId);
        }

        public async Task<List<OrganizationNameandID>> Get(OrganizationByID objOrganizationByID)
        {
            return await organizationRepository.Get(objOrganizationByID);
        }
        public async Task<int> IsOwnerRelationshipExist(int VehicleID)
        {
            return await organizationRepository.IsOwnerRelationshipExist(VehicleID);
        }
        public async Task<IEnumerable<Organization>> GetAllOrganizations(int OrganizationID)
        {
            return await organizationRepository.GetAllOrganizations(OrganizationID);
        }

        public async Task<int> GetLevelByRoleId(int orgId, int roleId)
        {
            return await organizationRepository.GetLevelByRoleId(orgId, roleId);
        }
    }
}
