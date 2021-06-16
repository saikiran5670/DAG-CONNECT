using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using notificationservice.protos;
using notificationservice.Entity;

namespace notificationservice.Services
{
    public class PushNotificationManagementService : PushNotificationService.PushNotificationServiceBase
    {
        public override async Task GetAlertMessageStream(Google.Protobuf.WellKnownTypes.Empty _, IServerStreamWriter<AlertMessageData> responseStream, ServerCallContext context)
        {
            var i = 0;
            while (!context.CancellationToken.IsCancellationRequested && i < 20)
            {
                await Task.Delay(500); 

                var forecast = new AlertMessageData
                {
                    Id = 1,
                    Tripid="1",
                    Vin="VIN",
                    CategoryType="L",
                    Type="E",
                    Name="Test",
                    Alertid=1,
                    ThresholdValue=300,
                    ThresholdValueUnitType="M",
                    ValueAtAlertTime= 1620272821,
                    Latitude = 51.07,
                    Longitude = 57.07,
                    AlertGeneratedTime = 1620272821,
                    MessageTimestamp = 1620272821,
                    CreatedAt = 1620272821,
                    ModifiedAt = 1620272821
                };
                i++;
                await responseStream.WriteAsync(forecast);
            }
        }
    }
}
