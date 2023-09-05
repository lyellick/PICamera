using Microsoft.Extensions.Configuration;

namespace PICamera.Shared.Extensions
{
    public static class IConfigurationExtension
    {
        public static bool TryGetValue(this IConfiguration configuration, string key, out string value)
        {
            if (!string.IsNullOrEmpty(configuration[key]))
            {
                value = configuration[key];

                return true;
            }
            else
            {
                value = null;

                return false;
            }
        }
    }
}
