using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.confluentkafka.entity;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.kafkacdc
{
    public class VehicleCdcManager : IVehicleCdcManager
    {

        internal Task<string> PrepareVehicleKafkaJSON(VehicleCdc vehicleCdc, string operation)
        {
            //vehicleCdc.State = "A";
            Payload payload = new Payload()
            {
                Data = JsonConvert.SerializeObject(vehicleCdc),
                Op = operation,
                Namespace = "master.vehicle",
                Ts_ms = 0
            };
            VehicleAlertRefKafkaMessage vehicleAlertRefKafkaMessage = new VehicleAlertRefKafkaMessage()
            {
                Payload = payload,
                Schema = new List<object>()
            };
            return Task.FromResult(JsonConvert.SerializeObject(vehicleAlertRefKafkaMessage));
        }
        public async Task VehicleCdcProducer(List<VehicleCdc> vehicleCdcList, KafkaConfiguration kafkaConfiguration)
        {
            if (vehicleCdcList.Count > 0)
            {
                foreach (VehicleCdc vlr in vehicleCdcList)
                {
                    KafkaEntity kafkaEntity = new KafkaEntity()
                    {
                        BrokerList = kafkaConfiguration.EH_FQDN,
                        ConnString = kafkaConfiguration.EH_CONNECTION_STRING,
                        Topic = kafkaConfiguration.EH_NAME,
                        Cacertlocation = kafkaConfiguration.CA_CERT_LOCATION,
                        ProducerMessage = PrepareVehicleKafkaJSON(vlr, "I").Result
                    };
                    await KafkaConfluentWorker.Producer(kafkaEntity);
                }
            }
        }
    }
}
