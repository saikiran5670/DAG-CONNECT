using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.confluentkafka.entity;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace net.atos.daf.ct2.kafkacdc
{
    public class VehicleCdcManager : IVehicleCdcManager
    {
        private readonly IVehicleCdcRepository _vehicleCdcRepository;

        public VehicleCdcManager(IVehicleCdcRepository vehicleCdcRepository)
        {
            _vehicleCdcRepository = vehicleCdcRepository;
        }
        internal Task<string> PrepareVehicleKafkaJSON(VehicleCdc vehicleCdc, string operation)
        {
            //vehicleCdc.State = "A";
            VehicleMgmtPayload payload = new VehicleMgmtPayload()
            {
                Data = JsonConvert.SerializeObject(vehicleCdc),
                Operation = operation,
                Namespace = "vehicleManagement",
                Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
            };
            VehicleMgmtKafkaMessage vehicleMgmtKafkaMessage = new VehicleMgmtKafkaMessage()
            {
                Payload = payload,
                Schema = "master.Vehicle"
            };


            return Task.FromResult(JsonConvert.SerializeObject(vehicleMgmtKafkaMessage, Formatting.Indented));
        }
        public async Task VehicleCdcProducer(List<VehicleCdc> vehicleCdcList, KafkaConfiguration kafkaConfiguration)
        {
            if (vehicleCdcList.Count > 0)
            {
                foreach (VehicleCdc vlr in vehicleCdcList)
                {

                    var message = PrepareVehicleKafkaJSON(vlr, "I").Result.Replace(@"\n", string.Empty).Replace(@"\", string.Empty).Replace("\"\"", string.Empty).Replace("\"{", "{").Replace("}\"", "}");


                    KafkaEntity kafkaEntity = new KafkaEntity()
                    {
                        BrokerList = kafkaConfiguration.EH_FQDN,
                        ConnString = kafkaConfiguration.EH_CONNECTION_STRING,
                        Topic = kafkaConfiguration.EH_NAME,
                        Cacertlocation = kafkaConfiguration.CA_CERT_LOCATION,
                        ProducerMessage = message.Replace("\"", "'")
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

        public async Task<List<VehicleCdc>> GetVehicleCdc(List<int> vids)
        {
            try
            {
                return await _vehicleCdcRepository.GetVehicleCdc(vids);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
