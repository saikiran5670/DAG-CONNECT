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
using Microsoft.Net.Http.Headers;
using AccountComponent = net.atos.daf.ct2.account;
using AccountEntity = net.atos.daf.ct2.account.entity;
using IdentityComponent = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;

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
         AccountComponent.IAccountIdentityManager accountIdentityManager;

        private IHttpContextAccessor _httpContextAccessor;

        public CustomerManagementController(ILogger<CustomerManagementController> logger, IAuditTraillib AuditTrail, IOrganizationManager _organizationmanager,IPreferenceManager _preferencemanager,IVehicleManager _vehicleManager,IHttpContextAccessor httpContextAccessor,AccountComponent.IAccountIdentityManager _accountIdentityManager)
        {
           _logger = logger;
           _AuditTrail = AuditTrail;
            organizationtmanager = _organizationmanager;
            preferencemanager=_preferencemanager;
            vehicleManager=_vehicleManager;
           _httpContextAccessor=httpContextAccessor;
            accountIdentityManager=_accountIdentityManager;
        } 
        
        [HttpPost]
        [Route("UpdateCustomerData")]
        public async Task<IActionResult> update(Customer customer)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); 
            bool valid=false;
            try 
            {
                if(string.IsNullOrEmpty(token))
                {
                    return StatusCode(400,"Bad Request:");
                }
                else
                {
                     valid = await accountIdentityManager.ValidateToken(token);
                     if(valid)
                     {
                        var OrgId= await organizationtmanager.UpdateCustomer(customer);
                        return Ok(OrgId);
                     }
                     else
                     {
                         return StatusCode(401,"Invalid_Grant:");
                     }
                }
            }
            catch(Exception ex)
            {
                valid = false;
               // logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Internal Server Error.");
            }                        
        }  

        [HttpPost]
        [Route("KeyHandOverEvent")]
        public async Task<IActionResult> KeyHandOverEvent(KeyHandOver keyHandOver)
        {
           string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); 
            bool valid=false;
            try 
            {
                if(string.IsNullOrEmpty(token))
                {
                    return StatusCode(400,"Bad Request:");
                }
                else
                {
                     valid = await accountIdentityManager.ValidateToken(token);
                     if(valid)
                     {
                        var OrgId= await organizationtmanager.KeyHandOverEvent(keyHandOver);
                        return Ok(OrgId);
                     }
                     else
                     {
                         return StatusCode(401,"Invalid_Grant:");
                     }
                }
            }
            catch(Exception ex)
            {
                valid = false;
               // logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Internal Server Error.");
            }   
        }       
    }
}
