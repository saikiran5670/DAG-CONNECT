using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.email;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.applications
{
    public class PasswordExpiryWorker : BackgroundService
    {
        private readonly ILogger<PasswordExpiryWorker> _logger;
        private readonly IAuditTraillib _auditlog;
        private readonly IEmailNotificationPasswordExpiryManager _emailNotificationPasswordExpiryManager;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _configuration;

        public PasswordExpiryWorker(ILogger<PasswordExpiryWorker> logger,
                        IConfiguration configuration,
                        IAuditTraillib auditlog,
                        IEmailNotificationPasswordExpiryManager emailNotificationPasswordExpiryManager,
                        IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _configuration = configuration;
            _auditlog = auditlog;
            _emailNotificationPasswordExpiryManager = emailNotificationPasswordExpiryManager;
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

                var emailList = await _emailNotificationPasswordExpiryManager.SendEmailForPasswordExpiry(Convert.ToInt32(_configuration["PasswordExpiryInDays"]));
                var isPartial = emailList.Where(w => w.IsSend == false).Count() > 0;
                await _auditlog.AddLogs(new AuditTrail
                {
                    Created_at = DateTime.Now,
                    Performed_at = DateTime.Now,
                    Performed_by = 2,
                    Component_name = "Email Notication Pasword Expiry",
                    Service_name = "Backend Process",
                    Event_type = AuditTrailEnum.Event_type.Mail,
                    Event_status = isPartial ? AuditTrailEnum.Event_status.PARTIAL : AuditTrailEnum.Event_status.SUCCESS,
                    Message = isPartial ? "Email send was partially successful. Please check audit log for more info." : "Email send to all users",
                    Sourceobject_id = 0,
                    Targetobject_id = 0,
                    Updated_data = "EmailNotificationForPasswordExpiry"
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
                    Component_name = "Email Notication Pasword Expiry",
                    Service_name = "Backend Process",
                    Event_type = AuditTrailEnum.Event_type.Mail,
                    Event_status = AuditTrailEnum.Event_status.FAILED,
                    Message = $"Failed to send email. with error : {ex.Message}",
                    Sourceobject_id = 0,
                    Targetobject_id = 0,
                    Updated_data = "EmailNotificationForPasswordExpiry"
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
                    Component_name = "Email Notication Pasword Expiry",
                    Service_name = "Backend Process",
                    Event_type = AuditTrailEnum.Event_type.Mail,
                    Event_status = AuditTrailEnum.Event_status.FAILED,
                    Message = $"Failed to send email. with error : {ex.Message}",
                    Sourceobject_id = 0,
                    Targetobject_id = 0,
                    Updated_data = "EmailNotificationForPasswordExpiry"
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
