using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization.repository;


namespace net.atos.daf.ct2.organization
{
    public class OrganizationManager : IOrganizationManager
    {
        readonly IOrganizationRepository _organizationRepository;
        readonly IAuditTraillib _auditlog;

        public OrganizationManager(IOrganizationRepository organizationRepository, IAuditTraillib auditlog)
        {
            _organizationRepository = organizationRepository;
            _auditlog = auditlog;
        }

        public async Task<Organization> Create(Organization organization)
        {
            return await _organizationRepository.Create(organization);
        }
        public async Task<Organization> Update(Organization organization)
        {
            return await _organizationRepository.Update(organization);
        }
        public async Task<bool> Delete(int organizationId)
        {
            return await _organizationRepository.Delete(organizationId);
        }
        public async Task<OrganizationResponse> Get(int organizationId)
        {
            return await _organizationRepository.Get(organizationId);
        }
        public async Task<OrganizationDetailsResponse> GetOrganizationDetails(int organizationId)
        {
            return await _organizationRepository.GetOrganizationDetails(organizationId);
        }
        public async Task<PreferenceResponse> GetPreference(int organizationId)
        {
            return await _organizationRepository.GetPreference(organizationId);
        }

        public async Task<CustomerRequest> UpdateCustomer(CustomerRequest customer)
        {
            return await _organizationRepository.UpdateCustomer(customer);
        }
        public async Task<HandOver> KeyHandOverEvent(HandOver keyHandOver)
        {
            return await _organizationRepository.KeyHandOverEvent(keyHandOver);
        }

        // public async Task<int> CreateVehicleParty(List<Customer> customers)
        // {
        //     return await organizationRepository.CreateVehicleParty(customers);
        // }
        public async Task<int> CreateOwnerRelationship(RelationshipMapping relationshipMapping)
        {
            return await _organizationRepository.CreateOwnerRelationship(relationshipMapping);
        }

        public async Task<List<OrganizationResponse>> GetAll(int organizationId)
        {
            return await _organizationRepository.GetAll(organizationId);
        }

        public async Task<IEnumerable<OrganizationContextListResponse>> GetAllOrganizationsForContext()
        {
            return await _organizationRepository.GetAllOrganizationsForContext();
        }

        public async Task<List<OrganizationNameandID>> Get(OrganizationByID objOrganizationByID)
        {
            return await _organizationRepository.Get(objOrganizationByID);
        }
        public async Task<int> IsOwnerRelationshipExist(int VehicleID)
        {
            return await _organizationRepository.IsOwnerRelationshipExist(VehicleID);
        }
        public async Task<IEnumerable<Organization>> GetAllOrganizations(int OrganizationID)
        {
            return await _organizationRepository.GetAllOrganizations(OrganizationID);
        }

        public async Task<int> GetLevelByRoleId(int orgId, int roleId)
        {
            return await _organizationRepository.GetLevelByRoleId(orgId, roleId);
        }

        public async Task<Organization> GetOrganizationByOrgCode(string organizationCode)
        {
            return await _organizationRepository.GetOrganizationByOrgCode(organizationCode);
        }

        #region Provisioning Data Service

        public async Task<bool> GetOrganisationList(ProvisioningOrganisationDataServiceRequest request)
        {
            return await _organizationRepository.GetOrganisationList(request);
        }

        #endregion

    }
}
