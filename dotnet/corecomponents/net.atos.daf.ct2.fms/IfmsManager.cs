
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.fms.entity;

namespace net.atos.daf.ct2.fms
{
    public interface IFmsManager
    {
        Task<VehiclePositionResponse> GetVehiclePosition(string vin, string since);
        Task<VehicleStatusResponse> GetVehicleStatus(string vin, string since);
    }
}