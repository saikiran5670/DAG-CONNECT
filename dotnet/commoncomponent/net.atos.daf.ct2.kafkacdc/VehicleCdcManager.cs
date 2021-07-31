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
using Newtonsoft.Json.Serialization;

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
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            VehicleMgmtKafkaMessage vehicleMgmtKafkaMessage = new VehicleMgmtKafkaMessage()
            {
                Payload = JsonConvert.SerializeObject(vehicleCdc, new JsonSerializerSettings
                {
                    ContractResolver = contractResolver
                }),
                Schema = "master.Vehicle",
                Operation = operation,
                Namespace = "vehicleManagement",
                Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
            };


            return Task.FromResult(JsonConvert.SerializeObject(vehicleMgmtKafkaMessage, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            }));
        }
        public async Task VehicleCdcProducer(List<int> vehicleIds, KafkaConfiguration kafkaConfiguration)
        {
            if (vehicleIds.Count > 0)
            {
                List<VehicleCdc> vehicleCdcList = await GetVehicleCdc(vehicleIds);
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
