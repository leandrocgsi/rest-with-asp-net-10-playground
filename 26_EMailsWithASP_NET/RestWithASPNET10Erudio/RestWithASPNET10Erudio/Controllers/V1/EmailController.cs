using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Services;

namespace RestWithASPNET10Erudio.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]/v1")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SendEmail([FromBody] EmailRequestDTO emailRequest)
        {
            _logger.LogInformation("Request received to send simple e-mail to {To}", emailRequest.To);

            _emailService.SendSimpleMail(emailRequest.To, emailRequest.Subject, emailRequest.Body);

            return Ok("E-mail sent successfully!");
        }

        [HttpPost("with-attachment")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendEmailWithAttachment(
            [FromForm] EmailRequestDTO emailRequest,
            [FromForm] IFormFile attachment)
        {
            _logger.LogInformation("Request received to send e-mail with attachment to {To}", emailRequest.To);

            await _emailService.SendEmailWithAttachmentAsync(emailRequest, attachment);

            return Ok("E-mail with attachment sent successfully!");
        }
    }
}
