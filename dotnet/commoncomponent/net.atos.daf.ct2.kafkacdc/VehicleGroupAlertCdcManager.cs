
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Newtonsoft.Json;
using net.atos.daf.ct2.visibility;

namespace net.atos.daf.ct2.kafkacdc
{
    public class VehicleGroupAlertCdcManager : IVehicleGroupAlertCdcManager
    {
        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IVehicleGroupAlertCdcRepository _vehicleGroupAlertCdcRepository;
        private readonly KafkaCdcHelper _kafkaCdcHelper;
        private readonly IConfiguration _configuration;
        private readonly entity.KafkaConfiguration _kafkaConfig;
        private readonly IAlertMgmAlertCdcRepository _vehicleAlertRepository;
        private readonly IVisibilityManager _visibilityManager;

        public VehicleGroupAlertCdcManager(IAlertMgmAlertCdcRepository vehicleAlertRepository, IVehicleGroupAlertCdcRepository vehicleGroupAlertCdcRepository, IConfiguration configuration, IVisibilityManager visibilityManager)
        {
            this._configuration = configuration;
            _kafkaConfig = new entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfigurationAlertCDC").Bind(_kafkaConfig);
            _visibilityManager = visibilityManager;
            _vehicleGroupAlertCdcRepository = vehicleGroupAlertCdcRepository;
            _vehicleAlertRepository = vehicleAlertRepository;
            _kafkaCdcHelper = new KafkaCdcHelper();
        }
        public Task<bool> GetVehicleGroupAlertConfiguration(int vehicleGroupId, string operation, int organizationId, int accountId, int loggedInOrgId, int[] featureIds) => ExtractAndSyncVehicleGroupAlertRefByVehicleGroupIds(vehicleGroupId, operation, organizationId, accountId, loggedInOrgId, featureIds);
        internal async Task<bool> ExtractAndSyncVehicleGroupAlertRefByVehicleGroupIds(int vehicleGroupId, string operation, int organizationId, int accountId, int loggedInOrgId, int[] featureIds)
        {
            bool result = false;
            List<int> alertIds = new List<int>();
            List<VehicleAlertRef> unmodifiedMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> insertionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> deletionMapping = new List<VehicleAlertRef>();
            List<VehicleAlertRef> finalmapping = new List<VehicleAlertRef>();
            try
            {
                //alertIds.Add(vehicleGroupId);
                // get all the vehicles & alert mapping under the vehicle group for given alert id
                // List<VehicleAlertRef> masterDBVehicleGroupAlerts = await _vehicleGroupAlertCdcRepository.GetVehiclesGroupFromAlertConfiguration(vehicleGroupId, organizationId);
                List<VehicleAlertRef> masterDBVehicleGroupAlerts = await GetVisibilityVehicleGroupAlertRefByGroupIds(vehicleGroupId, organizationId, accountId, loggedInOrgId, featureIds);
                alertIds = masterDBVehicleGroupAlerts.Select(x => x.AlertId).ToList();
                List<VehicleAlertRef> datamartVehicleGroupAlerts = await _vehicleGroupAlertCdcRepository.GetVehicleGroupAlertRefByAlertIds(alertIds);
                // Preparing data for sending to kafka topic
                unmodifiedMapping = datamartVehicleGroupAlerts.Where(datamart => masterDBVehicleGroupAlerts.Any(master => master.VIN == datamart.VIN && master.AlertId == datamart.AlertId)).ToList().Distinct().ToList();

                if (masterDBVehicleGroupAlerts.Count > 0)
                {
                    //Identify mapping for deletion i.e. present in datamart but not in master database 
                    deletionMapping = datamartVehicleGroupAlerts.Where(datamart => !masterDBVehicleGroupAlerts.Any(master => master.VIN == datamart.VIN && master.AlertId == datamart.AlertId))
                                                           .Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "D" })
                                                           .ToList();
                }
                else
                {
                    //all are eligible for deletion  //break the deep copy (reference of list) while coping from one list to another 
                    deletionMapping = datamartVehicleGroupAlerts.Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "D" }).ToList();
                }
                //removing duplicate records if any 
                deletionMapping = deletionMapping.GroupBy(c => c.VIN, (key, c) => c.FirstOrDefault()).ToList();

                if (datamartVehicleGroupAlerts.Count > 0)
                {
                    //Identify mapping for insertion i.e. present in master but not in datamart database 
                    insertionMapping = masterDBVehicleGroupAlerts.Where(master => !datamartVehicleGroupAlerts.Any(datamart => master.VIN == datamart.VIN && master.AlertId == datamart.AlertId))
                                                            .Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "I" })
                                                            .ToList();
                }
                else
                {
                    //all are eligible for insertion  //break the deep copy (reference of list) while coping from one list to another 
                    insertionMapping = masterDBVehicleGroupAlerts.Select(obj => new VehicleAlertRef { VIN = obj.VIN, AlertId = obj.AlertId, Op = "I" }).ToList();
                }
                //removing duplicate records if any 
                insertionMapping = insertionMapping.GroupBy(c => c.VIN, (key, c) => c.FirstOrDefault()).ToList();

                //Update datamart with lastest mapping 
                //set alert operation for state column into datamart
                masterDBVehicleGroupAlerts.ForEach(s => s.Op = operation);
                //Update datamart table vehiclealertref based on latest modification.
                await _vehicleAlertRepository.DeleteAndInsertVehicleAlertRef(alertIds, masterDBVehicleGroupAlerts);
                //sent message to Kafka topic 
                //Union mapping for sending to kafka topic
                finalmapping = insertionMapping.Union(deletionMapping).ToList();
                //sending only states I & D with combined mapping of vehicle and alertid
                if (finalmapping.Count() > 0)
                    foreach (var alertId in alertIds)
                    {
                        await _kafkaCdcHelper.ProduceMessageToKafka(finalmapping, alertId, operation, _kafkaConfig);
                    }
                result = true;
            }
            catch (Exception ex)
            {
                _log.Info("Vehicle Group CDC has failed for Vehicle Group Id :" + vehicleGroupId.ToString() + " and operation " + operation);
                _log.Error(ex.ToString());
                result = false;
            }
            return result;
        }
        internal async Task<List<VehicleAlertRef>> GetVisibilityVehicleGroupAlertRefByGroupIds(int vehicleGroupId, int organisationId, int accountId, int loggedInOrgId, int[] featureIds)
        {
            try
            {
                List<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleDetailsAccountVisibilty = new List<visibility.entity.VehicleDetailsAccountVisibilityForAlert>();

                var visibilityVehicle = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(accountId, loggedInOrgId, organisationId, featureIds.ToArray());

                var vehicleGroupdIds = visibilityVehicle.Where(x => (int)x.VehicleGroupDetails.Split(new[] { '~' }, 3).GetValue(0) == vehicleGroupId).Select(x => (int)x.VehicleGroupDetails.Split(new[] { '~' }, 3).GetValue(0)).ToList();

                List<AlertGroupId> alertVehicleGroup = await _vehicleGroupAlertCdcRepository.GetAlertIdsandVGIds(vehicleGroupdIds, featureIds.ToList());

                List<VehicleAlertRef> vehicleRefList = new List<VehicleAlertRef>();

                foreach (var item in alertVehicleGroup)
                {
                    var vinDetails = visibilityVehicle.Where(x => (int)x.VehicleGroupDetails.Split(new[] { '~' }, 3).GetValue(0) == vehicleGroupId).Select(x => x.Vin).ToList();
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

