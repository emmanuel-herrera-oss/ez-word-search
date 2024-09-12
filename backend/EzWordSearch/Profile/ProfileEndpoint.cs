using EzWordSearch.Persistence.Contract;
using EzWordSearch.Service.Contract;

namespace EzWordSearch.Profile
{
    public static class ProfileEndpoint
    {
        public static async Task<IResult> GetProfile(IIdentityService identityService, IPlayerRepository playerRepository)
        {
            var player = await playerRepository.GetOrCreateAsync(identityService.User);
            if(player.Name != identityService.User.Username)
            {
                player.Name = identityService.User.Username;
                await playerRepository.SaveChangesAsync(player);
            }
            return Results.Ok(player);
        }
    }
}
