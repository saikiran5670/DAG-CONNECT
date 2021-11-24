using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using System.Linq;
using net.atos.daf.ct2.visibility;
using net.atos.daf.ct2.visibility.entity;

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
        private readonly IVisibilityManager _visibilityManager;

        public PackageAlertCdcManager(IPackageAlertCdcRepository vehicleAlertPackageRepository, IConfiguration configuration, IAlertMgmAlertCdcRepository vehicleAlertRepository, IVisibilityManager visibilityManager)
        {

            this._configuration = configuration;
            _kafkaConfig = new entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfig);
            _visibilityManager = visibilityManager;
            _vehicleAlertPackageRepository = vehicleAlertPackageRepository;
            _vehicleAlertRepository = vehicleAlertRepository;
            _kafkaCdcHelper = new KafkaCdcHelper();
        }
        public Task<bool> GetVehiclesAndAlertFromPackageConfiguration(int packageId, string operation, int orgContextId, int accountId, int loggedInOrgId, int[] featureIds) => ExtractAndSyncVehicleAlertRefByPackageIds(packageId, operation, accountId, loggedInOrgId, orgContextId, featureIds.ToArray());
        internal async Task<bool> ExtractAndSyncVehicleAlertRefByPackageIds(int packageId, string operation, int orgContextId, int accountId, int loggedInOrgId, int[] featureIds)
        {
            bool result = false;
            List<int> alertIds = new List<int>();
            List<VehicleAlertRef> unmodifiedMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> insertionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> deletionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> finalmapping = new List<VehicleAlertRef>();
            operation = "N";
            try
            {
                // get all the vehicles & alert mapping under the vehicle group for given alert id
                //List<VehicleAlertRef> masterDBPackageVehicleAlerts = await _vehicleAlertPackageRepository.GetVehiclesAndAlertFromPackageConfiguration(packageId);
                List<VehicleAlertRef> masterDBPackageVehicleAlerts = await GetVehiclesAndAlertFromPackage(packageId, orgContextId, accountId, loggedInOrgId, featureIds);
                alertIds = masterDBPackageVehicleAlerts.Select(x => x.AlertId).Distinct().ToList();
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
                deletionMapping = deletionMapping.GroupBy(c => new { c.VIN, c.AlertId }, (key, c) => c.FirstOrDefault()).ToList();

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
                insertionMapping = insertionMapping.GroupBy(c => new { c.VIN, c.AlertId }, (key, c) => c.FirstOrDefault()).ToList();

                //Update datamart with lastest mapping 
                //set alert operation for state column into datamart
                masterDBPackageVehicleAlerts.ForEach(s => s.Op = operation);
                //Update datamart table vehiclealertref based on latest modification.
                await _vehicleAlertRepository.DeleteAndInsertVehicleAlertRef(alertIds, masterDBPackageVehicleAlerts);
                //sent message to Kafka topic 
                //Union mapping for sending to kafka topic
                finalmapping = insertionMapping.Union(deletionMapping).ToList();
                //sending only states I & D with combined mapping of vehicle and alertid
                if (finalmapping.Count() > 0)
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

        internal async Task<List<VehicleAlertRef>> GetVehiclesAndAlertFromPackage(int packageId, int orgContextId, int accountId, int loggedInOrgId, int[] featureIds)
        {
            try
            {

                IEnumerable<int> featureIdss = await _vehicleAlertPackageRepository.GetAlertPackageIds(orgContextId, packageId, featureIds.ToList());
                IEnumerable<VehicleDetailsAccountVisibilityForAlert> visibilityVehicle = null;
                if (featureIdss.Count() > 0)
                {
                    visibilityVehicle = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(accountId, loggedInOrgId, orgContextId, featureIdss.ToArray());
                }
                List<int> vehicleIds = null;
                vehicleIds = visibilityVehicle.Select(x => x.VehicleId).ToList();
                var vehgroupIds = visibilityVehicle.Where(x => vehicleIds.Contains(x.VehicleId)).Select(x => x.VehicleGroupIds).ToArray();
                List<int> groupIds = new List<int>();
                foreach (var item in vehgroupIds)
                {
                    for (int i = 0; i < item.Length; i++)
                    {
                        var grpId = item[i];
                        groupIds.Add(grpId);
                    }
                }

                List<AlertGroupId> alertVehicleGroup = await _vehicleAlertPackageRepository.GetAlertIdsandVGIds(groupIds, featureIdss.ToList());
                List<VehicleAlertRef> vehicleRefList = new List<VehicleAlertRef>();

                foreach (var item in alertVehicleGroup)
                {
                    var vinDetails = visibilityVehicle.Where(x => x.VehicleGroupIds.Contains(item.GroupId) && vehicleIds.Contains(x.VehicleId)).Select(x => x.Vin).ToList();
                    if (vinDetails.Any())
                    {
                        foreach (var vin in vinDetails)
                        {
                            VehicleAlertRef vehicleRef = new VehicleAlertRef();
                            vehicleRef.AlertId = item.Alertid;
                            vehicleRef.VIN = vin;
                            vehicleRefList.Add(vehicleRef);
                        }

                    }
                }
                return vehicleRefList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
