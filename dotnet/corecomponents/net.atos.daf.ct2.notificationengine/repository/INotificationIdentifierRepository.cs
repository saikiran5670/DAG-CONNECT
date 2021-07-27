using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;

namespace net.atos.daf.ct2.notificationengine.repository
{
    public interface INotificationIdentifierRepository
    {
        Task<List<Notification>> GetNotificationDetails(TripAlert tripAlert);
        Task<List<NotificationHistory>> GetNotificationHistory(TripAlert tripAlert);
        Task<List<TripAlert>> GetGeneratedTripAlert(TripAlert tripAlert);
        Task<NotificationHistory> InsertNotificationSentHistory(NotificationHistory notificationHistory);
        Task<TripAlert> GetVehicleIdForTrip(TripAlert tripAlert);
    }
}
