
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.fms.entity;

namespace net.atos.daf.ct2.fms.repository
{
    public interface IFmsRepository
    {
        Task<VehiclePositionResponse> GetVehiclePosition(string vin, string since);
        Task<VehiclePositionResponse> GetVehiclePosition(List<string> vin, string since);
        Task<VehicleStatusResponse> GetVehicleStatus(string vin, string since);
        Task<VehicleStatusResponse> GetVehicleStatus(List<string> vin, string since);
    }
}