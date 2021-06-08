using System;
using System.Linq;
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
using net.atos.daf.ct2.tcucore;
using net.atos.daf.ct2.translation;
using net.atos.daf.ct2.translation.repository;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.tcuvehiclebusinessservice
{
    public class ProvisionVehicle : ITcuProvisioningDataReceive
    {
        private string brokerList;
        private string connStr;
        private string consumergroup;
        private string topic;
        private string psqlconnstring;
        private string datamartpsqlconnstring;
        private string cacertlocation;
        private string orgName;
        private string boschTcuBrand;
        private string boschTcuVesrion;
        IDataAccess dataacess = null;
        IDataMartDataAccess datamartDataacess = null;
        private ILog log = null;
        IConfiguration config = null;
        IAuditTraillib auditlog = null;
        IAuditLogRepository auditrepo = null;
        IVehicleRepository vehiclerepo = null;

        public ProvisionVehicle(ILog _log, IConfiguration _config)
        {
            log = _log;
            config = _config;
            brokerList = config.GetSection("EH_FQDN").Value;
            connStr = config.GetSection("EH_CONNECTION_STRING").Value;
            consumergroup = config.GetSection("CONSUMER_GROUP").Value;
            topic = config.GetSection("EH_NAME").Value;
            psqlconnstring = config.GetSection("PSQL_CONNSTRING").Value;
            datamartpsqlconnstring = config.GetSection("DATAMART_CONNECTION_STRING").Value;
            cacertlocation = config.GetSection("CA_CERT_LOCATION").Value;
            orgName = config.GetSection("DEFAULT_ORG").Value;
            boschTcuBrand = config.GetSection("BOSCH_TCU_BRAND").Value;
            boschTcuVesrion = config.GetSection("BOSCH_TCU_VERSION").Value;

            dataacess = new PgSQLDataAccess(psqlconnstring);
            datamartDataacess = new PgSQLDataMartDataAccess(datamartpsqlconnstring);
            auditrepo = new AuditLogRepository(dataacess);
            auditlog = new AuditTraillib(auditrepo);
            vehiclerepo = new VehicleRepository(dataacess, datamartDataacess);
        }

        public async Task ReadTcuProvisioningData()
        {
            KafkaConfig kafkaConfig = new KafkaConfig();
            ConsumerConfig consumerConfig = kafkaConfig.GetConsumerConfig(brokerList, connStr, cacertlocation, consumergroup);

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
                        TCUDataReceive tcuDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(msg.Message.Value);
                        await UpdateVehicleDetails(tcuDataReceive);

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

        private async Task UpdateVehicleDetails(TCUDataReceive tcuDataReceive)
        {
            try
            {
                log.Info("Fetching Vehicle object from database");

                VehicleManager vehicleManager = new VehicleManager(vehiclerepo, auditlog);

                var receivedVehicle = await GetVehicle(tcuDataReceive, vehicleManager);

                if (receivedVehicle == null)
                {
                    receivedVehicle = await CreateVehicle(tcuDataReceive, vehicleManager);
                    await CreateOrgRelationship(vehicleManager, receivedVehicle.ID, (int)receivedVehicle.Organization_Id);
                }
                else
                {
                    receivedVehicle = await UpdateVehicle(receivedVehicle, tcuDataReceive, vehicleManager);
                    await CreateOrgRelationship(vehicleManager, receivedVehicle.ID, (int)receivedVehicle.Organization_Id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private VehicleFilter GetFilteredVehicle(TCUDataReceive tcuDataReceive)
        {
            VehicleFilter vehicleFilter = new VehicleFilter
            {
                OrganizationId = 0,
                VIN = tcuDataReceive.Vin,
                VehicleId = 0,
                VehicleGroupId = 0,
                AccountId = 0,
                FeatureId = 0,
                VehicleIdList = "",
                Status = VehicleStatusType.None,
                AccountGroupId = 0,
            };
            return vehicleFilter;
        }

        private async Task<Vehicle> GetVehicle(TCUDataReceive tcuDataReceive, IVehicleManager vehicleManager)
        {
            try
            {
                VehicleFilter vehicleFilter = GetFilteredVehicle(tcuDataReceive);
                Vehicle receivedVehicle = null;
                var vehiclesList = await vehicleManager.Get(vehicleFilter);
                if (vehiclesList.Count() > 0)
                    receivedVehicle = vehiclesList.Where(x => x is Vehicle).First();

                return receivedVehicle;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private async Task<Vehicle> CreateVehicle(TCUDataReceive tcuDataReceive, VehicleManager vehicleManager)
        {
            Vehicle veh = null;
            int OrgId = 0;

            try
            {
                log.Info("Vehicle is not present in database proceeding to create vehicle");

                OrgId = await dataacess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.organization where lower(name)=@name), null)", new { name = orgName });

                var vehicle = new Vehicle
                {
                    VIN = tcuDataReceive.Vin,
                    Vid = tcuDataReceive.Correlations.VehicleId,
                    Tcu_Id = tcuDataReceive.DeviceIdentifier,
                    Tcu_Serial_Number = tcuDataReceive.DeviceSerialNumber,
                    Is_Tcu_Register = true,
                    Tcu_Brand = boschTcuBrand,
                    Tcu_Version = boschTcuVesrion,
                    Reference_Date = tcuDataReceive.ReferenceDate,
                    VehiclePropertiesId = 0,
                    Opt_In = VehicleStatusType.Inherit,
                    Is_Ota = false,
                    Organization_Id = OrgId
                };

                log.Info("Creating Vehicle Object in database");
                veh = await vehicleManager.Create(vehicle);

                await auditlog.AddLogs(DateTime.Now, DateTime.Now, OrgId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create method in TCU Vehicle Component_Sucess", 0, veh.ID, JsonConvert.SerializeObject(veh));

            }
            catch (Exception)
            {
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, OrgId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Create vehicle in TCU Vehicle Component_Failed", 0, 0, JsonConvert.SerializeObject(veh));
                throw;
            }

            return veh;
        }

        private async Task<Vehicle> UpdateVehicle(Vehicle receivedVehicle, TCUDataReceive tcuDataReceive, VehicleManager vehicleManager)
        {
            log.Info("Vehicle is  present in database proceeding to update vehicle");

            Vehicle veh = null;
            try
            {
                receivedVehicle.Tcu_Id = tcuDataReceive.DeviceIdentifier;
                receivedVehicle.Tcu_Serial_Number = tcuDataReceive.DeviceSerialNumber;
                receivedVehicle.Is_Tcu_Register = true;
                receivedVehicle.Reference_Date = tcuDataReceive.ReferenceDate;
                receivedVehicle.Tcu_Brand = boschTcuBrand;
                receivedVehicle.Tcu_Version = boschTcuVesrion;

                log.Info("Updating Vehicle details in database");
                veh = await vehicleManager.Update(receivedVehicle);
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, (int)veh.Organization_Id, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "update vehicle in TCU Vehicle Component_Sucess", 0, veh.ID, JsonConvert.SerializeObject(veh));

            }
            catch (Exception)
            {
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, (int)veh.Organization_Id, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "update vehicle in TCU Vehicle Component_Failed", 0, 0, JsonConvert.SerializeObject(veh));
                throw;
            }
            return veh;
        }

        private async Task CreateOrgRelationship(VehicleManager vehicleManager, int vehicleId, int organizationId)
        {
            RelationshipMapping relationship = null;
            OrganizationManager org = GetOrgnisationManager(vehicleManager);

            try
            {
                int IsVehicleIdExist = await org.IsOwnerRelationshipExist(vehicleId);

                if (IsVehicleIdExist <= 0)
                {
                    log.Info("Organisation relationship is not present in database proceeding to create relationship");

                    int OwnerRelationship = Convert.ToInt32(this.config.GetSection("DefaultSettings").GetSection("OwnerRelationship").Value);
                    int DAFPACCAR = Convert.ToInt32(this.config.GetSection("DefaultSettings").GetSection("DAFPACCAR").Value);

                    relationship = new RelationshipMapping
                    {
                        RelationshipId = OwnerRelationship,
                        VehicleId = vehicleId,
                        VehicleGroupId = 0,
                        OwnerOrgId = DAFPACCAR,
                        CreatedOrgId = DAFPACCAR,
                        TargetOrgId = DAFPACCAR,
                        IsFirstRelation = true,
                        AllowChain = true
                    };

                    await org.CreateOwnerRelationship(relationship);
                    await auditlog.AddLogs(DateTime.Now, DateTime.Now, organizationId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create org relationship in TCU Vehicle Component_Sucess", 0, vehicleId, JsonConvert.SerializeObject(relationship));
                }
            }
            catch (Exception)
            {
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, organizationId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Create org relationship in TCU Vehicle Component_Failed", 0, vehicleId, JsonConvert.SerializeObject(relationship));
                throw;
            }
        }

        private OrganizationManager GetOrgnisationManager(VehicleManager vehicleManager)
        {
            GroupRepository groupRepository = new GroupRepository(dataacess);
            IGroupManager groupManager = new GroupManager(groupRepository, auditlog);

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
            net.atos.daf.ct2.account.IAccountManager accManager = new net.atos.daf.ct2.account.AccountManager(accountrepo, auditlog, iaccountManager, config, translationManager);

            OrganizationRepository orgRepo = new OrganizationRepository(dataacess, vehicleManager, groupManager, accManager, subscriptionManager, accountSessionManager, accountTokenManager);
            OrganizationManager org = new OrganizationManager(orgRepo, auditlog);

            return org;
        }

    }
}
