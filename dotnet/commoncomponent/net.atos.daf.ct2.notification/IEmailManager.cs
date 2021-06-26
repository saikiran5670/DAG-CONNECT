using System.Threading.Tasks;
using net.atos.daf.ct2.notification.entity;

namespace net.atos.daf.ct2.notification
{
    public interface IEmailManager
    {
        Task<bool> TriggerSendEmail(MailNotificationRequest mailNotificationRequest);
    }
}