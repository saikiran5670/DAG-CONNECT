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
using net.atos.daf.ct2.organizationservicerest.entity;
using OrganizationComponent = net.atos.daf.ct2.organization;
using AccountEntity = net.atos.daf.ct2.account.entity;
using IdentityComponent = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;


namespace net.atos.daf.ct2.organizationservicerest.Controllers
{
    [ApiController]
    [Route("organization")]
    public class OrganizationController: ControllerBase
    {
        private readonly ILogger<OrganizationController> logger; 
      //  private readonly IAuditLogRepository _IAuditLogRepository;       
        private readonly IAuditTraillib _AuditTrail;      
        private readonly IOrganizationManager organizationtmanager;
        private readonly IPreferenceManager preferencemanager;
        private readonly IVehicleManager vehicleManager;
         AccountComponent.IAccountIdentityManager accountIdentityManager;
        private IHttpContextAccessor _httpContextAccessor;
        public OrganizationController(ILogger<OrganizationController> _logger, IAuditTraillib AuditTrail, IOrganizationManager _organizationmanager,IPreferenceManager _preferencemanager,IVehicleManager _vehicleManager,IHttpContextAccessor httpContextAccessor,AccountComponent.IAccountIdentityManager _accountIdentityManager)
        {
            logger = _logger;
           _AuditTrail = AuditTrail;
            organizationtmanager = _organizationmanager;
            preferencemanager=_preferencemanager;
            vehicleManager=_vehicleManager;
           _httpContextAccessor=httpContextAccessor;
            accountIdentityManager=_accountIdentityManager;
        } 
     [HttpPost]      
     [Route("create")]
     public async Task<IActionResult> create(OrganizationRequest request)
        {             
            try 
            {   
                OrganizationComponent.entity.Organization organization = new  OrganizationComponent.entity.Organization();    
                
                logger.LogInformation("Organization create function called ");
                if (string.IsNullOrEmpty(request.org_id) || (request.org_id.Trim().Length<1))
                {
                     return StatusCode(400,"Please provide organization org_id:");
                }
                if (string.IsNullOrEmpty(request.name)|| (request.name.Trim().Length<1))
                {
                     return StatusCode(400,"Please provide organization name:");
                }
                organization.OrganizationId=request.org_id;
                organization.Name=request.name;
                organization.Type=request.type;
                organization.AddressType=request.address_type;
                organization.AddressStreet=request.street_number;
                organization.AddressStreetNumber=request.street_number;
                organization.City=request.city;
                organization.CountryCode=request.country_code;
                organization.reference_date=request.reference_date;
                organization.OptOutStatus=request.optout_status;
                organization.optout_status_changed_date=request.optout_status_changed_date;
                organization.IsActive=request.is_active;
              
                var OrgId= await organizationtmanager.Create(organization);   
                return Ok("Organization Created :");      
             }
            catch(Exception ex)
            {
                 logger.LogError(ex.Message +" " +ex.StackTrace);
                 //return StatusCode(500,"Internal Server Error.");
                 return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }   
        } 

     [HttpPut]     
     [Route("update")]
     public async Task<IActionResult> Update(OrganizationRequest request)
        {              
            try 
            {   
                OrganizationComponent.entity.Organization organization = new  OrganizationComponent.entity.Organization();    
               
                logger.LogInformation("Organization update function called ");    
                if (request.Id<1)
                {
                     return StatusCode(400,"Please provide organization ID:");
                }
                if (string.IsNullOrEmpty(request.org_id) || (request.org_id.Trim().Length<1))
                {
                     return StatusCode(400,"Please provide organization org_id:");
                }
                if (string.IsNullOrEmpty(request.name) || (request.name.Trim().Length<1))
                {
                     return StatusCode(400,"Please provide organization name:");
                }

                organization.Id=request.Id;
                organization.OrganizationId=request.org_id;
                organization.Name=request.name;
                organization.Type=request.type;
                organization.AddressType=request.address_type;
                organization.AddressStreet=request.street_number;
                organization.AddressStreetNumber=request.street_number;
                organization.City=request.city;
                organization.CountryCode=request.country_code;
                organization.reference_date=request.reference_date;
                organization.OptOutStatus=request.optout_status;
                organization.optout_status_changed_date=request.optout_status_changed_date;
                organization.IsActive=request.is_active;
                var OrgId= await organizationtmanager.Update(organization);   
                return Ok("Organization updated :");    
             }
            catch(Exception ex)
            {         
                logger.LogError(ex.Message +" " +ex.StackTrace);
                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }           
        }       


     [HttpDelete]   
     [Route("delete")]
     public async Task<IActionResult> Delete(int organizationId)
        {              
            try 
            {      
                logger.LogInformation("Organization delete function called "); 
                if (organizationId<1)
                {
                     return StatusCode(400,"Please provide organization ID:");
                }
                var OrgId= await organizationtmanager.Delete(organizationId);   
                return Ok("Organization Deleted : " +organizationId);    
             }
            catch(Exception ex)
            {            
                logger.LogError(ex.Message +" " +ex.StackTrace);
               // return StatusCode(500,"Internal Server Error.");
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }   
        }     
     
     [HttpGet]     
     [Route("get")]
     public async Task<IActionResult> Get(int organizationId)
        {              
            try 
            {      
                logger.LogInformation("Organization get function called "); 
                if (organizationId<1)
                {
                     return StatusCode(400,"Please provide organization ID:");
                }              
                return Ok(await organizationtmanager.Get(organizationId));
             }
            catch(Exception ex)
            {            
                logger.LogError(ex.Message +" " +ex.StackTrace);
                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }   
        }    
    }
}
