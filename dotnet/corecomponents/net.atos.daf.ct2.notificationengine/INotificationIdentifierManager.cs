﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;

namespace net.atos.daf.ct2.notificationengine
{
    public interface INotificationIdentifierManager
    {
        Task<List<NotificationHistory>> GetNotificationDetails(TripAlert tripAlert);
        Task<NotificationHistory> InsertNotificationSentHistory(NotificationHistory notificationHistory);
        Task<string> GetTranslateValue(string languageCode, string key);
        Task<string> GetLanguageCodePreference(string emailId);
        Task<AlertMessageAndAccountClientEntity> GetEligibleAccountForAlert(AlertMessageAndAccountClientEntity alertMessageAndAccountClientEntity);
    }
}
