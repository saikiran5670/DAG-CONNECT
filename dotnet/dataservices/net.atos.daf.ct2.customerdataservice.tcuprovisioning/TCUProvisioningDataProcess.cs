using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCUReceive;
using TCUSend;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using System.Threading.Tasks;

namespace TCUProvisioning
{
    class TCUProvisioningDataProcess : ITCUProvisioningData,ITCUProvisioningDataReceiver
    {
        
        private static String ehubNamespaceConnectionString = ConfigurationManager.AppSetting["ehubNamespaceConnectionString"];
        private static String eventHubName = ConfigurationManager.AppSetting["eventHubName"];
        private static String blobStorageConnectionString = ConfigurationManager.AppSetting["blobStorageConnectionString"];
        private static String blobContainerName = ConfigurationManager.AppSetting["blobContainerName"];
        private static String consumerGroup = ConfigurationManager.AppSetting["GroupId"];

        public async Task subscribeTCUProvisioningTopic() {

            // Create a blob container client that the event processor will use 
            BlobContainerClient storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

            // Create an event processor client to process events in the event hub
            EventProcessorClient processor = new EventProcessorClient(storageClient, consumerGroup, ehubNamespaceConnectionString, eventHubName);

            // Register handlers for processing events and handling errors
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            await processor.StartProcessingAsync();

            // Wait for 10 seconds for the events to be processed
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Stop the processing
            await processor.StopProcessingAsync();
        }


        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            String TCUDataFromTopic = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());

            // Write the body of the event to the console window
            Console.WriteLine(TCUDataFromTopic);
            var TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);
            var TCUDataSend = createTCUDataInDAFFormat(TCUDataReceive);
            Console.WriteLine(TCUDataSend);
            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }


        private static String createTCUDataInDAFFormat(TCUDataReceive TCUDataReceive)
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
