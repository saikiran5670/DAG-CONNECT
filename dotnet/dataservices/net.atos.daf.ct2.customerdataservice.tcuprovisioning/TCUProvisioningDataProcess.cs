using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCUReceive;
using TCUSend;


namespace TCUProvisioning
{
    class TCUProvisioningDataProcess : ITCUProvisioningData,ITCUProvisioningDataReceiver
    {
        private static String kafkaEndpoint = ConfigurationManager.AppSetting["BootstrapServers"];
        private static String topicname = ConfigurationManager.AppSetting["TCUProvisioningTopic"];
        private static String groupID = ConfigurationManager.AppSetting["GroupId"];

        public void subscribeTCUProvisioningTopic() {

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
                    var TCUDataSend = createTCUDataInDAFFormat(TCUDataReceive);
                    Console.WriteLine(TCUDataSend);
             
                }

            }
        }

        public String createTCUDataInDAFFormat(TCUDataReceive TCUDataReceive)
        {
            
            DateTime dateTime = DateTime.Now;
            var Currdate = new DateTime(dateTime.Ticks);
            Currdate = Currdate.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));

            TCU tcu = new TCU(TCUDataReceive.DeviceIdentifier, null, null);
            TCURegistrationEvent TCURegistrationEvent = new TCURegistrationEvent(TCUDataReceive.Vin,tcu,"Yes", Currdate);
            List<TCURegistrationEvent> TCURegistrationEvents = new List<TCURegistrationEvent>();
            TCURegistrationEvents.Add(TCURegistrationEvent);
            TCUDataSend send = new TCUDataSend(new TCURegistrationEvents(TCURegistrationEvents));

            String TCUDataSendJson = JsonConvert.SerializeObject(send);
            return TCUDataSendJson;
        }


        public void postTCUProvisioningMessageToDAF(String TCUDataDAF) { 
            
           
        
        
        }

    }
}
