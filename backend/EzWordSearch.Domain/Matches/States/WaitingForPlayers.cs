using EzWordSearch.Domain.Matches.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.States
{
    internal class WaitingForPlayers : IMatchState
    {
        public void Enter(Match match)
        {
            match.WaitingForPlayers(new WaitingForPlayersEventArgs());
        }

        public void Exit(Match match)
        {
        }

        public IMatchState? HandleEvent(Match match, WordSearchEventArgs e)
        {
            if(e is PlayerJoinedEventArgs)
            {
                var playerJoinedEventArgs = e as PlayerJoinedEventArgs ?? throw new Exception($"Could not cast to {nameof(PlayerJoinedEventArgs)}.");
                match.AddPlayer(playerJoinedEventArgs);
                if (match.IsFull)
                {
                    return new WaitingToStart();
                }
                return null;
            }
            else if(e is PlayerLeftEventArgs)
            {
                var playerLeftEventArgs = e as PlayerLeftEventArgs ?? throw new Exception($"Could not cast to {nameof(PlayerLeftEventArgs)}");
                match.RemovePlayer(playerLeftEventArgs);
                return null;
            }
            else
            {
                return null;
            }
        }

        public MatchStateType Type => MatchStateType.WaitingForPlayers;
    }
}
