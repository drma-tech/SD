namespace SD.WEB.Modules.Auth.Core
{
    public class GravatarApi(IHttpClientFactory factory) : ApiExternal(factory)
    {
        public async Task<Gravatar?> GetGravatar(string? email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            try
            {
                var root = await GetAsync<GravatarRoot>($"https://en.gravatar.com/{email.GenerateHash()}.json", null);
                return root?.entry.LastOrDefault();
            }
            catch (Exception)
            {
                return new()
                {
                    displayName = email.Split("@")[0],
                    photos = [new Photo { value = $"https://en.gravatar.com/avatar/{email.GenerateHash()}?d=retro" }]
                };
            }
        }
    }
}