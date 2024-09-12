using EzWordSearch.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.MatchManagement
{
    internal class MatchManagerReservation
    {
        private DateTime ExpirationTime;
        internal Match Match { get; private set; }
        internal MatchManagerReservation(int durationSeconds, Match match)
        {
            ExpirationTime = DateTime.UtcNow.AddSeconds(durationSeconds);
            Match = match;
        }
        private bool Expired => DateTime.UtcNow >= ExpirationTime;
        internal bool IsValid(int numberOfPlayers)
        {
            return
                Match.AcceptingPlayers &&
                !Expired &&
                Match.Settings.NumberOfPlayers == numberOfPlayers;
        }
    }
}
