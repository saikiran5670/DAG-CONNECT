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
using System.Linq;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.kafkacdc
{
    public class VehicleAlertRefManager : IVehicleAlertRefManager
    {
        private readonly IConfiguration _configuration;

        private readonly IVehicleAlertRepository _vehicleAlertRepository;
        private readonly KafkaConfiguration _kafkaConfig;

        public VehicleAlertRefManager(IVehicleAlertRepository vehicleAlertRepository, IConfiguration configuration)
        {
            _vehicleAlertRepository = vehicleAlertRepository;
            this._configuration = configuration;
            _kafkaConfig = new KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfig);
        }

        public Task<bool> GetVehicleAlertRefFromAlertConfiguration(int alertId, string operation) => ExtractAndSyncVehicleAlertRefByAlertIds(alertId, operation);
        internal async Task<bool> ExtractAndSyncVehicleAlertRefByAlertIds(int alertId, string operation)
        {
            bool result = false;
            List<int> alertIds = new List<int>();
            List<VehicleAlertRef> unmodifiedMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> insertionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> deletionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> finalmapping = new List<VehicleAlertRef>();
            try
            {
                alertIds.Add(alertId);
                // get all the vehicles & alert mapping under the vehicle group for given alert id
                List<VehicleAlertRef> masterDBVehicleAlerts = await _vehicleAlertRepository.GetVehiclesFromAlertConfiguration(alertId);
                List<VehicleAlertRef> datamartVehicleAlerts = await _vehicleAlertRepository.GetVehicleAlertRefByAlertIds(alertId);
                // Preparing data for sending to kafka topic
                unmodifiedMapping = datamartVehicleAlerts.Where(datamart => masterDBVehicleAlerts.Any(master => master.VIN == datamart.VIN && master.AlertId == datamart.AlertId)).ToList().Distinct().ToList();

                if (masterDBVehicleAlerts.Count > 0)
                {
                    //Identify mapping for deletion i.e. present in datamart but not in master database 
                    deletionMapping = datamartVehicleAlerts.Where(datamart => !masterDBVehicleAlerts.Any(master => master.VIN == datamart.VIN && master.AlertId == datamart.AlertId))
                                                           .Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "D" })
                                                           .ToList();
                }
                else
                {
                    //all are eligible for deletion  //break the deep copy (reference of list) while coping from one list to another 
                    deletionMapping = datamartVehicleAlerts.Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "D" }).ToList();
                }
                //removing duplicate records if any 
                deletionMapping = deletionMapping.GroupBy(c => c.VIN, (key, c) => c.FirstOrDefault()).ToList();

                if (datamartVehicleAlerts.Count > 0)
                {
                    //Identify mapping for insertion i.e. present in master but not in datamart database 
                    insertionMapping = masterDBVehicleAlerts.Where(master => !datamartVehicleAlerts.Any(datamart => master.VIN == datamart.VIN && master.AlertId == datamart.AlertId))
                                                            .Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "I" })
                                                            .ToList();
                }
                else
                {
                    //all are eligible for insertion  //break the deep copy (reference of list) while coping from one list to another 
                    insertionMapping = masterDBVehicleAlerts.Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "I" }).ToList();
                }
                //removing duplicate records if any 
                insertionMapping = insertionMapping.GroupBy(c => c.VIN, (key, c) => c.FirstOrDefault()).ToList();

                //Update datamart with lastest mapping 
                //set alert operation for state column into datamart
                masterDBVehicleAlerts.ForEach(s => s.Op = operation);
                //Update datamart table vehiclealertref based on latest modification.
                await _vehicleAlertRepository.DeleteAndInsertVehicleAlertRef(alertIds, masterDBVehicleAlerts);
                //sent message to Kafka topic 
                //Union mapping for sending to kafka topic
                finalmapping = insertionMapping.Union(deletionMapping).ToList();
                //sending only states I & D with combined mapping of vehicle and alertid
                await ProduceMessageToKafka(finalmapping, alertId, operation);
                result = true;
            }
            catch (Exception Ex)
            {
                result = false;
                throw Ex;
            }
            return result;
        }
        internal async Task ProduceMessageToKafka(List<VehicleAlertRef> vehicleAlertRefList, int alertId, string operation)
        {
            KafkaEntity kafkaEntity = new KafkaEntity()
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
            VehicleAlertRefKafkaMessage vehicleAlertRefKafkaMessage = new VehicleAlertRefKafkaMessage()
            {
                Payload = payload,
                Schema = "master.vehiclealertref"
            };
            return Task.FromResult(JsonConvert.SerializeObject(vehicleAlertRefKafkaMessage));
        }
    }
}
