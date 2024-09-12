using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Boards
{
    public interface IBoardFactory
    {
        public Task<Board> CreateBoardAsync();
    }
}
