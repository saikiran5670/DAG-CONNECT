using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;


namespace net.atos.daf.ct2.reportscheduler.repository
{
    public interface IReportSchedulerRepository
    {
        #region Parameter Report Schedular
        Task<IEnumerable<ReportType>> GetReportType(int accountid, int organizationid);
        Task<IEnumerable<ReportType>> GetReportType(int accountId, int organizationId, int contextorgId, int roleId);
        Task<IEnumerable<ReceiptEmails>> GetRecipientsEmails(int organizationid);
        Task<IEnumerable<DriverDetail>> GetDriverDetails(int organizationid);
        Task<ReportSchedulerMap> CreateReportScheduler(ReportSchedulerMap report);
        Task<ReportSchedulerMap> UpdateReportScheduler(ReportSchedulerMap report);
        Task<IEnumerable<ReportSchedulerMap>> GetReportSchedulerList(int organizationid);
        Task<int> ManipulateReportSchedular(ReportStatusUpdateDeleteModel objReportStatusUpdateDeleteModel);
        Task<PDFReportScreenModel> GetPDFBinaryFormatById(ReportPDFByidModel request);
        Task<PDFReportScreenModel> GetPDFBinaryFormatByToken(ReportPDFBytokenModel request);
        Task<string> UpdatePDFBinaryRecordByToken(string token);
        #endregion

        #region Report Creation Scheduler
        Task<IEnumerable<ReportCreationScheduler>> GetReportCreationSchedulerList();
        Task<IEnumerable<VehicleList>> GetVehicleList(int reprotSchedulerId);
        Task<VehicleList> GetVehicleListForSingle(int reprotSchedulerId);
        Task<ReportLogo> GetReportLogo(int accountId);
        Task<IEnumerable<UserTimeZone>> GetUserTimeZone();
        Task<IEnumerable<UserDateFormat>> GetUserDateFormat();
        Task<IEnumerable<UserTimeFormat>> GetUserTimeFormat();
        Task<int> InsertReportPDF(ScheduledReport scheduledReport);
        Task<IEnumerable<ReportColumnName>> GetColumnName(int reportId, string languageCode);
        #endregion

        Task<IEnumerable<ReportSchedulerEmailResult>> GetReportEmailDetails();
        Task<int> UpdateTimeRangeByDate(ReportEmailFrequency reportEmailFrequency);

    }
}
