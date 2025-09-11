using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNETErudio.Data.DTO;
using RestWithASPNETErudio.Services;

namespace RestWithASPNET10Erudio.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILoginService loginService, ILogger<AuthController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [HttpPost("signin")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(TokenDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] UserDTO userCredentials)
        {
            _logger.LogInformation("Attempting to sign in user {username}", userCredentials?.Username);

            if (userCredentials == null ||
                string.IsNullOrEmpty(userCredentials.Username) ||
                string.IsNullOrEmpty(userCredentials.Password))
            {
                _logger.LogWarning("Invalid credentials provided");
                return BadRequest("Username and password must be informed.");
            }

            var token = _loginService.ValidateCredentials(userCredentials);

            if (token == null)
            {
                _logger.LogWarning("Unauthorized login attempt for user {username}", userCredentials.Username);
                return Unauthorized();
            }

            _logger.LogInformation("User {username} signed in successfully", userCredentials.Username);
            return Ok(token);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(200, Type = typeof(TokenDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Refresh([FromBody] TokenDTO tokenDto)
        {
            _logger.LogInformation("Attempting to refresh token");

            if (tokenDto == null)
            {
                _logger.LogWarning("Invalid token provided for refresh");
                return BadRequest("Invalid token.");
            }

            var token = _loginService.ValidateCredentials(tokenDto);

            if (token == null)
            {
                _logger.LogWarning("Unauthorized token refresh attempt");
                return Unauthorized();
            }

            _logger.LogInformation("Token refreshed successfully");
            return Ok(token);
        }

        [HttpPost("createUser")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(AccountCredentialsDTO))]
        [ProducesResponseType(400)]
        public IActionResult Create([FromBody] AccountCredentialsDTO user)
        {
            if (user == null) return BadRequest("Invalid client request");
            var createdUser = _loginService.Create(user);
            return Ok(createdUser);
        }


        [HttpGet("revoke")]
        [Authorize("Bearer")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Revoke()
        {
            var username = User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Unauthorized revoke attempt");
                return Unauthorized();
            }

            var result = _loginService.RevokeToken(username);

            if (!result)
            {
                _logger.LogError("Failed to revoke token for user {username}", username);
                return BadRequest("Could not revoke the token.");
            }

            _logger.LogInformation("Token revoked successfully for user {username}", username);
            return NoContent();
        }
    }
}
