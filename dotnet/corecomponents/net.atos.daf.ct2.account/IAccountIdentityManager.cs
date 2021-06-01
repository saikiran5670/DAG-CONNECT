using System.Threading.Tasks;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.identity.entity;

namespace net.atos.daf.ct2.account
{
    public interface IAccountIdentityManager
    {
        Task<AccountIdentity> Login(Identity user);
        Task<AccountToken> GenerateToken(Identity user);
        Task<AccountToken> GenerateTokenGUID(Identity user);
        Task<bool> ValidateToken(string token);
        Task<ValidTokenResponse> ValidateTokenGuid(string token);
        Task<bool> LogoutByJwtToken(string token);
        Task<bool> LogoutByAccountId(int accountId);
        Task<bool> LogoutByTokenId(string tokenid);
        Task<SSOToken> GenerateSSOToken(TokenSSORequest email);
        Task<SSOResponse> ValidateSSOToken(string tokenGuid);
    }
}
