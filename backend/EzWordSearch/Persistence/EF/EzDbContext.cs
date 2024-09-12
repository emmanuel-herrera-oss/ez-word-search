using EzWordSearch.Domain.Matches;
using EzWordSearch.Domain.Players;
using Microsoft.EntityFrameworkCore;

namespace EzWordSearch.Persistence.EF
{
    public class EzDbContext : DbContext
    {
        public EzDbContext(DbContextOptions ctx) : base(ctx)
        {
        }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<MatchPlayer> MatchPlayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PlayerId comes from IdP
            modelBuilder.Entity<Player>().HasKey(p => p.PlayerId);
            modelBuilder.Entity<Player>().Property(p => p.PlayerId).ValueGeneratedNever();

            // MatchId generated when match is created and played. 
            modelBuilder.Entity<Match>().HasKey(p => p.MatchId);
            modelBuilder.Entity<Match>().Property(p => p.MatchId).ValueGeneratedNever();
        }
    }
}
