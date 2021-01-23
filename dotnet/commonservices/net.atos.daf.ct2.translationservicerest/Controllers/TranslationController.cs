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
    [Route("[controller]")]
    public class TranslationController : ControllerBase
    {
         private readonly ILogger<TranslationController> _logger;

       
        private readonly ITranslationManager translationmanager;
        
        public TranslationController(ILogger<TranslationController> logger,ITranslationManager _TranslationManager)
        {
            _logger = logger;
            translationmanager=_TranslationManager;
        }

        [HttpGet]
        [Route("GetMenuTranslations")]
         public async  Task<IActionResult> GetTranslations(Translations request)
        {
             List<Translations> responce = new List<Translations>();
            // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
            var translations = await translationmanager.GetTranslationsByMenu(request.MenuId,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString()),request.Code);
            

          return Ok(translations);
        }

        [HttpGet]
        [Route("GetCommonTranslations")]
        public async  Task<IActionResult> GetCommonTranslations(string LanguageCode)
        {
          try
          {
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

        [HttpPost]
        [Route("GetAlltranslatonByKey")]
        public async Task<IActionResult> GetLangagugeTranslationByKey(string key)
        {
          try
          {
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
        [Route("GetKeyTranslationByLanguageCode")]
         public async  Task<IActionResult> GetKeyTranslationByLanguageCode(string langaguecode,string key)
        {
            try
            {
              var translation = await translationmanager.GetKeyTranslationByLanguageCode(langaguecode,key);
              return Ok(translation);
            }
            catch(Exception ex)
            {
                      _logger.LogError(ex.Message +" " +ex.StackTrace);
                      return StatusCode(500,"Internal Server Error.");
            }

        }

        [HttpGet]
        [Route("GetTranslationsForDropDowns")]
         public async  Task<IActionResult> GetTranslationsForDropDowns(string Dropdownname, string langagugecode)
        {
            try
            {
              var translation = await translationmanager.GetTranslationsForDropDowns(Dropdownname,langagugecode);
              return Ok(translation);
            }
            catch(Exception ex)
            {
                      _logger.LogError(ex.Message +" " +ex.StackTrace);
                      return StatusCode(500,"Internal Server Error.");
            }

        }


        [HttpGet]
        [Route("GetAllLangaugecodes")]
         public async  Task<IActionResult> GetAllLangaugecodes()
         {
                 _logger.LogInformation("All langauges method get");
            // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
            var translations = await translationmanager.GetAllLanguageCode();
            return Ok(translations);
         }
        

    }
}
