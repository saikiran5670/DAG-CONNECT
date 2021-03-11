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
            foreach (var item in drivers)
            {
                net.atos.daf.ct2.driverservice.DriversImport objdriverr=new driverservice.DriversImport();
                objdriverr.DriverIdExt=item.DriverID;
                objdriverr.FirstName=item.FirstName;
                objdriverr.LastName=item.LastName;
                objdriverr.Email=item.Email;
                objReturn.Drivers.Add(objdriverr);
            }        

            return objReturn;
        }
    }
}
