using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.email.Repository
{
    public interface IEmailNotificationPasswordExpiryRepository
    {
        Task<IEnumerable<string>> GetEmailOfPasswordExpiry(int noOfDays);
    }
}