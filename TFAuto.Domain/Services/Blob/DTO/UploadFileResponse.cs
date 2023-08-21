namespace TFAuto.Domain.Services.Blob.DTO
{
    public class UploadFileResponse
    {
        public string Uri { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public Stream Content { get; set; }

        public string Status { get; set; }

        public bool Error { get; set; }
    }
}