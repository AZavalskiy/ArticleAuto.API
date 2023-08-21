﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using TFAuto.Domain.Service.Configurations;
using TFAuto.Domain.Services.Blob.DTO;

namespace TFAuto.Domain.Services.Blob
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _container;
        private readonly IConfiguration _configuration;

        public BlobService(IConfiguration configuration)
        {
            _configuration = configuration;
            var blobStorageConnectionString = configuration.GetConnectionString("BlobStorageConnectionString");
            var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
            var blobStorageSettings = GetBlobStorageSettings();
            _container = blobServiceClient.GetBlobContainerClient(blobStorageSettings.ContainerId);
        }

        public async ValueTask<IEnumerable<GetFileListResponse>> GetListAsync()
        {
            List<GetFileListResponse> blobs = new List<GetFileListResponse>();

            await foreach (var file in _container.GetBlobsAsync())
            {
                string uri = _container.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                blobs.Add(new GetFileListResponse
                {
                    Uri = fullUri,
                    FileName = name,
                    ContentType = file.Properties.ContentType,
                });
            }
            return blobs;
        }

        public async ValueTask<GetFileResponse> GetAsync(GetFileRequest request)
        {
            BlobClient blob = _container.GetBlobClient(request.FileName);

            bool exists = await blob.ExistsAsync();
            if (!exists)
                throw new ValidationException(ErrorMessages.FILE_NOT_FOUND);

            string uri = _container.Uri.ToString();
            var name = blob.Name;
            var fullUri = $"{uri}/{name}";

            var content = await blob.DownloadContentAsync();
            string contentType = content.Value.Details.ContentType;

            GetFileResponse response = new()
            {
                Uri = fullUri,
                FileName = name,
                ContentType = contentType
            };

            return response;
        }

        public async ValueTask<UploadFileResponse> UploadAsync(UploadFileRequest request)
        {
            if (request == null || request.File == null)
                throw new ArgumentException(ErrorMessages.FILE_OR_REQUEST_INVALID);

            var blobStorageSettings = GetBlobStorageSettings();

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(request.File.FileName);
            string storageFileName = $"{fileNameWithoutExtension}-{Guid.NewGuid()}";

            BlobClient blob = _container.GetBlobClient(storageFileName);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await request.File.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var blobUploadOptions = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = blobStorageSettings.ContentType
                    }
                };

                await blob.UploadAsync(memoryStream, blobUploadOptions);
            }

            UploadFileResponse response = new()
            {
                Status = $"File {request.File.FileName} uploaded Successfully",
                Error = false,
                Uri = blob.Uri.AbsoluteUri,
                FileName = blob.Name
            };

            return response;
        }

        public async ValueTask<DownloadFileResponse> DownloadAsync(DownloadFileRequest request)
        {
            BlobClient blob = _container.GetBlobClient(request.FileName);

            bool exists = await blob.ExistsAsync();
            if (!exists)
                throw new ValidationException(ErrorMessages.FILE_NOT_FOUND);

            var blobStorageSettings = GetBlobStorageSettings();

            if (await blob.ExistsAsync())
            {
                var content = await blob.DownloadAsync();
                Stream fileContent = content.Value.Content;

                string name = $"{request.FileName}{blobStorageSettings.DowloadFileExtensions}";
                string contentType = content.Value.Details.ContentType;

                var message = new DownloadFileResponse { Content = fileContent, FileName = name, ContentType = contentType };
                return message;
            }
            return null;
        }

        public async ValueTask<DeleteFileResponse> DeleteAsync(DeleteFileRequest request)
        {
            BlobClient blob = _container.GetBlobClient(request.FileName);

            bool exists = await blob.ExistsAsync();
            if (!exists)
                throw new ValidationException(ErrorMessages.FILE_NOT_FOUND);

            await blob.DeleteAsync();

            var message = new DeleteFileResponse { Error = false, Status = $"File: {request.FileName} has been successfully deleted." };
            return message;
        }

        private BlobStorageSettings GetBlobStorageSettings()
        {
            return _configuration.GetSection("BlobStorageSettings").Get<BlobStorageSettings>();
        }
    }
}