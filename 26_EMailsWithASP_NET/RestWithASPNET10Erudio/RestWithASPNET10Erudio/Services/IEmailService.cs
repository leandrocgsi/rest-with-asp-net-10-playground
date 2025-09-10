using RestWithASPNET10Erudio.Data.DTO.V1;

namespace RestWithASPNET10Erudio.Services
{
    public interface IEmailService
    {
        void SendSimpleMail(string to, string subject, string body);
        Task SendEmailWithAttachmentAsync(EmailRequestDTO emailRequest, IFormFile attachment);
    }
}
