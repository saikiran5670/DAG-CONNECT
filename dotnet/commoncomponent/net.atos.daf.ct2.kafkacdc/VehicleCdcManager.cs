using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
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
                    };
                    await KafkaConfluentWorker.Producer(kafkaEntity);
                    var test = VehicleCdcConsumer(kafkaConfiguration);
                }
            }
        }

        public async Task VehicleCdcConsumer(KafkaConfiguration kafkaConfiguration)
        {
            try
            {
                KafkaEntity kafkaEntity = new KafkaEntity()
                {
                    BrokerList = kafkaConfiguration.EH_FQDN,
                    ConnString = kafkaConfiguration.EH_CONNECTION_STRING,
                    Topic = kafkaConfiguration.EH_NAME,
                    Cacertlocation = kafkaConfiguration.CA_CERT_LOCATION,
                    Consumergroup = kafkaConfiguration.CONSUMER_GROUP

                };
                VehicleCdc vehicleCdc = new VehicleCdc();
                ConsumeResult<Null, string> message = KafkaConfluentWorker.Consumer(kafkaEntity);
                while (message != null)
                {
                    vehicleCdc = JsonConvert.DeserializeObject<VehicleCdc>(message.Message.Value);
                }


            }

            catch (Exception ex)
            {

            }
        }
    }
}
