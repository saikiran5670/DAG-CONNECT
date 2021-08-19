using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace net.atos.daf.ct2.kafkacdc
{
    public class KafkaCdcHelper
    {

        public KafkaCdcHelper()
        {
        }
        internal async Task ProduceMessageToKafka(List<VehicleAlertRef> vehicleAlertRefList, int alertId, string operation, KafkaConfiguration _kafkaConfig)
        {
            confluentkafka.entity.KafkaConfiguration kafkaEntity = new confluentkafka.entity.KafkaConfiguration()
            {
                BrokerList = _kafkaConfig.EH_FQDN,
                ConnString = _kafkaConfig.EH_CONNECTION_STRING,
                Topic = _kafkaConfig.EH_NAME,
                Cacertlocation = _kafkaConfig.CA_CERT_LOCATION,
                ProducerMessage = PrepareKafkaJSON(vehicleAlertRefList, alertId, operation).Result
            };
            //Pushing message to kafka topic
            Debug.WriteLine("/////////////////////////////////////////////////////////");
            Debug.WriteLine(kafkaEntity.ProducerMessage);
            await KafkaConfluentWorker.Producer(kafkaEntity);
        }
        internal Task<string> PrepareKafkaJSON(List<VehicleAlertRef> vehicleAlertRefList, int alertId, string operation)
        {
            VehicleAlertRefMsgFormat data = new VehicleAlertRefMsgFormat();
            data.AlertId = alertId;
            data.VinOps = vehicleAlertRefList.Count > 0 ? vehicleAlertRefList.Select(result => new VehicleStateMsgFormat() { VIN = result.VIN, Op = result.Op }).ToList() : new List<VehicleStateMsgFormat>(); ;
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            AlertCdcKafkaJsonMessage vehicleAlertRefKafkaMessage = new AlertCdcKafkaJsonMessage()
            {
                Schema = "master.vehiclealertref",
                Payload = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    ContractResolver = contractResolver
                }),
                Operation = operation,
                Namespace = "alerts",
                TimeStamp = UTCHandling.GetUTCFromDateTime(DateTime.Now)
            };
            return Task.FromResult(JsonConvert.SerializeObject(vehicleAlertRefKafkaMessage,
                new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                }));
        }
    }
}
