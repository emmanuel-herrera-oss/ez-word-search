using EzWordSearch.Domain.Matches;
using EzWordSearch.Persistence.Contract;
using Microsoft.EntityFrameworkCore;

namespace EzWordSearch.Persistence.EF
{
    public class MatchRepository : IMatchRepository
    {
        private readonly EzDbContext _ctx;
        public MatchRepository(EzDbContext ctx)
        {
            _ctx = ctx;
        }
        public void Add(Match match)
        {
            foreach (var i in match.Players)
            {
                _ctx.Update(i.Player);
            }
            _ctx.Add(match);
        }

        public async Task<List<Match>> GetMatchesByPlayerIdAsync(Guid playerId)
        {
            // TODO: Pagination
            return await _ctx
                .Matches
                .Include(m => m.Players)
                .ThenInclude(p => p.Player)
                .Where(m => m.Players.Any(i => i.Player.PlayerId == playerId))
                .OrderByDescending(m => m.StartTime)
                .Take(100)
                .ToListAsync();
        }

        public void SaveChanges()
        {
            _ctx.SaveChanges();
        }
    }
}
