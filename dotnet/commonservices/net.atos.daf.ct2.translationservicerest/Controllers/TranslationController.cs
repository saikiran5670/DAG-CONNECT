using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.translation.Enum;
using net.atos.daf.ct2.translationservicerest.Entity;

namespace net.atos.daf.ct2.translationservicerest.Controllers
{

    [ApiController]
    [Route("translation")]
    public class TranslationController : ControllerBase
    {
        private readonly ILogger<TranslationController> _logger;


        private readonly ITranslationManager translationmanager;

        public TranslationController(ILogger<TranslationController> logger, ITranslationManager _TranslationManager)
        {
            _logger = logger;
            translationmanager = _TranslationManager;
        }

        [HttpPost]
        [Route("menutranslations")]
        public async Task<IActionResult> GetTranslations(Translations request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.Type))
                {
                    return StatusCode(400, "Langauge code and type required..");
                }
                _logger.LogInformation("Get translation Menu  method get " + request.Code + " " + request.MenuId);
                List<Translations> responce = new List<Translations>();
                // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
                var translations = await translationmanager.GetTranslationsByMenu(request.MenuId, (Translationenum.MenuType)Enum.Parse(typeof(Translationenum.MenuType), request.Type.ToString()), request.Code);


                return Ok(translations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("commontranslations")]
        public async Task<IActionResult> GetCommonTranslations(string LanguageCode)
        {
            try
            {
                if (string.IsNullOrEmpty(LanguageCode))
                {
                    return StatusCode(400, "Langauge code  required..");
                }
                _logger.LogInformation("Get translation Common  method get " + LanguageCode);

                List<Translations> responce = new List<Translations>();
                // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
                var translations = await translationmanager.GetTranslationsByMenu(0, Translationenum.MenuType.Menu, LanguageCode);


                return Ok(translations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("languagetranslationsbykey")]
        public async Task<IActionResult> GetLangagugeTranslationByKey(string key)
        {
            try
            {
                _logger.LogInformation("Get translation key method get " + key);
                var translation = await translationmanager.GetLangagugeTranslationByKey(key);
                return Ok(translation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }


        }

        [HttpGet]
        [Route("keytranslationbylanguagecode")]
        public async Task<IActionResult> GetKeyTranslationByLanguageCode(string languagecode, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(languagecode) || string.IsNullOrEmpty(key))
                {
                    return StatusCode(400, "Langauge code  and key required..");
                }
                _logger.LogInformation("Get translation key lan code method get " + languagecode + " " + key);
                var translation = await translationmanager.GetKeyTranslationByLanguageCode(languagecode, key);
                return Ok(translation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }

        }

        [HttpGet]
        [Route("translationsfordropdowns")]
        public async Task<IActionResult> GetTranslationsForDropDowns(string Dropdownname, string languagecode)
        {
            try
            {
                if (string.IsNullOrEmpty(languagecode.Trim()) || string.IsNullOrEmpty(Dropdownname.Trim()))
                {
                    return StatusCode(400, "Langauge code and dropdown  required..");
                }
                _logger.LogInformation("Drop down method get" + Dropdownname + languagecode);
                var translation = await translationmanager.GetTranslationsForDropDowns(Dropdownname, languagecode);
                return Ok(translation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }

        }

        [HttpPost]
        [Route("multipledropdowns")]
        public async Task<IActionResult> GetTranslationsFormultipleDropDowns(string[] Dropdownname, string languagecode)
        {
            try
            {
                if (string.IsNullOrEmpty(languagecode.Trim()))
                {
                    return StatusCode(400, "Langauge code and dropdown  required..");
                }
                if (Dropdownname.Length == 0)
                {
                    return StatusCode(400, "Dropdown names are required.");
                }
                List<Translations> Dropdowns = new List<Translations>();
                foreach (var item in Dropdownname)
                {
                    _logger.LogInformation("Drop down method get" + item + languagecode);
                    Dropdowns.AddRange(await translationmanager.GetTranslationsForDropDowns(item, languagecode));
                }


                return Ok(Dropdowns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }

        }


        [HttpGet]
        [Route("preferences")]
        public async Task<IActionResult> GetTranslationsPreferencDropDowns(string languagecode)
        {
            try
            {
                if (string.IsNullOrEmpty(languagecode.Trim()))
                {
                    return StatusCode(400, "Langauge code and dropdown  required..");
                }

                PreferenceResponce Dropdowns = new PreferenceResponce();

                foreach (var item in Dropdowns.GetType().GetProperties())
                {
                    _logger.LogInformation("Drop down method get" + item.Name + languagecode);
                    var Translations = await translationmanager.GetTranslationsForDropDowns(item.Name, languagecode);

                    switch (item.Name)
                    {
                        case "language":
                            Dropdowns.language = new List<Translations>();
                            Dropdowns.language.AddRange(Translations);
                            // code block
                            break;
                        case "timezone":
                            Dropdowns.timezone = new List<Translations>();

                            Dropdowns.timezone.AddRange(Translations);
                            // code block
                            break;
                        case "unit":
                            Dropdowns.unit = new List<Translations>();

                            Dropdowns.unit.AddRange(Translations);
                            // code block
                            break;
                        case "currency":
                            Dropdowns.currency = new List<Translations>();

                            Dropdowns.currency.AddRange(Translations);
                            // code block
                            break;
                        case "landingpagedisplay":
                            Dropdowns.landingpagedisplay = new List<Translations>();

                            Dropdowns.landingpagedisplay.AddRange(Translations);
                            // code block
                            break;
                        case "dateformat":
                            Dropdowns.dateformat = new List<Translations>();

                            Dropdowns.dateformat.AddRange(Translations);
                            // code block
                            break;
                        case "timeformat":
                            Dropdowns.timeformat = new List<Translations>();

                            Dropdowns.timeformat.AddRange(Translations);
                            // code block
                            break;
                        case "vehicledisplay":
                            Dropdowns.vehicledisplay = new List<Translations>();

                            Dropdowns.vehicledisplay.AddRange(Translations);
                            // code block
                            break;
                        default:
                            // code block
                            break;
                    }
                }


                return Ok(Dropdowns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }

        }



        [HttpGet]
        [Route("languagecodes")]
        public async Task<IActionResult> GetAllLangaugecodes()
        {
            _logger.LogInformation("All langauges method get");
            // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
            var translations = await translationmanager.GetAllLanguageCode();
            return Ok(translations);
        }


    }
}
