//using net.atos.daf.ct2.driver.repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.driver.entity;

namespace net.atos.daf.ct2.driver
{
    public class DriverManager : IDriverManager
    {
        readonly IDriverRepository _driverRepository;
        readonly IAuditTraillib _auditlog;
        public DriverManager(IDriverRepository driverRepository, IAuditTraillib auditlog)
        {
            _driverRepository = driverRepository;
            _auditlog = auditlog;
        }
        public async Task<List<DriverImportResponse>> ImportDrivers(List<Driver> driver, int orgid)
        {
            return await _driverRepository.ImportDrivers(driver, orgid);
        }
        public async Task<IEnumerable<DriverResponse>> GetDriver(int OrganizationId, int DriverID)
        {
            return await _driverRepository.GetDriver(OrganizationId, DriverID);
        }

        public async Task<Driver> UpdateDriver(Driver driver)
        {
            return await _driverRepository.UpdateDriver(driver);
        }
        public async Task<bool> DeleteDriver(int OrganizationId, int DriverId)
        {
            return await _driverRepository.DeleteDriver(OrganizationId, DriverId);
        }
        public async Task<bool> UpdateOptinOptout(int organizationId, string optoutStatus)
        {
            return await _driverRepository.UpdateOptinOptout(organizationId, optoutStatus);
        }
    }
}
