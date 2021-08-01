using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Grpc.Core;
using net.atos.daf.ct2.notificationengine;
using NotificationEngineEntity = net.atos.daf.ct2.notificationengine.entity;
using log4net;
using System.Reflection;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.confluentkafka.entity;
using net.atos.daf.ct2.notificationservice.entity;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.notificationservice.HostedServices
{
    public class NotificationHostedService : IHostedService
    {
        private readonly ILog _logger;
        private readonly Server _server;
        private readonly INotificationIdentifierManager _notificationIdentifierManager;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly KafkaConfiguration _kafkaConfiguration;
        private readonly IConfiguration _configuration;
        public NotificationHostedService(INotificationIdentifierManager notificationIdentifierManager, Server server, IHostApplicationLifetime appLifetime, IConfiguration configuration)
        {
            _notificationIdentifierManager = notificationIdentifierManager;
            _server = server;
            _appLifetime = appLifetime;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Info("Construtor called");
            this._configuration = configuration;
            _kafkaConfiguration = new KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Start async called");
            // _server.Start();           
            while (true)
            {
                //ReadAndProcessAlertMessage();
                Thread.Sleep(10000); // 10 sec sleep mode
            }
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        private void ReadAndProcessAlertMessage()
        {
            try
            {
                NotificationEngineEntity.TripAlert tripAlert = new NotificationEngineEntity.TripAlert();
                KafkaEntity kafkaEntity = new KafkaEntity()
                {
                    BrokerList = _kafkaConfiguration.EH_FQDN,
                    ConnString = _kafkaConfiguration.EH_CONNECTION_STRING,
                    Topic = _kafkaConfiguration.EH_NAME,
                    Cacertlocation = _kafkaConfiguration.CA_CERT_LOCATION,
                    Consumergroup = _kafkaConfiguration.CONSUMER_GROUP
                };
                //Pushing message to kafka topic
                ConsumeResult<Null, string> response = KafkaConfluentWorker.Consumer(kafkaEntity);
                if (response != null)
                {
                    Console.WriteLine(response.Message.Value);
                    tripAlert = JsonConvert.DeserializeObject<NotificationEngineEntity.TripAlert>(response.Message.Value);
                    _notificationIdentifierManager.GetNotificationDetails(tripAlert);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw;
            }
        }
    }
}
