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
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using System.Threading;
using Azure.Messaging.EventHubs.Producer;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.audit;

namespace TCUProvisioning
{
    class TCUProvision : ITCUProvisioningDataReceiver
    {

        //private const string ehubNamespaceConnectionString = ConfigurationManager.AppSetting["ehubNamespaceConnectionString"];

        private static String psqlConnString = ConfigurationManager.AppSetting["psqlconnstring"];
        private static String ehubNamespaceConnectionString = ConfigurationManager.AppSetting["ehubNamespaceConnectionString"];
        private static String eventHubName = ConfigurationManager.AppSetting["eventHubName"];
        private static String blobStorageConnectionString = ConfigurationManager.AppSetting["blobStorageConnectionString"];
        private static String blobContainerName = ConfigurationManager.AppSetting["blobContainerName"];
        private static String vehicleChangeTopic = ConfigurationManager.AppSetting["VehicleChangeTopic"];
        private static String consumerGroup = ConfigurationManager.AppSetting["GroupId"];

        // private readonly IVehicleManager vehicleManager;

        /*    public TCUProvision(IVehicleManager _vehicleManager)
            {
                vehicleManager = _vehicleManager;
            }*/

        public async Task subcribeTCUProvisioningTopic()
        {
            
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
            await Task.Delay(TimeSpan.FromSeconds(60));

            // Stop the processing
            await processor.StopProcessingAsync();
        }

        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            String TCUDataFromTopic = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());

            // Write the body of the event to the console window
            Console.WriteLine(TCUDataFromTopic);
            var TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);

            await updateVehicleDetails(TCUDataReceive);

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


        static async Task updateVehicleDetails(TCUDataReceive TCUDataReceive) {

            Console.WriteLine("Inside updateVehicleDetails method");

            DateTime dateTime = DateTime.Now;
            var Currdate = new DateTime(dateTime.Ticks);
            Currdate = Currdate.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));

            Console.WriteLine("After Date");

            IDataAccess dataacess = new PgSQLDataAccess(psqlConnString);
            IVehicleRepository vehiclerepo = new VehicleRepository(dataacess);
            IAuditLogRepository auditrepo = new AuditLogRepository(dataacess);
            IAuditTraillib audit = new AuditTraillib(auditrepo);
            IVehicleManager vehicleManager = new VehicleManager(vehiclerepo,audit);

            Console.WriteLine("After vehicle object");

            VehicleFilter vehicleFilter = new VehicleFilter();

            Console.WriteLine("After vehicle filter object");

            vehicleFilter.OrganizationId = 0;
            vehicleFilter.VIN = TCUDataReceive.Vin;
            vehicleFilter.VehicleId = 0;
            vehicleFilter.VehicleGroupId = 0;
            vehicleFilter.AccountId = 0;
            vehicleFilter.FeatureId = 0;
            vehicleFilter.VehicleIdList = "";
            vehicleFilter.Status = 0;
            vehicleFilter.AccountGroupId = 0;

            Console.WriteLine("After vehicle filter object filling");

            Vehicle receivedVehicle = (Vehicle)await vehicleManager.Get(vehicleFilter);

            Console.WriteLine("Received vehicle value");
            Console.WriteLine(receivedVehicle);

            if (receivedVehicle == null)
            {
                Console.WriteLine("Vehicle is null proceeding to create vehicle");
                receivedVehicle.VIN = TCUDataReceive.Vin;
                receivedVehicle.Vid = TCUDataReceive.Correlations.VehicleId;
                receivedVehicle.Tcu_Id = TCUDataReceive.Correlations.DeviceId;
                receivedVehicle.Tcu_Serial_Number = TCUDataReceive.DeviceSerialNumber;
                receivedVehicle.Is_Tcu_Register = true;
                receivedVehicle.Reference_Date = Currdate;
                await vehicleManager.Create(receivedVehicle);
            }
            else {

                Console.WriteLine("Vehicle is present proceeding to update vehicle");
                receivedVehicle.VIN = TCUDataReceive.Vin;
                receivedVehicle.Vid = TCUDataReceive.Correlations.VehicleId;
                receivedVehicle.Tcu_Id = TCUDataReceive.Correlations.DeviceId;
                receivedVehicle.Tcu_Serial_Number = TCUDataReceive.DeviceSerialNumber;
                receivedVehicle.Is_Tcu_Register = true;
                receivedVehicle.Reference_Date = Currdate;
                await vehicleManager.Update(receivedVehicle);
                await raiseVehicleChangeEvent(TCUDataReceive);
            }


        }

        static async Task raiseVehicleChangeEvent(TCUDataReceive TCUDataReceive) {

            //Console.WriteLine("A batch of 1 events has been published.");

            Console.WriteLine("Inside raiseVehicleChangeEvent method");

            VehicleChangeEvent vehicleChangeEvent = new VehicleChangeEvent(TCUDataReceive.Vin,TCUDataReceive.DeviceIdentifier);
            TCUDataSend tcuDataSend = new TCUDataSend(vehicleChangeEvent);

            string TCUDataSendJson = JsonConvert.SerializeObject(tcuDataSend);

            await using (var producerClient = new EventHubProducerClient(ehubNamespaceConnectionString, vehicleChangeTopic))
            {
                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(TCUDataSendJson)));

                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
               // Console.WriteLine("A batch of 2 events has been published.");
            }

        }

        
    }
}
