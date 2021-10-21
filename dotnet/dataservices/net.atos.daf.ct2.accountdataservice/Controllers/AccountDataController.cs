using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.audit;
using System;
using log4net;
using System.Net;
using System.Threading.Tasks;
using System.Reflection;
using net.atos.daf.ct2.accountdataservice.Entity;
using net.atos.daf.ct2.audit.Enum;
using System.Linq;
using Newtonsoft.Json;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.vehicle;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.accountdataservice.CustomAttributes;
using net.atos.daf.ct2.driver;
using System.Text;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.identity.Common;
using System.Text.RegularExpressions;
using IdentityComponent = net.atos.daf.ct2.identity;

namespace net.atos.daf.ct2.accountdataservice.Controllers
{
    [ApiController]
    [Route("account")]
    [Authorize(Policy = AccessPolicies.MAIN_ACCESS_POLICY)]
    public class AccountDataController : ControllerBase
    {
        private readonly IAuditTraillib _auditTrail;
        private readonly ILog _logger;
        private readonly IAccountManager _accountManager;
        private readonly IAccountIdentityManager _accountIdentityManager;
        private readonly IOrganizationManager _organizationManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IDriverManager _driverManager;
        private readonly IConfiguration _configuration;
        private readonly IdentityComponent.IAccountAuthenticator _autheticator;

        public AccountDataController(IAuditTraillib auditTrail, IDriverManager driverManager, IAccountManager accountManager,
                                     IOrganizationManager organizationManager, IVehicleManager vehicleManager,
                                     IAccountIdentityManager accountIdentityManager, IConfiguration configuration,
                                     IdentityComponent.IAccountAuthenticator autheticator)
        {
            _accountManager = accountManager;
            _accountIdentityManager = accountIdentityManager;
            _organizationManager = organizationManager;
            _vehicleManager = vehicleManager;
            _driverManager = driverManager;
            _auditTrail = auditTrail;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _configuration = configuration;
            _autheticator = autheticator;
        }

        #region Driver Lookup

        [HttpGet]
        [Route("driver-lookup")]
        public async Task<IActionResult> GetDriver([FromQuery] DriverLookupRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(GetDriver), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get driver in Account data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var response = await _driverManager.GetDriver(request.DriverId, request.Email);

                if (response.DriverLookup.Count() == 0)
                {
                    return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.DriverId));
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account API - Driver Lookup request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(GetDriver), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get driver in Account data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Register Driver

        [HttpPost]
        [Route("register/driver")]
        public async Task<IActionResult> RegisterDriver([FromBody] DriverRegisterRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(RegisterDriver), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Register driver in Account data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is OkObjectResult)
                {
                    var resultObj = (result as ObjectResult).Value as dynamic;
                    var response = await _accountManager.RegisterDriver(new RegisterDriverDataServiceRequest
                    {
                        AccountEmail = resultObj.Email,
                        Password = resultObj.Password,
                        DriverId = request.DriverId,
                        OrganisationId = resultObj.OrgId,
                        IsLoginSuccessful = resultObj.IsLoginSuccessful
                    });

                    if (response.StatusCode == HttpStatusCode.OK)
                        return Ok();
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: response.Message, parameter: nameof(request.DriverId));
                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: response.Message, parameter: nameof(request.Authorization));
                    else
                    {
                        _logger.Error($"Account API - Register Driver request was unsuccessful. { response.StatusCode } - { response.Message }");
                        return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "NOT_VALIDATED", parameter: nameof(request.Authorization));
                    }
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account API - Register Driver request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(RegisterDriver), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Register driver in Account data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Validate Driver

        [HttpPost]
        [Route("validate/driver")]
        public async Task<IActionResult> ValidateDriver([FromBody] DriverValidateRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(ValidateDriver), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Validate driver in Account data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is OkObjectResult)
                {
                    var resultObj = (result as ObjectResult).Value as dynamic;
                    var response = await _accountManager.ValidateDriver(resultObj.Email, resultObj.OrgId);

                    if (resultObj.OrgId > 0)
                    {
                        return Ok(new
                        {
                            AccountID = response.AccountID,
                            AccountName = response.AccountName,
                            RoleID = response.RoleID,
                            DateFormat = response.DateFormat,
                            TimeFormat = response.TimeFormat,
                            TimeZone = response.TimeZone,
                            UnitDisplay = response.UnitDisplay,
                            VehicleDisplay = response.VehicleDisplay,
                            Language = response.Language
                        });
                    }
                    else
                        return Ok(response);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account API - Validate Driver request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(ValidateDriver), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Validate driver in Account data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Password

        [HttpPost]
        [Route("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(ChangePassword), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Change password in Account data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is OkObjectResult)
                {
                    var identity = (string)(result as ObjectResult).Value;
                    Account account = new Account();
                    account.EmailId = identity.Split(":")[0];
                    account.Password = identity.Split(":")[1];
                    var identityResult = await _accountManager.ChangePassword(account);
                    if (identityResult.StatusCode == HttpStatusCode.NoContent)
                        return Ok();
                    else if (identityResult.StatusCode == HttpStatusCode.BadRequest || identityResult.StatusCode == HttpStatusCode.Forbidden)
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "PASSWORD_NON_COMPLIANT", nameof(request.NewAuthorization));
                    else
                        return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "NOT_VALIDATED", parameter: nameof(request.Authorization));
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account API - Change password request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(ChangePassword), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Change password in Account data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpPost]
        [Route("password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(ResetPassword), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Reset password in Account data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var response = await _accountManager.ResetPasswordInitiate(request.AccountId);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.Error("Account API - Reset password request was unsuccessful. " + JsonConvert.SerializeObject(response));
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account API - Reset password request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(ResetPassword), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Reset password in Account data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Preferences

        [HttpGet]
        [Route("preferences")]
        public async Task<IActionResult> GetPreferences([FromQuery] GetPreferencesRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(GetPreferences), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get preferences in Account data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is OkObjectResult)
                {
                    var orgId = (int)(result as ObjectResult).Value;
                    var response = await _accountManager.GetAccountPreferences(request.AccountId, orgId);

                    return Ok(response);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account API - Get preferences request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(GetPreferences), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Get preferences in Account data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpPost]
        [Route("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(UpdatePreferences), AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.PARTIAL, "Update preferences in Account data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is NoContentResult)
                {
                    await _accountManager.UpdateAccountPreferences(MapRequest(request));
                    return Ok();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account API - Update preferences request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(UpdatePreferences), AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "Update preferences in Account data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Model Validations

        private async Task<IActionResult> ValidateParameters(DriverRegisterRequest request)
        {
            string identity;
            try
            {
                identity = Encoding.UTF8.GetString(Convert.FromBase64String(request.Authorization));
            }
            catch (Exception)
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.Authorization));
            }

            if (identity.Split(":").Count() != 2)
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.Authorization));
            }

            if (string.IsNullOrEmpty(identity.Split(":")[0].Trim()) || string.IsNullOrEmpty(identity.Split(":")[1].Trim()))
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.Authorization));
            }

            var email = identity.Split(":")[0].Trim();
            var password = identity.Split(":")[1].Trim();

            if (!IdentityUtilities.ValidationByRegex(new Regex(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[""''!*@#$%^&+=~`^()\\/-_;:<>|{}\[\]]).{10,256})"), password))
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "PASSWORD_NON_COMPLIANT", parameter: nameof(request.Authorization));
            }

            var accountIdentity = await _accountIdentityManager.ValidateUser(new Identity { UserName = email, Password = password });

            var org = await _organizationManager.GetOrganizationByOrgCode(request.OrganisationId);

            var isExists = await _driverManager.CheckIfDriverExists(request.DriverId, org?.Id ?? 0, email);
            if (!isExists)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.DriverId));

            return new OkObjectResult(
                new
                {
                    Email = email,
                    Password = password,
                    OrgId = org?.Id ?? 0,
                    IsLoginSuccessful = (accountIdentity != null && !string.IsNullOrEmpty(accountIdentity.TokenIdentifier))
                });
        }

        private async Task<IActionResult> ValidateParameters(DriverValidateRequest request)
        {
            string identity;
            try
            {
                identity = Encoding.UTF8.GetString(Convert.FromBase64String(request.Authorization));
            }
            catch (Exception)
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.Authorization));
            }

            if (identity.Split(":").Count() != 2)
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.Authorization));
            }

            if (string.IsNullOrEmpty(identity.Split(":")[0].Trim()) || string.IsNullOrEmpty(identity.Split(":")[1].Trim()))
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.Authorization));
            }

            var email = identity.Split(":")[0].Trim();
            var password = identity.Split(":")[1].Trim();

            int? orgId = null;
            if (!string.IsNullOrEmpty(request.OrganisationId))
            {
                var org = await _organizationManager.GetOrganizationByOrgCode(request.OrganisationId);
                orgId = org?.Id ?? 0;
            }

            var isExists = await _driverManager.CheckIfDriverExists(request.DriverId, orgId, email);
            if (!isExists)
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "NOT_VALIDATED", parameter: string.Empty);

            var accountIdentity = await _accountIdentityManager.ValidateUser(new Identity { UserName = email, Password = password });
            if (accountIdentity == null || (accountIdentity != null && string.IsNullOrEmpty(accountIdentity.TokenIdentifier)))
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "NOT_VALIDATED", parameter: string.Empty);

            return new OkObjectResult(new { Email = email, OrgId = orgId ?? 0 });
        }

        private async Task<IActionResult> ValidateParameters(ChangePasswordRequest request)
        {
            string identity;
            try
            {
                identity = Encoding.UTF8.GetString(Convert.FromBase64String(request.Authorization));
            }
            catch (Exception)
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.Authorization));
            }

            if (identity.Split(":").Count() != 2)
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.Authorization));
            }

            if (string.IsNullOrEmpty(identity.Split(":")[0].Trim()) || string.IsNullOrEmpty(identity.Split(":")[1].Trim()))
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.Authorization));
            }

            string newIdentity;
            try
            {
                newIdentity = Encoding.UTF8.GetString(Convert.FromBase64String(request.NewAuthorization));
            }
            catch (Exception)
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.NewAuthorization));
            }

            if (newIdentity.Split(":").Count() != 2)
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.NewAuthorization));
            }

            if (string.IsNullOrEmpty(newIdentity.Split(":")[0].Trim()) || string.IsNullOrEmpty(newIdentity.Split(":")[1].Trim()))
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.NewAuthorization));
            }

            var email = identity.Split(":")[0].Trim();
            var password = identity.Split(":")[1].Trim();

            if (email.ToLower().Equals(newIdentity.Split(":")[0].Trim().ToLower()))
            {
                var accountIdentity = await _accountIdentityManager.ValidateUser(new Identity { UserName = email, Password = password });
                if (accountIdentity == null || (accountIdentity != null && string.IsNullOrEmpty(accountIdentity.TokenIdentifier)))
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "NOT_VALIDATED", parameter: nameof(request.Authorization));
            }
            else
            {
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_FIELD", parameter: nameof(request.NewAuthorization));
            }

            return new OkObjectResult(newIdentity);
        }

        private async Task<IActionResult> ValidateParameters(GetPreferencesRequest request)
        {
            var account = await _accountManager.GetAccountByEmailId(request.AccountId);
            if (account == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.AccountId));

            if (string.IsNullOrEmpty(account.DriverId) || (!string.IsNullOrEmpty(account.DriverId) && !account.DriverId.Equals(request.DriverId)))
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.DriverId));

            var org = await _organizationManager.GetOrganizationByOrgCode(request.OrganisationId);
            if (org == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.OrganisationId));

            var orgs = await _accountManager.GetAccountOrg(account.Id);
            if (!orgs.Select(x => x.Id).ToArray().Contains(org.Id))
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.AccountId));

            return new OkObjectResult(org.Id);
        }

        private async Task<IActionResult> ValidateParameters(UpdatePreferencesRequest request)
        {
            var account = await _accountManager.GetAccountByEmailId(request.AccountId);
            if (account == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.AccountId));

            if (string.IsNullOrEmpty(account.DriverId) || (!string.IsNullOrEmpty(account.DriverId) && !account.DriverId.Equals(request.DriverId)))
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.DriverId));

            return NoContent();
        }

        #endregion

        #region Map Request

        private UpdatePreferencesDataServiceRequest MapRequest(UpdatePreferencesRequest request)
        {
            return new UpdatePreferencesDataServiceRequest
            {
                AccountEmail = request.AccountId,
                DriverId = request.DriverId,
                Language = request.Language,
                DateFormat = request.DateFormat,
                TimeFormat = request.TimeFormat,
                TimeZone = request.TimeZone,
                UnitDisplay = request.UnitDisplay,
                VehicleDisplay = request.VehicleDisplay.ToLower()
            };
        }

        #endregion

        #region Generate Error Response

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string errorCode, string parameter)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = errorCode,
                Value = parameter
            });
        }

        #endregion
    }
}
