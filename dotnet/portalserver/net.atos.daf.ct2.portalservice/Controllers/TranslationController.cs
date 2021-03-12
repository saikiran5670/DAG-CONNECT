using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.utilities;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.translationservice;
using net.atos.daf.ct2.portalservice.Entity.Translation;


namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Route("translation")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly ILogger<TranslationController> _logger;
        private readonly TranslationService.TranslationServiceClient _translationServiceClient;
        private readonly Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";

        //Constructor
        public TranslationController(TranslationService.TranslationServiceClient translationServiceClient, ILogger<TranslationController> logger)
        {
            _translationServiceClient = translationServiceClient;
            _logger = logger;
            _mapper = new Mapper();

        }


        [HttpPost]
        [Route("menutranslations")]
        public async Task<IActionResult> GetTranslations(TranslationsRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.Type))
                {
                    return StatusCode(400, "Language code and type required..");
                }
                _logger.LogInformation("Get translation Menu  method get " + request.Code + " " + request.MenuId);

                TranslationsResponce translationsResponce = await _translationServiceClient.GetTranslationsAsync(request);

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
                    return StatusCode(500, "GetTranslations Response is null");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("commontranslations")]
        public async Task<IActionResult> GetCommonTranslations([FromQuery]CodeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Languagecode))
                {
                    return StatusCode(400, "Language code  required..");
                }
                _logger.LogInformation("Get translation Common  method get " + request.Languagecode);

                CodeResponce CommontranslationResponseList = await _translationServiceClient.GetCommonTranslationsAsync(request);


                if (CommontranslationResponseList != null
                 && CommontranslationResponseList.Message == "There is an error In GetCommonTranslations.")
                {
                    return StatusCode(500, "There is an error In GetCommonTranslations.");
                }
                else if (CommontranslationResponseList != null && CommontranslationResponseList.Code == Responcecode.Success)
                {
                    return Ok(CommontranslationResponseList.CodeTranslationsList);
                }
                else
                {
                    return StatusCode(500, "GetCommonTranslations Response is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpGet]
        [Route("languagetranslationsbykey")]
        public async Task<IActionResult> GetLangagugeTranslationByKey([FromQuery] KeyRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Key))
                {
                    return StatusCode(400, " Key  required..");
                }
                _logger.LogInformation("GetLangagugeTranslationByKey  method " + request.Key);

                KeyResponce KeyResponseList = await _translationServiceClient.GetLangagugeTranslationByKeyAsync(request);


                if (KeyResponseList != null
                 && KeyResponseList.Message == "There is an error In GetLangagugeTranslationByKey.")
                {
                    return StatusCode(500, "There is an error In GetLangagugeTranslationByKey.");
                }
                else if (KeyResponseList != null && KeyResponseList.Code == Responcecode.Success)
                {
                    return Ok(KeyResponseList.KeyTranslationsList);
                }
                else
                {
                    return StatusCode(500, "GetLangagugeTranslationByKey Response is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("keytranslationbylanguagecode")]
        public async Task<IActionResult> GetKeyTranslationByLanguageCode([FromQuery] KeyCodeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Key))
                {
                    return StatusCode(400, " Key  required..");
                }
                _logger.LogInformation("GetKeyTranslationByLanguageCode  method " + request.Key);

                KeyCodeResponce CodeResponseList = await _translationServiceClient.GetKeyTranslationByLanguageCodeAsync(request);


                if (CodeResponseList != null
                 && CodeResponseList.Message == "There is an error In GetKeyTranslationByLanguageCode.")
                {
                    return StatusCode(500, "There is an error In GetKeyTranslationByLanguageCode.");
                }
                else if (CodeResponseList != null && CodeResponseList.Code == Responcecode.Success)
                {
                    return Ok(CodeResponseList.KeyCodeTranslationsList);
                }
                else
                {
                    return StatusCode(500, "GetKeyTranslationByLanguageCode Response is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
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
                    return StatusCode(400, "Language code and dropdown  required..");
                }
                _logger.LogInformation("Drop down method get" + request.Dropdownname + request.Languagecode);

                dropdownnameResponce dropdownResponseList = await _translationServiceClient.GetTranslationsForDropDownsAsync(request);


                if (dropdownResponseList != null
                 && dropdownResponseList.Message == "There is an error In GetKeyTranslationByLanguageCode.")
                {
                    return StatusCode(500, "There is an error In GetKeyTranslationByLanguageCode.");
                }
                else if (dropdownResponseList != null && dropdownResponseList.Code == Responcecode.Success)
                {
                    return Ok(dropdownResponseList.DropdownnameTranslationsList);
                }
                else
                {
                    return StatusCode(500, "GetKeyTranslationByLanguageCode Response is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }

        }

        [HttpPost]
        [Route("multipledropdowns")]
        public async Task<IActionResult> GetTranslationsFormultipleDropDowns(DropdownRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Languagecode.Trim()))
                {
                    return StatusCode(400, "Language code and dropdown  required..");
                }

                dropdownarrayRequest transdropdown = new dropdownarrayRequest();
                transdropdown = _mapper.MapDropdown(request);
                dropdownarrayResponce dropdownResponseList = await _translationServiceClient.GetTranslationsFormultipleDropDownsAsync(transdropdown);

               // var dropdownResponseList = await _translationServiceClient.GetTranslationsFormultipleDropDownsAsync(request);


                if (dropdownResponseList != null
                && dropdownResponseList.Message == "There is an error In GetTranslationsFormultipleDropDowns.")
                {
                    return StatusCode(500, "There is an error In GetTranslationsFormultipleDropDowns.");
                }
                else if (dropdownResponseList != null && dropdownResponseList.Code == Responcecode.Success)
                {
                    return Ok(dropdownResponseList.DropdownnamearrayList);
                }
                else
                {
                    return StatusCode(500, "GetTranslationsFormultipleDropDowns Response is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
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
                    return StatusCode(400, "Language code and dropdown  required..");
                }
                PreferenceResponse ResponseList = await _translationServiceClient.GetTranslationsPreferencDropDownsAsync(request);

                
                    return Ok(ResponseList);
              

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("languagecodes")]
        public async Task<IActionResult> GetAllLanguagecodes([FromQuery] Request request)
        {
            _logger.LogInformation("All languages method get");

            TranslationListResponce ResponseList = await _translationServiceClient.GetAllLanguagecodesAsync(request);

            if (ResponseList != null
                 && ResponseList.Message == "There is an error In GetAllLanguagecodes.")
            {
                return StatusCode(500, "There is an error In GetAllLanguagecodes.");
            }
            else if (ResponseList != null && ResponseList.Code == Responcecode.Success)
            {
                return Ok(ResponseList.Languagelist);
            }
            else
            {
                return StatusCode(500, "GetAllLanguagecodes Response is null");
            }
        }

        [HttpPost]
        [Route("Import")]
        public async Task<IActionResult> InsertTranslationFileDetails(FileUploadRequest request)
        {
            _logger.LogInformation("InsertTranslationFileDetails Method post");

            TranslationUploadRequest transupload = new TranslationUploadRequest();
            transupload = _mapper.MapFileDetailRequest(request);

            TranslationUploadResponse ResponseList = await _translationServiceClient.InsertTranslationFileDetailsAsync(transupload);

            if (ResponseList != null
                 && ResponseList.Message == "There is an error In InsertTranslationFileDetails.")
            {
                return StatusCode(500, "There is an error In InsertTranslationFileDetails.");
            }
            else if (ResponseList != null && ResponseList.Code == Responcecode.Success)
            {
                return Ok(ResponseList);
            }
            else
            {
                return StatusCode(500, "InsertTranslationFileDetails Response is null");
            }

        }
        [HttpGet]
        [Route("UploadDetails")]
        public async Task<IActionResult> GetFileUploadDetails([FromQuery] FileUploadDetailsRequest request)
        {
            _logger.LogInformation("GetFileUploadDetails Method get");

            FileUploadDetailsResponse ResponseList = await _translationServiceClient.GetFileUploadDetailsAsync(request);

            if (ResponseList != null
                 && ResponseList.Message == "There is an error In GetFileUploadDetails.")
            {
                return StatusCode(500, "There is an error In GetFileUploadDetails.");
            }
            else if (ResponseList != null && ResponseList.Code == Responcecode.Success)
            {
                return Ok(ResponseList);
            }
            else
            {
                return StatusCode(500, "GetFileUploadDetails Response is null");
            }

        }

    }
}