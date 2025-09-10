using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Mail;

namespace RestWithASPNET10Erudio.Services.Impl
{
    public class EmailServiceImpl : IEmailService
    {
        private readonly EmailSender _emailSender;
        private readonly ILogger<EmailServiceImpl> _logger;

        public EmailServiceImpl(
            EmailSender emailSender,
            ILogger<EmailServiceImpl> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public void SendSimpleMail(
            string to, string subject, string body)
        {
            _logger.LogInformation(
                "Preparing to send simple e-mail to {To}", to);

            _emailSender
                .To(to)
                .WithSubject(subject)
                .WithMessage(body)
                .Send();
        }

        public async Task SendEmailWithAttachmentAsync(EmailRequestDTO emailRequest, IFormFile attachment)
        {
            if (attachment == null || attachment.Length == 0)
            {
                throw new ArgumentException("Attachment is missing.");
            }

            string tempFilePath = Path.GetTempFileName();

            try
            {
                // Copia o conteúdo do IFormFile para o arquivo temporário
                await using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }

                // Envia o e-mail
                _emailSender
                    .To(emailRequest.To)
                    .WithSubject(emailRequest.Subject)
                    .WithMessage(emailRequest.Body)
                    .Attach(tempFilePath)
                    .Send();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending e-mail with attachment to {To}", emailRequest.To);
                throw;
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
    }
}
