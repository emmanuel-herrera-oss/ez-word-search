using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.Events
{
    public abstract class WordSearchEventArgs : EventArgs
    {
        public Guid CorrelationId { get; set; }
        public abstract WordSearchEventType Type { get; }
        public DateTime Time { get; private set; }
        public WordSearchEventArgs()
        {
            Time = DateTime.UtcNow;
        }
    }
    public enum WordSearchEventType
    {
        WaitingToStart,
        PlayedJoined,
        MatchStarted,
        MoveMade,
        MatchEnded,
        PlayerLeft,
        Error,
        MatchStartTimerExpired,
        MatchTimerExpired,
        WaitingForPlayers
    }
}
