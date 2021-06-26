using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.notification.entity;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportSchedulerManager : IReportSchedulerManager
    {

        public async Task<bool> SendReportEmail()
        {
            var emailDetails = await _reportSchedulerRepository.GetReportEmailDetails();
            var mailNotifictaion = new MailNotificationRequest()
            {
                MessageRequest = new MessageRequest()
                {
                    AccountInfo = new AccountInfo() { }
                },
                ContentType = EmailContentType.Html,
                EventType = EmailEventType.SendReport
            };
            var isSuccess = await _emailNotificationManager.TriggerSendEmail(mailNotifictaion);
            return isSuccess;
        }




    }
}
