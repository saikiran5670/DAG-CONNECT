using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface ILandmarkAlertCdcRepository
    {
        Task<List<VehicleAlertRef>> GetVehiclesFromAlertConfiguration(List<int> alertId);
        Task<List<VehicleAlertRef>> GetVehicleAlertRefByAlertIds(List<int> alertId);
        Task<List<int>> GetAlertsbyLandmarkId(int landmarkid, string landmarktype);
    }
}
