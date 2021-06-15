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
            return reportscheduler;
        }
    }
}