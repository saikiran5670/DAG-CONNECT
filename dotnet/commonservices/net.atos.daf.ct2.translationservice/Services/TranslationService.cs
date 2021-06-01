using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.termsandconditions;
using net.atos.daf.ct2.termsandconditions.entity;
using net.atos.daf.ct2.translation;
//using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.translation.Enum;
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
        private readonly ITermsAndConditionsManager termsandconditionsmanager;
        private readonly IIconManager iconmanager;

        public TranslationManagementService(ILogger<TranslationManagementService> logger, ITranslationManager _TranslationManager, ITermsAndConditionsManager _termsandconditionsmanager, IIconManager _iconmanager)
        {
            _logger = logger;
            translationmanager = _TranslationManager;
            // auditlog = _auditlog;
            termsandconditionsmanager = _termsandconditionsmanager;
            _mapper = new Mapper();
            iconmanager = _iconmanager;
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
                    Message = "GetCommonTranslations Failed due to - " + ex.Message
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

                var translation = await translationmanager.GetKeyTranslationByLanguageCode(request.Languagecode.Trim(), request.Key.Trim());


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
        public override async Task<TranslationListResponce> GetAllLanguagecodes(Request request, ServerCallContext context)
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
            catch (Exception ex)
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
                ICollection collection = ObjRetrieveFileUploadList as ICollection;

                if (collection.Count == 0)
                {
                    response.Message = "bad Request please Enter Valid Data";
                    response.Code = Responcecode.NotFound;
                }
                else
                {
                    response.Message = "Translations data retrieved";
                    response.Code = Responcecode.Success;
                }
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

        public override async Task<WarningDataResponse> ImportDTCWarningData(WarningDataRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("ImportDTCWarningData Method");
                var dtcWarning = new List<DTCwarning>();
                var response = new WarningDataResponse();
                string foreignkeymessage = string.Empty;

                dtcWarning.AddRange(request.DtcData.Select(x => new DTCwarning()
                {
                    code = x.Code,
                    type = x.Type,
                    veh_type = x.VehType,
                    warning_class = x.WarningClass,
                    number = x.Number,
                    description = x.Description,
                    advice = x.Advice,
                    icon_id = x.IconId,
                    expires_at = x.ExpiresAt

                }).ToList());

                var DTCData = await translationmanager.ImportDTCWarningData(dtcWarning);

                foreach (var item in DTCData)
                {
                    if (item.message == "violates foreign key constraint for Icon_ID")
                        foreignkeymessage = item.message;
                }

                if (foreignkeymessage == "violates foreign key constraint for Icon_ID")
                {
                    response.Code = Responcecode.Failed;
                    response.Message = "violates foreign key constraint for Icon_ID , Please enter valid data for Warning_Class and Warning_Number";
                    return await Task.FromResult(response);
                }
                else
                {
                    response.DtcDataResponse.AddRange(DTCData
                                  .Select(x => new dtcwarning()
                                  {
                                      Id = x.id,
                                      Code = x.code,
                                      Type = x.type,
                                      VehType = x.veh_type,
                                      WarningClass = x.warning_class,
                                      Number = x.number,
                                      Description = x.description,
                                      Advice = x.advice,
                                      IconId = x.icon_id,
                                      ExpiresAt = x.expires_at,
                                      CreatedBy = x.created_by
                                  }).ToList());

                    response.Code = Responcecode.Success;
                    response.Message = "DTC warning Data imported successfully.";
                    return await Task.FromResult(response);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:ImportDTCWarningData : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new WarningDataResponse
                {
                    Code = Responcecode.Failed,
                    Message = "ImportDTCWarningData Faile due to - " + ex.Message
                });
            }

        }
        public async override Task<WarningGetResponse> GetDTCWarningData(WarningGetRequest request, ServerCallContext context)
        {
            try
            {

                var dtcData = await translationmanager.GetDTCWarningData(request.LanguageCode);

                WarningGetResponse getResponseList = new WarningGetResponse();
                foreach (var item in dtcData)
                {
                    var WarnData = new dtcwarning();
                    WarnData.Id = item.id;
                    WarnData.Code = item.code;
                    WarnData.Type = item.Warning_type;
                    WarnData.VehType = item.veh_type;
                    WarnData.WarningClass = item.warning_class;
                    WarnData.Number = item.number;
                    WarnData.Description = item.description;
                    WarnData.Advice = item.advice;
                    WarnData.IconId = item.icon_id;
                    WarnData.ExpiresAt = item.expires_at;
                    WarnData.CreatedBy = item.created_by;
                    getResponseList.DtcGetDataResponse.Add(WarnData);
                }
                return await Task.FromResult(getResponseList);

            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetDTCWarningData : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new WarningGetResponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetTranslationsForDropDowns Faile due to - " + ex.Message
                });
            }
        }

        public override async Task<WarningDataResponse> UpdateDTCWarningData(WarningDataRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("UpdateDTCWarningData Method");
                var dtcWarning = new List<DTCwarning>();
                var response = new WarningDataResponse();


                dtcWarning.AddRange(request.DtcData.Select(x => new DTCwarning()
                {
                    code = x.Code,
                    type = x.Type,
                    veh_type = x.VehType,
                    warning_class = x.WarningClass,
                    number = x.Number,
                    description = x.Description,
                    advice = x.Advice,
                    icon_id = x.IconId,
                    expires_at = x.ExpiresAt

                }).ToList());

                var DTCData = await translationmanager.UpdateDTCWarningData(dtcWarning);


                response.DtcDataResponse.AddRange(DTCData
                                   .Select(x => new dtcwarning()
                                   {
                                       Id = x.id,
                                       Code = x.code,
                                       Type = x.type,
                                       VehType = x.veh_type,
                                       WarningClass = x.warning_class,
                                       Number = x.number,
                                       Description = x.description,
                                       Advice = x.advice,
                                       IconId = x.icon_id,
                                       ExpiresAt = x.expires_at,
                                       CreatedBy = x.created_by
                                   }).ToList());

                response.Code = Responcecode.Success;
                response.Message = "DTC warning Data updated successfully.";
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:UpdateDTCWarningData : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new WarningDataResponse
                {
                    Code = Responcecode.Failed,
                    Message = "UpdateDTCWarningData Faile due to - " + ex.Message
                });
            }

        }

        #region Terms And Conditions

        public override async Task<AcceptedTermConditionResponse> AddUserAcceptedTermCondition(AcceptedTermConditionRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("AddUserAcceptedTermCondition method ");
                AccountTermsCondition ObjAccountTermsCondition = new AccountTermsCondition();

                ObjAccountTermsCondition = _mapper.ToAcceptedTermConditionEntity(request);

                var result = await termsandconditionsmanager.AddUserAcceptedTermCondition(ObjAccountTermsCondition);
                _logger.LogInformation("AddUserAcceptedTermCondition service called.");
                return await Task.FromResult(new AcceptedTermConditionResponse
                {
                    Message = "Terms and condition accepted by user",
                    Code = Responcecode.Success,
                    AcceptedTermCondition = _mapper.ToAcceptedTermConditionRequestEntity(result)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:AddUserAcceptedTermCondition : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new AcceptedTermConditionResponse
                {
                    Code = Responcecode.Failed,
                    Message = "AddUserAcceptedTermCondition Faile due to - " + ex.Message
                });
            }
        }

        public override async Task<VersionNoResponse> GetAllVersionNo(VersionID request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("GetAllVersionNo method ");
                net.atos.daf.ct2.termsandconditions.entity.VersionByID objVersionByID = new VersionByID();
                objVersionByID.orgId = request.OrgId;
                objVersionByID.accountId = request.AccountId;
                objVersionByID.levelCode = request.LevelCode;
                var result = await termsandconditionsmanager.GetAllVersionNo(objVersionByID);
                _logger.LogInformation("GetAllVersionNo service called.");
                VersionNoResponse response = new VersionNoResponse();
                foreach (var item in result.Distinct())
                {
                    response.VersionNos.Add(item);
                }
                response.Message = "All version retrived.";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetAllVersionNo : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new VersionNoResponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetAllVersionNo Failed due to - " + ex.Message
                });
            }
        }

        public override async Task<TermCondDetailsReponse> GetTermConditionForVersionNo(VersionNoRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("GetTermConditionForVersionNo method ");
                var result = await termsandconditionsmanager.GetTermConditionForVersionNo(request.VersionNo, request.Languagecode);
                _logger.LogInformation("GetTermConditionForVersionNo service called.");

                TermCondDetailsReponse Response = new TermCondDetailsReponse();
                foreach (var item in result)
                {
                    var tramcond = new TermConditionReponse();
                    tramcond.Id = item.Id;
                    tramcond.Code = item.Code;
                    tramcond.Versionno = item.version_no;
                    if (item.Description != null)
                    {
                        tramcond.Description = ByteString.CopyFrom(item.Description);
                    }
                    tramcond.StartDate = item.StartDate.ToString();
                    Response.TermCondition.Add(tramcond);
                }
                Response.Code = Responcecode.Success;
                Response.Message = "Terms and condition details retrived for version no.";
                return await Task.FromResult(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetTermConditionForVersionNo : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new TermCondDetailsReponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetTermConditionForVersionNo Failed due to - " + ex.Message
                });
            }
        }

        public override async Task<TermCondDetailsReponse> GetAcceptedTermConditionByUser(UserAcceptedTermConditionRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("GetAcceptedTermConditionByUser method ");
                var result = await termsandconditionsmanager.GetAcceptedTermConditionByUser(request.AccountId, request.OrganizationId);
                _logger.LogInformation("GetAcceptedTermConditionByUser service called.");

                TermCondDetailsReponse Response = new TermCondDetailsReponse();
                foreach (var item in result)
                {
                    var tramcond = new TermConditionReponse();
                    tramcond.Id = item.Id;
                    tramcond.Code = item.Code;
                    tramcond.Versionno = item.version_no;
                    if (item.Description != null)
                    {
                        tramcond.Description = ByteString.CopyFrom(item.Description);
                    }
                    tramcond.State = item.State.ToString();
                    tramcond.StartDate = item.StartDate.ToString();
                    tramcond.AcceptedDate = item.Accepted_Date.ToString();
                    tramcond.CreatedAt = item.Created_At.ToString();
                    tramcond.FirstName = item.FirstName == null ? "" : item.FirstName;
                    tramcond.Lastname = item.Lastname == null ? "" : item.Lastname;
                    Response.TermCondition.Add(tramcond);
                }
                Response.Code = Responcecode.Success;
                Response.Message = "Terms and condition details retrived for version no.";
                return await Task.FromResult(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetAcceptedTermConditionByUser : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new TermCondDetailsReponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetAcceptedTermConditionByUser Failed due to - " + ex.Message
                });
            }
        }
        #endregion

        #region  DTC Translation Icon 
        public override async Task<IconUpdateResponse> UpdateDTCTranslationIcon(IconUpdateRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("UpdateDTCTranslationIcon method ");

                var icons = new List<Icon>();

                icons.AddRange(request.IconData.Select(x => new Icon()
                {
                    name = x.Name,
                    icon = x.Icon.ToArray(),
                    modified_at = x.ModifiedAt,
                    modified_by = x.ModifiedBy

                }).ToList());

                bool result = await iconmanager.UpdateIcons(icons);
                _logger.LogInformation("UpdateDTCTranslationIcon service called.");

                IconUpdateResponse Response = new IconUpdateResponse();
                if (result)
                {
                    Response.Code = Responcecode.Success;
                    Response.Message = "Update Icon in DTC translation.";
                }
                else
                {
                    Response.Code = Responcecode.Failed;
                    Response.Message = "File Name not exist .";
                }
                return await Task.FromResult(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:UpdateDTCTranslationIcon : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new IconUpdateResponse
                {
                    Code = Responcecode.Failed,
                    Message = "UpdateDTCTranslationIcon Failed due to - " + ex.Message
                });
            }
        }

        public override async Task<IconGetResponse> GetDTCTranslationIcon(IconGetRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("GetDTCTranslationIcon method ");

                var icons = new List<Icon>();

                IconGetResponse Response = new IconGetResponse();
                icons = await iconmanager.GetIcons(request.Id);
                foreach (var itemicon in icons)
                {
                    var icon = new dtcIcon();
                    icon.Id = itemicon.id;
                    icon.Name = itemicon.name;
                    if (itemicon.icon != null)
                    {
                        icon.Icon = ByteString.CopyFrom(itemicon.icon);
                    }
                    icon.ModifiedAt = itemicon.modified_at == null ? 0 : (long)itemicon.modified_at;
                    icon.ModifiedBy = itemicon.modified_by == null ? 0 : (int)itemicon.modified_by;
                    icon.Type = itemicon.type.ToString();
                    icon.WarningClass = itemicon.warning_class;
                    icon.WarningNumber = itemicon.warning_number;
                    icon.ColorName = itemicon.color_name.ToString();
                    icon.State = itemicon.state.ToString();
                    Response.IconData.Add(icon);
                }
                _logger.LogInformation("GetDTCTranslationIcon service called.");

                if (icons.Count() > 0)
                {
                    Response.Code = Responcecode.Success;
                    Response.Message = "Get Icon in DTC translation.";
                }
                else
                {
                    Response.Code = Responcecode.Failed;
                    Response.Message = "Resource Not Found ";
                }
                return await Task.FromResult(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetDTCTranslationIcon : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new IconGetResponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetDTCTranslationIcon Failed due to - " + ex.Message
                });
            }
        }
        public override async Task<UploadTermandConditionResponseList> UploadTermsAndCondition(UploadTermandConditionRequestList request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("UploadTermsAndCondition method ");
                UploadTermandConditionResponseList objUploadTermandConditionResponseList = new UploadTermandConditionResponseList();
                net.atos.daf.ct2.termsandconditions.entity.TermsandConFileDataList objTermsandConFileDataList = new ct2.termsandconditions.entity.TermsandConFileDataList();
                objTermsandConFileDataList._data = new List<TermsandConFileData>();
                objTermsandConFileDataList.start_date = request.StartDate;
                objTermsandConFileDataList.end_date = request.EndDate;
                objTermsandConFileDataList.created_by = request.CreatedBy;
                if (request == null)
                {
                    return objUploadTermandConditionResponseList;
                }
                foreach (var item in request.Data)
                {
                    net.atos.daf.ct2.termsandconditions.entity.TermsandConFileData objTermsandConFileData = new ct2.termsandconditions.entity.TermsandConFileData();
                    objTermsandConFileData.fileName = item.FileName;
                    objTermsandConFileData.version_no = item.Versionno;
                    objTermsandConFileData.code = item.Code;
                    objTermsandConFileData.description = item.Description.ToByteArray();

                    objTermsandConFileDataList._data.Add(objTermsandConFileData);
                }

                var data = await termsandconditionsmanager.UploadTermsAndCondition(objTermsandConFileDataList);
                _logger.LogInformation("UploadTermsAndCondition service called ");

                if (data == null)
                {
                    return objUploadTermandConditionResponseList;
                }
                _logger.LogInformation("UploadTermsAndCondition service called.");
                //objUploadTermandConditionResponseList.Uploadedfilesaction = new Google.Protobuf.Collections.RepeatedField<UploadTermandConditionResponse>();
                foreach (var items in data.termsAndConditionDetails)
                {
                    var objUploadTermandConditionResponse = new UploadTermandConditionResponse();
                    objUploadTermandConditionResponse.FileName = items.fileName;
                    objUploadTermandConditionResponse.Id = items.id;
                    objUploadTermandConditionResponse.Action = items.action;
                    objUploadTermandConditionResponseList.Uploadedfilesaction.Add(objUploadTermandConditionResponse);
                }
                objUploadTermandConditionResponseList.Code = Responcecode.Success;
                objUploadTermandConditionResponseList.Message = "Uploded Terms and condition details retrived for Individual file.";
                return await Task.FromResult(objUploadTermandConditionResponseList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:UploadTermsAndCondition : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new UploadTermandConditionResponseList
                {
                    Code = Responcecode.Failed,
                    Message = $"UploadTermsAndCondition Failed due to - {ex.Message}"
                });
            }
        }


        public override async Task<TermCondDetailsReponse> GetLatestTermCondition(UserAcceptedTermConditionRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("GetAcceptedTermConditionByUser method ");
                var result = await termsandconditionsmanager.GetLatestTermCondition(request.AccountId, request.OrganizationId);
                _logger.LogInformation("GetAcceptedTermConditionByUser service called.");
                TermCondDetailsReponse Response = new TermCondDetailsReponse();
                if (result.Id > 0)
                {
                    TermConditionReponse tramcond = new TermConditionReponse();
                    tramcond.Id = result.Id;
                    tramcond.Code = result.Code == null ? "" : result.Code;
                    tramcond.Versionno = result.version_no == null ? "" : result.version_no;
                    if (result.Description != null)
                    {
                        tramcond.Description = ByteString.CopyFrom(result.Description);
                    }
                    tramcond.StartDate = result.StartDate.ToString();
                    tramcond.State = result.State.ToString();
                    Response.TermCondition.Add(tramcond);
                }
                Response.Code = Responcecode.Success;
                Response.Message = "Terms and condition details retrived for version no.";
                return await Task.FromResult(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetLatestTermCondition : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new TermCondDetailsReponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetLatestTermCondition Failed due to - " + ex.Message
                });
            }
        }

        public override async Task<UserAcceptedTermConditionResponse> CheckUserAcceptedTermCondition(UserAcceptedTermConditionRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("CheckUserAcceptedTermCondition method ");
                var result = await termsandconditionsmanager.CheckUserAcceptedTermCondition(request.AccountId, request.OrganizationId);
                _logger.LogInformation("CheckUserAcceptedTermCondition service called.");
                UserAcceptedTermConditionResponse Response = new UserAcceptedTermConditionResponse();
                Response.IsUserAcceptedTC = result;
                Response.Code = Responcecode.Success;
                Response.Message = "Terms and condition details retrived for version no.";
                return await Task.FromResult(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Translation Service:GetLatestTermCondition : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new UserAcceptedTermConditionResponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetLatestTermCondition Failed due to - " + ex.Message
                });
            }
        }



        #endregion

    }
}
