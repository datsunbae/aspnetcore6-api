using api_aspnetcore6.Dtos;

namespace api_aspnetcore6.Services.Interfaces
{
    public interface ISendMailService
    {
        void SendMail(MailMessage message);
        Task SendEmailAsync(MailMessage message);
    }
}