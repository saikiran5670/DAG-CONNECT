
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.fms.entity;

namespace net.atos.daf.ct2.rfms
{
    public interface IFmsManager
    {
        Task<List<VehiclePositionResponse>> GetVehiclePosition(string vin, string since);
        Task<List<VehiclePositionResponse>> GetVehicleStatus(string vin, string since);
    }
}