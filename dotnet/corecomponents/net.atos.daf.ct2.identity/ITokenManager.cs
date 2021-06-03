using System.Threading.Tasks;
using net.atos.daf.ct2.identity.entity;
namespace net.atos.daf.ct2.identity
{
    public interface ITokenManager
    {
        AccountToken CreateToken(AccountIDPClaim customclaims);
        Task<bool> ValidateToken(string token);
        AccountIDPClaim DecodeToken(string jwtInput);
        AccountIDPClaim DecodeOLD(string jwtInput);
    }

}
