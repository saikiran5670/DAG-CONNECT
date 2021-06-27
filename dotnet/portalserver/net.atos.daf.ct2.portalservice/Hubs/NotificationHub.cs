using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using net.atos.daf.ct2.pushnotificationservice;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.hubs
{
    public class NotificationHub : Hub
    {
        private readonly PushNotificationService.PushNotificationServiceClient _pushNotofocationServiceClient;
        public NotificationHub(PushNotificationService.PushNotificationServiceClient pushNotofocationServiceClient)
        {
            _pushNotofocationServiceClient = pushNotofocationServiceClient;
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
                    await Clients.All.SendAsync("NotifyAlertResponse", this.Context.ConnectionId + " " + JsonConvert.SerializeObject(alertMessageData));
                }
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
            catch (Exception ex)
            {
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
        }
    }
}
