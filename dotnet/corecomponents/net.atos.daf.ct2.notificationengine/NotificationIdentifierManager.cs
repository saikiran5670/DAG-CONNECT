using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.notification;
using net.atos.daf.ct2.notification.entity;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.notificationengine.repository;
using net.atos.daf.ct2.sms;
using net.atos.daf.ct2.sms.entity;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.webservice;
using net.atos.daf.ct2.webservice.entity;

namespace net.atos.daf.ct2.notificationengine
{
    public class NotificationIdentifierManager : INotificationIdentifierManager
    {
        private readonly INotificationIdentifierRepository _notificationIdentifierRepository;
        private readonly IEmailNotificationManager _emailNotificationManager;
        private readonly ISMSManager _smsManager;
        public NotificationIdentifierManager(INotificationIdentifierRepository notificationIdentifierRepository, IEmailNotificationManager emailNotificationManager, ISMSManager smsManager)
        {
            _notificationIdentifierRepository = notificationIdentifierRepository;
            _emailNotificationManager = emailNotificationManager;
            _smsManager = smsManager;
        }
        public async Task<List<Notification>> GetNotificationDetails(TripAlert tripAlert)
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
                        notificationHistory.AlertCategoryKey = generatedAlertForVehicle[0].AlertCategoryKey;
                        notificationHistory.AlertCategoryEnum = generatedAlertForVehicle[0].CategoryType;
                        notificationHistory.AlertTypeKey = generatedAlertForVehicle[0].AlertTypeKey;
                        notificationHistory.AlertTypeEnum = generatedAlertForVehicle[0].Type;
                        notificationHistory.UrgencyTypeKey = generatedAlertForVehicle[0].UrgencyTypeKey;
                        notificationHistory.UrgencyTypeEnum = generatedAlertForVehicle[0].UrgencyLevelType;
                        notificationHistory.ThresholdValue = tripAlert.ThresholdValue;
                        notificationHistory.ThresholdValueUnitType = tripAlert.ThresholdValueUnitType;
                        notificationHistory.ValueAtAlertTime = tripAlert.ValueAtAlertTime;
                        notificationHistory.SMS = item.Notrec_sms;
                        notificationHistory.AlertName = item.Ale_name;
                        notificationHistory.Vehicle_group_vehicle_name = item.Vehicle_group_vehicle_name;
                        notificationHistory.Vin = tripAlert.Vin;
                        notificationHistory.AlertGeneratedTime = tripAlert.AlertGeneratedTime;

                        identifiedNotificationRec.Add(notificationHistory);
                    }
                }

                if (identifiedNotificationRec.Where(x => x.NotificationModeType.ToUpper() == "E").Count() > 0)
                {
                    await SendEmailNotification(identifiedNotificationRec);
                }

                if (identifiedNotificationRec.Where(x => x.NotificationModeType.ToUpper() == "S").Count() > 0)
                {
                    await SendSMS(identifiedNotificationRec);
                }

                if (identifiedNotificationRec.Where(x => x.NotificationModeType.ToUpper() == "W").Count() > 0)
                {
                    await GetWebServiceCall(identifiedNotificationRec);
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
        public async Task<bool> SendEmailNotification(List<NotificationHistory> notificationHistoryEmail)
        {
            try
            {
                bool isResult = false;

                foreach (var item in notificationHistoryEmail)
                {
                    string alertCategoryValue = await GetTranslateValue(string.Empty, item.AlertCategoryKey);
                    string urgencyTypeValue = await GetTranslateValue(string.Empty, item.UrgencyTypeKey);
                    string languageCode = await GetLanguageCodePreference(item.EmailId);
                    string alertGenTime = UTCHandling.GetConvertedDateTimeFromUTC(item.AlertGeneratedTime, "UTC", null);
                    Dictionary<string, string> addAddress = new Dictionary<string, string>();
                    if (!addAddress.ContainsKey(item.EmailId))
                    {
                        addAddress.Add(item.EmailId, null);
                    }
                    var mailNotification = new MailNotificationRequest()
                    {
                        MessageRequest = new MessageRequest()
                        {
                            AccountInfo = new AccountInfo() { EmailId = item.EmailId, Organization_Id = item.OrganizationId },
                            ToAddressList = addAddress,
                            Subject = item.EmailSub,
                            Description = item.EmailText,
                            AlertNotification = new AlertNotification() { AlertName = item.AlertName, AlertLevel = urgencyTypeValue, AlertLevelCls = GetAlertTypeCls(urgencyTypeValue), DefinedThreshold = item.ThresholdValue, ActualThresholdValue = item.ValueAtAlertTime, AlertCategory = alertCategoryValue, VehicleGroup = item.Vehicle_group_vehicle_name, AlertDateTime = alertGenTime }
                        },
                        ContentType = EmailContentType.Html,
                        EventType = EmailEventType.AlertNotificationEmail
                    };

                    isResult = await _emailNotificationManager.TriggerSendEmail(mailNotification);
                    item.Status = isResult ? ((char)NotificationSendType.Successful).ToString() : ((char)NotificationSendType.Failed).ToString();
                    await InsertNotificationSentHistory(item);
                }
                return isResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> GetWebServiceCall(List<NotificationHistory> notificationHistoryWebService)
        {
            try
            {
                bool isResult = false;
                foreach (var item in notificationHistoryWebService)
                {
                    WebServiceManager wsClient = new WebServiceManager();
                    HeaderDetails headerDetails = new HeaderDetails();
                    headerDetails.BaseUrl = item.WsUrl;
                    headerDetails.Body = item.WsText;
                    headerDetails.AuthType = item.WsAuthType;
                    headerDetails.UserName = item.WsLogin;
                    headerDetails.Password = item.WsPassword;
                    headerDetails.ContentType = "application/json";
                    HttpResponseMessage response = await wsClient.HttpClientCall(headerDetails);
                    item.Status = response.StatusCode == System.Net.HttpStatusCode.OK ? ((char)NotificationSendType.Successful).ToString() : ((char)NotificationSendType.Failed).ToString();
                    await InsertNotificationSentHistory(item);
                    isResult = true;
                }

                return isResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SendSMS(List<NotificationHistory> notificationHistory)
        {
            try
            {
                bool isResult = false;
                foreach (var item in notificationHistory)
                {
                    string alertTypeValue = await GetTranslateValue(string.Empty, item.AlertTypeKey);
                    string urgencyTypeValue = await GetTranslateValue(string.Empty, item.UrgencyTypeKey);
                    string smsDescription = string.IsNullOrEmpty(item.SMS) ? item.SMS : item.SMS.Length <= 50 ? item.SMS : item.SMS.Substring(0, 50);
                    string smsBody = alertTypeValue + " " + item.ThresholdValue + " " + item.ThresholdValueUnitType + " " + item.ValueAtAlertTime + " " + urgencyTypeValue + " " + smsDescription;
                    SMS sms = new SMS();
                    sms.ToPhoneNumber = item.PhoneNo;
                    sms.Body = smsBody;
                    var status = await _smsManager.SendSMS(sms);
                    SMSStatus smsStatus = (SMSStatus)Enum.Parse(typeof(SMSStatus), status);
                    item.Status = ((char)smsStatus).ToString();
                    await InsertNotificationSentHistory(item);
                    isResult = true;
                }

                return isResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetTranslateValue(string languageCode, string key)
        {
            return await _notificationIdentifierRepository.GetTranslateValue(languageCode, key);
        }
        public async Task<string> GetLanguageCodePreference(string emailId)
        {
            return await _notificationIdentifierRepository.GetLanguageCodePreference(emailId);
        }
        private string GetAlertTypeCls(string alertType)
        {
            string alertTypeCls = string.Empty;
            switch (alertType)
            {
                case "Critical":
                    alertTypeCls = "alertCriticalLevel";
                    break;
                case "Warning":
                    alertTypeCls = "alertWarningLevel";
                    break;
                case "Advisory":
                    alertTypeCls = "alertAdvisoryLevel";
                    break;
            }
            return alertTypeCls;
        }
    }
}
