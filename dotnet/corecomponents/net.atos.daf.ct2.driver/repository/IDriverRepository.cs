using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.driver.entity;

namespace net.atos.daf.ct2.driver
{
    public interface IDriverRepository
    {
      //   Task<IEnumerable<DriverMaster>> GetDriverList (int DriverId);
      //  // Task<DriverTemplate> DownloadDriverTemplate(string languageCode);
      //   Task<int> UploadDriverTemplate();
      //   Task<int> DeleteDriverDetails(List<DriverMaster> Drivers);
      // //  Task<string> ShowConsentForm(string languageCode,int OrganizationId);
      //   Task<List<string>> InertUpdateDriverDetails(List<DriverMaster> driverdetails);
        Task<List<DriverImportResponse>> ImportDrivers(List <Driver> driver,int orgid);
        Task<IEnumerable<DriverResponse>> GetDriver(int OrganizationId,int DriverID);
        Task<Driver> UpdateDriver(Driver driver);
        Task<bool> DeleteDriver(int OrganizationId, int DriverId);
        Task<bool> UpdateOptinOptout(int organizationId, string optoutStatus);

    }
}
