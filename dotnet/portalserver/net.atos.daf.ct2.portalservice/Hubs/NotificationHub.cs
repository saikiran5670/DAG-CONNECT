using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.AspNetCore.SignalR;
using net.atos.daf.ct2.pushnotificationservice;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILog _logger;
        private readonly PushNotificationService.PushNotificationServiceClient _pushNotofocationServiceClient;
        public NotificationHub(PushNotificationService.PushNotificationServiceClient pushNotofocationServiceClient)
        {
            _pushNotofocationServiceClient = pushNotofocationServiceClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }
        public async Task NotifyAlert(string someTextFromClient)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(50));
            using var streamingCall = _pushNotofocationServiceClient.GetAlertMessageStream(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: cts.Token);
            string str = string.Empty;
            try
            {
                await foreach (var alertMessageData in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {
                    await Clients.All.SendAsync("NotifyAlertResponse", this.Context.ConnectionId + " " + JsonConvert.SerializeObject(alertMessageData) + " " + someTextFromClient);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                _logger.Error(null, ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
        }
    }
}
