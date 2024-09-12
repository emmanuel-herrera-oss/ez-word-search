using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.Events
{
    public class MatchStartedEventArgs : WordSearchEventArgs
    {
        public override WordSearchEventType Type => WordSearchEventType.MatchStarted;
        public required char[][] Board { get; set; }
        public required List<string> Words { get; set; }
        public int TimeLimitSeconds { get; set; }
    }
}
