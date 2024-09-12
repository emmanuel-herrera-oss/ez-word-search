using EzWordSearch.Domain.MatchManagement;
using EzWordSearch.Persistence.Contract;
using EzWordSearch.Service.Contract;

namespace EzWordSearch.Matches
{
    public static class MatchEndpoint
    {
        public static async Task<IResult> FindMatch(
            FindMatchRequest request,
            MatchManager mm, 
            IPlayerRepository playerRepository,
            IIdentityService identityService)
        {
            var player = await playerRepository.GetOrCreateAsync(identityService.User);
            var searchResult = mm.FindAvailableMatch(player, request.SinglePlayer ? 1 : 2);
            return Results.Ok(searchResult);
        }
        public static IResult GetPlayerCount(MatchManager mm)
        {
            return Results.Ok(mm.GetPlayerCount());
        }
        public static async Task<IResult> GetHistory(IPlayerRepository playerRepository, IMatchRepository matchRepository, IIdentityService identityService)
        {
            var matches = await matchRepository.GetMatchesByPlayerIdAsync(identityService.User.Id);
            return Results.Ok(matches);
        }
    }
    public class FindMatchRequest
    {
        public bool SinglePlayer { get; set; }
    }
}
