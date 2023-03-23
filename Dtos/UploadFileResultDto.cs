namespace TestApiSalon.Dtos
{
    public class UploadFileResultDto
    {
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
    }
}
