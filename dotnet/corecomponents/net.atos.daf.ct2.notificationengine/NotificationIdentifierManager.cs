using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.notificationengine.repository;

namespace net.atos.daf.ct2.notificationengine
{
    public class NotificationIdentifierManager : INotificationIdentifierManager
    {
        private readonly INotificationIdentifierRepository _notificationIdentifierRepository;
        public NotificationIdentifierManager(INotificationIdentifierRepository notificationIdentifierRepository)
        {
            _notificationIdentifierRepository = notificationIdentifierRepository;
        }
        public async Task<List<Notification>> GetNotificationDetails(TripAlert tripAlert)
        {

            List<Notification> notificationDetails = await _notificationIdentifierRepository.GetNotificationDetails(tripAlert);

            foreach (var item in notificationDetails)
            {
                if (item.Noti_frequency_type == "O")
                {
                    List<NotificationHistory> notificatinFrequencyCheck = await _notificationIdentifierRepository.GetNotificationHistory(tripAlert);

                }

            }


            return notificationDetails;
        }
    }
}
