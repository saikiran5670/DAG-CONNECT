using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.vehicle.repository
{
    public partial class VehicleRepository : IVehicleRepository
    {
        public async Task<VehicleConnectedResult> UpdateVehicleConnection(List<VehicleConnect> vehicleConnects)
        {
            var connectedVehicles = new VehicleConnectedResult() { VehicleConnectedList = new List<VehicleConnect>(), VehicleConnectionfailedList = new List<VehicleConnect>() };
            try
            {
                foreach (var vehicle in vehicleConnects)
                {
                    bool result = await SetOptInStatus(vehicle.Opt_In, vehicle.ModifiedBy, vehicle.VehicleId);
                    if (result)
                    {
                        connectedVehicles.VehicleConnectedList.Add(vehicle);
                    }
                    else
                    {
                        connectedVehicles.VehicleConnectionfailedList.Add(vehicle);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            return connectedVehicles;
        }
    }
}
