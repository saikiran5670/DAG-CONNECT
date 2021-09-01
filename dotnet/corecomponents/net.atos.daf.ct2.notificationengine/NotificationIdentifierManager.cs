﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        public async Task<List<NotificationHistory>> GetNotificationDetails(TripAlert tripAlert)
        {
            try
            {
                tripAlert = await _notificationIdentifierRepository.GetVehicleIdForTrip(tripAlert);
                List<Notification> notificationOutput = new List<Notification>();
                List<Notification> notificationDetails = await _notificationIdentifierRepository.GetNotificationDetails(tripAlert);
                // Condition added to check for trip base alert and need to condition for non trip based alert type
                List<NotificationHistory> notificatinFrequencyCheck = await _notificationIdentifierRepository.GetNotificationHistory(tripAlert);
                List<TripAlert> generatedAlertForVehicle = await _notificationIdentifierRepository.GetGeneratedTripAlert(tripAlert);
                int numberOfAlertForvehicle = notificatinFrequencyCheck.Count();
                List<Notification> notificationTimingDetails = new List<Notification>();
                List<Notification> notificationNotifyDetails = new List<Notification>();
                List<NotificationHistory> identifiedNotificationRec = new List<NotificationHistory>();
                string frequencyType = notificationDetails.Select(f => f.Noti_frequency_type).FirstOrDefault();
                int frequencyThreshold = notificationDetails.Select(f => f.Noti_frequency_threshhold_value).FirstOrDefault();
                string validityType = notificationDetails.Select(f => f.Noti_validity_type).FirstOrDefault();
                // check frequency type of  notification
                //foreach (var item in notificationDetails)
                //{
                if (frequencyType.ToUpper() == "O")
                {
                    notificationOutput = notificationDetails.Where(p => notificatinFrequencyCheck.All(p2 => p2.AlertId != p.Noti_alert_id)).ToList();
                }
                else if (frequencyType.ToUpper() == "T" && numberOfAlertForvehicle != generatedAlertForVehicle.Count())
                {
                    //notificationOutput = notificationDetails.Where(p => notificatinFrequencyCheck.All(p2 => p2.AlertId != p.Noti_alert_id)).ToList();
                    notificationOutput = notificationDetails.Where(p => p.Noti_frequency_type.ToUpper() == "T").ToList();
                }
                else if (frequencyType.ToUpper() == "E")
                {
                    int index = 0;
                    var nGenAlertDetails = generatedAlertForVehicle.OrderBy(o => o.AlertGeneratedTime).GroupBy(e => new { e.Alertid, e.Vin }); //order by alert generated time  //.Where(e => e.Count() == item.Noti_frequency_threshhold_value);
                    for (int i = 1; i <= nGenAlertDetails.Count(); i++)
                    {
                        if (i % frequencyThreshold == 0)
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
                if (validityType.ToUpper() == "C")
                {
                    notificationTimingDetails = notificationOutput.Where(t => t.Aletimenoti_period_type.ToUpper() == "A").ToList();
                    var customeTimingDetails = notificationOutput.Where(t => t.Aletimenoti_period_type.ToUpper() == "C");
                    foreach (Notification customeTimingItem in customeTimingDetails)
                    {
                        //var bitsWithIndex = customeTimingItem.Aletimenoti_day_type.Cast<bool>() // we need to use Cast because BitArray does not provide generic IEnumerable
                        //   .Select((bit, index) => new { Bit = bit, Index = index }); // projection, we will save bit indices
                        for (int i = 0; i < customeTimingItem.Aletimenoti_day_type.Count; i++)
                        {
                            //if (customeTimingItem.Aletimenoti_day_type[i] == true && DateTime.Today.DayOfWeek.ToString().ToLower() == "monday")
                            //if (bitsWithIndex.Where(x => x.Bit == true && x.Index == (int)DateTime.Today.DayOfWeek).Select(x => x.Index).Count() > 0)
                            if (customeTimingItem.Aletimenoti_day_type[i] == true && i == (int)DateTime.Today.DayOfWeek)
                            {
                                int hourInSecond = DateTime.Now.Hour * 3600;
                                int minInSecond = DateTime.Now.Minute * 60;
                                int totalSecond = hourInSecond + minInSecond;
                                if (customeTimingItem.Aletimenoti_start_date <= totalSecond && customeTimingItem.Aletimenoti_end_date >= totalSecond)
                                    notificationTimingDetails.Add(customeTimingItem);
                            }
                        }
                    }
                }
                else if (validityType.ToUpper() == "A")
                {
                    notificationTimingDetails = notificationOutput.Where(t => t.Noti_validity_type.ToUpper() == "A").ToList();
                }
                //}
                //always
                int maxNotLim = 10;

                foreach (var item in notificationTimingDetails)
                {
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
                        notificationHistory.AlertCategoryKey = generatedAlertForVehicle.Select(c => c.AlertCategoryKey).FirstOrDefault();
                        notificationHistory.AlertCategoryEnum = generatedAlertForVehicle.Select(c => c.CategoryType).FirstOrDefault();
                        notificationHistory.AlertTypeKey = generatedAlertForVehicle.Select(c => c.AlertTypeKey).FirstOrDefault();
                        notificationHistory.AlertTypeEnum = generatedAlertForVehicle.Select(c => c.Type).FirstOrDefault();
                        notificationHistory.UrgencyTypeKey = generatedAlertForVehicle.Where(x => x.UrgencyLevelType == tripAlert.UrgencyLevelType).Select(c => c.UrgencyTypeKey).FirstOrDefault();
                        notificationHistory.UrgencyTypeEnum = generatedAlertForVehicle.Where(x => x.UrgencyLevelType == tripAlert.UrgencyLevelType).Select(c => c.UrgencyLevelType).FirstOrDefault();
                        notificationHistory.ThresholdUnitEnum = await _notificationIdentifierRepository.GetUnitType(item.Noti_alert_id, notificationHistory.UrgencyTypeEnum);
                        notificationHistory.ThresholdValue = UOMHandling.GetConvertedThresholdValue(tripAlert.ThresholdValue, notificationHistory.ThresholdUnitEnum);
                        notificationHistory.ThresholdValueUnitType = UOMHandling.GetUnitName(notificationHistory.ThresholdUnitEnum);
                        if (notificationHistory.AlertTypeEnum == "S" && notificationHistory.AlertCategoryEnum == "L")
                        {
                            long valueAtTimemilisecond = Convert.ToInt64(tripAlert.ValueAtAlertTime);
                            notificationHistory.ValueAtAlertTimeForHoursofServices = UTCHandling.GetConvertedDateTimeFromUTC(valueAtTimemilisecond, "UTC", "yyyy-MM-ddTHH:mm:ss.fffz");
                        }
                        else
                        {
                            if (notificationHistory.ThresholdUnitEnum == "H" || notificationHistory.ThresholdUnitEnum == "T")
                            {
                                notificationHistory.TimeBasedValueAtAlertTime = UOMHandling.GetConvertedTimeBasedThreshold(tripAlert.ValueAtAlertTime, notificationHistory.ThresholdUnitEnum);
                            }
                            else
                            {
                                notificationHistory.ValueAtAlertTime = UOMHandling.GetConvertedThresholdValue(tripAlert.ValueAtAlertTime, notificationHistory.ThresholdUnitEnum);
                            }
                        }
                        notificationHistory.SMS = item.Notrec_sms;
                        notificationHistory.AlertName = item.Ale_name;
                        notificationHistory.Vehicle_group_vehicle_name = item.Vehicle_group_vehicle_name;
                        notificationHistory.Vin = tripAlert.Vin;
                        notificationHistory.AlertGeneratedTime = tripAlert.AlertGeneratedTime;

                        identifiedNotificationRec.Add(notificationHistory);
                    }
                }
                return identifiedNotificationRec;
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
        public async Task<string> GetTranslateValue(string languageCode, string key)
        {
            return await _notificationIdentifierRepository.GetTranslateValue(languageCode, key);
        }
        public async Task<string> GetLanguageCodePreference(string emailId)
        {
            return await _notificationIdentifierRepository.GetLanguageCodePreference(emailId);
        }
    }
}
