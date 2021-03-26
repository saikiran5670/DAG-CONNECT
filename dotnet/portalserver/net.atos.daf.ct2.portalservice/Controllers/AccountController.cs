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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Account;
using net.atos.daf.ct2.utilities;
using AccountBusinessService = net.atos.daf.ct2.accountservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    // [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {

        #region Private Variable
        private readonly ILogger<AccountController> _logger;
        private readonly AccountBusinessService.AccountService.AccountServiceClient _accountClient;
        private readonly Mapper _mapper;
        private readonly IMemoryCacheExtensions _cache;

        #endregion

        #region Constructor
        public AccountController(AccountBusinessService.AccountService.AccountServiceClient accountClient, ILogger<AccountController> logger, IMemoryCacheExtensions cache)
        {
            _accountClient = accountClient;
            _logger = logger;
            _mapper = new Mapper();
            _cache = cache;
        }
        #endregion

        #region Account
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(AccountRequest request)
        {
            try
            {
                // Validation 
                if ((string.IsNullOrEmpty(request.EmailId)) || (string.IsNullOrEmpty(request.FirstName))
                || (string.IsNullOrEmpty(request.LastName)) || (request.OrganizationId <= 0) || (string.IsNullOrEmpty(request.Type)))
                {
                    return StatusCode(400, PortalConstants.AccountValidation.CreateRequired);
                }
                // Length validation
                Int32 validOrgId = 0;
                if ((request.EmailId.Length > 50) || (request.FirstName.Length > 30)
                || (request.LastName.Length > 20) || !Int32.TryParse(request.OrganizationId.ToString(), out validOrgId))
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
                AccountBusinessService.AccountData accountResponse = await _accountClient.CreateAsync(accountRequest);
                AccountResponse response = new AccountResponse();
                response = _mapper.ToAccount(accountResponse.Account);
                if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Conflict)
                {
                    var accountPreference = new AccountPreference();
                    accountPreference.Preference = null;
                    accountPreference.Account = response;
                    AccountBusinessService.AccountPreferenceFilter preferenceRequest = new AccountBusinessService.AccountPreferenceFilter();
                    preferenceRequest.Id = response.PreferenceId;
                    // get preference
                    AccountBusinessService.AccountPreferenceResponse accountPreferenceResponse = await _accountClient.GetPreferenceAsync(preferenceRequest);                    
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
                else if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Failed
                    && accountResponse.Message == PortalConstants.AccountValidation.ErrorMessage)
                {
                    return StatusCode(500, PortalConstants.AccountValidation.ErrorMessage);
                }
                else if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Failed
                    && accountResponse.Message == PortalConstants.AccountValidation.EmailSendingFailedMessage)
                {
                    return StatusCode(500, PortalConstants.AccountValidation.EmailSendingFailedMessage);
                }
                else if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response);
                }
                else
                {
                    if (accountResponse.Account == null) return Ok(accountResponse.Account);
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Account Service:Create : " + ex.Message + " " + ex.StackTrace);
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
                // Validation 
                if ((request.Id <= 0) || (string.IsNullOrEmpty(request.EmailId))
                    || (string.IsNullOrEmpty(request.FirstName)) || (string.IsNullOrEmpty(request.LastName)))
                {
                    return StatusCode(400, "The AccountId, EmailId address, first name, last name is required.");
                }
                // Length validation
                Int32 validOrgId = 0;
                if ((request.EmailId.Length > 50) || (request.FirstName.Length > 30)
                || (request.LastName.Length > 20) || !Int32.TryParse(request.OrganizationId.ToString(), out validOrgId))
                {
                    return StatusCode(400, "The EmailId address, first name, last name and organization id should be valid.");
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
                AccountBusinessService.AccountData accountResponse = await _accountClient.UpdateAsync(accountRequest);
                if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Failed)
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
                else if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(request);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "02"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Account Service:Update : " + ex.Message + " " + ex.StackTrace);
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
            try
            {
                // Validation                 
                if ((string.IsNullOrEmpty(EmailId)) || (Convert.ToInt32(AccountId) <= 0) || (Convert.ToInt32(OrganizationId) <= 0))
                {
                    return StatusCode(400, "The Email address, account id and organization id is required.");
                }
                AccountBusinessService.AccountRequest accountRequest = new AccountBusinessService.AccountRequest();
                accountRequest.Id = AccountId;
                accountRequest.EmailId = EmailId;
                accountRequest.OrganizationId = OrganizationId;
                AccountBusinessService.AccountResponse response = await _accountClient.DeleteAsync(accountRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                    return Ok(accountRequest);
                else
                    return StatusCode(404, "Account not configured.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:delete account with exception - " + ex.Message + ex.StackTrace);
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
                AccountBusinessService.AccountResponse response = await _accountClient.ChangePasswordAsync(changePasswordRequest);
                if (response.Code == AccountBusinessService.Responcecode.Success)
                    return Ok("Password has been changed.");
                else if (response.Code == AccountBusinessService.Responcecode.BadRequest)
                    return BadRequest(response.Message);
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NotFound(response.Message);
                else
                    return StatusCode(500, "Account not configured or failed to change password.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:delete account with exception - " + ex.Message + ex.StackTrace);
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
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
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
                _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
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
                accountRequest.OrganizationId = request.OrganizationId;
                accountRequest.AccountId = request.AccountId;
                accountRequest.StartDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                accountRequest.EndDate = 0;

                AccountBusinessService.AccountOrganizationResponse response = await _accountClient.AddAccountToOrgAsync(accountRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Failed)
                {
                    return StatusCode(500, "Internal Server Error.(0)");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Account Service:AddAccountToOrg : " + ex.Message + " " + ex.StackTrace);
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
                    return Ok(response.Message);
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NotFound(response.Message);
                else
                    return StatusCode(500, "Password reset process failed to initiate or Error while sending email.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:ResetPasswordInitiate with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("resetpassword")]
        [Route("createpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if(!Guid.TryParse(request.ProcessToken, out _))
                {
                    return BadRequest($"{nameof(request.ProcessToken)} field is tampered or has invalid value.");
                }
                var resetPasswordRequest = new AccountBusinessService.ResetPasswordRequest();
                resetPasswordRequest.ProcessToken = request.ProcessToken;
                resetPasswordRequest.Password = request.Password;

                var response = await _accountClient.ResetPasswordAsync(resetPasswordRequest);
                if (response.Code == AccountBusinessService.Responcecode.Success)
                    return Ok("Reset password process is successfully completed.");
                else if (response.Code == AccountBusinessService.Responcecode.BadRequest)
                    return BadRequest(response.Message);
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NotFound(response.Message);
                else
                    return StatusCode(500, "Reset password process failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:ResetPassword with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

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
                    return Ok(response.Message);
                else if (response.Code == AccountBusinessService.Responcecode.NotFound)
                    return NotFound(response.Message);
                else
                    return StatusCode(500, "Reset password invalidate process failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:ResetPasswordInvalidate with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("getmenufeatures")]
        public async Task<IActionResult> GetMenuFeatures([FromBody] MenuFeatureRequest request)
        {
            try
            {
                var menuFeatureRequest = new AccountBusinessService.MenuFeatureRequest();
                menuFeatureRequest.AccountId = request.AccountId;
                menuFeatureRequest.RoleId = request.RoleId;
                menuFeatureRequest.OrganizationId = request.OrganizationId;

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
                _logger.LogError("Error in account service:GetMenuFeatures with exception - " + ex.Message + ex.StackTrace);
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create profile picture with exception - " + ex.Message + ex.StackTrace);
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
                _logger.LogError("Error in account service:create profile picture with exception - " + ex.Message + ex.StackTrace);
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
                _logger.LogError("Error in account service:create profile picture with exception - " + ex.Message + ex.StackTrace);
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
            //Task<AccountPreferenceResponse> CreatePreference(AccountPreference request, 
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
                _logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
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
                _logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
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
                _logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
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
                // Validation                 
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
                _logger.LogError("Error in account service:get account preference with exception - " + ex.Message + ex.StackTrace);
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
            //Task<AccountPreferenceResponse> CreatePreference(AccountPreference request, 
            try
            {
                // Validation                 
                if ((request.Id <= 0) || (request.OrganizationId<= 0) || (request.AssociatedData == null))                    
                {
                    return StatusCode(400, "Invalid Payload");
                }
                // validate account type
                char accessType = Convert.ToChar(request.AccessType);
                if (!EnumValidator.ValidateAccessType(accessType))
                {
                    return StatusCode(400, "Invalid Payload");
                }
                var accessRelationship = _mapper.ToAccessRelationship(request);
                var result = await _accountClient.CreateVehicleAccessRelationshipAsync(accessRelationship);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(request);
                }
                else
                {
                    return StatusCode(500,string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create vehicle access relationship with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return BadRequest("F01");
                }
                return StatusCode(500,string.Empty);
            }
        }
        // update vehicle access relationship
        [HttpPost]
        [Route("accessrelationship/vehicle/update")]
        public async Task<IActionResult> UpdateVehicleAccessRelationship(AccessRelationshipRequest request)
        {
            try
            {
                // Validation                 
                if ((request.Id <= 0) || (request.OrganizationId <= 0) || (request.AssociatedData == null))
                {
                    return BadRequest();
                }
                var accessRelationship = _mapper.ToAccessRelationship(request);
                var result = await _accountClient.UpdateVehicleAccessRelationshipAsync(accessRelationship);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(request);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:update vehicle access relationship with exception - " + ex.Message + ex.StackTrace);
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
        public async Task<IActionResult> DeleteVehicleAccessRelationship(int organizationId,int Id, bool isGroup)
        {
            try
            {
                // Validation                 
                if ((organizationId <= 0) || (Id <= 0))
                {
                    return BadRequest();
                }
                AccountBusinessService.DeleteAccessRelationRequest deleteRequest = new AccountBusinessService.DeleteAccessRelationRequest();
                deleteRequest.OrganizationId = organizationId;
                deleteRequest.Id = Id;
                deleteRequest.IsGroup = isGroup;
                var result = await _accountClient.DeleteVehicleAccessRelationshipAsync(deleteRequest);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:delete vehicle access relationship with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }
        }
        // create account access relationship
        [HttpPost]
        [Route("accessrelationship/account/create")]
        public async Task<IActionResult> CreateAccountAccessRelationshipAsync(AccessRelationshipRequest request)
        {
            //Task<AccountPreferenceResponse> CreatePreference(AccountPreference request, 
            try
            {
                // Validation                 
                if ((request.Id <= 0) || (request.OrganizationId <= 0) || (request.AssociatedData == null))
                {
                    return BadRequest();
                }
                var accessRelationship = _mapper.ToAccountAccessRelationship(request);
                var result = await _accountClient.CreateAccountAccessRelationshipAsync(accessRelationship);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(request);
                }
                else
                {
                    return StatusCode(500, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:create account access relationship with exception - " + ex.Message + ex.StackTrace);
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
                // Validation                 
                if ((request.Id <= 0) || (request.OrganizationId <= 0) || (request.AssociatedData == null))
                {
                    return BadRequest();
                }
                var accessRelationship = _mapper.ToAccountAccessRelationship(request);
                var result = await _accountClient.UpdateAccountAccessRelationshipAsync(accessRelationship);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(request);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:update vehicle access relationship with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, PortalConstants.ResponseError.KeyConstraintError);
                }
                return StatusCode(500,string.Empty);
            }
        }

        // delete account access relationship

        [HttpDelete]
        [Route("accessrelationship/account/delete")]
        public async Task<IActionResult> DeleteAccountAccessRelationship(int organizationId, int Id, bool isGroup)
        {
            try
            {
                // Validation                 
                if ((organizationId <= 0) || (Id <= 0))
                {
                    return StatusCode(400,string.Empty);
                }
                AccountBusinessService.DeleteAccessRelationRequest deleteRequest = new AccountBusinessService.DeleteAccessRelationRequest();
                deleteRequest.OrganizationId = organizationId;
                deleteRequest.Id = Id;
                deleteRequest.IsGroup = isGroup;
                var result = await _accountClient.DeleteAccountAccessRelationshipAsync(deleteRequest);
                if (result != null && result.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "01"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:delete account access relationship with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, string.Format(PortalConstants.ResponseError.InternalServerError, "02"));
            }
        }
        // update vehicle access relationship
        [HttpGet]
        [Route("accessrelationship/get")]
        public async Task<IActionResult> GetAccessRelationship(int organizationId)
        {
            try
            {
                // Validation                 
                if (organizationId <= 0)
                {
                    return BadRequest();
                }
                AccountBusinessService.AccessRelationshipFilter filter = new AccountBusinessService.AccessRelationshipFilter();
                filter.OrganizationId = organizationId;
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
                _logger.LogError("Error in account service:update vehicle access relationship with exception - " + ex.Message + ex.StackTrace);
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
        public async Task<IActionResult> GetAccountVehicleAccessRelationship(int organizationId,bool isAccount)
        {
            try
            {
                // Validation                 
                if (organizationId <= 0)
                {
                    return BadRequest();
                }
                AccountBusinessService.AccessRelationshipFilter filter = new AccountBusinessService.AccessRelationshipFilter();
                filter.IsAccount = isAccount;
                filter.OrganizationId = organizationId;
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
                _logger.LogError("Error in account service:update vehicle access relationship with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
                {
                    return StatusCode(400, PortalConstants.ResponseError.KeyConstraintError);
                }
                return StatusCode(500, string.Empty);
            }
        }

        #endregion


        #region OLD AccessRelationship  Need to be Deleted
        //TODO: Need to refactor this based on screen.
        // Begin - AccessRelationship
        //[HttpPost]
        //[Route("accessrelationship/create")]
        //public async Task<IActionResult> CreateAccessRelationship(AccountBusinessService.AccessRelationship request)
        //{
        //    try
        //    {
        //        // Validation                 
        //        if ((request.AccountGroupId <= 0) || (request.VehicleGroupId <= 0) || (string.IsNullOrEmpty(Convert.ToString(request.AccessRelationType))))
        //        {
        //            return StatusCode(400, "The AccountGroupId,VehicleGroupId and AccessRelationshipType is required");
        //        }
        //        else if ((!request.AccessRelationType.Equals("R")) && (!request.AccessRelationType.Equals("W")))
        //        {
        //            return StatusCode(400, "The AccessRelationshipType should be ReadOnly and ReadWrite (R/W) only.");
        //        }
        //        AccountBusinessService.AccessRelationshipResponse accessRelationship = await _accountClient.CreateAccessRelationshipAsync(request);
        //        if (accessRelationship != null && accessRelationship.Code == AccountBusinessService.Responcecode.Success)
        //        {
        //            return Ok(accessRelationship.AccessRelationship);
        //        }
        //        else if (accessRelationship != null && accessRelationship.Code == AccountBusinessService.Responcecode.Failed
        //            && accessRelationship.Message == "The AccessType should be ReadOnly / ReadWrite.(R/W).")
        //        {
        //            return StatusCode(400, "The AccessType should be ReadOnly / ReadWrite.(R/W).");
        //        }
        //        else
        //        {
        //            return StatusCode(500, "accessRelationship is null" + accessRelationship.Message);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
        //        // check for fk violation
        //        if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_Constraint))
        //        {
        //            return StatusCode(400, "The foreign key violation in one of dependant data.");
        //        }
        //        return StatusCode(500, ex.Message + " " + ex.StackTrace);
        //    }
        //}
        //[HttpPost]
        //[Route("accessrelationship/update")]
        //public async Task<IActionResult> UpdateAccessRelationship(AccountBusinessService.AccessRelationship request)
        //{
        //    try
        //    {
        //        // Validation                 
        //        if ((request.AccountGroupId <= 0) || (request.VehicleGroupId <= 0) || (string.IsNullOrEmpty(Convert.ToString(request.AccessRelationType))))
        //        {
        //            return StatusCode(400, "The AccountGroupId,VehicleGroupId and AccessRelationshipType is required");
        //        }
        //        else if ((!request.AccessRelationType.Equals("R")) && (!request.AccessRelationType.Equals("W")))
        //        {
        //            return StatusCode(400, "The AccessRelationshipType should be ReadOnly and ReadWrite (R/W) only.");
        //        }
        //        AccountBusinessService.AccessRelationshipResponse accessRelationship = await _accountClient.UpdateAccessRelationshipAsync(request);
        //        if (accessRelationship != null && accessRelationship.Code == AccountBusinessService.Responcecode.Success)
        //        {
        //            return Ok(accessRelationship.AccessRelationship);
        //        }
        //        else if (accessRelationship != null && accessRelationship.Code == AccountBusinessService.Responcecode.Failed
        //            && accessRelationship.Message == "The AccessType should be ReadOnly / ReadWrite.(R/W).")
        //        {
        //            return StatusCode(400, "The AccessType should be ReadOnly / ReadWrite.(R/W).");
        //        }
        //        else
        //        {
        //            return StatusCode(500, "accessRelationship is null" + accessRelationship.Message);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error in account service:get accounts with exception - " + ex.Message + ex.StackTrace);
        //        return StatusCode(500, ex.Message + " " + ex.StackTrace);
        //    }
        //}
        //[HttpPost]
        //[Route("accessrelationship/delete")]
        //public async Task<IActionResult> DeleteAccessRelationship(AccountBusinessService.AccessRelationshipDeleteRequest request)
        //{
        //    try
        //    {
        //        // Validation                 
        //        if (request.AccountGroupId <= 0 || request.VehicleGroupId <= 0)
        //        {
        //            return StatusCode(400, "The AccountGroupId,VehicleGroupId is required");
        //        }
        //        AccountBusinessService.AccessRelationshipResponse accessRelationship = await _accountClient.DeleteAccessRelationshipAsync(request);
        //        if (accessRelationship != null && accessRelationship.Code == AccountBusinessService.Responcecode.Success)
        //        {
        //            return Ok(accessRelationship.AccessRelationship);
        //        }
        //        else if (accessRelationship != null && accessRelationship.Code == AccountBusinessService.Responcecode.Failed
        //            && accessRelationship.Message == "The delete access group , Account Group Id and Vehicle Group Id is required.")
        //        {
        //            return StatusCode(400, "The delete access group , Account Group Id and Vehicle Group Id is required.");
        //        }
        //        else
        //        {
        //            return StatusCode(500, "accessRelationship is null" + accessRelationship.Message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error in account service:create access relationship with exception - " + ex.Message);
        //        return StatusCode(500, ex.Message + " " + ex.StackTrace);
        //    }
        //}
        //[HttpGet]
        //[Route("accessrelationship/get")]
        //public async Task<IActionResult> GetAccessRelationship(int AccountId, int AccountGroupId, int VehicleGroupId)
        //{
        //    try
        //    {
        //        // Validation                 
        //        if ((AccountId <= 0) && (AccountGroupId <= 0))
        //        {
        //            return StatusCode(400, "The AccountId or AccountGroupId is required");
        //        }
        //        AccountBusinessService.AccessRelationshipFilter request = new AccountBusinessService.AccessRelationshipFilter();
        //        request.AccountId = AccountId;
        //        request.AccountGroupId = AccountGroupId;
        //        request.VehicleGroupId = VehicleGroupId;
        //        AccountBusinessService.AccessRelationshipDataList accessRelationship = await _accountClient.GetAccessRelationshipAsync(request);
        //        if (accessRelationship != null && accessRelationship.Code == AccountBusinessService.Responcecode.Success)
        //        {
        //            if (accessRelationship.AccessRelationship != null && accessRelationship.AccessRelationship.Count > 0)
        //            {
        //                return Ok(accessRelationship.AccessRelationship);
        //            }
        //            else
        //            {
        //                return StatusCode(404, "access relationship details are found.");
        //            }
        //        }
        //        else if (accessRelationship != null && accessRelationship.Code == AccountBusinessService.Responcecode.Failed
        //            && accessRelationship.Message == "Please provide AccountId or AccountGroupId or VehicleGroupId to get AccessRelationship.")
        //        {
        //            return StatusCode(400, "Please provide AccountId or AccountGroupId or VehicleGroupId to get AccessRelationship.");
        //        }
        //        else
        //        {
        //            return StatusCode(500, accessRelationship.Message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error in account service:get accessrelatioship with exception - " + ex.Message + ex.StackTrace);
        //        return StatusCode(500, ex.Message + " " + ex.StackTrace);
        //    }
        //}
        // End - AccessRelationshhip
        #endregion

        #region Account Group

        [HttpPost]
        [Route("accountgroup/create")]
        public async Task<IActionResult> CreateAccountGroup(AccountGroupRequest request)
        {
            try
            {
                // Validation                 
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
                AccountBusinessService.AccountGroupResponce response = await _accountClient.CreateGroupAsync(accountGroupRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
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
                _logger.LogError("Error in account service:create account group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("accountgroup/update")]
        public async Task<IActionResult> UpdateAccountGroup(AccountGroupRequest request)
        {
            try
            {
                // Validation                 
                if ((request.Id <= 0) || (string.IsNullOrEmpty(request.Name)))
                {
                    return StatusCode(400, "The AccountGroup name and id is required");
                }
                AccountBusinessService.AccountGroupRequest accountGroupRequest = new AccountBusinessService.AccountGroupRequest();
                accountGroupRequest = _mapper.ToAccountGroup(request);
                AccountBusinessService.AccountGroupResponce response = await _accountClient.UpdateGroupAsync(accountGroupRequest);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
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
                _logger.LogError("Error in account service:create account group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPut]
        [Route("accountgroup/delete")]
        public async Task<IActionResult> DeleteAccountGroup(int id)
        {
            try
            {
                // Validation                 
                if ((Convert.ToInt32(id) <= 0))
                {
                    return StatusCode(400, "The account group id is required.");
                }
                AccountBusinessService.IdRequest request = new AccountBusinessService.IdRequest();
                request.Id = id;
                AccountBusinessService.AccountGroupResponce response = await _accountClient.RemoveGroupAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(response.AccountGroup);
                }
                else
                {
                    return StatusCode(500, "AccountGroupResponce is null " + response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in delete account group :DeleteGroup with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("accountgroup/addaccounts")]
        public async Task<IActionResult> AddAccountsToGroup(AccountGroupAccount request)
        {
            try
            {
                // Validation  
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
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, "AccountGroupRefResponce is null or " + response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in delete account group :DeleteGroup with exception - " + ex.Message + ex.StackTrace);
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
            try
            {
                // Validation  
                if (id <= 0)
                {
                    return StatusCode(400, "The Account Id is required");
                }
                AccountBusinessService.IdRequest request = new AccountBusinessService.IdRequest();
                request.Id = id;
                var response = await _accountClient.DeleteAccountFromGroupsAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, "AccountGroupRefResponce is null or " + response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in delete account group reference :DeleteAccountsGroupReference with exception - " + ex.Message + ex.StackTrace);
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
                // Validation  
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
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
                _logger.LogError("Error in account service:get account group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPost]
        [Route("accountgroup/getdetails")]
        public async Task<IActionResult> GetAccountGroupDetails(AccountBusinessService.AccountGroupDetailsRequest request)
        {
            try
            {
                // Validation  
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "The Organization id is required");
                }
                //AccountBusinessService.AccountGroupDetailsRequest accountGroupRequest = _mapper.ToAccountGroupFilter(request);
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
                _logger.LogError("Error in account service:get account group details with exception - " + ex.Message + ex.StackTrace);
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
                // Validation  
                if (request.AccountId <= 0 || request.OrganizationId <= 0
                   || Convert.ToInt16(request.Roles.Count) <= 0)
                {
                    return StatusCode(400, "The Account Id and Organization id and role id is required");
                }
                AccountBusinessService.AccountRoleRequest roles = new AccountBusinessService.AccountRoleRequest();
                roles = _mapper.ToRole(request);
                AccountBusinessService.AccountRoleResponse response = await _accountClient.AddRolesAsync(roles);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group details with exception - " + ex.Message + ex.StackTrace);
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
                // Validation  
                if (request == null && request.OrganizationId <= 0 || request.AccountId <= 0)
                {
                    return StatusCode(400, "The Organization id and account id is required");
                }
                AccountBusinessService.AccountRoleResponse response = await _accountClient.RemoveRolesAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in account service:get account group details with exception - " + ex.Message + ex.StackTrace);
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
                _logger.LogError("Error in account service:get account roles with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion


        #region TestMethods 
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("authmethodpost")]
        public async Task<OkObjectResult> AuthMethodPost()
        {
            return await Task.FromResult(Ok(new { Message = "You are authenticated user " + Dns.GetHostName() }));
        }

        [HttpPost]
        [Route("withoutauthmethodpost")]
        public async Task<OkObjectResult> WithoutAuthMethod()
        {
            return await Task.FromResult(Ok(new { Message = "This method does not need any authentication " + Dns.GetHostName() }));
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("authmethodget")]
        public async Task<OkObjectResult> AuthMethodGet()
        {
            return await Task.FromResult(Ok(new { Message = "You will need authentication " + Dns.GetHostName() }));
        }

        [HttpGet]
        [Route("withoutauthmethodget")]
        public async Task<OkObjectResult> WithoutAuthMethodGet()
        {
            return await Task.FromResult(Ok(new { Message = "This method does not need any authentication " + Dns.GetHostName() }));
        }
        #endregion
    }

}
