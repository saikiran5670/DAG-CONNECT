using Confluent.Kafka;
using Dapper;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.identity;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.identity;
using net.atos.daf.ct2.identity.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TCUReceive;
using net.atos.daf.ct2.organization.entity;

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
        string OwnerRelationship = ConfigurationManager.AppSetting["OwnerRelationship"];
        string DAFPACCAR = ConfigurationManager.AppSetting["DAFPACCAR"];

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
            VehicleManager vehicleManager = getVehicleManager(psqlConnString);
            //VehicleFilter vehicleFilter = getFilteredVehicle(TCUDataReceive);
            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            Vehicle receivedVehicle = null;
            OrganizationManager org = getOrgnisationManager(psqlConnString,vehicleManager);

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
                    Vehicle veh = await vehicleManager.Create(receivedVehicle);

                    int OwnerRelationship = Convert.ToInt32(this.OwnerRelationship);
                    int DAFPACCAR = Convert.ToInt32(this.DAFPACCAR);

                    RelationshipMapping relationship = new RelationshipMapping();
                    relationship.relationship_id = OwnerRelationship;
                    relationship.vehicle_id = veh.ID;
                    relationship.vehicle_group_id = 0;
                    relationship.owner_org_id = DAFPACCAR;
                    relationship.created_org_id = DAFPACCAR;
                    relationship.target_org_id = DAFPACCAR;
                    relationship.isFirstRelation = true;
                    relationship.allow_chain = true;
                    await org.CreateOwnerRelationship(relationship);
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



        private OrganizationManager getOrgnisationManager(string psqlConnString, VehicleManager vehicleManager)
        {
            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            IAuditLogRepository auditrepo = new AuditLogRepository(dataacess);
            IAuditTraillib audit = new AuditTraillib(auditrepo);

            GroupRepository groupRepository = new GroupRepository(dataacess);
            IGroupManager groupManager = new GroupManager(groupRepository,audit);

            SubscriptionRepository subscriptionRepository = new SubscriptionRepository(dataacess);
            ISubscriptionManager subscriptionManager = new SubscriptionManager(subscriptionRepository);


            IAccountIdentityManager accountIdentityManager;
            IConfiguration _config = null;

            
                var idenityconfiguration = new IdentityJsonConfiguration()
                {
                    Realm = "DAFConnect",
                    BaseUrl = "http://104.45.77.70:8080",
                    AuthUrl = "/auth/realms/{{realm}}/protocol/openid-connect/token",
                    UserMgmUrl = "/auth/admin/realms/{{realm}}/users",
                    AuthClientId = "admin-cli",
                    AuthClientSecret = "57149493-4055-45cc-abee-fd9f621fc34c",
                    UserMgmClientId = "DAF-Admin",
                    UserMgmClientSecret = "57149493-4055-45cc-abee-fd9f621fc34c",
                    // ReferralUrl="https://dafexternal",
                    Issuer = "Me",
                    Audience = "You",
                    // ReferralId="8c51b38a-f773-4810-8ac5-63b5fb9ca217",
                    RsaPrivateKey = "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",
                    RsaPublicKey = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAqyFYwF13lXMGZV7/nDiaQ4oPDAH8y23yV0EfSa8Oc0eqnIZd/6GrvirhejmDl5tAJHZANfLbS5Pmj4nScu3SizhoEbb4yhXgp7uJpRGADRAFs9E1v08VBHFQSCaSo4vOXxgrG5UtQjNpSjJWqBIG2kvA6kz1ZDbtK5xaZS+K2vQ64/9o9gYd3Rof/0BqrfMcg0+vq7N7+gTwiDMqcu93EiLbDbIbEQpLohJdQ7DgnxvlcGoPY47mHucR9RALlq0C31U2NDwqErNJZ6BeiSCnRW+aA0mW5zfvD1TS5S9Fdi3Bhb4lEocP/qcfqZC9YYlFu0vhbAz3JJEHIiuVG0V39Rd+De+bi/3Hwj8617+IeuB/pXSBp2C2eTez+dmDewiqFXg5Pv2k3P4FnQU0cbTCj53zIyfwon3p8UF/7wYS1BPMQe2VqhfdjzgvnhLmSd3PXA4gul6gZdSnnUOE0exZ6af1ldqrxi3X3JVqK3S+/WLEpfpCw+nE3jxq/9h+qydcIWr+p0zYwTeh3xxHyGS9dU1SdjwfL4EkDJxxTjAshXOg+4w+IHHFGDpu+nQbm8vQfZTm+NQZFkCsVnueWPthqj3sCz7DL6oh41XCYBPkoFrFXa+e8O3ByMyMs4Uv/5BtIDjXYDHCxF1kY2nR0ySVLWXRAJHgZlt8+8qMbgWSoRsCAwEAAQ=="
                };


            
            IOptions<IdentityJsonConfiguration> setting = Options.Create(idenityconfiguration);


            var connectionString = _config.GetConnectionString(psqlConnString);
            //IDataAccess _dataAccess = new PgSQLDataAccess(connectionString);
           // IAuditLogRepository _auditLogRepository = new AuditLogRepository(_dataAccess);
           // IAuditTraillib _auditlog = new AuditTraillib(_auditLogRepository);
            IAccountPreferenceRepository _repository = new AccountPreferenceRepository(dataacess);
            IPreferenceManager _preferenceManager = new PreferenceManager(_repository, audit);
            net.atos.daf.ct2.identity.ITokenManager _tokenManager = new net.atos.daf.ct2.identity.TokenManager(setting);
            net.atos.daf.ct2.identity.IAccountManager _accountManager = new net.atos.daf.ct2.identity.AccountManager(setting);
            net.atos.daf.ct2.identity.IAccountAuthenticator _autheticator = new net.atos.daf.ct2.identity.AccountAuthenticator(setting);
            IAccountRepository _repo = new AccountRepository(dataacess);
            net.atos.daf.ct2.account.IAccountManager _accManager = new net.atos.daf.ct2.account.AccountManager(_repo, audit, _accountManager, _config
                );
            accountIdentityManager = new AccountIdentityManager(_tokenManager, _autheticator, _preferenceManager, _accManager);


           // IdentityJsonConfiguration iconfig = new IdentityJsonConfiguration();
            //net.atos.daf.ct2.identity.IAccountManager identity = new net.atos.daf.ct2.identity.AccountManager(iconfig);
           // IAccountManager accountManager = new AccountManager(accountRepository,audit,identity,);

            OrganizationRepository orgRepo = new OrganizationRepository(dataacess, vehicleManager, groupManager, _accManager, subscriptionManager);
            
            
            OrganizationManager org = new OrganizationManager(orgRepo,audit);
            return org;
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
