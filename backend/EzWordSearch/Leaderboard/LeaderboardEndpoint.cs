using EzWordSearch.Persistence.Contract;

namespace EzWordSearch.Leaderboard
{
    public static class LeaderboardEndpoint
    {
        public static async Task<IResult> GetLeaderboard(IPlayerRepository playerRepository)
        {
            return Results.Ok(await playerRepository.GetTopAsync(100));
        }
    }
}
