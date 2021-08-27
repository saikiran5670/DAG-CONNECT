using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.repository;

namespace net.atos.daf.ct2.rfms.common
{
    public class RfmsVehicleMasterTableCache
    {
        private readonly IRfmsRepository _rfmsRepository;
        private readonly IMemoryCache _cache;
        public RfmsVehicleMasterTableCache(IRfmsRepository rfmsRepository, IMemoryCache cache)
        {
            _rfmsRepository = rfmsRepository;
            _cache = cache;
        }

        internal string GetMasterDataValueFromCache(string tableName, string key, bool isGetName)
        {
            string result = string.Empty;
            if (_cache.TryGetValue(MasterMemoryObjectCacheConstants.MASTER_DATA_MEMORY_CACHEKEY, out IDictionary<string, List<MasterTableCacheObject>> cacheMasterDataDictionary))
            {
                try
                {
                    var table = cacheMasterDataDictionary[tableName];
                    if (isGetName)
                    {
                        result = table.Where(x => x.Name.ToLower().Contains(key)).Select(p => p.Id.ToString()).FirstOrDefault();
                    }
                    else
                    {
                        if (key != null)
                        {
                            int id = Convert.ToInt32(key);
                            result = table.Where(x => x.Id == id).Select(p => p.Name).FirstOrDefault();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return result;
        }
        internal async Task AddMasterDataToCache()
        {
            if (!_cache.TryGetValue(MasterMemoryObjectCacheConstants.MASTER_DATA_MEMORY_CACHEKEY, out IDictionary<string, List<MasterTableCacheObject>> cacheMasterDataDictionary))
            {
                var lstMasterDbObjects = await _rfmsRepository.GetMasterTableCacheData();

                IDictionary<string, List<MasterTableCacheObject>> masterDataDictionary = new Dictionary<string, List<MasterTableCacheObject>>();

                List<MasterTableCacheObject> lstVehicleMsgTriggerType = lstMasterDbObjects.Where(x => x.TableName == MasterMemoryObjectCacheConstants.TRIGGER_TYPE).ToList();
                List<MasterTableCacheObject> lstDriverAuthEquipment = lstMasterDbObjects.Where(x => x.TableName == MasterMemoryObjectCacheConstants.DRIVER_AUTH_EQUIPMENT).ToList();
                List<MasterTableCacheObject> lstTellTale = lstMasterDbObjects.Where(x => x.TableName == MasterMemoryObjectCacheConstants.TALE_TELL).ToList();
                List<MasterTableCacheObject> lstTState = lstMasterDbObjects.Where(x => x.TableName == MasterMemoryObjectCacheConstants.TALE_TELL_STATE).ToList();

                masterDataDictionary.Add(MasterMemoryObjectCacheConstants.TRIGGER_TYPE, lstVehicleMsgTriggerType);
                masterDataDictionary.Add(MasterMemoryObjectCacheConstants.DRIVER_AUTH_EQUIPMENT, lstDriverAuthEquipment);
                masterDataDictionary.Add(MasterMemoryObjectCacheConstants.TALE_TELL, lstTellTale);
                masterDataDictionary.Add(MasterMemoryObjectCacheConstants.TALE_TELL_STATE, lstTState);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(100));
                // Save data in cache.
                _cache.Set(MasterMemoryObjectCacheConstants.MASTER_DATA_MEMORY_CACHEKEY, masterDataDictionary, cacheEntryOptions);
            }
        }




    }
}
