using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.Events
{
    public class MatchEndedEventArgs : WordSearchEventArgs
    {
        public override WordSearchEventType Type => WordSearchEventType.MatchEnded;
        public Guid? WinnerId { get; set; }
        public Guid? LeaverId { get; set; }
        public bool SinglePlayer { get; set; }
    }
}
