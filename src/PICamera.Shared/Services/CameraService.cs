using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using NCrontab;
using System.Linq;
using PICamera.Shared.Models;
using static MMALSharp.Common.MMALEncoding;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace PICamera.Shared.Services
{
    public interface ICameraService
    {
        Task<string> TakeInMemoryPictureAsync(Guid id);

        Task<bool> TakePictureAsync(Guid id);

        Task<bool> TakeVideoAsync(Guid id, int seconds);

        Task<string[]> GetFilesAsync(Guid id, string search = null);

        Task<byte[]> GetFileAsync(Guid id, string name);
    }

    public class CameraService : ICameraService
    {
        private readonly IConfigurationService _config;

        public CameraService(IConfigurationService config) => _config = config;

        public async Task<byte[]> GetFileAsync(Guid id, string name)
        {
            string picture = (await GetFilesAsync(id, name)).FirstOrDefault();

            if (!string.IsNullOrEmpty(picture))
                return File.ReadAllBytes(picture);

            return default;
        }

        public async Task<string[]> GetFilesAsync(Guid id, string search = null)
        {
            Configuration configuration = await _config.GetConfigurationAsync(id);

            if (!string.IsNullOrEmpty(configuration.Directory))
            {
                if (Directory.Exists(configuration.Directory))
                {
                    return !string.IsNullOrEmpty(search) ?
                        Directory.GetFiles(configuration.Directory).Where(path => path.ToLower().Contains(search.ToLower())).ToArray() :
                        Directory.GetFiles(configuration.Directory);
                }
            }

            return default;
        }

        public async Task<string> TakeInMemoryPictureAsync(Guid id)
        {
            Configuration configuration = await _config.GetConfigurationAsync(id);

            if (configuration.Encoding == EncodingType.Image)
            {
                MMALCamera camera = MMALCamera.Instance;

                MMALCameraConfig.StillResolution = new Resolution(configuration.Width, configuration.Height);
                MMALCameraConfig.Flips = configuration.Rotation;

                using InMemoryCaptureHandler handler = new();

                await camera.TakePicture(handler, JPEG, I420);

                camera.Cleanup();

                byte[] raw = handler.WorkingData.ToArray();

                string data = Convert.ToBase64String(raw);

                return data;
            }

            return default;
        }

        public async Task<bool> TakePictureAsync(Guid id)
        {
            Configuration configuration = await _config.GetConfigurationAsync(id);

            if (configuration.Encoding == EncodingType.Image)
            {
                if (!Directory.Exists(configuration.Directory))
                    Directory.CreateDirectory(configuration.Directory);

                MMALCamera camera = MMALCamera.Instance;

                MMALCameraConfig.StillResolution = new Resolution(configuration.Width, configuration.Height);
                MMALCameraConfig.Flips = configuration.Rotation;

                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(configuration.TimeZone);

                var timestamp = new DateTimeOffset(DateTime.Now, zone.GetUtcOffset(DateTime.Now));

                string prefix = !string.IsNullOrEmpty(configuration.Prefix) ? $"{configuration.Prefix}-" : "";

                using var handler = new ImageStreamCaptureHandler($"{configuration.Directory}/{prefix}{timestamp.ToUnixTimeSeconds()}.jpg");

                await camera.TakePicture(handler, JPEG, I420);

                camera.Cleanup();

                return true;
            }

            return false;
        }

        public async Task<bool> TakeVideoAsync(Guid id, int seconds)
        {
            Configuration configuration = await _config.GetConfigurationAsync(id);

            if (configuration.Encoding == EncodingType.Video)
            {
                if (!Directory.Exists(configuration.Directory))
                    Directory.CreateDirectory(configuration.Directory);

                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(configuration.TimeZone);

                var timestamp = new DateTimeOffset(DateTime.Now, zone.GetUtcOffset(DateTime.Now));

                string prefix = !string.IsNullOrEmpty(configuration.Prefix) ? $"{configuration.Prefix}-" : "";

                string path = $"{configuration.Directory}/{prefix}{timestamp.ToUnixTimeSeconds()}";

                int duration = configuration.RecordingDuration.HasValue ? 
                    configuration.RecordingDuration.Value != 0 ? configuration.RecordingDuration.Value : 10 : 10;

                ProcessStartInfo raspivid = new() { FileName = "/usr/bin/raspivid", Arguments = $"-o {path}.h264 -fps 30 -h {configuration.Height} -w {configuration.Width} -t {1000 * (duration + 1)}", };
                Process raspividProcess = new() { StartInfo = raspivid };

                raspividProcess.Start();
                raspividProcess.WaitForExit();

                ProcessStartInfo ffmpeg = new() { FileName = "/usr/bin/ffmpeg", Arguments = $"-framerate 30 -i {path}.h264 -c copy {path}.mp4" };
                Process ffmpegProcess = new() { StartInfo = ffmpeg };

                ffmpegProcess.Start();
                ffmpegProcess.WaitForExit();

                if (File.Exists($"{path}.h264"))
                    File.Delete($"{path}.h264");

                return true;
            }

            return false;
        }
    }
}
