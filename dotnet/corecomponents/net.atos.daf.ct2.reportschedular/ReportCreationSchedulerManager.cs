using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.report;
using net.atos.daf.ct2.reportscheduler.repository;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportCreationSchedulerManager : IReportCreationSchedulerManager
    {
        private readonly ILogger<ReportCreationSchedulerManager> _logger;
        private readonly IReportSchedulerRepository _reportSchedulerRepository;
        private readonly IAuditTraillib _auditLog;
        //private readonly IGeneratePdf _generatePdf;
        private readonly IReportCreator _reportCreator;

        public ReportCreationSchedulerManager(ILogger<ReportCreationSchedulerManager> logger,
                                      IReportSchedulerRepository reportSchedularRepository,
                                      IAuditTraillib auditLog,
                                      //IGeneratePdf generatePdf,
                                      IReportCreator reportCreator)
        {
            _logger = logger;
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
                        //Check for Subscription                        
                        if (CheckForSubscription(reportSchedulerData, await _reportSchedulerRepository.GetReportType(reportSchedulerData.CreatedBy, reportSchedulerData.OrganizationId)))
                        {
                            _reportCreator.SetParameters(reportSchedulerData);
                            var isCreated = await _reportCreator.GenerateReport();
                            await AddAuditLog($"SchedulerId: {reportSchedulerData.Id}, IsSuccess: {isCreated}", isCreated ? AuditTrailEnum.Event_status.SUCCESS : AuditTrailEnum.Event_status.FAILED);
                        }
                        else
                        {
                            await AddAuditLog($"Scheduler Id: {reportSchedulerData.Id}, No subscription available for the report.", AuditTrailEnum.Event_status.FAILED, CreationConstants.LOG_UNSUBSCRIBED, reportSchedulerData.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Source == "Npgsql" && ex.InnerException != null && ex.InnerException.Message.Contains("Timeout"))
                        {
                            flag = false;
                            await AddAuditLog($"SchedulerId: {reportSchedulerData.Id}, Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED, CreationConstants.LOG_SQL_TIMEOUT, reportSchedulerData.Id);
                        }
                        else
                        {
                            flag = false;
                            await AddAuditLog($"SchedulerId: {reportSchedulerData.Id}, Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED, CreationConstants.LOG_MSG, reportSchedulerData.Id);
                        }
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

        private static bool CheckForSubscription(ReportCreationScheduler reportSchedulerData, IEnumerable<ReportType> reportSubscriptions) => reportSubscriptions.Any(w => w.Key == reportSchedulerData.ReportKey) && reportSubscriptions.Any(w => w.Key == ReportNameConstants.REPORT_SCHEDULE);

        private async Task AddAuditLog(string message, AuditTrailEnum.Event_status eventStatus, string updated_data = CreationConstants.LOG_MSG, int sourceObjectId = 0)
        {
            await _auditLog.AddLogs(new AuditTrail
            {
                Created_at = DateTime.Now,
                Performed_at = DateTime.Now,
                Performed_by = 2,
                Component_name = "Report_Creation_Scheduler",
                Service_name = "reportscheduler.CoreComponent",
                Event_type = AuditTrailEnum.Event_type.CREATE,
                Event_status = eventStatus,
                Message = message,
                Sourceobject_id = sourceObjectId,
                Targetobject_id = 0,
                Updated_data = updated_data
            });
        }
    }
}
