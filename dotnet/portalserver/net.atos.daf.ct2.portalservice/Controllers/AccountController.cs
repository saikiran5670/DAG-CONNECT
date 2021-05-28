using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Account;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;
using log4net;
using AccountBusinessService = net.atos.daf.ct2.accountservice;
using System.Reflection;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("account")]
    public class AccountController : BaseController
    {
        #region Private Variable
        private readonly AuditHelper _auditHelper;
        private readonly AccountBusinessService.AccountService.AccountServiceClient _accountClient;
        private readonly Mapper _mapper;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;

        private readonly ILog _logger;
        private readonly IMemoryCacheExtensions _cache;

        #endregion

        #region Constructor
        public AccountController(AccountBusinessService.AccountService.AccountServiceClient accountClient, IMemoryCacheExtensions cache,
             AuditHelper auditHelper, IHttpContextAccessor _httpContextAccessor, SessionHelper sessionHelper, Common.AccountPrivilegeChecker privilegeChecker) : base(_httpContextAccessor, sessionHelper)
        {
            _accountClient = accountClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _mapper = new Mapper();
            _cache = cache;
            _auditHelper = auditHelper;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
            _privilegeChecker = privilegeChecker;
        }
        #endregion

        #region Account
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(AccountRequest request)
        {
            try
            {
                var accountResponse = new AccountBusinessService.AccountData();
                var accountPreferenceResponse = new AccountBusinessService.AccountPreferenceResponse();
                var preferenceRequest = new AccountBusinessService.AccountPreferenceFilter();
                // Validation 
                if ((string.IsNullOrEmpty(request.EmailId)) || (string.IsNullOrEmpty(request.FirstName))
                || (string.IsNullOrEmpty(request.LastName)) || (request.OrganizationId <= 0) || (string.IsNullOrEmpty(request.Type)))
                {
                    return StatusCode(400, PortalConstants.AccountValidation.CreateRequired);
                }
                // Length validation
                if ((request.EmailId.Length > 50) || (request.FirstName.Length > 30)
                || (request.LastName.Length > 20) || !Int32.TryParse(request.OrganizationId.ToString(), out int validOrgId))
                {
                    return StatusCode(400, PortalConstants.AccountValidation.InvalidData);
                }
                // The account type should be single character
                if (request.Type.Length > 1)
                {
                    return StatusCode(400, PortalConstants.AccountValidation.InvalidAccountType);
                }
                // validate account type
                char accountType = Convert.ToChar(request.Type);
                if (!EnumValidator.ValidateAccountType(accountType))
                {
                    return StatusCode(400, PortalConstants.AccountValidation.InvalidAccountType);
                }
                var accountRequest = _mapper.ToAccount(request);
                accountRequest.OrganizationId = GetContextOrgId();

                accountResponse = await _accountClient.CreateAsync(accountRequest);
                AccountResponse response = new AccountResponse();
                response = _mapper.ToAccount(accountResponse.Account);
                if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Conflict)
                {
                    var accountPreference = new AccountPreference();
                    accountPreference.Preference = null;
                    accountPreference.Account = response;

                    preferenceRequest.Id = response.PreferenceId;
                    // get preference
                    accountPreferenceResponse = await _accountClient.GetPreferenceAsync(preferenceRequest);
                    if (accountPreferenceResponse != null && accountPreferenceResponse.Code == AccountBusinessService.Responcecode.Success)
                    {
                        if (accountPreferenceResponse.AccountPreference != null)
                        {
                            accountPreference.Preference = new AccountPreferenceResponse();
                            accountPreference.Preference = _mapper.ToAccountPreference(accountPreferenceResponse.AccountPreference);
                        }
                    }
                    return StatusCode(409, accountPreference);
                }
                else if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Success)
                {

                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                           "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "Create Account method in Account controller", 0, accountResponse.Account.Id, JsonConvert.SerializeObject(request),
                                            Request);
                    return Ok(response);
                }
                else
                {
                    if (accountResponse.Message == PortalConstants.AccountValidation.ErrorMessage)
                    {
                        return StatusCode(500, PortalConstants.AccountValidation.ErrorMessage);
                    }
                    else if (accountResponse.Message == PortalConstants.AccountValidation.EmailSendingFailedMessage)
                    {
                        return StatusCode(500, PortalConstants.AccountValidation.EmailSendingFailedMessage);
                    }
                    else
                    {
                        return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                    }
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                          "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                          "Create Account method in Account controller", 0, 0, JsonConvert.SerializeObject(request),
                                           Request);

                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "02"));
                }
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.SocketException))
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "03"));
                }
                return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "04"));
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(AccountRequest request)
        {
            try
            {
                bool isSameEmail = false;

                // Validation 
                if ((request.Id <= 0) || (string.IsNullOrEmpty(request.EmailId))
                    || (string.IsNullOrEmpty(request.FirstName)) || (string.IsNullOrEmpty(request.LastName)))
                {
                    return StatusCode(400, "The AccountId, EmailId address, first name, last name is required.");
                }
                // Length validation
                if ((request.EmailId.Length > 50) || (request.FirstName.Length > 30)
                || (request.LastName.Length > 20) || !Int32.TryParse(request.OrganizationId.ToString(), out int validOrgId))
                {
                    return StatusCode(400, "The EmailId address, first name, last name and organization id should be valid.");
                }
                // The account type should be single character
                if (string.IsNullOrEmpty(request.Type) || request.Type.Length > 1)
                {
                    return StatusCode(400, PortalConstants.AccountValidation.InvalidAccountType);
                }
                request.Type = request.Type.ToUpper();
                // validate account type
                char accountType = Convert.ToChar(request.Type);
                if (!EnumValidator.ValidateAccountType(accountType))
                {
                    return StatusCode(400, PortalConstants.AccountValidation.InvalidAccountType);
                }
                AccountBusinessService.AccountFilter accFilter = new AccountBusinessService.AccountFilter();
                accFilter.Id = request.Id;
                AccountBusinessService.AccountDataList accountList = await _accountClient.GetAsync(accFilter);
                foreach (AccountBusinessService.AccountRequest entity in accountList.Accounts)
                {
                    if (entity.EmailId.ToUpper() == request.EmailId.ToUpper())
                    {
                        isSameEmail = true;
                    }
                    //As filtering by id, hence only one row is expected.
                    break;
                }
                if (!isSameEmail)
                {
                    return StatusCode(400, PortalConstants.AccountValidation.EmailUpdateNotAllowed);
                }
                var accountResponse = new AccountBusinessService.AccountData();
                var accountRequest = new AccountBusinessService.AccountRequest();
                accountRequest = _mapper.ToAccount(request);
                accountRequest.OrganizationId = AssignOrgContextByAccountId(request.Id);

                accountResponse = await _accountClient.UpdateAsync(accountRequest);
                if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Failed)
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
                else if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Success)
                {

                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                           "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "Update Account method in Account controller", accountResponse.Account.Id, accountResponse.Account.Id, JsonConvert.SerializeObject(request),
                                            Request);
                    return Ok(request);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "02"));
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                           "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                           "Update Account method in Account controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                                            Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "03"));
                }
                return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "04"));
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(string EmailId, int AccountId, int OrganizationId)
        {
            var response = new AccountBusinessService.AccountResponse();
            AccountBusinessService.AccountRequest accountRequest = new AccountBusinessService.AccountRequest();
            try
            {
                // Validation                 
                if ((string.IsNullOrEmpty(EmailId)) || (Convert.ToInt32(AccountId) <= 0) || (Convert.ToInt32(OrganizationId) <= 0))
                {
                    return StatusCode(400, "The Email address, account id and organization id is required.");
                }

                accountRequest.Id = AccountId;
                accountRequest.EmailId = EmailId;
                accountRequest.OrganizationId = AssignOrgContextByAccountId(AccountId);
                response = await _accountClient.DeleteAsync(accountRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                           "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "Update Account method in Account controller", AccountId, AccountId, JsonConvert.SerializeObject(accountRequest),
                                            Request);
                    return Ok(accountRequest);
                }
                else
                    return StatusCode(404, "Account not configured.");
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                         "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "Delete Account method in Account controller", AccountId, AccountId, JsonConvert.SerializeObject(accountRequest),
                                          Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.EmailId) || string.IsNullOrEmpty(request.Password))
                {
                    return StatusCode(404, "The Email address and password is required.");
                }
                AccountBusinessService.ChangePasswordRequest changePasswordRequest = new AccountBusinessService.ChangePasswordRequest();
                changePasswordRequest.EmailId = request.EmailId;
                changePasswordRequest.Password = request.Password;
                changePasswordRequest.OrgId = GetUserSelectedOrgId();
                var response = await _accountClient.ChangePasswordAsync(changePasswordRequest);
                if (response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                             "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "ChangePassword  method in Account controller", _userDetails.accountId, _userDetails.accountId, JsonConvert.SerializeObject(request),
                                              Request);//accountid requred here
                    return Ok("Password has been changed.");

                }
                else if (response.Code == AccountBusinessService.Responcecode.BadRequest)
                    return BadRequest(response.Message);
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NotFound(response.Message);
                else if (response.Code == AccountBusinessService.Responcecode.Forbidden)
                    return StatusCode(403, response.Message);
                else
                    return StatusCode(500, "Account not configured or failed to change password.");
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                             "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                             "ChangePassword  method in Account controller", _userDetails.accountId, _userDetails.accountId, JsonConvert.SerializeObject(request),
                                              Request);//accountid requred here
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Get(AccountFilterRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Name)
                    && (request.Id <= 0) && (request.OrganizationId <= 0) && (string.IsNullOrEmpty(request.AccountIds))
                    && (request.AccountGroupId <= 0)
                    )
                {
                    return StatusCode(404, "One of the parameter to filter account is required.");
                }
                AccountBusinessService.AccountFilter accountFilter = new AccountBusinessService.AccountFilter();
                accountFilter = _mapper.ToAccountFilter(request);
                accountFilter.OrganizationId = AssignOrgContextByAccountId(request.Id);
                AccountBusinessService.AccountDataList accountResponse = await _accountClient.GetAsync(accountFilter);
                List<AccountResponse> response = new List<AccountResponse>();
                response = _mapper.ToAccounts(accountResponse);

                if (response != null && accountResponse.Code == AccountBusinessService.Responcecode.Success)
                {
                    if (accountResponse.Accounts != null && accountResponse.Accounts.Count > 0)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(404, "Account not configured.");
                    }
                }
                else
                {
                    return StatusCode(500, "Internal Server Error");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("getaccountdetail")]
        public async Task<IActionResult> GetAccountDetail(AccountDetailRequest request)
        {
            try
            {
                // Validation                 
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The organization is required");
                }
                AccountBusinessService.AccountGroupDetailsRequest accountRequest = new AccountBusinessService.AccountGroupDetailsRequest();
                accountRequest = _mapper.ToAccountDetailsFilter(request);
                accountRequest.OrganizationId = AssignOrgContextByAccountId(request.AccountId);
                AccountBusinessService.AccountDetailsResponse accountResponse = await _accountClient.GetAccountDetailAsync(accountRequest);

                if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Success)
                {
                    if (accountResponse.AccountDetails != null && accountResponse.AccountDetails.Count > 0)
                    {
                        return Ok(_mapper.ToAccountDetailsResponse(accountResponse));
                    }
                    else
                    {
                        return StatusCode(404, "Account not configured.");
                    }
                }
                else
                {
                    return StatusCode(500, PortalConstants.ResponseError.InternalServerError + "01");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, PortalConstants.ResponseError.InternalServerError + "02");
            }
        }

        [HttpPost]
        [Route("organization/add")]
        public async Task<IActionResult> AddAccountOrg(AccountOrganizationRequest request)
        {
            try
            {
                // Validation 
                if ((request.OrganizationId <= 0) || (request.AccountId <= 0))
                {
                    return StatusCode(400, "The organization id and account id is required.");
                }

                var accountRequest = new AccountBusinessService.AccountOrganization();
                accountRequest.OrganizationId = AssignOrgContextByAccountId(request.AccountId);
                accountRequest.AccountId = request.AccountId;
                accountRequest.StartDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                accountRequest.EndDate = 0;
                var response = new AccountBusinessService.AccountOrganizationResponse();
                response = await _accountClient.AddAccountToOrgAsync(accountRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Failed)
                {
                    return StatusCode(500, "Internal Server Error.(0)");
                }
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                            "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                            "AddAccountOrg  method in Account controller", request.OrganizationId, response.AccountOrgId, JsonConvert.SerializeObject(request),
                                             Request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                            "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                            "AddAccountOrg  method in Account controller", 0, 0, JsonConvert.SerializeObject(request),
                                             Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("resetpasswordinitiate")]
        public async Task<IActionResult> ResetPasswordInitiate([FromBody] ResetPasswordInitiateRequest request)
        {
            try
            {
                var resetPasswordInitiateRequest = new AccountBusinessService.ResetPasswordInitiateRequest();
                resetPasswordInitiateRequest.EmailId = request.EmailId;
                var response = await _accountClient.ResetPasswordInitiateAsync(resetPasswordInitiateRequest);
                if (response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                           "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "ResetPasswordInitiate  method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                           JsonConvert.SerializeObject(request), Request);
                    return Ok(response.Message);
                }
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NotFound(response.Message);
                else
                    return StatusCode(500, "Password reset process failed to initiate or Error while sending email.");
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                          "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                          "ResetPasswordInitiate  method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                          JsonConvert.SerializeObject(request), Request);
                _logger.Error(null, ex);
                return StatusCode(500, "Error while initiating reset password process.");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getresetpasswordtokenstatus")]
        public async Task<IActionResult> GetResetPasswordTokenStatus([FromQuery] GetResetPasswordTokenStatusRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.ProcessToken, out _))
                {
                    return BadRequest($"{nameof(request.ProcessToken)} field is tampered or has invalid value.");
                }
                var resetPasswordTokenStatusRequest = new AccountBusinessService.GetResetPasswordTokenStatusRequest();
                resetPasswordTokenStatusRequest.ProcessToken = request.ProcessToken;

                var response = await _accountClient.GetResetPasswordTokenStatusAsync(resetPasswordTokenStatusRequest);

                if (response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                        "GetResetPasswordTokenStatus method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                        JsonConvert.SerializeObject(request), Request);
                    return Ok(response.Message);
                }
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NotFound(response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                        "GetResetPasswordTokenStatus  method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                        JsonConvert.SerializeObject(request), Request);
                _logger.Error(null, ex);
                return StatusCode(500, "Error while fetching reset password token status.");
            }
            return StatusCode(500, "Error while fetching reset password token status.");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("resetpassword")]
        [Route("createpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.ProcessToken, out _))
                {
                    return BadRequest($"{nameof(request.ProcessToken)} field is tampered or has invalid value.");
                }
                var resetPasswordRequest = new AccountBusinessService.ResetPasswordRequest();
                resetPasswordRequest.ProcessToken = request.ProcessToken;
                resetPasswordRequest.Password = request.Password;
                var response = await _accountClient.ResetPasswordAsync(resetPasswordRequest);
                if (response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                        "ResetPassword  method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                        JsonConvert.SerializeObject(request), Request);
                    return Ok("Reset password process is successfully completed.");
                }
                else if (response.Code == AccountBusinessService.Responcecode.BadRequest)
                    return BadRequest(response.Message);
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NotFound(response.Message);
                else
                    return StatusCode(500, "Reset password process failed.");
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                        "ResetPassword  method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                        JsonConvert.SerializeObject(request), Request);
                _logger.Error(null, ex);
                return StatusCode(500, "Error while reseting account password");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("resetpasswordinvalidate")]
        public async Task<IActionResult> ResetPasswordInvalidate([FromBody] ResetPasswordInvalidateRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.ResetToken, out _))
                {
                    return BadRequest($"{nameof(request.ResetToken)} field is tampered or has invalid value.");
                }
                var resetPasswordInvalidateRequest = new AccountBusinessService.ResetPasswordInvalidateRequest();
                resetPasswordInvalidateRequest.ResetToken = request.ResetToken;

                var response = await _accountClient.ResetPasswordInvalidateAsync(resetPasswordInvalidateRequest);

                if (response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                        "ResetPasswordInvalidate  method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                        JsonConvert.SerializeObject(request), Request);
                    return Ok(response.Message);
                }
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NotFound(response.Message);
                else
                    return StatusCode(500, "Reset password invalidate process failed.");
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                        "ResetPasswordInvalidate  method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                        JsonConvert.SerializeObject(request), Request);
                _logger.Error(null, ex);
                return StatusCode(500, "Error while invalidating reset password invalidate process.");
            }
        }

        [HttpPost]
        [Route("getmenufeatures")]
        public async Task<IActionResult> GetMenuFeatures([FromBody] MenuFeatureRequest request)
        {
            try
            {
                var menuFeatureRequest = new AccountBusinessService.MenuFeatureRequest
                {
                    AccountId = request.AccountId,
                    RoleId = request.RoleId,
                    OrganizationId = request.OrganizationId,
                    LanguageCode = request.LanguageCode,
                    ContextOrgId = GetContextOrgId()
                };

                var response = await _accountClient.GetMenuFeaturesAsync(menuFeatureRequest);

                if (response.Code == AccountBusinessService.Responcecode.Success)
                    return Ok(response.MenuFeatures);
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NoContent();
                else
                    return StatusCode(500, "Error occurred while fetching menu items and features.");
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Account Picture

        // Save Profile Picture
        [HttpPost]
        [Route("savepprofilepicture")]
        public async Task<IActionResult> SaveProfilePicture(AccountBlobRequest accountBlobRequest)
        {
            try
            {
                // Validation                 
                if ((accountBlobRequest.BlobId <= 0 || accountBlobRequest.AccountId <= 0)
                    && (accountBlobRequest.Image == null) && (string.IsNullOrEmpty(accountBlobRequest.ImageType)))
                {
                    return StatusCode(400, "The BlobId or AccountId and Image is required.");
                }
                // Validation for Image Type.
                char imageType = Convert.ToChar(accountBlobRequest.ImageType);
                if (!EnumValidator.ValidateImageType(imageType))
                {
                    return StatusCode(400, "The Image type is not valid.");
                }
                AccountBusinessService.AccountBlobRequest blobRequest = new AccountBusinessService.AccountBlobRequest();
                blobRequest.Id = accountBlobRequest.BlobId;
                blobRequest.AccountId = accountBlobRequest.AccountId;
                blobRequest.ImageType = "J";
                blobRequest.Image = ByteString.CopyFrom(accountBlobRequest.Image);
                AccountBusinessService.AccountBlobResponse response = await _accountClient.SaveProfilePictureAsync(blobRequest);
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                           "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "SaveProfilePicture  method in Account controller", accountBlobRequest.AccountId, accountBlobRequest.AccountId, JsonConvert.SerializeObject(accountBlobRequest),
                                            Request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                          "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                          "SaveProfilePicture  method in Account controller", accountBlobRequest.AccountId, accountBlobRequest.AccountId, JsonConvert.SerializeObject(accountBlobRequest),
                                           Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getprofilepicture")]
        public async Task<IActionResult> GetProfilePicture(int BlobId)
        {

            try
            {
                // Validation                 
                if (BlobId <= 0)
                {
                    return StatusCode(400, "The BlobId is required.");
                }
                // Validation for Image Type.
                AccountBusinessService.IdRequest blobRequest = new AccountBusinessService.IdRequest();
                blobRequest.Id = BlobId;
                AccountBusinessService.AccountBlobResponse response = await _accountClient.GetProfilePictureAsync(blobRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    AccountBlobResponse accountBlobResponse = new AccountBlobResponse();
                    if (response != null)
                    {
                        accountBlobResponse.BlobId = response.BlobId;
                        accountBlobResponse.Image = response.Image.ToArray();
                    }
                    return Ok(accountBlobResponse);
                }
                else if (response != null && response.Code == AccountBusinessService.Responcecode.NotFound)
                {
                    return StatusCode(404, "Profile picture for this account not found.");
                }
                else return StatusCode(500, "Internal Server Error.");
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getprofilepicturetest")]
        public async Task<IActionResult> GetProfileImageTest(int BlobId)
        {

            try
            {
                AccountBusinessService.IdRequest request = new AccountBusinessService.IdRequest();
                request.Id = BlobId;
                AccountBusinessService.AccountBlobResponse response = await _accountClient.GetProfilePictureAsync(request);
                //When creating a stream, you need to reset the position, without it you will see that you always write files with a 0 byte length. 
                var imageDataStream = new MemoryStream(response.Image.ToArray());
                imageDataStream.Position = 0;
                return File(imageDataStream, "image/png");

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Account Preference
        // Begin Account Preference
        [HttpPost]
        [Route("preference/create")]
        public async Task<IActionResult> CreateAccountPreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.RefId <= 0) || (request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatTypeId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }
                var accountPreference = new AccountBusinessService.AccountPreference();
                var preference = new AccountBusinessService.AccountPreferenceResponse();
                accountPreference = _mapper.ToAccountPreference(request);
                preference = await _accountClient.CreatePreferenceAsync(accountPreference);
                if (preference != null && preference.Code == AccountBusinessService.Responcecode.Success)
                {

                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                         "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                         "CreateAccountPreference  method in Account controller", 0, preference.AccountPreference.Id, JsonConvert.SerializeObject(request),
                                          Request);

                    return Ok(_mapper.ToAccountPreference(preference.AccountPreference));
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                        "CreateAccountPreference  method in Account controller", 0, 0, JsonConvert.SerializeObject(request),
                                         Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("preference/update")]
        public async Task<IActionResult> UpdateAccountPreference(AccountPreferenceRequest request)
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
                var preference = new AccountBusinessService.AccountPreferenceResponse();
                var accountPreference = new AccountBusinessService.AccountPreference();
                accountPreference = _mapper.ToAccountPreference(request);
                preference = await _accountClient.UpdatePreferenceAsync(accountPreference);
                if (preference != null && preference.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                       "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                       "UpdateAccountPreference  method in Account controller", request.Id, preference.AccountPreference.Id, JsonConvert.SerializeObject(request),
                                        Request);
                    return Ok(_mapper.ToAccountPreference(preference.AccountPreference));
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                       "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                       "UpdateAccountPreference  method in Account controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                                        Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("preference/delete")]
        public async Task<IActionResult> DeleteAccountPreference(int preferenceId)
        {
            AccountBusinessService.IdRequest request = new AccountBusinessService.IdRequest();
            try
            {
                if (preferenceId <= 0)
                {
                    return StatusCode(400, "The preferenceId Id is required");
                }
                request.Id = preferenceId;
                AccountBusinessService.AccountPreferenceResponse response = await _accountClient.DeletePreferenceAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                     "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                     "DeleteAccountPreference  method in Account controller", preferenceId, preferenceId, JsonConvert.SerializeObject(request),
                                      Request);

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
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                     "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                     "DeleteAccountPreference  method in Account controller", preferenceId, preferenceId, JsonConvert.SerializeObject(request),
                                      Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("preference/get")]
        public async Task<IActionResult> GetAccountPreference(int preferenceId)
        {
            try
            {
                if ((preferenceId <= 0))
                {
                    return StatusCode(400, "The preferenceId Id is required");
                }
                AccountBusinessService.AccountPreferenceFilter request = new AccountBusinessService.AccountPreferenceFilter();
                request.Id = preferenceId;
                AccountBusinessService.AccountPreferenceResponse response = await _accountClient.GetPreferenceAsync(request);

                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    if (response.AccountPreference != null)
                    {
                        return Ok(_mapper.ToAccountPreference(response.AccountPreference));
                    }
                    else
                    {
                        return StatusCode(404, "Preference details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Account Vehicle Relationship

        // create vehicle access relationship
        [HttpPost]
        [Route("accessrelationship/vehicle/create")]
        public async Task<IActionResult> CreateVehicleAccessRelationship(AccessRelationshipRequest request)
        {
            try
            {
                if ((request.Id <= 0) || (request.OrganizationId <= 0) || (request.AssociatedData == null))
                {
                    return StatusCode(400, "Invalid Payload");
                }
                // validate account type
                char accessType = Convert.ToChar(request.AccessType);
                if (!EnumValidator.ValidateAccessType(accessType))
                {
                    return StatusCode(400, "Invalid Payload");
                }
                var accessRelationship = new AccountBusinessService.VehicleAccessRelationship();
                accessRelationship = _mapper.ToAccessRelationship(request);
                accessRelationship.OrganizationId = GetContextOrgId();
                var result = await _accountClient.CreateVehicleAccessRelationshipAsync(accessRelationship);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {

                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                   "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                   "CreateVehicleAccessRelationship  method in Account controller", request.Id, 0, JsonConvert.SerializeObject(request),
                                    Request);

                    return Ok(request);
                }
                else
                {
                    return StatusCode(500, string.Empty);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                     "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                     "CreateVehicleAccessRelationship  method in Account controller", request.Id, 0, JsonConvert.SerializeObject(request),
                                      Request);

                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return BadRequest("F01");
                }
                return StatusCode(500, string.Empty);
            }
        }

        // update vehicle access relationship
        [HttpPost]
        [Route("accessrelationship/vehicle/update")]
        public async Task<IActionResult> UpdateVehicleAccessRelationship(AccessRelationshipRequest request)
        {
            try
            {
                if ((request.Id <= 0) || (request.OrganizationId <= 0) || (request.AssociatedData == null))
                {
                    return BadRequest();
                }
                var accessRelationship = new AccountBusinessService.VehicleAccessRelationship();
                accessRelationship = _mapper.ToAccessRelationship(request);
                accessRelationship.OrganizationId = GetContextOrgId();
                var result = await _accountClient.UpdateVehicleAccessRelationshipAsync(accessRelationship);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                    "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                    "UpdateVehicleAccessRelationship  method in Account controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                                     Request);
                    return Ok(request);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                   "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                   "UpdateVehicleAccessRelationship  method in Account controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                                    Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, PortalConstants.ResponseError.BadRequest);
                }
                return StatusCode(500, string.Empty);
            }
        }

        // delete vehicle access relationship
        [HttpDelete]
        [Route("accessrelationship/vehicle/delete")]
        public async Task<IActionResult> DeleteVehicleAccessRelationship(int organizationId, int Id, bool isGroup)
        {
            AccountBusinessService.DeleteAccessRelationRequest deleteRequest = new AccountBusinessService.DeleteAccessRelationRequest();
            try
            {
                // Validation                 
                if ((organizationId <= 0) || (Id <= 0))
                {
                    return BadRequest();
                }

                deleteRequest.OrganizationId = GetContextOrgId();
                deleteRequest.Id = Id;
                deleteRequest.IsGroup = isGroup;
                var result = await _accountClient.DeleteVehicleAccessRelationshipAsync(deleteRequest);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                   "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                   "DeleteVehicleAccessRelationship  method in Account controller", deleteRequest.Id, deleteRequest.Id, JsonConvert.SerializeObject(deleteRequest),
                                    Request);
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                 "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                 "DeleteVehicleAccessRelationship  method in Account controller", deleteRequest.Id, deleteRequest.Id, JsonConvert.SerializeObject(deleteRequest),
                                  Request);
                _logger.Error(null, ex);
                return StatusCode(500, string.Empty);
            }
        }

        // create account access relationship
        [HttpPost]
        [Route("accessrelationship/account/create")]
        public async Task<IActionResult> CreateAccountAccessRelationshipAsync(AccessRelationshipRequest request)
        {
            try
            {
                if ((request.Id <= 0) || (request.OrganizationId <= 0) || (request.AssociatedData == null))
                {
                    return BadRequest();
                }
                var accessRelationship = _mapper.ToAccountAccessRelationship(request);
                accessRelationship.OrganizationId = GetContextOrgId();
                var result = await _accountClient.CreateAccountAccessRelationshipAsync(accessRelationship);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                "CreateAccountAccessRelationshipAsync  method in Account controller", 0, request.Id, JsonConvert.SerializeObject(request),
                                 Request);
                    return Ok(request);
                }
                else
                {
                    return StatusCode(500, string.Empty);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                               "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                               "CreateAccountAccessRelationshipAsync  method in Account controller", 0, request.Id, JsonConvert.SerializeObject(request),
                                Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, PortalConstants.ResponseError.KeyConstraintError);
                }
                return StatusCode(500, string.Empty);
            }
        }
        // update vehicle access relationship

        [HttpPost]
        [Route("accessrelationship/account/update")]
        public async Task<IActionResult> UpdateAccountAccessRelationship(AccessRelationshipRequest request)
        {
            try
            {
                if ((request.Id <= 0) || (request.OrganizationId <= 0) || (request.AssociatedData == null))
                {
                    return BadRequest();
                }
                var accessRelationship = _mapper.ToAccountAccessRelationship(request);
                accessRelationship.OrganizationId = GetContextOrgId();
                var result = await _accountClient.UpdateAccountAccessRelationshipAsync(accessRelationship);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                               "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                               "UpdateAccountAccessRelationship  method in Account controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                                Request);
                    return Ok(request);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                              "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                              "UpdateAccountAccessRelationship  method in Account controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                               Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, PortalConstants.ResponseError.KeyConstraintError);
                }
                return StatusCode(500, string.Empty);
            }
        }

        // delete account access relationship
        [HttpDelete]
        [Route("accessrelationship/account/delete")]
        public async Task<IActionResult> DeleteAccountAccessRelationship(int organizationId, int Id, bool isGroup)
        {
            AccountBusinessService.DeleteAccessRelationRequest deleteRequest = new AccountBusinessService.DeleteAccessRelationRequest();
            try
            {
                if ((organizationId <= 0) || (Id <= 0))
                {
                    return StatusCode(400, string.Empty);
                }

                deleteRequest.OrganizationId = GetContextOrgId();
                deleteRequest.Id = Id;
                deleteRequest.IsGroup = isGroup;
                var result = await _accountClient.DeleteAccountAccessRelationshipAsync(deleteRequest);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                              "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                              "DeleteAccountAccessRelationship  method in Account controller", deleteRequest.Id, deleteRequest.Id, JsonConvert.SerializeObject(deleteRequest),
                               Request);
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                             "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                             "DeleteAccountAccessRelationship  method in Account controller", deleteRequest.Id, deleteRequest.Id, JsonConvert.SerializeObject(deleteRequest),
                              Request);
                _logger.Error(null, ex);
                return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "02"));
            }
        }

        // Get vehicle access relationship
        [HttpGet]
        [Route("accessrelationship/get")]
        public async Task<IActionResult> GetAccessRelationship(int organizationId)
        {
            try
            {
                if (organizationId <= 0)
                {
                    return BadRequest();
                }
                AccountBusinessService.AccessRelationshipFilter filter = new AccountBusinessService.AccessRelationshipFilter();
                filter.OrganizationId = GetContextOrgId();
                var vehicleAccessRelation = await _accountClient.GetAccessRelationshipAsync(filter);
                AccessRelationshipResponse response = new AccessRelationshipResponse();
                if (vehicleAccessRelation != null && vehicleAccessRelation.Code == AccountBusinessService.Responcecode.Success)
                {
                    response = _mapper.ToAccessRelationshipData(vehicleAccessRelation);
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, PortalConstants.ResponseError.KeyConstraintError);
                }
                return StatusCode(500, string.Empty);
            }
        }

        [HttpGet]
        [Route("accessrelationship/getdetails")]
        public async Task<IActionResult> GetAccountVehicleAccessRelationship(int organizationId, bool isAccount)
        {
            try
            {
                if (organizationId <= 0)
                {
                    return BadRequest();
                }
                AccountBusinessService.AccessRelationshipFilter filter = new AccountBusinessService.AccessRelationshipFilter();
                filter.IsAccount = isAccount;
                filter.OrganizationId = GetContextOrgId();
                var vehicleAccessRelation = await _accountClient.GetAccountsVehiclesAsync(filter);
                AccessRelationshipResponseDetail response = new AccessRelationshipResponseDetail();
                if (vehicleAccessRelation != null && vehicleAccessRelation.Code == AccountBusinessService.Responcecode.Success)
                {
                    response = _mapper.ToAccessRelationshipData(vehicleAccessRelation);
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, PortalConstants.ResponseError.KeyConstraintError);
                }
                return StatusCode(500, string.Empty);
            }
        }

        #endregion

        #region Account Group

        [HttpPost]
        [Route("accountgroup/create")]
        public async Task<IActionResult> CreateAccountGroup(AccountGroupRequest request)
        {
            try
            {
                if ((string.IsNullOrEmpty(request.Name)) || (request.OrganizationId <= 0) || (string.IsNullOrEmpty(request.GroupType)))
                {
                    return StatusCode(400, "The Account group name, organization id and group type is required");
                }
                // check for valid group type                
                char groupType = Convert.ToChar(request.GroupType);
                if (!EnumValidator.ValidateGroupType(groupType))
                {
                    return StatusCode(400, "The group type is not valid.");
                }
                AccountBusinessService.AccountGroupRequest accountGroupRequest = new AccountBusinessService.AccountGroupRequest();
                accountGroupRequest = _mapper.ToAccountGroup(request);
                accountGroupRequest.OrganizationId = GetContextOrgId();
                AccountBusinessService.AccountGroupResponce response = await _accountClient.CreateGroupAsync(accountGroupRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                            "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                            "CreateAccountGroup  method in Account controller", 0, response.AccountGroup.Id, JsonConvert.SerializeObject(request),
                             Request);


                    return Ok(_mapper.ToAccountGroup(response));
                }
                else if (response != null && response.Code == AccountBusinessService.Responcecode.Conflict)
                {
                    return StatusCode(409, "Duplicate Account Group.");
                }
                else
                {
                    return StatusCode(500, "AccountGroupResponce is empty " + response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                          "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                          "CreateAccountGroup  method in Account controller", 0, 0, JsonConvert.SerializeObject(request),
                           Request);
                _logger.Error(null, ex);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("accountgroup/update")]
        public async Task<IActionResult> UpdateAccountGroup(AccountGroupRequest request)
        {
            try
            {
                if ((request.Id <= 0) || (string.IsNullOrEmpty(request.Name)))
                {
                    return StatusCode(400, "The AccountGroup name and id is required");
                }
                AccountBusinessService.AccountGroupRequest accountGroupRequest = new AccountBusinessService.AccountGroupRequest();
                accountGroupRequest = _mapper.ToAccountGroup(request);
                accountGroupRequest.OrganizationId = GetContextOrgId();
                AccountBusinessService.AccountGroupResponce response = await _accountClient.UpdateGroupAsync(accountGroupRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                         "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                         "UpdateAccountGroup  method in Account controller", request.Id, response.AccountGroup.Id, JsonConvert.SerializeObject(request),
                          Request);
                    return Ok(_mapper.ToAccountGroup(response));
                }
                else if (response != null && response.Code == AccountBusinessService.Responcecode.Conflict)
                {
                    return StatusCode(409, "Duplicate Account Group.");
                }
                else
                {
                    return StatusCode(500, "AccountGroupResponce is null " + response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                        "UpdateAccountGroup  method in Account controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                         Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("accountgroup/delete")]
        public async Task<IActionResult> DeleteAccountGroup(int id)
        {
            AccountBusinessService.IdRequest request = new AccountBusinessService.IdRequest();
            try
            {
                if ((Convert.ToInt32(id) <= 0))
                {
                    return StatusCode(400, "The account group id is required.");
                }

                request.Id = id;
                AccountBusinessService.AccountGroupResponce response = await _accountClient.RemoveGroupAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                       "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                       "DeleteAccountGroup  method in Account controller", 0, request.Id, JsonConvert.SerializeObject(request),
                        Request);
                    return Ok(response.AccountGroup);
                }
                else
                {
                    return StatusCode(500, "AccountGroupResponce is null " + response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                     "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                     "DeleteAccountGroup  method in Account controller", 0, request.Id, JsonConvert.SerializeObject(request),
                      Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("accountgroup/addaccounts")]
        public async Task<IActionResult> AddAccountsToGroup(AccountGroupAccount request)
        {
            try
            {
                if (request == null)
                {
                    return StatusCode(400, "The AccountGroup account is required");
                }
                AccountBusinessService.AccountGroupRefRequest groupRequest = new AccountBusinessService.AccountGroupRefRequest();

                if (request != null && request.Accounts != null)
                {
                    foreach (var groupref in request.Accounts)
                    {
                        groupRequest.GroupRef.Add(new AccountBusinessService.AccountGroupRef() { GroupId = groupref.AccountGroupId, RefId = groupref.AccountId });
                    }
                }
                AccountBusinessService.AccountGroupRefResponce response = await _accountClient.AddAccountToGroupsAsync(groupRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                   "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                   "AddAccountsToGroup  method in Account controller", 0, 0, JsonConvert.SerializeObject(request),
                    Request);//verify
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, "AccountGroupRefResponce is null or " + response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                  "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                  "AddAccountsToGroup  method in Account controller", 0, 0, JsonConvert.SerializeObject(request),
                   Request);//verify
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("accountgroup/deleteaccounts")]
        public async Task<IActionResult> DeleteAccountFromGroup(int id)
        {
            AccountBusinessService.IdRequest request = new AccountBusinessService.IdRequest();
            try
            {
                // Validation  
                if (id <= 0)
                {
                    return StatusCode(400, "The Account Id is required");
                }

                request.Id = id;
                var response = await _accountClient.DeleteAccountFromGroupsAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                         "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                         "DeleteAccountFromGroup  method in Account controller", id, id, JsonConvert.SerializeObject(request),
                          Request);
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, "AccountGroupRefResponce is null or " + response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                         "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                         "DeleteAccountFromGroup  method in Account controller", id, id, JsonConvert.SerializeObject(request),
                          Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("accountgroup/get")]
        public async Task<IActionResult> GetAccountGroup(AccountBusinessService.AccountGroupFilterRequest request)
        {
            try
            {
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
                request.OrganizationId = GetContextOrgId();
                AccountBusinessService.AccountGroupDataList response = await _accountClient.GetAccountGroupAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    if (response.AccountGroupRequest != null && response.AccountGroupRequest.Count > 0)
                    {
                        return Ok(response.AccountGroupRequest);
                    }
                    else
                    {
                        return StatusCode(404, "Account Groups are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("accountgroup/getdetails")]
        public async Task<IActionResult> GetAccountGroupDetails(AccountBusinessService.AccountGroupDetailsRequest request)
        {
            try
            {
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
                request.OrganizationId = GetContextOrgId();
                AccountBusinessService.AccountGroupDetailsDataList response = await _accountClient.GetAccountGroupDetailAsync(request);

                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    if (response.AccountGroupDetail != null && response.AccountGroupDetail.Count > 0)
                    {
                        return Ok(response.AccountGroupDetail);
                    }
                    else
                    {
                        return StatusCode(404, "Account Group details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Account Role   

        [HttpPost]
        [Route("addroles")]
        public async Task<IActionResult> AddRoles(AccountRoleRequest request)
        {
            try
            {
                if (request.AccountId <= 0 || request.OrganizationId <= 0
                   || Convert.ToInt16(request.Roles.Count) <= 0)
                {
                    return StatusCode(400, "The Account Id and Organization id and role id is required");
                }
                AccountBusinessService.AccountRoleRequest roles = new AccountBusinessService.AccountRoleRequest();

                roles = _mapper.ToRole(request);
                roles.OrganizationId = AssignOrgContextByAccountId(request.AccountId);

                AccountBusinessService.AccountRoleResponse response = await _accountClient.AddRolesAsync(roles);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                         "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                         "AddRoles  method in Account controller", roles.AccountId, roles.AccountId, JsonConvert.SerializeObject(request),
                          Request);
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                        "AddRoles  method in Account controller", request.AccountId, request.AccountId, JsonConvert.SerializeObject(request),
                         Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("deleteroles")]
        public async Task<IActionResult> RemoveRoles(AccountBusinessService.AccountRoleDeleteRequest request)
        {
            try
            {
                if (request == null && request.OrganizationId <= 0 || request.AccountId <= 0)
                {
                    return StatusCode(400, "The Organization id and account id is required");
                }
                request.OrganizationId = AssignOrgContextByAccountId(request.AccountId);

                AccountBusinessService.AccountRoleResponse response = await _accountClient.RemoveRolesAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                        "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                        "RemoveRoles  method in Account controller", request.AccountId, request.AccountId, JsonConvert.SerializeObject(request),
                         Request);
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                       "Account service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                       "RemoveRoles  method in Account controller", request.AccountId, request.AccountId, JsonConvert.SerializeObject(request),
                        Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("getroles")]
        public async Task<IActionResult> GetRoles(AccountBusinessService.AccountRoleDeleteRequest request)
        {
            try
            {
                if (request == null && request.OrganizationId <= 0 || request.AccountId <= 0)
                {
                    return StatusCode(400, "The Organization id and account id is required");
                }
                request.OrganizationId = AssignOrgContextByAccountId(request.AccountId);

                AccountBusinessService.AccountRoles response = await _accountClient.GetRolesAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response.Roles);
                }
                else if (response != null && response.Code == AccountBusinessService.Responcecode.Failed
                    && response.Message == "Please provide accountid and organizationid to get roles details.")
                {
                    return StatusCode(400, "Please provide accountid and organizationid to get roles details.");
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Single Sign On
        [HttpPost]
        [Route("sso")]
        public async Task<IActionResult> GenerateSSOToken()
        {
            try
            {
                AccountBusinessService.TokenSSORequest ssoRequest = new AccountBusinessService.TokenSSORequest();
                ssoRequest.AccountID = _userDetails.accountId;
                ssoRequest.RoleID = _userDetails.roleId;
                ssoRequest.OrganizationID = _userDetails.contextOrgId > 0 ? _userDetails.contextOrgId : _userDetails.orgId;
                if (ssoRequest.AccountID <= 0 || ssoRequest.RoleID <= 0 || ssoRequest.OrganizationID <= 0)
                {
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "MISSING_PARAMETER", nameof(HeaderObj));
                }
                var response = await _accountClient.GenerateSSOAsync(ssoRequest);
                if (response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                           "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "GenerateSSOToken method in Account controller", ssoRequest.AccountID, ssoRequest.AccountID,
                                           JsonConvert.SerializeObject(ssoRequest), Request);
                    return Ok(response.Token);
                }
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                {
                    return GenerateErrorResponse(HttpStatusCode.NotFound, "INVALID_USER!", Convert.ToString(ssoRequest.AccountID));
                }
                else
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "BAD REQUEST", Convert.ToString(ssoRequest.AccountID));
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                          "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                          "GenerateSSOToken method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                          null, Request);
                _logger.Error(null, ex);
                return StatusCode(500, "Internal server error");
            }
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode StatusCode, string Massage, string Value)
        {
            return base.StatusCode((int)StatusCode, new ErrorResponse()
            {
                ResponseCode = ((int)StatusCode).ToString(),
                Message = Massage,
                Value = Value
            });
        }
        #endregion

        #region Session org context switching

        [HttpPost]
        [Route("setuserselection")]
        public async Task<IActionResult> SetUserSelection([FromBody] AccountInfoRequest request)
        {
            try
            {
                if (request.AccountId == _userDetails.accountId)
                {
                    HttpContext.Session.SetInt32(SessionConstants.RoleKey, request.RoleId);
                    HttpContext.Session.SetInt32(SessionConstants.OrgKey, request.OrgId);
                    HttpContext.Session.SetInt32(SessionConstants.ContextOrgKey, request.OrgId);
                    return Ok();
                }
                else
                    return BadRequest("Account Id mismatch in the request.");
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                          "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                          "SetAccountInfo method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                          null, Request);
                _logger.Error(null, ex);
                return StatusCode(500, "Error occurred while saving account information.");
            }
        }

        [HttpPost]
        [Route("switchorgcontext")]
        public async Task<IActionResult> SwitchOrgContext([FromBody] OrgSwitchRequest request)
        {
            try
            {
                int s_accountId = _userDetails.accountId;
                int s_roleId = _userDetails.roleId;
                int s_orgId = _userDetails.orgId;
                int s_contextOrgId = _userDetails.contextOrgId;

                if (request.AccountId != s_accountId)
                    return BadRequest("Account Id mismatched");

                // check for DAF Admin
                int level = await _privilegeChecker.GetLevelByRoleId(s_orgId, s_roleId);

                //Add context org id to session
                if (level >= 30)
                    return Unauthorized("Unauthorized access");

                if (s_contextOrgId != request.ContextOrgId)
                {
                    HttpContext.Session.SetInt32(SessionConstants.ContextOrgKey, request.ContextOrgId);

                    //return menu items
                    var response = await _accountClient.GetMenuFeaturesAsync(new AccountBusinessService.MenuFeatureRequest()
                    {
                        AccountId = s_accountId,
                        OrganizationId = s_orgId,
                        RoleId = s_roleId,
                        LanguageCode = request.LanguageCode,
                        ContextOrgId = request.ContextOrgId
                    });

                    if (response.Code == AccountBusinessService.Responcecode.Success)
                        return Ok(response.MenuFeatures);
                    else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                        return NoContent();
                    else
                        return StatusCode(500, "Error occurred while fetching menu items and features for the context.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Account Component",
                                          "Account service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                          "SwitchOrgContext method in Account controller", _userDetails.accountId, _userDetails.accountId,
                                          null, Request);
                _logger.Error(null, ex);
                return StatusCode(500, "Error occurred while fetching menu items and features for the context.");
            }
        }

        #endregion
    }

}
