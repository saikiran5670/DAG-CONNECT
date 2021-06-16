using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportschedulerservice.Entity
{
    public class Mapper
    {
        public ReportTypeRequest MapReportType(ReportType reportType)
        {
            ReportTypeRequest objreporttype = new ReportTypeRequest();
            objreporttype.Id = reportType.Id;
            objreporttype.ReportName = string.IsNullOrEmpty(reportType.ReportName) ? string.Empty : reportType.ReportName;
            //objreporttype.IsDriver = string.IsNullOrEmpty(reportType.Enum) ? string.Empty : reportType.Enum;           
            return objreporttype;
        }

        public DriverDetailRequest MapDriverDetail(DriverDetail driverDetail)
        {
            DriverDetailRequest objdriverdetail = new DriverDetailRequest();
            objdriverdetail.Id = driverDetail.Id;
            objdriverdetail.DriverId = driverDetail.DriverId;
            objdriverdetail.DriverName = string.IsNullOrEmpty(driverDetail.DriverName) ? string.Empty : driverDetail.DriverName;
            return objdriverdetail;
        }

        public ReceiptEmailsRequest MapReceiptEmail(ReceiptEmails receiptEmail)
        {
            ReceiptEmailsRequest objreceiptemail = new ReceiptEmailsRequest();
            objreceiptemail.Email = receiptEmail.Email;
            return objreceiptemail;
        }

        public ReportScheduler ToReportSchedulerEntity(ReportSchedulerRequest request)
        {
            ReportScheduler reportscheduler = new ReportScheduler();
            reportscheduler.Id = request.Id;
            reportscheduler.OrganizationId = request.OrganizationId;
            reportscheduler.ReportId = request.ReportId;
            reportscheduler.FrequencyType = System.Convert.ToChar(request.FrequencyType);
            reportscheduler.Status = System.Convert.ToChar(request.Status);
            reportscheduler.Type = System.Convert.ToChar(request.Type);
            reportscheduler.FileName = request.FileName;
            reportscheduler.StartDate = request.StartDate;
            reportscheduler.EndDate = request.EndDate;
            reportscheduler.Code = request.Code;
            reportscheduler.LastScheduleRunDate = request.LastScheduleRunDate;
            reportscheduler.NextScheduleRunDate = request.NextScheduleRunDate;
            reportscheduler.CreatedAt = request.CreatedAt;
            reportscheduler.CreatedBy = request.CreatedBy;
            reportscheduler.ModifiedAt = request.ModifiedAt;
            reportscheduler.ModifiedBy = request.ModifiedBy;
            reportscheduler.MailDescription = request.MailDescription;
            reportscheduler.MailSubject = request.MailSubject;
            reportscheduler.ReportDispatchTime = request.ReportDispatchTime;
            return reportscheduler;
        }
    }
}