using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;

namespace net.atos.daf.ct2.kafkacdc
{
    public class VehicleManagementAlertCDCManager : IVehicleManagementAlertCDCManager
    {
        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfiguration _configuration;
        private readonly KafkaConfiguration _kafkaConfig;
        private readonly IVehicleManagementAlertCDCRepository _vehicleManagementAlertCDCRepository;
        private readonly IAlertMgmAlertCdcRepository _vehicleAlertRepository;
        private readonly KafkaCdcHelper _kafkaCdcHelper;

        public VehicleManagementAlertCDCManager(IAlertMgmAlertCdcRepository vehicleAlertRepository
                                                , IVehicleManagementAlertCDCRepository vehicleManagementAlertCDCRepository
                                                , IConfiguration configuration)
        {
            this._configuration = configuration;
            _kafkaConfig = new entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfig);
            _vehicleManagementAlertCDCRepository = vehicleManagementAlertCDCRepository;
            _vehicleAlertRepository = vehicleAlertRepository;
            _kafkaCdcHelper = new KafkaCdcHelper();
        }

        public Task<bool> GetVehicleAlertRefFromVehicleId(IEnumerable<int> vehicleIds, string operation, int organizationId) => ExtractAndSyncVehicleAlertRefFromVehicleId(vehicleIds, operation, organizationId);

        internal async Task<bool> ExtractAndSyncVehicleAlertRefFromVehicleId(IEnumerable<int> vehicleIds, string operation, int organizationId)
        {
            bool result = false;
            List<VehicleAlertRef> unmodifiedMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> insertionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> deletionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> finalmapping = new List<VehicleAlertRef>();
            List<int> alertIds = new List<int>();
            try
            {
                // get all the vehicles & alert mapping under the vehicle group for given alert id
                List<VehicleAlertRef> masterDBVehicleAlerts = await _vehicleManagementAlertCDCRepository.GetVehicleAlertByvehicleId(vehicleIds, organizationId);//Context Org ID
                alertIds = masterDBVehicleAlerts.Select(x => x.AlertId).Distinct().ToList();
                List<VehicleAlertRef> datamartVehicleAlerts = await _vehicleManagementAlertCDCRepository.GetVehicleAlertRefFromvehicleId(alertIds);

                // Preparing data for sending to kafka topic
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
                //TODO Need to check on this logic
                //deletionMapping = deletionMapping.GroupBy(c => c.VIN, (key, c) => c.FirstOrDefault()).ToList();

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
                //TODO Need to check on this logic
                //insertionMapping = insertionMapping.GroupBy(c => c.VIN, (key, c) => c.FirstOrDefault()).ToList();

                //Update datamart with lastest mapping 
                //set alert operation for state column into datamart                
                masterDBVehicleAlerts.ForEach(s => s.Op = operation);
                //Update datamart table vehiclealertref based on latest modification.
                await _vehicleAlertRepository.DeleteAndInsertVehicleAlertRef(alertIds, masterDBVehicleAlerts);
                //sent message to Kafka topic 
                //Union mapping for sending to kafka topic
                finalmapping = insertionMapping.Union(deletionMapping).ToList();
                //sending only states I & D with combined mapping of vehicle and alertid
                if (finalmapping.Count() > 0)
                    foreach (var alertId in alertIds)
                    {
                        if (finalmapping.Where(w => w.AlertId == alertId).Count() > 0)
                            await _kafkaCdcHelper.ProduceMessageToKafka(finalmapping.Where(w => w.AlertId == alertId).ToList(), alertId, operation, _kafkaConfig);
                    }
                result = true;
            }
            catch (Exception ex)
            {
                var vehicleString = vehicleIds.Count() > 10 ? string.Join(",", vehicleIds.Take(10)) : string.Join(", ", vehicleIds);
                _log.Info($"Alert CDC has failed for Vehicle Id :{vehicleString} and operation " + operation);
                _log.Error(ex.ToString());
                result = false;
            }
            return result;
        }
    }
}
