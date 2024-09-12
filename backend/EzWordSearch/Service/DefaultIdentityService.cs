using EzWordSearch.Service.Contract;
using System.Security.Claims;

namespace EzWordSearch.Service
{
    public class DefaultIdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DefaultIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IdentityServiceUser User
        {
            get
            {
                return new IdentityServiceUser
                {
                    Id = Guid.Parse(GetRequiredClaim("sub")),
                    Username = GetRequiredClaim("preferred_username"),
                    Email = GetClaim("email")
                };
            }
        }
        private string GetRequiredClaim(string claimType)
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue(claimType);
            if(string.IsNullOrEmpty(claim))
            {
                throw new Exception($"Missing claim: ${claimType}");
            }
            return claim;
        }
        private string? GetClaim(string claimType)
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue(claimType);
            if (string.IsNullOrEmpty(claim))
            {
                return null;
            }
            return claim;
        }
    }
}
