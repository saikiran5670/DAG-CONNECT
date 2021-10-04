using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.rfms.response;
using System.Linq;
using System;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using net.atos.daf.ct2.rfms.common;
using net.atos.daf.ct2.vehicle.entity;
using System.Diagnostics.CodeAnalysis;

namespace net.atos.daf.ct2.rfms
{
    public class RfmsManager : IRfmsManager
    {
        readonly IRfmsRepository _rfmsRepository;
        readonly IVehicleManager _vehicleManager;
        readonly IMemoryCache _cache;
        internal int LastVinId { get; set; }
        private readonly VehiclePositionExtension _vehiclePositionExtension;
        private readonly RfmsVehicleMasterTableCache _rfmsVehicleMasterTableCache;
        private readonly RfmsVehicleStatusExtension _rfmsVehicleStatusExtension;


        public RfmsManager(IRfmsRepository rfmsRepository, IVehicleManager vehicleManager, IMemoryCache memoryCache)
        {
            _rfmsRepository = rfmsRepository;
            _vehicleManager = vehicleManager;
            _cache = memoryCache;
            _rfmsVehicleMasterTableCache = new RfmsVehicleMasterTableCache(_rfmsRepository, _cache);
            _vehiclePositionExtension = new VehiclePositionExtension(_rfmsVehicleMasterTableCache);
            _rfmsVehicleStatusExtension = new RfmsVehicleStatusExtension(_rfmsVehicleMasterTableCache);

        }

        public async Task<RfmsVehicles> GetVehicles(string lastVin, int thresholdValue, int accountId, int orgId)
        {
            string visibleVins = string.Empty;
            var result = await _vehicleManager.GetVisibilityVehicles(accountId, orgId);
            var visibleVehicles = result.Values.SelectMany(x => x).Distinct(new ObjectComparer()).ToList();
            int lastVinId = 0;
            RfmsVehicles rfmsVehicles = new RfmsVehicles();
            rfmsVehicles.Vehicles = new List<response.Vehicle>();
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
                rfmsVehicles = await _rfmsRepository.GetVehicles(visibleVins, lastVinId);

            }

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
            await _rfmsVehicleMasterTableCache.AddMasterDataToCache();
            LastVinId = 0;
            var visibleVins = await GetVisibleVins(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter, rfmsVehiclePositionRequest.AccountId, rfmsVehiclePositionRequest.OrgId) ?? string.Empty;
            RfmsVehiclePosition rfmsVehiclePosition = await _rfmsRepository.GetVehiclePosition(rfmsVehiclePositionRequest, visibleVins, LastVinId);

            if (rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Count() > rfmsVehiclePositionRequest.ThresholdValue)
            {
                if (rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.LatestOnly && string.IsNullOrEmpty(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.LastVin))
                {
                    _vehiclePositionExtension.GetLatestOnlyVehiclePostion(rfmsVehiclePosition, rfmsVehiclePositionRequest.ThresholdValue);
                }
                else if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StartTime))
                {
                    _vehiclePositionExtension.GetStartDateTimeVehiclePostion(rfmsVehiclePosition, rfmsVehiclePositionRequest.ThresholdValue);
                }
                if (!string.IsNullOrEmpty(rfmsVehiclePosition.MoreDataAvailableLink))
                {
                    _vehiclePositionExtension.SetPostionMoreDataAvailableLink(rfmsVehiclePositionRequest, rfmsVehiclePosition);
                }
            }

            _vehiclePositionExtension.SetVehiclePostionTriggerData(rfmsVehiclePosition);
            return rfmsVehiclePosition;
        }


        public async Task<string> GetRFMSFeatureRate(string emailId, string featureName)
        {
            return await _rfmsRepository.GetRFMSFeatureRate(emailId, featureName);
        }


        public async Task<RfmsVehicleStatus> GetRfmsVehicleStatus(RfmsVehicleStatusRequest rfmsVehicleStatusRequest)
        {
            await _rfmsVehicleMasterTableCache.AddMasterDataToCache();

            LastVinId = 0;
            DateTime currentdatetime = DateTime.Now;
            var visibleVins = await GetVisibleVins(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter, rfmsVehicleStatusRequest.AccountId, rfmsVehicleStatusRequest.OrgId) ?? string.Empty;

            RfmsVehicleStatus rfmsVehicleStatus = await _rfmsRepository.GetRfmsVehicleStatus(rfmsVehicleStatusRequest, visibleVins, LastVinId);
            if (rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses.Count() > rfmsVehicleStatusRequest.ThresholdValue)
            {
                if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LatestOnly && string.IsNullOrEmpty(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LastVin))
                {
                    _rfmsVehicleStatusExtension.GetLatestOnlyVehicleStatus(rfmsVehicleStatus, rfmsVehicleStatusRequest.ThresholdValue);

                }
                else if (!string.IsNullOrEmpty(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StartTime))
                {
                    _rfmsVehicleStatusExtension.GetStartDateTimeVehicleStatus(rfmsVehicleStatus, rfmsVehicleStatusRequest.ThresholdValue);


                }
                if (!string.IsNullOrEmpty(rfmsVehicleStatus.MoreDataAvailableLink))
                {
                    _rfmsVehicleStatusExtension.SetStatusMoreDataAvailableLink(rfmsVehicleStatusRequest, rfmsVehicleStatus);
                }
            }
            _rfmsVehicleStatusExtension.SetVehicleStatusTriggerData(rfmsVehicleStatus);

            rfmsVehicleStatus.RequestServerDateTime = currentdatetime.ToString("yyyy-MM-ddThh:mm:ss.fffZ");

            return rfmsVehicleStatus;
        }


        private async Task<string> GetVisibleVins(RfmsVehiclePositionStatusFilter rfmsVehicleStatusFilter, int accountId, int OrgId)
        {
            string visibleVins = string.Empty;

            //CHECK VISIBLE VEHICLES FOR USER
            var result = await _vehicleManager.GetVisibilityVehicles(accountId, OrgId);
            var visibleVehicles = result.Values.SelectMany(x => x).Distinct(new ObjectComparer()).ToList();

            //ADD MASTER DATA TO CACHE
            //AddMasterDataToCache();
            if (visibleVehicles.Count > 0)
            {
                if (!string.IsNullOrEmpty(rfmsVehicleStatusFilter.TriggerFilter))
                {
                    var triggerFilterId = _rfmsVehicleMasterTableCache.GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TRIGGER_TYPE, rfmsVehicleStatusFilter.TriggerFilter.ToLower(), true);
                    if (triggerFilterId != null)
                    {
                        rfmsVehicleStatusFilter.TriggerFilter = triggerFilterId;
                    }
                    else
                    {
                        rfmsVehicleStatusFilter.TriggerFilter = CommonConstants.NOT_APPLICABLE;
                    }
                }
                if (!string.IsNullOrEmpty(rfmsVehicleStatusFilter.LastVin))
                {
                    //Get Id for the last vin
                    var id = visibleVehicles.Where(x => x.VIN == rfmsVehicleStatusFilter.LastVin).Select(p => p.Id);
                    if (id != null)
                    {
                        LastVinId = Convert.ToInt32(id.FirstOrDefault());
                    }
                }
                if (!string.IsNullOrEmpty(rfmsVehicleStatusFilter.Vin))
                {
                    var validVin = visibleVehicles.Where(x => x.VIN == rfmsVehicleStatusFilter.Vin).Select(p => p.VIN).FirstOrDefault();
                    visibleVins = validVin;
                }
                else
                {
                    visibleVins = string.Join(",", visibleVehicles.Select(p => p.VIN.ToString()));
                }
            }

            return visibleVins;
        }

        internal class ObjectComparer : IEqualityComparer<VisibilityVehicle>
        {
            public bool Equals(VisibilityVehicle x, VisibilityVehicle y)
            {
                if (object.ReferenceEquals(x, y))
                {
                    return true;
                }
                if (x is null || y is null)
                {
                    return false;
                }
                return x.Id == y.Id && x.VIN == y.VIN;
            }

            public int GetHashCode([DisallowNull] VisibilityVehicle obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                int idHashCode = obj.Id.GetHashCode();
                int vinHashCode = obj.VIN == null ? 0 : obj.VIN.GetHashCode();
                return idHashCode ^ vinHashCode;
            }
        }
    }
}
