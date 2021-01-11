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
        public AccountToken CreateToken(AccountCustomClaims claims)
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

            var jwt = new JwtSecurityToken(
                audience: _settings.Audience,
                issuer: _settings.Issuer,
                claims: new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(nameof(claims.FirstName), claims.FirstName),
                    new Claim(nameof(claims.LastName), claims.LastName),
                    new Claim(nameof(claims.Email), claims.Email)
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
        public string DecodeToken(string token)
        {

            var handler = new JwtSecurityTokenHandler();
            var decodedValue = handler.ReadJwtToken(token);
            
            return decodedValue.ToString();
        }
    }
    public static class TypeConverterExtension
    {
        public static byte[] ToByteArray(this string value) =>
         Convert.FromBase64String(value);
    }
}