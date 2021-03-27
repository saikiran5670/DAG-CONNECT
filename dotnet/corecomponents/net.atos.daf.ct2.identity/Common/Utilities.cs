using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace net.atos.daf.ct2.identity.Common
{
    public static class IdentityUtilities
    {
        public static bool ValidationByRegex(Regex regEx, string valueToTest)
        {
            return regEx.IsMatch(valueToTest);
        }
    }
}
