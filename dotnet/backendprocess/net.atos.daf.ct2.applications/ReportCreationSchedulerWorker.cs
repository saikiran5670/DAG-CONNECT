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
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.applications
{
    internal class ReportCreationSchedulerWorker : BackgroundService
    {
        private readonly ILogger<PasswordExpiryWorker> _logger;
        private readonly IAuditTraillib _auditlog;
        private readonly IAccountManager _accountManager;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _configuration;

        public ReportCreationSchedulerWorker(ILogger<PasswordExpiryWorker> logger,
                        IConfiguration configuration,
                        IAuditTraillib auditlog,
                        IAccountManager accountManager,
                        IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _configuration = configuration;
            _auditlog = auditlog;
            _accountManager = accountManager;
            _hostApplicationLifetime = hostApplicationLifetime;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
            {
                _logger.LogInformation("Ending the process...");
            });
            try
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await _auditlog.AddLogs(new AuditTrail
                {
                    Created_at = DateTime.Now,
                    Performed_at = DateTime.Now,
                    Performed_by = 2,
                    Component_name = "Report Scheduler",
                    Service_name = "Backend Process",
                    Event_type = AuditTrailEnum.Event_type.Mail,
                    Event_status = AuditTrailEnum.Event_status.SUCCESS,
                    Message = $"Email send process run successfully",
                    Sourceobject_id = 0,
                    Targetobject_id = 0,
                    Updated_data = "Report Scheduler"
                });
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}