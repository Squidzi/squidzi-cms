using System.Threading.Tasks;

namespace Squidzi.Domain.SendGridRepo.Contracts
{
    public interface ISendGridRepo
    {
        Task SendSystemEmail(string subject, string htmlContent);
    }
}
