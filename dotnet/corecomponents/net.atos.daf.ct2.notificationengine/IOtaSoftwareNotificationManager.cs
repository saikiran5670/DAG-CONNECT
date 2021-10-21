using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.confluentkafka.entity;
using net.atos.daf.ct2.notificationengine.entity;

namespace net.atos.daf.ct2.notificationengine
{
    public interface IOtaSoftwareNotificationManager
    {
        Task<int> CreateCampaignEvent(TripAlertOtaConfigParam tripAlertOtaConfigParam, KafkaConfiguration kafkaEntity);
    }
}
