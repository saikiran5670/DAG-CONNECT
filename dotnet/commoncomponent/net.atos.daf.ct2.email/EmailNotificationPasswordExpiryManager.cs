using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.email.Constants;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.email
{
    public class EmailNotificationPasswordExpiryManager : IEmailNotificationPasswordExpiryManager
    {
        private readonly IEmailNotificationPasswordExpiryRepository _emailNotificaton;
        private readonly EmailConfiguration emailConfiguration;
        private readonly IConfiguration _configuration;
        private readonly IAuditTraillib _auditlog;

        public EmailNotificationPasswordExpiryManager(IConfiguration configuration,
                                                        IAuditTraillib auditlog,
                                                        IEmailNotificationPasswordExpiryRepository emailNotificaton)
        {
            _emailNotificaton = emailNotificaton;
            _configuration = configuration;
            _auditlog = auditlog;
            emailConfiguration = new EmailConfiguration();
            configuration.GetSection("EmailConfiguration").Bind(emailConfiguration);
        }

        public async Task<IEnumerable<EmailList>> SendEmailForPasswordExpiry(int noOfDays)
        {
            var emailSendList = new ConcurrentBag<EmailList>();
            object synRoot = new object();

            Parallel.ForEach<string>(await _emailNotificaton.GetEmailOfPasswordExpiry(noOfDays), email =>
             {
                 try
                 {
                     var isSuccuss = EmailHelper.SendEmail(new MessageRequest
                     {
                         Configuration = emailConfiguration,
                         Subject = EmailContants.Subject,
                         Content = EmailContants.Content,
                         ContentMimeType = EmailContants.ContentMimeType,
                         ToAddressList = new Dictionary<string, string> { { email, null } ,
                                                                         { "harneet.rekhi@atos.net", null} }
                     }, null).Result;

                     lock (synRoot)
                     {
                         emailSendList.Add(new EmailList { Email = email, IsSend = isSuccuss });
                         _auditlog.AddLogs(new AuditTrail
                         {
                             Created_at = DateTime.Now,
                             Performed_at = DateTime.Now,
                             Performed_by = 2,
                             Component_name = "Email Notication Pasword Expiry",
                             Service_name = "Email Component",
                             Event_type = AuditTrailEnum.Event_type.Mail,
                             Event_status = isSuccuss ? AuditTrailEnum.Event_status.SUCCESS : AuditTrailEnum.Event_status.FAILED,
                             Message = isSuccuss ? $"Email send to {email}" : $"Email is not send to {email}",
                             Sourceobject_id = 0,
                             Targetobject_id = 0,
                             Updated_data = "EmailNotificationForPasswordExpiry"
                         }).Wait();
                     }
                 }
                 catch (Exception ex)
                 {
                     lock (synRoot)
                     {
                         emailSendList.Add(new EmailList { Email = email, IsSend = false });
                         _auditlog.AddLogs(new AuditTrail
                         {
                             Created_at = DateTime.Now,
                             Performed_at = DateTime.Now,
                             Performed_by = 2,
                             Component_name = "Email Notication Pasword Expiry",
                             Service_name = "Email Component",
                             Event_type = AuditTrailEnum.Event_type.Mail,
                             Event_status = AuditTrailEnum.Event_status.FAILED,
                             Message = $"Email is not send to {email} with error as {ex.Message}",
                             Sourceobject_id = 0,
                             Targetobject_id = 0,
                             Updated_data = "EmailNotificationForPasswordExpiry"
                         }).Wait();
                     }
                 }
             });
            return emailSendList.ToArray();
        }
    }
}
