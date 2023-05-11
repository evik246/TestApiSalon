using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestApiSalon.Extensions;
using TestApiSalon.Services.FileService;

namespace TestApiSalon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IEnumerable<IFormFile> files)
        {
            var fileNames = await _fileService.UploadFiles(files);
            return fileNames.MakeResponse();
        }

        [HttpGet("{storedFileName}")]
        public async Task<IActionResult> GetFile(string storedFileName)
        {
            var file = await _fileService.DownloadFile(storedFileName);
            return file.MakeFileResponse(this);
        }
    }
}
