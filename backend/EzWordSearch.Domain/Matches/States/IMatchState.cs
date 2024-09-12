using EzWordSearch.Domain.Matches.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.States
{
    internal interface IMatchState
    {
        void Enter(Match match);
        void Exit(Match match);
        IMatchState? HandleEvent(Match match, WordSearchEventArgs e);
        MatchStateType Type { get; }
    }

    public enum MatchStateType
    {
        WaitingForPlayers,
        WaitingToStart,
        InProgress,
        Ended
    }
}
