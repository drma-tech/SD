using System.Text;

namespace SD.Shared.Helper
{
    public static class UriParameterHelper
    {
        public static string ConfigureParameters(this string uri, Dictionary<string, string> parameters)
        {
            if (!parameters.Any()) return uri;

            var sb = new StringBuilder(uri);
            for (int i = 0; i < parameters.Count; i++)
            {
                var item = parameters.ElementAt(i);

                if (i == 0)
                    sb.Append($"?{item.Key}={item.Value}");
                else
                    sb.Append($"&{item.Key}={item.Value}");
            }

            return sb.ToString();
        }
    }
}