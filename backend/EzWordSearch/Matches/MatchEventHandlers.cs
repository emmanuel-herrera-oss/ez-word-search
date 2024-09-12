using EzWordSearch.Domain.Matches;
using EzWordSearch.Domain.Matches.Events;
using EzWordSearch.Persistence.EF;
using Microsoft.AspNetCore.SignalR;

namespace EzWordSearch.Matches
{
    public class MatchEventHandlers
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public MatchEventHandlers(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void MatchStarted(object? sender, MatchStartedEventArgs e)
        {
            SendToAll(sender, e);
        }
        public void WaitingToStart(object? sender, WaitingToStartEventArgs e)
        {
            SendToAll(sender, e);
        }
        public void WaitingForPlayers(object? sender, WaitingForPlayersEventArgs e)
        {
            SendToAll(sender, e);
        }
        public void MatchEnded(object? sender, MatchEndedEventArgs e)
        {
            SendToAll(sender, e);
            var match = sender as Match ?? throw new Exception($"Unable to cast to match.");
            if (match.Players.Count > 1)
            {
                using var scope = _scopeFactory.CreateScope();
                using var ctx = scope.ServiceProvider.GetRequiredService<EzDbContext>();
                var matchRepository = new MatchRepository(ctx);
                matchRepository.Add(match);
                matchRepository.SaveChanges();
            }
        }
        public void MoveMade(object? sender, MoveMadeEventArgs e)
        {
            SendToAll(sender, e);
        }
        public void PlayerLeft(object? sender, PlayerLeftEventArgs e)
        {
            SendToAll(sender, e);
        }
        public void PlayerJoined(object? sender, PlayerJoinedEventArgs e)
        {
            SendToAll(sender, e);
        }
        public void Error(object? sender, WordSearchErrorEventArgs e)
        {
            SendToAll(sender, e);
        }
        private void SendToAll(object? obj, WordSearchEventArgs e)
        {
            using var scope = _scopeFactory.CreateScope();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<MatchHub>>();
            var match = obj as Match ?? throw new Exception($"Unable to cast to match.");
            foreach (var player in match.Players)
            {
                hubContext.Clients.User(player.Player.PlayerId.ToString()).SendAsync("ReceiveMessage", e);
            }
        }
    }
}
