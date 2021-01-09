using System.Threading.Tasks;
using net.atos.daf.ct2.identity.entity;
namespace net.atos.daf.ct2.identity
{
    public interface ITokenManager
    {
        AccountToken CreateToken(AccountCustomClaims claims);
        bool ValidateToken(string token);
        string DecodeToken(string token);
    }
}