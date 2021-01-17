using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using net.atos.daf.ct2.identity.entity;
namespace net.atos.daf.ct2.identity
{
    public class TokenManager : ITokenManager
    {
        private readonly IdentityJsonConfiguration _settings;
        public TokenManager(IOptions<IdentityJsonConfiguration> setting)
        {
            _settings = setting.Value;
        }
        public AccountToken CreateToken(AccountIDPClaim customclaims)
        {
            var privateKey = _settings.RsaPrivateKey.ToByteArray();
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);
            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
            var now = DateTime.Now;
            var unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();
            List<Claim> claimList = new List<Claim>();
            if (!String.IsNullOrEmpty(customclaims.ValidTo))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Exp, customclaims.ValidTo));
            }
            if (!String.IsNullOrEmpty(customclaims.IssuedAt))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Iat, customclaims.IssuedAt));
            }
            if (!String.IsNullOrEmpty(customclaims.Id))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Jti, customclaims.Id));
            }
            if (!String.IsNullOrEmpty(customclaims.Issuer))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Iss, customclaims.Issuer));
            }
            if (!String.IsNullOrEmpty(customclaims.Subject))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Sub, customclaims.Subject));
            }
            if (!String.IsNullOrEmpty(customclaims.Audience))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Aud, customclaims.Audience));
            }
            if (!String.IsNullOrEmpty(customclaims.TokenType))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Typ, customclaims.TokenType));
            }
            if (!String.IsNullOrEmpty(customclaims.Sid))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Sid, customclaims.Sid));
            }
            if (!String.IsNullOrEmpty(customclaims.AuthorizedParty))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Azp, customclaims.AuthorizedParty));
            }

            //  {

            //     new Claim(JwtRegisteredClaimNames.Iat, customclaims.IssuedAt, ClaimValueTypes.Integer64),
            //     new Claim(JwtRegisteredClaimNames.Jti, customclaims.Id),
            //     new Claim(JwtRegisteredClaimNames.Iss, customclaims.Issuer),
            //     new Claim(JwtRegisteredClaimNames., customclaims.),
            //     new Claim(JwtRegisteredClaimNames., customclaims.),
            //     new Claim(JwtRegisteredClaimNames., customclaims.),
            //     new Claim(JwtRegisteredClaimNames., customclaims.),
            //     new Claim(JwtRegisteredClaimNames., customclaims.)
            // };
            foreach (var assertion in customclaims.Assertions)
            {
                if (!String.IsNullOrEmpty(assertion.Value))
                {
                    claimList.Add(new Claim(assertion.Key, assertion.Value));
                }
            }

            var jwt = new JwtSecurityToken(
                claims: claimList.ToArray(),
                notBefore: now,
                expires: now.AddMinutes(15),
                signingCredentials: signingCredentials
            );
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new AccountToken
            {
                AccessToken = token,
                ExpiresIn = 15,
            };
        }
        public bool ValidateToken(string token)
        {
            var publicKey = _settings.RsaPublicKey.ToByteArray();
            //   AccountIDPClaim customclaims = DecodeToken(token);
            using RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(_settings.RsaPublicKey), out _);
            //rsa.ImportRSAPublicKey(publicKey, out _);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _settings.Issuer,
                ValidAudience = _settings.Audience,
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };
            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, validationParameters, out var validatedSecurityToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public AccountIDPClaim DecodeToken(string jwtInput)
        {
            AccountIDPClaim customclaims = new AccountIDPClaim();
            List<AccountAssertion> assertionList = new List<AccountAssertion>();
            AccountAssertion assertion;
            var handler = new JwtSecurityTokenHandler();
            var readable = handler.CanReadToken(jwtInput);

            if (readable == true)
            {
                var jwtToken = handler.ReadJwtToken(jwtInput);
                var headers = jwtToken.Header;
                foreach (var h in headers)
                {
                    switch (h.Key.ToString())
                    {
                        case "alg":
                            customclaims.Algorithm = h.Value.ToString();
                            break;
                        case "typ":
                            customclaims.AlgoType = h.Value.ToString();
                            break;
                        case "sid":
                            customclaims.Sid = h.Value.ToString();
                            break;
                    }
                    // jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
                }
                var claims = jwtToken.Claims;
                foreach (Claim c in claims)
                {
                    switch (c.Type)
                    {
                        /*Jwt Registered ClaimNames */
                        case "exp":
                            customclaims.ValidTo = c.Value;
                            break;
                        case "iat":
                            customclaims.IssuedAt = c.Value;
                            break;
                        case "jti":
                            customclaims.Id = c.Value;
                            break;
                        case "iss":
                            customclaims.Issuer = c.Value;
                            break;
                        case "sub":
                            customclaims.Subject = c.Value;
                            break;
                        case "aud":
                            customclaims.Audience = c.Value;
                            break;
                        case "typ":
                            customclaims.TokenType = c.Value;
                            break;
                        case "azp":
                            customclaims.AuthorizedParty = c.Value;
                            break;
                        /*User defined account specific ClaimNames */
                        default:
                            assertion = new AccountAssertion();
                            assertion.Key = c.Type;
                            assertion.Value = c.Value;
                            assertion.SessionState = "";
                            assertion.AccountId = "";
                            assertion.CreatedAt = "";
                            assertionList.Add(assertion);
                            break;
                    }
                    // jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";
                }
                customclaims.Assertions = assertionList;
            }
            else
            {
                customclaims = new AccountIDPClaim();
            }
            // code to extract token and bind claim & assestoin object
            // claim.
            //return decodedValue.ToString();
            // var claims= new Claim[] {
            //         new Claim(JwtRegisteredClaimNames.Exp, customclaims.ValidTo),
            //         new Claim(JwtRegisteredClaimNames.Iat, "unixTimeSeconds.ToString()", ClaimValueTypes.Integer64),
            //         new Claim(JwtRegisteredClaimNames.Jti, customclaims.Id),
            //         new Claim(JwtRegisteredClaimNames.Iss, customclaims.Issuer),
            //         new Claim(JwtRegisteredClaimNames.Sub, customclaims.Subject),
            //         new Claim(JwtRegisteredClaimNames.Aud, customclaims.Audience),
            //         new Claim(JwtRegisteredClaimNames.Typ, customclaims.TokenType),
            //         new Claim(JwtRegisteredClaimNames.Sid, customclaims.Sid),
            //         new Claim(JwtRegisteredClaimNames.Azp, customclaims.AuthorizedParty)
            // };
            return customclaims;
        }
        public AccountIDPClaim DecodeOLD(string jwtInput)
        {
            AccountIDPClaim customclaims = new AccountIDPClaim();
            List<AccountAssertion> assertionList = new List<AccountAssertion>();

            return customclaims;
        }
    }
    public static class TypeConverterExtension
    {
        public static byte[] ToByteArray(this string value) =>
         Convert.FromBase64String(value);
    }
}