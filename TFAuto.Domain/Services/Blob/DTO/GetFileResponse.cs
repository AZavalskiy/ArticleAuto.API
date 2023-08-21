namespace TFAuto.Domain.Services.Blob.DTO
{
    public class GetFileResponse
    {
        public string Id { get; set; }

        public string Uri { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public Stream Content { get; set; }
    }
}