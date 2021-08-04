using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;
using net.atos.daf.ct2.kafkacdc.repository;
using Microsoft.Extensions.Configuration;
using System.Linq;
using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.kafkacdc
{
    public class AlertMgmAlertCdcManager : IAlertMgmAlertCdcManager
    {
        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAlertMgmAlertCdcRepository _vehicleAlertRepository;
        private readonly KafkaCdcHelper _kafkaCdcHelper;
        private readonly IConfiguration _configuration;
        private readonly entity.KafkaConfiguration _kafkaConfig;

        //private readonly IDataAccess _dataAccess;
        //private readonly IDataMartDataAccess _datamartDataacess;

        public AlertMgmAlertCdcManager(IAlertMgmAlertCdcRepository vehicleAlertRepository, IConfiguration configuration)
        {

            this._configuration = configuration;

            //Need to handle background dataaccess and dependency injection
            //string connectionString = _configuration.GetConnectionString("ConnectionString");
            //string datamartconnectionString = _configuration.GetConnectionString("DataMartConnectionString");
            //_dataAccess = new PgSQLDataAccess(connectionString);
            //_datamartDataacess = new PgSQLDataMartDataAccess(datamartconnectionString);
            //_vehicleAlertRepository = new AlertMgmAlertCdcRepository(_dataAccess, _datamartDataacess);

            _kafkaConfig = new entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfig);

            _vehicleAlertRepository = vehicleAlertRepository;
            _kafkaCdcHelper = new KafkaCdcHelper();
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
                //send only of there is any change in vehiclealertref table from datamart 
                if (finalmapping.Count > 0)
                {
                    //sending only states I & D with combined mapping of vehicle and alertid
                    await _kafkaCdcHelper.ProduceMessageToKafka(finalmapping, alertId, operation, _kafkaConfig);
                }
                result = true;
            }
            catch (Exception ex)
            {
                _log.Info("Alert CDC has failed for Alert Id :" + alertId.ToString() + " and operation " + operation);
                _log.Error(ex.ToString());
                result = false;
            }
            return result;
        }
    }
}
