using Microsoft.Extensions.Options;

namespace EzWordSearch.Auth
{
    public static class AuthEndpoint
    {
        public static async Task<IResult> Login(
            KeycloakAuthorizationRequest request,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AuthOptions> optionsSnapshot)
        {
            var options = optionsSnapshot.Value;
            using var client = httpClientFactory.CreateClient();
            Dictionary<string, string> data = new Dictionary<string, string>(5);
            data["grant_type"] = "authorization_code";
            data["client_id"] = options.ClientId;
            data["client_secret"] = options.ClientSecret;
            data["code"] = request.Code;
            data["redirect_uri"] = options.RedirectUri;
            HttpRequestMessage msg = new HttpRequestMessage
            {
                Content = new FormUrlEncodedContent(data),
                Method = HttpMethod.Post,
                RequestUri = new Uri(options.TokenUri)
            };
            var result = await client.SendAsync(msg);
            var loginResult = await result.Content.ReadFromJsonAsync<KeycloakAuthorizationResponse>();
            if(loginResult?.access_token == null || loginResult?.refresh_token == null)
            {
                throw new Exception($"Unable to read token from response. Got status code {result.StatusCode}");
            }
            CookieOptions accessTokenCookieOptions = new CookieOptions
            {
                Domain = options.Domain,
                HttpOnly = true,
                IsEssential = true,
                Path = "/",
                SameSite = SameSiteMode.Strict,
                Secure = options.Secure
            };
            CookieOptions refreshTokenCookieOptions = new CookieOptions
            {
                Domain = options.Domain,
                HttpOnly = true,
                IsEssential = true,
                Path = "/",
                SameSite = SameSiteMode.Strict,
                Secure = options.Secure
            };
            httpContextAccessor.HttpContext?.Response.Cookies.Append(AuthOptions.AccessTokenCookieName, loginResult.access_token, accessTokenCookieOptions);
            httpContextAccessor.HttpContext?.Response.Cookies.Append(AuthOptions.RefreshTokenCookieName, loginResult.refresh_token, refreshTokenCookieOptions);
            return Results.Ok(new LoginResponseModel { RefreshAfter = loginResult.expires_in });
        }
        public static async Task<IResult> Logout(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AuthOptions> optionsSnapshot)
        {
            var options = optionsSnapshot.Value;
            if(httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue("WS_REFRESH_TOKEN", out var refreshToken) ?? false)
            {
                using var client = httpClientFactory.CreateClient();
                Dictionary<string, string?> data = new Dictionary<string, string?>(3);
                data["client_id"] = options.ClientId;
                data["client_secret"] = options.ClientSecret;
                data["refresh_token"] = refreshToken;
                HttpRequestMessage msg = new HttpRequestMessage
                {
                    Content = new FormUrlEncodedContent(data),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(options.LogoutUri)
                };
                var response = await client.SendAsync(msg);
            }
            httpContextAccessor.HttpContext?.Response.Cookies.Delete("WS_ACCESS_TOKEN");
            httpContextAccessor.HttpContext?.Response.Cookies.Delete("WS_REFRESH_TOKEN");
            return Results.NoContent();
        }
    }
    public class KeycloakAuthorizationRequest
    {
        public required string Code { get; set; }
    }
    public class KeycloakAuthorizationResponse
    {
        public required string access_token { get; set; }
        public required string refresh_token { get; set; }
        public long expires_in { get; set; }
        public long refresh_expires_in { get; set; }
    }
    public class LoginResponseModel
    {
        public required long RefreshAfter { get; set; }
    }
}
