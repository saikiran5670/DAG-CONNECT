using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.driver.entity;

namespace net.atos.daf.ct2.driver
{
    public interface IDriverRepository
    {
        Task<List<DriverImportResponse>> ImportDrivers(List <Driver> driver, int orgid);
        Task<IEnumerable<DriverResponse>> GetDriver(int organizationId, int driverId);
        Task<Driver> UpdateDriver(Driver driver);
        Task<bool> DeleteDriver(int OrganizationId, int DriverId);
        Task<bool> UpdateOptinOptout(int organizationId, string optoutStatus);

    }
}
