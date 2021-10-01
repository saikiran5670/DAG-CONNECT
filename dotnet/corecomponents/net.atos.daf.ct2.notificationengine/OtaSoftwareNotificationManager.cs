using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.notificationengine.repository;

namespace net.atos.daf.ct2.notificationengine
{
    public class OtaSoftwareNotificationManager : IOtaSoftwareNotificationManager
    {
        private readonly IOtaSoftwareNotificationRepository _otaSoftwareNotificationRepository;
        public OtaSoftwareNotificationManager(IOtaSoftwareNotificationRepository otaSoftwareNotificationRepository)
        {
            _otaSoftwareNotificationRepository = otaSoftwareNotificationRepository;
        }

        public async Task<TripAlertOtaConfigParam> InsertTripAlertOtaConfigParam(TripAlertOtaConfigParam tripAlertOtaConfigParam)
        {
            return await _otaSoftwareNotificationRepository.InsertTripAlertOtaConfigParam(tripAlertOtaConfigParam);
        }
    }
}
