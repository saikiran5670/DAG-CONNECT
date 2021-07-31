﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Grpc.Core;
using net.atos.daf.ct2.notificationengine;
using net.atos.daf.ct2.notificationengine.entity;
using log4net;
using System.Reflection;
using System.Timers;

namespace net.atos.daf.ct2.notificationservice.HostedServices
{
    public class NotificationHostedService : IHostedService
    {
        private readonly ILog _logger;
        private readonly Server _server;
        private readonly INotificationIdentifierManager _notificationIdentifierManager;
        private readonly IHostApplicationLifetime _appLifetime;
        private static System.Timers.Timer _aTimer;
        public NotificationHostedService(INotificationIdentifierManager notificationIdentifierManager, Server server, IHostApplicationLifetime appLifetime)
        {
            _notificationIdentifierManager = notificationIdentifierManager;
            _server = server;
            _appLifetime = appLifetime;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Info("Construtor called");
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Start async called");
            // _server.Start();
            // Create a timer with a two second interval.
            _aTimer = new System.Timers.Timer(150000);
            // Hook up the Elapsed event for the timer. 
            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.AutoReset = true;
            _aTimer.Enabled = true;
            while (true)
            {
                OnStarted(); //_appLifetime.ApplicationStarted.Register(OnStarted);
                Thread.Sleep(30000);
            }
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        private void OnStarted()
        {
            try
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
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw;
            }
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            throw new Exception();
        }
    }
}
