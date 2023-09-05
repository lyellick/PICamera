using Microsoft.AspNetCore.Mvc;

namespace PICamera.Service.Controllers
{
    [Route("health")]
    [ApiController]
    [ApiVersion("1.0")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public HealthController(
            IConfiguration configuration,
            ILogger<HealthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to check the health of the api.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult GetHealth() => Ok();
    }
}