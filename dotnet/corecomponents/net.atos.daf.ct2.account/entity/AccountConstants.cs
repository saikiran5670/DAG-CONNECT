using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.account.entity
{
    public static class AccountConstants
    {
        public const string Error_Invalid_Grant = "invalid_grant";
        public const string Error_Pwd_Expired = "Account is not fully set up";
        public const string Error_Reset_Token_NotFound = "Token not generated, Please contact DAF system Admin";
    }
}
