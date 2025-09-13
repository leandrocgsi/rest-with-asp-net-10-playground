using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Services;
using System.Text.Json;

namespace RestWithASPNET10Erudio.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]/v1")]
    [Authorize("Bearer")]
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
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult SendEmail([FromBody] EmailRequestDTO emailRequest)
        {
            _logger.LogInformation("Request received to send simple e-mail to {To}", emailRequest.To);

            _emailService.SendSimpleMail(emailRequest.To, emailRequest.Subject, emailRequest.Body);

            return Ok("E-mail sent successfully!");
        }

        [HttpPost("with-attachment")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SendEmailWithAttachment(
            [FromForm] string emailRequest,
            [FromForm] FileUploadDTO attachment)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Desserializar o JSON
            var emailRequestDto = JsonSerializer.Deserialize<EmailRequestDTO>(emailRequest, options);
            if (emailRequestDto == null)
            {
                return BadRequest("Invalid email request JSON.");
            }

            _logger.LogInformation("Request received to send e-mail with attachment to {To}", emailRequestDto.To);

            await _emailService.SendEmailWithAttachmentAsync(emailRequestDto, attachment.File);

            return Ok("E-mail with attachment sent successfully!");
        }
    }
}
