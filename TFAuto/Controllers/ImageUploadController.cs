using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.Domain.Services.Blob;
using TFAuto.Domain.Services.Blob.DTO;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Route("images")]

    public class ImageUploadController : ControllerBase
    {
        private readonly IBlobService _imageService;

        public ImageUploadController(IBlobService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetFileListResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetFileListResponse>> GetList()
        {
            var images = await _imageService.GetListAsync();
            return Ok(images);
        }

        [HttpGet]
        [Route("filename")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetFileResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetFileResponse>> Get([FromQuery] GetFileRequest request)
        {
            var image = await _imageService.GetAsync(request);
            return Ok(image);
        }

        [HttpPost]
        [Route("upload")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UploadFileResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<UploadFileResponse>> Upload([FromForm] UploadFileRequest request)
        {
            var image = await _imageService.UploadAsync(request);
            return Ok(image);
        }

        [HttpGet]
        [Route("download")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<IActionResult> Download([FromQuery] DownloadFileRequest request)
        {
            var image = await _imageService.DownloadAsync(request);
            return File(image.Content, image.ContentType, image.FileName);
        }

        [HttpDelete]
        [Route("filename")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(DeleteFileResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<DeleteFileResponse>> Delete([FromQuery] DeleteFileRequest request)
        {
            var image = await _imageService.DeleteAsync(request);
            return Ok(image);
        }
    }
}