using System;
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
using System.Linq;
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

            var reportEmailResults = from p in emailDetails
                                     group p by new { p.ReportCreatedBy, p.LanguageCode, p.EmailId } into g
                                     select new { CreatedBy = g.Key, ReportSchedulerEmailResult = g.ToList() };
            if (reportEmailResults.Any())
            {

                foreach (var item in reportEmailResults)
                {
                    try
                    {
                        MailNotificationRequest mailNotification = new MailNotificationRequest();
                        mailNotification.MessageRequest = new MessageRequest();
                        Dictionary<string, string> reportTokens = new Dictionary<string, string>();
                        Dictionary<string, string> toAddressList = new Dictionary<string, string>();
                        var mailSent = new ReportEmailDetail();
                        List<ReportTokens> reportTokensList = new List<ReportTokens>();
                        foreach (var emailItem in item.ReportSchedulerEmailResult)
                        {
                            ReportTokens objReportTokens = new ReportTokens();
                            objReportTokens.Token = emailItem.ReportToken.ToString();
                            objReportTokens.ReportName = emailItem.Key.Trim();
                            objReportTokens.RecipentId = emailItem.RecipentId;
                            reportTokensList.Add(objReportTokens);
                            if (!toAddressList.ContainsKey(emailItem.EmailId))
                            {
                                toAddressList.Add(emailItem.EmailId, null);
                            }
                            mailNotification.MessageRequest = new MessageRequest()
                            {
                                AccountInfo = new AccountInfo() { EmailId = emailItem.EmailId, Organization_Id = emailItem.OrganizationId },
                                LanguageCode = emailItem.LanguageCode.Trim(),
                                ReportTokens = reportTokensList,
                                ToAddressList = toAddressList,
                                Subject = emailItem.MailSubject,
                                Description = emailItem.MailDescription,
                                IsBcc = true
                            };
                            mailSent.EmailId = emailItem.EmailId;
                            mailSent.IsMailSent = emailItem.IsMailSent;
                            mailSent.ReportId = emailItem.ReportSchedulerId;
                            await AddAuditLog(mailSent.IsMailSent, mailSent.EmailId);
                        }
                        mailNotification.ContentType = EmailContentType.Html;
                        mailNotification.EventType = EmailEventType.ScheduledReportEmail;
                        var isSuccess = await _emailNotificationManager.TriggerSendEmail(mailNotification);

                        reportsSent.Add(mailSent);
                        await AddAuditLog(isSuccess, mailSent.EmailId);
                        if (isSuccess)
                        {
                            var nextUpdatedDate = await UpdateNextTimeDate(item.ReportSchedulerEmailResult);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Source == "Npgsql" && ex.InnerException != null && ex.InnerException.Message.Contains("Timeout"))
                        {
                            await AddAuditLog($"SendReportEmail: Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED, CreationConstants.LOG_SQL_TIMEOUT);
                        }
                        else
                        {
                            await AddAuditLog($"SendReportEmail: Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED, CreationConstants.LOG_EMAIL_MSG);
                        }
                    }
                }
            }
            return reportsSent;
        }

        public async Task<bool> UpdateMissingSchedulerFrequecy()
        {
            var flag = true;
            int count = 0;
            try
            {
                foreach (var item in await _reportSchedulerRepository.GetMissingSchedulerData())
                {
                    try
                    {
                        var countUpdatedId = await UpdateNextTimeDate(item);
                        if (count == 0 && countUpdatedId == 0)
                        {
                            flag = false;
                            count += 1;
                        }
                    }
                    catch (Exception)
                    {
                        flag = false;
                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                await AddAuditLog($"Failed to run, Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED);
            }
            return flag;
        }

        private async Task AddAuditLog(string message, AuditTrailEnum.Event_status eventStatus, string updated_data = CreationConstants.LOG_MISSING_MSG, int sourceObjectId = 0)
        {
            await _auditlog.AddLogs(new AuditTrail
            {
                Created_at = DateTime.Now,
                Performed_at = DateTime.Now,
                Performed_by = 2,
                Component_name = "Report Scheduler Email Notification",
                Service_name = "Report Email Scheduler Email Component",
                Event_type = AuditTrailEnum.Event_type.CREATE,
                Event_status = eventStatus,
                Message = message,
                Sourceobject_id = sourceObjectId,
                Targetobject_id = 0,
                Updated_data = updated_data
            });
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

        private async Task<int> UpdateNextTimeDate(List<ReportSchedulerEmailResult> emailItemList)
        {

            int timeupdated = 0;
            foreach (var emailItem in emailItemList)
            {
                try
                {
                    var reportEmailFrequency = new ReportEmailFrequency()
                    {
                        ReportSchedulerId = emailItem.ReportSchedulerId,
                        EndDate = emailItem.EndDate,
                        FrequencyType = (TimeFrequenyType)Enum.Parse(typeof(TimeFrequenyType), GetEnumValue(emailItem.FrequencyType)),
                        ReportNextScheduleRunDate = emailItem.NextScheduleRunDate,
                        ReportPrevioudScheduleRunDate = emailItem.NextScheduleRunDate,
                        StartDate = emailItem.StartDate,
                        ReportScheduleRunDate = emailItem.NextScheduleRunDate,
                        TimeZoneName = emailItem.TimeZoneName
                    };

                    if (emailItem.FrequencyType == ((char)TimeFrequenyType.Daily).ToString() || emailItem.FrequencyType == ((char)TimeFrequenyType.Weekly).ToString() || emailItem.FrequencyType == ((char)TimeFrequenyType.BiWeekly).ToString())
                    {
                        timeupdated = await _reportSchedulerRepository.UpdateTimeRangeByDate(reportEmailFrequency);

                    }
                    else
                    {
                        timeupdated = await _reportSchedulerRepository.UpdateTimeRangeByCalenderTime(reportEmailFrequency);
                    }
                    emailItem.IsMailSent = true;
                    await _reportSchedulerRepository.UpdateIsMailSend(emailItem.ReportToken, emailItem.IsMailSent);
                }
                catch (Exception ex)
                {
                    if (ex.Source == "Npgsql" && ex.InnerException != null && ex.InnerException.Message.Contains("Timeout"))
                    {
                        await AddAuditLog($"UpdateNextTimeDate: Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED, CreationConstants.LOG_SQL_TIMEOUT, emailItem.ReportSchedulerId);
                    }
                    else
                    {
                        await AddAuditLog($"UpdateNextTimeDate: Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED, CreationConstants.LOG_EMAIL_MSG, emailItem.ReportSchedulerId);
                    }
                }
            }
            return timeupdated;

        }

        private string GetEnumValue(string frequencyType)
        {
            string enumtype = string.Empty;
            switch (frequencyType)
            {
                case "D":
                    enumtype = TimeFrequenyType.Daily.ToString();
                    break;
                case "W":
                    enumtype = TimeFrequenyType.Weekly.ToString();
                    break;
                case "B":
                    enumtype = TimeFrequenyType.BiWeekly.ToString();
                    break;
                case "M":
                    enumtype = TimeFrequenyType.Monthly.ToString();
                    break;
                case "Q":
                    enumtype = TimeFrequenyType.Quartly.ToString();
                    break;
            }
            return enumtype;
        }

        private async Task<int> UpdateNextTimeDate(ReportEmailFrequency emailItem)
        {
            try
            {
                emailItem.ReportPrevioudScheduleRunDate = emailItem.ReportScheduleRunDate = emailItem.ReportNextScheduleRunDate;
                emailItem.FrequencyType = (TimeFrequenyType)emailItem.FrequencyTypeValue;
                if (emailItem.FrequencyType == TimeFrequenyType.Daily || emailItem.FrequencyType == TimeFrequenyType.Weekly || emailItem.FrequencyType == TimeFrequenyType.BiWeekly)
                {
                    return await _reportSchedulerRepository.UpdateTimeRangeByDate(emailItem);

                }
                else
                {
                    return await _reportSchedulerRepository.UpdateTimeRangeByCalenderTime(emailItem);
                }
            }
            catch (Exception ex)
            {
                if (ex.Source == "Npgsql" && ex.InnerException != null && ex.InnerException.Message.Contains("Timeout"))
                {
                    await AddAuditLog($"UpdateNextTimeDate: Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED, CreationConstants.LOG_SQL_TIMEOUT, emailItem.ReportSchedulerId);
                }
                else
                {
                    await AddAuditLog($"UpdateNextTimeDate: Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED, CreationConstants.LOG_MISSING_MSG, emailItem.ReportSchedulerId);
                }
                return 0;
            }
        }
    }
}