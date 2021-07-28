using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface IVehicleCdcRepository
    {
        Task<List<VehicleCdc>> GetVehicleCdc(List<int> vid);
    }
}