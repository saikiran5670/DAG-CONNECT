﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.notification;
using net.atos.daf.ct2.notification.entity;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.ENUM;
using net.atos.daf.ct2.reportscheduler.repository;

namespace net.atos.daf.ct2.reportscheduler
{
    public class ReportEmailSchedulerManager : IReportEmailSchedulerManager
    {
        private readonly IEmailNotificationManager _emailNotificationManager;
        private readonly IReportSchedulerRepository _reportSchedulerRepository;
        readonly IAuditTraillib _auditlog;
        public ReportEmailSchedulerManager(IEmailNotificationManager emailNotificationManager,
            IReportSchedulerRepository reportSchedulerRepository, IAuditTraillib auditTraillib)
        {
            _emailNotificationManager = emailNotificationManager;
            _reportSchedulerRepository = reportSchedulerRepository;
            _auditlog = auditTraillib;
        }
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
                await AddAuditLog(isSuccess, mailSent.EmailId);
                if (isSuccess)
                {
                    var nextUpdatedDate = UpdateNextTimeDate(emailItem);
                }
            }
            return reportsSent;
        }

        private async Task AddAuditLog(bool isSuccess, string emailId)
        {

            await _auditlog.AddLogs(new AuditTrail
            {
                Created_at = DateTime.Now,
                Performed_at = DateTime.Now,
                Performed_by = 2,
                Component_name = "Report Scheduler Email Notification",
                Service_name = "Report Email Scheduler Email Component",
                Event_type = AuditTrailEnum.Event_type.Mail,
                Event_status = isSuccess ? AuditTrailEnum.Event_status.SUCCESS : AuditTrailEnum.Event_status.FAILED,
                Message = isSuccess ? $"Email send to {emailId}" : $"Email is not send to {emailId}",
                Sourceobject_id = 0,
                Targetobject_id = 0,
                Updated_data = "EmailNotificationForReportSchedule"
            });
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
