using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;

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
            await KafkaConfluentWorker.Producer(kafkaEntity);
        }
        internal Task<string> PrepareKafkaJSON(List<VehicleAlertRef> vehicleAlertRefList, int alertId, string operation)
        {
            VehicleAlertRefMsgFormat data = new VehicleAlertRefMsgFormat();
            data.AlertId = alertId;
            data.VinOps = new List<VehicleStateMsgFormat>();
            data.VinOps = vehicleAlertRefList.Select(result => new VehicleStateMsgFormat() { VIN = result.VIN, Op = result.Op }).ToList();

            Payload payload = new Payload()
            {
                Data = JsonConvert.SerializeObject(data),
                Operation = operation,
                Namespace = "alerts",
                Ts_ms = UTCHandling.GetUTCFromDateTime(DateTime.Now)
            };
            AlertCdcKafkaJsonMessage vehicleAlertRefKafkaMessage = new AlertCdcKafkaJsonMessage()
            {
                Payload = payload,
                Schema = "master.vehiclealertref"
            };
            return Task.FromResult(JsonConvert.SerializeObject(vehicleAlertRefKafkaMessage));
        }
    }
}
