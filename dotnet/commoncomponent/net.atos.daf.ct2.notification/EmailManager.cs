using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.notification.entity;
using net.atos.daf.ct2.notification.repository;
using net.atos.daf.ct2.translation;

namespace net.atos.daf.ct2.notification
{
    public class EmailManager : IEmailManager
    {
        private readonly IEmailRepository _emailRepository;

        private readonly IConfiguration _configuration;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ITranslationManager _translationManager;
        public EmailManager(IEmailRepository emailRepository, IConfiguration configuration, ITranslationManager translationManager)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;
            _emailConfiguration = new EmailConfiguration();
            _configuration.GetSection("EmailConfiguration").Bind(_emailConfiguration);
            this._translationManager = translationManager;
        }
        public async Task<bool> TriggerSendEmail(MailNotificationRequest mailNotificationRequest)
        {
            if (mailNotificationRequest.EventType == EmailEventType.PasswordExpiryNotification)
            {
                mailNotificationRequest.MessageRequest.RemainingDaysToExpire = Convert.ToInt32(_configuration["RemainingDaysToExpire"]);
            }

            mailNotificationRequest.MessageRequest.Configuration = _emailConfiguration;
            mailNotificationRequest.MessageRequest.TokenSecret = mailNotificationRequest.TokenSecret;
            try
            {
                var languageCode = await GetLanguageCodePreference(mailNotificationRequest.MessageRequest.AccountInfo.EmailId,
                                                                   mailNotificationRequest.MessageRequest.AccountInfo.Organization_Id);
                var emailTemplate = await _translationManager.GetEmailTemplateTranslations(mailNotificationRequest.EventType,
                                                                                           mailNotificationRequest.ContentType,
                                                                                           languageCode);

                return await EmailHelper.SendEmail(mailNotificationRequest.MessageRequest, emailTemplate);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<string> GetLanguageCodePreference(string emailId, int? orgId)
        {
            return await _emailRepository.GetLanguageCodePreference(emailId.ToLower(), orgId);
        }
    }
}
