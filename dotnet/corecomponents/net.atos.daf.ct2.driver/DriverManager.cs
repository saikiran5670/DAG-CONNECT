using net.atos.daf.ct2.audit;
using System.Threading.Tasks;
using net.atos.daf.ct2.driver.entity;
//using net.atos.daf.ct2.driver.repository;
using System.Collections.Generic;

namespace net.atos.daf.ct2.driver
{
    public class DriverManager:IDriverManager
    {
        IDriverRepository driverRepository;
        IAuditTraillib auditlog;

        public DriverManager(IDriverRepository _driverRepository, IAuditTraillib _auditlog)
        {
            driverRepository = _driverRepository;
            auditlog = _auditlog;
        }
        public async Task<List<string>> ImportDrivers(List <Driver> driver)
        {
            return await driverRepository.ImportDrivers(driver);
        }
        public async Task<IEnumerable<Driver>> GetAllDrivers(int OrganizationId)
        {
            return await driverRepository.GetAllDrivers(OrganizationId);
        }
        
         public async Task<Driver> UpdateDriver(Driver driver)
          {
              return await driverRepository.UpdateDriver(driver);
          }
         public async Task<bool> DeleteDriver(int OrganizationId, int DriverId)
         {
              return await driverRepository.DeleteDriver(OrganizationId,DriverId);
         }
         public async Task<bool> UpdateOptinOptout(int organizationId, bool optoutStatus)
         {
              return await driverRepository.UpdateOptinOptout(organizationId,optoutStatus);
         }
    }
}
