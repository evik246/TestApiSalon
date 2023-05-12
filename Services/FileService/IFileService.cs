using TestApiSalon.Dtos.File;
using TestApiSalon.Dtos.Other;

namespace TestApiSalon.Services.FileService
{
    public interface IFileService
    {
        string StoragePath { get; set; }
        Task<Result<IEnumerable<UploadFileResultDto>>> UploadFiles(IEnumerable<IFormFile> files);
        Task<Result<Stream>> DownloadFile(string storedFileName);
    }
}
