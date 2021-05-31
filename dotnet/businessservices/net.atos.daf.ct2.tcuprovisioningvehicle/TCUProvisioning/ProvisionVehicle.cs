using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.identity;
using net.atos.daf.ct2.identitysession;
using net.atos.daf.ct2.identitysession.repository;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.repository;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using Newtonsoft.Json;
using TCUReceive;

namespace TCUProvisioning
{
    class ProvisionVehicle
    {
        private ILog log;
        private string brokerList;
        private string connStr;
        private string consumergroup;
        private string topic;
        private string psqlconnstring;
        private string datamartpsqlconnstring;
        private string cacertlocation;
        IConfiguration config = null;
        IAuditTraillib _auditlog;

        public ProvisionVehicle(ILog log, IConfiguration config, IAuditTraillib auditlog)
        {
            this.log = log;
            this.config = config;
            _auditlog = auditlog;
            brokerList = config.GetSection("EH_FQDN").Value;
            connStr = config.GetSection("EH_CONNECTION_STRING").Value;
            consumergroup = config.GetSection("CONSUMER_GROUP").Value;
            topic = config.GetSection("EH_NAME").Value;
            psqlconnstring = config.GetSection("psqlconnstring").Value;
            cacertlocation = config.GetSection("CA_CERT_LOCATION").Value;
            datamartpsqlconnstring = config.GetSection("psqlconnstring").Value;
        }

        public async Task ReadTCUProvisioningData()
        {
            ConsumerConfig consumerConfig = GetConsumer();

            using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
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
                        await UpdateVehicleDetails(TCUDataReceive, psqlconnstring);

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

        private ConsumerConfig GetConsumer()
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

        async Task UpdateVehicleDetails(TCUDataReceive TCUDataReceive, string psqlConnString)
        {
            try
            {
                log.Info("Fetching Vehicle object from database");

                IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
                VehicleManager vehicleManager = GetVehicleManager(psqlConnString);

                Vehicle receivedVehicle = null;
                receivedVehicle = await GetVehicle(TCUDataReceive, psqlConnString, vehicleManager);

                if (receivedVehicle == null)
                {
                    receivedVehicle = await CreateVehicle(receivedVehicle, TCUDataReceive, dataacess, psqlConnString, vehicleManager);
                    await CreateOrgRelationship(vehicleManager, psqlConnString, receivedVehicle.ID, (int)receivedVehicle.Organization_Id);
                }
                else
                {
                    receivedVehicle = await UpdateVehicle(receivedVehicle, TCUDataReceive, vehicleManager);
                    await CreateOrgRelationship(vehicleManager, psqlConnString, receivedVehicle.ID, (int)receivedVehicle.Organization_Id);
                }
            }
            catch (Exception)
            {

                throw;

            }

        }

        private async Task<Vehicle> CreateVehicle(Vehicle receivedVehicle, TCUDataReceive TCUDataReceive, IDataAccess dataacess, string psqlConnString, VehicleManager vehicleManager)
        {
            int OrgId = 0;
            Vehicle veh;
            try
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

                OrgId = await dataacess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.organization where lower(name)=@name), null)", new { name = "daf-paccar" });
                receivedVehicle.Organization_Id = OrgId;

                log.Info("Creating Vehicle Object in database");
                veh = await vehicleManager.Create(receivedVehicle);

                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, OrgId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create method in TCU Vehicle Component", 0, veh.ID, JsonConvert.SerializeObject(receivedVehicle));

            }
            catch (Exception)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, OrgId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Create vehicle in TCU Vehicle Component", 0, 0, JsonConvert.SerializeObject(receivedVehicle));
                throw;
            }

            return veh;
        }

        private async Task CreateOrgRelationship(VehicleManager vehicleManager, string psqlConnString, int vehId, int OrgId)
        {
            RelationshipMapping relationship = null;
            OrganizationManager org = GetOrgnisationManager(psqlConnString, vehicleManager);

            try
            {
                int IsVehicleIdExist = await org.IsOwnerRelationshipExist(vehId);

                if (IsVehicleIdExist <= 0)
                {
                    log.Info("Organisation relationship is not present in database proceeding to create relationship");

                    int OwnerRelationship = Convert.ToInt32(this.config.GetSection("DefaultSettings").GetSection("OwnerRelationship").Value);
                    int DAFPACCAR = Convert.ToInt32(this.config.GetSection("DefaultSettings").GetSection("DAFPACCAR").Value);

                    relationship = new RelationshipMapping();
                    relationship.relationship_id = OwnerRelationship;
                    relationship.vehicle_id = vehId;
                    relationship.vehicle_group_id = 0;
                    relationship.owner_org_id = DAFPACCAR;
                    relationship.created_org_id = DAFPACCAR;
                    relationship.target_org_id = DAFPACCAR;
                    relationship.isFirstRelation = true;
                    relationship.allow_chain = true;

                    await org.CreateOwnerRelationship(relationship);
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, OrgId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create org relationship in TCU Vehicle Component", 0, vehId, JsonConvert.SerializeObject(relationship));
                }
            }
            catch (Exception)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, OrgId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Create org relationship in TCU Vehicle Component", 0, vehId, JsonConvert.SerializeObject(relationship));
                throw;
            }
        }

        private async Task<Vehicle> UpdateVehicle(Vehicle receivedVehicle, TCUDataReceive TCUDataReceive, VehicleManager vehicleManager)
        {
            log.Info("Vehicle is  present in database proceeding to update vehicle");

            Vehicle veh = null;
            try
            {
                receivedVehicle.Tcu_Id = TCUDataReceive.DeviceIdentifier;
                receivedVehicle.Tcu_Serial_Number = TCUDataReceive.DeviceSerialNumber;
                receivedVehicle.Is_Tcu_Register = true;
                receivedVehicle.Reference_Date = TCUDataReceive.ReferenceDate;
                receivedVehicle.Tcu_Brand = "Bosch";
                receivedVehicle.Tcu_Version = "1.0";

                log.Info("Updating Vehicle details in database");
                veh = await vehicleManager.Update(receivedVehicle);
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, (int)veh.Organization_Id, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "update vehicle in TCU Vehicle Component", 0, veh.ID, JsonConvert.SerializeObject(receivedVehicle));

            }
            catch (Exception)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, (int)veh.Organization_Id, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "update vehicle in TCU Vehicle Component", 0, 0, JsonConvert.SerializeObject(receivedVehicle));
                throw;
            }
            return veh;
        }

        private VehicleFilter GetFilteredVehicle(TCUDataReceive TCUDataReceive)
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

        private OrganizationManager GetOrgnisationManager(string psqlConnString, VehicleManager vehicleManager)
        {
            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            IAuditLogRepository auditrepo = new AuditLogRepository(dataacess);
            IAuditTraillib audit = new AuditTraillib(auditrepo);

            GroupRepository groupRepository = new GroupRepository(dataacess);
            IGroupManager groupManager = new GroupManager(groupRepository, audit);

            SubscriptionRepository subscriptionRepository = new SubscriptionRepository(dataacess);
            ISubscriptionManager subscriptionManager = new SubscriptionManager(subscriptionRepository);

            AccountSessionRepository sessionRepository = new AccountSessionRepository(dataacess);
            IAccountSessionManager accountSessionManager = new AccountSessionManager(sessionRepository);

            AccountTokenRepository tokenRepository = new AccountTokenRepository(dataacess);
            IAccountTokenManager accountTokenManager = new AccountTokenManager(tokenRepository);

            var idenityconfiguration = new IdentityJsonConfiguration()
            {
                Realm = this.config.GetSection("IdentityConfiguration").GetSection("realm").Value,
                BaseUrl = this.config.GetSection("IdentityConfiguration").GetSection("baseUrl").Value,
                AuthUrl = this.config.GetSection("IdentityConfiguration").GetSection("authUrl").Value,
                UserMgmUrl = this.config.GetSection("IdentityConfiguration").GetSection("userMgmUrl").Value,
                AuthClientId = this.config.GetSection("IdentityConfiguration").GetSection("AuthClientId").Value,
                AuthClientSecret = this.config.GetSection("IdentityConfiguration").GetSection("AuthClientSecret").Value,
                UserMgmClientId = this.config.GetSection("IdentityConfiguration").GetSection("UserMgmClientId").Value,
                UserMgmClientSecret = this.config.GetSection("IdentityConfiguration").GetSection("UserMgmClientSecret").Value,
                // ReferralUrl="https://dafexternal",
                Issuer = this.config.GetSection("IdentityConfiguration").GetSection("Issuer").Value,
                Audience = this.config.GetSection("IdentityConfiguration").GetSection("Audience").Value,
                // ReferralId="8c51b38a-f773-4810-8ac5-63b5fb9ca217",
                RsaPrivateKey = this.config.GetSection("IdentityConfiguration").GetSection("RsaPrivateKey").Value,
                RsaPublicKey = this.config.GetSection("IdentityConfiguration").GetSection("RsaPublicKey").Value
            };

            IOptions<IdentityJsonConfiguration> setting = Options.Create(idenityconfiguration);
            net.atos.daf.ct2.identity.IAccountManager iaccountManager = new net.atos.daf.ct2.identity.AccountManager(setting);

            TranslationRepository translationRepository = new TranslationRepository(dataacess);
            ITranslationManager translationManager = new TranslationManager(translationRepository);

            IAccountRepository accountrepo = new AccountRepository(dataacess);
            net.atos.daf.ct2.account.IAccountManager accManager = new net.atos.daf.ct2.account.AccountManager(accountrepo, audit, iaccountManager, config, translationManager);

            OrganizationRepository orgRepo = new OrganizationRepository(dataacess, vehicleManager, groupManager, accManager, subscriptionManager, accountSessionManager, accountTokenManager);
            OrganizationManager org = new OrganizationManager(orgRepo, audit);
            return org;
        }

        private VehicleManager GetVehicleManager(string psqlConnString)
        {
            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            IDataMartDataAccess datamartDataacess = new PgSQLDataMartDataAccess(datamartpsqlconnstring);
            IVehicleRepository vehiclerepo = new VehicleRepository(dataacess, datamartDataacess);
            IAuditLogRepository auditrepo = new AuditLogRepository(dataacess);
            IAuditTraillib audit = new AuditTraillib(auditrepo);
            VehicleManager vehicleManager = new VehicleManager(vehiclerepo, audit);
            return vehicleManager;
        }

        private async Task<Vehicle> GetVehicle(TCUDataReceive TCUDataReceive, string psqlConnString, IVehicleManager vehicleManager)
        {
            try
            {
                VehicleFilter vehicleFilter = GetFilteredVehicle(TCUDataReceive);
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
            catch (Exception)
            {
                throw;
            }

        }

    }
}
