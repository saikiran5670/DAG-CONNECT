using System.Threading.Tasks;
using net.atos.daf.ct2.notification.entity;

namespace net.atos.daf.ct2.notification.repository
{
    public interface IEmailRepository
    {
        Task<string> GetLanguageCodePreference(string v, int? orgId);
        Task<NotificationAccount> GetAccountByEmailId(string emailId);
    }
}