
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.fms.entity;

namespace net.atos.daf.ct2.fms.repository
{
    public interface IFmsRepository
    {
        Task<List<VehiclePositionResponse>> GetVehiclePosition(string vin, string since);
        Task<List<VehiclePositionResponse>> GetVehicleStatus(string vin, string since);
    }
}