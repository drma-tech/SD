using System.Security.Cryptography;
using System.Text;

namespace SD.Shared.Core.Models
{
    public class Gravatar
    {
        public string? hash { get; set; }
        public string? requestHash { get; set; }
        public string? profileUrl { get; set; }
        public string? preferredUsername { get; set; }
        public string? thumbnailUrl { get; set; }
        public List<Photo> photos { get; set; } = [];
        public string? displayName { get; set; }
        public List<object> urls { get; set; } = [];
        public ShareFlags? share_flags { get; set; }
    }

    public class Photo
    {
        public string? value { get; set; }
        public string? type { get; set; }
    }

    public class GravatarRoot
    {
        public List<Gravatar> entry { get; set; } = [];
    }

    public class ShareFlags
    {
        public bool search_engines { get; set; }
    }

    public static class GravatarUtils
    {
        public static string GenerateHash(this string randomString)
        {
            var hash = new StringBuilder();
            byte[] crypto = SHA256.HashData(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}