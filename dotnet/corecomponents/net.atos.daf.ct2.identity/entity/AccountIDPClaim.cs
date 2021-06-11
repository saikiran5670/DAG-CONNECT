using System.Collections.Generic;

namespace net.atos.daf.ct2.identity.entity
{
    public class AccountIDPClaim
    {
        public double ValidTo { get; set; }
        public double IssuedAt { get; set; }
        public string Id { get; set; }
        public string Issuer { get; set; }
        public string Subject { get; set; }
        public string Audience { get; set; }
        public string AuthorizedParty { get; set; }
        public string Sessionstate { get; set; }
        public string TokenType { get; set; }
        public string Algorithm { get; set; }
        public string AlgoType { get; set; }
        public string Sid { get; set; }
        public int TokenExpiresIn { get; set; }
        public string Email { get; set; }
        public List<AccountAssertion> Assertions { get; set; }

        // validto:/*exp*/
        // IssuedAt:/*iat*/
        // id=/*jti*/
        // issuer:/*iss*/
        // subject:/*sub*/
        // audience:/*aud*/
        // tokentype:/*typ*/
        // claims: new Claim[] {
        //     new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
        //     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //     new Claim(nameof(claims.FirstName), claims.FirstName),
        //     new Claim(nameof(claims.LastName), claims.LastName),
        //     new Claim(nameof(claims.Email), claims.Email)
        // },
    }
}