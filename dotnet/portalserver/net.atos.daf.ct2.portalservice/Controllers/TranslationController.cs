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
                    return StatusCode(400, "Langauge code and type required..");
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
                    return Ok(translationsResponce);
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
                if (string.IsNullOrEmpty(request.Langaugecode))
                {
                    return StatusCode(400, "Langauge code  required..");
                }
                _logger.LogInformation("Get translation Common  method get " + request.Langaugecode);

                CodeResponce CommontranslationResponseList = await _translationServiceClient.GetCommonTranslationsAsync(request);


                if (CommontranslationResponseList != null
                 && CommontranslationResponseList.Message == "There is an error In GetCommonTranslations.")
                {
                    return StatusCode(500, "There is an error In GetCommonTranslations.");
                }
                else if (CommontranslationResponseList != null && CommontranslationResponseList.Code == Responcecode.Success)
                {
                    return Ok(CommontranslationResponseList);
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
                    return Ok(KeyResponseList);
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
                    return Ok(CodeResponseList);
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
                if (string.IsNullOrEmpty(request.Langaugecode.Trim()) || string.IsNullOrEmpty(request.Dropdownname.Trim()))
                {
                    return StatusCode(400, "Langauge code and dropdown  required..");
                }
                _logger.LogInformation("Drop down method get" + request.Dropdownname + request.Langaugecode);

                dropdownnameResponce dropdownResponseList = await _translationServiceClient.GetTranslationsForDropDownsAsync(request);


                if (dropdownResponseList != null
                 && dropdownResponseList.Message == "There is an error In GetKeyTranslationByLanguageCode.")
                {
                    return StatusCode(500, "There is an error In GetKeyTranslationByLanguageCode.");
                }
                else if (dropdownResponseList != null && dropdownResponseList.Code == Responcecode.Success)
                {
                    return Ok(dropdownResponseList);
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
                if (string.IsNullOrEmpty(request.Langaugecode.Trim()))
                {
                    return StatusCode(400, "Langauge code and dropdown  required..");
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
                    return Ok(dropdownResponseList);
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
                if (string.IsNullOrEmpty(request.Langaugecode.Trim()))
                {
                    return StatusCode(400, "Langauge code and dropdown  required..");
                }
                PreferenceResponse ResponseList = await _translationServiceClient.GetTranslationsPreferencDropDownsAsync(request);

                if (ResponseList != null
                && ResponseList.Message == "There is an error In GetTranslationsPreferencDropDowns.")
                {
                    return StatusCode(500, "There is an error In GetTranslationsPreferencDropDowns.");
                }
                else if (ResponseList != null && ResponseList.Code == Responcecode.Success)
                {
                    return Ok(ResponseList);
                }
                else
                {
                    return StatusCode(500, "GetTranslationsPreferencDropDowns Response is null");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("languagecodes")]
        public async Task<IActionResult> GetAllLangaugecodes([FromQuery] Request request)
        {
            _logger.LogInformation("All langauges method get");

            TranslationListResponce ResponseList = await _translationServiceClient.GetAllLangaugecodesAsync(request);

            if (ResponseList != null
                 && ResponseList.Message == "There is an error In GetAllLangaugecodes.")
            {
                return StatusCode(500, "There is an error In GetAllLangaugecodes.");
            }
            else if (ResponseList != null && ResponseList.Code == Responcecode.Success)
            {
                return Ok(ResponseList);
            }
            else
            {
                return StatusCode(500, "GetAllLangaugecodes Response is null");
            }
        }
    }
}