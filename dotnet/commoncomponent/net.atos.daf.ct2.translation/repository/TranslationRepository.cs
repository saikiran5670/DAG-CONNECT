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
	                                    FROM translation.language";

                                        
            var parameter = new DynamicParameters();
             IEnumerable<Langauge> LangagugeCodes = await dataAccess.QueryAsync<Langauge>(LangagugeQuery, parameter);
            return LangagugeCodes;
        }

        public async Task<IEnumerable<Translations>> GetKeyTranslationByLanguageCode(string langaguecode,string key)
        {
                string LangagugeQuery= @"select  t.id,t.name,t.value,t.type from translation.translation t
                                        where t.code = @langaguecode and t.name = @key";

                                        
                var parameter = new DynamicParameters();
                parameter.Add("@langaguecode", langaguecode);
                parameter.Add("@key", key);
                IEnumerable<Translations> translations = await dataAccess.QueryAsync<Translations>(LangagugeQuery, parameter);
                return translations;
        }

        public async Task<IEnumerable<Translations>> GetLangagugeTranslationByKey(string key)
        {
                string LangagugeQuery= @"select  t.id,t.name,t.code,t.value,t.type from translation.translation t
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

        public async Task<IEnumerable<Translations>> GetTranslationsByMenu(int  MenuId, string type,string langaguecode)
        {
                string LangagugeQuery= @"select tg.id,t.name,t.value,t.type,t.code,tg.ref_id from translation.translation t
                                        inner join translation.translationgrouping tg
                                        on t.name = tg.name where t.code=@langaguecode";

                    var parameter = new DynamicParameters();
                    parameter.Add("@langaguecode", langaguecode);
                   
                        parameter.Add("@menuid", MenuId);
                        LangagugeQuery = LangagugeQuery + " and tg.ref_id  = @menuid";


                    if (type != null)
                    {
                        parameter.Add("@type", type.ToString());
                        LangagugeQuery = LangagugeQuery + " and tg.type  = @type";

                    }  
                    List<Translations> list=  new      List<Translations>();               
                var Translations = await dataAccess.QueryAsync<dynamic>(LangagugeQuery, parameter);
                foreach(var item in Translations)
                {
                        list.Add(Map(item));
                }
                var names =  list.Where(T=>T.Type == ((char)TranslationType.Dropdown).ToString()).SelectMany(p=> p.Name.Split('_')).Distinct().Where(K=> K[0] == 'd');
                foreach(var name in names)
                {
                    IEnumerable<Translations> dropdowntranslation = await GetTranslationsForDropDowns(name.Substring(1),langaguecode) ;
                    foreach(var item in dropdowntranslation)
                    {
                        list.Where(P=>P.Name == item.Name).ToList().ForEach(i=>
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
                if(Dropdownname == "language")
                LangagugeQuery= @"select tc.id,t.name,t.code,t.value,t.type from translation.language tc inner join translation.translation t on tc.key = t.name ";
                else
                {
                    LangagugeQuery= @"select tc.id,t.name,t.code,t.value,t.type from master." + Dropdownname +" tc inner join translation.translation t on tc.key = t.name ";
                }   
                                        
                var parameter = new DynamicParameters();
                parameter.Add("@code", langagugecode);
                LangagugeQuery = LangagugeQuery + " Where t.code=  @code";
                IEnumerable<Translations> Translations = await  dataAccess.QueryAsync<Translations>(LangagugeQuery,parameter);
                return Translations;
            }
            catch (Exception ex)
            {
               return Enumerable.Empty<Translations>();
            }
            
        }

            private Translations Map(dynamic record)
            {
                Translations Entity = new Translations();
                Entity.Id= record.id;
                Entity.Code = record.code;
                Entity.Type = record.type;
                Entity.Name =  record.name;
                Entity.Value = record.value;
                Entity.Filter =  record.filter;
                Entity.MenuId =  record.ref_id;
                return Entity;
            }   

    }
}
