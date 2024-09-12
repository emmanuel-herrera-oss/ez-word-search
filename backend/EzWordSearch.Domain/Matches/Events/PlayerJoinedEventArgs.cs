using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EzWordSearch.Domain.Players;

namespace EzWordSearch.Domain.Matches.Events
{
    public class PlayerJoinedEventArgs : WordSearchEventArgs
    {
        public override WordSearchEventType Type => WordSearchEventType.PlayedJoined;
        public required MatchPlayer Player { get; set; }
    }
}
