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
<<<<<<< HEAD
      //  Task<Organization> UpdateCustomer(Organization organization);
        Task<Customer> UpdateCustomer(Customer customer);
        Task<KeyHandOver> KeyHandOverEvent(KeyHandOver keyHandOver);

=======
        Task<int> CreateVehicleParty(List<Organization> organization);
>>>>>>> 9b7b71e724160c3712e493c72530b1eb3e50f0ce
    }
}
