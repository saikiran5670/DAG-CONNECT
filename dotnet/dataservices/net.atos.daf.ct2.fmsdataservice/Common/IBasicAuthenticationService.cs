﻿using System.Threading.Tasks;

namespace net.atos.daf.ct2.fmsdataservice.Common
{
    public interface IBasicAuthenticationService
    {
        Task<string> ValidateTokenGuid(string token);
    }
}
