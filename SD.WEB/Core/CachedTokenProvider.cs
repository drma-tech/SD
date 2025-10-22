using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SD.WEB.Core
{
    public class CachedTokenProvider(IAccessTokenProvider tokenProvider, IConfiguration configuration)
    {
        private AccessToken? _cachedToken;
        private DateTimeOffset _expiresAt;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        public async Task<AccessToken?> GetTokenAsync()
        {
            if (_cachedToken != null && DateTimeOffset.UtcNow < _expiresAt)
                return _cachedToken;

            await _refreshLock.WaitAsync();
            try
            {
                if (_cachedToken != null && DateTimeOffset.UtcNow < _expiresAt)
                    return _cachedToken;

                var result = await tokenProvider.RequestAccessToken();

                if (!result.TryGetToken(out var token))
                {
                    result = await tokenProvider.RequestAccessToken(new AccessTokenRequestOptions
                    {
                        Scopes = ["openid", "email", configuration["DownstreamApi:Scopes"] ?? throw new UnhandledException("Scopes null")],
                        ReturnUrl = "/"
                    });

                    if (!result.TryGetToken(out token))
                        return null;
                }

                _cachedToken = token;
                _expiresAt = token.Expires.AddSeconds(-30);
                return token;
            }
            finally
            {
                _refreshLock.Release();
            }
        }
    }
}