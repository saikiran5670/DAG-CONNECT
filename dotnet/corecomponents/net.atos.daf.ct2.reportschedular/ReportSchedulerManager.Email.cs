using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.notification.entity;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.ENUM;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportSchedulerManager : IReportSchedulerManager
    {

        public async Task<List<ReportEmailDetail>> SendReportEmail()
        {
            var reportsSent = new List<ReportEmailDetail>();
            var emailDetails = await _reportSchedulerRepository.GetReportEmailDetails();
            foreach (var emailItem in emailDetails)
            {
                var mailNotifictaion = new MailNotificationRequest()
                {
                    MessageRequest = new MessageRequest()
                    {
                        AccountInfo = new AccountInfo() { EmailId = emailItem.EmailId, Organization_Id = emailItem.OrganizationId }
                    },
                    ContentType = EmailContentType.Html,
                    EventType = EmailEventType.SendReport
                };
                var isSuccess = await _emailNotificationManager.TriggerSendEmail(mailNotifictaion);
                var mailSent = new ReportEmailDetail() { EmailId = emailItem.EmailId, IsMailSent = emailItem.IsMailSent, ReportId = emailItem.ReportSchedulerId };
                reportsSent.Add(mailSent);
                if (isSuccess)
                {
                    var nextUpdatedDate = UpdateNextTimeDate(emailItem);
                }
            }
            return reportsSent;
        }

        private async Task<int> UpdateNextTimeDate(ReportSchedulerEmailResult emailItem)
        {

            var reportEmailFrequency = new ReportEmailFrequency()
            {
                ReportId = emailItem.ReportSchedulerId,
                EndDate = emailItem.EndDate,
                FrequencyType = (TimeFrequenyType)Enum.Parse(typeof(TimeFrequenyType), emailItem.FrequencyType),
                ReportNextScheduleRunDate = emailItem.NextScheduleRunDate,
                ReportPrevioudScheduleRunDate = emailItem.LastScheduleRunDate,
                StartDate = emailItem.StartDate,
                ReportScheduleRunDate = emailItem.NextScheduleRunDate

            };
            var timeupdated = await _reportSchedulerRepository.UpdateTimeRangeByDate(reportEmailFrequency);
            return timeupdated;
        }


    }
}
