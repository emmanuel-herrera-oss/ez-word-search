using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EzWordSearch.Domain.Matches;
using EzWordSearch.Domain.Matches.Events;
using EzWordSearch.Domain.Players;

namespace EzWordSearch.Domain.MatchManagement
{
    public class MatchManager
    {
        private readonly object _lock = new();
        private readonly IMatchFactory _matchFactory;
        private readonly List<Match> _matches = [];
        public MatchManager(IMatchFactory matchFactory)
        {
            _matchFactory = matchFactory;
        }
        public FindMatchResult FindAvailableMatch(Player player, int playerCount)
        {
            lock(_lock)
            {
                foreach(var match in _matches)
                {
                    if (match.Ended) continue;
                    foreach(var matchPlayer in match.Players)
                    {
                        if(matchPlayer.Player.PlayerId == player.PlayerId)
                        {
                            return FindMatchResult.Failure(FindMatchResultType.PlayerAlreadyInAMatch);
                        }
                    }
                }

                foreach (var existingMatch in _matches)
                {
                    if (existingMatch.CanAddPlayer(player) == null && existingMatch.Settings.NumberOfPlayers == playerCount)
                    {
                        existingMatch.HandleEvent(new PlayerJoinedEventArgs { Player = MatchPlayer.Create(existingMatch, player) });
                        return FindMatchResult.Success();
                    }
                }

                var newMatch = _matchFactory.CreateMatch(playerCount);
                newMatch.HandleEvent(new PlayerJoinedEventArgs { Player = MatchPlayer.Create(newMatch, player) });
                _matches.Add(newMatch);
                return FindMatchResult.Success();
            }
        }

        public Match? GetMatchByPlayerId(Guid playerId)
        {
            return _matches.FirstOrDefault(m => m.Players.Any(p =>
                p.Player.PlayerId == playerId &&
                !m.Ended
            ));
        }

        public int GetPlayerCount()
        {
            return _matches.Where(m => m.InProgress).Sum(i => i.Players.Count);
        }
    }
    public class FindMatchResult
    {
        public FindMatchResultType Type { get; set; }
        private FindMatchResult() { }
        public static FindMatchResult Success()
        {
            return new FindMatchResult { Type = FindMatchResultType.Success };
        }
        public static FindMatchResult Failure(FindMatchResultType type)
        {
            return new FindMatchResult { Type = type };
        }
    }
    public enum FindMatchResultType
    {
        Success,
        PlayerAlreadyInAMatch
    }
}
