using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.Events
{
    internal class MatchTimerExpiredEventArgs : WordSearchEventArgs
    {
        public override WordSearchEventType Type => WordSearchEventType.MatchTimerExpired;
    }
}
