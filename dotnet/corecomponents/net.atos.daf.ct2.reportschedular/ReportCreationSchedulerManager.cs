using System;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.reportscheduler.report;
using net.atos.daf.ct2.reportscheduler.repository;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportCreationSchedulerManager : IReportCreationSchedulerManager
    {

        private readonly IReportSchedulerRepository _reportSchedulerRepository;
        private readonly IAuditTraillib _auditLog;
        //private readonly IGeneratePdf _generatePdf;
        private readonly IReportCreator _reportCreator;

        public ReportCreationSchedulerManager(IReportSchedulerRepository reportSchedularRepository,
                                      IAuditTraillib auditLog,
                                      //IGeneratePdf generatePdf,
                                      IReportCreator reportCreator)
        {
            _reportSchedulerRepository = reportSchedularRepository;
            _auditLog = auditLog;
            //_generatePdf = generatePdf;
            _reportCreator = reportCreator;
        }

        public async Task<bool> GenerateReport()
        {
            var flag = true;
            try
            {
                foreach (var reportSchedulerData in await _reportSchedulerRepository.GetReportCreationSchedulerList())
                {
                    try
                    {
                        _reportCreator.SetParameters(reportSchedulerData);
                        var isCreated = await _reportCreator.GenerateReport();
                        await AddAuditLog($"SchedulerId: {reportSchedulerData.Id}, IsSuccess: {isCreated}", isCreated ? AuditTrailEnum.Event_status.SUCCESS : AuditTrailEnum.Event_status.FAILED);
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        await AddAuditLog($"SchedulerId: {reportSchedulerData.Id}, Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED);
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

        private async Task AddAuditLog(string message, AuditTrailEnum.Event_status eventStatus)
        {
            await _auditLog.AddLogs(new AuditTrail
            {
                Created_at = DateTime.Now,
                Performed_at = DateTime.Now,
                Performed_by = 2,
                Component_name = "Report Creation Scheduler",
                Service_name = "reportscheduler.CoreComponent",
                Event_type = AuditTrailEnum.Event_type.CREATE,
                Event_status = eventStatus,
                Message = message,
                Sourceobject_id = 0,
                Targetobject_id = 0,
                Updated_data = "ReportCreationScheduler"
            });
        }
    }
}
