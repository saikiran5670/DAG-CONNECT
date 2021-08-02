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

namespace net.atos.daf.ct2.notificationservice.HostedServices
{
    public class NotificationHostedService : IHostedService
    {
        private readonly ILog _logger;
        private readonly Server _server;
        private readonly INotificationIdentifierManager _notificationIdentifierManager;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly KafkaConfiguration _kafkaConfiguration;
        private readonly IConfiguration _configuration;
        private readonly IEmailNotificationManager _emailNotificationManager;
        private readonly ISMSManager _smsManager;

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
            _kafkaConfiguration = new KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Start async called");
            // _server.Start();           
            while (true)
            {
                //ReadAndProcessAlertMessage();
                Thread.Sleep(10000); // 10 sec sleep mode
            }
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        private async Task ReadAndProcessAlertMessage()
        {
            try
            {
                NotificationEngineEntity.TripAlert tripAlert = new NotificationEngineEntity.TripAlert();
                KafkaEntity kafkaEntity = new KafkaEntity()
                {
                    BrokerList = _kafkaConfiguration.EH_FQDN,
                    ConnString = _kafkaConfiguration.EH_CONNECTION_STRING,
                    Topic = _kafkaConfiguration.EH_NAME,
                    Cacertlocation = _kafkaConfiguration.CA_CERT_LOCATION,
                    Consumergroup = _kafkaConfiguration.CONSUMER_GROUP
                };
                //Pushing message to kafka topic
                ConsumeResult<Null, string> response = KafkaConfluentWorker.Consumer(kafkaEntity);
                if (response != null)
                {
                    Console.WriteLine(response.Message.Value);
                    tripAlert = JsonConvert.DeserializeObject<NotificationEngineEntity.TripAlert>(response.Message.Value);
                    List<NotificationHistory> identifiedNotificationRec = await _notificationIdentifierManager.GetNotificationDetails(tripAlert);
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
                        await SendViaWebService(identifiedNotificationRec);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw;
            }
        }
        public async Task<bool> SendEmailNotification(List<NotificationHistory> notificationHistoryEmail)
        {
            try
            {
                bool isResult = false;

                foreach (var item in notificationHistoryEmail)
                {
                    string alertCategoryValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, item.AlertCategoryKey);
                    string urgencyTypeValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, item.UrgencyTypeKey);
                    string languageCode = await _notificationIdentifierManager.GetLanguageCodePreference(item.EmailId);
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
                    await _notificationIdentifierManager.InsertNotificationSentHistory(item);
                }
                return isResult;
            }
            catch (Exception)
            {
                throw;
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
                    headerDetails.Body = item.WsText;
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
                    string alertTypeValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, item.AlertTypeKey);
                    string urgencyTypeValue = await _notificationIdentifierManager.GetTranslateValue(string.Empty, item.UrgencyTypeKey);
                    string smsDescription = string.IsNullOrEmpty(item.SMS) ? item.SMS : item.SMS.Length <= 50 ? item.SMS : item.SMS.Substring(0, 50);
                    string smsBody = alertTypeValue + " " + item.ThresholdValue + " " + item.ThresholdValueUnitType + " " + item.ValueAtAlertTime + " " + urgencyTypeValue + " " + smsDescription;
                    SMS sms = new SMS();
                    sms.ToPhoneNumber = item.PhoneNo;
                    sms.Body = smsBody;
                    var status = await _smsManager.SendSMS(sms);
                    SMSStatus smsStatus = (SMSStatus)Enum.Parse(typeof(SMSStatus), status);
                    item.Status = ((char)smsStatus).ToString();
                    await _notificationIdentifierManager.InsertNotificationSentHistory(item);
                    isResult = true;
                }
                return isResult;
            }
            catch (Exception)
            {
                throw;
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
    }
}
