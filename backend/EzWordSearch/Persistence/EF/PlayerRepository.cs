using EzWordSearch.Domain.Players;
using EzWordSearch.Persistence.Contract;
using EzWordSearch.Service.Contract;
using Microsoft.EntityFrameworkCore;

namespace EzWordSearch.Persistence.EF
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly EzDbContext _ctx;
        public PlayerRepository(EzDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<Player> GetOrCreateAsync(IdentityServiceUser user)
        {
            var player = await _ctx.Players.FirstOrDefaultAsync(p => p.PlayerId == user.Id);
            if (player != null) return player;
            player = new Player
            {
                PlayerId = user.Id,
                Name = user.Username
            };
            _ctx.Players.Add(player);
            await _ctx.SaveChangesAsync();
            return player;
        }

        public async Task SaveChangesAsync(Player player)
        {
            await _ctx.SaveChangesAsync();
        }

        public async Task<List<Player>> GetTopAsync(int n)
        {
            return await _ctx
                .Players
                .Where(i => (i.Wins + i.Losses + i.Ties) > 0)
                .OrderByDescending(i => i.Wins)
                .ThenByDescending(i => i.Wins + i.Losses + i.Ties)
                .ThenByDescending(i => i.LastMatch ?? DateTime.MinValue)
                .Take(n)
                .ToListAsync();
        }
    }
}
