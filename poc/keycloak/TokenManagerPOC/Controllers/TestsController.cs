using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TokenManagerPOC.Controllers
{
    [Route("api/v1/[controller]")]
    public class TestsController : ControllerBase
    {
        private readonly ITokenManager _TokenManager;
        public TestsController(ITokenManager TokenManager)
        {
            _TokenManager = TokenManager;
        }

        [HttpPost]
        [Route("token")]
        [ProducesResponseType(typeof(string), Status200OK)]
        public IActionResult GenerateToken()
        {

            var claims = new AccountCustomClaims
            {
                FirstName = "Vynn",
                LastName = "Durano",
                Email = "whatever@email.com"
            };

            var jwt = _TokenManager.CreateToken(claims);

 //           var link = _TokenManager.GenerateLink(jwt.Token);

            return Ok(jwt);
        }

        [HttpPost]
        [Route("token/validate")]
        [ProducesResponseType(typeof(string), Status200OK)]
        public IActionResult ValidateToken([FromBody] string token)
        {

            if (_TokenManager.ValidateToken(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                var claims = new AccountCustomClaims
                {
                    FirstName = jwtToken.Claims.First(claim => claim.Type == "FirstName").Value,
                    LastName = jwtToken.Claims.First(claim => claim.Type == "LastName").Value,
                    Email = jwtToken.Claims.First(claim => claim.Type == "Email").Value
                };

                return Ok(claims);
            }

            return BadRequest("Token is invalid.");
        }
        
        [HttpPost]
        [Route("token/decode")]
        [ProducesResponseType(typeof(string), Status200OK)]
        public IActionResult DecodeToken([FromBody] string token)
        {
            return Ok(_TokenManager.DecodeToken(token));
        }
    }
}