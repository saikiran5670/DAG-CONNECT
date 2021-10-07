using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.confluentkafka.entity;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.notificationengine.repository;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.notificationengine
{
    public class OtaSoftwareNotificationManager : IOtaSoftwareNotificationManager
    {
        private readonly IOtaSoftwareNotificationRepository _otaSoftwareNotificationRepository;

        public OtaSoftwareNotificationManager(IOtaSoftwareNotificationRepository otaSoftwareNotificationRepository)
        {
            _otaSoftwareNotificationRepository = otaSoftwareNotificationRepository;
        }

        public async Task<int> CreateCampaignEvent(TripAlertOtaConfigParam tripAlertOtaConfigParam, KafkaConfiguration kafkaEntity)
        {
            TripAlert tripAlert = await _otaSoftwareNotificationRepository.InsertTripAlertOtaConfigParam(tripAlertOtaConfigParam);

            //TripAlert object conver into json
            kafkaEntity.ProducerMessage = JsonConvert.SerializeObject(tripAlert);
            await KafkaConfluentWorker.Producer(kafkaEntity);
            return tripAlert.Id;
        }

    }
}
