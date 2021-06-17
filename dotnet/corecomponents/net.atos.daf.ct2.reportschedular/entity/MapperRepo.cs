using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class MapperRepo
    {
        public IEnumerable<ReportScheduler> GetReportSchedulerList(IEnumerable<ReportSchedulerResult> reportSchedulerResult)
        {
            List<ReportScheduler> reportSchedulerList = new List<ReportScheduler>();

            //Lookups are implemeted to avoid inserting duplicate entry of same id into the list
            Dictionary<int, ReportScheduler> reportSchedulerLookup = new Dictionary<int, ReportScheduler>();
            Dictionary<int, ScheduledReport> scheduledReportLookup = new Dictionary<int, ScheduledReport>();
            Dictionary<int, ScheduledReportRecipient> scheduledReportRecipientLookup = new Dictionary<int, ScheduledReportRecipient>();
            Dictionary<int, ScheduledReportVehicleRef> scheduledReportVehicleRefLookup = new Dictionary<int, ScheduledReportVehicleRef>();
            Dictionary<int, ScheduledReportDriverRef> scheduledReportDriverRefLookup = new Dictionary<int, ScheduledReportDriverRef>();

            foreach (var reportSchedulerItem in reportSchedulerResult)
            {
                if (!reportSchedulerLookup.TryGetValue(Convert.ToInt32(reportSchedulerItem.Repsch_id), out ReportScheduler reportScheduler))
                {
                    reportSchedulerLookup.Add(Convert.ToInt32(reportSchedulerItem.Repsch_id), reportScheduler = ToReportSchedulerModel(reportSchedulerItem));
                }
                if (reportScheduler.ScheduledReportRecipient == null)
                {
                    reportScheduler.ScheduledReportRecipient = new List<ScheduledReportRecipient>();
                }
                if (reportScheduler.ScheduledReportVehicleRef == null)
                {
                    reportScheduler.ScheduledReportVehicleRef = new List<ScheduledReportVehicleRef>();
                }
                if (reportScheduler.ScheduledReport == null)
                {
                    reportScheduler.ScheduledReport = new List<ScheduledReport>();
                }
                //if (reportSchedulerItem.Receipt_id > 0 && reportSchedulerItem.Repsch_id == reportSchedulerItem.Receipt_schedule_report_id)
                //{
                //    if (!scheduledReportRecipientLookup.TryGetValue(Convert.ToInt32(reportSchedulerItem.Receipt_schedule_report_id), out _))
                //    {
                //        var scheduledReportRecipient = ToAlertUrgencyLevelRefModel(alertItem);
                //        scheduledReportRecipientLookup.Add(Convert.ToInt32(reportSchedulerItem.Receipt_schedule_report_id), scheduledReportRecipient);
                //        reportScheduler.ScheduledReportRecipient.Add(scheduledReportRecipient);
                //    }
                //}
            }
            foreach (var keyValuePair in reportSchedulerLookup)
            {
                //add alert object along with child tables to alert list 
                reportSchedulerList.Add(keyValuePair.Value);
            }
            return reportSchedulerList;
        }

        public ReportScheduler ToReportSchedulerModel(ReportSchedulerResult request)
        {
            ReportScheduler reportScheduler = new ReportScheduler();
            reportScheduler.Id = request.Repsch_id;
            reportScheduler.OrganizationId = request.Repsch_organization_id;
            reportScheduler.ReportId = request.Repsch_report_id;
            reportScheduler.FrequencyType = request.Repsch_frequency_type;
            reportScheduler.Status = request.Repsch_status;
            reportScheduler.Type = request.Repsch_type;
            reportScheduler.FileName = request.Repsch_file_name;
            reportScheduler.StartDate = request.Repsch_start_date;
            reportScheduler.EndDate = request.Repsch_end_date;
            reportScheduler.Code = request.Repsch_code;
            reportScheduler.LastScheduleRunDate = request.Repsch_last_schedule_run_date;
            reportScheduler.NextScheduleRunDate = request.Repsch_next_schedule_run_date;
            reportScheduler.CreatedAt = request.Repsch_created_at;
            reportScheduler.CreatedBy = request.Repsch_created_by;
            reportScheduler.ModifiedAt = request.Repsch_modified_at;
            reportScheduler.ModifiedBy = request.Repsch_modified_by;
            reportScheduler.MailDescription = request.Repsch_mail_description;
            reportScheduler.MailSubject = request.Repsch_mail_subject;
            reportScheduler.ReportDispatchTime = request.Repsch_report_dispatch_time;
            reportScheduler.ScheduledReport = new List<ScheduledReport>();
            reportScheduler.ScheduledReportDriverRef = new ScheduledReportDriverRef();

            return reportScheduler;
        }
    }
}
