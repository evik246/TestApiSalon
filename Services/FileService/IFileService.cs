using TestApiSalon.Dtos;

namespace TestApiSalon.Services.FileService
{
    public interface IFileService
    {
        string StoragePath { get; set; }
        Task<IEnumerable<UploadFileResultDto>> UploadFiles(IEnumerable<IFormFile> files);
        Task<Stream?> DownloadFile(string storedFileName);
    }
}
