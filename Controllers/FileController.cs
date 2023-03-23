using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
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
        public async Task<ActionResult<IEnumerable<string>>> UploadFiles(IEnumerable<IFormFile> files)
        {
            var fileNames = await _fileService.UploadFiles(files);
            return Ok(fileNames);
        }

        [HttpGet("images/{storedFileName}")]
        public async Task<ActionResult> GetFile(string storedFileName)
        {
            var file = await _fileService.DownloadFile(storedFileName);
            return File(file, MediaTypeNames.Image.Jpeg);
        }
    }
}
