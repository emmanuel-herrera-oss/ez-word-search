using EzWordSearch.Domain.Matches.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches
{
    public class MatchSnapshot
    {
        public MatchStateType State { get; internal set; }
        public List<MatchPlayer> Players { get; internal set; } = null!;
        public double StartTimer { get; internal set; }
        internal MatchSnapshot() { }
    }
}
