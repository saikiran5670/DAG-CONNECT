using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.rfms.response;
using System.Linq;
using System;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace net.atos.daf.ct2.rfms
{
    public class RfmsManager : IRfmsManager
    {
        readonly IRfmsRepository _rfmsRepository;
        readonly IVehicleManager _vehicleManager;
        readonly IMemoryCache _cache;


        public RfmsManager(IRfmsRepository rfmsRepository, IVehicleManager vehicleManager, IMemoryCache memoryCache)
        {
            _rfmsRepository = rfmsRepository;
            _vehicleManager = vehicleManager;
            _cache = memoryCache;
        }

        public async Task<RfmsVehicles> GetVehicles(string lastVin, int thresholdValue, int accountId, int orgId)
        {
            string visibleVins = string.Empty;
            var visibleVehicles = await _vehicleManager.GetVisibilityVehicles(accountId, orgId);
            int lastVinId = 0;
            if (visibleVehicles.Count > 0)
            {
                if (!string.IsNullOrEmpty(lastVin))
                {
                    //Get Id for the last vin
                    var id = visibleVehicles.Where(x => x.VIN == lastVin).Select(p => p.Id);
                    if (id != null)
                        lastVinId = Convert.ToInt32(id.FirstOrDefault());
                }
                visibleVins = string.Join(",", visibleVehicles.Select(p => p.VIN.ToString()));

            }
            RfmsVehicles rfmsVehicles = await _rfmsRepository.GetVehicles(visibleVins, lastVinId);

            if (rfmsVehicles.Vehicles.Count > thresholdValue)
            {
                rfmsVehicles.Vehicles = rfmsVehicles.Vehicles.Take(thresholdValue).ToList();
                rfmsVehicles.MoreDataAvailable = true;
            }

            return rfmsVehicles;
        }

        //OPTIMIZE & CLEAN THE CODE
        public async Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest)
        {
            int lastVinId = 0;
            string visibleVins = string.Empty;

            //CHECK VISIBLE VEHICLES FOR USER
            var visibleVehicles = await _vehicleManager.GetVisibilityVehicles(rfmsVehiclePositionRequest.AccountId, rfmsVehiclePositionRequest.OrgId);

            //ADD MASTER DATA TO CACHE
            AddMasterDataToCache();

            if (visibleVehicles.Count > 0)
            {
                if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.TriggerFilter))
                {
                    var triggerFilterId = GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TRIGGER_TYPE, rfmsVehiclePositionRequest.TriggerFilter.ToLower(), true);
                    if (triggerFilterId != null)
                    {
                        rfmsVehiclePositionRequest.TriggerFilter = triggerFilterId;
                    }
                    else
                    {
                        rfmsVehiclePositionRequest.TriggerFilter = CommonConstants.NOT_APPLICABLE;
                    }
                }
                if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.LastVin))
                {
                    //Get Id for the last vin
                    var id = visibleVehicles.Where(x => x.VIN == rfmsVehiclePositionRequest.LastVin).Select(p => p.Id);
                    if (id != null)
                        lastVinId = Convert.ToInt32(id.FirstOrDefault());
                }
                if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.Vin))
                {
                    var validVin = visibleVehicles.Where(x => x.VIN == rfmsVehiclePositionRequest.Vin).Select(p => p.VIN).FirstOrDefault();
                    visibleVins = validVin;
                }
                else
                {
                    visibleVins = string.Join(",", visibleVehicles.Select(p => p.VIN.ToString()));
                }
            }

            RfmsVehiclePosition rfmsVehiclePosition = await _rfmsRepository.GetVehiclePosition(rfmsVehiclePositionRequest, visibleVins, lastVinId);

            if (rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Count() > rfmsVehiclePositionRequest.ThresholdValue)
            {
                if (rfmsVehiclePositionRequest.LatestOnly && string.IsNullOrEmpty(rfmsVehiclePositionRequest.LastVin))
                {
                    rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Take(rfmsVehiclePositionRequest.ThresholdValue).ToList();
                    string lastVin = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Last().Vin;
                    string lastReceivedDateTime = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Last().ReceivedDateTime.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                    if (rfmsVehiclePositionRequest.LatestOnly)
                        rfmsVehiclePosition.MoreDataAvailableLink = "/vehiclepositions?LatestOnly=true&lastVin=" + lastVin;
                    else
                        rfmsVehiclePosition.MoreDataAvailableLink = "/vehiclepositions?starttime=" + lastReceivedDateTime + "&lastVin=" + lastVin;
                    rfmsVehiclePosition.MoreDataAvailable = true;
                }
                else if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.StartTime))
                {
                    rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Take(rfmsVehiclePositionRequest.ThresholdValue).ToList();
                    string lastVin = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Last().Vin;
                    string lastReceivedDateTime = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Last().ReceivedDateTime.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                    rfmsVehiclePosition.MoreDataAvailableLink = "/vehiclepositions?starttime=" + lastReceivedDateTime + "&lastVin=" + lastVin;
                    rfmsVehiclePosition.MoreDataAvailable = true;
                }
                if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.Vin))
                {
                    rfmsVehiclePosition.MoreDataAvailableLink += "&vin=" + rfmsVehiclePositionRequest.Vin;
                }
                if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.TriggerFilter))
                {
                    rfmsVehiclePosition.MoreDataAvailableLink += "&triggerFilter=" + GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TRIGGER_TYPE, rfmsVehiclePositionRequest.TriggerFilter, false);
                }
            }

            int vehicleCnt = 0;

            foreach (var vehiclePos in rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions)
            {
                if (!string.IsNullOrEmpty(vehiclePos.TriggerType.Type))
                {
                    string triggerName = GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TRIGGER_TYPE, vehiclePos.TriggerType.Type, false);
                    vehiclePos.TriggerType.Type = triggerName;
                }
                if (!string.IsNullOrEmpty(vehiclePos.TriggerType.DriverId.TachoDriverIdentification.DriverAuthenticationEquipment))
                {
                    string driverAuthId = GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.DRIVER_AUTH_EQUIPMENT, vehiclePos.TriggerType.DriverId.TachoDriverIdentification.DriverAuthenticationEquipment, false);
                    vehiclePos.TriggerType.DriverId.TachoDriverIdentification.DriverAuthenticationEquipment = driverAuthId;
                }
                if (!string.IsNullOrEmpty(vehiclePos.TriggerType.TellTaleInfo.TellTale))
                {
                    string tellTale = GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TALE_TELL, vehiclePos.TriggerType.TellTaleInfo.TellTale, false);
                    vehiclePos.TriggerType.TellTaleInfo.TellTale = tellTale;
                }
                if (!string.IsNullOrEmpty(vehiclePos.TriggerType.TellTaleInfo.State))
                {
                    string state = GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TALE_TELL_STATE, vehiclePos.TriggerType.TellTaleInfo.State, false);
                    vehiclePos.TriggerType.TellTaleInfo.State = state;
                }
                vehicleCnt++;
            }
            return rfmsVehiclePosition;
        }

        public async Task<string> GetRFMSFeatureRate(string emailId, string featureName)
        {
            return await _rfmsRepository.GetRFMSFeatureRate(emailId, featureName);
        }
        private string GetMasterDataValueFromCache(string tableName, string key, bool isGetName)
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
        private async void AddMasterDataToCache()
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
