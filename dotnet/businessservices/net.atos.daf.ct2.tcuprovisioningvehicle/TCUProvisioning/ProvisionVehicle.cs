using Confluent.Kafka;
using Dapper;
using log4net;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TCUReceive;


namespace TCUProvisioning
{
    class ProvisionVehicle
    {
        private ILog log;
        private string brokerList = ConfigurationManager.AppSetting["EH_FQDN"];
        private string connStr = ConfigurationManager.AppSetting["EH_CONNECTION_STRING"];
        private string consumergroup = ConfigurationManager.AppSetting["CONSUMER_GROUP"];
        private string topic = ConfigurationManager.AppSetting["EH_NAME"];
        private string psqlconnstring = ConfigurationManager.AppSetting["psqlconnstring"];
        private string cacertlocation = ConfigurationManager.AppSetting["CA_CERT_LOCATION"];

        public ProvisionVehicle(ILog log)
        {
            this.log = log;
        }

        public async Task readTCUProvisioningData()
        {
            ConsumerConfig config = getConsumer();

            using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
            {
               
                log.Info("Subscribing Topic");
                consumer.Subscribe(topic);

                while (true)
                {
                    try
                    {
                        log.Info("Consuming Messages");
                        var msg = consumer.Consume();
                        String TCUDataFromTopic = msg.Message.Value;
                        TCUDataReceive TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);
                        await updateVehicleDetails(TCUDataReceive, psqlconnstring);

                        log.Info("Commiting message");
                        consumer.Commit(msg);

                    }
                    catch (ConsumeException e)
                    {
                        log.Error($"Consume error: {e.Error.Reason}");
                        consumer.Close();

                    }
                    catch (Exception e)
                    {
                        log.Error($"Error: {e.Message}");
                        consumer.Close();

                    }
                }
            }
        }

        private ConsumerConfig getConsumer()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SocketTimeoutMs = 60000,
                SessionTimeoutMs = 30000,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connStr,
                SslCaLocation = cacertlocation,
                GroupId = consumergroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BrokerVersionFallback = "1.0.0",
                EnableAutoCommit = false
                //Debug = "security,broker,protocol"    //Uncomment for librdkafka debugging information
            };
            return config;
        }


        async Task updateVehicleDetails(TCUDataReceive TCUDataReceive, string psqlConnString)
        {

            log.Info("Fetching Vehicle object from database");
            IVehicleManager vehicleManager = getVehicleManager(psqlConnString);
            //VehicleFilter vehicleFilter = getFilteredVehicle(TCUDataReceive);
            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            Vehicle receivedVehicle = null;


            try

            {

                receivedVehicle = await getVehicle(TCUDataReceive, psqlConnString, vehicleManager);

                if (receivedVehicle == null)
                {

                    log.Info("Vehicle is not present in database proceeding to create vehicle");

                    receivedVehicle = new Vehicle();

                    receivedVehicle.VIN = TCUDataReceive.Vin;
                    receivedVehicle.Vid = TCUDataReceive.Correlations.VehicleId;
                    receivedVehicle.Tcu_Id = TCUDataReceive.DeviceIdentifier;
                    receivedVehicle.Tcu_Serial_Number = TCUDataReceive.DeviceSerialNumber;
                    receivedVehicle.Is_Tcu_Register = true;
                    receivedVehicle.Tcu_Brand = "Bosch";
                    receivedVehicle.Tcu_Version = "1.0";
                    receivedVehicle.Reference_Date = TCUDataReceive.ReferenceDate;


                    receivedVehicle.VehiclePropertiesId = 0;
                    receivedVehicle.Opt_In = VehicleStatusType.Inherit;
                    receivedVehicle.Is_Ota = false;

                    int OrgId = await dataacess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.organization where lower(name)=@name), null)", new { name = "daf-paccar" });
                    receivedVehicle.Organization_Id = OrgId;
                   
                    log.Info("Creating Vehicle Object in database");
                    await vehicleManager.Create(receivedVehicle);
                }
                else
                {

                    receivedVehicle.Tcu_Id = TCUDataReceive.DeviceIdentifier;
                    receivedVehicle.Tcu_Serial_Number = TCUDataReceive.DeviceSerialNumber;
                    receivedVehicle.Is_Tcu_Register = true;
                    receivedVehicle.Reference_Date = TCUDataReceive.ReferenceDate;
                    receivedVehicle.Tcu_Brand = "Bosch";
                    receivedVehicle.Tcu_Version = "1.0";

                    log.Info("Updating Vehicle details in database");
                    await vehicleManager.Update(receivedVehicle);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private  VehicleFilter getFilteredVehicle(TCUDataReceive TCUDataReceive)
        {
            VehicleFilter vehicleFilter = new VehicleFilter();

            vehicleFilter.OrganizationId = 0;
            vehicleFilter.VIN = TCUDataReceive.Vin;
            vehicleFilter.VehicleId = 0;
            vehicleFilter.VehicleGroupId = 0;
            vehicleFilter.AccountId = 0;
            vehicleFilter.FeatureId = 0;
            vehicleFilter.VehicleIdList = "";
            vehicleFilter.Status = 0;
            vehicleFilter.AccountGroupId = 0;
            return vehicleFilter;
        }

        private VehicleManager getVehicleManager(string psqlConnString)
        {
            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            IVehicleRepository vehiclerepo = new VehicleRepository(dataacess);
            IAuditLogRepository auditrepo = new AuditLogRepository(dataacess);
            IAuditTraillib audit = new AuditTraillib(auditrepo);
            VehicleManager vehicleManager = new VehicleManager(vehiclerepo, audit);
            return vehicleManager;
        }

        private async Task<Vehicle> getVehicle(TCUDataReceive TCUDataReceive, string psqlConnString, IVehicleManager vehicleManager)
        {
            try
            {
                VehicleFilter vehicleFilter = getFilteredVehicle(TCUDataReceive);
                IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
                Vehicle receivedVehicle = null;
                IEnumerable<Vehicle> vehicles = await vehicleManager.Get(vehicleFilter);


                foreach (Vehicle vehicle in vehicles)
                {
                    receivedVehicle = vehicle;
                    break;
                }

                return receivedVehicle;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }


    }
}
