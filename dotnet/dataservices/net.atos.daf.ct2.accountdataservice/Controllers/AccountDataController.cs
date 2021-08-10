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
using net.atos.daf.ct2.vehicle.entity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.accountdataservice.CustomAttributes;
using net.atos.daf.ct2.driver.entity;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.driver;
using System.Text;
using net.atos.daf.ct2.identity.entity;

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
        public AccountDataController(IAuditTraillib auditTrail, IDriverManager driverManager, IAccountManager accountManager, IOrganizationManager organizationManager, IVehicleManager vehicleManager, IAccountIdentityManager accountIdentityManager, IConfiguration configuration)
        {
            _accountManager = accountManager;
            _accountIdentityManager = accountIdentityManager;
            _organizationManager = organizationManager;
            _vehicleManager = vehicleManager;
            _driverManager = driverManager;
            _auditTrail = auditTrail;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _configuration = configuration;
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
                _logger.Error("Error occurred while processing Account - Driver Lookup request.", ex);
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
                    var identity = (string)(result as ObjectResult).Value;
                    //var response = await _driverManager.RegisterDriver(MapRequest(request, identity));

                    return Ok();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account - Register Driver request.", ex);
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
                    var identity = (string)(result as ObjectResult).Value;
                    //var response = await _driverManager.ValidateDriver(MapRequest(request, identity));

                    return Ok();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account - Register Driver request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(ValidateDriver), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Validate driver in Account data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Password

        [HttpPost]
        [Route("password/change")]
        public async Task<IActionResult> ChangePassword([FromQuery] ChangePasswordRequest request)
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
                    //var response = await _driverManager.GetCurrentDriver(MapRequest(request, (int)(result as ObjectResult).Value));

                    return Ok();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account - Change password request.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(ChangePassword), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.FAILED, "Change password in Account data service", 0, 0, ex.Message, 0, 0);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpGet]
        [Route("password/reset")]
        public async Task<IActionResult> ResetPassword([FromQuery] ResetPasswordRequest request)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Account Data Service", nameof(ResetPassword), AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Reset password in Account data service", 0, 0, JsonConvert.SerializeObject(request), 0, 0);

                if (!ModelState.IsValid)
                {
                    var modelState = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: modelState.Value.Errors.First().ErrorMessage, parameter: modelState.Key);
                }

                var result = await ValidateParameters(request);
                if (result is NoContentResult)
                {
                    //var response = await _driverManager.GetDriverList(MapRequest(request));

                    return Ok();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account - Reset password request.", ex);
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
                if (result is NoContentResult)
                {
                    //var response = await _driverManager.GetCurrentDriver(MapRequest(request));

                    return Ok();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account - Get preferences request.", ex);
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
                    //var response = await _driverManager.GetDriverList(MapRequest(request));

                    return Ok();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while processing Account - Update preferences request.", ex);
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
            var isExists = await _driverManager.CheckIfDriverExists(request.DriverId, request.OrganisationId, email);
            if (!isExists)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "DRIVER_NOT_FOUND", parameter: nameof(request.DriverId));

            return new OkObjectResult(identity);
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
            var isExists = await _driverManager.CheckIfDriverExists(request.DriverId, request.OrganisationId, email);
            if (!isExists)
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "NOT_VALIDATED", parameter: nameof(request.DriverId));

            return new OkObjectResult(identity);
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

            var email = identity.Split(":")[0].Trim();
            var password = identity.Split(":")[1].Trim();
            var accountIdentity = await _accountIdentityManager.Login(new Identity { UserName = email, Password = password });
            if (accountIdentity != null && !string.IsNullOrEmpty(accountIdentity.TokenIdentifier))
                return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "NOT_VALIDATED", parameter: nameof(request.Authorization));

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

            return new OkObjectResult(newIdentity);
        }

        private async Task<IActionResult> ValidateParameters(ResetPasswordRequest request)
        {
            var account = await _accountManager.GetAccountByEmailId(request.AccountId);
            if (account == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.AccountId));

            return NoContent();
        }

        private async Task<IActionResult> ValidateParameters(GetPreferencesRequest request)
        {
            var account = await _accountManager.GetAccountByEmailId(request.AccountId);
            if (account == null)
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.AccountId));

            if (string.IsNullOrEmpty(account.DriverId) || (!string.IsNullOrEmpty(account.DriverId) && !account.DriverId.Equals(request.DriverId)))
                return GenerateErrorResponse(HttpStatusCode.NotFound, errorCode: "ACCOUNT_NOT_FOUND", parameter: nameof(request.DriverId));

            return NoContent();
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

        #region Map Requests

        private RegisterDriverDataServiceRequest MapRequest(DriverRegisterRequest request, string identity)
        {
            return new RegisterDriverDataServiceRequest
            {
                OrganisationId = request.OrganisationId,
                DriverId = request.DriverId,
                Username = identity.Split(":")[0],
                Password = identity.Split(":")[1]
            };
        }

        private RegisterDriverDataServiceRequest MapRequest(DriverValidateRequest request, string identity)
        {
            return new RegisterDriverDataServiceRequest
            {
                OrganisationId = request.OrganisationId,
                DriverId = request.DriverId,
                Username = identity.Split(":")[0],
                Password = identity.Split(":")[1]
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
