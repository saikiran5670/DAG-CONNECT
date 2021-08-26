using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.driver.entity;

namespace net.atos.daf.ct2.driver
{
    public interface IDriverRepository
    {
        Task<List<DriverImportResponse>> ImportDrivers(List<Driver> driver, int orgid);
        Task<IEnumerable<DriverResponse>> GetDriver(int organizationId, int driverId);
        Task<DriverLookupResponse> GetDriver(string driverId, string email);
        Task<Driver> UpdateDriver(Driver driver);
        Task<bool> DeleteDriver(int organizationId, int driverId);
        Task<bool> UpdateOptinOptout(int organizationId, string optoutStatus);
        Task<ProvisioningDriver> GetCurrentDriver(ProvisioningDriverDataServiceRequest request);
        Task<IEnumerable<ProvisioningDriver>> GetDriverList(ProvisioningDriverDataServiceRequest request);
        Task<bool> CheckIfDriverExists(string driverId, string organisationId, string email);
    }
}
