using EzWordSearch.Domain.Players;
using EzWordSearch.Service.Contract;

namespace EzWordSearch.Persistence.Contract
{
    public interface IPlayerRepository
    {
        public Task<Player> GetOrCreateAsync(IdentityServiceUser user);
        public Task SaveChangesAsync(Player player);
        public Task<List<Player>> GetTopAsync(int n);

    }
}
