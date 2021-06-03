using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace net.atos.daf.ct2.portalservice.Entity.Driver
{
    public class DriverMapper
    {
        public net.atos.daf.ct2.driverservice.DriverImportRequest ToDriverImport(List<DriverRequest> drivers,
                                                                    out List<net.atos.daf.ct2.driverservice.DriverReturns> diverReturns)
        {
            diverReturns = new List<driverservice.DriverReturns>();
            net.atos.daf.ct2.driverservice.DriverImportRequest objReturn = new net.atos.daf.ct2.driverservice.DriverImportRequest();
            var driverIdValidation = new Regex(@"^[A-Z]{1,1}[A-Z\s]{1,1}[\s]{1,1}[A-Z0-9]{16,16}$");
            foreach (DriverRequest item in drivers)
            {
                int OrganizationId = item.OrganizationId;
                net.atos.daf.ct2.driverservice.DriversImport objdriver;
                foreach (Driver driver in item.Drivers)
                {
                    objdriver = new driverservice.DriversImport();
                    if (!driverIdValidation.IsMatch(driver.DriverID))
                    {
                        diverReturns.Add(
                            new driverservice.DriverReturns()
                            {
                                ReturnMassage = "Driver Id is invalid.",
                                Status = "FAIL",
                                DriverID = driver.DriverID,
                                FirstName = driver.FirstName,
                                LastName = driver.LastName,
                                Email = driver.Email
                            }
                            );
                        continue;
                    }

                    objdriver.DriverIdExt = driver.DriverID;
                    objdriver.FirstName = driver.FirstName;
                    objdriver.LastName = driver.LastName;
                    objdriver.Email = driver.Email;

                    objReturn.Drivers.Add(objdriver);
                }
                objReturn.OrgID = OrganizationId;
            }

            return objReturn;
        }
        //  public  List<List<DriverRequest>> ToDriverImportList(List<DriverRequest> lstDrivers) 
        //     {          
        //         List<List<DriverRequest>> objReturn=new  List<List<DriverRequest>>();    
        //         List<net.atos.daf.ct2.driverservice.DriversImport> ValidDriversList=new List<net.atos.daf.ct2.driverservice.DriversImport>();
        //         List<net.atos.daf.ct2.driverservice.DriversImport> NotValidDriversList=new List<net.atos.daf.ct2.driverservice.DriversImport>();

        //         foreach (DriverRequest item in lstDrivers)
        //         { 
        //            int OrganizationId=item.OrganizationId;

        //            DriverValidate objDriverValidate=new DriverValidate();
        //            string isDriveIdValid=string.Empty;
        //            foreach (Driver driver in item.Drivers)
        //            {     
        //               net.atos.daf.ct2.driverservice.DriversImport validDriver;
        //               net.atos.daf.ct2.driverservice.DriversImport notValidDriver;

        //             isDriveIdValid=CheckDriveIdId(driver.Email);

        //             if(isDriveIdValid!="")
        //             {
        //                  objdriver.Email=driver.Email;
        //                  objDriverValidate.Message=isDriveIdValid; 
        //                  NotValidDriversList.Add(objdriver);                    
        //             }
        //             else{

        //             }

        //             objdriver.DriverIdExt=driver.DriverID;
        //             objdriver.FirstName=driver.FirstName;
        //             objdriver.LastName=driver.LastName;
        //             objdriver.Email=driver.Email;

        //             // objReturn.Add(ValidDriversList);
        //             // objReturn.Add(NotValidDriversList);                
        //            }               

        //           // objReturn.OrgID=OrganizationId;     
        //         }        

        //         return objReturn;
        //     }

        //     public bool isValidEmail(string email)
        //     {
        //         return true;
        //     }

        //     public string CheckDriveIdId(string driverid)
        //     {
        //        string returnMessage=string.Empty; 
        //        if(driverid == "" || driverid.Length == 0)
        //        { 
        //          return returnMessage = "Required driverID field";   
        //        }

        //       if(driverid.Length> 19)
        //       {
        //        return returnMessage = "DriverID length can not be (>19)";                
        //       }   
        //       return returnMessage;
        // }
    }

}
