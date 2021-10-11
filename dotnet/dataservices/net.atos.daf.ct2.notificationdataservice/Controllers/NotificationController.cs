using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.notificationdataservice.CustomAttributes;
using net.atos.daf.ct2.notificationdataservice.Entity;
using net.atos.daf.ct2.notificationengine;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.notificationdataservice.Controllers
{
    [ApiController]
    [Route("notification")]
    [Authorize(Policy = AccessPolicies.MAIN_ACCESS_POLICY)]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly IAuditTraillib _auditTrail;
        private readonly IAccountManager _accountManager;
        private readonly IOtaSoftwareNotificationManager _otaSoftwareNotificationManager;
        private readonly KafkaConfiguration _kafkaConfiguration;
        public NotificationController(ILogger<NotificationController> logger, IAuditTraillib auditTrail, IAccountManager accountManager, IConfiguration configuration, IOtaSoftwareNotificationManager otaSoftwareNotificationManager)
        {
            this._auditTrail = auditTrail;
            this._accountManager = accountManager;
            this._logger = logger;
            _kafkaConfiguration = new KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
            _otaSoftwareNotificationManager = otaSoftwareNotificationManager;
        }
        [HttpPost]
        [Route("event")]
        public async Task<IActionResult> CreateCampaignEvent(Root campaign)
        {
            try
            {
                if (campaign.CampaignEvent == null)
                {
                    return StatusCode(400, string.Empty);
                }
                confluentkafka.entity.KafkaConfiguration kafkaEntity = new confluentkafka.entity.KafkaConfiguration()
                {
                    BrokerList = _kafkaConfiguration.EH_FQDN,
                    ConnString = _kafkaConfiguration.EH_CONNECTION_STRING,
                    Topic = _kafkaConfiguration.EH_NAME,
                    Cacertlocation = _kafkaConfiguration.CA_CERT_LOCATION,
                    Consumergroup = _kafkaConfiguration.CONSUMER_GROUP
                };
                TripAlertOtaConfigParam tripAlertOta = new TripAlertOtaConfigParam();
                tripAlertOta.Vin = campaign.CampaignEvent.VIN;
                tripAlertOta.Campaign = campaign.CampaignEvent.Campaign;
                tripAlertOta.Baseline = campaign.CampaignEvent.Baseline;
                tripAlertOta.StatusCode = (int)campaign.CampaignEvent.StatusCode;
                tripAlertOta.Status = campaign.CampaignEvent.Status;
                tripAlertOta.CampaignId = campaign.CampaignEvent.CampaignID;
                tripAlertOta.Subject = campaign.CampaignEvent.Subject;
                tripAlertOta.TimeStamp = campaign.CampaignEvent.Timestamp;

                int result = await _otaSoftwareNotificationManager.CreateCampaignEvent(tripAlertOta, kafkaEntity);
                if (result > 0)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(400, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while processing Notification data.", ex);
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Notification Data Service", "Notification data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "Notification dataservice modified object", 0, 0, ex.InnerException.ToString(), 0, 0);
                return StatusCode(500, string.Empty);
            }
        }
    }
}
