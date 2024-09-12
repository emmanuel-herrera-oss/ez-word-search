namespace EzWordSearch.Auth
{
    public class AuthOptions
    {
        public const string Name = "Auth";
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string RedirectUri { get; set; }
        public required string TokenUri { get; set; }
        public required string Domain { get; set; }
        public bool Secure { get; set; } = true;
        public const string AccessTokenCookieName = "WS_ACCESS_TOKEN";
        public const string RefreshTokenCookieName = "WS_REFRESH_TOKEN";
        public required string LogoutUri { get; set; }
    }
}
