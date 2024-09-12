namespace EzWordSearch.Service.Contract
{
    public interface IIdentityService
    {
        public IdentityServiceUser User { get; }
    }
    public class IdentityServiceUser
    {
        public Guid Id { get; init; }
        public required string Username { get; init; }
        public string? Email { get; init; }
    }
}
