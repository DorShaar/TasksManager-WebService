using System.Threading.Tasks;

namespace Tasker.App.Services
{
    public interface IArchiverService
    {
        string ArchiveExtension { get; }
        Task<int> Extract(string archivePath, string directoryToExtract, string password);
        Task<int> Pack(string directoryToPack, string packedArchivePath, string password);
    }
}