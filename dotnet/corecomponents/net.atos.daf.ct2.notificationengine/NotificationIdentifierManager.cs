using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.notificationengine.repository;
using net.atos.daf.ct2.utilities;

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
            try
            {
                tripAlert = await _notificationIdentifierRepository.GetVehicleIdForTrip(tripAlert);
                List<Notification> notificationOutput = new List<Notification>();
                List<Notification> notificationDetails = await _notificationIdentifierRepository.GetNotificationDetails(tripAlert);
                List<NotificationHistory> notificatinFrequencyCheck = await _notificationIdentifierRepository.GetNotificationHistory(tripAlert);
                List<TripAlert> generatedAlertForVehicle = await _notificationIdentifierRepository.GetGeneratedTripAlert(tripAlert);
                int numberOfAlertForvehicle = notificatinFrequencyCheck.Count();
                List<Notification> notificationTimingDetails = new List<Notification>();
                List<Notification> notificationNotifyDetails = new List<Notification>();
                List<NotificationHistory> identifiedNotificationRec = new List<NotificationHistory>();
                // check frequency type of  notification
                foreach (var item in notificationDetails)
                {
                    if (item.Noti_frequency_type == "O")
                    {
                        notificationOutput = notificationDetails.Where(p => notificatinFrequencyCheck.All(p2 => p2.AlertId != p.Noti_alert_id)).ToList();
                    }
                    else if (item.Noti_frequency_type == "T" && item.Notlim_max_limit > numberOfAlertForvehicle)
                    {
                        notificationOutput = notificationDetails.Where(p => notificatinFrequencyCheck.All(p2 => p2.AlertId != p.Noti_alert_id)).ToList();
                    }
                    else if (item.Noti_frequency_type == "E")
                    {
                        int index = 0;
                        List<TripAlert> nGenAlertDetails = (List<TripAlert>)generatedAlertForVehicle.OrderBy(o => o.AlertGeneratedTime).GroupBy(e => new { e.Alertid, e.Vin }); //order by alert generated time  //.Where(e => e.Count() == item.Noti_frequency_threshhold_value);
                        for (int i = 1; i <= nGenAlertDetails.Count(); i++)
                        {
                            if (i / item.Noti_frequency_threshhold_value == 0)
                            {
                                //index = 0;
                                index = i;
                            }
                        }
                        if (index == nGenAlertDetails.Count())
                        {
                            notificationOutput = notificationDetails.Where(f => f.Noti_frequency_type.ToUpper() == "E").ToList();
                        }
                    }
                    // check notification filter custom
                    if (item.Noti_validity_type.ToUpper() == "C")
                    {
                        notificationTimingDetails = notificationOutput.Where(t => t.Aletimenoti_period_type.ToUpper() == "A").ToList();
                        var customeTimingDetails = notificationOutput.Where(t => t.Aletimenoti_period_type.ToUpper() == "C");
                        foreach (Notification customeTimingItem in customeTimingDetails)
                        {
                            var bitsWithIndex = customeTimingItem.Aletimenoti_day_type.Cast<bool>() // we need to use Cast because BitArray does not provide generic IEnumerable
                                .Select((bit, index) => new { Bit = bit, Index = index }); // projection, we will save bit indices
                            for (int i = 0; i < bitsWithIndex.Count(); i++)
                            {
                                //if (customeTimingItem.Aletimenoti_day_type[i] == true && DateTime.Today.DayOfWeek.ToString().ToLower() == "monday")
                                if (bitsWithIndex.Where(x => x.Bit == true && x.Index == (int)DateTime.Today.DayOfWeek).Select(x => x.Index).Count() > 0)
                                {
                                    int hourInSecond = DateTime.Now.Hour * 3600;
                                    int minInSecond = DateTime.Now.Minute * 60;
                                    int totalSecond = hourInSecond + minInSecond;
                                    if (customeTimingItem.Aletimenoti_start_date >= totalSecond && customeTimingItem.Aletimenoti_end_date <= totalSecond)
                                        notificationTimingDetails.Add(customeTimingItem);
                                }
                            }
                        }
                    }
                    else if (item.Noti_validity_type.ToUpper() == "A")
                    {
                        notificationTimingDetails = notificationOutput.Where(t => t.Noti_validity_type.ToUpper() == "A").ToList();
                    }
                }

                foreach (var item in notificationTimingDetails)
                {
                    //always
                    int maxNotLim = 10;
                    if (item.Notlim_notification_mode_type.ToUpper() == "A")
                    {
                        item.Notlim_max_limit = maxNotLim;
                    }
                    //Custom
                    int sentNotificationCount = notificationTimingDetails.Count();
                    if (item.Notlim_notification_mode_type.ToUpper() == "C")
                    {
                        if (item.Notlim_notification_period_type.ToUpper() == "Y")
                        {
                            item.Notlim_period_limit = item.Notlim_period_limit * 60;
                        }
                    }
                    if (item.Notlim_max_limit > numberOfAlertForvehicle)
                    {
                        NotificationHistory notificationHistory = new NotificationHistory();
                        notificationHistory.OrganizationId = item.Ale_organization_id;
                        notificationHistory.AlertId = item.Noti_alert_id;
                        notificationHistory.TripId = tripAlert.Tripid;
                        notificationHistory.NotificationId = item.Noti_id;
                        notificationHistory.VehicleId = tripAlert.VehicleId;
                        notificationHistory.RecipientId = item.Notrec_id;
                        notificationHistory.NotificationModeType = item.Notrec_notification_mode_type;
                        notificationHistory.PhoneNo = item.Notrec_phone_no;
                        notificationHistory.EmailId = item.Notrec_email_id;
                        notificationHistory.WsUrl = item.Notrec_ws_url;
                        notificationHistory.NotificationSendDate = UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString());
                        notificationHistory.Status = "";
                        notificationHistory.EmailSub = item.Notrec_email_sub;
                        notificationHistory.EmailText = item.Notrec_email_text;
                        notificationHistory.WsAuthType = item.Notrec_ws_type;
                        notificationHistory.WsLogin = item.Notrec_ws_login;
                        notificationHistory.WsPassword = item.Notrec_ws_password;
                        notificationHistory.WsText = item.Notrec_ws_text;
                        identifiedNotificationRec.Add(notificationHistory);
                    }
                }

                if (identifiedNotificationRec.Where(x => x.NotificationModeType == "E").Count() > 0)
                {

                }
                else if (identifiedNotificationRec.Where(x => x.NotificationModeType == "S").Count() > 0)
                {
                }
                else if (identifiedNotificationRec.Where(x => x.NotificationModeType == "W").Count() > 0)
                {
                }

                return notificationDetails;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<NotificationHistory> InsertNotificationSentHistory(NotificationHistory notificationHistory)
        {
            return await _notificationIdentifierRepository.InsertNotificationSentHistory(notificationHistory);
        }
    }
}
