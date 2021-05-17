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
        public async Task<List<DriverImportResponse>> ImportDrivers(List <Driver> driver,int orgid)
        {
            return await driverRepository.ImportDrivers(driver,orgid);
        }
        public async Task<IEnumerable<DriverResponse>> GetDriver(int OrganizationId, int DriverID)
        {
            return await driverRepository.GetDriver(OrganizationId,DriverID);
        }

        
         public async Task<Driver> UpdateDriver(Driver driver)
          {
              return await driverRepository.UpdateDriver(driver);
          }
         public async Task<bool> DeleteDriver(int OrganizationId, int DriverId)
         {
              return await driverRepository.DeleteDriver(OrganizationId,DriverId);
         }
         public async Task<bool> UpdateOptinOptout(int organizationId, string optoutStatus)
         {
              return await driverRepository.UpdateOptinOptout(organizationId,optoutStatus);
         }        
    }
}
