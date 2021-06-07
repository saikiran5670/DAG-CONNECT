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
        private readonly string _brokerList;
        private readonly string _connStr;
        private readonly string _consumerGroup;
        private readonly string _topic;
        private readonly string _psqlconnstring;
        private readonly string _datamartpsqlconnstring;
        private readonly string _cacertlocation;
        private readonly string _orgName;
        private readonly string _boschTcuBrand;
        private readonly string _boschTcuVesrion;
        private readonly IDataAccess _dataacess = null;
        private readonly IDataMartDataAccess _datamartDataacess = null;
        private readonly ILog _log = null;
        private readonly IConfiguration _config = null;
        private readonly IAuditTraillib _auditlog = null;
        private readonly IAuditLogRepository _auditrepo = null;
        private readonly IVehicleRepository _vehiclerepo = null;

        public ProvisionVehicle(ILog _log, IConfiguration _config)
        {
            this._log = _log;
            this._config = _config;
            _brokerList = this._config.GetSection("EH_FQDN").Value;
            _connStr = this._config.GetSection("EH_CONNECTION_STRING").Value;
            _consumerGroup = this._config.GetSection("CONSUMER_GROUP").Value;
            _topic = this._config.GetSection("EH_NAME").Value;
            _psqlconnstring = this._config.GetSection("PSQL_CONNSTRING").Value;
            _datamartpsqlconnstring = this._config.GetSection("DATAMART_CONNECTION_STRING").Value;
            _cacertlocation = this._config.GetSection("CA_CERT_LOCATION").Value;
            _orgName = this._config.GetSection("DEFAULT_ORG").Value;
            _boschTcuBrand = this._config.GetSection("BOSCH_TCU_BRAND").Value;
            _boschTcuVesrion = this._config.GetSection("BOSCH_TCU_VERSION").Value;

            _dataacess = new PgSQLDataAccess(_psqlconnstring);
            _datamartDataacess = new PgSQLDataMartDataAccess(_datamartpsqlconnstring);
            _auditrepo = new AuditLogRepository(_dataacess);
            _auditlog = new AuditTraillib(_auditrepo);
            _vehiclerepo = new VehicleRepository(_dataacess, _datamartDataacess);
        }

        public async Task ReadTcuProvisioningData()
        {
            KafkaConfig kafkaConfig = new KafkaConfig();
            ConsumerConfig consumerConfig = kafkaConfig.GetConsumerConfig(_brokerList, _connStr, _cacertlocation, _consumerGroup);

            using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
            {
                _log.Info("Subscribing Topic");
                consumer.Subscribe(_topic);

                while (true)
                {
                    try
                    {
                        _log.Info("Consuming Messages");
                        var msg = consumer.Consume();
                        TCUDataReceive tcuDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(msg.Message.Value);
                        await UpdateVehicleDetails(tcuDataReceive);

                        _log.Info("Commiting message");
                        consumer.Commit(msg);

                    }
                    catch (ConsumeException e)
                    {
                        _log.Error($"Consume error: {e.Error.Reason}");
                        consumer.Close();
                    }
                    catch (Exception e)
                    {
                        _log.Error($"Error: {e.Message}");
                        consumer.Close();
                    }
                }
            }
        }

        private async Task UpdateVehicleDetails(TCUDataReceive tcuDataReceive)
        {
            try
            {
                _log.Info("Fetching Vehicle object from database");

                VehicleManager vehicleManager = new VehicleManager(_vehiclerepo, _auditlog);

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
                _log.Info("Vehicle is not present in database proceeding to create vehicle");

                OrgId = await _dataacess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.organization where lower(name)=@name), null)", new { name = _orgName });

                var vehicle = new Vehicle
                {
                    VIN = tcuDataReceive.Vin,
                    Vid = tcuDataReceive.Correlations.VehicleId,
                    Tcu_Id = tcuDataReceive.DeviceIdentifier,
                    Tcu_Serial_Number = tcuDataReceive.DeviceSerialNumber,
                    Is_Tcu_Register = true,
                    Tcu_Brand = _boschTcuBrand,
                    Tcu_Version = _boschTcuVesrion,
                    Reference_Date = tcuDataReceive.ReferenceDate,
                    VehiclePropertiesId = 0,
                    Opt_In = VehicleStatusType.Inherit,
                    Is_Ota = false,
                    Organization_Id = OrgId
                };

                _log.Info("Creating Vehicle Object in database");
                veh = await vehicleManager.Create(vehicle);

                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, OrgId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create method in TCU Vehicle Component_Sucess", 0, veh.ID, JsonConvert.SerializeObject(veh));

            }
            catch (Exception)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, OrgId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Create vehicle in TCU Vehicle Component_Failed", 0, 0, JsonConvert.SerializeObject(veh));
                throw;
            }

            return veh;
        }

        private async Task<Vehicle> UpdateVehicle(Vehicle receivedVehicle, TCUDataReceive tcuDataReceive, VehicleManager vehicleManager)
        {
            _log.Info("Vehicle is  present in database proceeding to update vehicle");

            Vehicle veh = null;
            try
            {
                receivedVehicle.Tcu_Id = tcuDataReceive.DeviceIdentifier;
                receivedVehicle.Tcu_Serial_Number = tcuDataReceive.DeviceSerialNumber;
                receivedVehicle.Is_Tcu_Register = true;
                receivedVehicle.Reference_Date = tcuDataReceive.ReferenceDate;
                receivedVehicle.Tcu_Brand = _boschTcuBrand;
                receivedVehicle.Tcu_Version = _boschTcuVesrion;

                _log.Info("Updating Vehicle details in database");
                veh = await vehicleManager.Update(receivedVehicle);
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, (int)veh.Organization_Id, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "update vehicle in TCU Vehicle Component_Sucess", 0, veh.ID, JsonConvert.SerializeObject(veh));

            }
            catch (Exception)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, (int)veh.Organization_Id, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "update vehicle in TCU Vehicle Component_Failed", 0, 0, JsonConvert.SerializeObject(veh));
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
                    _log.Info("Organisation relationship is not present in database proceeding to create relationship");

                    int OwnerRelationship = Convert.ToInt32(this._config.GetSection("DefaultSettings").GetSection("OwnerRelationship").Value);
                    int DAFPACCAR = Convert.ToInt32(this._config.GetSection("DefaultSettings").GetSection("DAFPACCAR").Value);

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
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, organizationId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create org relationship in TCU Vehicle Component_Sucess", 0, vehicleId, JsonConvert.SerializeObject(relationship));
                }
            }
            catch (Exception)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, organizationId, "TCU Vehicle Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Create org relationship in TCU Vehicle Component_Failed", 0, vehicleId, JsonConvert.SerializeObject(relationship));
                throw;
            }
        }

        private OrganizationManager GetOrgnisationManager(VehicleManager vehicleManager)
        {
            GroupRepository groupRepository = new GroupRepository(_dataacess);
            IGroupManager groupManager = new GroupManager(groupRepository, _auditlog);

            SubscriptionRepository subscriptionRepository = new SubscriptionRepository(_dataacess);
            ISubscriptionManager subscriptionManager = new SubscriptionManager(subscriptionRepository);

            AccountSessionRepository sessionRepository = new AccountSessionRepository(_dataacess);
            IAccountSessionManager accountSessionManager = new AccountSessionManager(sessionRepository);

            AccountTokenRepository tokenRepository = new AccountTokenRepository(_dataacess);
            IAccountTokenManager accountTokenManager = new AccountTokenManager(tokenRepository);

            var idenityconfiguration = new IdentityJsonConfiguration()
            {
                Realm = this._config.GetSection("IdentityConfiguration").GetSection("realm").Value,
                BaseUrl = this._config.GetSection("IdentityConfiguration").GetSection("baseUrl").Value,
                AuthUrl = this._config.GetSection("IdentityConfiguration").GetSection("authUrl").Value,
                UserMgmUrl = this._config.GetSection("IdentityConfiguration").GetSection("userMgmUrl").Value,
                AuthClientId = this._config.GetSection("IdentityConfiguration").GetSection("AuthClientId").Value,
                AuthClientSecret = this._config.GetSection("IdentityConfiguration").GetSection("AuthClientSecret").Value,
                UserMgmClientId = this._config.GetSection("IdentityConfiguration").GetSection("UserMgmClientId").Value,
                UserMgmClientSecret = this._config.GetSection("IdentityConfiguration").GetSection("UserMgmClientSecret").Value,
                // ReferralUrl="https://dafexternal",
                Issuer = this._config.GetSection("IdentityConfiguration").GetSection("Issuer").Value,
                Audience = this._config.GetSection("IdentityConfiguration").GetSection("Audience").Value,
                // ReferralId="8c51b38a-f773-4810-8ac5-63b5fb9ca217",
                RsaPrivateKey = this._config.GetSection("IdentityConfiguration").GetSection("RsaPrivateKey").Value,
                RsaPublicKey = this._config.GetSection("IdentityConfiguration").GetSection("RsaPublicKey").Value
            };

            IOptions<IdentityJsonConfiguration> setting = Options.Create(idenityconfiguration);
            net.atos.daf.ct2.identity.IAccountManager iaccountManager = new net.atos.daf.ct2.identity.AccountManager(setting);

            TranslationRepository translationRepository = new TranslationRepository(_dataacess);
            ITranslationManager translationManager = new TranslationManager(translationRepository);

            IAccountRepository accountrepo = new AccountRepository(_dataacess);
            net.atos.daf.ct2.account.IAccountManager accManager = new net.atos.daf.ct2.account.AccountManager(accountrepo, _auditlog, iaccountManager, _config, translationManager);

            OrganizationRepository orgRepo = new OrganizationRepository(_dataacess, vehicleManager, groupManager, accManager, subscriptionManager, accountSessionManager, accountTokenManager);
            OrganizationManager org = new OrganizationManager(orgRepo, _auditlog);

            return org;
        }

    }
}
