using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.Enum;
using net.atos.daf.ct2.translation.entity;
using static net.atos.daf.ct2.translation.Enum.translationenum;

namespace net.atos.daf.ct2.translationservicerest.Controllers
{

    [ApiController]
    [Route("translation")]
    public class TranslationController : ControllerBase
    {
         private readonly ILogger<TranslationController> _logger;

       
        private readonly ITranslationManager translationmanager;
        
        public TranslationController(ILogger<TranslationController> logger,ITranslationManager _TranslationManager)
        {
            _logger = logger;
            translationmanager=_TranslationManager;
        }

        [HttpPost]
        [Route("menutranslations")]
         public async  Task<IActionResult> GetTranslations(Translations request)
        {
          try
          {
            if(string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.Type))
            {
                return StatusCode(400, "Langauge code and type required..");
            }
            _logger.LogInformation("Get translation Menu  method get "  + request.Code + " "+ request.MenuId);
             List<Translations> responce = new List<Translations>();
            // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
            var translations = await translationmanager.GetTranslationsByMenu(request.MenuId,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString()),request.Code);
            

            return Ok(translations);
          }
          catch(Exception ex)
          {
                    _logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
          }
        }

        [HttpGet]
        [Route("commontranslations")]
        public async  Task<IActionResult> GetCommonTranslations(string LanguageCode)
        {
          try
          {
            if(string.IsNullOrEmpty(LanguageCode))
            {
                return StatusCode(400, "Langauge code  required..");
            }
            _logger.LogInformation("Get translation Common  method get "  + LanguageCode);

             List<Translations> responce = new List<Translations>();
            // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
            var translations = await translationmanager.GetTranslationsByMenu(0,translationenum.MenuType.Menu,LanguageCode);
            

          return Ok(translations);
          }
          catch(Exception ex)
          {
                    _logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
          }
        }

        [HttpGet]
        [Route("languagetranslationsbykey")]
        public async Task<IActionResult> GetLangagugeTranslationByKey(string key)
        {
          try
          {
            _logger.LogInformation("Get translation key method get "  + key);
             var translation = await translationmanager.GetLangagugeTranslationByKey(key);
             return Ok(translation);
          }
          catch(Exception ex)
          {
                    _logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
          }
         

        }

        [HttpGet]
        [Route("keytranslationbylanguagecode")]
         public async  Task<IActionResult> GetKeyTranslationByLanguageCode(string languagecode,string key)
        {
            try
            {
              if(string.IsNullOrEmpty(languagecode) || string.IsNullOrEmpty(key))
            {
                return StatusCode(400, "Langauge code  and key required..");
            }
              _logger.LogInformation("Get translation key lan code method get " + languagecode +" " + key);
              var translation = await translationmanager.GetKeyTranslationByLanguageCode(languagecode,key);
              return Ok(translation);
            }
            catch(Exception ex)
            {
                      _logger.LogError(ex.Message +" " +ex.StackTrace);
                      return StatusCode(500,"Internal Server Error.");
            }

        }

        [HttpGet]
        [Route("translationsfordropdowns")]
         public async  Task<IActionResult> GetTranslationsForDropDowns(string Dropdownname, string languagecode)
        {
            try
            {
              if(string.IsNullOrEmpty(languagecode.Trim()) || string.IsNullOrEmpty(Dropdownname.Trim()))
              {
                  return StatusCode(400, "Langauge code and dropdown  required..");
              }
              _logger.LogInformation("Drop down method get" + Dropdownname + languagecode);
              var translation = await translationmanager.GetTranslationsForDropDowns(Dropdownname,languagecode);
              return Ok(translation);
            }
            catch(Exception ex)
            {
                      _logger.LogError(ex.Message +" " +ex.StackTrace);
                      return StatusCode(500,"Internal Server Error.");
            }

        }

        [HttpPost]
        [Route("multipledropdowns")]
         public async  Task<IActionResult> GetTranslationsFormultipleDropDowns(string[] Dropdownname, string languagecode)
        {
            try
            {
              if(string.IsNullOrEmpty(languagecode.Trim()))
              {
                  return StatusCode(400, "Langauge code and dropdown  required..");
              }
               if(Dropdownname.Length ==  0 )
                {
                      return StatusCode(400, "Dropdown names are required.");
                }
              List<Translations> Dropdowns = new List<Translations>();
              foreach(var item in Dropdownname)
              {
                _logger.LogInformation("Drop down method get" + item + languagecode);
                 Dropdowns.AddRange(await translationmanager.GetTranslationsForDropDowns(item,languagecode));
              }
              
              
              return Ok(Dropdowns);
            }
            catch(Exception ex)
            {
                      _logger.LogError(ex.Message +" " +ex.StackTrace);
                      return StatusCode(500,"Internal Server Error.");
            }

        }


        [HttpGet]
        [Route("languagecodes")]
         public async  Task<IActionResult> GetAllLangaugecodes()
         {
                 _logger.LogInformation("All langauges method get");
            // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
            var translations = await translationmanager.GetAllLanguageCode();
            return Ok(translations);
         }
        

    }
}
