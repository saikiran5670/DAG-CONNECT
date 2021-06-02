using System;

namespace net.atos.daf.ct2.driverservice.entity
{
    public class DriverMapper
    {
        public net.atos.daf.ct2.driverservice.DriverResponse ToDriverResponse(net.atos.daf.ct2.driver.entity.DriverResponse request)
        {
            net.atos.daf.ct2.driverservice.DriverResponse driver = new net.atos.daf.ct2.driverservice.DriverResponse();
            driver.Id = request.Id;
            driver.OrganizationId = request.Organization_id;
            if (!(string.IsNullOrEmpty(request.Email)))
            {
                driver.Email = request.Email;
            }
            if (!(string.IsNullOrEmpty(request.FirstName)))
            {
                driver.FirstName = request.FirstName;
            }
            if (!(string.IsNullOrEmpty(request.LastName)))
            {
                driver.LastName = request.LastName;
            }
            if (!(string.IsNullOrEmpty(request.Status)))
            {
                driver.Status = request.Status;
            }
            if (!(string.IsNullOrEmpty(request.OptIn)))
            {
                driver.OptIn = request.OptIn;
            }
            if (!(string.IsNullOrEmpty(request.Driver_id_ext)))
            {
                driver.DriverIdExt = request.Driver_id_ext;
            }
            
            if (!(string.IsNullOrEmpty(request.CreatedAt.ToString())))
            {
                driver.CreatedAt = Convert.ToString(request.CreatedAt);
            }
            if (!(string.IsNullOrEmpty(request.ModifiedAt.ToString())))
            {
                driver.ModifiedAt = Convert.ToString(request.ModifiedAt);
            }
            if (!(string.IsNullOrEmpty(request.ModifiedBy)))
            {
                driver.ModifiedBy = Convert.ToString(request.ModifiedBy);
            }
            return driver;
        }

        public net.atos.daf.ct2.driver.entity.Driver ToDriverUpdateResponse(net.atos.daf.ct2.driverservice.DriverUpdateRequest request)
        {
            driver.entity.Driver driver = new driver.entity.Driver();
            driver.Id = request.Id;
            driver.Organization_id = request.OrganizationId;
            driver.Email = request.Email;
            driver.FirstName = request.FirstName;
            driver.LastName = request.LastName;
            driver.Status = request.Status;
            driver.OptIn = request.OptIn;
            driver.Driver_id_ext = request.DriverIdExt;
            driver.ModifiedBy = request.ModifiedBy;
            driver.Status = request.Status;
            // driver.IsActive= request.IsActive;          
            return driver;
        }

        public net.atos.daf.ct2.driverservice.DriverUpdateRequest DriverToDriverResponse(driver.entity.Driver request)
        {
            net.atos.daf.ct2.driverservice.DriverUpdateRequest driver = new DriverUpdateRequest();
            driver.Id = request.Id;
            driver.OrganizationId = request.Organization_id;
            driver.Email = request.Email;
            driver.FirstName = request.FirstName;
            driver.LastName = request.LastName;
            driver.Status = request.Status;
            driver.OptIn = request.OptIn;
            driver.ModifiedBy = request.ModifiedBy;
            driver.Status = request.Status;
            // driver.IsActive= request.IsActive;          
            return driver;
        }
        public driver.entity.Driver ToDriver(net.atos.daf.ct2.driverservice.DriversImport request)
        {
            driver.entity.Driver driver = new driver.entity.Driver();
            driver.Driver_id_ext = request.DriverIdExt;
            driver.Email = request.Email;
            driver.FirstName = request.FirstName;
            driver.LastName = request.LastName;
            return driver;
        }

        public DriverReturns ToDriverImportResponse(driver.entity.DriverImportResponse request)
        {
            DriverReturns driver = new DriverReturns();
            if (!string.IsNullOrEmpty(request.DriverID))
                driver.DriverID = request.DriverID;
            if (!string.IsNullOrEmpty(request.Email))
                driver.Email = request.Email;
            if (!string.IsNullOrEmpty(request.FirstName))
                driver.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName))
                driver.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.ReturnMessage))
                driver.ReturnMassage = request.ReturnMessage;
            if (!string.IsNullOrEmpty(request.Status))
                driver.Status = request.Status;
            return driver;
        }
    }
}
