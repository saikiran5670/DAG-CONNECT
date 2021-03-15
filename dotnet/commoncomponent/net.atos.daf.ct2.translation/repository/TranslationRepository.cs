using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;
using Dapper;
using static Dapper.SqlMapper;
using Npgsql;
using NpgsqlTypes;
using System.Threading.Tasks;
using net.atos.daf.ct2.translation.entity;
using static net.atos.daf.ct2.translation.Enum.translationenum;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Transactions;

namespace net.atos.daf.ct2.translation.repository
{
    public class TranslationRepository : ITranslationRepository
    {
        private readonly IConfiguration _config;

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
        private readonly IDataAccess dataAccess;
        public TranslationRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        public async Task<IEnumerable<Langauge>> GetAllLanguageCode()
        {
            string LangagugeQuery = @"SELECT id, name, code, key, description
	                                    FROM translation.language";


            var parameter = new DynamicParameters();
            IEnumerable<Langauge> LangagugeCodes = await dataAccess.QueryAsync<Langauge>(LangagugeQuery, parameter);
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
            IEnumerable<Translations> translations = await dataAccess.QueryAsync<Translations>(LangagugeQuery, parameter);
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
            IEnumerable<Translations> Translations = await dataAccess.QueryAsync<Translations>(LangagugeQuery, parameter);
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
            var Translations = await dataAccess.QueryAsync<dynamic>(LangagugeQuery, parameter);
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
                    IEnumerable<Translations> Translations = await dataAccess.QueryAsync<Translations>(LangagugeQuery, parameter);

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

                    IEnumerable<Translations> Translations = await dataAccess.QueryAsync<Translations>(LangagugeQuery, parameter);
                    return Translations;

                }


            }
            catch (Exception ex)
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
                            parameter.Add("@Created_at", item.created_at);
                            parameter.Add("@modified_at", item.modified_at);
                            query = @"INSERT INTO translation.translation(code, type, name, value, created_at, modified_at) " +
                                    "values(@Code,@Type,@Name,@Value,@Created_at,@modified_at) RETURNING id";
                            var translationId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
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
            catch (Exception ex)
            {
                throw ex;
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

                dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

                foreach (dynamic record in result)
                {

                    translations.Add(Map(record));
                }
                // var result = dataAccess.ExecuteScalar<List<Translations>>(QueryStatement, parameter);
                return translations;
            }
            catch (Exception ex)
            {
                throw ex;
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
                    parameter.Add("@file_name", translationupload.file_name);
                    parameter.Add("@description", translationupload.description);
                    parameter.Add("@file_size", translationupload.file_size);
                    parameter.Add("@failure_count", translationupload.failure_count);
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameter.Add("@file", translationupload.file);
                    parameter.Add("@added_count", translationupload.added_count);
                    parameter.Add("@updated_count", translationupload.updated_count);
                    parameter.Add("@created_by", translationupload.created_by);

                    int InsertedFileUploadID = await dataAccess.ExecuteScalarAsync<int>(InsertFileDetailsQueryStatement, parameter);

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
                        translationupload.id = InsertedFileUploadID;
                    }

                    transactionScope.Complete();

                    return translationupload;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<translationStatus> InsertTranslationFileData(Translations translationdata,List<Translations> TranslationsList)
        {
            try
            {
                int IsUpdated = 0;
                var parameter = new DynamicParameters();
                string query = string.Empty;
                //TranslationsList = GetAllTranslations(translationdata.Name, translationdata.Code);
                
                var translationcodeList = TranslationsList.Where(I => I.Name == translationdata.Name).ToList();


                if (translationcodeList != null && translationcodeList.Count > 0)
                {
                    var translationobjdata = translationcodeList.Where(I => I.Name == translationdata.Name && I.Code == translationdata.Code).FirstOrDefault();
                    if (translationobjdata != null)
                    {
                        parameter = new DynamicParameters();
                        parameter.Add("@id", translationobjdata.Id);
                        parameter.Add("@Code", translationobjdata.Code);
                        parameter.Add("@Type", translationobjdata.Type);
                        parameter.Add("@Name", translationobjdata.Name);
                        parameter.Add("@Value", translationobjdata.Value);
                        //parameter.Add("@Created_at", translationdata.created_at);
                        parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                        query = @"update translation.translation set 
                                code= @Code,type= @Type,name= @Name,value = @Value,modified_at = @modified_at Where id=@id RETURNING id";
                        var translationId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        return translationStatus.Updated;
                    }
                    else
                    {

                        parameter = new DynamicParameters();
                        parameter.Add("@Code", translationdata.Code);
                        parameter.Add("@Type", translationdata.Type);
                        parameter.Add("@Name", translationdata.Name);
                        parameter.Add("@Value", translationdata.Value);
                        parameter.Add("@Created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                        //parameter.Add("@modified_at", translationdata.modified_at);
                        query = @"INSERT INTO translation.translation(code, type, name, value, created_at) " +
                                "values(@Code,@Type,@Name,@Value,@Created_at) RETURNING id";
                        var translationId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        return translationStatus.Added;
                    }
                }
                else
                {
                    return translationStatus.Failed;
                }
                

               

            }
            catch (Exception ex)
            {

                throw ex;
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
                dynamic result = await dataAccess.QueryAsync<dynamic>(InsertFileDetailsQueryStatement, parameter);
                foreach (dynamic record in result)
                {
                    fileuploadlist.Add(MapfileDetails(record));
                }
                return fileuploadlist.AsEnumerable();


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private Translationupload MapfileDetails(dynamic record)
        {
            Translationupload Entity = new Translationupload();
            Entity.id = record.id;
            Entity.file_name = record.file_name;
            Entity.description = record.description;
            Entity.file_size = record.file_size;
            Entity.failure_count = record.failure_count;
            Entity.created_at = record.created_at;
            Entity.file = record.file;
            Entity.added_count = record.added_count;
            Entity.updated_count = record.updated_count;
            Entity.created_by = record.created_by;
            return Entity;
        }
    }
}
