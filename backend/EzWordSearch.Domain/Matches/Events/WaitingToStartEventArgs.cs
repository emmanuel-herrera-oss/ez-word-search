using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.Events
{
    public class WaitingToStartEventArgs : WordSearchEventArgs
    {
        public override WordSearchEventType Type => WordSearchEventType.WaitingToStart;
        public int SecondsToWait { get; set; }
    }
}
