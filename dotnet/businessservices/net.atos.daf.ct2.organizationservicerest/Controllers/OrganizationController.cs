using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.vehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AccountComponent = net.atos.daf.ct2.account;
using net.atos.daf.ct2.organizationservicerest.entity;
using OrganizationComponent = net.atos.daf.ct2.organization;
using Preference = net.atos.daf.ct2.accountpreference;
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
         private readonly IAuditTraillib auditlog;
        private readonly EntityMapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        public OrganizationController(ILogger<OrganizationController> _logger, IAuditTraillib AuditTrail, IOrganizationManager _organizationmanager,IPreferenceManager _preferencemanager,IVehicleManager _vehicleManager,IHttpContextAccessor httpContextAccessor,AccountComponent.IAccountIdentityManager _accountIdentityManager, IAuditTraillib _auditlog)
        {
            logger = _logger;
           _AuditTrail = AuditTrail;
            organizationtmanager = _organizationmanager;
            preferencemanager=_preferencemanager;
            vehicleManager=_vehicleManager;
           _httpContextAccessor=httpContextAccessor;
            accountIdentityManager=_accountIdentityManager;
            auditlog = _auditlog;
            _mapper = new EntityMapper();
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
                //organization.IsActive=request.is_active;
              
                var OrgId= await organizationtmanager.Create(organization);   
                if (OrgId.Id<1)
                {
                    return StatusCode(400,"This organization is already exist :" + request.org_id);
                }
                else
                {
                    return Ok("Organization Created :" +OrgId.Id);    
                }  
             }
            catch(Exception ex)
            {
                 logger.LogError(ex.Message +" " +ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
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
                // if (request.Id.GetType()!=typeof(int))
                // {
                //      return StatusCode(400,"Please provide numeric value for organization ID:");
                // }           
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
                // organization.IsActive=request.is_active;
                var OrgId= await organizationtmanager.Update(organization);   
                if(OrgId.Id==0)
                {
                    return StatusCode(400,"Organization ID not exist: "+ request.Id); 
                }
                else if(OrgId.Id==-1)
                {
                    return StatusCode(400,"This organization is already exist :"+ request.org_id); 
                }
                else
                {
                   return Ok("Organization updated :"+ OrgId.Id);    
                }
             }
            catch(Exception ex)
            {         
                logger.LogError(ex.Message +" " +ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
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
                if (OrgId)   
                {
                     return Ok("Organization Deleted : " +organizationId);   
                } 
                else{
                 return StatusCode(400,"Organization ID not exist: "+organizationId); 
                } 
             }
            catch(Exception ex)
            {            
                logger.LogError(ex.Message +" " +ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
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
                 var onjRetun=await organizationtmanager.Get(organizationId);
                 if(onjRetun.Id<1)
                 {
                     return StatusCode(400,"Organization ID not exist :" + organizationId);
                 }  
                 else
                 {  
                     return Ok(await organizationtmanager.Get(organizationId));
                 }
             }
            catch(Exception ex)
            {            
                logger.LogError(ex.Message +" " +ex.StackTrace);
                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }   
        }

         // Begin Account Preference
        [HttpPost]
        [Route("preference/create")]
        public async Task<IActionResult> CreateAccountPreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.OrgId <= 0) || (request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatTypeId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }
                accountpreference.AccountPreference preference = new Preference.AccountPreference();                
                preference = _mapper.ToOrganizationPreference(request);
                preference.Exists=false;
                preference = await preferencemanager.Create(preference);
                // check for exists
                if (preference.Exists)
                {
                    return StatusCode(409, "Duplicate Organization Preference.");
                }
                // check for exists
                else if (preference.RefIdNotValid)
                {
                    return StatusCode(400, "The Organization ID is not valid or created.");
                }

               // var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Preference Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Preference", 1, 2, Convert.ToString(preference.RefId));
               return Ok("Organization Preference Created : " +preference.Id);
                            
            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
               if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("preference/update")]
        public async Task<IActionResult> UpdateAccountPreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.OrgId <= 0) || (request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatTypeId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }               
                accountpreference.AccountPreference preference = new Preference.AccountPreference();                
                preference = _mapper.ToOrganizationPreference(request);
                preference = await preferencemanager.Update(preference);
                //var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Preference Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Preference", 1, 2, Convert.ToString(preference.RefId));
                return Ok("Preference Updated : " +preference.Id); 
            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
               if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpDelete]
        [Route("preference/delete")]
        public async Task<IActionResult> DeleteAccountPreference(int accountId)
        {
            try
            {
                // Validation                 
                if ((accountId <= 0))
                {
                    return StatusCode(400, "The Account Id is required");
                }
                var result = await preferencemanager.Delete(accountId);
                //var auditResult = await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Preference Component", "Account Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Delete Preference", 1, 2, Convert.ToString(accountId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("preference/get")]
        public async Task<IActionResult> GetAccountPreference(int accountId)
        {
            try
            {
                // Validation                 
                if ((accountId <= 0))
                {
                    return StatusCode(400, "The Account Id is required");
                }
                Preference.AccountPreferenceFilter preferenceFilter = new Preference.AccountPreferenceFilter();
                preferenceFilter.Id = 0;
                preferenceFilter.Ref_Id = accountId;
                preferenceFilter.PreferenceType = Preference.PreferenceType.Account;
                var result = await preferencemanager.Get(preferenceFilter);
                if ((result == null) || Convert.ToInt16(result.Count()) <= 0)
                {
                    return StatusCode(404, "Account Preference for this account is not configured.");
                }
                logger.LogInformation("Get account preference.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:get account preference with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
        // End - Account Preference    
    }
}
