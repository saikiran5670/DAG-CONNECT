using net.atos.daf.ct2.account.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.singlesignonservice.Common
{
   public interface IBasicAuthenticationService
    {
        Task<string> ValidateTokenGuid(string token);
    }
}
