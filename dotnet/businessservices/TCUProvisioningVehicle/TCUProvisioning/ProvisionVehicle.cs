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
using TCUSend;

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
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
                log.Info("Subscribing Topic");
                consumer.Subscribe(topic);

                while (true)
                {
                    try
                    {
                        log.Info("Consuming Messages");
                        var msg = consumer.Consume(cts.Token);
                        String TCUDataFromTopic = msg.Message.Value;
                        TCUDataReceive TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);
                        await updateVehicleDetails(TCUDataReceive, psqlconnstring);

                    }
                    catch (ConsumeException e)
                    {
                        log.Error($"Consume error: {e.Error.Reason}");

                    }
                    catch (Exception e)
                    {
                        log.Error($"Error: {e.Message}");

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
                  
                    dynamic oiedetail = await GetOEM_Id(TCUDataReceive.Vin, dataacess);
                    if (oiedetail != null)
                    {
                        receivedVehicle.Oem_id = oiedetail[0].id;
                        receivedVehicle.Oem_Organisation_id = oiedetail[0].oem_organisation_id;
                    }

                    int OrgId = await dataacess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.organization where lower(name)=@name), null)", new { name = "daf-paccar" });
                    receivedVehicle.Organization_Id = OrgId;
                    char org_status = await GetOrganisationStatusofVehicle(OrgId, dataacess);
                    receivedVehicle.Status = (VehicleCalculatedStatus)GetCalculatedVehicleStatus(org_status, receivedVehicle.Is_Ota);

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


        private  async Task<dynamic> GetOEM_Id(string vin, IDataAccess dataacess)
        {
            string vin_prefix = vin.Substring(0, 3);
            var QueryStatement = @"SELECT id, oem_organisation_id
	                             FROM master.oem
                                 where vin_prefix=@vin_prefix";

            var parameter = new DynamicParameters();
            parameter.Add("@vin_prefix", vin_prefix);

            dynamic result = await dataacess.QueryAsync<dynamic>(QueryStatement, parameter);

            return result;
        }


        public  async Task<char> GetOrganisationStatusofVehicle(int org_id, IDataAccess dataacess)
        {

            char optin = await dataacess.QuerySingleAsync<char>("SELECT vehicle_default_opt_in FROM master.organization where id=@id", new { id = org_id });

            return optin;
        }

        public  char GetCalculatedVehicleStatus(char opt_in, bool is_ota)
        {
            char calVehicleStatus = 'I';
            //Connected
            if (opt_in == (char)VehicleStatusType.OptIn && !is_ota)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.Connected;
            }
            //Off 
            if (opt_in == (char)VehicleStatusType.OptOut && !is_ota)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.Off;
            }
            //Connected + OTA
            if (opt_in == (char)VehicleStatusType.OptIn && is_ota)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.Connected_OTA;
            }
            //OTA only
            if (opt_in == (char)VehicleStatusType.OptOut && is_ota)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.OTA;
            }
            //Terminated
            if (opt_in == (char)VehicleStatusType.Terminate)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.Terminate;
            }
            return calVehicleStatus;
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
