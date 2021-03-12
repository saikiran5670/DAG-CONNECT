using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DriverBusinessService = net.atos.daf.ct2.driverservice;
using net.atos.daf.ct2.portalservice.Entity.Driver;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("driver")]
    public class DriverController : ControllerBase
    {
       private readonly ILogger<DriverController> logger;
         private readonly DriverMapper mapper;
            
        private readonly DriverBusinessService.DriverService.DriverServiceClient driverClient;
        private string FK_Constraint = "violates foreign key constraint";
       // private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        
        public DriverController(ILogger<DriverController> _logger, DriverBusinessService.DriverService.DriverServiceClient _driverClient)
        {
           logger = _logger;
           driverClient = _driverClient;   
           mapper=new DriverMapper();          
        } 

      [HttpGet]     
      [Route("get")]
        public async Task<IActionResult> Get(int organizationId,int driverId)
        {              
            try 
            {  
               if (organizationId<0)                   
                {
                    return StatusCode(404, "Please provide the organizationId.");
                }
                else if (driverId<0)                   
                {
                    return StatusCode(404, "Please provide the driverId.");
                }
                DriverBusinessService.IdRequest idRequest=new DriverBusinessService.IdRequest();
                idRequest.DriverID=driverId; 
                idRequest.OrgID=organizationId;

                logger.LogInformation("Driver get function called "); 
                if (organizationId<1)
                {
                     return StatusCode(400,"Please provide organization ID:");
                }                 
                DriverBusinessService.DriverDataList drvResponse = await driverClient.GetAsync(idRequest);
                return Ok(drvResponse);  
            }
            catch(Exception ex)
            {            
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }   
        }

     [HttpPut]     
     [Route("update")]
     public async Task<IActionResult> Update(net.atos.daf.ct2.driverservice.DriverUpdateRequest request)
        {              
            try 
            {   
               if (string.IsNullOrEmpty(request.FirstName))
                {
                    return StatusCode(404, "Please provde driver first name.");
                }  
                else if (string.IsNullOrEmpty(request.LastName))
                {
                    return StatusCode(404, "Please provde driver last name.");
                } 
                else if (request.Id <= 0)
                {
                    return StatusCode(404, "Please provde correct driverId.");
                }  
                else if (request.OrganizationId <= 0)
                {
                    return StatusCode(404, "Please provde correct organizationId.");
                }         

                logger.LogInformation("Driver update function called ");     
                DriverBusinessService.DriverUpdateResponse orgResponse = await driverClient.UpdateAsync(request); 
                return Ok(orgResponse);                 
             }
            catch(Exception ex)
            {         
                logger.LogError(ex.Message +" " +ex.StackTrace);             
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }              
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }           
        } 

     [HttpDelete]     
     [Route("delete")]
     public async Task<IActionResult> Delete(int organizationId,int driverId)
        {              
            try 
            {   
                if (organizationId<=0)                   
                {
                    return StatusCode(404, "Please provide the correct organizationId.");
                }
                else if (driverId<=0)                   
                {
                    return StatusCode(404, "Please provide the correct driverId.");
                }
               
                DriverBusinessService.IdRequest idRequest=new DriverBusinessService.IdRequest();
                idRequest.DriverID=driverId; 
                idRequest.OrgID=organizationId;          
                logger.LogInformation("Driver update function called ");     
                DriverBusinessService.DriverDeleteResponse response = await driverClient.DeleteAsync(idRequest); 
                return Ok(response);                 
             }
            catch(Exception ex)
            {         
                logger.LogError(ex.Message +" " +ex.StackTrace);             
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }              
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }           
        }

     [HttpPut]     
     [Route("updateoptinoptout")]
     public async Task<IActionResult> UpdateOptinOptout(net.atos.daf.ct2.driverservice.OptOutOptInRequest Optrequest)
        {              
            try 
            {   
                if (Optrequest.OrgID<=0)                   
                {
                    return StatusCode(404, "Please provide the correct organizationId.");
                }
                else if (string.IsNullOrEmpty(Optrequest.Optoutoptinstatus))                   
                {
                    return StatusCode(404, "Please provide the Optoutoptinstatus.");
                }             
                logger.LogInformation("Driver UpdateOptinOptout function called ");     
                DriverBusinessService.OptOutOptInResponse response = await driverClient.UpdateOptinOptoutAsync(Optrequest); 
                return Ok(response);                 
             }
            catch(Exception ex)
            {         
                logger.LogError(ex.Message +" " +ex.StackTrace);             
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }              
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }           
        } 

     [HttpPost]      
     [Route("importdrivers")]
     public async Task<IActionResult> ImportDrivers(List<DriverRequest> drivers)
        {              
            try 
            {   
                if (drivers.Count<=0)                   
                {
                    return StatusCode(404, "Please provide the driver list to import.");
                }       
                logger.LogInformation("Driver import function called ");   
                net.atos.daf.ct2.driverservice.DriverImportRequest request=new DriverBusinessService.DriverImportRequest();
                request=mapper.ToDriverImport(drivers);
               
                if (request.OrgID<=0)                   
                {
                    return StatusCode(404, "Please provide the oganizationid import.");
                }                                  
                DriverBusinessService.DriverImportData response = await driverClient.ImportDriversAsync(request); 
                
                return Ok(response);                 
             }
            catch(Exception ex)
            {         
                logger.LogError(ex.Message +" " +ex.StackTrace);             
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }              
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }           
        }              
    }
}
