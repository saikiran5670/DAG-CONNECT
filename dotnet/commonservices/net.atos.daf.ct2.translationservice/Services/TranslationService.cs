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
//using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.translationservice.Entity;
using static net.atos.daf.ct2.translationservice.Entity.Mapper;

namespace net.atos.daf.ct2.translationservice
{
    public class TranslationManagementService : TranslationService.TranslationServiceBase
    {
        private readonly ILogger _logger;
        //private readonly IAuditTraillib auditlog;
        private readonly ITranslationManager translationmanager;
        private readonly Mapper _mapper;

        public TranslationManagementService(ILogger<TranslationManagementService> logger,ITranslationManager _TranslationManager)
        {
            _logger = logger;
            translationmanager=_TranslationManager;
            // auditlog = _auditlog;
            _mapper = new Mapper();
        }

        // Translation

        public async override Task<TranslationsResponce> GetTranslations(TranslationsRequest request, ServerCallContext context)
        {
            try
            {
                Translations trans = new Translations();

                var translations = await translationmanager.GetTranslationsByMenu(request.MenuId, (translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString()), request.Code);
               // await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Translation Component", "Translation Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Translation GetTranslations", 1, 2, trans.Name);

                // response 
                TranslationsResponce gettranslationList = new TranslationsResponce();
                foreach (var item in translations)
                {
                     var gettranslation = new Translation();
                    gettranslation.Id = item.Id;
                    gettranslation.Code = item.Code;
                    gettranslation.Type = item.Type;
                    gettranslation.Name = item.Name;
                    gettranslation.Value = item.Value;
                    gettranslation.Filter = item.Filter == null ? "" : item.Filter;
                    gettranslation.MenuId = item.MenuId;
                    gettranslationList.TranslationsList.Add(gettranslation);
                }
                return await Task.FromResult(gettranslationList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetTranslations : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new TranslationsResponce
                {
                    Code = Responcecode.Failed,
                    Message = "GetTranslations Faile due to - " + ex.Message
                });
            }
        }
        public async override Task<CodeResponce> GetCommonTranslations(CodeRequest request, ServerCallContext context)
        {
            try
            {
                List<Translations> translist = new List<Translations>();

                var translations = await translationmanager.GetTranslationsByMenu(0, translationenum.MenuType.Menu, request.Languagecode);

                CodeResponce commontranslationList = new CodeResponce();
                foreach (var item in translations)
                {
                     var commontranslation = new Translation();
                    commontranslation.Id = item.Id;
                    commontranslation.Code = item.Code;
                    commontranslation.Type = item.Type;
                    commontranslation.Name = item.Name;
                    commontranslation.Value = item.Value;
                   // commontranslation.Filter = item.Filter;
                    commontranslation.MenuId = item.MenuId;
                    commontranslationList.CodeTranslationsList.Add(commontranslation);
                }
                return await Task.FromResult(commontranslationList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetCommonTranslations : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new CodeResponce
                {
                    Code = Responcecode.Failed,
                    Message = "GetCommonTranslations Faile due to - " + ex.Message
                });

            }
            //End Translation 
        }

        public async override Task<KeyResponce> GetLangagugeTranslationByKey(KeyRequest request, ServerCallContext context)
        {
            try
            {
                var translation = await translationmanager.GetLangagugeTranslationByKey(request.Key);
                KeyResponce langtranslationList = new KeyResponce();
                foreach (var item in translation)
                {
                    var langtranslation = new Translation();
                    langtranslation.Id = item.Id;
                    langtranslation.Code = item.Code;
                    langtranslation.Type = item.Type;
                    langtranslation.Name = item.Name;
                    langtranslation.Value = item.Value;
                    langtranslation.Filter = item.Filter == null ? "" : item.Filter;
                    langtranslation.MenuId = item.MenuId;
                    langtranslationList.KeyTranslationsList.Add(langtranslation);
                }
                return await Task.FromResult(langtranslationList);

            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetLangagugeTranslationByKey : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new KeyResponce
                {
                    Code = Responcecode.Failed,
                    Message = "GetLangagugeTranslationByKey Faile due to - " + ex.Message
                });

            }
        }
        public async override Task<KeyCodeResponce> GetKeyTranslationByLanguageCode(KeyCodeRequest request, ServerCallContext context)
        {
            try
            {
               
                var translation = await translationmanager.GetKeyTranslationByLanguageCode(request.Languagecode, request.Key);


                KeyCodeResponce keytranslationList = new KeyCodeResponce();
                foreach (var item in translation)
                {
                     var keytranslation = new Translation();
                    keytranslation.Id = item.Id;
                    keytranslation.Code = item.Code;
                    keytranslation.Type = item.Type;
                    keytranslation.Name = item.Name;
                    keytranslation.Value = item.Value;
                    keytranslation.Filter = item.Filter == null ? "" : item.Filter;
                    keytranslation.MenuId = item.MenuId;
                    keytranslationList.KeyCodeTranslationsList.Add(keytranslation);
                }
                return await Task.FromResult(keytranslationList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetKeyTranslationByLanguageCode : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new KeyCodeResponce
                {
                    Code = Responcecode.Failed,
                    Message = "GetKeyTranslationByLanguageCode Faile due to - " + ex.Message
                });
            }

        }
        public async override Task<dropdownnameResponce> GetTranslationsForDropDowns(dropdownnameRequest request, ServerCallContext context)
        {
            try
            {
                var translation = await translationmanager.GetTranslationsForDropDowns(request.Dropdownname, request.Languagecode);

                dropdownnameResponce translationfordropdownList = new dropdownnameResponce();
                foreach (var item in translation)
                {
                     var translationfordropdown = new Translation();
                    translationfordropdown.Id = item.Id;
                    translationfordropdown.Code = item.Code;
                    translationfordropdown.Type = item.Type;
                    translationfordropdown.Name = item.Name;
                    translationfordropdown.Value = item.Value;
                    translationfordropdown.Filter = item.Filter == null ? "" : item.Filter;
                    translationfordropdown.MenuId = item.MenuId;
                    translationfordropdownList.DropdownnameTranslationsList.Add(translationfordropdown);
                }
                return await Task.FromResult(translationfordropdownList);

            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetTranslationsForDropDowns : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new dropdownnameResponce
                {
                    Code = Responcecode.Failed,
                    Message = "GetTranslationsForDropDowns Faile due to - " + ex.Message
                });
            }

        }

        public async override Task<dropdownarrayResponce> GetTranslationsFormultipleDropDowns(dropdownarrayRequest request, ServerCallContext context)
        {
            try
            {
                
                List<Translations> Dropdowns = new List<Translations>();
                dropdownarrayResponce responce = new dropdownarrayResponce();
                foreach (var item in request.Dropdownname)
                {
                    _logger.LogInformation("Drop down method get" + item + request.Languagecode);
                    Dropdowns.AddRange(await translationmanager.GetTranslationsForDropDowns(item.Dropdownname, request.Languagecode));

                    foreach (var Ditem in Dropdowns)
                    {
                        var translationfordropdown = new Translation();
                        translationfordropdown.Id = Ditem.Id;
                        translationfordropdown.Code = Ditem.Code;
                        translationfordropdown.Type = Ditem.Type;
                        translationfordropdown.Name = Ditem.Name;
                        translationfordropdown.Value = Ditem.Value;
                        translationfordropdown.Filter = Ditem.Filter == null ? "" : Ditem.Filter;
                        translationfordropdown.MenuId = Ditem.MenuId;

                        responce.DropdownnamearrayList.Add(translationfordropdown);
                    }
                }
                
                responce.Message = "The dropdown translation data retrieved";
                responce.Code = Responcecode.Success;
                return await Task.FromResult(responce);


            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetTranslationsFormultipleDropDowns : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new dropdownarrayResponce
                {
                    Code = Responcecode.Failed,
                    Message = "GetTranslationsFormultipleDropDowns Faile due to - " + ex.Message
                });
            }

        }


        public async override Task<PreferenceResponse> GetTranslationsPreferencDropDowns(PreferenceRequest request, ServerCallContext context)
        {
            try
            {


                 PreferenceResponse Dropdowns = new PreferenceResponse();
                Preferences obj = new Preferences();

                foreach (var item in obj.GetType().GetProperties())
                {
                    _logger.LogInformation("Drop down method get" + item.Name + request.Languagecode);
                    var Translations = await translationmanager.GetTranslationsForDropDowns(item.Name, request.Languagecode);

                    switch (item.Name)
                    {
                        case "language":
                            
                           // PreferenceResponceList list = new PreferenceResponceList();
                            foreach (var lang in Translations)
                            {
                                var tlang = new Translation();
                                tlang.Id = lang.Id;
                                tlang.Code = lang.Code;
                                tlang.Type = lang.Type;
                                tlang.Name = lang.Name;
                                tlang.Value = lang.Value;
                                tlang.Filter = lang.Filter == null ? "" : lang.Filter;
                                tlang.MenuId = lang.MenuId;
                                Dropdowns.Language.Add(tlang);
                            }

                            // code block
                            break;
                        case "timezone":

                            foreach (var itemt in Translations)
                            {
                                var tTime = new Translation();
                                tTime.Id = itemt.Id;
                                tTime.Code = itemt.Code;
                                tTime.Type = itemt.Type;
                                tTime.Name = itemt.Name;
                                tTime.Value = itemt.Value;
                                tTime.Filter = itemt.Filter == null ? "" : itemt.Filter;
                                tTime.MenuId = itemt.MenuId;
                                Dropdowns.Timezone.Add(tTime);
                            }
                            //Dropdowns.timezone = new List<Translations>();

                            //Dropdowns.timezone.AddRange(Translations);
                            // code block
                            break;
                        case "unit":

                            foreach (var itemt in Translations)
                            {
                                var tunit = new Translation();
                                tunit.Id = itemt.Id;
                                tunit.Code = itemt.Code;
                                tunit.Type = itemt.Type;
                                tunit.Name = itemt.Name;
                                tunit.Value = itemt.Value;
                                tunit.Filter = itemt.Filter == null ? "" : itemt.Filter;
                                tunit.MenuId = itemt.MenuId;
                                Dropdowns.Unit.Add(tunit);
                            }

                           
                            // code block
                            break;
                        case "currency":
                            foreach (var itemt in Translations)
                            {
                                var tcurrency = new Translation();
                                tcurrency.Id = itemt.Id;
                                tcurrency.Code = itemt.Code;
                                tcurrency.Type = itemt.Type;
                                tcurrency.Name = itemt.Name;
                                tcurrency.Value = itemt.Value;
                                tcurrency.Filter = itemt.Filter == null ? "" : itemt.Filter;
                                tcurrency.MenuId = itemt.MenuId;
                                Dropdowns.Currency.Add(tcurrency);
                            }

                          
                            // code block
                            break;
                        case "landingpagedisplay":

                            foreach (var itemt in Translations)
                            {
                                var tlandingpagedisplay = new Translation();
                                tlandingpagedisplay.Id = itemt.Id;
                                tlandingpagedisplay.Code = itemt.Code;
                                tlandingpagedisplay.Type = itemt.Type;
                                tlandingpagedisplay.Name = itemt.Name;
                                tlandingpagedisplay.Value = itemt.Value;
                                tlandingpagedisplay.Filter = itemt.Filter == null ? "" : itemt.Filter;
                                tlandingpagedisplay.MenuId = itemt.MenuId;
                                Dropdowns.Landingpagedisplay.Add(tlandingpagedisplay);
                            }
                            // code block
                            break;
                        case "dateformat":
                            foreach (var itemt in Translations)
                            {
                                var tdateformat = new Translation();
                                tdateformat.Id = itemt.Id;
                                tdateformat.Code = itemt.Code;
                                tdateformat.Type = itemt.Type;
                                tdateformat.Name = itemt.Name;
                                tdateformat.Value = itemt.Value;
                                tdateformat.Filter = itemt.Filter == null ? "" : itemt.Filter;
                                tdateformat.MenuId = itemt.MenuId;
                                Dropdowns.Dateformat.Add(tdateformat);
                            }
                            // code block
                            break;
                        case "timeformat":
                            foreach (var itemt in Translations)
                            {
                                var ttimeformat = new Translation();
                                ttimeformat.Id = itemt.Id;
                                ttimeformat.Code = itemt.Code;
                                ttimeformat.Type = itemt.Type;
                                ttimeformat.Name = itemt.Name;
                                ttimeformat.Value = itemt.Value;
                                ttimeformat.Filter = itemt.Filter == null ? "" : itemt.Filter;
                                ttimeformat.MenuId = itemt.MenuId;
                                Dropdowns.Timeformat.Add(ttimeformat);
                            }
                            // code block
                            break;
                        case "vehicledisplay":
                            foreach (var itemt in Translations)
                            {
                                var tvehicledisplay = new Translation();
                                tvehicledisplay.Id = itemt.Id;
                                tvehicledisplay.Code = itemt.Code;
                                tvehicledisplay.Type = itemt.Type;
                                tvehicledisplay.Name = itemt.Name;
                                tvehicledisplay.Value = itemt.Value;
                                tvehicledisplay.Filter = itemt.Filter == null ? "" : itemt.Filter;
                                tvehicledisplay.MenuId = itemt.MenuId;
                                Dropdowns.Vehicledisplay.Add(tvehicledisplay);
                            }
                            // code block
                            break;
                        default:
                            // code block
                            break;
                    }
                }
                return await Task.FromResult(Dropdowns);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetTranslationsPreferencDropDowns : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new PreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetTranslationsPreferencDropDowns Faile due to - " + ex.Message
                });
            }

        }
        public override async Task<TranslationListResponce> GetAllLanguagecodes( Request request , ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("All langauges method get");
                // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
                var translations = await translationmanager.GetAllLanguageCode();

                TranslationListResponce allLanguageCodeResponse = new TranslationListResponce();
                foreach (var item in translations)
                {
                    var tranlang = new Language();
                    tranlang.Id = item.Id;
                    tranlang.Name = item.Name;
                    tranlang.Code = item.Code;
                    tranlang.Key = item.Key;
                    tranlang.Description = item.Description == null ? "" : item.Description;
                    allLanguageCodeResponse.Languagelist.Add(tranlang);
                }

                return await Task.FromResult(allLanguageCodeResponse);
            }
            catch( Exception ex)
            {
                _logger.LogError("Translation Service:GetAllLangaugecodes : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new TranslationListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "GetAllLangaugecodes Faile due to - " + ex.Message
                });

            }
        }

        public override async Task<TranslationUploadResponse> InsertTranslationFileDetails(TranslationUploadRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("InsertTranslationFileDetails method ");
                Translationupload Objtranslationupload = new Translationupload();

                Objtranslationupload = _mapper.ToTranslationUploadEntity(request);

                var result = await translationmanager.InsertTranslationFileDetails(Objtranslationupload);
                _logger.LogInformation("InsertTranslationFileDetails service called.");
                TranslationRecordResponce objresponce = new TranslationRecordResponce();
                objresponce.Added = result.AddCount;
                objresponce.Updated = result.UpdateCount;
                objresponce.Failed = result.FailedCount;

                return await Task.FromResult(new TranslationUploadResponse
                {
                    Message = "FileDetails uploaded",
                    Code = Responcecode.Success,
                    Translationupload = objresponce

                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:InsertTranslationFileDetails : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new TranslationUploadResponse
                {
                    Code = Responcecode.Failed,
                    Message = "InsertTranslationFileDetails Faile due to - " + ex.Message
                });
            }
        }

        public override async Task<FileUploadDetailsResponse> GetFileUploadDetails(FileUploadDetailsRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("GetFileUploadDetails Method");
               // Translationupload Objtranslationupload = new Translationupload();
                var fileID = _mapper.ToTranslationEntity(request);
                IEnumerable<Translationupload> ObjRetrieveFileUploadList = await translationmanager.GetFileUploadDetails(fileID);
                FileUploadDetailsResponse response = new FileUploadDetailsResponse();
                foreach (var item in ObjRetrieveFileUploadList)
                {
                    response.Translationupload.Add(_mapper.ToTranslationUploadDetailEntity(item));
                }

                response.Message = "Translations data retrieved";
                response.Code = Responcecode.Success;
                _logger.LogInformation("Get method in vehicle service called.");
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetFileUploadDetails : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new FileUploadDetailsResponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetFileUploadDetails Faile due to - " + ex.Message
                });
            }
        }

    }
}
