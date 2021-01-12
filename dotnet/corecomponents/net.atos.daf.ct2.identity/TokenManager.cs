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
    public class TokenManager:ITokenManager
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

            // customclaims.
            // customclaims.
            // customclaims.
            // customclaims.

            var jwt = new JwtSecurityToken(
                audience: customclaims.Audience,/*aud*/
                issuer: customclaims.Issuer,/*iss*/
                // validto:customclaims.ValidTo,/*exp*/
                // IssuedAt:customclaims.IssuedAt,/*iat*/
                // id=customclaims.Id,/*jti*/
                // subject:customclaims.Subject,/*sub*/
                claims: new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Exp, customclaims.ValidTo),
                    new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Jti, customclaims.Id),
                    new Claim(JwtRegisteredClaimNames.Iss, customclaims.Issuer),
                    new Claim(JwtRegisteredClaimNames.Sub, customclaims.Subject),
                    new Claim(JwtRegisteredClaimNames.Aud, customclaims.Audience),
                    new Claim(JwtRegisteredClaimNames.Typ, customclaims.TokenType),
                    new Claim(JwtRegisteredClaimNames.Sid, customclaims.Sid),
                    new Claim(JwtRegisteredClaimNames.Azp, customclaims.AuthorizedParty)
                },
                notBefore: now,
                expires: now.AddSeconds(60),
                signingCredentials: signingCredentials
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new AccountToken
            {
                AccessToken = token,
                ExpiresIn = 30,
            };
        }
        public bool ValidateToken(string token)
        {

            var publicKey = _settings.RsaPublicKey.ToByteArray();

            using RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(_settings.RsaPublicKey), out _);
            //rsa.ImportRSAPublicKey(publicKey, out _);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
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

            // var handler = new JwtSecurityTokenHandler();
            // var decodedValue = handler.ReadJwtToken(token);
            string JwtOut;
            var jwtHandler = new JwtSecurityTokenHandler();

            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(jwtInput);

            if(readableToken != true)
            {
            JwtOut = "The token doesn't seem to be in a proper JWT format.";
            }
            if(readableToken == true)
            {
            var token = jwtHandler.ReadJwtToken(jwtInput);
                
            //Extract the headers of the JWT
            var headers = token.Header;
            var jwtHeader = "{";
            foreach(var h in headers)
            {
                jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
            }
            jwtHeader += "}";
            JwtOut = "Header:\r\n" + JToken.Parse(jwtHeader).ToString(Formatting.Indented);

            //Extract the payload of the JWT
            var claims = token.Claims;
            var jwtPayload = "{";
            foreach(Claim c in claims)
            {
                jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";
            }
            jwtPayload += "}";
            JwtOut += "\r\nPayload:\r\n" + JToken.Parse(jwtPayload).ToString(Formatting.Indented);  
            }

            AccountIDPClaim customclaims=new AccountIDPClaim();
            AccountAssertion assertion=new AccountAssertion(); 
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
    }
    public static class TypeConverterExtension
    {
        public static byte[] ToByteArray(this string value) =>
         Convert.FromBase64String(value);
    }
}