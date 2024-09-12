using EzWordSearch.Domain.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.Events
{
    public class PlayerLeftEventArgs : WordSearchEventArgs
    {
        public override WordSearchEventType Type => WordSearchEventType.PlayerLeft;
        public Guid PlayerId { get; set; }
    }
}
