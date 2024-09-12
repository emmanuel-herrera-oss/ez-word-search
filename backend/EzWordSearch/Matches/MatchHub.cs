using EzWordSearch.Domain.Matches;
using EzWordSearch.Domain.Matches.Events;
using EzWordSearch.Domain.MatchManagement;
using EzWordSearch.Persistence.Contract;
using EzWordSearch.Service.Contract;
using Microsoft.AspNetCore.SignalR;

namespace EzWordSearch.Matches
{
    public class MatchHub : Hub
    {
        private readonly MatchManager _mm;
        private readonly IPlayerRepository _playerRepository;
        private readonly IIdentityService _identityService;
        public MatchHub(
            MatchManager mm,
            IPlayerRepository playerRepository, 
            IIdentityService identityService)
        {
            _mm = mm;
            _playerRepository = playerRepository;
            _identityService = identityService;
        }
        public MatchSnapshot? GetSnapshot()
        {
            var match = _mm.GetMatchByPlayerId(_identityService.User.Id);
            return match?.GetSnapshot();
        }
        public void MakeMove(List<int> start, List<int> end)
        {
            var playerId = _identityService.User.Id;
            var match = _mm.GetMatchByPlayerId(playerId) ?? throw new Exception("Not in a match.");
            match.HandleEvent(new MoveMadeEventArgs { Start = start.ToArray(), End = end.ToArray(), PlayerId = playerId });
        }
        public void LeaveMatch()
        {
            var match = _mm.GetMatchByPlayerId(_identityService.User.Id);
            match?.HandleEvent(new PlayerLeftEventArgs { PlayerId = _identityService.User.Id });
        }
        // Handle user closing browser / refreshing page same way as them explicitly abandoning the match.
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            LeaveMatch();
            return base.OnDisconnectedAsync(exception);
        }
    }
}
