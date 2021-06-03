using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.customerdataservice.CustomAttributes;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.organization.entity;

namespace net.atos.daf.ct2.customerdataservice.Controllers
{
    [ApiController]
    [Route("customer-data")]
    [Authorize(Policy = AccessPolicies.MAIN_ACCESS_POLICY)]
    public class CustomerDataController : ControllerBase
    {
        private readonly ILogger<CustomerDataController> _logger;
        private readonly IOrganizationManager _organizationManager;
        private readonly IConfiguration _configuration;
        public CustomerDataController(ILogger<CustomerDataController> logger, IOrganizationManager organizationmanager, IConfiguration configuration)
        {
            this._logger = logger;
            _organizationManager = organizationmanager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(Customer customer)
        {
            try
            {
                string dateformat = "yyyy-MM-ddTHH:mm:ss";
                if (DateTime.TryParseExact(customer.CompanyUpdatedEvent.Company.ReferenceDateTime.Trim(), dateformat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsedRefDateTime))
                {
                    if (parsedRefDateTime.ToUniversalTime() > DateTime.Now.ToUniversalTime())
                    {
                        return StatusCode(400, string.Empty);
                    }
                }
                else
                    return StatusCode(400, string.Empty);

                CustomerRequest customerRequest = new CustomerRequest
                {
                    CustomerID = customer.CompanyUpdatedEvent.Company.ID.Trim(),
                    ReferenceDateTime = parsedRefDateTime,
                    OrgCreationPackage = _configuration.GetSection("DefaultSettings").GetSection("OrgCreationPackage").Value,
                    CompanyType = customer.CompanyUpdatedEvent.Company.Type?.Trim(),
                    CustomerName = customer.CompanyUpdatedEvent.Company.Name?.Trim(),
                    AddressType = customer.CompanyUpdatedEvent.Company.Address?.Type?.Trim(),
                    Street = customer.CompanyUpdatedEvent.Company.Address?.Street?.Trim(),
                    StreetNumber = customer.CompanyUpdatedEvent.Company.Address?.StreetNumber?.Trim(),
                    PostalCode = customer.CompanyUpdatedEvent.Company.Address?.PostalCode?.Trim(),
                    City = customer.CompanyUpdatedEvent.Company.Address?.City?.Trim(),
                    CountryCode = customer.CompanyUpdatedEvent.Company.Address?.CountryCode?.Trim()
                };

                await _organizationManager.UpdateCustomer(customerRequest);
                _logger.LogInformation("Customer data has been updated, company ID -" + customerRequest.CustomerID);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpPost]
        [Route("keyhandover")]
        public async Task<IActionResult> KeyHandover(KeyHandOver keyHandOver)
        {
            try
            {
                if (!(keyHandOver.KeyHandOverEvent.TCUActivation.Trim().ToUpper().Equals("YES") || keyHandOver.KeyHandOverEvent.TCUActivation.Trim().ToUpper().Equals("NO")))
                {
                    return StatusCode(400, string.Empty);
                }

                string dateformat = "yyyy-MM-ddTHH:mm:ss";
                if (DateTime.TryParseExact(keyHandOver.KeyHandOverEvent.ReferenceDateTime.Trim(), dateformat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsedRefDateTime))
                {
                    if (parsedRefDateTime.ToUniversalTime() > DateTime.Now.ToUniversalTime())
                    {
                        return StatusCode(400, string.Empty);
                    }
                }
                else
                    return StatusCode(400, string.Empty);

                HandOver objHandOver = new HandOver
                {
                    VIN = keyHandOver.KeyHandOverEvent.VIN.Trim(),
                    TCUID = keyHandOver.KeyHandOverEvent.TCUID.Trim(),
                    TCUActivation = keyHandOver.KeyHandOverEvent.TCUActivation.Trim(),
                    CustomerID = keyHandOver.KeyHandOverEvent.EndCustomer.ID.Trim(),

                    // Configuarable values                                       
                    OwnerRelationship = _configuration.GetSection("DefaultSettings").GetSection("OwnerRelationship").Value,
                    OEMRelationship = _configuration.GetSection("DefaultSettings").GetSection("OEMRelationship").Value,
                    OrgCreationPackage = _configuration.GetSection("DefaultSettings").GetSection("OrgCreationPackage").Value,
                    DAFPACCAR = _configuration.GetSection("DefaultSettings").GetSection("DAFPACCAR").Value,

                    ReferenceDateTime = parsedRefDateTime,
                    CustomerName = keyHandOver.KeyHandOverEvent.EndCustomer.Name?.Trim(),

                    Type = keyHandOver.KeyHandOverEvent.EndCustomer.Address?.Type?.Trim(),
                    Street = keyHandOver.KeyHandOverEvent.EndCustomer.Address?.Street?.Trim(),
                    StreetNumber = keyHandOver.KeyHandOverEvent.EndCustomer.Address?.StreetNumber?.Trim(),
                    PostalCode = keyHandOver.KeyHandOverEvent.EndCustomer.Address?.PostalCode?.Trim(),
                    City = keyHandOver.KeyHandOverEvent.EndCustomer.Address?.City?.Trim(),
                    CountryCode = keyHandOver.KeyHandOverEvent.EndCustomer.Address?.CountryCode?.Trim()
                };

                await _organizationManager.KeyHandOverEvent(objHandOver);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }
        }
    }
}
