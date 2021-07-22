using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.confluentkafka.entity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace net.atos.daf.ct2.kafkacdc
{
    public class VehicleAlertRefIntegrator : IVehicleAlertRefIntegrator
    {
        private readonly IConfiguration _configuration;

        private readonly IVehicleAlertRepository _vehicleAlertRepository;
        private readonly KafkaConfiguration _kafkaConfig;

        public VehicleAlertRefIntegrator(IVehicleAlertRepository vehicleAlertRepository, IConfiguration configuration)
        {
            _vehicleAlertRepository = vehicleAlertRepository;
            this._configuration = configuration;
            _kafkaConfig = new KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfig);
        }

        public Task GetVehicleAlertRefFromAlertConfiguration(List<int> alertIds) => ExtractAndSyncVehicleAlertRefByAlertIds(alertIds);
        public Task GetVehicleAlertRefFromAccountVehicleGroupMapping(List<int> vins, List<int> accounts) => Task.CompletedTask;
        public Task GetVehicleAlertRefFromSubscriptionManagement(List<int> subscriptionIds) => Task.CompletedTask;
        public Task GetVehicleAlertRefFromVehicleManagement(List<int> vins) => Task.CompletedTask;

        internal async Task<List<VehicleAlertRef>> ExtractAndSyncVehicleAlertRefByAlertIds(List<int> alertIds)
        {
            List<VehicleAlertRef> masterVehicleAlerts = await _vehicleAlertRepository.GetVehiclesFromAlertConfiguration(alertIds);
            await _vehicleAlertRepository.DeleteVehicleAlertRef(alertIds);
            await _vehicleAlertRepository.InsertVehicleAlertRef(masterVehicleAlerts);
            foreach (VehicleAlertRef vlr in masterVehicleAlerts)
            {

                KafkaEntity kafkaEntity = new KafkaEntity()
                {
                    BrokerList = _kafkaConfig.EH_FQDN,
                    ConnString = _kafkaConfig.EH_CONNECTION_STRING,
                    Topic = _kafkaConfig.EH_NAME,
                    Cacertlocation = _kafkaConfig.CA_CERT_LOCATION,
                    ProducerMessage = PrepareKafkaJSON(vlr).Result
                };
                await KafkaConfluentWorker.Producer(kafkaEntity);
            }
            return masterVehicleAlerts;
        }
        internal Task<string> PrepareKafkaJSON(VehicleAlertRef vehicleAlertRef)
        {
            vehicleAlertRef.State = "A";
            Payload payload = new Payload()
            {
                Data = JsonConvert.SerializeObject(vehicleAlertRef),
                Op = "I",
                Namespace = "master.vehiclealertref",
                Ts_ms = 0
            };
            VehicleAlertRefKafkaMessage vehicleAlertRefKafkaMessage = new VehicleAlertRefKafkaMessage()
            {
                Payload = payload,
                Schema = new List<object>()
            };
            return Task.FromResult(JsonConvert.SerializeObject(vehicleAlertRefKafkaMessage));
        }
    }
}
