using Microsoft.AspNetCore.Http;
using TFAuto.Domain.Services.Blob.DTO;

namespace TFAuto.Domain.Services.Blob
{
    public interface IBlobService
    {
        ValueTask<GetFileResponse> GetAsync(string requestFileName);

        ValueTask<UploadFileResponse> UploadAsync(IFormFile uploadedFile);

        ValueTask<DownloadFileResponse> DownloadAsync(string requestFileName);

        ValueTask<DeleteFileResponse> DeleteAsync(string requestFileName);

        ValueTask<UploadFileResponse> UpdateAsync(string oldFileName, IFormFile request);
    }
}