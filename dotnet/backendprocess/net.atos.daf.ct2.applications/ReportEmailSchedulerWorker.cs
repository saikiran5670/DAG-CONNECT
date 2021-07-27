using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.reportscheduler;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.applications
{
    internal class ReportEmailSchedulerWorker : BackgroundService
    {
        private readonly ILogger<ReportEmailSchedulerWorker> _logger;
        private readonly IAuditTraillib _auditlog;
        private readonly IReportEmailSchedulerManager _reportEmailSchedulerManager;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _configuration;

        public ReportEmailSchedulerWorker(ILogger<ReportEmailSchedulerWorker> logger,
                        IConfiguration configuration,
                        IAuditTraillib auditlog,
                        IReportEmailSchedulerManager reportEmailSchedulerManager,
                        IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _configuration = configuration;
            _auditlog = auditlog;
            _reportEmailSchedulerManager = reportEmailSchedulerManager;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
            {
                _logger.LogInformation("Ending the process...at: {time}", DateTimeOffset.Now);
            });
            try
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var isSuccess = await _reportEmailSchedulerManager.SendReportEmail();
                await AddAuditLog(isSuccess.Count() > 0 ? "Process run successfully" : "Proccess run failed, For more details, please check audit logs.", isSuccess.Count() > 0 ? AuditTrailEnum.Event_status.SUCCESS : AuditTrailEnum.Event_status.FAILED);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to run, Error: {ex.Message}");
                await AddAuditLog($"Failed to email run, Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED);
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }

        private async Task AddAuditLog(string message, AuditTrailEnum.Event_status eventStatus)
        {
            await _auditlog.AddLogs(new AuditTrail
            {
                Created_at = DateTime.Now,
                Performed_at = DateTime.Now,
                Performed_by = 2,
                Component_name = "Report Scheduler Email Notification",
                Service_name = "Backend Process",
                Event_type = AuditTrailEnum.Event_type.CREATE,
                Event_status = eventStatus,
                Message = message,
                Sourceobject_id = 0,
                Targetobject_id = 0,
                Updated_data = "EmailNotificationForReportSchedule"
            });
        }
    }
}