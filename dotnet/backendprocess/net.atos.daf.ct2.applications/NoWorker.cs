using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;

namespace net.atos.daf.ct2.applications
{
    public class NoWorker : BackgroundService
    {
        private readonly ILogger<PasswordExpiryWorker> _logger;
        private readonly IAuditTraillib _auditlog;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public NoWorker(ILogger<PasswordExpiryWorker> logger,
                        IAuditTraillib auditlog,
                        IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _auditlog = auditlog;
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
                    Component_name = "NoWorker",
                    Service_name = "Backend_Process",
                    Event_type = AuditTrailEnum.Event_type.Mail,
                    Event_status = AuditTrailEnum.Event_status.SUCCESS,
                    Message = "NoWorker process got executed.",
                    Sourceobject_id = 0,
                    Targetobject_id = 0,
                    Updated_data = "NoWorker"
                });
            }
            catch (OperationCanceledException ex)
            {
                // Swallow this since we expect this to occur when shutdown has been signalled. 
                await _auditlog.AddLogs(new AuditTrail
                {
                    Created_at = DateTime.Now,
                    Performed_at = DateTime.Now,
                    Performed_by = 2,
                    Component_name = "NoWorker",
                    Service_name = "Backend Process",
                    Event_type = AuditTrailEnum.Event_type.Mail,
                    Event_status = AuditTrailEnum.Event_status.FAILED,
                    Message = "NoWorker process got executed.",
                    Sourceobject_id = 0,
                    Targetobject_id = 0,
                    Updated_data = "NoWorker"
                });
                _logger.LogError(ex, "An OperationCanceled exception was thrown.");
            }
            catch (Exception ex)
            {
                //await Task.Delay(1000, stoppingToken);
                await _auditlog.AddLogs(new AuditTrail
                {
                    Created_at = DateTime.Now,
                    Performed_at = DateTime.Now,
                    Performed_by = 2,
                    Component_name = "NoWorker",
                    Service_name = "Backend Process",
                    Event_type = AuditTrailEnum.Event_type.Mail,
                    Event_status = AuditTrailEnum.Event_status.FAILED,
                    Message = "NoWorker process got executed.",
                    Sourceobject_id = 0,
                    Targetobject_id = 0,
                    Updated_data = "NoWorker"
                });
                _logger.LogError(ex, "An unhandled exception was thrown.");
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}
