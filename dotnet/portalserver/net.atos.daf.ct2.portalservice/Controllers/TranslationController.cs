using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Translation;
using net.atos.daf.ct2.translationservice;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;
namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Route("translation")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TranslationController : BaseController
    {
        private readonly AuditHelper _audit;
        private readonly ILog _logger;
        private readonly TranslationService.TranslationServiceClient _translationServiceClient;
        private readonly Mapper _mapper;

        //Constructor
        public TranslationController(TranslationService.TranslationServiceClient translationServiceClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _translationServiceClient = translationServiceClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _mapper = new Mapper();
            _audit = auditHelper;
        }


        [HttpPost]
        [Route("menutranslations")]
        public async Task<IActionResult> GetTranslations(TranslationRequest request)
        {
            try
            {
                if (request.Code == null || request.Type == null)
                {
                    return StatusCode(400, "Language code and type required.");
                }
                if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.Type))
                {
                    return StatusCode(400, "Language code and type required.");
                }
                if (!string.IsNullOrEmpty(request.Type.Trim()) && request.Type.Trim() != "Menu")
                {
                    if (!string.IsNullOrEmpty(request.Type.Trim()) && request.Type.Trim() != "Feature" && request.Type.Trim() != "Menu")
                    {
                        return StatusCode(400, "Invalid Type In Request ");
                    }
                    //return StatusCode(400, "Invalid Type In Request ");
                }

                _logger.Info("Get translation Menu  method get " + request.Code + " " + request.MenuId);

                TranslationsRequest obj = new TranslationsRequest();
                obj = _mapper.MapGetTranslations(request);

                TranslationsResponce translationsResponce = await _translationServiceClient.GetTranslationsAsync(obj);

                if (translationsResponce != null
                  && translationsResponce.Message == "There is an error In GetTranslation.")
                {
                    return StatusCode(500, "There is an error In GetTranslation.");
                }
                else if (translationsResponce != null && translationsResponce.Code == Responcecode.Success)
                {
                    return Ok(translationsResponce.TranslationsList);
                }
                else
                {
                    return StatusCode(404, "GetTranslations Response is null");
                }


            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("commontranslations")]
        public async Task<IActionResult> GetCommonTranslations([FromQuery] CodeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Languagecode.Trim()))
                {
                    return StatusCode(400, "Language code required.");
                }
                if (request.Languagecode.Any(char.IsDigit))
                {
                    return StatusCode(400, "Invalid langauge code.");
                }
                _logger.Info("Get translation Common method get " + request.Languagecode);

                CodeResponce CommontranslationResponseList = await _translationServiceClient.GetCommonTranslationsAsync(request);


                if (CommontranslationResponseList != null && CommontranslationResponseList.Code == Responcecode.Success)
                {
                    if (CommontranslationResponseList.CodeTranslationsList.Count > 0)
                    {
                        return Ok(CommontranslationResponseList.CodeTranslationsList);
                    }
                    else
                        return StatusCode(404, "Translations not found for provided details");

                }
                else
                {
                    return StatusCode(500, "Failed to fetch translations");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpGet]
        [Route("translations")]
        public async Task<IActionResult> GetLangagugeTranslationByKey([FromQuery] KeyRequest request)
        {
            try
            {
                //if (string.IsNullOrEmpty(request.Key))
                //{
                //    return StatusCode(400, " Key  required..");
                //}
                _logger.Info("GetLangagugeTranslationByKey  method " + request.Key);

                KeyResponce KeyResponseList = await _translationServiceClient.GetLangagugeTranslationByKeyAsync(request);


                if (KeyResponseList != null && KeyResponseList.Code == Responcecode.Success)
                {
                    if (KeyResponseList.KeyTranslationsList.Count > 0)
                    {
                        return Ok(KeyResponseList.KeyTranslationsList);
                    }
                    else
                    {
                        return StatusCode(404, "Translations not found for provide key");
                    }

                }
                else
                {
                    _logger.Error(KeyResponseList.Message);
                    return StatusCode(500, "Failed to fetch translations");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("keytranslationbylanguagecode")]
        public async Task<IActionResult> GetKeyTranslationByLanguageCode([FromQuery] KeyCodeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Key.Trim()) || string.IsNullOrEmpty(request.Languagecode.Trim()))
                {
                    return StatusCode(400, "Language code and key required.");
                }

                if (request.Languagecode.Any(char.IsDigit))
                {
                    return StatusCode(400, "Invalid langauge code.");
                }
                KeyCodeResponce CodeResponseList = await _translationServiceClient.GetKeyTranslationByLanguageCodeAsync(request);


                if (CodeResponseList.KeyCodeTranslationsList.Count() > 0 && CodeResponseList.Code == Responcecode.Success)
                {
                    return Ok(CodeResponseList.KeyCodeTranslationsList);
                }
                else
                {
                    return StatusCode(404, "Translations not found for provided details");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Failed to fetch translations");
            }
        }


        [HttpGet]
        [Route("translationsfordropdowns")]
        public async Task<IActionResult> GetTranslationsForDropDowns([FromQuery] dropdownnameRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Languagecode.Trim()) || string.IsNullOrEmpty(request.Dropdownname.Trim()))
                {
                    return StatusCode(400, "Language code and dropdown  required.");
                }
                if (request.Languagecode.Any(char.IsDigit))
                {
                    return StatusCode(400, "Invalid langauge code.");
                }
                if (request.Dropdownname.Any(char.IsDigit))
                {
                    return StatusCode(400, "Invalid dropdown name.");
                }
                _logger.Info("Drop down method get" + request.Dropdownname + request.Languagecode);

                dropdownnameResponce dropdownResponseList = await _translationServiceClient.GetTranslationsForDropDownsAsync(request);


                if (dropdownResponseList.DropdownnameTranslationsList.Count() > 0 && dropdownResponseList.Code == Responcecode.Success)
                {
                    return Ok(dropdownResponseList.DropdownnameTranslationsList);
                }
                else
                {
                    return StatusCode(404, "Translations not found for provided details");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Failed to fetch translations for dropdown");
            }

        }

        // [AllowAnonymous]
        [HttpPost]
        [Route("multipledropdowns")]
        public async Task<IActionResult> GetTranslationsFormultipleDropDowns(DropdownRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Languagecode.Trim()))
                {
                    return StatusCode(400, "Language code required.");
                }
                if (request.Languagecode.Any(char.IsDigit))
                {
                    return StatusCode(400, "Invalid langauge code.");
                }

                for (int i = 0; i < request.Dropdownname.Count; i++)
                {
                    if (string.IsNullOrEmpty(request.Dropdownname[i].Trim()))
                    {
                        return StatusCode(400, "Dropdownname invalid.");
                    }
                    if (request.Dropdownname[i].Any(char.IsDigit))
                    {
                        return StatusCode(400, "Dropdownname invalid.");
                    }
                }

                dropdownarrayRequest transdropdown = new dropdownarrayRequest();
                transdropdown = _mapper.MapDropdown(request);
                dropdownarrayResponce dropdownResponseList = await _translationServiceClient.GetTranslationsFormultipleDropDownsAsync(transdropdown);

                // var dropdownResponseList = await _translationServiceClient.GetTranslationsFormultipleDropDownsAsync(request);


                if (dropdownResponseList.DropdownnamearrayList.Count() > 0 && dropdownResponseList.Code == Responcecode.Success)
                {
                    return Ok(dropdownResponseList.DropdownnamearrayList);
                }
                else
                {
                    return StatusCode(404, "Translations not found for provided details");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Failed to fetch translations");
            }

        }

        [HttpGet]
        [Route("preferences")]
        public async Task<IActionResult> GetTranslationsPreferencDropDowns([FromQuery] PreferenceRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Languagecode.Trim()))
                {
                    return StatusCode(400, "Language code required.");
                }
                if (request.Languagecode.Any(char.IsDigit))
                {
                    return StatusCode(400, "Invalid langauge code.");
                }
                PreferenceResponse ResponseList = await _translationServiceClient.GetTranslationsPreferencDropDownsAsync(request);

                if (ResponseList.Code == Responcecode.Success)
                {
                    return Ok(ResponseList);
                }
                else
                {
                    return StatusCode(404, "Translations not found for provided details");
                }



            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Failed to fetch translations");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("languagecodes")]
        public async Task<IActionResult> GetAllLanguagecodes([FromQuery] Request request)
        {
            _logger.Info("All languages method get");
            try
            {
                TranslationListResponce ResponseList = await _translationServiceClient.GetAllLanguagecodesAsync(request);

                if (ResponseList != null
                     && ResponseList.Message == "There is an error In GetAllLanguagecodes.")
                {
                    return StatusCode(400, "Translations not found for provided details");
                }
                else if (ResponseList != null && ResponseList.Code == Responcecode.Success)
                {
                    return Ok(ResponseList.Languagelist);
                }
                else
                {
                    return StatusCode(404, "Translations not found for provided details");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Failed to fetch langauges");
            }


        }

        [HttpPost]
        [Route("Import")]
        // [AllowAnonymous]
        public async Task<IActionResult> InsertTranslationFileDetails(FileUploadRequest request)
        {
            try
            {
                _logger.Info("InsertTranslationFileDetails Method post");
                if (request.File_name == "" || request.File_name == null || request.File_size <= 0)
                {
                    return StatusCode(400, "File name and valid file size is required.");
                }
                if (request.File.Count() <= 0)
                {
                    return StatusCode(400, "File translations data is required.");
                }
                // request.file[0].code
                for (int i = 0; i < request.File.Count; i++)
                {
                    if (request.File[i].Code == null || request.File[i].Code == "")
                    {
                        return StatusCode(400, "invalid langauge code in file.");
                    }
                }

                TranslationUploadRequest transupload = new TranslationUploadRequest();
                transupload = _mapper.MapFileDetailRequest(request);

                TranslationUploadResponse ResponseList = await _translationServiceClient.InsertTranslationFileDetailsAsync(transupload);

                if (ResponseList != null && ResponseList.Code == Responcecode.Success)
                {
                    await _audit.AddLogs(DateTime.Now, "Translation Component",
                       "Translation service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                       "InsertTranslationFileDetails  method in Translation controller", 0, 0, JsonConvert.SerializeObject(request),
                        _userDetails);

                    return Ok(ResponseList);
                }
                else
                {
                    return StatusCode(500, "Failed to upload translations details");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Failed to upload translations details");
            }
        }

        // [AllowAnonymous]
        [HttpGet]
        [Route("getUploadDetails")]
        public async Task<IActionResult> GetFileUploadDetails([FromQuery] FileUploadDetailsRequest request)
        {
            try
            {
                _logger.Info("GetFileUploadDetails Method get");

                FileUploadDetailsResponse ResponseList = await _translationServiceClient.GetFileUploadDetailsAsync(request);

                if (ResponseList != null && ResponseList.Code == Responcecode.Success)
                {
                    if (request.FileID > 0 && ResponseList.Translationupload.Count > 0)
                    {
                        return Ok(ResponseList.Translationupload.FirstOrDefault().File);
                    }
                    return Ok(ResponseList.Translationupload);
                }
                else if (ResponseList != null && ResponseList.Code == Responcecode.NotFound)
                {
                    return StatusCode(404, "File Details not found.");
                }
                else // if responce code is failed 
                {
                    return StatusCode(500, "Failed to fetch file details");
                }
            }
            catch (Exception Ex)
            {
                _logger.Error(null, Ex);
                return StatusCode(500, "Failed to fetch file details");
            }
        }

        [HttpPost]
        [Route("ImportdtcWarning")]
        [AllowAnonymous]
        public async Task<IActionResult> ImportDTCWarningData(DTCWarningImportRequest request)
        {
            try
            {
                //Validation
                if (request.DtcWarningToImport.Count <= 0)
                {
                    return StatusCode(400, "DTC Warning Data is required.");
                }

                var dtcRequest = _mapper.ToImportDTCWarning(request);
                var dtcResponse = await _translationServiceClient.ImportDTCWarningDataAsync(dtcRequest);

                if (dtcResponse != null
                   && dtcResponse.Message == "There is an error importing dtc Warning Data.")
                {
                    return StatusCode(500, "There is an error importing  dtc Warning Data.");
                }
                else if (dtcResponse != null && dtcResponse.Code == Responcecode.Success)
                {
                    await _audit.AddLogs(DateTime.Now, "Translation Component",
                          "Translation service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                          "ImportDTCWarningData  method in Translation controller", 0, 0, JsonConvert.SerializeObject(request),
                           _userDetails);
                    return Ok(dtcResponse);
                }
                else if (dtcResponse != null && dtcResponse.Message == "violates foreign key constraint for Icon_ID , Please enter valid data for Warning_Class and Warning_Number")
                {
                    return StatusCode(400, dtcResponse.Message);
                }
                else if (dtcResponse != null && dtcResponse.Code == Responcecode.Failed)
                {
                    return Ok(dtcResponse);
                }
                else
                {
                    return StatusCode(500, "Warning response is null");
                }

            }
            catch (Exception ex)
            {
                await _audit.AddLogs(DateTime.Now, "Translation Component",
                      "Translation service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                      "ImportDTCWarningData  method in Translation controller", 0, 0, JsonConvert.SerializeObject(request),
                       _userDetails);
                _logger.Error(null, ex);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_CONSTRAINT))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }

        }

        [HttpGet]
        [Route("getdtcWarningDetails")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDTCWarningData([FromQuery] WarningGetRequest Request)
        {
            try
            {

                var response = await _translationServiceClient.GetDTCWarningDataAsync(Request);


                if (response != null && response.Code == Responcecode.Success)
                {
                    if (response.DtcGetDataResponse != null && response.DtcGetDataResponse.Count > 0)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(404, "DTC warning details are not found.");
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
        [Route("UpdatedtcWarning")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateDTCWarningData(DTCWarningImportRequest request)
        {
            try
            {
                //Validation
                if (request.DtcWarningToImport.Count <= 0)
                {
                    return StatusCode(400, "DTC Warning Data is required.");
                }

                var dtcRequest = _mapper.ToImportDTCWarning(request);
                var DTCResponse = await _translationServiceClient.UpdateDTCWarningDataAsync(dtcRequest);

                if (DTCResponse != null
                   && DTCResponse.Message == "There is an error updating dtc Warning Data.")
                {
                    return StatusCode(500, "There is an error updating  dtc Warning Data.");
                }
                else if (DTCResponse != null && DTCResponse.Code == Responcecode.Success)
                {
                    await _audit.AddLogs(DateTime.Now, "Translation Component",
                      "Translation service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                      "UpdateDTCWarningData  method in Translation controller", 0, 0, JsonConvert.SerializeObject(request),
                       _userDetails);
                    return Ok(DTCResponse);
                }
                else
                {
                    return StatusCode(500, "Warning response is null");
                }
            }
            catch (Exception ex)
            {
                await _audit.AddLogs(DateTime.Now, "Translation Component",
                     "Translation service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                     "UpdateDTCWarningData  method in Translation controller", 0, 0, JsonConvert.SerializeObject(request),
                      _userDetails);
                _logger.Error(null, ex);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_CONSTRAINT))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }
        }

        #region  Terms And Conditions

        [HttpPost]
        [Route("tac/adduseracceptedtac")]
        // [AllowAnonymous]
        public async Task<IActionResult> AddUserAcceptedTermCondition(AccountTermsCondition request)
        {
            try
            {
                //Validation
                if (request.Account_Id <= 0)
                {
                    return StatusCode(400, "Account Id is required.");
                }

                if (request.Organization_Id <= 0)
                {
                    return StatusCode(400, "Organization Id is required.");
                }

                if (request.Terms_And_Condition_Id <= 0)
                {
                    return StatusCode(400, "Terms And Conditions Id is required.");
                }
                //Assign context orgId
                request.Organization_Id = GetContextOrgId();
                var termsAndCondRequest = _mapper.ToAcceptedTermConditionRequestEntity(request);
                var termsAndCondResponse = await _translationServiceClient.AddUserAcceptedTermConditionAsync(termsAndCondRequest);

                if (termsAndCondResponse != null
                   && termsAndCondResponse.Message == "There is an error in Terms And Conditions Data.")
                {
                    return StatusCode(500, "There is an error importing Terms And Conditions.");
                }
                else if (termsAndCondResponse != null && termsAndCondResponse.Code == Responcecode.Success)
                {
                    await _audit.AddLogs(DateTime.Now, "Translation Component",
                                        "Translation service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                        "AddUserAcceptedTermCondition  method in Translation controller", 0, termsAndCondResponse.AcceptedTermCondition != null ? termsAndCondResponse.AcceptedTermCondition.Id : 0,
                                        JsonConvert.SerializeObject(request),
                                         _userDetails);
                    return Ok(termsAndCondResponse);
                }
                else
                {
                    return StatusCode(404, "Terms And Conditions response is null");
                }

            }
            catch (Exception ex)
            {
                await _audit.AddLogs(DateTime.Now, "Translation Component",
                                        "Translation service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                        "AddUserAcceptedTermCondition  method in Translation controller", 0, 0,
                                        JsonConvert.SerializeObject(request),
                                         _userDetails);
                _logger.Error(null, ex);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_CONSTRAINT))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }

        }

        [HttpGet]
        [Route("tac/getallversionsfortac")]

        public async Task<IActionResult> GetAllVersionNo([FromQuery] VersionByID objVersionByID)
        {
            try
            {

                switch (objVersionByID.LevelCode)
                {
                    case 0:
                        return StatusCode(400, "Level code is required.");
                    case 30:
                    case 40:
                        if (objVersionByID.OrgId <= 0)
                            return StatusCode(400, "Organization id is required.");
                        if (objVersionByID.AccountId <= 0)
                            return StatusCode(400, "Account id is required.");
                        break;

                }
                //Assign context orgId
                objVersionByID.OrgId = GetContextOrgId();
                net.atos.daf.ct2.translationservice.VersionID objVersionID = new VersionID();
                objVersionID.LevelCode = objVersionByID.LevelCode;
                objVersionID.OrgId = objVersionByID.OrgId;
                objVersionID.AccountId = objVersionByID.AccountId;
                var response = await _translationServiceClient.GetAllVersionNoAsync(objVersionID);
                TermsAndConditions termsAndConditions = new TermsAndConditions();
                //termsAndConditions=_mapper.

                if (response != null && response.Code == Responcecode.Success)
                {
                    if (response.VersionNos != null && response.VersionNos.Count > 0)
                    {
                        return Ok(response.VersionNos);
                    }
                    else
                    {
                        return StatusCode(404, "version nos details are not found.");
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

        [HttpGet]
        [Route("tac/gettacforversionno")]
        public async Task<IActionResult> GetTermConditionForVersionNo([FromQuery] string versionNo, string languageCode)
        {
            try
            {
                if (string.IsNullOrEmpty(versionNo))
                {
                    return StatusCode(400, "Version number is required.");
                }

                VersionNoRequest request = new VersionNoRequest();
                request.VersionNo = versionNo;
                request.Languagecode = languageCode;
                TermCondDetailsReponse response = await _translationServiceClient.GetTermConditionForVersionNoAsync(request);
                if (response.TermCondition != null && response.Code == translationservice.Responcecode.Failed)
                {
                    return StatusCode(500, "There is an error fetching Terms and condition.");
                }
                else if (response.TermCondition.Count() > 0 && response.Code == translationservice.Responcecode.Success)
                {
                    return Ok(response.TermCondition);
                }
                else
                {
                    return StatusCode(404, "Terms and condition is null");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("tac/getacceptedbyusertac")]
        public async Task<IActionResult> GetAcceptedTermConditionByUser([FromQuery] int AccountId, int OrganizationId)
        {
            try
            {
                if (OrganizationId <= 0)
                {
                    return StatusCode(400, "Organization Id is required.");
                }
                //Assign context orgId
                OrganizationId = GetContextOrgId();

                UserAcceptedTermConditionRequest request = new UserAcceptedTermConditionRequest();
                request.AccountId = AccountId;
                request.OrganizationId = OrganizationId;
                TermCondDetailsReponse response = await _translationServiceClient.GetAcceptedTermConditionByUserAsync(request);
                if (response.TermCondition != null && response.Code == translationservice.Responcecode.Failed)
                {
                    return StatusCode(500, "There is an error fetching Terms and condition.");
                }
                else if (response.TermCondition.Count() > 0 && response.Code == translationservice.Responcecode.Success)
                {
                    return Ok(response.TermCondition);
                }
                else
                {
                    return StatusCode(404, "Terms and condition is null");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("tac/getlatesttac")]
        public async Task<IActionResult> GetLatestTermCondition([FromQuery] int AccountId, int OrganizationId)
        {
            try
            {

                if (OrganizationId <= 0 || AccountId <= 0)
                {
                    return StatusCode(400, "Organization Id and Account Id both are required.");
                }
                //Assign context orgId
                OrganizationId = GetContextOrgId();

                UserAcceptedTermConditionRequest request = new UserAcceptedTermConditionRequest();
                request.AccountId = AccountId;
                request.OrganizationId = OrganizationId;
                TermCondDetailsReponse response = await _translationServiceClient.GetLatestTermConditionAsync(request);
                if (response.TermCondition != null && response.Code == translationservice.Responcecode.Failed)
                {
                    return StatusCode(500, "There is an error fetching Terms and condition.");
                }
                else if (response.TermCondition.Count() > 0 && response.Code == translationservice.Responcecode.Success)
                {
                    return Ok(response.TermCondition);
                }
                else
                {
                    return StatusCode(404, "Terms and condition is not avaliable");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("tac/checkuseracceptedtac")]
        public async Task<IActionResult> CheckUserAcceptedTermCondition([FromQuery] int AccountId, int OrganizationId)
        {
            try
            {

                if (OrganizationId <= 0 || AccountId <= 0)
                {
                    return StatusCode(400, "Organization Id and Account Id both are required.");
                }
                //Assign context orgId
                OrganizationId = GetContextOrgId();
                UserAcceptedTermConditionRequest request = new UserAcceptedTermConditionRequest();
                request.AccountId = AccountId;
                request.OrganizationId = OrganizationId;
                UserAcceptedTermConditionResponse response = await _translationServiceClient.CheckUserAcceptedTermConditionAsync(request);
                if (response != null && response.Code == translationservice.Responcecode.Failed)
                {
                    return StatusCode(500, "There is an error fetching Terms and condition.");
                }
                else if (response != null && response.Code == translationservice.Responcecode.Success)
                {
                    return Ok(response.IsUserAcceptedTC);
                }
                else
                {
                    return StatusCode(404, "Terms and condition is not avaliable");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpPost]
        [Route("tac/uploadtac")]
        //[AllowAnonymous]
        public async Task<IActionResult> UploadTermsAndCondition(TermsandConFileDataList request)
        {
            _logger.Info("UploadTermsAndCondition Method post");

            UploadTermandConditionRequestList objUploadTermandConditionRequestList = new UploadTermandConditionRequestList();
            try
            {
                long startdatetime = 0; long enddatetime = 0;
                if (request.Start_date != string.Empty)
                {
                    startdatetime = UTCHandling.GetUTCFromDateTime(Convert.ToDateTime(request.Start_date));
                }

                if (request.End_date != string.Empty)
                {//Assign only if enddate is passed
                    enddatetime = UTCHandling.GetUTCFromDateTime(Convert.ToDateTime(request.End_date));
                }
                objUploadTermandConditionRequestList.StartDate = startdatetime;
                objUploadTermandConditionRequestList.EndDate = enddatetime;
            }
            catch (Exception)
            {
                _logger.Info($"Not valid date in subcription event - {Newtonsoft.Json.JsonConvert.SerializeObject(request.Start_date)}");
                return StatusCode(400, string.Empty); ;
            }
            objUploadTermandConditionRequestList.CreatedBy = request.Created_by;
            foreach (var item in request.Data)
            {
                string[] aryFileNameContent = item.FileName.Split('_');
                UploadTermandConditionRequest objUploadTermandConditionRequest = new UploadTermandConditionRequest();
                if (aryFileNameContent != null && aryFileNameContent.Length > 1)
                {

                    objUploadTermandConditionRequest.Code = aryFileNameContent[2].ToUpper();
                    objUploadTermandConditionRequest.Versionno = aryFileNameContent[1].ToUpper();
                    objUploadTermandConditionRequest.FileName = item.FileName;
                    objUploadTermandConditionRequest.Description = ByteString.CopyFrom(item.Description);
                    objUploadTermandConditionRequestList.Data.Add(objUploadTermandConditionRequest);
                }
                else
                {
                    return StatusCode(400, string.Empty);
                }

            }
            var data = await _translationServiceClient.UploadTermsAndConditionAsync(objUploadTermandConditionRequestList);
            _logger.Info("UploadTermsAndCondition Service called");

            if (data != null && data.Code == translationservice.Responcecode.Failed)
            {
                return StatusCode(500, "There is an error while inserting/updating terms and conditions.");
            }
            else if (data != null && data.Code == translationservice.Responcecode.Success)
            {
                await _audit.AddLogs(DateTime.Now, "Translation Component",
                                        "Translation service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                        "UploadTermsAndCondition  method in Translation controller", 0, 0,
                                        JsonConvert.SerializeObject(request),
                                         _userDetails);
                return Ok(data.Uploadedfilesaction);
            }
            else
            {
                return StatusCode(404, "term and condition is null");
            }
        }

        #endregion

        [HttpPost]
        [Route("updatedtcIconDetails")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateDTCTranslationIcon(DTCWarningIconUpdateRequest request)
        {
            try
            {
                //Validation
                if (request.DtcWarningUpdateIcon.Count <= 0)
                {
                    return StatusCode(400, "DTC Warning Icon Data is required.");
                }

                foreach (var item in request.DtcWarningUpdateIcon)
                {
                    if (item.Icon.Length <= 0 || item.Name == null || item.Name == "")
                    {
                        return StatusCode(400, "Icon name and valid Icon size is required.");
                    }
                }

                var dtcRequest = _mapper.ToImportDTCWarningIcon(request);
                var DTCResponse = await _translationServiceClient.UpdateDTCTranslationIconAsync(dtcRequest);

                if (DTCResponse != null
                   && DTCResponse.Message == "There is an error updating dtc Warning Icon.")
                {
                    return StatusCode(500, "There is an error updating  dtc Warning Icon.");
                }
                else if (DTCResponse != null && DTCResponse.Code == Responcecode.Success)
                {
                    await _audit.AddLogs(DateTime.Now, "Translation Component",
                      "Translation service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                      "UpdateDTCTranslationIcon  method in Translation controller", 0, 0, JsonConvert.SerializeObject(request),
                       _userDetails);
                    return Ok(DTCResponse);
                }
                else if (DTCResponse.Message == "File Name not exist .")
                {
                    return Ok(DTCResponse.Message);
                }
                else
                {
                    return StatusCode(500, "Warning response is null");
                }

            }
            catch (Exception ex)
            {
                await _audit.AddLogs(DateTime.Now, "Translation Component",
                     "Translation service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                     "UpdateDTCTranslationIcon  method in Translation controller", 0, 0, JsonConvert.SerializeObject(request),
                      _userDetails);
                _logger.Error(null, ex);
                if (ex.Message.Contains(PortalConstants.ExceptionKeyWord.FK_CONSTRAINT))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. " + ex.Message + " " + ex.StackTrace);
            }

        }

        [HttpGet]
        [Route("getdtcIconDetails")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDTCTranslationIcon([FromQuery] IconGetRequest Request)
        {
            try
            {
                if (Request.Id > 0)
                {
                    var response = await _translationServiceClient.GetDTCTranslationIconAsync(Request);


                    if (response != null)
                    {
                        if (response.IconData != null && response.IconData.Count > 0)
                        {
                            return Ok(response);
                        }
                        else
                        {
                            return StatusCode(404, "DTC warning Icon details are not found.");
                        }
                    }
                    else
                    {
                        return StatusCode(500, response.Message);
                    }
                }
                else
                {
                    return StatusCode(400, "Valid ID is Required");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

    }
}