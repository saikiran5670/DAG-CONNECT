using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.driver.entity;

namespace net.atos.daf.ct2.driver
{
    public interface IDriverManager
    {
        //      Task<IEnumerable<Driver>> GetDriverList (int DriverId);
        //    // Task<DriverTemplate> DownloadDriverTemplate(string languageCode);
        //     Task<int> UploadDriverTemplate();
        //     Task<int> DeleteDriverDetails(List<Driver> Drivers);
        //   //  Task<string> ShowConsentForm(string languageCode,int OrganizationId);
        //     Task<List<string>> InertUpdateDriverDetails(List<Driver> driverdetails);
        Task<List<DriverImportResponse>> ImportDrivers(List<Driver> driver, int orgid);
        //Task<IEnumerable<Driver>> GetAllDrivers(int OrganizationId);
        Task<Driver> UpdateDriver(Driver driver);
        Task<bool> DeleteDriver(int organizationId, int driverId);
        Task<bool> UpdateOptinOptout(int organizationId, string optoutStatus);
        Task<IEnumerable<DriverResponse>> GetDriver(int organizationId, int driverID);
        Task<DriverLookupResponse> GetDriver(string driverId, string email);

        #region Provisioning Data Service

        Task<ProvisioningDriverDataServiceResponse> GetCurrentDriver(ProvisioningDriverDataServiceRequest request);
        Task<ProvisioningDriverDataServiceResponse> GetDriverList(ProvisioningDriverDataServiceRequest request);
        Task<bool> CheckIfDriverExists(string driverId, string organisationId, string email);

        #endregion
    }
}
