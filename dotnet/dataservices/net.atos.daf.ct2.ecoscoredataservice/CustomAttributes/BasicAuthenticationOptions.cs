﻿using Microsoft.AspNetCore.Authentication;

namespace net.atos.daf.ct2.ecoscoredataservice.CustomAttributes
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string ApplicationName { get; set; }
    }
}
