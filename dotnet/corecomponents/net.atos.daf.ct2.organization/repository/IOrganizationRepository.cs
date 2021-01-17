using System.Threading.Tasks;
using net.atos.daf.ct2.organization.entity;

namespace net.atos.daf.ct2.organization.repository
{
    public interface IOrganizationRepository
    {
       Task<Organization> Create(Organization organization);
        Task<Organization> Update(Organization organization);
        Task<bool> Delete(int organizationId);        
        Task<Organization> Get(int organizationId);
      //  Task<Organization> UpdateCustomer(Organization organization);
        Task<Customer> UpdateCustomer(Customer customer);
        Task<KeyHandOver> KeyHandOverEvent(KeyHandOver keyHandOver);

    }
}
