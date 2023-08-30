using Microsoft.AspNetCore.Http;
using TFAuto.Domain.Services.Blob.DTO;

namespace TFAuto.Domain.Services.Blob
{
    public interface IBlobService
    {
        ValueTask<GetFileResponse> GetAsync(string request);

        ValueTask<UploadFileResponse> UploadAsync(IFormFile request);

        ValueTask<DownloadFileResponse> DownloadAsync(DownloadFileRequest request);

        ValueTask<DeleteFileResponse> DeleteAsync(string request);

        ValueTask<UpdateFileResponse> UpdateAsync(string oldFileNameRequest, IFormFile request);
    }
}