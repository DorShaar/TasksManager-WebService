using System.Threading.Tasks;

namespace Tasker.App.Services
{
    public interface ICloudService
    {
        Task<bool> Upload(string destinationDirectory = default);
    }
}