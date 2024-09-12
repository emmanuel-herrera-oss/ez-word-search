using EzWordSearch.Domain.Matches;

namespace EzWordSearch.Persistence.Contract
{
    public interface IMatchRepository
    {
        public void Add(Match match);
        public void SaveChanges();
        public Task<List<Match>> GetMatchesByPlayerIdAsync(Guid playerId);
    }
}
