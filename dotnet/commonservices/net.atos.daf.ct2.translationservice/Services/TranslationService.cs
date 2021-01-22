using System;

using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using net.atos.daf.ct2.translation.repository;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.Enum;

namespace net.atos.daf.ct2.translationservice
{
    public class TranslationManagementService : TranslationService.TranslationServiceBase
    {
         private readonly ILogger _logger;
        
        private readonly ITranslationManager translationmanager;
        
        public TranslationManagementService(ILogger<TranslationManagementService> logger,ITranslationManager _TranslationManager)
        {
            _logger = logger;
            translationmanager=_TranslationManager;
        }

        // Translation

         public async override Task<TranslationsResponce> GetTranslations(TranslationsRequest request, ServerCallContext context)
        {
            TranslationsResponce responce = new TranslationsResponce();
            // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
            var translations = await translationmanager.GetTranslationsByMenu(request.MenuId,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Menutype.ToString()));
            foreach(var item in translations)
            {
                    TranslationsRequest obj = new TranslationsRequest();
                    // string Type =  (translationenum.TranslationType)Enum.Parse(typeof(translationenum.TranslationType),item.Type.ToString()).ToString();
                    obj.Name = item.Name;
                    obj.Langaugecode = item.Code  == null ? "" : item.Code;
                    obj.Value = item.Value == null ? "" :item.Value;
                    obj.Contenttype = Contenttype.Dropdown;
                    // obj.Type=item.Type;
                    obj.ID = item.Id;
                    obj.Filter = item.Filter == null ? "" : item.Filter;
                    obj.MenuId = item.MenuId;
                    responce.Translations.Add(obj);
            }   

                 responce.Message = "Translations data retrieved";
                responce.Code = Responcecode.Success;
                return await Task.FromResult(responce);
        }
        //End Translation 

    }
}
