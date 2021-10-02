using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;

namespace net.atos.daf.ct2.notificationengine.repository
{
    public interface IOtaSoftwareNotificationRepository
    {
        Task<TripAlertOtaConfigParam> InsertTripAlertOtaConfigParam(TripAlertOtaConfigParam tripAlertOtaConfigParam);
        Task<TripAlert> InsertTripAlert(TripAlert tripAlert);

    }
}
