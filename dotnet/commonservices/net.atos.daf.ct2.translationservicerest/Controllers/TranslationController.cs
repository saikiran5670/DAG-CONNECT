using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.Enum;
using net.atos.daf.ct2.translation.entity;

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

        [HttpPost]
        [Route("GetTranslations")]
         public async  Task<IActionResult> GetTranslations(Translations request)
        {
             List<Translations> responce = new List<Translations>();
            // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
            var translations = await translationmanager.GetTranslationsByMenu(request.MenuId,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString()));
            // foreach(var item in translations)
            // {
            //         Translations obj = new Translations();
            //         // string Type =  (translationenum.TranslationType)Enum.Parse(typeof(translationenum.TranslationType),item.Type.ToString()).ToString();
            //         obj.Name = item.Name;
            //         obj.Code = item.Code  == null ? "" : item.Code;
            //         obj.Value = item.Value == null ? "" :item.Value;
            //         obj.Type = Contenttype.Dropdown;
            //         // obj.Type=item.Type;
            //         obj.ID = item.Id;
            //         obj.Filter = item.Filter == null ? "" : item.Filter;
            //         obj.MenuId = item.MenuId;
            //         responce.Translations.Add(obj);
            // }   

          return Ok(translations);
        }
        [HttpGet]
        [Route("GetAllLangaugecodes")]
         public async  Task<IActionResult> GetAllLangaugecodes()
         {
             try
             {
                 _logger.LogInformation("All langauges method get");
            // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
            var translations = await translationmanager.GetAllLanguageCode();
            return Ok(translations);

             }
             catch(Exception ex)
             {
                    _logger.LogError("All langauges method get failed " + ex.ToString());
                       return StatusCode(500, "Internal server error."); 
             }
         
         }
        

    }
}
