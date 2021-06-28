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
    internal class ReportCreationSchedulerWorker : BackgroundService
    {
        private readonly ILogger<ReportCreationSchedulerWorker> _logger;
        private readonly IAuditTraillib _auditlog;
        private readonly IReportCreationSchedulerManager _reportCreationSchedulerManager;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _configuration;

        public ReportCreationSchedulerWorker(ILogger<ReportCreationSchedulerWorker> logger,
                        IConfiguration configuration,
                        IAuditTraillib auditlog,
                        IReportCreationSchedulerManager reportCreationSchedulerManager,
                        IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _configuration = configuration;
            _auditlog = auditlog;
            _reportCreationSchedulerManager = reportCreationSchedulerManager;
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
                var isSuccess = await _reportCreationSchedulerManager.GenerateReport();
                await AddAuditLog(isSuccess ? "Process ran successfully" : "Proccess ran failed, For more details, please check audit logs.", isSuccess ? AuditTrailEnum.Event_status.SUCCESS : AuditTrailEnum.Event_status.FAILED);
                //while (true) { }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to run, Error: {ex.Message}");
                await AddAuditLog($"Failed to run, Error: {ex.Message}", AuditTrailEnum.Event_status.FAILED);
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
                Component_name = "Report Creation Scheduler",
                Service_name = "Backend Process",
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