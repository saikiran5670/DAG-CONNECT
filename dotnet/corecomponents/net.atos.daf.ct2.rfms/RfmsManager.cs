using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.rfms.response;
using System.Linq;


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

        public async Task<RfmsVehicles> GetVehicles(string lastVin, bool moreData, int accountId, int orgId)
        {
            //Fetch all vehicle details from the database if value for lastVin is not provided
            //If value for lastVin is provided only fetch for specific Vin number
            //We might need to add further filteration criteria here once DB Design is finalized
            //***********THIS NEEDS CHANGE LATER*********
            RfmsVehicles rfmsVehicles = await _rfmsRepository.GetVehicles(lastVin, moreData);

            //Check if the count for vehicles is more than 0
            if (rfmsVehicles.Vehicles.Count > 0)
            {
                //Check to find if the vehicle is visible to given user or not
                //Based on the results the vehicles from the list would be filtered and result provide to the user
                var visibleVehicles = await _vehicleManager.GetVisibilityVehicles(accountId, orgId);
                rfmsVehicles.Vehicles = rfmsVehicles.Vehicles.Where(x => visibleVehicles.Any(veh => veh.VIN == x.Vin)).ToList();
            }
            return rfmsVehicles;
        }

        public async Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest)
        {
            return await _rfmsRepository.GetVehiclePosition(rfmsVehiclePositionRequest);
        }
    }
}
