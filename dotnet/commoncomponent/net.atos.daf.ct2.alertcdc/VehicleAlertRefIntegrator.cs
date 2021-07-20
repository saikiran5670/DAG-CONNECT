using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.alertcdc.entity;
using net.atos.daf.ct2.alertcdc.repository;

namespace net.atos.daf.ct2.alertcdc
{
    public class VehicleAlertRefIntegrator : IVehicleAlertRefIntegrator
    {
        private readonly IVehicleAlertRepository _vehicleAlertRepository;

        public VehicleAlertRefIntegrator(IVehicleAlertRepository vehicleAlertRepository)
        {
            _vehicleAlertRepository = vehicleAlertRepository;
        }

        public Task GetVehicleAlertRefFromAlertConfiguration(List<int> alertIds) => ExtractAndSyncVehicleAlertRefByAlertIds(alertIds);
        public Task GetVehicleAlertRefFromAccountVehicleGroupMapping(List<int> vins, List<int> accounts) => Task.CompletedTask;
        public Task GetVehicleAlertRefFromSubscriptionManagement(List<int> subscriptionIds) => Task.CompletedTask;
        public Task GetVehicleAlertRefFromVehicleManagement(List<int> vins) => Task.CompletedTask;

        internal async Task<List<VehicleAlertRef>> ExtractAndSyncVehicleAlertRefByAlertIds(List<int> alertIds)
        {
            List<VehicleAlertRef> masterVehicleAlerts = await _vehicleAlertRepository.GetVehiclesFromAlertConfiguration(alertIds);
            List<VehicleAlertRef> datamartVehicleAlerts = await _vehicleAlertRepository.GetVehicleAlertRefByAlertIds(alertIds);
            await _vehicleAlertRepository.InsertVehicleAlertRef(datamartVehicleAlerts);
            await _vehicleAlertRepository.UpdateVehicleAlertRef(datamartVehicleAlerts);
            await _vehicleAlertRepository.DeleteVehicleAlertRef(datamartVehicleAlerts);
            return datamartVehicleAlerts;
        }
    }
}
