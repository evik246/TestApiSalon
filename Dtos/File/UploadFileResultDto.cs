namespace TestApiSalon.Dtos.File
{
    public class UploadFileResultDto
    {
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
    }
}
