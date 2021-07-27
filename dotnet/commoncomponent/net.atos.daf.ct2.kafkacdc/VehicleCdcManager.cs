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
            VehicleMgmtPayload payload = new VehicleMgmtPayload()
            {
                Data = JsonConvert.SerializeObject(vehicleCdc),
                Operation = operation,
                Namespace = "vehicleManagement",
                Timestamp = 1627107669236
            };
            VehicleMgmtKafkaMessage vehicleMgmtKafkaMessage = new VehicleMgmtKafkaMessage()
            {
                Payload = payload,
                Schema = "master.Vehicle"
            };
            return Task.FromResult(JsonConvert.SerializeObject(vehicleMgmtKafkaMessage));
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
                        ProducerMessage = PrepareVehicleKafkaJSON(vlr, "I").Result,
                        // Consumergroup = kafkaConfiguration.CONSUMER_GROUP
                    };
                    await KafkaConfluentWorker.Producer(kafkaEntity);
                    //var test = KafkaConfluentWorker.Consumer(kafkaEntity);
                }
            }
        }
    }
}
