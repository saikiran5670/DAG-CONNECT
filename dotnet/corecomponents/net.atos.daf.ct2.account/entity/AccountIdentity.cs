using System.Collections.Generic;

namespace net.atos.daf.ct2.account.entity
{
    public class AccountIdentity
    {
        public string TokenIdentifier { get; set; }
        public Account AccountInfo { get; set; }
        public List<KeyValue> AccountOrganization { get; set; }
        public List<AccountOrgRole> AccountRole { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusCode { get; set; }
        public ExpiryToken Token { get; set; }
    }

    public class ExpiryToken
    {
        public ExpiryToken(string processToken)
        {
            ProcessToken = processToken;
        }
        public string ProcessToken { get; set; }
    }
}
