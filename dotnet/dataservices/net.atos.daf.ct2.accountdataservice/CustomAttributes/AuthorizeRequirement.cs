﻿using Microsoft.AspNetCore.Authorization;

namespace net.atos.daf.ct2.accountdataservice.CustomAttributes
{
    public class AuthorizeRequirement : IAuthorizationRequirement
    {
        public string FeatureName { get; set; }
        public AuthorizeRequirement(string featureName)
        {
            FeatureName = featureName;
        }
    }
}
