using Microsoft.AspNetCore.Mvc;
using PICamera.Shared.Attributes;
using PICamera.Shared.Models;
using PICamera.Shared.Services;

namespace PICamera.Service.Controllers
{
    [Route("config")]
    [ApiController]
    [AdminAuthorize]
    [ApiVersion("1.0")]
    public class ConfigurationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationService _storage;

        public ConfigurationController(
            IConfiguration configuration,
            ILogger<ConfigurationController> logger,
            IConfigurationService storage)
        {
            _configuration = configuration;
            _logger = logger;
            _storage = storage;
        }

        /// <summary>
        /// Get configurations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Produces("application/json")]
        public async Task<IActionResult> GetConfigurations()
        {
            Configuration[] configurations = await _storage.GetConfigurationsAsync();

            return Ok(configurations);
        }

        /// <summary>
        /// Get configuration by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetConfiguration(Guid id)
        {
            Configuration configuration = await _storage.GetConfigurationAsync(id);

            return Ok(configuration);
        }

        /// <summary>
        /// Create a new configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateConfiguration([FromBody] ConfigurationDto configuration)
        {
            Configuration created = await _storage.CreateConfigurationAsync(configuration);

            return Ok(created);
        }

        /// <summary>
        /// Update a configuration by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateConfiguration(Guid id, [FromBody] ConfigurationDto configuration)
        {
            bool updated = await _storage.UpdateConfigurationAsync(id, configuration);

            return updated ? Ok() : BadRequest();
        }

        /// <summary>
        /// Delete configuration by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteConfiguration(Guid id)
        {
            bool deleted = await _storage.DeleteConfigurationAsync(id);

            return deleted ? Ok() : BadRequest();
        }

        /// <summary>
        /// Get the next run time based off the configuration.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/next")]
        [Produces("application/json")]
        public async Task<IActionResult> GetNextRun(Guid id)
        {
            DateTime next = await _storage.GetNextRunAsync(id);

            return Ok(new { NextRun = next });
        }

        /// <summary>
        /// Get the time of sunrise and sunet based off the configuration.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/sunrise-sunset")]
        [Produces("application/json")]
        public async Task<IActionResult> GetSunriseSunset(Guid id)
        {
            (DateTimeOffset sunrise, DateTimeOffset sunset) = await _storage.GetSunriseSunset(id);

            return Ok(new { sunrise, sunset });
        }
    }
}