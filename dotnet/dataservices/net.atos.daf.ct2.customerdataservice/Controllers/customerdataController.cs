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
        public customerdataController(ILogger<customerdataController> _logger, IAuditTraillib AuditTrail, IOrganizationManager _organizationmanager,IPreferenceManager _preferencemanager,IVehicleManager _vehicleManager,IHttpContextAccessor httpContextAccessor,AccountComponent.IAccountIdentityManager _accountIdentityManager, SubscriptionComponent.ISubscriptionManager _subscriptionManager)
        {
            logger = _logger;
           _AuditTrail = AuditTrail;
            organizationtmanager = _organizationmanager;
            preferencemanager=_preferencemanager;
            vehicleManager=_vehicleManager;
           _httpContextAccessor=httpContextAccessor;
            accountIdentityManager=_accountIdentityManager;
           subscriptionManager=_subscriptionManager;
        } 
        
        [HttpPost]      
        [Route("update")]
        public async Task<IActionResult> update(Customer customer)
        {
         
        //    var OrgId= await organizationtmanager.UpdateCustomer(customer);
        //    return Ok("done");
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); 
            bool valid=false;
            try 
            {               
                if(string.IsNullOrEmpty(token))
                {
                    logger.LogInformation("UpdateCustomerData function called with empty token, company ID -" + customer.CompanyUpdatedEvent.Company.ID);
                    return StatusCode(400,"Bad Request:");
                }
                else
                {  
                     logger.LogInformation("UpdateCustomerData function called , company ID -" + customer.CompanyUpdatedEvent.Company.ID);
                     valid = await accountIdentityManager.ValidateToken(token);
                     if(valid)
                     {
                        if (string.IsNullOrEmpty(customer.CompanyUpdatedEvent.Company.ID) || (customer.CompanyUpdatedEvent.Company.ID.Trim().Length<1))
                        {
                             return StatusCode(400,"Please provide company ID:");
                        } 
                        else if (string.IsNullOrEmpty(customer.CompanyUpdatedEvent.Company.type) || (customer.CompanyUpdatedEvent.Company.type.Trim().Length<1))
                        {
                             return StatusCode(400,"Please provide company type:");
                        }
                        else if (string.IsNullOrEmpty(customer.CompanyUpdatedEvent.Company.Name) || (customer.CompanyUpdatedEvent.Company.Name.Trim().Length<1))
                        {
                             return StatusCode(400,"Please provide company name:");
                        }
                        else  if ((customer.CompanyUpdatedEvent.Company.ReferenceDateTime == null) && (DateTime.Compare(DateTime.MinValue, customer.CompanyUpdatedEvent.Company.ReferenceDateTime)< 0))
                        {
                             return StatusCode(400,"Please provide company reference date:");                            
                        }
                        else if (customer.CompanyUpdatedEvent.Company.ReferenceDateTime.ToUniversalTime() >System.DateTime.Now.ToUniversalTime() )
                        {
                             return StatusCode(400,"Future date time is not allowed in company reference date, please provide company reference date time less then toaday :");
                        }

                        var OrgId= await organizationtmanager.UpdateCustomer(customer);
                        logger.LogInformation("Customer data has been updated, company ID -" + customer.CompanyUpdatedEvent.Company.ID);
                        return Ok(OrgId);
                    }
                     else
                     {
                         logger.LogInformation("Customer data not updated, company ID -" + customer.CompanyUpdatedEvent.Company.ID);
                         return StatusCode(401,"Forbidden:");
                     }
             }
            }
            catch(Exception ex)
            {
                valid = false;
                logger.LogError(ex.Message +" " +ex.StackTrace);
                return StatusCode(500,"Internal Server Error.");
               // return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }                        
        }  

        [HttpPost]
        [Route("keyhandover")]
        public async Task<IActionResult> keyhandover(KeyHandOver keyHandOver)
        {
            //   var OrgId= await organizationtmanager.KeyHandOverEvent(keyHandOver);
            //   return Ok("done");             
              
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
               // var connectionString = Configuration.GetConnectionString("ConnectionString");              
               objHandOver.OwnerRelationship=Configuration.DefaultSettings("OwnerRelationship");
               objHandOver.OEMRelationship=Configuration.DefaultSettings("OEMRelationship");
               objHandOver.OrgCreationPackage=Configuration.DefaultSettings("OrgCreationPackage");
               objHandOver.DAFPACCAR=Configuration.DefaultSettings("DAFPACCAR");
               
                var OrgId= await organizationtmanager.KeyHandOverEvent(objHandOver);
                return Ok(1);

        //     string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); 
        //     bool valid=false;
        //     try 
        //     {
        //         if(string.IsNullOrEmpty(token))
        //         {
        //             logger.LogInformation("KeyHandOverEvent function called with empty token, company ID -" + keyHandOver.KeyHandOverEvent.EndCustomer.ID);
        //             return StatusCode(400,"Bad Request:");
        //         }
        //         else
        //         {
        //              valid = await accountIdentityManager.ValidateToken(token);
        //              if(valid)
        //              {
        //                 if (string.IsNullOrEmpty(keyHandOver.KeyHandOverEvent.VIN)  || (keyHandOver.KeyHandOverEvent.VIN.Trim().Length<1))
        //                 {
        //                      return StatusCode(400,"Please provide VIN:");
        //                 } 
        //                 else if (string.IsNullOrEmpty(keyHandOver.KeyHandOverEvent.TCUID) || (keyHandOver.KeyHandOverEvent.TCUID.Trim().Length<1))
        //                 {
        //                      return StatusCode(400,"Please provide TCUID:");
        //                 }
        //                 else if (string.IsNullOrEmpty(keyHandOver.KeyHandOverEvent.EndCustomer.ID) || (keyHandOver.KeyHandOverEvent.EndCustomer.ID.Trim().Length<1))
        //                 {
        //                      return StatusCode(400,"Please provide company ID:");
        //                 }
        //                 else if (string.IsNullOrEmpty(keyHandOver.KeyHandOverEvent.EndCustomer.Name) || (keyHandOver.KeyHandOverEvent.EndCustomer.Name.Trim().Length<1))
        //                 {
        //                      return StatusCode(400,"Please provide company name:");
        //                 }
        //                 else  if (string.IsNullOrEmpty(keyHandOver.KeyHandOverEvent.ReferenceDateTime) || (keyHandOver.KeyHandOverEvent.ReferenceDateTime.Trim().Length<1))
        //                 {
        //                      return StatusCode(400,"Please provide company reference date:");
        //                 }
        //                 else  if (Convert.ToDateTime(keyHandOver.KeyHandOverEvent.ReferenceDateTime).ToUniversalTime()>System.DateTime.Now.ToUniversalTime())
        //                 {
        //                     return StatusCode(400,"Future date time is not allowed in company reference date, please provide company reference date time less then toaday :");
        //                 }
        //                 else  if (!((keyHandOver.KeyHandOverEvent.TCUActivation.ToUpper()=="YES") || (keyHandOver.KeyHandOverEvent.TCUActivation.ToUpper()=="NO")))
        //                 {
        //                       return StatusCode(400,"Please provide correct TCU Activation status:");
        //                  }                             
        //                 var OrgId= await organizationtmanager.KeyHandOverEvent(keyHandOver);
        //                 logger.LogInformation("KeyHandOverEvent executed successfully, company ID -" + keyHandOver.KeyHandOverEvent.EndCustomer.ID);
        //                 return Ok(OrgId);
        //              }
        //              else
        //              {
        //                  logger.LogInformation("KeyHandOverEvent not executed successfully, company ID -" + keyHandOver.KeyHandOverEvent.EndCustomer.ID);
        //                  return StatusCode(401,"Forbidden:");
        //              }
        //         }
        //     }
        //     catch(Exception ex)
        //     {
        //         valid = false;
        //         logger.LogError(ex.Message +" " +ex.StackTrace);
        //         return StatusCode(500,"Internal Server Error.");
        //        // return StatusCode(500,ex.Message +" " +ex.StackTrace);
        //     }   
        }       
    }
}
