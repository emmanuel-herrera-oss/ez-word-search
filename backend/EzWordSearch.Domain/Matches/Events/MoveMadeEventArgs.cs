using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.Events
{
    public class MoveMadeEventArgs : WordSearchEventArgs
    {
        public override WordSearchEventType Type => WordSearchEventType.MoveMade;
        public Guid PlayerId { get; set; }
        public string? Word { get; set; }
        public required int[] Start { get; set; }
        public required int[] End { get; set; }
    }
}
