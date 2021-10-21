using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;

namespace net.atos.daf.ct2.notificationengine.repository
{
    public interface IOtaSoftwareNotificationRepository
    {
        Task<TripAlert> InsertTripAlertOtaConfigParam(TripAlertOtaConfigParam tripAlertOtaConfigParam);
        Task<int> InsertTripAlert(TripAlert tripAlert);
    }
}
