using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using notificationservice.protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace portalservice.Controllers
{
    [ApiController]
    [Route("pushnotification")]
    public class PushNotificationController : ControllerBase
    {
        private readonly PushNotificationService.PushNotificationServiceClient _pushNotofocationServiceClient;
        public PushNotificationController(PushNotificationService.PushNotificationServiceClient pushNotofocationServiceClient)
        {
            _pushNotofocationServiceClient = pushNotofocationServiceClient;
        }

      [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var streamingCall = _pushNotofocationServiceClient.GetAlertMessageStream(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: cts.Token);

            try
            {
                await foreach (var alertMessageData in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {
                    return Ok($"{alertMessageData.Id} | {alertMessageData.Alertid} | {alertMessageData.CategoryType} ");
                }
                return Ok("Completed...");
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
