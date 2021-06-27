using System.Threading.Tasks;
using net.atos.daf.ct2.notification.entity;

namespace net.atos.daf.ct2.notification
{
    public interface IEmailNotificationManager
    {
        Task<bool> TriggerSendEmail(MailNotificationRequest mailNotificationRequest);
    }
}