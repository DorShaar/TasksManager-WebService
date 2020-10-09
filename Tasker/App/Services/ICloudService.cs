using System.IO;
using System.Threading.Tasks;

namespace Tasker.App.Services
{
    public interface ICloudService
    {
        Task<bool> Upload(string destinationDirectory = default);
        Task<Stream> Download(string fileName);
    }
}