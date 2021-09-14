using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.notification.entity;
using net.atos.daf.ct2.notification.repository;
using net.atos.daf.ct2.translation;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.notification
{
    public class EmailNotificationManager : IEmailNotificationManager
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IConfiguration _configuration;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ITranslationManager _translationManager;
        private readonly IAuditTraillib _auditlog;

        public EmailNotificationManager(IEmailRepository emailRepository, IConfiguration configuration,
            ITranslationManager translationManager, IAuditTraillib auditlog)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;
            _emailConfiguration = new EmailConfiguration();
            _configuration.GetSection("EmailConfiguration").Bind(_emailConfiguration);
            this._translationManager = translationManager;
            _auditlog = auditlog;
        }
        public async Task<bool> TriggerSendEmail(MailNotificationRequest mailNotificationRequest)
        {
            if (string.IsNullOrEmpty(mailNotificationRequest.MessageRequest.AccountInfo.FullName))
            {
                var account = await _emailRepository.GetAccountByEmailId(mailNotificationRequest.MessageRequest.AccountInfo.EmailId);
                if (account != null)
                {
                    mailNotificationRequest.MessageRequest.AccountInfo.FullName = account.FullName;
                }
            }

            if (mailNotificationRequest.EventType == EmailEventType.PasswordExpiryNotification)
            {
                mailNotificationRequest.MessageRequest.RemainingDaysToExpire = Convert.ToInt32(_configuration["RemainingDaysToExpire"]);
            }
            if (mailNotificationRequest.EventType == EmailEventType.AlertNotificationEmail)
            {
                mailNotificationRequest.MessageRequest.AlertNotification.DafEmailId = _emailConfiguration.DAFSupportEmailId;
            }

            mailNotificationRequest.MessageRequest.Configuration = _emailConfiguration;
            mailNotificationRequest.MessageRequest.TokenSecret = mailNotificationRequest.TokenSecret;
            try
            {
                var languageCode = string.IsNullOrEmpty(mailNotificationRequest.MessageRequest.LanguageCode) ?
                                        await GetLanguageCodePreference(mailNotificationRequest.MessageRequest.AccountInfo.EmailId,
                                                                        mailNotificationRequest.MessageRequest.AccountInfo.Organization_Id)
                                        : mailNotificationRequest.MessageRequest.LanguageCode;
                var emailTemplate = await _translationManager.GetEmailTemplateTranslations(mailNotificationRequest.EventType,
                                                                                           mailNotificationRequest.ContentType,
                                                                                           languageCode);

                var response = await EmailHelper.SendEmail(mailNotificationRequest.MessageRequest, emailTemplate);

                // Log unsuccessful email attempts
                if (response != null && !response.IsSuccessStatusCode)
                {
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager",
                        AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, $"Sending email failed to { mailNotificationRequest.MessageRequest.AccountInfo.EmailId }", 0, 0, JsonConvert.SerializeObject(await response.Body.ReadAsStringAsync()));
                }
                else
                {
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Account Component", "Account Manager",
                           AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "No content found for the email template", 0, 0, mailNotificationRequest.MessageRequest.AccountInfo.EmailId);
                }

                return response?.IsSuccessStatusCode ?? false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetLanguageCodePreference(string emailId, int? orgId)
        {
            return await _emailRepository.GetLanguageCodePreference(emailId.ToLower(), orgId);
        }
    }
}
