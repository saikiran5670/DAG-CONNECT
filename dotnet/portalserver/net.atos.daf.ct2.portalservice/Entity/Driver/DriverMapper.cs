using System;
using System.Collections.Generic;
using net.atos.daf.ct2.portalservice.Entity.Driver;

namespace net.atos.daf.ct2.portalservice.Entity.Driver
{
    public class DriverMapper
    {
        public  net.atos.daf.ct2.driverservice.DriverImportRequest ToDriverImport(List<DriverRequest> drivers) 
        {          
            net.atos.daf.ct2.driverservice.DriverImportRequest objReturn= new net.atos.daf.ct2.driverservice.DriverImportRequest();          
            foreach (DriverRequest item in drivers)
            { 
               int OrganizationId=item.OrganizationId;
               net.atos.daf.ct2.driverservice.DriversImport objdriver;
               foreach (Driver driver in item.Drivers)
               {              
                objdriver=new driverservice.DriversImport();
                objdriver.DriverIdExt=driver.DriverID;
                objdriver.FirstName=driver.FirstName;
                objdriver.LastName=driver.LastName;
                objdriver.Email=driver.Email;
                objReturn.Drivers.Add(objdriver);
               }               
               objReturn.OrgID=OrganizationId;     
            }        
            
            return objReturn;
        }
    }
}
