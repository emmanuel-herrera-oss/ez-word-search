using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches
{
    public interface IMatchFactory
    {
        public Match CreateMatch(int numberOfPlayers);
    }
}
