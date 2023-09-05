using Microsoft.AspNetCore.Http;

namespace PICamera.Shared.Extensions
{
    public static class HttpContextItemsExtension
    {
        public static bool TryParseItem<T>(this HttpContext context, string key, out T item)
        {
            item = (T)context.Items[key];

            return item != null;
        }
    }
}
