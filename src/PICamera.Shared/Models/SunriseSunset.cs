using Newtonsoft.Json;

namespace PICamera.Shared.Models
{
    public class Results
    {
        [JsonProperty("sunrise")]
        public string Sunrise;

        [JsonProperty("sunset")]
        public string Sunset;

        [JsonProperty("solar_noon")]
        public string SolarNoon;

        [JsonProperty("day_length")]
        public string DayLength;

        [JsonProperty("civil_twilight_begin")]
        public string CivilTwilightBegin;

        [JsonProperty("civil_twilight_end")]
        public string CivilTwilightEnd;

        [JsonProperty("nautical_twilight_begin")]
        public string NauticalTwilightBegin;

        [JsonProperty("nautical_twilight_end")]
        public string NauticalTwilightEnd;

        [JsonProperty("astronomical_twilight_begin")]
        public string AstronomicalTwilightBegin;

        [JsonProperty("astronomical_twilight_end")]
        public string AstronomicalTwilightEnd;
    }

    public class SunriseSunset
    {
        [JsonProperty("results")]
        public Results Results;

        [JsonProperty("status")]
        public string Status;
    }


}
