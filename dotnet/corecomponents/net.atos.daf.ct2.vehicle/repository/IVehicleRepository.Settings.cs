using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.vehicle.repository
{
    public partial interface IVehicleRepository
    {
        public Task<IEnumerable<Vehicle>> GetVehicleSetting(VehicleSettings vehicleSettings);

        public Task<List<VehicleConnect>> VehicleConnectAll(List<VehicleConnect> vehicleConnects);
    }
}
