using System.Net;
using TestApiSalon.Dtos;

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

        public async Task<IEnumerable<UploadFileResultDto>> UploadFiles(IEnumerable<IFormFile> files)
        {
            try
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
                }
                return result;
            }
            catch (IOException ex)
            {
                const string message = "File error on upload";
                _logger.LogError(ex, message);
                throw new IOException(message);
            }
        }

        public async Task<Stream> DownloadFile(string storedFileName)
        {
            var path = Path.Combine(StoragePath, storedFileName);
            var memory = new MemoryStream();

            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return memory;
        }
    }
}
