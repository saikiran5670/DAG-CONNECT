using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization.entity;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.vehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AccountComponent = net.atos.daf.ct2.account;
using SubscriptionComponent = net.atos.daf.ct2.subscription;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.customerdataservice.CustomAttributes;

namespace net.atos.daf.ct2.customerdataservice.Controllers
{
    [ApiController]
    [Route("customer-data")]
    [Authorize(Policy = AccessPolicies.MainAccessPolicy)]
    public class customerdataController: ControllerBase
    {       
        private readonly ILogger<customerdataController> logger;              
        private readonly IAuditTraillib AuditTrail;      
        private readonly IOrganizationManager organizationtmanager;
        private readonly IPreferenceManager preferencemanager;
        private readonly IVehicleManager vehicleManager;
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        SubscriptionComponent.ISubscriptionManager subscriptionManager;
        private IHttpContextAccessor httpContextAccessor;
        public IConfiguration Configuration { get; }
        public customerdataController(ILogger<customerdataController> _logger, IAuditTraillib _AuditTrail, IOrganizationManager _organizationmanager,IPreferenceManager _preferencemanager,IVehicleManager _vehicleManager,IHttpContextAccessor _httpContextAccessor,AccountComponent.IAccountIdentityManager _accountIdentityManager, SubscriptionComponent.ISubscriptionManager _subscriptionManager,IConfiguration configuration)
        {
           logger = _logger;
           AuditTrail = _AuditTrail;
           organizationtmanager = _organizationmanager;
           preferencemanager=_preferencemanager;
           vehicleManager=_vehicleManager;
           httpContextAccessor=_httpContextAccessor;
           accountIdentityManager=_accountIdentityManager;
           subscriptionManager=_subscriptionManager;
           Configuration=configuration;
        }

        [HttpPost]
        [Route("update")]        
        public async Task<IActionResult> update(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerRequest customerRequest = new CustomerRequest();
                    customerRequest.CompanyType = customer.CompanyUpdatedEvent.Company.type;
                    customerRequest.CustomerID = customer.CompanyUpdatedEvent.Company.ID;
                    customerRequest.CustomerName = customer.CompanyUpdatedEvent.Company.Name;
                    customerRequest.AddressType = customer.CompanyUpdatedEvent.Company.Address.Type;
                    customerRequest.Street = customer.CompanyUpdatedEvent.Company.Address.Street;
                    customerRequest.StreetNumber = customer.CompanyUpdatedEvent.Company.Address.StreetNumber;
                    customerRequest.PostalCode = customer.CompanyUpdatedEvent.Company.Address.PostalCode;
                    customerRequest.City = customer.CompanyUpdatedEvent.Company.Address.City;
                    customerRequest.CountryCode = customer.CompanyUpdatedEvent.Company.Address.CountryCode;
                    customerRequest.ReferenceDateTime = customer.CompanyUpdatedEvent.Company.ReferenceDateTime;

                    // Configuarable values   
                    customerRequest.OrgCreationPackage = Configuration.GetSection("DefaultSettings").GetSection("OrgCreationPackage").Value;

                    string dateformat = "yyyy-mm-ddThh:mm:ss";
                    DateTime parsed;
                    if (!(DateTime.TryParseExact(Convert.ToString(customerRequest.ReferenceDateTime), dateformat, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed)))
                    {
                        var regx = new Regex("[^a-zA-Z0-9_.]");
                        if ((
                                // Mandatory validation
                                (string.IsNullOrEmpty(customerRequest.CompanyType) || (customerRequest.CompanyType.Trim().Length < 1)) || (customerRequest.CompanyType.Trim().Length > 50)) //|| (regx.IsMatch(customerRequest.CompanyType))
                                || ((string.IsNullOrEmpty(customerRequest.CustomerID) || (customerRequest.CustomerID.Trim().Length < 1) || (customerRequest.CustomerID.Trim().Length > 100)) //|| (regx.IsMatch(customerRequest.CustomerID)))
                                || ((string.IsNullOrEmpty(customerRequest.CustomerName) || (customerRequest.CustomerName.Trim().Length < 1) || (customerRequest.CustomerName.Trim().Length > 100)) // || (regx.IsMatch(customerRequest.CustomerName))
                                || (string.IsNullOrEmpty(Convert.ToString(customerRequest.ReferenceDateTime).Trim())) || (Convert.ToString(customerRequest.ReferenceDateTime).Trim().Length < 1))

                                //Length validation
                                || ((customerRequest.AddressType.Trim().Length > 50))
                                || ((customerRequest.Street.Trim().Length > 50))
                                || ((customerRequest.StreetNumber.Trim().Length > 50))
                                || ((customerRequest.PostalCode.Trim().Length > 15))
                                || ((customerRequest.City.Trim().Length > 50))
                                || ((customerRequest.CountryCode.Trim().Length > 20))
                            ))
                        {
                            return StatusCode(400, string.Empty);
                        }
                        else
                        {
                            await organizationtmanager.UpdateCustomer(customerRequest);
                            logger.LogInformation("Customer data has been updated, company ID -" + customerRequest.CustomerID);
                            return Ok();
                        }
                    }
                    else
                    {
                        return StatusCode(400, string.Empty);
                    }
                }
                else
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                    return StatusCode(500, errors);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }            
        }

        [HttpPost]
        [Route("keyhandover")]
        public async Task<IActionResult> keyhandover(KeyHandOver keyHandOver)
        {
            try
            {
                HandOver objHandOver = new HandOver();
                objHandOver.VIN = keyHandOver.KeyHandOverEvent.VIN;
                objHandOver.TCUID = keyHandOver.KeyHandOverEvent.TCUID;
                objHandOver.TCUActivation = keyHandOver.KeyHandOverEvent.TCUActivation;
                objHandOver.ReferenceDateTime = keyHandOver.KeyHandOverEvent.ReferenceDateTime;
                objHandOver.CustomerID = keyHandOver.KeyHandOverEvent.EndCustomer.ID;
                objHandOver.CustomerName = keyHandOver.KeyHandOverEvent.EndCustomer.Name;
                objHandOver.Type = keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type;
                objHandOver.Street = keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street;
                objHandOver.StreetNumber = keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber;
                objHandOver.PostalCode = keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode;
                objHandOver.City = keyHandOver.KeyHandOverEvent.EndCustomer.Address.City;
                objHandOver.CountryCode = keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode;

                // Configuarable values                                       
                objHandOver.OwnerRelationship = Configuration.GetSection("DefaultSettings").GetSection("OwnerRelationship").Value;
                objHandOver.OEMRelationship = Configuration.GetSection("DefaultSettings").GetSection("OEMRelationship").Value;
                objHandOver.OrgCreationPackage = Configuration.GetSection("DefaultSettings").GetSection("OrgCreationPackage").Value;
                objHandOver.DAFPACCAR = Configuration.GetSection("DefaultSettings").GetSection("DAFPACCAR").Value;

                string dateformat = "yyyy-mm-ddThh:mm:ss";
                DateTime parsed;
                if (!(DateTime.TryParseExact(objHandOver.ReferenceDateTime, dateformat, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed)))
                {
                    var regx = new Regex("[^a-zA-Z0-9_.]");
                    if ((

                            //Mandetory validation 
                            (string.IsNullOrEmpty(objHandOver.VIN.Trim()) || (objHandOver.VIN.Trim().Length > 50)) || (objHandOver.VIN.Trim().Length < 0)) || (regx.IsMatch(objHandOver.VIN))
                            || (string.IsNullOrEmpty(objHandOver.TCUID.Trim()) || (objHandOver.TCUID.Trim().Length > 50) || (objHandOver.TCUID.Trim().Length < 1)// || (regx.IsMatch(objHandOver.TCUID))
                            || (string.IsNullOrEmpty(objHandOver.CustomerName.Trim()) || (objHandOver.CustomerName.Trim().Length > 100) || (objHandOver.CustomerName.Trim().Length < 1)// || (regx.IsMatch(objHandOver.CustomerName))
                            || (string.IsNullOrEmpty(objHandOver.ReferenceDateTime.Trim()) || (objHandOver.ReferenceDateTime.Trim().Length < 1))
                            //|| ((Convert.ToDateTime(objHandOver.ReferenceDateTime).ToUniversalTime()>System.DateTime.Now.ToUniversalTime())
                            || (!((objHandOver.TCUActivation.ToUpper() == "YES") || (objHandOver.TCUActivation.ToUpper() == "NO")))

                            //Length validation
                            || ((objHandOver.Type.Trim().Length > 50))
                            || ((objHandOver.Street.Trim().Length > 50))
                            || ((objHandOver.StreetNumber.Trim().Length > 50))
                            || ((objHandOver.PostalCode.Trim().Length > 15))
                            || ((objHandOver.City.Trim().Length > 50))
                            || ((objHandOver.CountryCode.Trim().Length > 20))

                    )))
                    {
                        return StatusCode(400, string.Empty);
                    }
                    else
                    {
                        await organizationtmanager.KeyHandOverEvent(objHandOver);
                        return Ok();
                    }
                }
                else
                {
                    return StatusCode(400, string.Empty);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }                               
        }      
    }
}
