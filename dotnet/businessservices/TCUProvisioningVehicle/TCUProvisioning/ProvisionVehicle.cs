using Confluent.Kafka;
using Dapper;
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
        
        public static async Task readTCUProvisioningData(string brokerList, string connStr, string consumergroup, string topic, string cacertlocation,string psqlconnstring)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SocketTimeoutMs = 60000,                //this corresponds to the Consumer config `request.timeout.ms`
                SessionTimeoutMs = 30000,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connStr,
                SslCaLocation = cacertlocation,
                GroupId = consumergroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BrokerVersionFallback = "1.0.0",        //Event Hubs for Kafka Ecosystems supports Kafka v1.0+, a fallback to an older API will fail
                //Debug = "security,broker,protocol"    //Uncomment for librdkafka debugging information
            };

            using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

                consumer.Subscribe(topic);

                //Console.WriteLine("Consuming messages from topic: " + topic + ", broker(s): " + brokerList);

                while (true)
                {
                    try
                    {
                        var msg = consumer.Consume(cts.Token);
                        //Console.WriteLine($"Received: '{msg.Value}'");
                        String TCUDataFromTopic = msg.Message.Value;
                        //Console.WriteLine(TCUDataFromTopic);
                        var TCUDataSend = JsonConvert.DeserializeObject<TCUDataSend>(TCUDataFromTopic);
                        var TCURegistrationEvents = TCUDataSend.TcuRegistrationEvents.TcuRegistrationEvent;
                        foreach (TCURegistrationEvent item in TCURegistrationEvents)
                        {
                            await updateVehicleDetails(item, psqlconnstring);
                        }

                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
            }
        }



        static async Task updateVehicleDetails(TCURegistrationEvent TCUDataSend, string psqlConnString)
        {
            

            IVehicleManager vehicleManager = getVehicleManager(psqlConnString);
            VehicleFilter vehicleFilter = getFilteredVehicle(TCUDataSend);
            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);

            try

            {

                IEnumerable<Vehicle> vehicles = await vehicleManager.Get(vehicleFilter);
                Vehicle receivedVehicle = null;

                foreach (Vehicle vehicle in vehicles)
                {
                    receivedVehicle = vehicle;
                    Console.WriteLine("VIN is" + receivedVehicle.VIN);
                    break;
                }

                if (receivedVehicle == null)
                {
                    
                    Console.WriteLine("Vehicle is null proceeding to create vehicle");

                    receivedVehicle = new Vehicle();

                    receivedVehicle.VIN = TCUDataSend.VIN;
                    receivedVehicle.Vid = "ALMQ84521AA";
                    receivedVehicle.Tcu_Id = TCUDataSend.TCU.ID;
                    receivedVehicle.Tcu_Serial_Number = "BAYGHD74506";
                    receivedVehicle.Is_Tcu_Register = true;
                    receivedVehicle.Reference_Date = TCUDataSend.ReferenceDate;
                    receivedVehicle.VehiclePropertiesId = 0;
                    receivedVehicle.Opt_In = VehicleStatusType.Inherit;
                    receivedVehicle.Is_Ota = false;
                  
                    dynamic oiedetail = await GetOEM_Id(TCUDataSend.VIN,dataacess);
                    if (oiedetail != null)
                    {
                        receivedVehicle.Oem_id = oiedetail[0].id;
                        receivedVehicle.Oem_Organisation_id = oiedetail[0].oem_organisation_id;
                    }

                    int OrgId = await dataacess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.organization where lower(name)=@name), null)", new { name = "daf-paccar" });
                    receivedVehicle.Organization_Id = OrgId;
                    char org_status = await GetOrganisationStatusofVehicle(OrgId, dataacess);
                    receivedVehicle.Status = (VehicleCalculatedStatus)GetCalculatedVehicleStatus(org_status, receivedVehicle.Is_Ota);

                    await vehicleManager.Create(receivedVehicle);
                }
                else
                {

                    Console.WriteLine("Vehicle is present proceeding to update vehicle");
                    receivedVehicle.VIN = TCUDataSend.VIN;
                    // receivedVehicle.Vid = TCUDataReceive.Correlations.VehicleId;
                    receivedVehicle.Tcu_Id = TCUDataSend.TCU.ID;
                    receivedVehicle.Tcu_Serial_Number = "MAYGHD74506";
                    receivedVehicle.Is_Tcu_Register = true;
                    receivedVehicle.Reference_Date = TCUDataSend.ReferenceDate;
                    await vehicleManager.Update(receivedVehicle);

                }

            }
            catch (Exception ex)
            {
               // Console.WriteLine($"Error: {ex.Message}");
                throw ex;

            }
        }


        private static async Task<dynamic> GetOEM_Id(string vin, IDataAccess dataacess)
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


        public static async Task<char> GetOrganisationStatusofVehicle(int org_id, IDataAccess dataacess)
        {

            char optin = await dataacess.QuerySingleAsync<char>("SELECT vehicle_default_opt_in FROM master.organization where id=@id", new { id = org_id });

            return optin;
        }

        public static char GetCalculatedVehicleStatus(char opt_in, bool is_ota)
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

        private static VehicleFilter getFilteredVehicle(TCURegistrationEvent TCUDataSend)
        {
            VehicleFilter vehicleFilter = new VehicleFilter();

            vehicleFilter.OrganizationId = 0;
            vehicleFilter.VIN = TCUDataSend.VIN;
            vehicleFilter.VehicleId = 0;
            vehicleFilter.VehicleGroupId = 0;
            vehicleFilter.AccountId = 0;
            vehicleFilter.FeatureId = 0;
            vehicleFilter.VehicleIdList = "";
            vehicleFilter.Status = 0;
            vehicleFilter.AccountGroupId = 0;
            return vehicleFilter;
        }

        private static VehicleManager getVehicleManager(string psqlConnString)
        {
            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            IVehicleRepository vehiclerepo = new VehicleRepository(dataacess);
            IAuditLogRepository auditrepo = new AuditLogRepository(dataacess);
            IAuditTraillib audit = new AuditTraillib(auditrepo);
            VehicleManager vehicleManager = new VehicleManager(vehiclerepo, audit);
            return vehicleManager;
        }
    }
}
