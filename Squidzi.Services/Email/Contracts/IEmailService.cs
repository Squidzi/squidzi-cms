using Squidzi.Models.ServiceModels;
using System.Threading.Tasks;

namespace Squidzi.Services.Email.Contracts
{
    public interface IEmailService
    {
        Task<SendContactEmailResponse> SendContactEmail(SendContactEmailRequest request);
    }
}
