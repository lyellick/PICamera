using Microsoft.AspNetCore.Mvc;
using PICamera.Shared.Attributes;
using PICamera.Shared.Services;

namespace PICamera.Service.Controllers
{
    [Route("camera")]
    [ApiController]
    [AdminAuthorize]
    [ApiVersion("1.0")]
    public class CameraController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICameraService _camera;

        public CameraController(
            IConfiguration configuration,
            ILogger<CameraController> logger,
            ICameraService camera)
        {
            _configuration = configuration;
            _logger = logger;
            _camera = camera;
        }

        /// <summary>
        /// Takes a picture and returns the image as base64 string.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [Produces("text/plain")]
        public async Task<IActionResult> TakeInMemoryPictureAsync(Guid id)
        {
            string picture = await _camera.TakeInMemoryPictureAsync(id);

            return Ok(picture);
        }

        /// <summary>
        /// Takes a picture and saves it to the configuration directory.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> TakePictureAsync(Guid id)
        {
            bool successful = await _camera.TakePictureAsync(id);

            return successful ? Ok() : BadRequest();
        }

        /// <summary>
        /// Takes a video and saves it to the configuration directory.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/{seconds}")]
        [Produces("application/json")]
        public async Task<IActionResult> TakeVideoAsync(Guid id, int seconds)
        {
            bool successful = await _camera.TakeVideoAsync(id, seconds);

            return successful ? Ok() : BadRequest();
        }

        /// <summary>
        /// Get paths of files saved.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/files")]
        [Produces("application/json")]
        public async Task<IActionResult> GetFilesAsync(Guid id, [FromQuery] string search)
        {
            string[] files = await _camera.GetFilesAsync(id, search);

            return Ok(files);
        }

        /// <summary>
        /// Get picture.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/picture/{name}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPictureAsync(Guid id, string name)
        {
            try
            {
                byte[] file = await _camera.GetFileAsync(id, name);

                return File(file, "image/jpg", name);
            }
            catch
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Get video.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/video/{name}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetVideoAsync(Guid id, string name)
        {
            try
            {
                byte[] file = await _camera.GetFileAsync(id, name);

                return File(file, "video/mp4", name);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}