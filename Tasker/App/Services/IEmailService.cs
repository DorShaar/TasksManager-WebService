using System.Threading.Tasks;

namespace Tasker.App.Services
{
    public interface IEmailService
    {
        Task SendEmail(string mailBody);
    }
}