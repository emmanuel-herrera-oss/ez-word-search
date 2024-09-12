using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.Events
{
    public class WordSearchErrorEventArgs : WordSearchEventArgs
    {
        public override WordSearchEventType Type => WordSearchEventType.Error;
        public WordSearchErrorType ErrorType { get; set; }
    }
    public enum WordSearchErrorType
    {
        PlayerAlreadyInAMatch,
        MatchIsNotAcceptingPlayers
    }
}
