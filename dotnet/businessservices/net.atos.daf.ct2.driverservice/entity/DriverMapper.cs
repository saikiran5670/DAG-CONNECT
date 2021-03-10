using System;
using System.Collections.Generic;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.driverservice;

namespace net.atos.daf.ct2.driverservice.entity
{
    public class DriverMapper
    {

        //net.atos.daf.ct2.driver.entity.DriverResponse
        public net.atos.daf.ct2.driverservice.DriverResponse ToDriverResponse(net.atos.daf.ct2.driver.entity.Driver request)
        {
            net.atos.daf.ct2.driverservice.DriverResponse driver = new  net.atos.daf.ct2.driverservice.DriverResponse();
            driver.Id = request.Id;    
            driver.OrganizationId=request.Organization_id;
            if (!(string.IsNullOrEmpty(request.Email)))
            {
                 driver.Email= request.Email;
            }
            if (!(string.IsNullOrEmpty(request.FirstName)))
            {
                 driver.FirstName= request.FirstName;
            }
            if (!(string.IsNullOrEmpty(request.LastName)))
            {
                 driver.LastName= request.LastName;
            }
            // if (!(string.IsNullOrEmpty(request.DateOfBith)))
            // {
            //      driver.DateOfBith= request.DateOfBith;
            // }
           
           // driver.DateOfBith= request.DateOfBith;
            driver.Status= request.Status;
            driver.IsActive= request.IsActive;
           // driver.opt_in= request.opt_in;
          //  driver.modified_at= request.modified_at;
          //  driver.modified_by= request.modified_by;
           // driver.created_at= request.created_at;
            return driver;
        }  

         public net.atos.daf.ct2.driver.entity.Driver ToDriverUpdateResponse(net.atos.daf.ct2.driverservice.DriverUpdateRequest request )
        {
            driver.entity.Driver driver=new driver.entity.Driver();
            driver.Id=request.Id;
            driver.Organization_id=request.OrganizationId;
            driver.Email=request.Email;
            driver.FirstName=request.FirstName;
            driver.LastName=request.LastName;
            driver.Status=request.Status;
            driver.opt_in=request.OptIn;
            //driver.Driver_id_ext=request.DriverIdExt;
            driver.modified_by=request.ModifiedBy;           
            driver.Status= request.Status;
            driver.IsActive= request.IsActive;          
            return driver;
        }  

         public net.atos.daf.ct2.driverservice.DriverUpdateRequest DriverToDriverResponse(driver.entity.Driver request )
        {
            net.atos.daf.ct2.driverservice.DriverUpdateRequest driver= new DriverUpdateRequest();           
            driver.Id=request.Id;
            driver.OrganizationId=request.Organization_id;
            driver.Email=request.Email;
            driver.FirstName=request.FirstName;
            driver.LastName=request.LastName;
            driver.Status=request.Status;
            driver.OptIn=request.opt_in;
            driver.ModifiedBy=request.modified_by;           
            driver.Status= request.Status;
            driver.IsActive= request.IsActive;          
            return driver;
        }  
        public driver.entity.Driver ToDriver(net.atos.daf.ct2.driverservice.DriversImport request )
        {
            driver.entity.Driver driver= new  driver.entity.Driver();      
            driver.Driver_id_ext=request.DriverIdExt;
            driver.Email=request.Email;
            driver.FirstName=request.FirstName;
            driver.LastName=request.LastName;                   
            return driver;
        }  

        public DriverReturns ToDriverImportResponse(driver.entity.DriverImportResponse request )
        {
            DriverReturns driver= new  DriverReturns();      
            driver.DriverID=request.DriverID;
            driver.Email=request.Email;
            driver.FirstName=request.FirstName;
            driver.LastName=request.LastName; 
            driver.ReturnMassage=request.ReturnMessage;                    
            return driver;
        }  
    }
}
