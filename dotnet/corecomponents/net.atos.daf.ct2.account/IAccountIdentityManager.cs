using System;
using System.Threading.Tasks;
using net.atos.daf.ct2.identity;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;

namespace net.atos.daf.ct2.account
{
    public interface IAccountIdentityManager
    {
       Task<AccountIdentity> Login(Identity user);
       Task<bool> ValidateToken(string token);
    }
}
