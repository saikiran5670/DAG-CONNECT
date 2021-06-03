using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.email.entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.utilities;
using static net.atos.daf.ct2.translation.Enum.Translationenum;

namespace net.atos.daf.ct2.translation.repository
{
    public class TranslationRepository : ITranslationRepository
    {
        //private readonly IConfiguration _config;
        private readonly TranslationCoreMapper _translationCoreMapper;

        //     private readonly IDataAccess dataAccess;

        //    public AuditLogRepository(IDataAccess _dataAccess) 
        //     {
        //         //_config = new ConfigurationBuilder()
        //        //.AddJsonFile("appsettings.Test.json")
        //        //.Build();
        //        // Get connection string
        //        //var connectionString = _config.GetConnectionString("DevAzure");
        //        //dataAccess = new PgSQLDataAccess(connectionString);
        //        dataAccess= _dataAccess;
        //     }
        private readonly IDataAccess _dataAccess;
        public TranslationRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _translationCoreMapper = new TranslationCoreMapper();
        }

        public async Task<IEnumerable<Langauge>> GetAllLanguageCode()
        {
            string LangagugeQuery = @"SELECT id, name, code, key, description
	                                    FROM translation.language";


            var parameter = new DynamicParameters();
            IEnumerable<Langauge> LangagugeCodes = await _dataAccess.QueryAsync<Langauge>(LangagugeQuery, parameter);
            return LangagugeCodes;
        }

        public async Task<IEnumerable<Translations>> GetKeyTranslationByLanguageCode(string langaguecode, string key)
        {
            string LangagugeQuery = @"SELECT t.id,
                                            t.name,
                                            t.value,
                                            t.type,
                                            t.code,
                                            tg.ref_id
                                            FROM translation.translation t 
                                            LEFT join translation.translationgrouping tg
                                            on tg.name= t.name
                                            where  (t.code= @langaguecode) and t.name = @key
                                            union
                                            SELECT t.id,
                                            t.name,
                                            t.value,
                                            t.type,
                                            t.code,
                                            tg.ref_id
                                            FROM translation.translation t 
                                            LEFT join translation.translationgrouping tg
                                            on tg.name= t.name
                                            where  (t.code= 'EN-GB') and t.name = @key
                                            and t.name not in (SELECT name
                                            FROM translation.translation where code= @langaguecode and  t.name = @key )";

            // string LangagugeQuery= @"select  t.id,t.name,t.value,t.type from translation.translation t
            //                         where t.code = @langaguecode and t.name = @key";


            var parameter = new DynamicParameters();
            parameter.Add("@langaguecode", langaguecode);
            parameter.Add("@key", key);
            IEnumerable<Translations> translations = await _dataAccess.QueryAsync<Translations>(LangagugeQuery, parameter);
            return translations;
        }

        public async Task<IEnumerable<Translations>> GetLangagugeTranslationByKey(string key)
        {
            string LangagugeQuery = @"select  t.id,t.name,t.code,t.value,t.type from translation.translation t
                                        where 1=1";


            var parameter = new DynamicParameters();
            if (key.Length > 0)
            {
                parameter.Add("@key", key);
                LangagugeQuery = LangagugeQuery + " and  t.name = @key";

            }
            IEnumerable<Translations> Translations = await _dataAccess.QueryAsync<Translations>(LangagugeQuery, parameter);
            return Translations;
        }

        public async Task<IEnumerable<Translations>> GetTranslationsByMenu(int MenuId, string type, string langaguecode)
        {
            string LangagugeQuery = @"SELECT t.id,
                                            t.name,
                                            t.value,
                                            t.type,
                                            t.code,
                                            tg.ref_id
                                            FROM translation.translation t 
                                            LEFT join translation.translationgrouping tg
                                            on tg.name= t.name
                                            where  tg.ref_id = @menuid
                                            and (t.code= @langaguecode)
                                            union
                                            SELECT t.id,
                                            t.name,
                                            t.value,
                                            t.type,
                                            t.code,
                                            tg.ref_id
                                            FROM translation.translation t 
                                            LEFT join translation.translationgrouping tg
                                            on tg.name= t.name
                                            where  tg.ref_id = @menuid
                                            and (t.code= 'EN-GB')
                                            and t.name not in (SELECT name
                                            FROM translation.translation where code= @langaguecode and tg.ref_id =@menuid )";

            if (langaguecode == "EN-GB")
            {
                LangagugeQuery = @"select tg.id,t.name,t.value,t.type,t.code,tg.ref_id from translation.translation t
                                        inner join translation.translationgrouping tg
                                        on t.name = tg.name where t.code=@langaguecode and tg.ref_id= @menuid";
            }
            var parameter = new DynamicParameters();
            parameter.Add("@langaguecode", langaguecode);

            parameter.Add("@menuid", MenuId);
            // LangagugeQuery = LangagugeQuery + " and tg.ref_id  = @menuid";


            if (type != null)
            {
                parameter.Add("@type", type.ToString());
                // LangagugeQuery = LangagugeQuery + " and tg.type  = @type";

            }
            List<Translations> list = new List<Translations>();
            var Translations = await _dataAccess.QueryAsync<dynamic>(LangagugeQuery, parameter);
            foreach (var item in Translations)
            {
                list.Add(Map(item));
            }
            var names = list.Where(T => T.Type == ((char)TranslationType.Dropdown).ToString()).SelectMany(p => p.Name.Split('_')).Distinct().Where(K => K[0] == 'd');
            foreach (var name in names)
            {
                IEnumerable<Translations> dropdowntranslation = await GetTranslationsForDropDowns(name.Substring(1), "");
                foreach (var item in dropdowntranslation)
                {
                    list.Where(P => P.Name == item.Name).ToList().ForEach(i =>
                                                                                  {
                                                                                      i.Id = item.Id;
                                                                                      i.Filter = name.Substring(1);
                                                                                  });
                }

            }

            return list;
        }

        public async Task<IEnumerable<Translations>> GetTranslationsForDropDowns(string Dropdownname, string langagugecode)
        {
            try
            {
                string LangagugeQuery = "";
                if (string.IsNullOrEmpty(langagugecode) || langagugecode == "EN-GB")
                {
                    if (Dropdownname == "language")
                        LangagugeQuery = @"select tc.id,t.name,t.code,t.value,t.type from translation.language tc inner join translation.translation t on tc.key = t.name ";
                    else
                    {
                        LangagugeQuery = @"select tc.id,t.name,t.code,t.value,t.type from master." + Dropdownname + " tc inner join translation.translation t on tc.key = t.name ";
                    }

                    var parameter = new DynamicParameters();
                    parameter.Add("@code", langagugecode);
                    LangagugeQuery = LangagugeQuery + " Where t.code=  'EN-GB'";
                    IEnumerable<Translations> Translations = await _dataAccess.QueryAsync<Translations>(LangagugeQuery, parameter);

                    return Translations;
                }
                else
                {
                    if (Dropdownname == "language")
                        LangagugeQuery = @"SELECT  tc.id,
                                        t.name,
                                        t.code,
                                        t.value,
                                        t.type
                                        from translation.language tc 
                                        LEFT join translation.translation t
                                        on tc.key = t.name 
                                        where  
                                        (t.code= @code)
                                        union
                                        SELECT  tc.id,t.name,t.code,t.value,t.type
                                        from translation.language tc 
                                        LEFT join translation.translation t
                                        on tc.key = t.name 
                                        where   (t.code= 'EN-GB')
                                        and t.name not in (SELECT name
                                        FROM translation.translation where code= @code ) ";
                    else
                    {
                        LangagugeQuery = @"SELECT  tc.id,
                                        t.name,
                                        t.code,
                                        t.value,
                                        t.type from master." + Dropdownname + @" tc LEFT join translation.translation t
                                        on tc.key = t.name 
                                        where  
                                        (t.code= @code)
                                        union
                                        SELECT  tc.id,t.name,t.code,t.value,t.type
                                        from master." + Dropdownname + @" tc 
                                        LEFT join translation.translation t
                                        on tc.key = t.name 
                                        where   (t.code= 'EN-GB')
                                        and t.name not in (SELECT name
                                        FROM translation.translation where code= @code ) ";
                    }

                    var parameter = new DynamicParameters();
                    parameter.Add("@code", langagugecode);

                    IEnumerable<Translations> Translations = await _dataAccess.QueryAsync<Translations>(LangagugeQuery, parameter);
                    return Translations;

                }


            }
            catch (Exception)
            {
                return Enumerable.Empty<Translations>();
            }

        }

        private Translations Map(dynamic record)
        {
            Translations Entity = new Translations();
            Entity.Id = record.id;
            Entity.Code = record.code;
            Entity.Type = record.type;
            Entity.Name = record.name;
            Entity.Value = record.value;
            Entity.Filter = record.filter;
            // Entity.MenuId = record.MenuId;
            return Entity;
        }

        public async Task<List<Translations>> ImportExcelDataIntoTranslations(List<Translations> translationslist)
        {
            try
            {
                List<Translations> translationlist = new List<Translations>();
                int failuer_count = 0;
                var parameter = new DynamicParameters();
                string query = string.Empty;
                var translations = new List<Translations>();
                if (translationslist != null)
                {
                    foreach (Translations item in translationslist)
                    {

                        if (item.Code != null && item.Type != null && item.Name != null && item.Value != null)
                        {
                            parameter = new DynamicParameters();
                            parameter.Add("@Code", item.Code);
                            parameter.Add("@Type", item.Type);
                            parameter.Add("@Name", item.Name);
                            parameter.Add("@Value", item.Value);
                            parameter.Add("@Created_at", item.Created_at);
                            parameter.Add("@modified_at", item.Modified_at);
                            query = @"INSERT INTO translation.translation(code, type, name, value, created_at, modified_at) " +
                                    "values(@Code,@Type,@Name,@Value,@Created_at,@modified_at) RETURNING id";
                            var translationId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                            item.Id = translationId;
                            if (translationId > 0)
                            {
                                translations.Add(item);
                            }
                        }
                        else
                        {
                            failuer_count++;
                        }
                    }

                }
                return translations;
            }
            catch (Exception)
            {
                throw;
            }


        }
        public async Task<List<Translations>> GetAllTranslations()
        {
            try
            {
                List<Translations> translations = new List<Translations>();
                var QueryStatement = @" SELECT *
                                    FROM translation.translation  ";
                var parameter = new DynamicParameters();

                dynamic result = await _dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

                foreach (dynamic record in result)
                {

                    translations.Add(Map(record));
                }
                
                return translations;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Translationupload> InsertTranslationFileDetails(Translationupload translationupload)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var InsertFileDetailsQueryStatement = @"INSERT INTO translation.translationupload(
                                                             file_name, description, file_size, failure_count, created_at, file, added_count, updated_count, created_by)
                                                           VALUES (@file_name, @description, @file_size, @failure_count, @created_at, @file, @added_count, @updated_count,@created_by)
                                                             RETURNING id";

                    var parameter = new DynamicParameters();
                    parameter.Add("@file_name", translationupload.FileName);
                    parameter.Add("@description", translationupload.Description);
                    parameter.Add("@file_size", translationupload.FileSize);
                    parameter.Add("@failure_count", translationupload.FailureCount);
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameter.Add("@file", translationupload.File);
                    parameter.Add("@added_count", translationupload.AddedCount);
                    parameter.Add("@updated_count", translationupload.UpdatedCount);
                    parameter.Add("@created_by", translationupload.CreatedBy);

                    int InsertedFileUploadID = await _dataAccess.ExecuteScalarAsync<int>(InsertFileDetailsQueryStatement, parameter);

                    // Convert Byte array to List Type
                    //List<Translations> myList;
                    //BinaryFormatter bf = new BinaryFormatter();
                    //using (Stream ms = new MemoryStream(translationupload.file))
                    //{
                    //    myList = (List<Translations>)bf.Deserialize(ms);
                    //}

                    //if (translationupload.translations != null)
                    //{
                    //    // foreach (var item in myList)
                    //    // {
                    //    var parameterfeature = ImportExcelDataIntoTranslations(translationupload.translations);
                    //    // }
                    //}
                    if (InsertedFileUploadID > 0)
                    {
                        translationupload.Id = InsertedFileUploadID;
                    }

                    transactionScope.Complete();

                    return translationupload;
                }


            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<TranslationStatus> InsertTranslationFileData(Translations translationdata, List<Translations> TranslationsList)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                //TranslationsList = GetAllTranslations(translationdata.Name, translationdata.Code);

                var translationcodeList = TranslationsList.Where(I => I.Name == translationdata.Name).ToList();


                if (translationcodeList != null && translationcodeList.Count > 0)
                {
                    var type = translationcodeList.FirstOrDefault().Type;
                    var translationobjdata = translationcodeList.Where(I => I.Name == translationdata.Name && I.Code == translationdata.Code).FirstOrDefault();
                    if (translationobjdata != null)
                    {
                        if (translationobjdata.Value == translationdata.Value)
                        {
                            // nO need to update the records
                            return TranslationStatus.Ignored;
                        }
                        else
                        {
                            parameter = new DynamicParameters();
                            parameter.Add("@id", translationobjdata.Id);
                            parameter.Add("@Code", translationobjdata.Code);
                            parameter.Add("@Type", type ?? "L");
                            parameter.Add("@Name", translationobjdata.Name);
                            parameter.Add("@Value", translationdata.Value);
                            //parameter.Add("@Created_at", translationdata.created_at);
                            parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                            query = @"update translation.translation set 
                                code= @Code,type= @Type,name= @Name,value = @Value,modified_at = @modified_at Where id=@id RETURNING id";
                            var translationId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                            return TranslationStatus.Updated;
                        }

                    }
                    else
                    {

                        parameter = new DynamicParameters();
                        parameter.Add("@Code", translationdata.Code);
                        parameter.Add("@Type", type ?? "L");
                        parameter.Add("@Name", translationdata.Name);
                        parameter.Add("@Value", translationdata.Value);
                        parameter.Add("@Created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                        //parameter.Add("@modified_at", translationdata.modified_at);
                        query = @"INSERT INTO translation.translation(code, type, name, value, created_at) " +
                                "values(@Code,@Type,@Name,@Value,@Created_at) RETURNING id";
                        var translationId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        return TranslationStatus.Added;
                    }
                }
                else
                {
                    return TranslationStatus.Failed;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private static Translations[] ConvertList(byte [] file)
        //{
        //    List<Translations> tmpList = new List<Translations>();
        //    foreach (Byte[] byteArray in file)
        //        foreach (Byte singleByte in byteArray)
        //            tmpList.Add(singleByte);
        //    return tmpList.ToArray();
        //}

        public async Task<IEnumerable<Translationupload>> GetFileUploadDetails(int FileID)
        {
            try
            {
                var parameter = new DynamicParameters();
                var InsertFileDetailsQueryStatement = @"SELECT id, file_name, description, file_size, failure_count, created_at, added_count, updated_count, created_by
                                                             FROM translation.translationupload
                                                                  where 1=1";
                if (FileID > 0)
                {
                    parameter.Add("@FileID", FileID);
                    InsertFileDetailsQueryStatement = @"SELECT id, file_name, description, file_size, failure_count, created_at, file, added_count, updated_count, created_by
                                                             FROM translation.translationupload
                                                                  where 1=1";
                    InsertFileDetailsQueryStatement = InsertFileDetailsQueryStatement + " and id=@FileID";

                }
                //// organization id filter
                //if (FileName != null || FileName !="")
                //{
                //    parameter.Add("@FileName", FileName);
                //    InsertFileDetailsQueryStatement = InsertFileDetailsQueryStatement + " and file_name=@FileName";

                //}

                List<Translationupload> fileuploadlist = new List<Translationupload>();
                dynamic result = await _dataAccess.QueryAsync<dynamic>(InsertFileDetailsQueryStatement, parameter);
                foreach (dynamic record in result)
                {
                    fileuploadlist.Add(MapfileDetails(record));
                }
                return fileuploadlist.AsEnumerable();


            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<EmailTemplate> GetEmailTemplateTranslations(EmailEventType eventType, EmailContentType contentType, string languageCode)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@contentType", (char)contentType);
                parameter.Add("@eventName", eventType.ToString());

                string emailTemplateQuery =
                    @"select id as TemplateId, description as Description, type as ContentType, event_name as EventType
                    from master.emailtemplate
                    where type=@contentType and event_name=@eventName";

                EmailTemplate template = await _dataAccess.QueryFirstAsync<EmailTemplate>(emailTemplateQuery, parameter);

                parameter = new DynamicParameters();
                parameter.Add("@languageCode", languageCode);
                parameter.Add("@templateId", template.TemplateId);

                string emailTemplateLabelQuery;
                if (languageCode.Equals("EN-GB"))
                {
                    emailTemplateLabelQuery =
                        @"select etl.key as LabelKey, tl.value as TranslatedValue 
                        from master.emailtemplatelabels etl
                        INNER JOIN translation.translation tl ON etl.key=tl.name and tl.code=@languageCode
                        WHERE etl.email_template_id=@templateId";
                }
                else
                {
                    emailTemplateLabelQuery =
                        @"select etl.key as LabelKey, 
	                    coalesce(tl1.value, tl2.value) as TranslatedValue 
	                    from master.emailtemplatelabels etl
	                    left JOIN translation.translation tl1  ON etl.key=tl1.name and tl1.code=@languageCode
	                    left JOIN translation.translation tl2  ON etl.key=tl2.name and tl2.code='EN-GB'
	                    WHERE etl.email_template_id=@templateId";
                }
                IEnumerable<EmailTemplateTranslationLabel> labels = await _dataAccess.QueryAsync<EmailTemplateTranslationLabel>(emailTemplateLabelQuery, parameter);
                template.TemplateLabels = labels;
                return template;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Translationupload MapfileDetails(dynamic record)
        {
            Translationupload Entity = new Translationupload();
            Entity.Id = record.id;
            Entity.FileName = record.file_name;
            Entity.Description = record.description;
            Entity.FileSize = record.file_size;
            Entity.FailureCount = record.failure_count;
            Entity.CreatedAt = record.created_at;
            Entity.File = record.file;
            Entity.AddedCount = record.added_count;
            try
            {
                Entity.UpdatedCount = Convert.ToInt32(record.updated_count);
            }
            catch (Exception)
            {

                Entity.UpdatedCount = 0; ;
            }

            Entity.CreatedBy = record.created_by;
            return Entity;
        }

        public async Task<List<DTCwarning>> ImportDTCWarningData(List<DTCwarning> dtcwarningList)
        {
            try
            {
                var dtcwarningLists = new List<DTCwarning>();
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    foreach (DTCwarning item in dtcwarningList)
                    {
                        // If warning data is already exist then update specific record 
                        int WarningId = CheckDtcWarningClassExist(item.WarningClass, item.Number, item.Code);
                        var iconID = GetIcocIDFromIcon(item.WarningClass, item.Number);

                        if (iconID == 0)
                        {
                            item.Message = "violates foreign key constraint for Icon_ID";
                            dtcwarningLists.Add(item);
                            return dtcwarningList;
                        }

                        var LanguageCode = _translationCoreMapper.MapDTCTLanguageCode(item.Code);


                        if (WarningId == 0)
                        {
                            // Insert

                            //DTCwarning dtcwarning = new DTCwarning();
                            var InsertWarningDataQueryStatement = @"INSERT INTO master.dtcwarning(
                                                              code, type, veh_type, class, number, description, advice, expires_at, icon_id, created_at, created_by)
                                                          VALUES(@code, @type, @veh_type, @class, @number, @description, @advice, @expires_at, @icon_id, @created_at, @created_by)
                                                             RETURNING id";

                            var parameter = new DynamicParameters();
                            parameter.Add("@code", LanguageCode);
                            parameter.Add("@type", item.Type);
                            parameter.Add("@veh_type", item.VehType);
                            parameter.Add("@class", item.WarningClass);
                            parameter.Add("@number", item.Number);
                            parameter.Add("@description", item.Description);
                            parameter.Add("@advice", item.Advice);
                            parameter.Add("@expires_at", item.ExpiresAt);
                            parameter.Add("@icon_id", iconID);
                            parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                            parameter.Add("@created_by", item.CreatedBy);


                            int InsertedDTCUploadID = await _dataAccess.ExecuteScalarAsync<int>(InsertWarningDataQueryStatement, parameter);
                            if (InsertedDTCUploadID > 0)
                            {
                                item.Id = InsertedDTCUploadID;
                                dtcwarningLists.Add(item);
                            }
                        }
                        else
                        {
                            //Update


                            var UpdateWarningDataQueryStatement = @"UPDATE master.dtcwarning
                                                              SET code=@code, 
                                                                  type=@type, 
                                                                  veh_type=@veh_type,
                                                                  class=@class,
                                                                  number=@number, 
                                                                  description=@description, 
                                                                  advice=@advice,
                                                                  expires_at=@expires_at,
                                                                  icon_id=@icon_id,
                                                                  modified_at=@modified_at,
                                                                  modified_by=@modified_by
                                                           WHERE class = @class and number = @number and code =@code  RETURNING id";

                            var parameter = new DynamicParameters();
                            parameter.Add("@code", LanguageCode);
                            parameter.Add("@type", item.Type);
                            parameter.Add("@veh_type", item.VehType);
                            parameter.Add("@class", item.WarningClass);
                            parameter.Add("@number", item.Number);
                            parameter.Add("@description", item.Description);
                            parameter.Add("@advice", item.Advice);
                            parameter.Add("@expires_at", item.ExpiresAt);
                            parameter.Add("@icon_id", iconID);
                            parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                            parameter.Add("@modified_by", item.ModifyBy);

                            int UpdateDTCUploadID = await _dataAccess.ExecuteScalarAsync<int>(UpdateWarningDataQueryStatement, parameter);
                            if (UpdateDTCUploadID > 0)
                            {
                                item.Id = UpdateDTCUploadID;
                                dtcwarningLists.Add(item);
                            }

                        }

                    }
                    transactionScope.Complete();
                }
                return dtcwarningLists;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetIcocIDFromIcon(int WarningClass, int Number)
        {
            try
            {
                var QueryStatement = @" select id from master.icon
                                   where warning_class = @class AND warning_number= @number
                                     ";
                var parameter = new DynamicParameters();
                parameter.Add("@class", WarningClass);
                parameter.Add("@number", Number);
                int iconID = _dataAccess.ExecuteScalar<int>(QueryStatement, parameter);
                return iconID;
            }
            catch (Exception)
            {
                throw;
            }


        }

        public async Task<IEnumerable<DTCwarning>> GetDTCWarningData(string LanguageCode)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<DTCwarning> dtcWarninglist = new List<DTCwarning>();
                string GetDTCWarningDataQueryStatement = string.Empty;
                if (LanguageCode.Length == 2)
                {
                    LanguageCode = _translationCoreMapper.MapDTCTLanguageCode(LanguageCode);
                }

                parameter.Add("@LanguageCode", LanguageCode);
                GetDTCWarningDataQueryStatement = @"SELECT id, code, type, veh_type, class as Warningclass, number, description, advice, expires_at,icon_id, created_at, created_by, modified_at, modified_by
                                                                FROM master.dtcwarning
                                                                  where 1=1";
                GetDTCWarningDataQueryStatement = GetDTCWarningDataQueryStatement + " and code=@LanguageCode";


                dynamic result = await _dataAccess.QueryAsync<dynamic>(GetDTCWarningDataQueryStatement, parameter);
                foreach (dynamic record in result)
                {
                    dtcWarninglist.Add(_translationCoreMapper.MapWarningDetails(record));
                }

                return dtcWarninglist;


            }
            catch (Exception)
            {
                throw;
            }

        }


        public int CheckDtcWarningClassExist(int WarningClass, int WarningNumber, string excelLanguageCode)
        {
            var LanguageCode = _translationCoreMapper.MapDTCTLanguageCode(excelLanguageCode);
            var QueryStatement = @"select id 
                                    from master.dtcwarning
                                   where class=@class and number=@number and code = @code";
            var parameter = new DynamicParameters();

            parameter.Add("@class", WarningClass);
            parameter.Add("@number", WarningNumber);
            parameter.Add("@code", LanguageCode);

            int resultWarningId = _dataAccess.ExecuteScalar<int>(QueryStatement, parameter);
            return resultWarningId;

        }

        public async Task<List<DTCwarning>> UpdateDTCWarningData(List<DTCwarning> dtcwarningList)
        {
            try
            {
                var dtcwarningLists = new List<DTCwarning>();
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {


                    foreach (DTCwarning item in dtcwarningList)
                    {
                        // If warning data is already exist then update specific record 
                        int WarningId = CheckDtcWarningClassExist(item.WarningClass, item.Number, item.Code);
                        // Get Icon id from Icon table
                        var iconID = GetIcocIDFromIcon(item.WarningClass, item.Number);
                        var LanguageCode = _translationCoreMapper.MapDTCTLanguageCode(item.Code);

                        if (WarningId > 0)
                        {
                            // Update

                            var UpdateWarningDataQueryStatement = @"UPDATE master.dtcwarning
                                                              SET code=@code, 
                                                                  type=@type, 
                                                                  veh_type=@veh_type,
                                                                  class=@class,
                                                                  number=@number, 
                                                                  description=@description, 
                                                                  advice=@advice,
                                                                  expires_at=@expires_at,
                                                                  icon_id=@icon_id,
                                                                  modified_at=@modified_at,
                                                                  modified_by=@modified_by
                                                           WHERE code = @code and number = @number and code =@code  RETURNING id ";

                            var parameter = new DynamicParameters();
                            parameter.Add("@code", LanguageCode);
                            parameter.Add("@type", item.Type);
                            parameter.Add("@veh_type", item.VehType);
                            parameter.Add("@class", item.WarningClass);
                            parameter.Add("@number", item.Number);
                            parameter.Add("@description", item.Description);
                            parameter.Add("@advice", item.Advice);
                            parameter.Add("@expires_at", item.ExpiresAt);
                            parameter.Add("@icon_id", iconID);
                            parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                            parameter.Add("@modified_by", item.ModifyBy);

                            int UpdateDTCUploadID = await _dataAccess.ExecuteScalarAsync<int>(UpdateWarningDataQueryStatement, parameter);
                            if (UpdateDTCUploadID > 0)
                            {
                                item.Id = UpdateDTCUploadID;
                                dtcwarningLists.Add(item);
                            }

                        }
                    }

                    transactionScope.Complete();
                }
                return dtcwarningLists;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<int> DeleteDTCWarningData(int id)
        {
            try
            {
                var dtcwarningLists = new List<DTCwarning>();
                int DeleteDTCID = 0;
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (id != 0)
                    {
                        // Delete

                        var UpdateWarningDataQueryStatement = @"DELETE FROM master.dtcwarning
                                                                   WHERE id = @id ";

                        var parameter = new DynamicParameters();
                        parameter.Add("@id", id);

                        DeleteDTCID = await _dataAccess.ExecuteScalarAsync<int>(UpdateWarningDataQueryStatement, parameter);

                    }


                    transactionScope.Complete();
                }
                return DeleteDTCID;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
