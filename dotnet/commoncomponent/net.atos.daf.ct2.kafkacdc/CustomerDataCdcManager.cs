using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using net.atos.daf.ct2.visibility;
using net.atos.daf.ct2.visibility.entity;

namespace net.atos.daf.ct2.kafkacdc
{
    public class CustomerDataCdcManager : ICustomerDataCdcManager
    {
        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICustomerDataCdcRepository _vehicleAlertCustomerDataRepository;
        private readonly KafkaCdcHelper _kafkaCdcHelper;
        private readonly IConfiguration _configuration;
        private readonly entity.KafkaConfiguration _kafkaConfig;
        private readonly IAlertMgmAlertCdcRepository _vehicleAlertRepository;
        private readonly IVisibilityManager _visibilityManager;

        public CustomerDataCdcManager(ICustomerDataCdcRepository vehicleAlertCustomerDataRepository, IConfiguration configuration, IAlertMgmAlertCdcRepository vehicleAlertRepository, IVisibilityManager visibilityManager)
        {

            this._configuration = configuration;
            _kafkaConfig = new entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfig);
            _visibilityManager = visibilityManager;
            _vehicleAlertCustomerDataRepository = vehicleAlertCustomerDataRepository;
            _vehicleAlertRepository = vehicleAlertRepository;
            _kafkaCdcHelper = new KafkaCdcHelper();
        }

        public Task<bool> GetVehiclesAndAlertFromCustomerDataConfiguration(int orgId, string operation, string vin) => ExtractAndSyncVehicleAlertRefBySubscriptionId(orgId, operation, vin);
        internal async Task<bool> ExtractAndSyncVehicleAlertRefBySubscriptionId(int orgId, string operation, string vin)
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
                List<VehicleAlertRef> masterDBPackageVehicleAlerts = await GetVisibilityVehicleAlertRefByAlertIds(orgId, vin);
                //await _vehicleAlertCustomerDataRepository.GetVehiclesAndAlertFromCustomerDataConfiguration(subscriptionId);
                alertIds = masterDBPackageVehicleAlerts.Select(x => x.AlertId).Distinct().ToList();
                List<VehicleAlertRef> datamartVehicleAlerts = await _vehicleAlertCustomerDataRepository.GetVehicleAlertRefByAlertIds(alertIds);
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
                _log.Info("KeyHandover CDC has failed for vin Id :" + vin + " and operation " + operation);
                _log.Error(ex.ToString());
                result = false;
            }
            return result;
        }

        internal async Task<List<VehicleAlertRef>> GetVisibilityVehicleAlertRefByAlertIds(int org_Id, string vin)
        {
            try
            {

                IEnumerable<int> featureIds = await _vehicleAlertCustomerDataRepository.GetAlertFeatureIds(org_Id);

                IEnumerable<VehicleDetailsAccountVisibilityForAlert> visibilityVehicle = null;
                if (featureIds.Count() > 0)
                {
                    visibilityVehicle = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(0, 0, org_Id, featureIds.ToArray());
                }
                List<VehicleAlertRef> vehicleRefList = new List<VehicleAlertRef>();

                if (visibilityVehicle != null)
                {
                    int vehicleId = 0;
                    if (!string.IsNullOrEmpty(vin))
                    {
                        vehicleId = visibilityVehicle.Where(x => x.Vin == vin).Select(x => x.VehicleId).FirstOrDefault();
                    }
                    else
                    {
                        vehicleId = visibilityVehicle.Select(x => x.VehicleId).FirstOrDefault();
                    }
                    var vehgroupIds = visibilityVehicle.Where(x => x.VehicleId == vehicleId).Select(x => x.VehicleGroupIds).ToArray();

                    List<int> groupIds = new List<int>();
                    foreach (var item in vehgroupIds)
                    {
                        for (int i = 0; i < item.Length; i++)
                        {
                            var grpId = item[i];
                            groupIds.Add(grpId);
                        }
                    }

                    List<AlertGroupId> alertVehicleGroup = await _vehicleAlertCustomerDataRepository.GetAlertIdsandVGIds(groupIds, featureIds.ToList());

                    foreach (var item in alertVehicleGroup)
                    {
                        var vinDetails = visibilityVehicle.Where(x => x.VehicleGroupIds.Contains(item.GroupId) && x.VehicleId == vehicleId).Select(x => x.Vin).ToList();
                        if (vinDetails.Any())
                        {
                            foreach (var itemvin in vinDetails)
                            {
                                VehicleAlertRef vehicleRef = new VehicleAlertRef();
                                vehicleRef.AlertId = item.Alertid;
                                vehicleRef.VIN = itemvin;
                                vehicleRefList.Add(vehicleRef);
                            }

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
