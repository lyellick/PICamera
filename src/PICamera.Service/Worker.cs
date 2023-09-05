using MMALSharp.Common.Utility;
using MMALSharp;
using PICamera.Shared.Services;
using PICamera.Shared.Models;
using static MMALSharp.Common.MMALEncoding;

namespace PICamera.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IServiceProvider _provider;

        public Worker(ILogger<Worker> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                List<Task> tasks = new();

                using IServiceScope scope = _provider.CreateScope();

                IConfigurationService configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();

                ICameraService cameraService = scope.ServiceProvider.GetRequiredService<ICameraService>();

                Configuration[] conigurations = await configurationService.GetConfigurationsAsync();

                _logger.LogInformation($"Running enabled configurations...");

                foreach (Guid id in conigurations.Select(config => config.ConfigurationGuid))
                {
                    Task task = Task.Run(async () =>
                    {
                        Configuration configuration = await configurationService.GetConfigurationAsync(id);

                        (DateTimeOffset sunrise, DateTimeOffset sunset) = await configurationService.GetSunriseSunset(id);

                        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(configuration.TimeZone);

                        DateTime next = TimeZoneInfo.ConvertTime(DateTime.Now, zone);

                        while (!token.IsCancellationRequested)
                        {
                            configuration = await configurationService.GetConfigurationAsync(id);

                            if (configuration.Enabled)
                            {
                                DateTime now = TimeZoneInfo.ConvertTime(DateTime.Now, zone);

                                if (now >= sunrise && now <= sunset)
                                {
                                    if (now >= next)
                                    {
                                        _logger.LogInformation($"[{configuration.Name}] Worker running at: {now:yyyy-MM-dd hh:mm:ss tt}");

                                        switch (configuration.Encoding)
                                        {
                                            case EncodingType.Image:
                                                await cameraService.TakePictureAsync(configuration.ConfigurationGuid);
                                                break;
                                            case EncodingType.Video:
                                                if (configuration.RecordingDuration.HasValue)
                                                    await cameraService.TakeVideoAsync(configuration.ConfigurationGuid, configuration.RecordingDuration.Value);
                                                break;
                                            default:
                                                break;
                                        }

                                        next = await configurationService.GetNextRunAsync(configuration.ConfigurationGuid);
                                    }
                                    else
                                    {
                                        TimeSpan delay = next - now;

                                        _logger.LogInformation($"[{configuration.Name}] Worker waiting till: {next:yyyy-MM-dd hh:mm:ss tt}");

                                        await Task.Delay(delay, token);
                                    }
                                }
                                else
                                {
                                    if (sunrise.Day < now.Day)
                                        (sunrise, sunset) = await configurationService.GetSunriseSunset(id);
                                }
                            }
                            else
                            {
                                _logger.LogInformation($"[{configuration.Name}] Configuration disabled. Waiting for 1 minute to check if state has changed.");
                                await Task.Delay(1000 * 60);

                            }
                        }
                    }, token);

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
            } 
            catch (Exception ex)
            {
                _logger.LogCritical($"Could not start worker service: {ex.Message}");
            }
        }
    }
}
