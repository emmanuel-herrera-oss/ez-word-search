using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches
{
    public class MatchSettings
    {
        public int NumberOfPlayers { get; init; } = 2;
        public int TimeLimitSeconds { get; init; } = 300;
        public int SecondsToWaitBeforeMatchStart { get; init; } = 10;
    }
}
