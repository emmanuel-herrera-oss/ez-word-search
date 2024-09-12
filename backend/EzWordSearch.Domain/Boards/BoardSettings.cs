using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Boards
{
    public class BoardSettings
    {
        public int Width { get; set; } = 14;
        public int Height { get; set; } = 14;
        public int MinWordLength { get; set; } = 4;
        public int MaxWordLength { get; set; } = 8;
        public int NumberOfWords { get; set; } = 30;
        public required IReadOnlyList<string> Dictionary { get; set; }
    }
}
