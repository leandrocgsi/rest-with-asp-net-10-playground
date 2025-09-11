using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Services;
using RestWithASPNETErudio.Data.DTO;
using RestWithASPNETErudio.Services;

namespace RestWithASPNET10Erudio.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IUserAuthService _userAuthService;

        public AuthController(ILoginService loginService, IUserAuthService userAuthService)
        {
            _loginService = loginService;
            _userAuthService = userAuthService;
        }

        [HttpPost("signin")]
        [AllowAnonymous]
        public IActionResult SignIn([FromBody] UserDTO user)
        {
            if (user == null) return BadRequest("Invalid client request");

            var token = _loginService.ValidateCredentials(user);
            if (token == null) return Unauthorized();

            return Ok(token);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public IActionResult Refresh([FromBody] TokenDTO tokenDto)
        {
            if (tokenDto == null) return BadRequest("Invalid client request");

            var token = _loginService.ValidateCredentials(tokenDto);
            if (token == null) return Unauthorized();

            return Ok(token);
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public IActionResult Create([FromBody] AccountCredentialsDTO user)
        {
            if (user == null) return BadRequest("Invalid client request");

            var result = _loginService.Create(user);
            return Ok(result);
        }

        [HttpPost("revoke")]
        [Authorize]
        public IActionResult Revoke()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("Invalid client request");

            var result = _userAuthService.RevokeToken(username);
            if (!result) return BadRequest("Invalid client request");

            return NoContent();
        }
    }
}