using MMALSharp.Native;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static MMALSharp.Common.MMALEncoding;

namespace PICamera.Shared.Models
{
    public class Configuration
    {
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConfigurationId { get; set; }

        public string Name { get; set; }

        public string Directory { get; set; }

        public string Prefix { get; set; } = null;

        public string Interval { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string TimeZone { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public int? RecordingDuration { get; set; } = null;

        public MMAL_PARAM_MIRROR_T Rotation { get; set; }

        public EncodingType Encoding { get; set; }

        public bool Enabled { get; set; } = true;

        public Guid ConfigurationGuid { get; set; }
    }
}
