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
        private readonly ILogger<PasswordExpiryWorker> _logger;
        private readonly IAuditTraillib _auditlog;
        private readonly IReportEmailSchedulerManager _reportEmailSchedulerManager;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _configuration;

        public ReportEmailSchedulerWorker(ILogger<PasswordExpiryWorker> logger,
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
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to run, Error: {ex.Message}");
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}