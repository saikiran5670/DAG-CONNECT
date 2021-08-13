using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using System.Linq;
namespace net.atos.daf.ct2.kafkacdc
{
    public class PackageAlertCdcManager : IPackageAlertCdcManager
    {
        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPackageAlertCdcRepository _vehicleAlertPackageRepository;
        private readonly KafkaCdcHelper _kafkaCdcHelper;
        private readonly IConfiguration _configuration;
        private readonly entity.KafkaConfiguration _kafkaConfig;
        private readonly IAlertMgmAlertCdcRepository _vehicleAlertRepository;

        public PackageAlertCdcManager(IPackageAlertCdcRepository vehicleAlertPackageRepository, IConfiguration configuration, IAlertMgmAlertCdcRepository vehicleAlertRepository)
        {

            this._configuration = configuration;
            _kafkaConfig = new entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfig);

            _vehicleAlertPackageRepository = vehicleAlertPackageRepository;
            _vehicleAlertRepository = vehicleAlertRepository;
            _kafkaCdcHelper = new KafkaCdcHelper();
        }
        public Task<bool> GetVehiclesAndAlertFromPackageConfiguration(int packageId, string operation) => ExtractAndSyncVehicleAlertRefByPackageIds(packageId, operation);
        internal async Task<bool> ExtractAndSyncVehicleAlertRefByPackageIds(int packageId, string operation)
        {
            bool result = false;
            List<int> alertIds = new List<int>();
            List<VehicleAlertRef> unmodifiedMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> insertionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> deletionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> finalmapping = new List<VehicleAlertRef>();
            try
            {
                // get all the vehicles & alert mapping under the vehicle group for given alert id
                List<VehicleAlertRef> masterDBPackageVehicleAlerts = await _vehicleAlertPackageRepository.GetVehiclesAndAlertFromPackageConfiguration(packageId);
                alertIds = masterDBPackageVehicleAlerts.Select(x => x.AlertId).ToList();
                List<VehicleAlertRef> datamartVehicleAlerts = await _vehicleAlertPackageRepository.GetVehicleAlertRefByAlertIds(alertIds);
                // Preparing data for sending to kafka topic
                unmodifiedMapping = datamartVehicleAlerts.Where(datamart => masterDBPackageVehicleAlerts.Any(master => master.VIN == datamart.VIN && master.AlertId == datamart.AlertId)).ToList().Distinct().ToList();

                if (masterDBPackageVehicleAlerts.Count > 0)
                {
                    //Identify mapping for deletion i.e. present in datamart but not in master database 
                    deletionMapping = datamartVehicleAlerts.Where(datamart => !masterDBPackageVehicleAlerts.Any(master => master.VIN == datamart.VIN && master.AlertId == datamart.AlertId))
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
                    insertionMapping = masterDBPackageVehicleAlerts.Where(master => !datamartVehicleAlerts.Any(datamart => master.VIN == datamart.VIN && master.AlertId == datamart.AlertId))
                                                            .Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "I" })
                                                            .ToList();
                }
                else
                {
                    //all are eligible for insertion  //break the deep copy (reference of list) while coping from one list to another 
                    insertionMapping = masterDBPackageVehicleAlerts.Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "I" }).ToList();
                }
                //removing duplicate records if any 
                insertionMapping = insertionMapping.GroupBy(c => c.VIN, (key, c) => c.FirstOrDefault()).ToList();

                //Update datamart with lastest mapping 
                //set alert operation for state column into datamart
                masterDBPackageVehicleAlerts.ForEach(s => s.Op = operation);
                //Update datamart table vehiclealertref based on latest modification.
                await _vehicleAlertRepository.DeleteAndInsertVehicleAlertRef(alertIds, masterDBPackageVehicleAlerts);
                //sent message to Kafka topic 
                //Union mapping for sending to kafka topic
                finalmapping = insertionMapping.Union(deletionMapping).ToList();
                //sending only states I & D with combined mapping of vehicle and alertid
                foreach (var item in alertIds)
                {
                    await _kafkaCdcHelper.ProduceMessageToKafka(finalmapping, item, operation, _kafkaConfig);
                }
                result = true;
            }
            catch (Exception ex)
            {
                _log.Info("Package CDC has failed for Package Id :" + packageId.ToString() + " and operation " + operation);
                _log.Error(ex.ToString());
                result = false;
            }
            return result;
        }
    }

}
