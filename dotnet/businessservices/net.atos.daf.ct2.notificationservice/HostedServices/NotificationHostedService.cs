using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Grpc.Core;
using net.atos.daf.ct2.notificationengine;
using NotificationEngineEntity = net.atos.daf.ct2.notificationengine.entity;
using log4net;
using System.Reflection;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.confluentkafka.entity;
using net.atos.daf.ct2.notificationservice.entity;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using Newtonsoft.Json;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.notification.entity;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.webservice;
using net.atos.daf.ct2.webservice.entity;
using System.Net.Http;
using net.atos.daf.ct2.sms.entity;
using net.atos.daf.ct2.notification;
using net.atos.daf.ct2.sms;
using System.Text;

namespace net.atos.daf.ct2.notificationservice.HostedServices
{
    public class NotificationHostedService : IHostedService
    {
        private readonly ILog _logger;
        private readonly Server _server;
        private readonly INotificationIdentifierManager _notificationIdentifierManager;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly entity.KafkaConfiguration _kafkaConfiguration;
        private readonly IConfiguration _configuration;
        private readonly IEmailNotificationManager _emailNotificationManager;
        private readonly ISMSManager _smsManager;
        private readonly NotificationConfiguration _notificationConfiguration;

        public NotificationHostedService(INotificationIdentifierManager notificationIdentifierManager, Server server, IHostApplicationLifetime appLifetime, IConfiguration configuration, IEmailNotificationManager emailNotificationManager, ISMSManager smsManager)
        {
            _notificationIdentifierManager = notificationIdentifierManager;
            _server = server;
            _appLifetime = appLifetime;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Info("Construtor called");
            _emailNotificationManager = emailNotificationManager;
            _smsManager = smsManager;
            this._configuration = configuration;
            _kafkaConfiguration = new entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
            _notificationConfiguration = new NotificationConfiguration();
            configuration.GetSection("NotificationConfiguration").Bind(_notificationConfiguration);
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Start async called");
            // _server.Start();           
            while (true)
            {
                await ReadAndProcessAlertMessage();
                Thread.Sleep(_notificationConfiguration.ThreadSleepTimeInSec); // 10 sec sleep mode
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        private async Task ReadAndProcessAlertMessage()
        {
            NotificationEngineEntity.TripAlert tripAlert = new NotificationEngineEntity.TripAlert();
            try
            {
                confluentkafka.entity.KafkaConfiguration kafkaEntity = new confluentkafka.entity.KafkaConfiguration()
                {
                    BrokerList = _kafkaConfiguration.EH_FQDN,
                    ConnString = _kafkaConfiguration.EH_CONNECTION_STRING,
                    Topic = _kafkaConfiguration.EH_NAME,
                    Cacertlocation = _kafkaConfiguration.CA_CERT_LOCATION,
                    Consumergroup = _kafkaConfiguration.CONSUMER_GROUP
                };
                //Pushing message to kafka topic
                ConsumeResult<string, string> response = KafkaConfluentWorker.Consumer(kafkaEntity);
                _logger.Info("Kafka consumer message: " + response.Message.Value);
                if (response != null)
                {
                    Console.WriteLine(response.Message.Value);
                    tripAlert = JsonConvert.DeserializeObject<NotificationEngineEntity.TripAlert>(response.Message.Value);
                    _logger.Info("Json trip alert object: " + tripAlert);
                    if (tripAlert != null && tripAlert.Alertid > 0)
                    {
                        List<NotificationHistory> identifiedNotificationRec = await _notificationIdentifierManager.GetNotificationDetails(tripAlert);
                        if (identifiedNotificationRec.Where(x => x.NotificationModeType.ToUpper() == "E").Count() > 0)
                        {
                            if (_notificationConfiguration.IsEmailSend == true)
                            {
                                await SendEmailNotification(identifiedNotificationRec.Where(x => x.NotificationModeType.ToUpper() == "E").ToList());
                            }
                        }

                        if (identifiedNotificationRec.Where(x => x.NotificationModeType.ToUpper() == "S").Count() > 0)
                        {
                            if (_notificationConfiguration.IsSMSSend == true)
                            {
                                await SendSMS(identifiedNotificationRec.Where(x => x.NotificationModeType.ToUpper() == "S").ToList());
                            }
                        }

                        if (identifiedNotificationRec.Where(x => x.NotificationModeType.ToUpper() == "W").Count() > 0)
                        {
                            if (_notificationConfiguration.IsWebServiceCall == true)
                            {
                                await SendViaWebService(identifiedNotificationRec.Where(x => x.NotificationModeType.ToUpper() == "W").ToList());
                            }
                        }
                    }
                    else
                    {
                        _logger.Info(JsonConvert.SerializeObject(tripAlert));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Notification Host Service", ex);
                ///failed message is getting logged.
                _logger.Info(JsonConvert.SerializeObject(tripAlert));
                //Need a discussion on handling failed kafka topic messages 
            }
        }
        public async Task<bool> SendEmailNotification(List<NotificationHistory> notificationHistoryEmail)
        {
            try
            {
                bool isResult = false;
                _logger.Info("Notification Email Method Call");
                foreach (var item in notificationHistoryEmail)
                {
                    string alertTypeValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, item.AlertTypeKey);
                    string alertCategoryValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, item.AlertCategoryKey);
                    string urgencyTypeValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, item.UrgencyTypeKey);
                    string languageCode = await _notificationIdentifierManager.GetLanguageCodePreference(item.EmailId);
                    string alertGenTime = UTCHandling.GetConvertedDateTimeFromUTC(item.AlertGeneratedTime, "UTC", null);
                    Dictionary<string, string> addAddress = new Dictionary<string, string>();
                    string[] splitemail = item?.EmailId.Split(';');
                    foreach (var itememail in splitemail)
                    {
                        if (!addAddress.ContainsKey(itememail))
                        {
                            addAddress.Add(itememail, null);
                        }
                    }
                    var mailNotification = new MailNotificationRequest()
                    {
                        MessageRequest = new MessageRequest()
                        {
                            AccountInfo = new AccountInfo() { EmailId = item.EmailId, Organization_Id = item.OrganizationId },
                            ToAddressList = addAddress,
                            Subject = item.EmailSub,
                            Description = item.EmailText,
                            AlertNotification = new AlertNotification()
                            {
                                AlertName = alertTypeValue,
                                AlertLevel = urgencyTypeValue,
                                AlertLevelCls = GetAlertTypeCls(urgencyTypeValue),
                                DefinedThreshold = (item.ThresholdUnitEnum == "H" || item.ThresholdUnitEnum == "T")
                                                    ? item.TimeBasedThresholdValue
                                                    : item.ThresholdValue == 0 ? "NA" : item.ThresholdValue.ToString() + " " + item.ThresholdValueUnitType,
                                ActualThresholdValue = item.AlertTypeEnum == "S" && item.AlertCategoryEnum == "L"
                                                        ? item.ValueAtAlertTimeForHoursofServices
                                                        : (
                                                            (item.ThresholdUnitEnum == "H" || item.ThresholdUnitEnum == "T")
                                                            ? item.TimeBasedValueAtAlertTime
                                                            : item.ValueAtAlertTime == 0 ? "NA" : item.ValueAtAlertTime.ToString() + " " + item.ThresholdValueUnitType
                                                           ),
                                AlertCategory = alertCategoryValue,
                                VehicleGroup = item.Vehiclegroup == "S" ? "NA" : item.Vehicle_group_vehicle_name,
                                VehicleName = item.Vehiclename,
                                AlertDateTime = alertGenTime
                            }
                        },
                        ContentType = EmailContentType.Html,
                        EventType = EmailEventType.AlertNotificationEmail
                    };

                    isResult = await _emailNotificationManager.TriggerSendEmail(mailNotification);
                    _logger.Info("Notification Email Method Result: " + isResult + "Alert Id: " + item.AlertId);
                    item.Status = isResult ? ((char)NotificationSendType.Successful).ToString() : ((char)NotificationSendType.Failed).ToString();
                    await _notificationIdentifierManager.InsertNotificationSentHistory(item);
                }
                return isResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Notification Host Service Email", ex);
                return false;
            }
        }
        public async Task<bool> SendViaWebService(List<NotificationHistory> notificationHistoryWebService)
        {
            try
            {
                bool isResult = false;
                foreach (var item in notificationHistoryWebService)
                {
                    WebServiceManager wsClient = new WebServiceManager();
                    HeaderDetails headerDetails = new HeaderDetails();
                    headerDetails.BaseUrl = item.WsUrl;
                    headerDetails.Body = await PrepareWSBody(item);
                    headerDetails.AuthType = item.WsAuthType;
                    headerDetails.UserName = item.WsLogin;
                    headerDetails.Password = item.WsPassword;
                    headerDetails.ContentType = "application/json";
                    HttpResponseMessage response = await wsClient.HttpClientCall(headerDetails);
                    item.Status = response.StatusCode == System.Net.HttpStatusCode.OK ? ((char)NotificationSendType.Successful).ToString() : ((char)NotificationSendType.Failed).ToString();
                    await _notificationIdentifierManager.InsertNotificationSentHistory(item);
                    isResult = true;
                }

                return isResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Notification Host Service WS", ex);
                return false;
            }
        }
        public async Task<bool> SendSMS(List<NotificationHistory> notificationHistory)
        {
            try
            {

                bool isResult = false;
                foreach (var item in notificationHistory)
                {
                    if (!string.IsNullOrEmpty(item.PhoneNo))
                    {
                        SMS sms = new SMS();
                        sms.ToPhoneNumber = item.PhoneNo;
                        sms.Body = await PrepareSMSBody(item);
                        var status = await _smsManager.SendSMS(sms);
                        SMSStatus smsStatus = (SMSStatus)Enum.Parse(typeof(SMSStatus), status);
                        item.Status = ((char)smsStatus).ToString();
                        await _notificationIdentifierManager.InsertNotificationSentHistory(item);
                        isResult = true;
                    }

                }
                return isResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Notification Host Service SMS", ex);
                return false;
                //throw;
            }
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
        private async Task<string> PrepareSMSBody(NotificationHistory notificationHistorySMS)
        {
            StringBuilder sbSMSText = new StringBuilder();
            string valueAtAlertTime;
            string thresholdNum;
            string alertCategoryValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, notificationHistorySMS.AlertCategoryKey);
            string alertTypeValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, notificationHistorySMS.AlertTypeKey);
            string urgencyTypeValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, notificationHistorySMS.UrgencyTypeKey);
            string smsDescription = string.IsNullOrEmpty(notificationHistorySMS.SMS) ? notificationHistorySMS.SMS : notificationHistorySMS.SMS.Length <= 50 ? notificationHistorySMS.SMS : notificationHistorySMS.SMS.Substring(0, 50);
            string vehicleGroup = notificationHistorySMS.Vehiclegroup == "S" ? "NA" : string.IsNullOrEmpty(notificationHistorySMS.Vehicle_group_vehicle_name) ? notificationHistorySMS.Vehicle_group_vehicle_name : notificationHistorySMS.Vehicle_group_vehicle_name.Length <= 17 ? notificationHistorySMS.Vehicle_group_vehicle_name : notificationHistorySMS.Vehicle_group_vehicle_name.Substring(0, 17);
            string vehicleName = string.IsNullOrEmpty(notificationHistorySMS.Vehiclename) ? notificationHistorySMS.Vehiclename : notificationHistorySMS.Vehiclename.Length <= 17 ? notificationHistorySMS.Vehiclename : notificationHistorySMS.Vehiclename.Substring(0, 17);
            string alertGenTime = UTCHandling.GetConvertedDateTimeFromUTC(notificationHistorySMS.AlertGeneratedTime, "UTC", null);
            if (notificationHistorySMS.ThresholdUnitEnum == "H" || notificationHistorySMS.ThresholdUnitEnum == "T")
            {
                thresholdNum = notificationHistorySMS.TimeBasedThresholdValue;
            }
            else
            {
                string[] thresholdNumSplit = notificationHistorySMS.ThresholdValue.ToString().Split('.');
                thresholdNum = notificationHistorySMS.ThresholdValue == 0 ? "NA" : thresholdNumSplit.Count() > 1 ? thresholdNumSplit[1].Length > 3 ? notificationHistorySMS.ThresholdValue.ToString("#.0000") : notificationHistorySMS.ThresholdValue.ToString() : notificationHistorySMS.ThresholdValue.ToString();
            }
            if (notificationHistorySMS.ThresholdUnitEnum == "H" || notificationHistorySMS.ThresholdUnitEnum == "T")
            {
                valueAtAlertTime = notificationHistorySMS.TimeBasedValueAtAlertTime;
            }
            else
            {
                string[] valueAtAlerttimeSplit = notificationHistorySMS.ValueAtAlertTime.ToString().Split('.');
                valueAtAlertTime = notificationHistorySMS.AlertTypeEnum == "S" && notificationHistorySMS.AlertCategoryEnum == "L"
                                          ? notificationHistorySMS.ValueAtAlertTimeForHoursofServices
                                          : notificationHistorySMS.ValueAtAlertTime == 0 ? "NA" : (valueAtAlerttimeSplit.Count() > 1)
                                            ? (valueAtAlerttimeSplit[1].Length > 3)
                                                ? notificationHistorySMS.ValueAtAlertTime.ToString("#.0000")
                                                : notificationHistorySMS.ValueAtAlertTime.ToString()
                                            : notificationHistorySMS.ValueAtAlertTime.ToString();
            }
            sbSMSText.AppendFormat("AN:{0}", alertTypeValue);
            sbSMSText.AppendFormat(",DT:{0}", thresholdNum);
            sbSMSText.AppendFormat(",AT:{0}", notificationHistorySMS.ThresholdValueUnitType);
            sbSMSText.AppendFormat(",AV:{0}", valueAtAlertTime);
            sbSMSText.AppendFormat(",{0}", alertCategoryValue);
            sbSMSText.AppendFormat(",VG:{0}", vehicleGroup);
            sbSMSText.AppendFormat(",VH:{0}", vehicleName);
            sbSMSText.AppendFormat(",UL:{0}", urgencyTypeValue);
            sbSMSText.AppendFormat(",T:{0}", alertGenTime);
            sbSMSText.AppendFormat(",{0}", smsDescription);
            return sbSMSText.ToString();
        }
        private async Task<string> PrepareWSBody(NotificationHistory notificationHistoryWS)
        {
            string valueAtAlertTime;
            string thresholdNum;
            StringBuilder sbWSText = new StringBuilder();
            string alertCategoryValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, notificationHistoryWS.AlertCategoryKey);
            string alertTypeValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, notificationHistoryWS.AlertTypeKey);
            string urgencyTypeValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, notificationHistoryWS.UrgencyTypeKey);
            string wsDescription = notificationHistoryWS.WsText;
            string vehicleGroup = notificationHistoryWS.Vehiclegroup == "S" ? "NA" : notificationHistoryWS.Vehicle_group_vehicle_name;
            string vehicleName = notificationHistoryWS.Vehiclename;
            string alertGenTime = UTCHandling.GetConvertedDateTimeFromUTC(notificationHistoryWS.AlertGeneratedTime, "UTC", null);
            if (notificationHistoryWS.ThresholdUnitEnum == "H" || notificationHistoryWS.ThresholdUnitEnum == "T")
            {
                thresholdNum = notificationHistoryWS.TimeBasedThresholdValue;
            }
            else
            {
                string[] thresholdNumSplit = notificationHistoryWS.ThresholdValue.ToString().Split('.');
                thresholdNum = notificationHistoryWS.ThresholdValue == 0 ? "NA" : thresholdNumSplit.Count() > 1 ? thresholdNumSplit[1].Length > 3 ? notificationHistoryWS.ThresholdValue.ToString("#.0000") : notificationHistoryWS.ThresholdValue.ToString() : notificationHistoryWS.ThresholdValue.ToString();
            }
            if (notificationHistoryWS.ThresholdUnitEnum == "H" || notificationHistoryWS.ThresholdUnitEnum == "T")
            {
                valueAtAlertTime = notificationHistoryWS.TimeBasedValueAtAlertTime;
            }
            else
            {
                string[] valueAtAlerttimeSplit = notificationHistoryWS.ValueAtAlertTime.ToString().Split('.');
                valueAtAlertTime = notificationHistoryWS.AlertTypeEnum == "S" && notificationHistoryWS.AlertCategoryEnum == "L"
                                    ? notificationHistoryWS.ValueAtAlertTimeForHoursofServices
                                    : notificationHistoryWS.ValueAtAlertTime == 0 ? "NA" : valueAtAlerttimeSplit.Count() > 1
                                        ? valueAtAlerttimeSplit[1].Length > 3
                                            ? notificationHistoryWS.ValueAtAlertTime.ToString("#.0000") + " " + notificationHistoryWS.ThresholdValueUnitType
                                            : notificationHistoryWS.ValueAtAlertTime.ToString() + " " + notificationHistoryWS.ThresholdValueUnitType
                                        : notificationHistoryWS.ValueAtAlertTime.ToString() + " " + notificationHistoryWS.ThresholdValueUnitType;
            }
            sbWSText.AppendFormat("Alert Name:{0}", alertTypeValue);
            sbWSText.AppendFormat(",Define Threshold:{0}", thresholdNum);
            sbWSText.AppendFormat(",Actual Threshold:{0}", notificationHistoryWS.ThresholdValueUnitType);
            sbWSText.AppendFormat(",Alert Vvalue at time:{0}", valueAtAlertTime);
            sbWSText.AppendFormat(",{0}", alertCategoryValue);
            sbWSText.AppendFormat(",Vehicle Group:{0}", vehicleGroup);
            sbWSText.AppendFormat(",Vehicle Name:{0}", vehicleName);
            sbWSText.AppendFormat(",Urgency Level:{0}", urgencyTypeValue);
            sbWSText.AppendFormat(",DateTime:{0}", alertGenTime);
            sbWSText.AppendFormat(",{0}", wsDescription);
            return sbWSText.ToString();
        }
    }
}
