using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Caching.Memory;
using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.accountdataservice
{
    public class MasterData
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILog _logger;
        private readonly IDataAccess _dataAccess;
        public MasterData(IMemoryCache memoryCache, IDataAccess dataAccess)
        {
            _memoryCache = memoryCache;
            _dataAccess = dataAccess;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public async Task<string[]> GetCachedLanguages()
        {
            if (_memoryCache.TryGetValue(CacheConstants.LanguageCodeKey, out string[] languages))
            {
                return languages;
            }
            else
            {
                _logger.Info("[AccountDataService] Master data fetched from database for Language.");
                var languageCodes = await _dataAccess.QueryAsync<string>("select lower(code) from translation.language");

                _memoryCache.Set(CacheConstants.LanguageCodeKey, languageCodes.ToArray());

                return languageCodes.ToArray();
            }
        }

        public async Task<string[]> GetCachedTimeZones()
        {
            if (_memoryCache.TryGetValue(CacheConstants.TimeZoneKey, out string[] timeZones))
            {
                return timeZones;
            }
            else
            {
                _logger.Info("[AccountDataService] Master data fetched from database for Timezone.");

                var timeZoneNames = await _dataAccess.QueryAsync<string>("select lower(name) from master.timezone");

                _memoryCache.Set(CacheConstants.TimeZoneKey, timeZoneNames.ToArray());

                return timeZoneNames.ToArray();
            }
        }

        public async Task<string[]> GetCachedDateFormats()
        {
            if (_memoryCache.TryGetValue(CacheConstants.DateFormatKey, out string[] dateFormats))
            {
                return dateFormats;
            }
            else
            {
                _logger.Info("[AccountDataService] Master data fetched from database for Date format.");

                var dateFormatNames = await _dataAccess.QueryAsync<string>("select lower(name) from master.dateformat");

                _memoryCache.Set(CacheConstants.DateFormatKey, dateFormatNames.ToArray());

                return dateFormatNames.ToArray();
            }
        }
    }
}
