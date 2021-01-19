using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TCUReceive;
using TCUSend;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.vehicle.entity;

namespace TCUProvisioning
{
    class TCUProvision : ITCUProvisioningDataReceiver
    {
        private readonly IVehicleManager vehicleManager;

        public TCUProvision(IVehicleManager _vehicleManager)
        {
            vehicleManager = _vehicleManager;
        }




        private static String kafkaEndpoint = ConfigurationManager.AppSetting["BootstrapServers"];
        private static String topicname = ConfigurationManager.AppSetting["TCUProvisioningTopic"];
        private static String groupID = ConfigurationManager.AppSetting["GroupId"];
        private static String clientID = ConfigurationManager.AppSetting["ClientId"];
        private static String vehicleChangeTopic = ConfigurationManager.AppSetting["VehicleChangeTopic"];
        private static String psqlConnString = ConfigurationManager.AppSetting["psqlconnstring"];





        public async void scribeTCUProvisioningTopic()
        {
            var config = new ConsumerConfig
            {
                GroupId = groupID,
                BootstrapServers = kafkaEndpoint
            };
                       
           //DataAccess dataacess = new PgSQLDataAccess(psqlConnString);
           //VehicleRepository vehiclerepo = new VehicleRepository(dataacess);
           //VehicleManager vehicleManager = new VehicleManager(vehiclerepo);
            VehicleFilter vehicleFilter = new VehicleFilter();
            
            


            using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
            {
                consumer.Subscribe(topicname);
                while (true)
                {
                    var cr = consumer.Consume();
                    String TCUDataFromTopic = cr.Message.Value;
                    var TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);
                    vehicleFilter.OrganizationId = 0;
                    vehicleFilter.VIN = TCUDataReceive.Vin;
                    vehicleFilter.VehicleId = 0;
                    vehicleFilter.VehicleGroupId = 0;
                    vehicleFilter.AccountId = 0;
                    vehicleFilter.FeatureId = 0;
                    vehicleFilter.VehicleIdList = "";
                    vehicleFilter.Status = 0;
                    vehicleFilter.AccountGroupId = 0;
                    Vehicle receivedVehicle = await vehicleManager.Get(vehicleFilter);


                }

            }
        }

        public void raiseVehicleChangeEvent(TCUDataReceive TCUDataReceive) {

            VehicleChangeEvent vehicleChangeEvent = new VehicleChangeEvent(TCUDataReceive.Vin,TCUDataReceive.DeviceIdentifier);
            TCUDataSend tcuDataSend = new TCUDataSend(vehicleChangeEvent);

            string TCUDataSendJson = JsonConvert.SerializeObject(tcuDataSend);

            var config = new ProducerConfig
            {
                BootstrapServers = kafkaEndpoint,
                ClientId = clientID
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.ProduceAsync(vehicleChangeTopic, new Message<Null, string> { Value = TCUDataSendJson });
                producer.Flush();
                
            }
        }


    }
}
