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
            driver.Status = request.Status;
            driver.OptIn = request.opt_in;
            driver.DriverIdExt = request.Driver_id_ext;
            if (!(string.IsNullOrEmpty(request.created_at.ToString())))
            {
                driver.CreatedAt = Convert.ToString(request.created_at);
            }
            if (!(string.IsNullOrEmpty(request.modified_at.ToString())))
            {
                driver.ModifiedAt = Convert.ToString(request.modified_at);
            }
            if (!(string.IsNullOrEmpty(request.modified_by)))
            {
                driver.ModifiedBy = Convert.ToString(request.modified_by);
            }
            return driver;
        }

        public net.atos.daf.ct2.driver.entity.Driver ToDriverUpdateResponse(net.atos.daf.ct2.driverservice.DriverUpdateRequest request)
        {
            driver.entity.Driver driver = new driver.entity.Driver();
            driver.Id = request.Id;
            driver.Organization_id = request.OrganizationId;
            driver.email = request.Email;
            driver.first_name = request.FirstName;
            driver.last_name = request.LastName;
            driver.Status = request.Status;
            driver.opt_in = request.OptIn;
            driver.Driver_id_ext = request.DriverIdExt;
            driver.modified_by = request.ModifiedBy;
            driver.Status = request.Status;
            // driver.IsActive= request.IsActive;          
            return driver;
        }

        public net.atos.daf.ct2.driverservice.DriverUpdateRequest DriverToDriverResponse(driver.entity.Driver request)
        {
            net.atos.daf.ct2.driverservice.DriverUpdateRequest driver = new DriverUpdateRequest();
            driver.Id = request.Id;
            driver.OrganizationId = request.Organization_id;
            driver.Email = request.email;
            driver.FirstName = request.first_name;
            driver.LastName = request.last_name;
            driver.Status = request.Status;
            driver.OptIn = request.opt_in;
            driver.ModifiedBy = request.modified_by;
            driver.Status = request.Status;
            // driver.IsActive= request.IsActive;          
            return driver;
        }
        public driver.entity.Driver ToDriver(net.atos.daf.ct2.driverservice.DriversImport request)
        {
            driver.entity.Driver driver = new driver.entity.Driver();
            driver.Driver_id_ext = request.DriverIdExt;
            driver.email = request.Email;
            driver.first_name = request.FirstName;
            driver.last_name = request.LastName;
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
