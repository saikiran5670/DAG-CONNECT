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
    public class EmailNotificationManager : IEmailNotificationManager
    {
        private readonly IEmailRepository _emailRepository;

        private readonly IConfiguration _configuration;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ITranslationManager _translationManager;
        public EmailNotificationManager(IEmailRepository emailRepository, IConfiguration configuration, ITranslationManager translationManager)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;
            _emailConfiguration = new EmailConfiguration();
            _configuration.GetSection("EmailConfiguration").Bind(_emailConfiguration);
            this._translationManager = translationManager;
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
                mailNotificationRequest.MessageRequest.AlertNotification.DafEmailId = _configuration["DAFSupportEmailId"];
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

                return await EmailHelper.SendEmail(mailNotificationRequest.MessageRequest, emailTemplate);
            }
            catch (Exception)
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
