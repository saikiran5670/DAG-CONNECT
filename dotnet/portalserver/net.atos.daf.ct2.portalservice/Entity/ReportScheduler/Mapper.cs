using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportschedulerservice;

namespace net.atos.daf.ct2.portalservice.Entity.ReportScheduler
{
    public class Mapper
    {
        public ReportSchedulerRequest ToReportSchedulerEntity(ReportScheduler request)
        {
            ReportSchedulerRequest reportscheduler = new ReportSchedulerRequest();
            reportscheduler.Id = request.Id;
            reportscheduler.OrganizationId = request.OrganizationId;
            reportscheduler.ReportId = request.ReportId;
            reportscheduler.FrequencyType = string.IsNullOrEmpty(request.FrequencyType) ? string.Empty : request.FrequencyType;
            reportscheduler.Status = string.IsNullOrEmpty(request.Status) ? string.Empty : request.Status;
            reportscheduler.Type = string.IsNullOrEmpty(request.Type) ? string.Empty : request.Type;
            reportscheduler.FileName = string.IsNullOrEmpty(request.FileName) ? string.Empty : request.FileName;
            reportscheduler.StartDate = request.StartDate;
            reportscheduler.EndDate = request.EndDate;
            reportscheduler.Code = string.IsNullOrEmpty(request.Code) ? string.Empty : request.Code;
            reportscheduler.LastScheduleRunDate = request.LastScheduleRunDate;
            reportscheduler.NextScheduleRunDate = request.NextScheduleRunDate;
            reportscheduler.CreatedAt = request.CreatedAt;
            reportscheduler.CreatedBy = request.CreatedBy;
            reportscheduler.ModifiedAt = request.ModifiedAt;
            reportscheduler.ModifiedBy = request.ModifiedBy;
            reportscheduler.MailDescription = string.IsNullOrEmpty(request.MailDescription) ? string.Empty : request.MailDescription;
            reportscheduler.MailSubject = string.IsNullOrEmpty(request.MailSubject) ? string.Empty : request.MailSubject;
            reportscheduler.ReportDispatchTime = request.ReportDispatchTime;
            if (request.ScheduledReportRecipient.Count > 0)
            {
                foreach (var item in request.ScheduledReportRecipient)
                {
                    reportscheduler.ScheduledReportRecipient.Add(ToScheduledReportRecipientEntity(item));
                }
            }
            if (reportscheduler.ScheduledReportDriverRef.Count > 0)
            {
                foreach (var item in request.ScheduledReportDriverRef)
                {
                    reportscheduler.ScheduledReportDriverRef.Add(ToScheduledReportDriverRefEntity(item));
                }
            }
            if (request.ScheduledReportVehicleRef.Count > 0)
            {
                foreach (var item in request.ScheduledReportVehicleRef)
                {
                    reportscheduler.ScheduledReportVehicleRef.Add(ToScheduledReportVehicleRefEntity(item));

                }
            }
            return reportscheduler;
        }

        public ScheduledReportRecipientRequest ToScheduledReportRecipientEntity(ScheduledReportRecipient request)
        {
            ScheduledReportRecipientRequest schedulereportsr = new ScheduledReportRecipientRequest();
            schedulereportsr.Id = request.Id;
            schedulereportsr.ScheduleReportId = request.ScheduleReportId;
            schedulereportsr.Email = string.IsNullOrEmpty(request.Email) ? string.Empty : request.Email;
            schedulereportsr.State = string.IsNullOrEmpty(request.State) ? string.Empty : request.State;
            schedulereportsr.CreatedAt = request.CreatedAt;
            schedulereportsr.ModifiedAt = request.ModifiedAt;
            return schedulereportsr;
        }

        public ScheduledReportDriverRefRequest ToScheduledReportDriverRefEntity(ScheduledReportDriverRef request)
        {
            ScheduledReportDriverRefRequest schedulereportdr = new ScheduledReportDriverRefRequest();
            schedulereportdr.ScheduleReportId = request.ScheduleReportId;
            schedulereportdr.DriverId = request.DriverId;
            schedulereportdr.State = string.IsNullOrEmpty(request.State) ? string.Empty : request.State;
            schedulereportdr.CreatedAt = request.CreatedAt;
            schedulereportdr.CreatedBy = request.CreatedBy;
            schedulereportdr.ModifiedAt = request.ModifiedAt;
            schedulereportdr.ModifiedBy = request.ModifiedBy;
            return schedulereportdr;
        }

        public ScheduledReportVehicleRefRequest ToScheduledReportVehicleRefEntity(ScheduledReportVehicleRef request)
        {
            ScheduledReportVehicleRefRequest schedulereportvr = new ScheduledReportVehicleRefRequest();
            schedulereportvr.ScheduleReportId = request.ScheduleReportId;
            schedulereportvr.VehicleGroupId = request.VehicleGroupId;
            schedulereportvr.State = string.IsNullOrEmpty(request.State) ? string.Empty : request.State;
            schedulereportvr.CreatedAt = request.CreatedAt;
            schedulereportvr.CreatedBy = request.CreatedBy;
            schedulereportvr.ModifiedAt = request.ModifiedAt;
            schedulereportvr.ModifiedBy = request.ModifiedBy;
            return schedulereportvr;
        }
    }
}
