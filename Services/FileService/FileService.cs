using System.Net;
using TestApiSalon.Dtos.File;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Exceptions;

namespace TestApiSalon.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;

        public string StoragePath { get; set; }

        public FileService(IWebHostEnvironment environment, ILogger<IFileService> logger)
        {
            _environment = environment;
            _logger = logger;
            StoragePath = Path.Combine(_environment.ContentRootPath,"Files", "Uploads");
        }

        public async Task<Result<IEnumerable<UploadFileResultDto>>> UploadFiles(IEnumerable<IFormFile> files)
        {
            List<UploadFileResultDto> result = new();
            foreach (var file in files)
            {
                string trustedFileNameForStorage = Path.GetRandomFileName();
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(file.FileName);

                var path = Path.Combine(StoragePath, trustedFileNameForStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                UploadFileResultDto uploadFile = new()
                {
                    StoredFileName = trustedFileNameForStorage,
                    FileName = trustedFileNameForDisplay,
                    Uploaded = true
                };
                result.Add(uploadFile);

                _logger.LogInformation($"Upload file path: {path}");
            }
            return new Result<IEnumerable<UploadFileResultDto>>(result);
        }

        public async Task<Result<Stream>> DownloadFile(string storedFileName)
        {
            try
            {
                var path = Path.Combine(StoragePath, storedFileName);
                var memory = new MemoryStream();

                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return new Result<Stream>(memory);
            }
            catch (FileNotFoundException)
            {
                _logger.LogError($"File {storedFileName} not found");
                return new Result<Stream>(new NotFoundException("File is not found"));
            }
        }
    }
}
