using Microsoft.AspNetCore.SignalR;

namespace EzWordSearch.Service
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User.Identity?.Name;
        }
    }
}
