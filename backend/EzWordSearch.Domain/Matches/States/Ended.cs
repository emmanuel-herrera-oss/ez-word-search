using EzWordSearch.Domain.Matches.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.States
{
    internal class Ended : IMatchState
    {
        public void Enter(Match match)
        {
            match.EndMatch();
        }

        public void Exit(Match match)
        {
        }

        public IMatchState? HandleEvent(Match match, WordSearchEventArgs e)
        {
            return null;
        }

        public MatchStateType Type => MatchStateType.Ended;
    }
}
