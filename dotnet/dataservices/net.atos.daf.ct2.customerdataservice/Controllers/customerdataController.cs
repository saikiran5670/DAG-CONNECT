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
using SubscriptionComponent=net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.repository;
//using net.atos.daf.ct2.customerdataservice.Entity;

namespace net.atos.daf.ct2.customerdataservice.Controllers
{
    [ApiController]
    [Route("customer-data")]
    public class customerdataController: ControllerBase
    {
        //private readonly ILogger _logger;       
        private readonly ILogger<customerdataController> logger; 
       // private readonly IAuditLogRepository _IAuditLogRepository;       
        private readonly IAuditTraillib _AuditTrail;      
        private readonly IOrganizationManager organizationtmanager;
        private readonly IPreferenceManager preferencemanager;
        private readonly IVehicleManager vehicleManager;
         AccountComponent.IAccountIdentityManager accountIdentityManager;
         SubscriptionComponent.ISubscriptionManager subscriptionManager;

        private IHttpContextAccessor _httpContextAccessor;
        public IConfiguration Configuration { get; }
        public customerdataController(ILogger<customerdataController> _logger, IAuditTraillib AuditTrail, IOrganizationManager _organizationmanager,IPreferenceManager _preferencemanager,IVehicleManager _vehicleManager,IHttpContextAccessor httpContextAccessor,AccountComponent.IAccountIdentityManager _accountIdentityManager, SubscriptionComponent.ISubscriptionManager _subscriptionManager,IConfiguration configuration)
        {
            logger = _logger;
           _AuditTrail = AuditTrail;
            organizationtmanager = _organizationmanager;
            preferencemanager=_preferencemanager;
            vehicleManager=_vehicleManager;
           _httpContextAccessor=httpContextAccessor;
            accountIdentityManager=_accountIdentityManager;
            subscriptionManager=_subscriptionManager;
            Configuration=configuration;
        } 
        
        [HttpPost]      
        [Route("update")]
        public async Task<IActionResult> update(Customer customer)
        {         
         CustomerRequest customerRequest=new CustomerRequest();
         customerRequest.CompanyType=customer.CompanyUpdatedEvent.Company.type;
         customerRequest.CustomerID=customer.CompanyUpdatedEvent.Company.ID;
         customerRequest.CustomerName=customer.CompanyUpdatedEvent.Company.Name;
         customerRequest.AddressType=customer.CompanyUpdatedEvent.Company.type;
         customerRequest.Street=customer.CompanyUpdatedEvent.Company.Address.Street;
         customerRequest.StreetNumber=customer.CompanyUpdatedEvent.Company.Address.StreetNumber;
         customerRequest.PostalCode=customer.CompanyUpdatedEvent.Company.Address.PostalCode;
         customerRequest.City=customer.CompanyUpdatedEvent.Company.Address.City;
         customerRequest.CountryCode=customer.CompanyUpdatedEvent.Company.Address.CountryCode;
         customerRequest.ReferenceDateTime=customer.CompanyUpdatedEvent.Company.ReferenceDateTime;
         string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); 
        // string token ="TestToken";
        // bool valid=true;  for testing only
         bool valid=false;
            try 
            {               
                if(string.IsNullOrEmpty(token))
                {
                    logger.LogInformation("UpdateCustomerData function called with empty token, company ID -" + customer.CompanyUpdatedEvent.Company.ID);
                    return StatusCode(400);
                }
                else
                {  
                     logger.LogInformation("UpdateCustomerData function called , company ID -" + customer.CompanyUpdatedEvent.Company.ID);
                     valid = await accountIdentityManager.ValidateToken(token);
                     if(valid)
                     {
                          if (!(
                                 (string.IsNullOrEmpty(customerRequest.CompanyType) || (customerRequest.CompanyType.Trim().Length<1))
                                 || ((string.IsNullOrEmpty(customerRequest.CustomerID) || (customerRequest.CustomerID.Trim().Length<1)))
                                 || ((string.IsNullOrEmpty(customerRequest.CustomerName) || (customerRequest.CustomerName.Trim().Length<1)))
                                 || ((string.IsNullOrEmpty( customerRequest.AddressType) || ( customerRequest.AddressType.Trim().Length<1)))
                                 || ((Convert.ToDateTime(customerRequest.ReferenceDateTime).ToUniversalTime()>System.DateTime.Now.ToUniversalTime()))
                              ))
                                {
                                   return StatusCode(400);
                                 }
                                 else{
                        
                                      await organizationtmanager.UpdateCustomer(customerRequest);
                                      logger.LogInformation("Customer data has been updated, company ID -" + customerRequest.CustomerID);
                                      return Ok();
                                     }
                    }
                     else
                     {
                         logger.LogInformation("Customer data not updated, company ID -" + customerRequest.CustomerID);
                         return StatusCode(401);
                     }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Internal Server Error.");              
            }                        
        }  

        [HttpPost]
        [Route("keyhandover")]
        public async Task<IActionResult> keyhandover(KeyHandOver keyHandOver)
        {
               HandOver objHandOver=new  HandOver();
               objHandOver.VIN=keyHandOver.KeyHandOverEvent.VIN;
               objHandOver.TCUID=keyHandOver.KeyHandOverEvent.TCUID;
               objHandOver.TCUActivation=keyHandOver.KeyHandOverEvent.TCUActivation;
               objHandOver.ReferenceDateTime=keyHandOver.KeyHandOverEvent.ReferenceDateTime;
               objHandOver.CustomerID=keyHandOver.KeyHandOverEvent.EndCustomer.ID;
               objHandOver.CustomerName=keyHandOver.KeyHandOverEvent.EndCustomer.Name;
               objHandOver.Type=keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type;
               objHandOver.Street=keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street;
               objHandOver.StreetNumber=keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber;
               objHandOver.PostalCode=keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode;
               objHandOver.City=keyHandOver.KeyHandOverEvent.EndCustomer.Address.City;
               objHandOver.CountryCode=keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode;
               // Configuarable values                                       
               objHandOver.OwnerRelationship= Configuration.GetSection("DefaultSettings").GetSection("OwnerRelationship").Value; 
               objHandOver.OEMRelationship=Configuration.GetSection("DefaultSettings").GetSection("OEMRelationship").Value; 
               objHandOver.OrgCreationPackage=Configuration.GetSection("DefaultSettings").GetSection("OrgCreationPackage").Value; 
               objHandOver.DAFPACCAR=Configuration.GetSection("DefaultSettings").GetSection("DAFPACCAR").Value; 
               
               string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); 
             
              // bool valid=true; for testing only
              // string token="testToken"; for testing only
              bool valid=false;
            try 
            {
                if(string.IsNullOrEmpty(token))
                {
                    logger.LogInformation("KeyHandOverEvent function called with empty token, company ID -" +objHandOver.CustomerID);
                    return StatusCode(400);
                }
                else
                {
                  valid = await accountIdentityManager.ValidateToken(token);
                   if(valid)
                     {
                        if (!(
                            (string.IsNullOrEmpty(objHandOver.VIN) || (objHandOver.VIN.Trim().Length<1))
                           || ((string.IsNullOrEmpty(objHandOver.TCUID) || (objHandOver.TCUID.Trim().Length<1)))
                           || ((string.IsNullOrEmpty(objHandOver.CustomerName) || (objHandOver.CustomerName.Trim().Length<1)))
                           || ((string.IsNullOrEmpty(objHandOver.ReferenceDateTime) || (objHandOver.ReferenceDateTime.Trim().Length<1)))
                           || ((Convert.ToDateTime(objHandOver.ReferenceDateTime).ToUniversalTime()>System.DateTime.Now.ToUniversalTime()))
                           || (!((objHandOver.TCUActivation.ToUpper()=="YES") || (objHandOver.TCUActivation.ToUpper()=="NO")))
                        ))
                        {
                         return StatusCode(400);
                        }                                              
                        else
                            {
                               await organizationtmanager.KeyHandOverEvent(objHandOver);
                                return Ok();
                            }                                         
                     }
                     else
                     {
                         logger.LogInformation("KeyHandOverEvent not executed successfully, company ID -" + keyHandOver.KeyHandOverEvent.EndCustomer.ID);
                         return StatusCode(401);
                     }
                }
            }
            catch(Exception ex)
            {               
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Internal Server Error.");             
            }   
        }       
    }
}
