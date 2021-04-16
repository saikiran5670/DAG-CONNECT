using net.atos.daf.ct2.email.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.email

{
    public interface IEmailNotificationPasswordExpiryManager
    {
        Task<IEnumerable<EmailList>> SendEmailForPasswordExpiry(int noOfDays);
    }
}