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

namespace net.atos.daf.ct2.translation.repository
{
    public class TranslationRepository : ITranslationRepository
    {
           private readonly IConfiguration  _config;
       
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
                string LangagugeQuery= @"SELECT id, name, code, key, description
	                                    FROM translation.languages";

                                        
            var parameter = new DynamicParameters();
             IEnumerable<Langauge> LangagugeCodes = await dataAccess.QueryAsync<Langauge>(LangagugeQuery, parameter);
            return LangagugeCodes;
        }

        public async Task<IEnumerable<translations>> GetKeyTranslationByLanguageCode(string langaguecode,string key)
        {
                string LangagugeQuery= @"select  t.id,t.name,t.value,t.type from translation.translation t
                                        where t.code = @langaguecode and t.name = @key";

                                        
                var parameter = new DynamicParameters();
                parameter.Add("@langaguecode", langaguecode);
                parameter.Add("@key", key);
                IEnumerable<translations> translations = await dataAccess.QueryAsync<translations>(LangagugeQuery, parameter);
                return translations;
        }

        public async Task<IEnumerable<translations>> GetLangagugeTranslationByKey(string key, string Type)
        {
                string LangagugeQuery= @"select  t.id,t.name,t.value,t.type from translation.translation t
                                        where 1=1";

                                        
                var parameter = new DynamicParameters();
                 if (key.Length > 0)
                    {
                        parameter.Add("@key", key);
                        LangagugeQuery = LangagugeQuery + " and  t.name = @key";

                    }

                if (Type!= null && Type.Length > 0)
                    {
                        parameter.Add("@type", Type);
                        LangagugeQuery = LangagugeQuery + " and  t.type = @type";

                    }

                parameter.Add("@key", key);
                IEnumerable<translations> Translations = await dataAccess.QueryAsync<translations>(LangagugeQuery, parameter);
                return Translations;
        }

        public async Task<IEnumerable<translations>> GetTranslationsByMenu(int  MenuId, string type)
        {
                string LangagugeQuery= @"select tg.id,t.name,t.value,t.type from translation.translation t
                                        inner join translation.translationgrouping tg
                                        on t.name = tg.name where 1=1";

                    var parameter = new DynamicParameters();
                    if (MenuId > 0)
                    {
                        parameter.Add("@menuid", MenuId);
                        LangagugeQuery = LangagugeQuery + " and tg.ref_id  = @menuid";

                    }

                    if (type != null)
                    {
                        parameter.Add("@type", type.ToString());
                        LangagugeQuery = LangagugeQuery + " and tg.type  = @type";

                    }                      
                IEnumerable<translations> Translations = await dataAccess.QueryAsync<translations>(LangagugeQuery, parameter);
                var names =  Translations.Where(T=>T.Type == ((char)TranslationType.Dropdown).ToString()).SelectMany(p=> p.Name.Split('_')).Distinct().Where(K=> K[0] == 'd');
                foreach(var name in names)
                {
                    IEnumerable<translations> dropdowntranslation = GetTranslationsForDropDowns(name.Substring(1),"EN-GB") ;
                    foreach(var item in dropdowntranslation)
                    {
                        Translations.Where(P=>P.Name == item.Name).ToList().ForEach(i=>
                                                                                    {
                                                                                        i.Id = item.Id;
                                                                                        i.Filter = name.Substring(1);
                                                                                    });
                    }
                    
                }
                
                return Translations;
        }

        public IEnumerable<translations> GetTranslationsForDropDowns(string Dropdownname, string langagugeid)
        {
            try
            {
                string LangagugeQuery = "";
                if(Dropdownname == "language")
                LangagugeQuery= @"select tc.id,t.name,t.value,t.type from translation.language tc inner join translation.translation t on tc.key = t.name ";
                else
                {
                    LangagugeQuery= @"select tc.id,t.name,t.value,t.type from master." + Dropdownname +" tc inner join translation.translation t on tc.key = t.name ";
                }   
                                        
                var parameter = new DynamicParameters();
                parameter.Add("@code", langagugeid);
                LangagugeQuery = LangagugeQuery + " Where t.code=  @code";
                IEnumerable<translations> Translations =  dataAccess.Query<translations>(LangagugeQuery,parameter);
                return Translations;
            }catch (Exception ex)
            {
                throw ex;
            }
            
        }

    }
}
