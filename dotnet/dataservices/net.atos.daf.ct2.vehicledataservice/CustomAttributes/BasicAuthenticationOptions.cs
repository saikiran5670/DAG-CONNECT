﻿using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.vehicledataservice.CustomAttributes
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string ApplicationName { get; set; }
    }
}
