using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.rfms.response;
using System.Linq;
using System;

namespace net.atos.daf.ct2.rfms
{
    public class RfmsManager : IRfmsManager
    {
        readonly IRfmsRepository _rfmsRepository;
        readonly IVehicleManager _vehicleManager;


        public RfmsManager(IRfmsRepository rfmsRepository, IVehicleManager vehicleManager)
        {
            _rfmsRepository = rfmsRepository;
            _vehicleManager = vehicleManager;
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

        public async Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest)
        {
            return await _rfmsRepository.GetVehiclePosition(rfmsVehiclePositionRequest);
        }

        public async Task<string> GetRFMSFeatureRate(string emailId, string featureName)
        {
            return await _rfmsRepository.GetRFMSFeatureRate(emailId, featureName);
        }
    }
}
