using Microsoft.AspNetCore.Mvc;
using RestWithASPNETErudio.Data.DTO;
using RestWithASPNETErudio.Services;

namespace RestWithASPNET10Erudio.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]/v1")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public AuthController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("signin")]
        [ProducesResponseType(200, Type = typeof(TokenDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] UserDTO userCredentials)
        {
            if (userCredentials == null ||
                string.IsNullOrEmpty(userCredentials.Username) ||
                string.IsNullOrEmpty(userCredentials.Password))
            {
                return BadRequest("Usuário e senha precisam ser informados!");
            }

            var token = _loginService.ValidateCredentials(userCredentials);

            if (token == null) return Unauthorized();

            return Ok(token);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(200, Type = typeof(TokenDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Refresh([FromBody] TokenDTO tokenDto)
        {
            if (tokenDto == null)
            {
                return BadRequest("Token inválido!");
            }

            var token = _loginService.ValidateCredentials(tokenDto);

            if (token == null) return Unauthorized();

            return Ok(token);
        }

        [HttpGet("revoke")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public IActionResult Revoke()
        {
            var username = User?.Identity?.Name;

            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var result = _loginService.RevokeToken(username);

            if (!result) return BadRequest("Não foi possível revogar o token.");

            return NoContent();
        }
    }
}
