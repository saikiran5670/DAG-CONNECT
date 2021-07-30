using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Grpc.Core;
using net.atos.daf.ct2.notificationengine;
using net.atos.daf.ct2.notificationengine.entity;

namespace net.atos.daf.ct2.notificationservice.HostedServices
{
    public class NotificationHostedService : IHostedService
    {
        private readonly Server _server;
        private readonly INotificationIdentifierManager _notificationIdentifierManager;
        private readonly IHostApplicationLifetime _appLifetime;
        public NotificationHostedService(INotificationIdentifierManager notificationIdentifierManager, Server server, IHostApplicationLifetime appLifetime)
        {
            _notificationIdentifierManager = notificationIdentifierManager;
            _server = server;
            _appLifetime = appLifetime;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // _server.Start();
            while (true)
            {
                OnStarted(); //_appLifetime.ApplicationStarted.Register(OnStarted);
                Thread.Sleep(60000);
            }
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        private void OnStarted()
        {
            TripAlert tripAlert = new TripAlert();
            tripAlert.Id = 1;
            tripAlert.Tripid = "a801403e-ae4c-42cf-bf2d-ae39009c69oi";
            tripAlert.Vin = "XLR0998HGFFT76657";
            tripAlert.CategoryType = "L";
            tripAlert.Type = "G";
            tripAlert.Alertid = 328;
            tripAlert.Latitude = 51.12768896;
            tripAlert.Longitude = 4.935644520;
            tripAlert.AlertGeneratedTime = 1626965785;
            tripAlert.ThresholdValue = 8766;
            tripAlert.ValueAtAlertTime = 8767;
            tripAlert.ThresholdValueUnitType = "M";
            _notificationIdentifierManager.GetNotificationDetails(tripAlert);
        }
    }
}
