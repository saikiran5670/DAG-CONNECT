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

        public async Task<IEnumerable<Langauge>> GetKeyTranslationByLanguageCode()
        {
                string LangagugeQuery= @"SELECT id, name, code, key, description
	                                    FROM translation.languages";

                                        
            var parameter = new DynamicParameters();
             IEnumerable<Langauge> LangagugeCodes = await dataAccess.QueryAsync<Langauge>(LangagugeQuery, parameter);
            return LangagugeCodes;
        }
    }
}
