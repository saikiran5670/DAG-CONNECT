//using net.atos.daf.ct2.driver.repository;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task<IEnumerable<DriverResponse>> GetDriver(int organizationId, int driverID)
        {
            return await _driverRepository.GetDriver(organizationId, driverID);
        }

        public async Task<DriverLookup> GetDriver(int organizationId, string driverID)
        {
            return await _driverRepository.GetDriver(organizationId, driverID);
        }

        public async Task<DriverLookupResponse> GetDriver(string driverId, string email)
        {
            return await _driverRepository.GetDriver(driverId, email);
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

        #region Provisioning Data Service

        public async Task<ProvisioningDriverDataServiceResponse> GetCurrentDriver(ProvisioningDriverDataServiceRequest request)
        {
            var provisioningDriver = await _driverRepository.GetCurrentDriver(request);
            var drivers = new List<ProvisioningDriver>();
            if (provisioningDriver != null)
                drivers.Add(provisioningDriver);

            return new ProvisioningDriverDataServiceResponse { Drivers = drivers };
        }

        public async Task<ProvisioningDriverDataServiceResponse> GetDriverList(ProvisioningDriverDataServiceRequest request)
        {
            var provisioningDrivers = await _driverRepository.GetDriverList(request);
            return new ProvisioningDriverDataServiceResponse { Drivers = provisioningDrivers.ToList() };
        }

        #endregion

        public async Task<bool> CheckIfDriverExists(string driverId, int? organisationId, string email)
        {
            return await _driverRepository.CheckIfDriverExists(driverId, organisationId, email);
        }
    }
}
