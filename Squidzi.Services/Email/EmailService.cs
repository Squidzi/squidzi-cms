using Squidzi.Services.Email.Contracts;
using Squidzi.Infrastructure.Configuration;
using Squidzi.Domain.SendGridRepo.Contracts;
using Squidzi.Models.ServiceModels;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Squidzi.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly ISendGridRepo _sendGridRepo;
        private readonly EmailSettings _settings;

        public EmailService(ISendGridRepo sendGridRepo, IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
            _sendGridRepo = sendGridRepo;
        }

        public async Task<SendContactEmailResponse> SendContactEmail(SendContactEmailRequest request)
        {
            if (_settings.IsEnabled)
            {
                //todo: this is a terrible way, rather use an html template
                await _sendGridRepo.SendSystemEmail($"New Message from {request.Name}",
                    "<html><head></head><body>" +
                    $"<h2><b>From:</b> {request.Name}</h2>" +
                    $"<h4><b>Email:</b> {request.Email}</h4>" +
                    $"<h4><b>Mobile:</b> {request.MobileNumber}</h4>" +
                    $"<p>{request.Message}</p>" +
                    "</body></html>");
            }
            return new SendContactEmailResponse();
        }
    }
}
