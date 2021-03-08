using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrganizationBusinessService = net.atos.daf.ct2.organizationservice;
using net.atos.daf.ct2.portalservice.Entity.Organization;
using AccountBusinessService = net.atos.daf.ct2.accountservice;
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("organization")]
    public class OrganizationController: ControllerBase
    {  
        private readonly ILogger<OrganizationController> logger;
        private readonly OrganizationMapper _mapper;
        private readonly AccountBusinessService.AccountService.AccountServiceClient _accountClient;
        private readonly OrganizationBusinessService.OrganizationService.OrganizationServiceClient organizationClient;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";

        public OrganizationController(ILogger<OrganizationController> _logger, OrganizationBusinessService.OrganizationService.OrganizationServiceClient _organizationClient,AccountBusinessService.AccountService.AccountServiceClient accountClient)
        {
           logger = _logger;
           organizationClient = _organizationClient;
           _accountClient = accountClient;
           _mapper = new OrganizationMapper();
        } 

     [HttpPost]      
     [Route("create")]
     public async Task<IActionResult> create(OrganizationRequest request)
        {             
            try 
            {   
                Organization organization = new Organization();  
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
                organization.PostalCode=request.postal_code;
                organization.reference_date=request.reference_date;               
                var objRequest = _mapper.ToOragnizationRequest(request);
                OrganizationBusinessService.OrganizationCreateData orgResponse = await organizationClient.CreateAsync(objRequest);
                if (orgResponse.Organization.Id<1)
                {
                    return StatusCode(400,"This organization is already exist :" + request.org_id);
                }
                else
                {
                   return Ok("Organization Created :" +orgResponse);    
                }  
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
     [Route("update")]
     public async Task<IActionResult> Update(OrganizationRequest request)
        {              
            try 
            {   
                 var objRequest = _mapper.ToOragnizationUpdateRequest(request);
                 Organization organization = new Organization(); 
              
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
               
                 OrganizationBusinessService.OrganizationUpdateData orgResponse = await organizationClient.UpdateAsync(objRequest); 
                if(orgResponse.Organization.Id==0)
                {
                    return StatusCode(400,"Organization ID not exist: "+ request.Id); 
                }
                else if(orgResponse.Organization.Id==-1)
                {
                    return StatusCode(400,"This organization is already exist :"+ request.org_id); 
                }
                else
                {
                   return Ok("Organization Created :" +orgResponse);   
                }
             }
            catch(Exception ex)
            {         
                logger.LogError(ex.Message +" " +ex.StackTrace);
              //  await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Organization Component","Organization Service",AuditTrailEnum.Event_type.DELETE,AuditTrailEnum.Event_status.FAILED,"Update method in organization manager",0,0,JsonConvert.SerializeObject(request));      
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }           
        }       

     [HttpGet]     
     [Route("get")]
     public async Task<IActionResult> Get(int organizationId)
        {              
            try 
            {      
                 //var objRequest = _mapper.ToOragnizationRequest(organizationId);
                 OrganizationBusinessService.IdRequest idRequest=new OrganizationBusinessService.IdRequest();
                 idRequest.Id=organizationId;
                // OrganizationBusinessService.OrganizationGetData orgResponse = await organizationClient.GetAsync(idRequest);
                 // return Ok("Organization Created :" +orgResponse); 
                logger.LogInformation("Organization get function called "); 
                if (organizationId<1)
                {
                     return StatusCode(400,"Please provide organization ID:");
                }  
                  OrganizationBusinessService.OrganizationGetData orgResponse = await organizationClient.GetAsync(idRequest);
                               
                return Ok(orgResponse);  
            }
            catch(Exception ex)
            {            
                logger.LogError(ex.Message +" " +ex.StackTrace);
                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500,ex.Message +" " +ex.StackTrace);
            }   
        }

         //Begin Account Preference
        [HttpPost]
        [Route("preference/create")]
        public async Task<IActionResult> CreatePreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }
                var accountPreference = _mapper.ToAccountPreference(request);
                AccountBusinessService.AccountPreferenceResponse preference = await _accountClient.CreatePreferenceAsync(accountPreference);
                if (preference != null && preference.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(_mapper.ToAccountPreference(preference.AccountPreference));
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        

         [HttpPost]
        [Route("preference/update")]
        public async Task<IActionResult> UpdatePreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.Id <= 0) || (request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatTypeId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Preference Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }
                var accountPreference = _mapper.ToAccountPreference(request);
                AccountBusinessService.AccountPreferenceResponse preference = await _accountClient.UpdatePreferenceAsync(accountPreference);
                if (preference != null && preference.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(_mapper.ToAccountPreference(preference.AccountPreference));
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

         [HttpDelete]
        [Route("preference/delete")]
        public async Task<IActionResult> DeletePreference(int preferenceId)
        {
            try
            {
                AccountBusinessService.IdRequest request = new AccountBusinessService.IdRequest();
                // Validation                 
                if (preferenceId <= 0)
                {
                    return StatusCode(400, "The preferenceId Id is required");
                }
                request.Id = preferenceId;
                AccountBusinessService.AccountPreferenceResponse response = await _accountClient.DeletePreferenceAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok("Preference Deleted.");
                }
                else if (response != null && response.Code == AccountBusinessService.Responcecode.NotFound)
                {
                    return StatusCode(404, "Preference not found.");
                }
                else
                {
                    return StatusCode(500, "preference is null" + response.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("preference/get")]
        public async Task<IActionResult> GetPreference(int organizationId)
        {
            try
            {
                 OrganizationBusinessService.IdRequest idRequest=new OrganizationBusinessService.IdRequest();
                 idRequest.Id=organizationId;
                 OrganizationBusinessService.OrganizationPreferenceResponse objResponse=new OrganizationBusinessService.OrganizationPreferenceResponse();
                // Validation                 
                if ((organizationId <= 0))
                {
                    return StatusCode(400, "Organization Id is required");
                }            

                objResponse= await organizationClient.GetPreferenceAsync(idRequest);                
                return Ok(objResponse);
            }
            catch (Exception ex)
            {
                logger.LogError("Error in organization service: get preference with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
       // End - Account Preference    
    }
}


