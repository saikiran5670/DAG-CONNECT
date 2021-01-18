using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization.entity;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.audit.repository;  
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace net.atos.daf.ct2.customerdataservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerManagementController: ControllerBase
    {
        private readonly ILogger _logger;        
        private readonly IAuditLogRepository _IAuditLogRepository;       
        private readonly IAuditTraillib _AuditTrail;      
        private readonly IOrganizationManager organizationtmanager;
        private readonly IPreferenceManager preferencemanager;
        private readonly IVehicleManager vehicleManager;

        public CustomerManagementController(ILogger<CustomerManagementController> logger, IAuditTraillib AuditTrail, IOrganizationManager _organizationmanager,IPreferenceManager _preferencemanager,IVehicleManager _vehicleManager)
        {
           _logger = logger;
           _AuditTrail = AuditTrail;
            organizationtmanager = _organizationmanager;
            preferencemanager=_preferencemanager;
            vehicleManager=_vehicleManager;
        } 
        
        [HttpPost]
        [Route("UpdateCustomerData")]
        public async Task<IActionResult> update(Customer customer)
        {
            try
            {               
               //  logger.LogInformation("Customer update function called -" + organization.OrganizationId);  
                 var OrgId= await organizationtmanager.UpdateCustomer(customer);
                // logger.LogInformation("Customer updated - " + organization.OrganizationId);
                 return Ok(OrgId);
            }
            catch (Exception ex)
            {
               // logger.LogError(ex.Message);
                var p = ex.Message;
               // return StatusCode(500, "Internal Server Error.");
               return StatusCode(500, ex.Message);
            }
        }  

        [HttpPost]
        [Route("KeyHandOverEvent")]
        public async Task<IActionResult> KeyHandOverEvent(KeyHandOver keyHandOver)
        {
            try
            {               
               //  logger.LogInformation("Customer update function called -" + organization.OrganizationId);  
                 var OrgId= await organizationtmanager.KeyHandOverEvent(keyHandOver);
                // logger.LogInformation("Customer updated - " + organization.OrganizationId);
                 return Ok(OrgId);
            }
            catch (Exception ex)
            {
               // logger.LogError(ex.Message);
                //var p = ex.Message;
              //  return StatusCode(500, "Internal Server Error.");
              return StatusCode(500, ex.Message);
            }
        }       
    }
}
