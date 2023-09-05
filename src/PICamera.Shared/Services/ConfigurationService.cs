using Microsoft.EntityFrameworkCore;
using NCrontab;
using Newtonsoft.Json;
using PICamera.Shared.Context;
using PICamera.Shared.Models;
using System;
using System.Globalization;
using System.Text;

namespace PICamera.Shared.Services
{
    public interface IConfigurationService
    {
        Task<Configuration[]> GetConfigurationsAsync();

        Task<Configuration> GetConfigurationAsync(Guid configurationGuid);

        Task<Configuration> CreateConfigurationAsync(ConfigurationDto create);

        Task<bool> UpdateConfigurationAsync(Guid id, ConfigurationDto update);

        Task<bool> DeleteConfigurationAsync(Guid id);

        Task<DateTime> GetNextRunAsync(Guid id);

        Task<(DateTimeOffset sunrise, DateTimeOffset sunset)> GetSunriseSunset(Guid id);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly StorageContext _context;

        public ConfigurationService(StorageContext context) => _context = context;

        public async Task<Configuration> CreateConfigurationAsync(ConfigurationDto create)
        {
            Configuration configuration = new()
            {
                Name = create.Name,
                Directory = create.Directory,
                Prefix = create.Prefix,
                Interval = create.Interval,
                Width = create.Width,
                Height = create.Height,
                TimeZone = create.TimeZone,
                Longitude = create.Longitude,
                Latitude = create.Latitude,
                Rotation = create.Rotation,
                RecordingDuration = create.RecordingDuration,
                Encoding = create.Encoding,
                Enabled = create.Enabled,
                ConfigurationGuid = Guid.NewGuid()
            };

            await _context.Configurations.AddAsync(configuration);

            await _context.SaveChangesAsync();

            return configuration;
        }

        public async Task<bool> DeleteConfigurationAsync(Guid id)
        {
            Configuration configuration = await GetConfigurationAsync(id);

            if (configuration != null)
            {
                _context.Remove(configuration);

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<Configuration> GetConfigurationAsync(Guid configurationGuid) => await _context.Configurations.FirstOrDefaultAsync(config => config.ConfigurationGuid == configurationGuid);

        public async Task<Configuration[]> GetConfigurationsAsync() => await _context.Configurations.ToArrayAsync();

        public async Task<bool> UpdateConfigurationAsync(Guid id, ConfigurationDto update)
        {
            Configuration found = await GetConfigurationAsync(id);

            if (found != null)
            {
                if (!string.IsNullOrEmpty(update.Name))
                    found.Name = update.Name;

                if (!string.IsNullOrEmpty(update.Directory))
                    found.Directory = update.Directory;

                if (!string.IsNullOrEmpty(update.Prefix))
                    found.Prefix = update.Prefix;

                if (!string.IsNullOrEmpty(update.Interval))
                    found.Interval = update.Interval;

                if (update.Width != 0)
                    found.Width = update.Width;

                if (update.Height != 0)
                    found.Height = update.Height;

                if (update.Longitude != 0)
                    found.Longitude = update.Longitude;

                if (update.Latitude != 0)
                    found.Latitude = update.Latitude;

                if (!string.IsNullOrEmpty(update.TimeZone))
                    found.TimeZone = update.TimeZone;

                if (update.Rotation != found.Rotation)
                    found.Rotation = update.Rotation;

                if (update.Encoding != found.Encoding)
                    found.Encoding = update.Encoding;

                if (update.Enabled != found.Enabled)
                    found.Enabled = update.Enabled;

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<DateTime> GetNextRunAsync(Guid id)
        {
            Configuration configuration = await GetConfigurationAsync(id);

            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(configuration.TimeZone);

            CrontabSchedule schedule = CrontabSchedule.Parse(configuration.Interval);

            DateTime next = schedule.GetNextOccurrence(TimeZoneInfo.ConvertTime(DateTime.Now, zone));

            return next;
        }

        public async Task<(DateTimeOffset sunrise, DateTimeOffset sunset)> GetSunriseSunset(Guid id)
        {
            Configuration configuration = await GetConfigurationAsync(id);

            using HttpClient client = new() { BaseAddress = new Uri("https://api.sunrise-sunset.org") };

            HttpResponseMessage response = await client.GetAsync($"/json?lat={configuration.Latitude}&lng={configuration.Longitude}&formatted=0");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                SunriseSunset sunriseSunset = JsonConvert.DeserializeObject<SunriseSunset>(json);

                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(configuration.TimeZone);

                DateTimeOffset sunrise = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset.Parse(sunriseSunset.Results.Sunrise), configuration.TimeZone);

                DateTimeOffset sunset = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset.Parse(sunriseSunset.Results.Sunset), configuration.TimeZone);

                return (sunrise, sunset);
            }

            return default;
        }
    }
}
