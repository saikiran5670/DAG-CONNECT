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
       

        public void subscribeTCUProvisioningTopic() {

            var config = new ConsumerConfig
            {
                GroupId = "TCUProvisioning-consumers",
                BootstrapServers = "localhost:9092"
            };

            using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
            {
                consumer.Subscribe("tcutopic");
                while (true)
                {
                    var cr = consumer.Consume();
                    String TCUDataFromTopic = cr.Message.Value;
                    var TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);
                    var TCUDataSend = createTCUDataInDAFFormat(TCUDataReceive);
                    Console.WriteLine(TCUDataSend);
                    //Console.WriteLine(TCUDataReceive.Vin) ;
                    //JObject TCUDataJson = JObject.Parse(TCUData);
                    //TCUDataJson.Value();
                    //Console.WriteLine(TCUDataFromTopic.ToString());
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
