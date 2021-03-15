using net.atos.daf.ct2.audit;
using System.Threading.Tasks;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization.repository;
using System.Collections.Generic;


namespace net.atos.daf.ct2.organization
{
    public class OrganizationManager:IOrganizationManager
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
         public async Task<PreferenceResponse> GetPreference(int organizationId)
        {
            return await organizationRepository.GetPreference(organizationId);
        }

       public async Task<Customer> UpdateCustomer(Customer customer)
        {
            return await organizationRepository.UpdateCustomer(customer);
        }
        public async Task<KeyHandOver> KeyHandOverEvent(KeyHandOver keyHandOver)
        {
            return await organizationRepository.KeyHandOverEvent(keyHandOver);
        }

        public async Task<int> CreateVehicleParty(List<Customer> customers)
        {
            return await organizationRepository.CreateVehicleParty(customers);
        }

       
    }
}
