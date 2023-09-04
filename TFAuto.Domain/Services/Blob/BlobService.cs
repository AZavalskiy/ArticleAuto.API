﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using TFAuto.Domain.Configurations;
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
            _container = blobServiceClient.GetBlobContainerClient(blobStorageSettings.ContainerName);
        }

        public async ValueTask<GetFileResponse> GetAsync(string request)
        {
            BlobClient blob = _container.GetBlobClient(request);

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

        public async ValueTask<UploadFileResponse> UploadAsync(IFormFile request)
        {
            if (request == null)
                throw new ArgumentException(ErrorMessages.FILE_OR_REQUEST_INVALID);

            var blobStorageSettings = GetBlobStorageSettings();

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(request.FileName);
            string storageFileName = $"{fileNameWithoutExtension}-{Guid.NewGuid()}";

            BlobClient blob = _container.GetBlobClient(storageFileName);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await request.CopyToAsync(memoryStream);
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
                Message = $"File {request.FileName} uploaded Successfully",
                Success = true,
                Uri = blob.Uri.AbsoluteUri,
                FileName = blob.Name
            };

            return response;
        }

        public async ValueTask<UpdateFileResponse> UpdateAsync(string existingFileNameRequest, IFormFile updatedFileRequest)
        {
            BlobClient blob = _container.GetBlobClient(existingFileNameRequest);

            bool exists = await blob.ExistsAsync();

            if (!exists)
                throw new ValidationException(ErrorMessages.FILE_NOT_FOUND);

            if (updatedFileRequest != null)
            {
                var blobStorageSettings = GetBlobStorageSettings();

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(updatedFileRequest.FileName);
                string storageFileName = $"{fileNameWithoutExtension}-{Guid.NewGuid()}";

                await blob.DeleteAsync();

                blob = _container.GetBlobClient(storageFileName);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await updatedFileRequest.CopyToAsync(memoryStream);
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
            }

            UpdateFileResponse response = new()
            {
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

            var content = await blob.DownloadAsync();
            Stream fileContent = content.Value.Content;

            string name = $"{request.FileName}{blobStorageSettings.DowloadFileExtensions}";
            string contentType = content.Value.Details.ContentType;

            var message = new DownloadFileResponse
            {
                Content = fileContent,
                FileName = name,
                ContentType = contentType
            };

            return message;
        }

        public async ValueTask<DeleteFileResponse> DeleteAsync(string request)
        {
            BlobClient blob = _container.GetBlobClient(request);

            bool exists = await blob.ExistsAsync();

            if (!exists)
                throw new ValidationException(ErrorMessages.FILE_NOT_FOUND);

            await blob.DeleteAsync();

            var message = new DeleteFileResponse { Success = true, Message = $"File: {request} has been successfully deleted." };
            return message;
        }

        private BlobStorageSettings GetBlobStorageSettings()
        {
            return _configuration.GetSection("BlobStorageSettings").Get<BlobStorageSettings>();
        }
    }
}