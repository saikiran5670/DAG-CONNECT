using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TCUReceive;
using TCUSend;

namespace TCUProvisioning
{
    class TCUProvision : ITCUProvisioningDataReceiver
    {
        private static String kafkaEndpoint = ConfigurationManager.AppSetting["BootstrapServers"];
        private static String topicname = ConfigurationManager.AppSetting["TCUProvisioningTopic"];
        private static String groupID = ConfigurationManager.AppSetting["GroupId"];
        private static String clientID = ConfigurationManager.AppSetting["ClientId"];
        private static String vehicleChangeTopic = ConfigurationManager.AppSetting["VehicleChangeTopic"];

        public void subscribeTCUProvisioningTopic()
        {
            var config = new ConsumerConfig
            {
                GroupId = groupID,
                BootstrapServers = kafkaEndpoint
            };

            using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
            {
                consumer.Subscribe(topicname);
                while (true)
                {
                    var cr = consumer.Consume();
                    String TCUDataFromTopic = cr.Message.Value;
                    var TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);
                    raiseVehicleChangeEvent(TCUDataReceive);

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
