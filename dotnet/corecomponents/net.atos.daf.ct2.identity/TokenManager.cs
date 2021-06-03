using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
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
            AccountToken accountToken = new AccountToken();
            accountToken.ExpiresIn = customclaims.TokenExpiresIn;

            var now = DateTime.Now;
            //long unixTimeSecondsIssueAt = new DateTimeOffset(now).ToUnixTimeSeconds();
            //long unixTimeSecondsExpiresAt = 0;
            //if (customclaims.TokenExpiresIn > 0)
            //{
            //    unixTimeSecondsExpiresAt = new DateTimeOffset(now.AddSeconds(customclaims.TokenExpiresIn)).ToUnixTimeSeconds();
            //}
            var privateKey = _settings.RsaPrivateKey.ToByteArray();
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);
            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            List<Claim> claimList = new List<Claim>();
            if (customclaims.ValidTo > 0)
            {
                //customclaims.ValidTo
                claimList.Add(new Claim(JwtRegisteredClaimNames.Exp, customclaims.ValidTo.ToString()));
            }
            if (customclaims.IssuedAt > 0)
            {
                //customclaims.IssuedAt
                claimList.Add(new Claim(JwtRegisteredClaimNames.Iat, customclaims.IssuedAt.ToString()));
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
                accountToken.TokenType = customclaims.TokenType;
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

            foreach (var assertion in customclaims.Assertions)
            {
                if (!String.IsNullOrEmpty(assertion.Value))
                {
                    //claimList.Add(new Claim(assertion.Key, assertion.Value));

                    switch (assertion.Key.ToString())
                    {
                        case "session_state":
                            accountToken.SessionState = assertion.Value.ToString();
                            claimList.Add(new Claim("session_state", assertion.Value.ToString()));
                            break;
                        case "scope":
                            accountToken.Scope = assertion.Value.ToString();
                            claimList.Add(new Claim("scope", assertion.Value.ToString()));
                            break;
                    }
                }
            }
            claimList.Add(new Claim("email", customclaims.Email));

            var jwt = new JwtSecurityToken(
                claims: claimList.ToArray(),
                notBefore: now,
                expires: now.AddSeconds(accountToken.ExpiresIn),
                signingCredentials: signingCredentials
            );
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);
            accountToken.AccessToken = token;
            return accountToken;
        }
        public async Task<bool> ValidateToken(string token)
        {
            // CryptoProviderFactory.DefaultCacheSignatureProviders = false;
            //var publicKey = _settings.RsaPublicKey.ToByteArray();
            //rsa.ImportRSAPublicKey(publicKey, out _);

            using RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(_settings.RsaPublicKey), out _);
            SecurityKey key = new RsaSecurityKey(rsa)
            {
                CryptoProviderFactory = new CryptoProviderFactory()
                {
                    CacheSignatureProviders = false
                }
            };
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _settings.Issuer,
                IssuerSigningKey = key,
                CryptoProviderFactory = new CryptoProviderFactory()
                {
                    CacheSignatureProviders = false
                }
            };
            IdentityModelEventSource.ShowPII = true;
            try
            {
                handler.ValidateToken(token, validationParameters, out var validatedSecurityToken);
            }
            catch
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
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
                    if (!string.IsNullOrEmpty(c.Value))
                    {
                        switch (c.Type)
                        {
                            /*Jwt Registered ClaimNames */
                            case "exp":
                                customclaims.ValidTo = Convert.ToDouble(c.Value);
                                break;
                            case "iat":
                                customclaims.IssuedAt = Convert.ToDouble(c.Value);
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
                    }
                }
                customclaims.Assertions = assertionList;
            }
            else
            {
                customclaims = new AccountIDPClaim();
            }
            return customclaims;
        }
        public AccountIDPClaim DecodeOLD(string jwtInput)
        {
            AccountIDPClaim customclaims = new AccountIDPClaim();
            List<AccountAssertion> assertionList = new List<AccountAssertion>();

            return customclaims;
        }
        private DateTime ConvertDoubleToDateTime(double utc)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(utc);
            return dateTime;
        }
    }
    public static class TypeConverterExtension
    {
        public static byte[] ToByteArray(this string value) =>
         Convert.FromBase64String(value);
    }

}