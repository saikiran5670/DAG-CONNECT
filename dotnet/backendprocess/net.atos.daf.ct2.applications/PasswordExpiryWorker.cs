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

namespace net.atos.daf.ct2.applications
{
    public class PasswordExpiryWorker : BackgroundService
    {
        private readonly ILogger<PasswordExpiryWorker> _logger;
        private readonly IAuditTraillib _auditlog;
        private readonly IAccountManager _accountManager;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _configuration;

        public PasswordExpiryWorker(ILogger<PasswordExpiryWorker> logger,
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
                var calcDayToSendMail = Convert.ToInt32(_configuration["PasswordExpiry:NumberOfDays"]) - Convert.ToInt32(_configuration["PasswordExpiry:RemainingDaysToExpire"]);
                var emailList = await _accountManager.SendEmailForPasswordExpiry(calcDayToSendMail);
                var totalCount = emailList.Count();
                var failureCount = emailList.Where(w => w.IsSend == false).Count();
                var isPartial = (failureCount > 0) && ((totalCount - failureCount) > 0);
                var isFailed = (failureCount > 0) && ((totalCount - failureCount) == 0);
                await _auditlog.AddLogs(new AuditTrail
                {
                    Created_at = DateTime.Now,
                    Performed_at = DateTime.Now,
                    Performed_by = 2,
                    Component_name = "Email Notification Password Expiry",
                    Service_name = "Backend Process",
                    Event_type = AuditTrailEnum.Event_type.Mail,
                    Event_status = isPartial ? AuditTrailEnum.Event_status.PARTIAL : isFailed ? AuditTrailEnum.Event_status.FAILED : AuditTrailEnum.Event_status.SUCCESS,
                    Message = isPartial ? "Email send was partially successful. Please check audit log for more info." : isFailed ? "Email send Failed" : $"Email send process run successfully with success count :{totalCount - failureCount}",
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
                    Component_name = "Email Notification Password Expiry",
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
                    Component_name = "Email Notification Password Expiry",
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
