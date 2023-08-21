using TFAuto.Domain.Services.Blob.DTO;

namespace TFAuto.Domain.Services.Blob
{
    public interface IBlobService
    {
        ValueTask<IEnumerable<GetFileListResponse>> GetListAsync();

        ValueTask<GetFileResponse> GetAsync(GetFileRequest request);

        ValueTask<UploadFileResponse> UploadAsync(UploadFileRequest request);

        ValueTask<DownloadFileResponse> DownloadAsync(DownloadFileRequest request);

        ValueTask<DeleteFileResponse> DeleteAsync(DeleteFileRequest request);
    }
}